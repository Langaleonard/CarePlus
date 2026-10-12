using CarePlusDesktop.Models;
using CarePlusDesktop.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarePlusDesktop.Controllers;

public class DoctorsController : BaseAdminController
{
    private readonly IFirebaseService _firebase;
    public DoctorsController(IFirebaseService firebase) => _firebase = firebase;

    public async Task<IActionResult> Index(string? q)
    {
        var items = await _firebase.GetDoctorsAsync();
        if (!string.IsNullOrWhiteSpace(q)) items = items.Where(x => (x.FullName + " " + x.Email + " " + x.Specialty).Contains(q, StringComparison.OrdinalIgnoreCase)).ToList();
        ViewBag.Query = q;
        return View(items);
    }

    [HttpGet] public IActionResult Create() => View(new Doctor());
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Doctor item)
    {
        if (!ModelState.IsValid) return View(item);
        await _firebase.SaveDoctorAsync(item);
        TempData["Success"] = "Doctor created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var item = await _firebase.GetDoctorAsync(id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Doctor item)
    {
        if (!ModelState.IsValid) return View(item);
        await _firebase.SaveDoctorAsync(item);
        TempData["Success"] = "Doctor updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(string id)
    {
        var item = await _firebase.GetDoctorAsync(id);
        if (item != null) { item.Status = "Approved"; await _firebase.SaveDoctorAsync(item); TempData["Success"] = "Doctor approved."; }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        await _firebase.DeleteDoctorAsync(id);
        TempData["Success"] = "Doctor deleted.";
        return RedirectToAction(nameof(Index));
    }
}
