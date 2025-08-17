using System.ComponentModel.DataAnnotations;
using AgriFieldHub.Models;

namespace AgriFieldHub.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UpdateUserRoleRequest
{
    [Required]
    public UserRole Role { get; set; }
}
