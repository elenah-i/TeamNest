using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TeamNest.Models.Entities;

/// <summary>A company or team using TeamNest. Each organization is one "tenant".</summary>
[Index(nameof(InviteCode), IsUnique = true)]
public class Organization
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    // Teammates type this code on the sign-up page to join
    [Required, MaxLength(12)]
    public string InviteCode { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Project> Projects { get; set; } = new List<Project>();
}
