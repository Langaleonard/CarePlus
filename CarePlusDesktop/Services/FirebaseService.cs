using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Text.Json;
using CarePlusDesktop.Models;

namespace CarePlusDesktop.Services;

public class FirebaseService : IFirebaseService
{
    private readonly HttpClient _http;
    private readonly string _databaseUrl;
    private readonly string _authToken;

    private static readonly ConcurrentDictionary<string, Patient> DemoPatients = new();
    private static readonly ConcurrentDictionary<string, Doctor> DemoDoctors = new();
    private static readonly ConcurrentDictionary<string, Appointment> DemoAppointments = new();
    private static readonly ConcurrentDictionary<string, MedicalRecord> DemoRecords = new();
    private static readonly ConcurrentDictionary<string, Prescription> DemoPrescriptions = new();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public bool IsConfigured => !string.IsNullOrWhiteSpace(_databaseUrl) && _databaseUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase);

    public FirebaseService(IHttpClientFactory clientFactory, IConfiguration config)
    {
        _http = clientFactory.CreateClient();
        _databaseUrl = (config["Firebase:DatabaseUrl"] ?? "").TrimEnd('/');
        _authToken = config["Firebase:AuthToken"] ?? "";
        SeedDemo();
    }

    private static void SeedDemo()
    {
        if (!DemoPatients.IsEmpty) return;

        DemoPatients["p001"] = new Patient { Id = "p001", FirstName = "Naledi", LastName = "Mokoena", Email = "naledi@example.com", Phone = "082 555 0101", Gender = "Female", DateOfBirth = "1993-04-12", Address = "Johannesburg" };
        DemoPatients["p002"] = new Patient { Id = "p002", FirstName = "Thabo", LastName = "Dlamini", Email = "thabo@example.com", Phone = "083 555 0199", Gender = "Male", DateOfBirth = "1988-11-03", Address = "Soweto" };

        DemoDoctors["d001"] = new Doctor { Id = "d001", FirstName = "Justice", LastName = "Ngwenya", Email = "justice@careplus.local", Phone = "071 555 1001", Specialty = "General Practice", LicenseNumber = "HPCSA-10021", Status = "Approved" };
        DemoDoctors["d002"] = new Doctor { Id = "d002", FirstName = "Anele", LastName = "Khumalo", Email = "anele@careplus.local", Phone = "072 555 1002", Specialty = "Primary Care", LicenseNumber = "HPCSA-10022", Status = "Pending" };

        DemoAppointments["a001"] = new Appointment { Id = "a001", PatientName = "Naledi Mokoena", DoctorName = "Justice Ngwenya", Date = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd"), Time = "09:30", Status = "Confirmed", Reason = "Follow-up consultation" };
        DemoAppointments["a002"] = new Appointment { Id = "a002", PatientName = "Thabo Dlamini", DoctorName = "Anele Khumalo", Date = DateTime.Today.AddDays(2).ToString("yyyy-MM-dd"), Time = "11:00", Status = "Requested", Reason = "General consultation" };

        DemoRecords["r001"] = new MedicalRecord { Id = "r001", PatientName = "Naledi Mokoena", DoctorName = "Justice Ngwenya", RecordDate = DateTime.Today.AddDays(-14).ToString("yyyy-MM-dd"), Diagnosis = "Routine follow-up", TreatmentNotes = "Monitor symptoms and return if they worsen." };
        DemoPrescriptions["rx001"] = new Prescription { Id = "rx001", PatientName = "Naledi Mokoena", DoctorName = "Justice Ngwenya", Medication = "Paracetamol", Dosage = "500 mg", Instructions = "Take as directed when needed.", PrescriptionDate = DateTime.Today.AddDays(-14).ToString("yyyy-MM-dd") };
    }

    private string NodeUrl(string node, string? id = null)
    {
        var path = string.IsNullOrWhiteSpace(id) ? node : $"{node}/{id}";
        var url = $"{_databaseUrl}/{path}.json";
        if (!string.IsNullOrWhiteSpace(_authToken)) url += $"?auth={Uri.EscapeDataString(_authToken)}";
        return url;
    }

    private async Task<JsonDocument?> GetJsonAsync(string node, string? id = null)
    {
        using var response = await _http.GetAsync(NodeUrl(node, id));
        if (!response.IsSuccessStatusCode) return null;
        var text = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(text) || text == "null") return null;
        return JsonDocument.Parse(text);
    }

    private async Task<List<T>> GetCollectionAsync<T>(string node, Func<string, JsonElement, T> mapper)
    {
        var result = new List<T>();
        var doc = await GetJsonAsync(node);
        if (doc == null || doc.RootElement.ValueKind != JsonValueKind.Object) return result;

        foreach (var item in doc.RootElement.EnumerateObject())
        {
            if (item.Value.ValueKind == JsonValueKind.Object)
                result.Add(mapper(item.Name, item.Value));
        }
        return result;
    }

    private async Task<string> SaveAsync<T>(string node, string? id, T item)
    {
        if (!string.IsNullOrWhiteSpace(id))
        {
            using var response = await _http.PutAsJsonAsync(NodeUrl(node, id), item, JsonOptions);
            response.EnsureSuccessStatusCode();
            return id;
        }

        using var post = await _http.PostAsJsonAsync(NodeUrl(node), item, JsonOptions);
        post.EnsureSuccessStatusCode();
        var payload = await post.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        return payload != null && payload.TryGetValue("name", out var key) ? key : Guid.NewGuid().ToString("N");
    }

    private async Task DeleteAsync(string node, string id)
    {
        using var response = await _http.DeleteAsync(NodeUrl(node, id));
        response.EnsureSuccessStatusCode();
    }

    private static string S(JsonElement e, params string[] names)
    {
        foreach (var p in e.EnumerateObject())
        {
            if (names.Any(n => p.Name.Equals(n, StringComparison.OrdinalIgnoreCase)))
            {
                if (p.Value.ValueKind == JsonValueKind.String) return p.Value.GetString() ?? "";
                if (p.Value.ValueKind is JsonValueKind.Number or JsonValueKind.True or JsonValueKind.False) return p.Value.ToString();
            }
        }
        return "";
    }

    private static Patient MapPatient(string id, JsonElement e) => new()
    {
        Id = id,
        FirstName = S(e, "firstName", "firstname", "name"),
        LastName = S(e, "lastName", "lastname", "surname"),
        Email = S(e, "email"),
        Phone = S(e, "phone", "contact", "contactNumber"),
        DateOfBirth = S(e, "dateOfBirth", "dob", "birthDate"),
        Gender = S(e, "gender"),
        Address = S(e, "address", "location"),
        MedicalInformation = S(e, "medicalInformation", "medicalInfo", "medicalHistory")
    };

    private static Doctor MapDoctor(string id, JsonElement e) => new()
    {
        Id = id,
        FirstName = S(e, "firstName", "firstname", "name"),
        LastName = S(e, "lastName", "lastname", "surname"),
        Email = S(e, "email"),
        Phone = S(e, "phone", "contact", "contactNumber"),
        Specialty = S(e, "specialty", "speciality", "specialisation", "specialization"),
        LicenseNumber = S(e, "licenseNumber", "licenseNo", "practiceNumber"),
        Status = string.IsNullOrWhiteSpace(S(e, "status")) ? "Pending" : S(e, "status")
    };

    private static Appointment MapAppointment(string id, JsonElement e) => new()
    {
        Id = id,
        PatientName = S(e, "patientName", "patient", "patientEmail"),
        DoctorName = S(e, "doctorName", "doctor", "doctorEmail"),
        Date = S(e, "date", "appointmentDate"),
        Time = S(e, "time", "appointmentTime"),
        Status = string.IsNullOrWhiteSpace(S(e, "status")) ? "Requested" : S(e, "status"),
        Reason = S(e, "reason", "purpose"),
        Notes = S(e, "notes", "comment")
    };

    private static MedicalRecord MapRecord(string id, JsonElement e) => new()
    {
        Id = id,
        PatientName = S(e, "patientName", "patient"),
        DoctorName = S(e, "doctorName", "doctor"),
        RecordDate = S(e, "recordDate", "date"),
        Diagnosis = S(e, "diagnosis", "condition"),
        TreatmentNotes = S(e, "treatmentNotes", "notes", "treatment")
    };

    private static Prescription MapPrescription(string id, JsonElement e) => new()
    {
        Id = id,
        PatientName = S(e, "patientName", "patient"),
        DoctorName = S(e, "doctorName", "doctor"),
        Medication = S(e, "medication", "medicine", "drug"),
        Dosage = S(e, "dosage", "dose"),
        Instructions = S(e, "instructions", "notes"),
        PrescriptionDate = S(e, "prescriptionDate", "date")
    };

    public async Task<List<Patient>> GetPatientsAsync() => IsConfigured ? await GetCollectionAsync("Patient", MapPatient) : DemoPatients.Values.OrderBy(x => x.LastName).ToList();
    public async Task<Patient?> GetPatientAsync(string id) => IsConfigured ? (await GetCollectionAsync("Patient", MapPatient)).FirstOrDefault(x => x.Id == id) : DemoPatients.GetValueOrDefault(id);
    public async Task<string> SavePatientAsync(Patient item)
    {
        if (IsConfigured) return await SaveAsync("Patient", item.Id, item);
        var id = item.Id ?? Guid.NewGuid().ToString("N")[..8]; item.Id = id; DemoPatients[id] = item; return id;
    }
    public async Task DeletePatientAsync(string id) { if (IsConfigured) await DeleteAsync("Patient", id); else DemoPatients.TryRemove(id, out _); }

    public async Task<List<Doctor>> GetDoctorsAsync() => IsConfigured ? await GetCollectionAsync("Doctor", MapDoctor) : DemoDoctors.Values.OrderBy(x => x.LastName).ToList();
    public async Task<Doctor?> GetDoctorAsync(string id) => IsConfigured ? (await GetCollectionAsync("Doctor", MapDoctor)).FirstOrDefault(x => x.Id == id) : DemoDoctors.GetValueOrDefault(id);
    public async Task<string> SaveDoctorAsync(Doctor item)
    {
        if (IsConfigured) return await SaveAsync("Doctor", item.Id, item);
        var id = item.Id ?? Guid.NewGuid().ToString("N")[..8]; item.Id = id; DemoDoctors[id] = item; return id;
    }
    public async Task DeleteDoctorAsync(string id) { if (IsConfigured) await DeleteAsync("Doctor", id); else DemoDoctors.TryRemove(id, out _); }

    public async Task<List<Appointment>> GetAppointmentsAsync() => IsConfigured ? await GetCollectionAsync("Appointment", MapAppointment) : DemoAppointments.Values.OrderBy(x => x.Date).ThenBy(x => x.Time).ToList();
    public async Task<Appointment?> GetAppointmentAsync(string id) => IsConfigured ? (await GetCollectionAsync("Appointment", MapAppointment)).FirstOrDefault(x => x.Id == id) : DemoAppointments.GetValueOrDefault(id);
    public async Task<string> SaveAppointmentAsync(Appointment item)
    {
        if (IsConfigured) return await SaveAsync("Appointment", item.Id, item);
        var id = item.Id ?? Guid.NewGuid().ToString("N")[..8]; item.Id = id; DemoAppointments[id] = item; return id;
    }
    public async Task DeleteAppointmentAsync(string id) { if (IsConfigured) await DeleteAsync("Appointment", id); else DemoAppointments.TryRemove(id, out _); }

    public async Task<List<MedicalRecord>> GetMedicalRecordsAsync() => IsConfigured ? await GetCollectionAsync("MedicalRecord", MapRecord) : DemoRecords.Values.OrderByDescending(x => x.RecordDate).ToList();
    public async Task<MedicalRecord?> GetMedicalRecordAsync(string id) => IsConfigured ? (await GetCollectionAsync("MedicalRecord", MapRecord)).FirstOrDefault(x => x.Id == id) : DemoRecords.GetValueOrDefault(id);
    public async Task<string> SaveMedicalRecordAsync(MedicalRecord item)
    {
        if (IsConfigured) return await SaveAsync("MedicalRecord", item.Id, item);
        var id = item.Id ?? Guid.NewGuid().ToString("N")[..8]; item.Id = id; DemoRecords[id] = item; return id;
    }
    public async Task DeleteMedicalRecordAsync(string id) { if (IsConfigured) await DeleteAsync("MedicalRecord", id); else DemoRecords.TryRemove(id, out _); }

    public async Task<List<Prescription>> GetPrescriptionsAsync() => IsConfigured ? await GetCollectionAsync("Prescription", MapPrescription) : DemoPrescriptions.Values.OrderByDescending(x => x.PrescriptionDate).ToList();
    public async Task<Prescription?> GetPrescriptionAsync(string id) => IsConfigured ? (await GetCollectionAsync("Prescription", MapPrescription)).FirstOrDefault(x => x.Id == id) : DemoPrescriptions.GetValueOrDefault(id);
    public async Task<string> SavePrescriptionAsync(Prescription item)
    {
        if (IsConfigured) return await SaveAsync("Prescription", item.Id, item);
        var id = item.Id ?? Guid.NewGuid().ToString("N")[..8]; item.Id = id; DemoPrescriptions[id] = item; return id;
    }
    public async Task DeletePrescriptionAsync(string id) { if (IsConfigured) await DeleteAsync("Prescription", id); else DemoPrescriptions.TryRemove(id, out _); }

    public async Task<DashboardViewModel> GetDashboardAsync()
    {
        var patientsTask = GetPatientsAsync();
        var doctorsTask = GetDoctorsAsync();
        var appointmentsTask = GetAppointmentsAsync();
        var recordsTask = GetMedicalRecordsAsync();
        var prescriptionsTask = GetPrescriptionsAsync();
        await Task.WhenAll(patientsTask, doctorsTask, appointmentsTask, recordsTask, prescriptionsTask);

        var appointments = appointmentsTask.Result;
        return new DashboardViewModel
        {
            Patients = patientsTask.Result.Count,
            Doctors = doctorsTask.Result.Count,
            Appointments = appointments.Count,
            PendingAppointments = appointments.Count(x => x.Status.Equals("Requested", StringComparison.OrdinalIgnoreCase) || x.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase)),
            MedicalRecords = recordsTask.Result.Count,
            Prescriptions = prescriptionsTask.Result.Count,
            FirebaseConnected = IsConfigured,
            RecentAppointments = appointments.OrderBy(x => x.Date).ThenBy(x => x.Time).Take(6).ToList()
        };
    }
}
