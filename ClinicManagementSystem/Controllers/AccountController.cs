using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.Controllers;

[AllowAnonymous]
public class AccountController : Controller
{
    private readonly SignInManager<IdentityUser> _signInManager;

    public AccountController(SignInManager<IdentityUser> signInManager)
    {
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string email, string password)
    {
        var result = await _signInManager.PasswordSignInAsync(email, password, isPersistent: true, lockoutOnFailure: false);
        if (result.Succeeded)
            return RedirectToAction("Index", "Home");

        ViewBag.Error = "E-posta veya şifre hatalı.";
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }

    public IActionResult AccessDenied() => View();
}