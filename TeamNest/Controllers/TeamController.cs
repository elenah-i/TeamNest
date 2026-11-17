using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamNest.Models;
using TeamNest.Models.ViewModels;
using TeamNest.Repositories;
using TeamNest.Services;

namespace TeamNest.Controllers;

[Authorize]
public class TeamController : Controller
{
    private readonly IOrganizationRepository _organizations;
    private readonly ICurrentUserService _currentUser;

    public TeamController(IOrganizationRepository organizations, ICurrentUserService currentUser)
    {
        _organizations = organizations;
        _currentUser = currentUser;
    }

    // GET: /Team
    public async Task<IActionResult> Index()
    {
        var organization = await _organizations.GetByIdAsync(_currentUser.OrganizationId);
        if (organization is null)
        {
            return NotFound();
        }

        var model = new TeamViewModel
        {
            OrganizationName = organization.Name,
            InviteCode = organization.InviteCode,
            Members = await _organizations.GetMembersAsync(organization.Id)
        };

        return View(model);
    }

    // POST: /Team/RegenerateInviteCode   (AJAX, Owner only)
    [HttpPost]
    [Authorize(Policy = AppPolicies.OwnerOnly)]
    public async Task<IActionResult> RegenerateInviteCode()
    {
        var newCode = await _organizations.GenerateUniqueInviteCodeAsync();
        await _organizations.UpdateInviteCodeAsync(_currentUser.OrganizationId, newCode);

        return Json(new { success = true, code = newCode, message = "New invite code created. The old one no longer works." });
    }
}
