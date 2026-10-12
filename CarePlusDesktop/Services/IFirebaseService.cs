using CarePlusDesktop.Models;

namespace CarePlusDesktop.Services;

public interface IFirebaseService
{
    bool IsConfigured { get; }
    Task<List<Patient>> GetPatientsAsync();
    Task<Patient?> GetPatientAsync(string id);
    Task<string> SavePatientAsync(Patient item);
    Task DeletePatientAsync(string id);

    Task<List<Doctor>> GetDoctorsAsync();
    Task<Doctor?> GetDoctorAsync(string id);
    Task<string> SaveDoctorAsync(Doctor item);
    Task DeleteDoctorAsync(string id);

    Task<List<Appointment>> GetAppointmentsAsync();
    Task<Appointment?> GetAppointmentAsync(string id);
    Task<string> SaveAppointmentAsync(Appointment item);
    Task DeleteAppointmentAsync(string id);

    Task<List<MedicalRecord>> GetMedicalRecordsAsync();
    Task<MedicalRecord?> GetMedicalRecordAsync(string id);
    Task<string> SaveMedicalRecordAsync(MedicalRecord item);
    Task DeleteMedicalRecordAsync(string id);

    Task<List<Prescription>> GetPrescriptionsAsync();
    Task<Prescription?> GetPrescriptionAsync(string id);
    Task<string> SavePrescriptionAsync(Prescription item);
    Task DeletePrescriptionAsync(string id);

    Task<DashboardViewModel> GetDashboardAsync();
}
