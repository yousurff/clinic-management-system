using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ClinicManagementSystem.Models;

namespace ClinicManagementSystem.Controllers;

[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    public UsersController(UserManager<IdentityUser> userManager) { _userManager = userManager; }

    public async Task<IActionResult> Index(string? search, string? role)
    {
        var model = new List<UserListItemViewModel>();
        foreach (var u in _userManager.Users.ToList())
        {
            var roles = await _userManager.GetRolesAsync(u);
            model.Add(new UserListItemViewModel { Id = u.Id, Email = u.Email ?? "", Roles = string.Join(", ", roles) });
        }

        if (!string.IsNullOrWhiteSpace(search))
            model = model.Where(m => m.Email.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
        if (!string.IsNullOrWhiteSpace(role))
            model = model.Where(m => m.Roles.Contains(role)).ToList();

        ViewBag.Search = search;
        ViewBag.Role = role;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(string id, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null && !string.IsNullOrWhiteSpace(newPassword))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user, token, newPassword);
        }
        return RedirectToAction(nameof(Index));
    }
}