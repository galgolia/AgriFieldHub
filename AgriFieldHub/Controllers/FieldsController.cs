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

    private int? CurrentUserId => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
    private bool IsAdmin => User.IsInRole(UserRole.Admin.ToString());

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FieldDto>>> GetAll(CancellationToken ct)
    {
        var userId = CurrentUserId;
        var fields = IsAdmin
            ? await _uow.Fields.GetAllAsync(ct)
            : await _uow.Fields.FindAsync(f => f.UserId == userId, ct);
        return Ok(fields.Select(ToDto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<FieldDto>> Get(int id, CancellationToken ct)
    {
        var field = await _uow.Fields.GetByIdAsync(id, ct);
        if (field == null) return NotFound();
        if (!IsAdmin && field.UserId != CurrentUserId) return Forbid();
        return Ok(ToDto(field));
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
        return CreatedAtAction(nameof(Get), new { id = field.Id }, ToDto(field));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateFieldRequest request, CancellationToken ct)
    {
        var field = await _uow.Fields.GetByIdAsync(id, ct);
        if (field == null) return NotFound();
        if (!IsAdmin && field.UserId != CurrentUserId) return Forbid();
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
        if (!IsAdmin && field.UserId != CurrentUserId) return Forbid();
        await _uow.Fields.DeleteAsync(field);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    private static FieldDto ToDto(Field f) => new()
    {
        Id = f.Id,
        Name = f.Name,
        Description = f.Description,
        Location = f.Location,
        UserId = f.UserId,
        CreatedAt = f.CreatedAt
    };
}
