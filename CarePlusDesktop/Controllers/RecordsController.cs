using CarePlusDesktop.Models;
using CarePlusDesktop.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarePlusDesktop.Controllers;

public class RecordsController : BaseAdminController
{
    private readonly IFirebaseService _firebase;
    public RecordsController(IFirebaseService firebase) => _firebase = firebase;

    public async Task<IActionResult> Index() => View(await _firebase.GetMedicalRecordsAsync());

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadLookupsAsync();
        return View(new MedicalRecord());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MedicalRecord item)
    {
        if (!ModelState.IsValid)
        {
            await LoadLookupsAsync();
            return View(item);
        }

        await _firebase.SaveMedicalRecordAsync(item);
        TempData["Success"] = "Medical record added.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var item = await _firebase.GetMedicalRecordAsync(id);
        if (item == null) return NotFound();

        await LoadLookupsAsync();
        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(MedicalRecord item)
    {
        if (!ModelState.IsValid)
        {
            await LoadLookupsAsync();
            return View(item);
        }

        await _firebase.SaveMedicalRecordAsync(item);
        TempData["Success"] = "Medical record updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        await _firebase.DeleteMedicalRecordAsync(id);
        TempData["Success"] = "Medical record deleted.";
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
