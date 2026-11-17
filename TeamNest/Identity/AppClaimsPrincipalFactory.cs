using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using TeamNest.Models;
using TeamNest.Models.Entities;

namespace TeamNest.Identity;

/// <summary>
/// Identity customization: when a user logs in, Identity builds a ClaimsPrincipal
/// that is stored in the login cookie. We add our own claims to it so every request
/// knows the user's organization without another database call.
/// </summary>
public class AppClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
{
    public AppClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<IdentityOptions> options)
        : base(userManager, roleManager, options)
    {
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        // The base class already adds user id, name, email and roles
        var identity = await base.GenerateClaimsAsync(user);

        identity.AddClaim(new Claim(AppClaimTypes.FullName, user.FullName));

        if (user.OrganizationId.HasValue)
        {
            identity.AddClaim(new Claim(AppClaimTypes.OrganizationId, user.OrganizationId.Value.ToString()));
        }

        return identity;
    }
}
