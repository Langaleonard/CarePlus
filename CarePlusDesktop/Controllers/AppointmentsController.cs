using CarePlusDesktop.Models;
using CarePlusDesktop.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarePlusDesktop.Controllers;

public class AppointmentsController : BaseAdminController
{
    private readonly IFirebaseService _firebase;
    public AppointmentsController(IFirebaseService firebase) => _firebase = firebase;

    public async Task<IActionResult> Index(string? status)
    {
        var items = await _firebase.GetAppointmentsAsync();
        if (!string.IsNullOrWhiteSpace(status))
        {
            items = items
                .Where(x => x.Status.Equals(status, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        ViewBag.Status = status;
        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadLookupsAsync();
        return View(new Appointment());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Appointment item)
    {
        if (!ModelState.IsValid)
        {
            await LoadLookupsAsync();
            return View(item);
        }

        await _firebase.SaveAppointmentAsync(item);
        TempData["Success"] = "Appointment created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var item = await _firebase.GetAppointmentAsync(id);
        if (item == null) return NotFound();

        await LoadLookupsAsync();
        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Appointment item)
    {
        if (!ModelState.IsValid)
        {
            await LoadLookupsAsync();
            return View(item);
        }

        await _firebase.SaveAppointmentAsync(item);
        TempData["Success"] = "Appointment updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SetStatus(string id, string status)
    {
        var allowed = new[] { "Requested", "Confirmed", "Completed", "Cancelled" };
        if (!allowed.Contains(status, StringComparer.OrdinalIgnoreCase))
        {
            return BadRequest();
        }

        var item = await _firebase.GetAppointmentAsync(id);
        if (item != null)
        {
            item.Status = status;
            await _firebase.SaveAppointmentAsync(item);
            TempData["Success"] = $"Appointment marked {status}.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        await _firebase.DeleteAppointmentAsync(id);
        TempData["Success"] = "Appointment deleted.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadLookupsAsync()
    {
        var patientsTask = _firebase.GetPatientsAsync();
        var doctorsTask = _firebase.GetDoctorsAsync();
        await Task.WhenAll(patientsTask, doctorsTask);

        ViewBag.Patients = patientsTask.Result
            .Select(x => x.FullName)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x)
            .ToList();

        ViewBag.Doctors = doctorsTask.Result
            .Select(x => x.FullName)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x)
            .ToList();
    }
}
