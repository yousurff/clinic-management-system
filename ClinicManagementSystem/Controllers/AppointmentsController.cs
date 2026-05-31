using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClinicManagementSystem.Data;
using ClinicManagementSystem.Models;
using ClosedXML.Excel;
using QuestPDF.Fluent;

namespace ClinicManagementSystem.Controllers;
using Microsoft.AspNetCore.Authorization;
[Authorize(Roles = "Admin,Receptionist")]
public class AppointmentsController : Controller
{
    private readonly AppDbContext _context;
    public AppointmentsController(AppDbContext context) { _context = context; }

    public async Task<IActionResult> Index(DateTime? date, int? departmentId, AppointmentStatus? status)
    {
        var query = _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor).ThenInclude(d => d.Department)
            .AsQueryable();

        if (date.HasValue)
        {
            var start = DateTime.SpecifyKind(date.Value.Date, DateTimeKind.Utc);
            var end = start.AddDays(1);
            query = query.Where(a => a.AppointmentDate >= start && a.AppointmentDate < end);
        }
        if (departmentId.HasValue)
            query = query.Where(a => a.Doctor!.DepartmentID == departmentId.Value);
        if (status.HasValue)
            query = query.Where(a => a.Status == status.Value);

        ViewBag.Date = date?.ToString("yyyy-MM-dd");
        ViewBag.DepartmentId = departmentId;
        ViewBag.Status = status;
        ViewBag.Departments = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
            await _context.Departments.ToListAsync(), "DepartmentID", "Name", departmentId);

        var appointments = await query.OrderByDescending(a => a.AppointmentDate).ToListAsync();
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
    
    public async Task<IActionResult> ExportExcel()
    {
        var appts = await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor).ThenInclude(d => d.Department)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Randevular");
        ws.Cell(1, 1).Value = "ID";
        ws.Cell(1, 2).Value = "Hasta";
        ws.Cell(1, 3).Value = "Doktor";
        ws.Cell(1, 4).Value = "Bölüm";
        ws.Cell(1, 5).Value = "Tarih";
        ws.Cell(1, 6).Value = "Durum";

        var row = 2;
        foreach (var a in appts)
        {
            ws.Cell(row, 1).Value = a.AppointmentID;
            ws.Cell(row, 2).Value = $"{a.Patient?.FirstName} {a.Patient?.LastName}";
            ws.Cell(row, 3).Value = $"{a.Doctor?.FirstName} {a.Doctor?.LastName}";
            ws.Cell(row, 4).Value = a.Doctor?.Department?.Name ?? "";
            ws.Cell(row, 5).Value = a.AppointmentDate.ToString("dd.MM.yyyy HH:mm");
            ws.Cell(row, 6).Value = a.Status.ToString();
            row++;
        }
        ws.Row(1).Style.Font.Bold = true;
        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        wb.SaveAs(stream);
        return File(stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "randevular.xlsx");
    }

    public async Task<IActionResult> ExportPdf()
    {
        var appts = await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor).ThenInclude(d => d.Department)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();

        var bytes = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Header().Text("Randevu Listesi").FontSize(18).SemiBold();
                page.Content().PaddingVertical(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(30);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn();
                    });
                    table.Header(header =>
                    {
                        header.Cell().Text("ID");
                        header.Cell().Text("Hasta");
                        header.Cell().Text("Doktor");
                        header.Cell().Text("Bölüm");
                        header.Cell().Text("Tarih");
                        header.Cell().Text("Durum");
                    });
                    foreach (var a in appts)
                    {
                        table.Cell().Text(a.AppointmentID.ToString());
                        table.Cell().Text($"{a.Patient?.FirstName} {a.Patient?.LastName}");
                        table.Cell().Text($"{a.Doctor?.FirstName} {a.Doctor?.LastName}");
                        table.Cell().Text(a.Doctor?.Department?.Name ?? "");
                        table.Cell().Text(a.AppointmentDate.ToString("dd.MM.yyyy HH:mm"));
                        table.Cell().Text(a.Status.ToString());
                    }
                });
                page.Footer().AlignCenter().Text($"Klinik YS — {DateTime.Now:dd.MM.yyyy}");
            });
        }).GeneratePdf();

        return File(bytes, "application/pdf", "randevular.pdf");
    }
}