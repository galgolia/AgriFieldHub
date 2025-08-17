using AgriFieldHub.DTOs;
using AgriFieldHub.Models;
using AgriFieldHub.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Controller = AgriFieldHub.Models.Controller;

namespace AgriFieldHub.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ControllersController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    public ControllersController(IUnitOfWork uow) => _uow = uow;

    private int? CurrentUserId => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
    private bool IsAdmin => User.IsInRole(UserRole.Admin.ToString());

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeviceControllerDto>>> GetAll(CancellationToken ct)
    {
        var userId = CurrentUserId;
        var list = IsAdmin
            ? await _uow.Controllers.GetAllAsync(ct)
            : await _uow.Controllers.FindAsync(c => c.Field.UserId == userId, ct);
        return Ok(list.Select(ToDto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DeviceControllerDto>> Get(int id, CancellationToken ct)
    {
        var entity = await _uow.Controllers.GetByIdAsync(id, ct);
        if (entity == null) return NotFound();
        if (!IsAdmin && entity.Field.UserId != CurrentUserId) return Forbid();
        return Ok(ToDto(entity));
    }

    [HttpPost]
    public async Task<ActionResult<DeviceControllerDto>> Create(CreateDeviceControllerRequest request, CancellationToken ct)
    {
        var userId = CurrentUserId;
        if (userId == null) return Forbid();

        // Validate field exists and ownership
        var field = await _uow.Fields.GetByIdAsync(request.FieldId, ct);
        if (field == null) return BadRequest("Field does not exist");
        if (!IsAdmin && field.UserId != userId) return Forbid();

        var entity = new Controller
        {
            Type = request.Type,
            Description = request.Description ?? string.Empty,
            Status = request.Status ?? string.Empty,
            FieldId = field.Id
        };
        await _uow.Controllers.AddAsync(entity, ct);
        await _uow.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = entity.Id }, ToDto(entity));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateDeviceControllerRequest request, CancellationToken ct)
    {
        var entity = await _uow.Controllers.GetByIdAsync(id, ct);
        if (entity == null) return NotFound();

        // Ownership check via existing entity field
        var field = await _uow.Fields.GetByIdAsync(entity.FieldId, ct);
        if (field == null) return BadRequest("Underlying field missing");
        if (!IsAdmin && field.UserId != CurrentUserId) return Forbid();

        // Optionally allow moving to another field if provided
        if (request.FieldId != entity.FieldId)
        {
            var targetField = await _uow.Fields.GetByIdAsync(request.FieldId, ct);
            if (targetField == null) return BadRequest("Target field does not exist");
            if (!IsAdmin && targetField.UserId != CurrentUserId) return Forbid();
            entity.FieldId = request.FieldId;
        }

        entity.Type = request.Type;
        entity.Description = request.Description ?? string.Empty;
        entity.Status = request.Status ?? string.Empty;
        await _uow.Controllers.UpdateAsync(entity);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var entity = await _uow.Controllers.GetByIdAsync(id, ct);
        if (entity == null) return NotFound();
        var field = await _uow.Fields.GetByIdAsync(entity.FieldId, ct);
        if (field == null) return BadRequest("Underlying field missing");
        if (!IsAdmin && field.UserId != CurrentUserId) return Forbid();
        await _uow.Controllers.DeleteAsync(entity);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    private static DeviceControllerDto ToDto(Controller c) => new()
    {
        Id = c.Id,
        Type = c.Type,
        Description = c.Description,
        Status = c.Status,
        FieldId = c.FieldId,
        CreatedAt = c.CreatedAt
    };
}
