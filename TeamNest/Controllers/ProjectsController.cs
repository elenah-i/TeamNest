using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamNest.Models;
using TeamNest.Models.Entities;
using TeamNest.Models.ViewModels;
using TeamNest.Repositories;
using TeamNest.Services;

namespace TeamNest.Controllers;

[Authorize]
public class ProjectsController : Controller
{
    private readonly IProjectRepository _projects;
    private readonly ICurrentUserService _currentUser;

    public ProjectsController(IProjectRepository projects, ICurrentUserService currentUser)
    {
        _projects = projects;
        _currentUser = currentUser;
    }

    // GET: /Projects
    public async Task<IActionResult> Index(string? term, ProjectStatus? status)
    {
        var model = new ProjectsIndexViewModel
        {
            Term = term,
            Status = status,
            Projects = await _projects.SearchAsync(term, status)
        };

        return View(model);
    }

    // GET: /Projects/Search?term=web&status=1   (AJAX: returns only the list as HTML)
    [HttpGet]
    public async Task<IActionResult> Search(string? term, ProjectStatus? status)
    {
        var projects = await _projects.SearchAsync(term, status);
        return PartialView("_ProjectList", projects);
    }

    // POST: /Projects/Create   (AJAX from the "New project" modal)
    [HttpPost]
    [Authorize(Policy = AppPolicies.CanManageProjects)]
    public async Task<IActionResult> Create(ProjectFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // Send the form back with error messages so the modal can show them
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return PartialView("_ProjectForm", model);
        }

        var project = new Project
        {
            Name = model.Name.Trim(),
            Description = model.Description?.Trim(),
            Status = model.Status,
            StartDate = model.StartDate,
            DueDate = model.DueDate,
            CreatedByUserId = _currentUser.UserId,
            CreatedAt = DateTime.UtcNow
        };

        var id = await _projects.CreateAsync(project);

        return Json(new { success = true, id, message = $"Project \"{project.Name}\" created." });
    }

    // GET: /Projects/Edit/5   (normal full-page form)
    [HttpGet]
    [Authorize(Policy = AppPolicies.CanManageProjects)]
    public async Task<IActionResult> Edit(int id)
    {
        var project = await _projects.GetByIdAsync(id);
        if (project is null)
        {
            return NotFound();
        }

        var model = new ProjectFormViewModel
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Status = project.Status,
            StartDate = project.StartDate,
            DueDate = project.DueDate
        };

        return View(model);
    }

    // POST: /Projects/Edit/5
    [HttpPost]
    [Authorize(Policy = AppPolicies.CanManageProjects)]
    public async Task<IActionResult> Edit(int id, ProjectFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Id = id;
            return View(model);
        }

        var updated = await _projects.UpdateAsync(new Project
        {
            Id = id,
            Name = model.Name.Trim(),
            Description = model.Description?.Trim(),
            Status = model.Status,
            StartDate = model.StartDate,
            DueDate = model.DueDate
        });

        if (!updated)
        {
            return NotFound();
        }

        TempData["Toast"] = "Changes saved.";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Projects/Delete/5   (AJAX)
    [HttpPost]
    [Authorize(Policy = AppPolicies.CanManageProjects)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _projects.SoftDeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new { success = false, message = "That project no longer exists." });
        }

        return Json(new { success = true, message = "Project deleted." });
    }
}
