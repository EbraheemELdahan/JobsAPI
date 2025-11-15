using JobsAPI.Models;
using JobsAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace JobsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _svc;
    public UsersController(IUserService svc) => _svc = svc;

    [HttpGet]
    public Task<IEnumerable<UserDto>> Get() => _svc.GetAllAsync();

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var u = await _svc.GetAsync(id);
        return u == null ? NotFound() : Ok(u);
    }

    // Keep create/update returning the created/updated entity (not DTO) to avoid broader changes.
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] User u)
    {
        var created = await _svc.CreateAsync(u);
        // Map created to DTO for response
        var dto = new UserDto
        {
            Id = created.Id,
            UserName = created.UserName,
            Email = created.Email,
            Role = created.Role,
            Phone = created.Phone,
            CompanyId = created.CompanyId,
            Company = created.Company == null ? null : new CompanyLiteDto { Id = created.Company.Id, Name = created.Company.Name, Logo = created.Company.Logo }
        };
        return CreatedAtAction(nameof(Get), new { id = created.Id }, dto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] User u)
    {
        if (id != u.Id) return BadRequest();
        var updated = await _svc.UpdateAsync(u);
        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public Task<IActionResult> Delete(int id) => _svc.DeleteAsync(id)
        .ContinueWith(t => t.Result ? (IActionResult)NoContent() : NotFound());
}