using AgriFieldHub.DTOs;
using AgriFieldHub.Models;
using AgriFieldHub.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriFieldHub.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = nameof(UserRole.Admin))]
public class UsersController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    public UsersController(IUnitOfWork uow) => _uow = uow;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll(CancellationToken ct)
    {
        var users = await _uow.Users.GetAllAsync(ct);
        return Ok(users.Select(u => new UserDto
        {
            Id = u.Id,
            Email = u.Email,
            Role = u.Role,
            CreatedAt = u.CreatedAt
        }));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> Get(int id, CancellationToken ct)
    {
        var user = await _uow.Users.GetByIdAsync(id, ct);
        if (user == null) return NotFound();
        return Ok(new UserDto { Id = user.Id, Email = user.Email, Role = user.Role, CreatedAt = user.CreatedAt });
    }

    [HttpPatch("{id:int}/role")]
    public async Task<IActionResult> UpdateRole(int id, UpdateUserRoleRequest request, CancellationToken ct)
    {
        var user = await _uow.Users.GetByIdAsync(id, ct);
        if (user == null) return NotFound();
        user.Role = request.Role;
        await _uow.Users.UpdateAsync(user);
        await _uow.SaveChangesAsync();
        return NoContent();
    }
}
