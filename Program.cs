using System.Net;
using System.Net.Mail;
using OttoAssistance.Helpers;
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Enable serving static files from wwwroot (or the project root)
app.UseStaticFiles();

// Map root to index.html
app.MapGet("/", async context =>
{
    context.Response.ContentType = "text/html";
    await context.Response.SendFileAsync("wwwroot/index.html");
});
app.MapPost("/submit", async (HttpContext context) =>
{
    var form = await context.Request.ReadFormAsync();
    var name = form["name"].ToString();
    var email = form["email"].ToString();
    var number = form["number"].ToString();

    // Validate fields using HelperMethods
    if (!HelperMethods.IsValidName(name))
        return Results.BadRequest(new { success = false, message = "Μη έγκυρο όνομα." });

    if (!HelperMethods.IsValidEmail(email))
        return Results.BadRequest(new { success = false, message = "Μη έγκυρο email." });

    if (!HelperMethods.IsValidGreekPhone(number))
        return Results.BadRequest(new { success = false, message = "Μη έγκυρο τηλέφωνο." });

    try
    {
        // Configure SMTP client
        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("ottoassistance.info@gmail.com", "zmjz johu xfqk dxyo"),
            EnableSsl = true,
        };

        // Generate promo code
        var promoCode = HelperMethods.GeneratePromoCode();

        // Send email to client
        var clientMail = new MailMessage
        {
            From = new MailAddress("ottoassistance.info@gmail.com", "Otto Assistance"),
            Subject = "Προσφορά του μήνα!",
            Body = $"Γεια σου {name}!!\nΛάβε κωδικό: {promoCode} με έκπτωση 10% για να το χρησιμοποιήσεις για μεταφορά ή ασφαλιστική κάλυψη.\n\nΣημείωση: ο κωδικός λήγει με το τέλος του μήνα",
            IsBodyHtml = false,
        };
        clientMail.To.Add(email);
        await smtpClient.SendMailAsync(clientMail);

        // Send admin notification
        var formattedTime = HelperMethods.GetGreeceTimeFormatted();
        var adminMail = new MailMessage
        {
            From = new MailAddress("ottoassistance.info@gmail.com", "Otto Assistance"),
            Subject = $"Εκδήλωση ενδιαφέροντος από {name}",
            Body = $"Ο χρήστης: {name} με email: {email} και αριθμό κινητού: {number} εκδήλωσε ενδιαφέρον.\n" +
                   $"Δόθηκε κωδικός προώθησης: {promoCode}\nΣτάλθηκε στις: {formattedTime}",
            IsBodyHtml = false,
        };
        adminMail.To.Add("jkirkilis@gmail.com");
        await smtpClient.SendMailAsync(adminMail);

        return Results.Json(new { success = true, message = "Θα σας έρθει ενημερωτικό email" });
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message); // log exception
        return Results.Json(new { success = false, message = "Υπήρξε πρόβλημα στην αποστολή email." });
    }
});
app.Run();