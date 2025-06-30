using System.Net;
using System.Net.Mail;

namespace MovieApp.Services;
public interface IEmailService
{
    Task SendEmail(string receptor, string verificationCode, string userName);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration configuration;

    public EmailService(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public async Task SendEmail(string receptor, string verificationCode, string userName)
    {
        var email = configuration.GetValue<string>("EMAIL_CONFIGURATION:EMAIL") ?? string.Empty;
        var password = configuration.GetValue<string>("EMAIL_CONFIGURATION:PASSWORD");
        var host = configuration.GetValue<string>("EMAIL_CONFIGURATION:HOST");
        var port = configuration.GetValue<int>("EMAIL_CONFIGURATION:PORT");
        var smtpClient = new SmtpClient(host, port);
        smtpClient.EnableSsl = true;
        smtpClient.UseDefaultCredentials = false;

        smtpClient.Credentials = new NetworkCredential(email, password);

        string Subject = "Your Verification Code";
        string Body = $"Hi [{userName}],\r\n\r\nYour verification code is: {verificationCode}\r\n\r\nPlease enter this code to complete your verification process. This code will expire in 5 minutes.\r\n\r\nIf you didn’t request this code, please ignore this email.\r\n\r\nThanks,\r\n[Todo App] Team";

        var message = new MailMessage(email, receptor, Subject, Body);
        await smtpClient.SendMailAsync(message);
    }
}