using System.Security.Claims;
using TeamNest.Models;

namespace TeamNest.Services;

/// <summary>Reads the signed-in user's details from the login cookie claims.</summary>
public interface ICurrentUserService
{
    string UserId { get; }
    string FullName { get; }

    /// <summary>The tenant id. Repositories use it to filter every query.</summary>
    int OrganizationId { get; }

    string Role { get; }
    bool IsInRole(string role);
}

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal User =>
        _httpContextAccessor.HttpContext?.User
        ?? throw new InvalidOperationException("There is no active HTTP request.");

    public string UserId =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new InvalidOperationException("No user is signed in.");

    public string FullName =>
        User.FindFirst(AppClaimTypes.FullName)?.Value ?? User.Identity?.Name ?? string.Empty;

    public int OrganizationId
    {
        get
        {
            var value = User.FindFirst(AppClaimTypes.OrganizationId)?.Value;
            if (int.TryParse(value, out var organizationId))
            {
                return organizationId;
            }

            throw new InvalidOperationException("The signed-in user does not belong to an organization.");
        }
    }

    public string Role =>
        AppRoles.All.FirstOrDefault(role => User.IsInRole(role)) ?? AppRoles.Member;

    public bool IsInRole(string role) => User.IsInRole(role);
}
