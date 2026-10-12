# Care Plus Desktop Admin Portal

ASP.NET Core MVC desktop-facing admin portal for the Care Plus Healthcare Management System.

## Run

```bash
dotnet restore
dotnet run
```

The included launch profile uses:

`http://localhost:5267`

## Demo login

- Email: `admin@careplus.local`
- Password: `CarePlus2026!`

Change these values in `appsettings.json` before final submission.

## Connect the existing Firebase Realtime Database

Open `appsettings.json` and set:

```json
"Firebase": {
  "DatabaseUrl": "https://YOUR-PROJECT-ID-default-rtdb.firebaseio.com",
  "AuthToken": ""
}
```

Use the exact Realtime Database URL from the Firebase console.

This project currently reads/writes these Firebase nodes:

- `Patient`
- `Doctor`
- `Appointment`
- `MedicalRecord`
- `Prescription`

The mobile screenshots supplied by the group already show `Patient`, `Doctor`, `Appointment` and `Admin` data in Firebase. If your mobile app uses different node names, update the node strings in `Services/FirebaseService.cs`.

If `DatabaseUrl` is blank, the application automatically runs with sample demo data so the UI can be tested immediately.

## Main features

- Admin login and session protection
- Dashboard and reporting summary
- Patient management
- Doctor management and doctor approval
- Appointment management and status updates
- Medical record management
- Prescription management
- Firebase Realtime Database integration
- Search and filtering
- Validation, confirmations and friendly error handling
- Responsive desktop admin UI

## Important before submission

1. Add the real Firebase database URL.
2. Test every screen against the shared mobile database.
3. Change the demo admin password.
4. Push the project to the team's GitHub repository.
5. Add the lecturer as collaborator.
6. Capture screenshots of login, dashboard, patients, doctors, appointments, records and reports.
7. Keep evidence of commits and testing for the final report.

## Architecture

Android mobile app -> Firebase / shared data -> Care Plus Desktop Admin Portal

The Task 1 document originally proposed ASP.NET Core REST services and Azure SQL. This implementation package prioritises the working desktop admin application and offers Firebase integration to match the group's already-built mobile application. Any implementation change should be explained in the Task 2/final report.
