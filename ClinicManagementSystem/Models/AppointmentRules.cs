namespace ClinicManagementSystem.Models;

public static class AppointmentRules
{
    // Geçerliyse null, geçersizse hata mesajı döndürür.
    public static string? Validate(DateTime date)
    {
        if (date < DateTime.Now)
            return "Randevu tarihi yanlış: geçmiş bir tarih/saat seçilemez. Lütfen ileri bir zaman seçin.";

        var time = date.TimeOfDay;
        if (time < new TimeSpan(8, 0, 0) || time > new TimeSpan(17, 0, 0))
            return "Randevu saati yalnızca 08:00 ile 17:00 arasında olabilir.";

        if (date.Minute % 15 != 0 || date.Second != 0)
            return "Randevu saati 15 dakikalık dilimlerde olmalıdır (örn. 15:00, 15:15, 15:30, 15:45).";

        return null;
    }
}