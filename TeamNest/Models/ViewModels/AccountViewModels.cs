using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace TeamNest.Models.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Enter your email.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Enter your password.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Keep me logged in")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}

public enum WorkspaceChoice
{
    Create,
    Join
}

/// <summary>
/// IValidatableObject lets us write rules that depend on more than one field,
/// e.g. "organization name is required only when creating a new workspace".
/// </summary>
public class RegisterViewModel : IValidatableObject
{
    [Required(ErrorMessage = "Enter your full name.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Use 3 to 100 characters.")]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Enter your email.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Choose a password.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Use at least 8 characters.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Type your password again.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Passwords don't match.")]
    [Display(Name = "Confirm password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    public WorkspaceChoice Choice { get; set; } = WorkspaceChoice.Create;

    [StringLength(100, ErrorMessage = "Use 100 characters or fewer.")]
    [Display(Name = "Organization name")]
    public string? OrganizationName { get; set; }

    // [Remote] = AJAX validation: the browser asks the server if the code exists while the user types
    [Remote(action: "CheckInviteCode", controller: "Account")]
    [Display(Name = "Invite code")]
    public string? InviteCode { get; set; }

    [Display(Name = "I agree to the terms of use")]
    public bool AcceptTerms { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Choice == WorkspaceChoice.Create && string.IsNullOrWhiteSpace(OrganizationName))
        {
            yield return new ValidationResult("Enter a name for your organization.", new[] { nameof(OrganizationName) });
        }

        if (Choice == WorkspaceChoice.Join && string.IsNullOrWhiteSpace(InviteCode))
        {
            yield return new ValidationResult("Enter the invite code you received.", new[] { nameof(InviteCode) });
        }

        if (!AcceptTerms)
        {
            yield return new ValidationResult("You need to accept the terms to continue.", new[] { nameof(AcceptTerms) });
        }
    }
}
