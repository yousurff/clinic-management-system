using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicManagementSystem.Data;
using ClinicManagementSystem.Models;

namespace ClinicManagementSystem.Controllers;

[Authorize(Roles = "Patient")]
public class PortalController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public PortalController(AppDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    private async Task<Patient?> CurrentPatientAsync()
    {
        var uid = _userManager.GetUserId(User);
        return await _context.Patients.FirstOrDefaultAsync(p => p.UserId == uid);
    }

    public async Task<IActionResult> Index()
    {
        var patient = await CurrentPatientAsync();
        if (patient == null) return NotFound();
        ViewBag.PatientName = patient.FirstName + " " + patient.LastName;
        var appts = await _context.Appointments
            .Include(a => a.Doctor).ThenInclude(d => d.Department)
            .Where(a => a.PatientID == patient.PatientID)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();
        return View(appts);
    }

    public async Task<IActionResult> Book()
    {
        await PopulateDoctorDropdown();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Book(int doctorId, DateTime appointmentDate, string? notes)
    {
        var patient = await CurrentPatientAsync();
        if (patient == null) return NotFound();
        _context.Appointments.Add(new Appointment
        {
            PatientID = patient.PatientID,
            DoctorID = doctorId,
            AppointmentDate = DateTime.SpecifyKind(appointmentDate, DateTimeKind.Utc),
            Status = AppointmentStatus.Pending,
            Notes = notes
        });
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var patient = await CurrentPatientAsync();
        var appt = await _context.Appointments.FirstOrDefaultAsync(a => a.AppointmentID == id);
        if (patient != null && appt != null && appt.PatientID == patient.PatientID
            && appt.Status != AppointmentStatus.Completed)
        {
            appt.Status = AppointmentStatus.Cancelled;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDoctorDropdown()
    {
        ViewBag.DoctorList = await _context.Doctors.Include(d => d.Department).ToListAsync();
        ViewBag.DepartmentList = await _context.Departments.ToListAsync();
    }
}