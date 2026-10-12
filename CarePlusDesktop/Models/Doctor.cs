using System.ComponentModel.DataAnnotations;

namespace CarePlusDesktop.Models;

public class Doctor
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

    [Required, StringLength(120)]
    public string Specialty { get; set; } = "General Practice";

    [Display(Name = "License number")]
    public string? LicenseNumber { get; set; }

    public string Status { get; set; } = "Pending";
    public string FullName => string.Join(" ", new[] { FirstName, LastName }.Where(x => !string.IsNullOrWhiteSpace(x))).Trim();
}
