using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicManagementSystem.Data;
using ClinicManagementSystem.Models;

namespace ClinicManagementSystem.Controllers;

public class PatientsController : Controller
{
    private readonly AppDbContext _context;

    public PatientsController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Patients.ToListAsync());
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Patient patient)
    {
        if (ModelState.IsValid)
        {
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(patient);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null) return NotFound();
        return View(patient);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Patient patient)
    {
        if (id != patient.PatientID) return NotFound();
        if (ModelState.IsValid)
        {
            _context.Update(patient);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(patient);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null) return NotFound();
        return View(patient);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient != null)
        {
            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}