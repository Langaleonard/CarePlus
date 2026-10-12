using System.ComponentModel.DataAnnotations;

namespace CarePlusDesktop.Models;

public class MedicalRecord
{
    public string? Id { get; set; }

    [Required, Display(Name = "Patient")]
    public string PatientName { get; set; } = "";

    [Required, Display(Name = "Doctor")]
    public string DoctorName { get; set; } = "";

    [Required, Display(Name = "Record date")]
    public string RecordDate { get; set; } = DateTime.Today.ToString("yyyy-MM-dd");

    [Required]
    public string Diagnosis { get; set; } = "";

    [Display(Name = "Treatment notes")]
    public string? TreatmentNotes { get; set; }
}
