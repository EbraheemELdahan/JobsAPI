using JobsAPI.Models;
using JobsAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace JobsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly IJobService _svc;
    public JobsController(IJobService svc) => _svc = svc;

    [HttpGet]
    public Task<IEnumerable<Job>> Get() => _svc.GetAllAsync();

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var j = await _svc.GetAsync(id);
        return j == null ? NotFound() : Ok(j);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Job j)
    {
        var created = await _svc.CreateAsync(j);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] Job j)
    {
        if (id != j.Id) return BadRequest();
        var updated = await _svc.UpdateAsync(j);
        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public Task<IActionResult> Delete(int id) => _svc.DeleteAsync(id)
        .ContinueWith(t => t.Result ? (IActionResult)NoContent() : NotFound());
}