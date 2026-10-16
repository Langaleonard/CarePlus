# Care Plus Desktop Admin Portal

ASP.NET Core MVC desktop-facing administration portal for the Care Plus Healthcare Management System.

## Main features
- Secure administrator session login
- Shared Firebase Realtime Database integration
- Patient and doctor management
- Doctor approval
- Appointment management with Patient and Doctor dropdowns
- Medical records
- Digital prescriptions
- Reports/dashboard
- Validation and error handling

## Run locally
```bash
dotnet restore
dotnet build
dotnet run
```

Open `http://localhost:5267`.

## Firebase
The project expects a local file named `firebase-service-account.json` in the project root. The private key is intentionally excluded from this ZIP and from Git by `.gitignore`.

Database URL is configured in `appsettings.json`.

## Screenshot checklist
Capture the Login, Dashboard, Patients, Doctors, Appointments, Medical Records, Prescriptions and Reports pages after adding suitable test data.
