using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamNest.Models.ViewModels;
using TeamNest.Repositories;
using TeamNest.Services;

namespace TeamNest.Controllers;

[Authorize] // only signed-in users
public class DashboardController : Controller
{
    private readonly IOrganizationRepository _organizations;
    private readonly IProjectRepository _projects;
    private readonly ICurrentUserService _currentUser;

    public DashboardController(
        IOrganizationRepository organizations,
        IProjectRepository projects,
        ICurrentUserService currentUser)
    {
        _organizations = organizations;
        _projects = projects;
        _currentUser = currentUser;
    }

    public async Task<IActionResult> Index()
    {
        var organizationId = _currentUser.OrganizationId;
        var organization = await _organizations.GetByIdAsync(organizationId);

        if (organization is null)
        {
            return NotFound();
        }

        var model = new DashboardViewModel
        {
            FullName = _currentUser.FullName,
            OrganizationName = organization.Name,
            InviteCode = organization.InviteCode,
            Role = _currentUser.Role,
            Stats = await _organizations.GetStatsAsync(organizationId),
            RecentProjects = await _projects.GetRecentAsync(5)
        };

        return View(model);
    }
}
