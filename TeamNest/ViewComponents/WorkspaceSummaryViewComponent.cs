using Microsoft.AspNetCore.Mvc;
using TeamNest.Models.ViewModels;
using TeamNest.Repositories;
using TeamNest.Services;

namespace TeamNest.ViewComponents;

/// <summary>
/// A ViewComponent is like a mini controller + view for a piece of a page.
/// This one renders the workspace name and user card in the sidebar on every app page.
/// Used in _AppLayout.cshtml with: &lt;vc:workspace-summary /&gt;
/// </summary>
public class WorkspaceSummaryViewComponent : ViewComponent
{
    private readonly IOrganizationRepository _organizations;
    private readonly ICurrentUserService _currentUser;

    public WorkspaceSummaryViewComponent(IOrganizationRepository organizations, ICurrentUserService currentUser)
    {
        _organizations = organizations;
        _currentUser = currentUser;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var organization = await _organizations.GetByIdAsync(_currentUser.OrganizationId);

        var model = new WorkspaceSummaryViewModel
        {
            OrganizationName = organization?.Name ?? "Workspace",
            FullName = _currentUser.FullName,
            Role = _currentUser.Role
        };

        return View(model);
    }
}
