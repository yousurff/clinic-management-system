using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClinicManagementSystem.Data;
using ClinicManagementSystem.Models;

namespace ClinicManagementSystem.Controllers;
using Microsoft.AspNetCore.Authorization;
[Authorize(Roles = "Admin,Receptionist")]
public class DoctorsController : Controller
{
    private readonly AppDbContext _context;
    public DoctorsController(AppDbContext context) { _context = context; }

    public async Task<IActionResult> Index()
    {
        var doctors = await _context.Doctors.Include(d => d.Department).ToListAsync();
        return View(doctors);
    }

    public IActionResult Create()
    {
        ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Doctor doctor)
    {
        if (ModelState.IsValid)
        {
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "Name", doctor.DepartmentID);
        return View(doctor);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var doctor = await _context.Doctors.FindAsync(id);
        if (doctor == null) return NotFound();
        ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "Name", doctor.DepartmentID);
        return View(doctor);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Doctor doctor)
    {
        if (id != doctor.DoctorID) return NotFound();
        if (ModelState.IsValid)
        {
            _context.Update(doctor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "Name", doctor.DepartmentID);
        return View(doctor);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var doctor = await _context.Doctors.Include(d => d.Department).FirstOrDefaultAsync(d => d.DoctorID == id);
        if (doctor == null) return NotFound();
        return View(doctor);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var doctor = await _context.Doctors.FindAsync(id);
        if (doctor != null)
        {
            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}