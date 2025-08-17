using System.ComponentModel.DataAnnotations;

namespace AgriFieldHub.DTOs;

public class FieldDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Location { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateFieldRequest
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(255)]
    public string? Description { get; set; }

    [StringLength(100)]
    public string? Location { get; set; }
}

public class UpdateFieldRequest : CreateFieldRequest { }
