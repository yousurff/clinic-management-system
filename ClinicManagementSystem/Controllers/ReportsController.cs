using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClinicManagementSystem.Data;
using ClinicManagementSystem.Models;

namespace ClinicManagementSystem.Controllers;

[Authorize(Roles = "Admin,Receptionist")]
public class ReportsController : Controller
{
    private readonly AppDbContext _context;
    public ReportsController(AppDbContext context) { _context = context; }

    public async Task<IActionResult> Index(int? patientId)
    {
        var appts = await _context.Appointments
            .Include(a => a.Doctor).ThenInclude(d => d.Department)
            .ToListAsync();

        ViewBag.ByDoctor = appts
            .GroupBy(a => a.Doctor != null ? a.Doctor.FirstName + " " + a.Doctor.LastName : "—")
            .Select(g => new ReportRow { Label = g.Key, Count = g.Count() })
            .OrderByDescending(r => r.Count)
            .ToList();

        ViewBag.ByDepartment = appts
            .GroupBy(a => a.Doctor?.Department?.Name ?? "—")
            .Select(g => new ReportRow { Label = g.Key, Count = g.Count() })
            .OrderByDescending(r => r.Count)
            .ToList();

        ViewBag.Monthly = appts
            .GroupBy(a => new { a.AppointmentDate.Year, a.AppointmentDate.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new ReportRow { Label = $"{g.Key.Month:00}/{g.Key.Year}", Count = g.Count() })
            .ToList();

        ViewBag.Patients = new SelectList(
            (await _context.Patients.ToListAsync())
                .Select(p => new { p.PatientID, FullName = p.FirstName + " " + p.LastName }),
            "PatientID", "FullName", patientId);

        if (patientId.HasValue)
        {
            ViewBag.PatientHistory = await _context.Appointments
                .Include(a => a.Doctor).ThenInclude(d => d.Department)
                .Where(a => a.PatientID == patientId.Value)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }

        return View();
    }
}