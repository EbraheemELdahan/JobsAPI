using System.Text.Json;
using System.Text.Json.Nodes;

namespace JobsAPI.Repositories;

public class JsonFileStore
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true, WriteIndented = true };
    private readonly SemaphoreSlim _lock = new(1,1);

    public JsonFileStore(string filePath)
    {
        _filePath = filePath;
    }

    public async Task<List<T>> ReadArrayAsync<T>(string arrayName)
    {
        await _lock.WaitAsync();
        try
        {
            if (!File.Exists(_filePath))
                return new List<T>();

            using var stream = File.OpenRead(_filePath);
            var root = await JsonNode.ParseAsync(stream);
            var node = root?[arrayName];
            if (node == null) return new List<T>();
            return node.Deserialize<List<T>>(_jsonOptions) ?? new List<T>();
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task WriteArrayAsync<T>(string arrayName, List<T> items)
    {
        await _lock.WaitAsync();
        try
        {
            JsonNode? root;
            if (File.Exists(_filePath))
            {
                using var stream = File.OpenRead(_filePath);
                root = await JsonNode.ParseAsync(stream) ?? new JsonObject();
            }
            else
            {
                root = new JsonObject();
            }

            root[arrayName] = JsonSerializer.SerializeToNode(items, _jsonOptions);
            // overwrite
            var temp = Path.GetTempFileName();
            await File.WriteAllTextAsync(temp, root.ToJsonString(_jsonOptions));
            File.Copy(temp, _filePath, overwrite: true);
            File.Delete(temp);
        }
        finally
        {
            _lock.Release();
        }
    }
}