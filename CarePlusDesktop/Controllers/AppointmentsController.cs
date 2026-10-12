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
        if (!string.IsNullOrWhiteSpace(status)) items = items.Where(x => x.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
        ViewBag.Status = status;
        return View(items);
    }

    [HttpGet] public IActionResult Create() => View(new Appointment());
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Appointment item)
    {
        if (!ModelState.IsValid) return View(item);
        await _firebase.SaveAppointmentAsync(item);
        TempData["Success"] = "Appointment created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var item = await _firebase.GetAppointmentAsync(id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Appointment item)
    {
        if (!ModelState.IsValid) return View(item);
        await _firebase.SaveAppointmentAsync(item);
        TempData["Success"] = "Appointment updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SetStatus(string id, string status)
    {
        var item = await _firebase.GetAppointmentAsync(id);
        if (item != null) { item.Status = status; await _firebase.SaveAppointmentAsync(item); TempData["Success"] = $"Appointment marked {status}."; }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        await _firebase.DeleteAppointmentAsync(id);
        TempData["Success"] = "Appointment deleted.";
        return RedirectToAction(nameof(Index));
    }
}
