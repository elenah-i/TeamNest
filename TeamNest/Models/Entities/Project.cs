using System.ComponentModel.DataAnnotations;

namespace TeamNest.Models.Entities;

public class Project
{
    public int Id { get; set; }

    // Tenant column: every query must filter by this
    public int OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public ProjectStatus Status { get; set; } = ProjectStatus.Planning;

    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }

    [Required, MaxLength(450)]
    public string CreatedByUserId { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Soft delete: rows are hidden, never physically removed
    public bool IsDeleted { get; set; }
}
