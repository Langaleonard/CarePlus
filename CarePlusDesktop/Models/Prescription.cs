using System.ComponentModel.DataAnnotations;

namespace CarePlusDesktop.Models;

public class Prescription
{
    public string? Id { get; set; }

    [Required, Display(Name = "Patient")]
    public string PatientName { get; set; } = "";

    [Required, Display(Name = "Doctor")]
    public string DoctorName { get; set; } = "";

    [Required]
    public string Medication { get; set; } = "";

    [Required]
    public string Dosage { get; set; } = "";

    public string? Instructions { get; set; }

    [Required, Display(Name = "Prescription date")]
    public string PrescriptionDate { get; set; } = DateTime.Today.ToString("yyyy-MM-dd");
}
