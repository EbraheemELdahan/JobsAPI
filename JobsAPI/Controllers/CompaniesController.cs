using JobsAPI.Models;
using JobsAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace JobsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompaniesController : ControllerBase
{
    private readonly ICompanyService _svc;
    public CompaniesController(ICompanyService svc) => _svc = svc;

    [HttpGet]
    [AllowAnonymous]
    public Task<IEnumerable<Company>> Get() => _svc.GetAllAsync();

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> Get(int id)
    {
        var c = await _svc.GetAsync(id);
        return c == null ? NotFound() : Ok(c);
    }

    // Only users with role "Company" can create companies
    [HttpPost]
    [Authorize(Roles = "Company")]
    public async Task<IActionResult> Post([FromBody] Company c)
    {
        var created = await _svc.CreateAsync(c);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    // Only users with role "Company" can update
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Company")]
    public async Task<IActionResult> Put(int id, [FromBody] Company c)
    {
        if (id != c.Id) return BadRequest();
        var updated = await _svc.UpdateAsync(c);
        return updated == null ? NotFound() : Ok(updated);
    }

    // Only users with role "Company" can delete
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Company")]
    public Task<IActionResult> Delete(int id) => _svc.DeleteAsync(id)
        .ContinueWith(t => t.Result ? (IActionResult)NoContent() : NotFound());
}