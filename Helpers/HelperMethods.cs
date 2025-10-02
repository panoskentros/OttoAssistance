namespace OttoAssistance.Helpers;

using System;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;

public static class HelperMethods
{
    private static readonly Random _random = new Random();

    /// <summary>
    /// Validate a name: letters (including accented), spaces, hyphens, apostrophes.
    /// </summary>
    public static bool IsValidName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        var regex = new Regex(@"^[\p{L} \-']+$");
        return regex.IsMatch(name);
    }

    /// <summary>
    /// Validate an email using System.Net.Mail.MailAddress
    /// </summary>
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Validate Greek phone number (landline or mobile)
    /// </summary>
    public static bool IsValidGreekPhone(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            return false;

        // Optional +30, landlines (2XXXXXXXXX), mobiles (69XXXXXXXX)
        var regex = new Regex(@"^(?:\+30)?(2\d{9}|69\d{8})$");
        return regex.IsMatch(number);
    }

    /// <summary>
    /// Generate a random alphanumeric promo code of given length
    /// </summary>
    public static string GeneratePromoCode(int length = 5)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Range(0, length)
            .Select(_ => chars[_random.Next(chars.Length)]).ToArray());
    }

    /// <summary>
    /// Get current Greece local time formatted as "dd/MM/yyyy HH:mm"
    /// </summary>
    public static string GetGreeceTimeFormatted()
    {
        var greeceTimeZone = TimeZoneInfo.FindSystemTimeZoneById("GTB Standard Time");
        var greeceTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, greeceTimeZone);
        return greeceTime.ToString("dd/MM/yyyy HH:mm");
    }
}
