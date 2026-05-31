using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClinicManagementSystem.Data;
using ClinicManagementSystem.Models;

namespace ClinicManagementSystem.Controllers;
using Microsoft.AspNetCore.Authorization;
[Authorize(Roles = "Admin,Receptionist")]
public class AppointmentsController : Controller
{
    private readonly AppDbContext _context;
    public AppointmentsController(AppDbContext context) { _context = context; }

    public async Task<IActionResult> Index()
    {
        var appointments = await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();
        return View(appointments);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateDropdowns();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Appointment appointment)
    {
        if (ModelState.IsValid)
        {
            appointment.Status = AppointmentStatus.Pending; // yeni randevu her zaman Pending
            appointment.AppointmentDate = DateTime.SpecifyKind(appointment.AppointmentDate, DateTimeKind.Utc);
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        await PopulateDropdowns(appointment);
        return View(appointment);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null) return NotFound();
        await PopulateDropdowns(appointment);
        return View(appointment);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Appointment appointment)
    {
        if (id != appointment.AppointmentID) return NotFound();
        if (ModelState.IsValid)
        {
            appointment.AppointmentDate = DateTime.SpecifyKind(appointment.AppointmentDate, DateTimeKind.Utc);
            _context.Update(appointment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        await PopulateDropdowns(appointment);
        return View(appointment);
    }

    // Listede durum rozetinden hızlı durum değiştirme
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeStatus(int id, AppointmentStatus status)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment != null)
        {
            appointment.Status = status;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Patient).Include(a => a.Doctor)
            .FirstOrDefaultAsync(a => a.AppointmentID == id);
        if (appointment == null) return NotFound();
        return View(appointment);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment != null)
        {
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdowns(Appointment? appointment = null)
    {
        var patients = await _context.Patients.ToListAsync();
        ViewBag.Patients = new SelectList(
            patients.Select(p => new { p.PatientID, FullName = p.FirstName + " " + p.LastName }),
            "PatientID", "FullName", appointment?.PatientID);

        ViewBag.DoctorList = await _context.Doctors.Include(d => d.Department).ToListAsync();
        ViewBag.DepartmentList = await _context.Departments.ToListAsync();
    }
}