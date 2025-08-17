using AgriFieldHub.DTOs;
using AgriFieldHub.Models;
using AgriFieldHub.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Controller = AgriFieldHub.Models.Controller;

namespace AgriFieldHub.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ControllersController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    public ControllersController(IUnitOfWork uow) => _uow = uow;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeviceControllerDto>>> GetAll(CancellationToken ct)
    {
        var list = await _uow.Controllers.GetAllAsync(ct);
        return Ok(list.Select(c => new DeviceControllerDto
        {
            Id = c.Id,
            Type = c.Type,
            Description = c.Description,
            Status = c.Status,
            FieldId = c.FieldId,
            CreatedAt = c.CreatedAt
        }));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DeviceControllerDto>> Get(int id, CancellationToken ct)
    {
        var controller = await _uow.Controllers.GetByIdAsync(id, ct);
        if (controller == null) return NotFound();
        return Ok(new DeviceControllerDto
        {
            Id = controller.Id,
            Type = controller.Type,
            Description = controller.Description,
            Status = controller.Status,
            FieldId = controller.FieldId,
            CreatedAt = controller.CreatedAt
        });
    }

    [HttpPost]
    public async Task<ActionResult<DeviceControllerDto>> Create(CreateDeviceControllerRequest request, CancellationToken ct)
    {
        var entity = new Controller
        {
            Type = request.Type,
            Description = request.Description ?? string.Empty,
            Status = request.Status ?? string.Empty,
            FieldId = 0 // TODO: associate with field via request extension later
        };
        await _uow.Controllers.AddAsync(entity, ct);
        await _uow.SaveChangesAsync();
        var dto = new DeviceControllerDto
        {
            Id = entity.Id,
            Type = entity.Type,
            Description = entity.Description,
            Status = entity.Status,
            FieldId = entity.FieldId,
            CreatedAt = entity.CreatedAt
        };
        return CreatedAtAction(nameof(Get), new { id = entity.Id }, dto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateDeviceControllerRequest request, CancellationToken ct)
    {
        var entity = await _uow.Controllers.GetByIdAsync(id, ct);
        if (entity == null) return NotFound();
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
        await _uow.Controllers.DeleteAsync(entity);
        await _uow.SaveChangesAsync();
        return NoContent();
    }
}
