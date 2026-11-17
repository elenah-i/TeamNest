using TeamNest.Models.Dtos;
using TeamNest.Models.Entities;

namespace TeamNest.Repositories;

/// <summary>
/// Every method is automatically limited to the signed-in user's organization,
/// so one company can never see or change another company's projects.
/// </summary>
public interface IProjectRepository
{
    Task<IEnumerable<ProjectListItem>> SearchAsync(string? term, ProjectStatus? status);
    Task<IEnumerable<ProjectListItem>> GetRecentAsync(int count);
    Task<Project?> GetByIdAsync(int id);
    Task<int> CreateAsync(Project project);
    Task<bool> UpdateAsync(Project project);
    Task<bool> SoftDeleteAsync(int id);
}
