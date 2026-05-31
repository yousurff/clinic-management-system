using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.Controllers;

[AllowAnonymous]
public class LandingController : Controller
{
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            if (User.IsInRole("Patient")) return RedirectToAction("Index", "Portal");
            return RedirectToAction("Index", "Home");
        }
        return View();
    }
}