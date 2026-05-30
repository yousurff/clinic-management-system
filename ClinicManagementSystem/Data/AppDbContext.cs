using Microsoft.EntityFrameworkCore;
using ClinicManagementSystem.Models;

namespace ClinicManagementSystem.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Patient> Patients => Set<Patient>();
}