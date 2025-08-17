using System.ComponentModel.DataAnnotations;

namespace AgriFieldHub.DTOs;

public class DeviceControllerDto
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Status { get; set; }
    public int FieldId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateDeviceControllerRequest
{
    [Required, StringLength(50)]
    public string Type { get; set; } = string.Empty;

    [StringLength(255)]
    public string? Description { get; set; }

    public string? Status { get; set; }

    [Required]
    public int FieldId { get; set; }
}

public class UpdateDeviceControllerRequest : CreateDeviceControllerRequest { }
