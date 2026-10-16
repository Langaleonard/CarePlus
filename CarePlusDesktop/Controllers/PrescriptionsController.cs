using CarePlusDesktop.Models;
using CarePlusDesktop.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarePlusDesktop.Controllers;

public class PrescriptionsController : BaseAdminController
{
    private readonly IFirebaseService _firebase;
    public PrescriptionsController(IFirebaseService firebase) => _firebase = firebase;

    public async Task<IActionResult> Index() => View(await _firebase.GetPrescriptionsAsync());

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadLookupsAsync();
        return View(new Prescription());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Prescription item)
    {
        if (!ModelState.IsValid)
        {
            await LoadLookupsAsync();
            return View(item);
        }

        await _firebase.SavePrescriptionAsync(item);
        TempData["Success"] = "Prescription added.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var item = await _firebase.GetPrescriptionAsync(id);
        if (item == null) return NotFound();

        await LoadLookupsAsync();
        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Prescription item)
    {
        if (!ModelState.IsValid)
        {
            await LoadLookupsAsync();
            return View(item);
        }

        await _firebase.SavePrescriptionAsync(item);
        TempData["Success"] = "Prescription updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        await _firebase.DeletePrescriptionAsync(id);
        TempData["Success"] = "Prescription deleted.";
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
