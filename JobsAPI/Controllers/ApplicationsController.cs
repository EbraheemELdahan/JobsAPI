using JobsAPI.Models;
using JobsAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace JobsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationService _svc;
    public ApplicationsController(IApplicationService svc) => _svc = svc;

    [HttpGet]
    [Authorize(Roles = "Company")] // companies can view all applications
    public Task<IEnumerable<Application>> Get() => _svc.GetAllAsync();

    [HttpGet("{id:int}")]
    [Authorize] // any authenticated user can view a specific application (tweak as needed)
    public async Task<IActionResult> Get(int id)
    {
        var a = await _svc.GetAsync(id);
        return a == null ? NotFound() : Ok(a);
    }

    // Only jobSeekers can create applications
    [HttpPost]
    [Authorize(Roles = "jobSeeker")]
    public async Task<IActionResult> Post([FromBody] Application a)
    {
        var created = await _svc.CreateAsync(a);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    // Allow Company to update application status
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Company")]
    public async Task<IActionResult> Put(int id, [FromBody] Application a)
    {
        if (id != a.Id) return BadRequest();
        var updated = await _svc.UpdateAsync(a);
        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Company")]
    public Task<IActionResult> Delete(int id) => _svc.DeleteAsync(id)
        .ContinueWith(t => t.Result ? (IActionResult)NoContent() : NotFound());
}