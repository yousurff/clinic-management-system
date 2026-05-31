using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicManagementSystem.Data;
using ClinicManagementSystem.Models;

namespace ClinicManagementSystem.Controllers;

[Authorize(Roles = "Admin,Receptionist")]
public class HomeController : Controller
{
    private readonly AppDbContext _context;
    public HomeController(AppDbContext context) { _context = context; }

    public async Task<IActionResult> Index()
    {
        ViewBag.PatientCount = await _context.Patients.CountAsync();
        ViewBag.DoctorCount = await _context.Doctors.CountAsync();
        ViewBag.DepartmentCount = await _context.Departments.CountAsync();
        ViewBag.AppointmentCount = await _context.Appointments.CountAsync();

        ViewBag.Pending = await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor).ThenInclude(d => d.Department)
            .Where(a => a.Status == AppointmentStatus.Pending)
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync();

        ViewBag.Approved = await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor).ThenInclude(d => d.Department)
            .Where(a => a.Status == AppointmentStatus.Approved)
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync();

        var all = await _context.Appointments.Include(a => a.Doctor).ThenInclude(d => d.Department).ToListAsync();
        ViewBag.StatusData = Enum.GetValues<AppointmentStatus>().Select(s => all.Count(a => a.Status == s)).ToList();
        var deptGroups = all.GroupBy(a => a.Doctor?.Department?.Name ?? "—")
            .Select(g => new { Name = g.Key, Count = g.Count() }).ToList();
        ViewBag.DeptLabels = deptGroups.Select(d => d.Name).ToList();
        ViewBag.DeptData = deptGroups.Select(d => d.Count).ToList();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        var a = await _context.Appointments.FindAsync(id);
        if (a != null && a.Status == AppointmentStatus.Pending)
        {
            a.Status = AppointmentStatus.Approved;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(int id)
    {
        var a = await _context.Appointments.FindAsync(id);
        if (a != null && a.Status == AppointmentStatus.Approved && DateTime.Now >= a.AppointmentDate)
        {
            a.Status = AppointmentStatus.Completed;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Privacy() => View();

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}