using System.Net;
using System.Net.Mail;

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
    var message = form["message"].ToString();

    if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name) ||
        string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email) ||
        string.IsNullOrEmpty(message) || string.IsNullOrWhiteSpace(message))
    {
        return Results.BadRequest(new 
        { 
            success = false, 
            message = "Λείπουν απαιτούμενα πεδία." 
        });
    }
    try
    {
        // Configure SMTP client
        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587, 
            //Credentials = new NetworkCredential("panoskentros", "mrtc pcdn kpmu mvsm"),
            Credentials = new NetworkCredential("ottoassistance.info@gmail.com", "zmjz johu xfqk dxyo"),
            EnableSsl = true,
        };

        // Create the email
        var mailMessage = new MailMessage
        {
            //From = new MailAddress("panoskentros@gmail.com", "Otto Assistance TEST"),
            From = new MailAddress("ottoassistance.info@gmail.com", "Otto Assistance"),
            Subject = "Προσφορά",
            Body = $"Γεία σου: {name}!!\nΛάβε κωδικό με έκπτωση 10% για να το χρησιμοποιήσεις για μεταφορά ή ασφαλιστική κάλυψη.",
            IsBodyHtml = false,
        };

        // Recipient
        mailMessage.To.Add(email);
        //mailMessage.To.Add("jkirkilis@gmail.com");

        // Send email
        await smtpClient.SendMailAsync(mailMessage);

        return Results.Json(new 
        { 
            success = true, 
            message = "Θα σας έρθει ενημερωτικό email" 
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message); // log exception
        return Results.Json(new 
        { 
            success = false, 
            message = "Υπήρξε πρόβλημα στην αποστολή email." 
        });
    }
    return Results.Json(new { success = true, message = "Θα σας έρθει ενημερωτικό email" });
});
app.Run();