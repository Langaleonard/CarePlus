namespace CarePlusDesktop.Models;

public class DashboardViewModel
{
    public int Patients { get; set; }
    public int Doctors { get; set; }
    public int Appointments { get; set; }
    public int PendingAppointments { get; set; }
    public int MedicalRecords { get; set; }
    public int Prescriptions { get; set; }
    public bool FirebaseConnected { get; set; }
    public List<Appointment> RecentAppointments { get; set; } = new();
}
