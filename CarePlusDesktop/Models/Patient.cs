using System.ComponentModel.DataAnnotations;

namespace CarePlusDesktop.Models;

public class Patient
{
    public string? Id { get; set; }

    [Required, StringLength(80)]
    [Display(Name = "First name")]
    public string FirstName { get; set; } = "";

    [Required, StringLength(80)]
    [Display(Name = "Last name")]
    public string LastName { get; set; } = "";

    [EmailAddress]
    public string? Email { get; set; }

    [Phone]
    public string? Phone { get; set; }

    [Display(Name = "Date of birth")]
    public string? DateOfBirth { get; set; }

    public string? Gender { get; set; }
    public string? Address { get; set; }

    [Display(Name = "Medical information")]
    public string? MedicalInformation { get; set; }

    public string FullName => string.Join(" ", new[] { FirstName, LastName }.Where(x => !string.IsNullOrWhiteSpace(x))).Trim();
}
