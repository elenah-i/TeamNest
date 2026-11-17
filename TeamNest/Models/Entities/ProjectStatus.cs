using System.ComponentModel.DataAnnotations;

namespace TeamNest.Models.Entities;

public enum ProjectStatus
{
    Planning = 0,
    Active = 1,

    [Display(Name = "On hold")]
    OnHold = 2,

    Completed = 3
}
