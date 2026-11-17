namespace TeamNest.Models;

/// <summary>Role names used with Identity. Keep them in one place to avoid typos.</summary>
public static class AppRoles
{
    public const string Owner = "Owner";
    public const string Admin = "Admin";
    public const string Member = "Member";

    public static readonly string[] All = { Owner, Admin, Member };
}

/// <summary>Authorization policy names registered in Program.cs.</summary>
public static class AppPolicies
{
    public const string CanManageProjects = "CanManageProjects";
    public const string OwnerOnly = "OwnerOnly";
}

/// <summary>Custom claim types we add to the login cookie.</summary>
public static class AppClaimTypes
{
    public const string OrganizationId = "teamnest:organization_id";
    public const string FullName = "teamnest:full_name";
}
