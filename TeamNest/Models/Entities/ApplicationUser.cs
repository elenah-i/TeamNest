using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TeamNest.Models.Entities;

/// <summary>
/// Identity customization: we extend the built-in IdentityUser with our own columns.
/// These become extra columns in the AspNetUsers table.
/// </summary>
public class ApplicationUser : IdentityUser
{
    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    // The organization (tenant) this user belongs to
    public int? OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
