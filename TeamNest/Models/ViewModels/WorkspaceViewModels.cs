using TeamNest.Models.Dtos;

namespace TeamNest.Models.ViewModels;

public class DashboardViewModel
{
    public string FullName { get; set; } = string.Empty;
    public string OrganizationName { get; set; } = string.Empty;
    public string InviteCode { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public WorkspaceStats Stats { get; set; } = new();
    public IEnumerable<ProjectListItem> RecentProjects { get; set; } = Enumerable.Empty<ProjectListItem>();
}

public class TeamViewModel
{
    public string OrganizationName { get; set; } = string.Empty;
    public string InviteCode { get; set; } = string.Empty;
    public IEnumerable<TeamMemberDto> Members { get; set; } = Enumerable.Empty<TeamMemberDto>();
}

/// <summary>Model for the sidebar ViewComponent.</summary>
public class WorkspaceSummaryViewModel
{
    public string OrganizationName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;

    public string Initials =>
        string.Concat(FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                              .Take(2)
                              .Select(part => char.ToUpperInvariant(part[0])));
}
