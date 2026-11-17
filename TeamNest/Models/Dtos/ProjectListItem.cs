using TeamNest.Models.Entities;

namespace TeamNest.Models.Dtos;

/// <summary>One row in the projects list. Filled directly by a Dapper query.</summary>
public class ProjectListItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ProjectStatus Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedByName { get; set; }

    public bool IsOverdue =>
        DueDate.HasValue && DueDate.Value.Date < DateTime.Today && Status != ProjectStatus.Completed;
}
