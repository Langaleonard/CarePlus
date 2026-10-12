using Microsoft.AspNetCore.Mvc;

namespace CarePlusDesktop.Controllers;

public class AccountController : Controller
{
    private readonly IConfiguration _config;
    public AccountController(IConfiguration config) => _config = config;

    [HttpGet]
    public IActionResult Login()
    {
        if (HttpContext.Session.GetString("AdminUser") != null) return RedirectToAction("Index", "Dashboard");
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Login(string email, string password)
    {
        var expectedEmail = _config["Admin:Email"] ?? "admin@careplus.local";
        var expectedPassword = _config["Admin:Password"] ?? "CarePlus2026!";

        if (email.Equals(expectedEmail, StringComparison.OrdinalIgnoreCase) && password == expectedPassword)
        {
            HttpContext.Session.SetString("AdminUser", email);
            return RedirectToAction("Index", "Dashboard");
        }

        ViewBag.Error = "Invalid email or password.";
        ViewBag.Email = email;
        return View();
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(Login));
    }
}
