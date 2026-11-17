using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TeamNest.Helpers;
using TeamNest.Models;
using TeamNest.Models.Entities;
using TeamNest.Models.ViewModels;
using TeamNest.Repositories;

namespace TeamNest.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IOrganizationRepository _organizations;
    private readonly ILogger<AccountController> _logger;

    // Dependency Injection: ASP.NET Core passes these in automatically
    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IOrganizationRepository organizations,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _organizations = organizations;
        _logger = logger;
    }

    // ---------------- Login ----------------

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Dashboard");
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // We use the email as the Identity user name
        var result = await _signInManager.PasswordSignInAsync(
            model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            _logger.LogInformation("User {Email} logged in.", model.Email);

            // Only redirect to pages on our own site (prevents "open redirect" attacks)
            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return LocalRedirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Dashboard");
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "Too many failed attempts. Try again in 10 minutes.");
            return View(model);
        }

        ModelState.AddModelError(string.Empty, "Email or password is incorrect.");
        return View(model);
    }

    // ---------------- Register ----------------

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Dashboard");
        }

        return View(new RegisterViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        int organizationId;
        string role;
        var createdNewOrganization = false;

        if (model.Choice == WorkspaceChoice.Join)
        {
            var organization = await _organizations.GetByInviteCodeAsync(model.InviteCode!);
            if (organization is null)
            {
                ModelState.AddModelError(nameof(model.InviteCode), "No organization uses this code. Check it and try again.");
                return View(model);
            }

            organizationId = organization.Id;
            role = AppRoles.Member;
        }
        else
        {
            var inviteCode = await _organizations.GenerateUniqueInviteCodeAsync();
            organizationId = await _organizations.CreateAsync(model.OrganizationName!.Trim(), inviteCode);
            role = AppRoles.Owner;
            createdNewOrganization = true;
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = model.FullName.Trim(),
            OrganizationId = organizationId,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            // Identity couldn't create the user (e.g. email already taken).
            // Remove the organization we just made so we don't leave an empty one behind.
            if (createdNewOrganization)
            {
                await _organizations.DeleteAsync(organizationId);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        await _userManager.AddToRoleAsync(user, role);
        await _signInManager.SignInAsync(user, isPersistent: false);

        _logger.LogInformation("New user {Email} registered as {Role}.", model.Email, role);

        TempData["Toast"] = createdNewOrganization
            ? $"Welcome to TeamNest! Your workspace \"{model.OrganizationName}\" is ready."
            : "Welcome to TeamNest! You've joined your team's workspace.";

        return RedirectToAction("Index", "Dashboard");
    }

    // Remote validation endpoint, called by AJAX from the register form.
    // Returns true when valid, or an error message string when not.
    [AcceptVerbs("GET", "POST")]
    public async Task<IActionResult> CheckInviteCode(string? inviteCode)
    {
        if (string.IsNullOrWhiteSpace(inviteCode))
        {
            return Json(true); // the "required" rule handles empty values
        }

        var exists = await _organizations.InviteCodeExistsAsync(InviteCodeGenerator.Normalize(inviteCode));
        return Json(exists ? true : "No organization uses this code. Check it and try again.");
    }

    // ---------------- Logout & access denied ----------------

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
