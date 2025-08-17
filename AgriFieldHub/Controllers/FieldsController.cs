using AgriFieldHub.DTOs;
using AgriFieldHub.Models;
using AgriFieldHub.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AgriFieldHub.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FieldsController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    public FieldsController(IUnitOfWork uow) => _uow = uow;

    private int? CurrentUserId => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FieldDto>>> GetAll(CancellationToken ct)
    {
        var fields = await _uow.Fields.GetAllAsync(ct);
        return Ok(fields.Select(f => new FieldDto
        {
            Id = f.Id,
            Name = f.Name,
            Description = f.Description,
            Location = f.Location,
            UserId = f.UserId,
            CreatedAt = f.CreatedAt
        }));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<FieldDto>> Get(int id, CancellationToken ct)
    {
        var field = await _uow.Fields.GetByIdAsync(id, ct);
        if (field == null) return NotFound();
        return Ok(new FieldDto
        {
            Id = field.Id,
            Name = field.Name,
            Description = field.Description,
            Location = field.Location,
            UserId = field.UserId,
            CreatedAt = field.CreatedAt
        });
    }

    [HttpPost]
    public async Task<ActionResult<FieldDto>> Create(CreateFieldRequest request, CancellationToken ct)
    {
        var userId = CurrentUserId;
        if (userId == null) return Forbid();

        var field = new Field
        {
            Name = request.Name,
            Description = request.Description ?? string.Empty,
            Location = request.Location ?? string.Empty,
            UserId = userId.Value
        };
        await _uow.Fields.AddAsync(field, ct);
        await _uow.SaveChangesAsync();
        var dto = new FieldDto { Id = field.Id, Name = field.Name, Description = field.Description, Location = field.Location, UserId = field.UserId, CreatedAt = field.CreatedAt };
        return CreatedAtAction(nameof(Get), new { id = field.Id }, dto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateFieldRequest request, CancellationToken ct)
    {
        var field = await _uow.Fields.GetByIdAsync(id, ct);
        if (field == null) return NotFound();
        field.Name = request.Name;
        field.Description = request.Description ?? string.Empty;
        field.Location = request.Location ?? string.Empty;
        await _uow.Fields.UpdateAsync(field);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var field = await _uow.Fields.GetByIdAsync(id, ct);
        if (field == null) return NotFound();
        await _uow.Fields.DeleteAsync(field);
        await _uow.SaveChangesAsync();
        return NoContent();
    }
}
