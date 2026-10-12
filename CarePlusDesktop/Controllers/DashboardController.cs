using CarePlusDesktop.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarePlusDesktop.Controllers;

public class DashboardController : BaseAdminController
{
    private readonly IFirebaseService _firebase;
    public DashboardController(IFirebaseService firebase) => _firebase = firebase;

    public async Task<IActionResult> Index()
    {
        try { return View(await _firebase.GetDashboardAsync()); }
        catch (Exception ex) { ViewBag.Error = ex.Message; return View(new Models.DashboardViewModel()); }
    }

    public IActionResult Error() => View();
}
