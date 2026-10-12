using System.ComponentModel.DataAnnotations;

namespace CarePlusDesktop.Models;

public class Appointment
{
    public string? Id { get; set; }

    [Required]
    [Display(Name = "Patient")]
    public string PatientName { get; set; } = "";

    [Required]
    [Display(Name = "Doctor")]
    public string DoctorName { get; set; } = "";

    [Required]
    [DataType(DataType.Date)]
    public string Date { get; set; } = DateTime.Today.ToString("yyyy-MM-dd");

    [Required]
    [DataType(DataType.Time)]
    public string Time { get; set; } = "09:00";

    public string Status { get; set; } = "Requested";
    public string? Reason { get; set; }
    public string? Notes { get; set; }
}
