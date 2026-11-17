using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TeamNest.Models.Entities;

namespace TeamNest.Data;

/// <summary>
/// EF Core context. We use it for two things only:
///   1. Identity stores users, roles and logins through it.
///   2. Migrations create every table (Identity + Organizations + Projects).
/// All other reading and writing is done with Dapper in the Repositories folder.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Organization> Organizations { get; set; } = null!;
    public DbSet<Project> Projects { get; set; } = null!;
}
