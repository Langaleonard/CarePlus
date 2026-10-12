using CarePlusDesktop.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarePlusDesktop.Controllers;

public class ReportsController : BaseAdminController
{
    private readonly IFirebaseService _firebase;
    public ReportsController(IFirebaseService firebase) => _firebase = firebase;
    public async Task<IActionResult> Index() => View(await _firebase.GetDashboardAsync());
}
