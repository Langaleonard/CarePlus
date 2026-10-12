using CarePlusDesktop.Models;
using CarePlusDesktop.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarePlusDesktop.Controllers;

public class PatientsController : BaseAdminController
{
    private readonly IFirebaseService _firebase;
    public PatientsController(IFirebaseService firebase) => _firebase = firebase;

    public async Task<IActionResult> Index(string? q)
    {
        var items = await _firebase.GetPatientsAsync();
        if (!string.IsNullOrWhiteSpace(q)) items = items.Where(x => (x.FullName + " " + x.Email + " " + x.Phone).Contains(q, StringComparison.OrdinalIgnoreCase)).ToList();
        ViewBag.Query = q;
        return View(items);
    }

    [HttpGet] public IActionResult Create() => View(new Patient());
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Patient item)
    {
        if (!ModelState.IsValid) return View(item);
        await _firebase.SavePatientAsync(item);
        TempData["Success"] = "Patient created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var item = await _firebase.GetPatientAsync(id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Patient item)
    {
        if (!ModelState.IsValid) return View(item);
        await _firebase.SavePatientAsync(item);
        TempData["Success"] = "Patient updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        await _firebase.DeletePatientAsync(id);
        TempData["Success"] = "Patient deleted.";
        return RedirectToAction(nameof(Index));
    }
}
