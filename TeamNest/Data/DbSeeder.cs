using Microsoft.AspNetCore.Identity;
using TeamNest.Models;

namespace TeamNest.Data;

public static class DbSeeder
{
    /// <summary>Creates the Owner, Admin and Member roles on first run.</summary>
    public static async Task SeedRolesAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DbSeeder");

        try
        {
            foreach (var role in AppRoles.All)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Could not create roles. Make sure you ran 'Update-Database' in Package Manager Console.");
        }
    }
}
