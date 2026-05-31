# Advanced Clinic Management System

**SE356 — Final Project**

Authors:
- Yusuf Talha Kamiloğlu — 220706006
- Hakan Ege Sarıçayır — 220706031


A role-based clinic management web application built with **ASP.NET Core MVC (.NET 8)**.
This is the improved and professionalized final version of the midterm project, migrated
from Razor Pages + MySQL to a layered MVC architecture backed by **PostgreSQL (Supabase)**.

## Roles
- **Admin** — full access, user & role management
- **Receptionist** — patients, doctors, departments, appointments, reports
- **Doctor** — *(role defined in the system)*
- **Patient** — self-service portal: book / cancel appointments, view doctors, manage profile

## Features
- Authentication & role-based authorization (ASP.NET Core Identity)
- Patient portal (register, view doctors, book/cancel, appointment history, profile)
- Appointment management with status tracking (Pending / Approved / Completed / Cancelled)
- Booking rules: no past dates, 08:00–17:00 quarter-hour slots, no double-booking per doctor
- Dashboard with summary cards and charts (Chart.js)
- Reports: by doctor, by department, monthly statistics, patient history
- Search & filtering (patients, doctors, appointments, users)
- Export appointments to **Excel** (ClosedXML) and **PDF** (QuestPDF)
- Light / dark mode
- Responsive, custom-themed UI

## Tech Stack
- ASP.NET Core MVC (.NET 8), C#
- Entity Framework Core + Npgsql
- PostgreSQL (Supabase)
- ASP.NET Core Identity
- HTML / CSS / JavaScript, Chart.js, Bootstrap Icons

## Project Structure
- `ClinicManagementSystem/` — the MVC application (Controllers, Models, Views, Data)
- `database_dump.sql` — full database export (schema + data)
- `database_schema.sql` — schema-only export
- `screenshots/` — system screenshots

## Running Locally
1. Install the .NET 8 SDK.
2. Set the database connection string as a user secret:
```bash
   cd ClinicManagementSystem
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your-postgresql-connection-string>"
```
3. Run:
```bash
   dotnet run
```
4. A default admin account is seeded on first run: `admin@clinic.com` / `Admin123!`