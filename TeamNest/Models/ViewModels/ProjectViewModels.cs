using System.ComponentModel.DataAnnotations;
using TeamNest.Models.Dtos;
using TeamNest.Models.Entities;

namespace TeamNest.Models.ViewModels;

/// <summary>Used by both the "New project" modal and the Edit page.</summary>
public class ProjectFormViewModel : IValidatableObject
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Give your project a name.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Use 3 to 100 characters.")]
    [Display(Name = "Project name")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Use 500 characters or fewer.")]
    public string? Description { get; set; }

    public ProjectStatus Status { get; set; } = ProjectStatus.Planning;

    [DataType(DataType.Date)]
    [Display(Name = "Start date")]
    public DateTime? StartDate { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Due date")]
    public DateTime? DueDate { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartDate.HasValue && DueDate.HasValue && DueDate.Value < StartDate.Value)
        {
            yield return new ValidationResult("Due date can't be before the start date.", new[] { nameof(DueDate) });
        }
    }
}

public class ProjectsIndexViewModel
{
    public IEnumerable<ProjectListItem> Projects { get; set; } = Enumerable.Empty<ProjectListItem>();
    public string? Term { get; set; }
    public ProjectStatus? Status { get; set; }
    public ProjectFormViewModel NewProject { get; set; } = new();
}
