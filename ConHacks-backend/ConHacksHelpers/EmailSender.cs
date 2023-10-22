using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace ConHacksHelpers;

public class EmailSender : IEmailSender
{
    private readonly string _email;
    private readonly string _password;
    private readonly string _smtpHost;
    private readonly int _smtpPort;

    public EmailSender(string email, string password, string smtpHost, int smtpPort)
    {
        _email = email;
        _password = password;
        _smtpHost = smtpHost;
        _smtpPort = smtpPort;
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        using (var client = new SmtpClient(_smtpHost, _smtpPort))
        {
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(_email, _password);
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;

            using (var mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(_email);
                mailMessage.To.Add(email);
                mailMessage.Subject = subject;
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = htmlMessage;

                try
                {
                    client.Send(mailMessage);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error sending email: " + ex.Message);
                }
            }
        }

        return Task.CompletedTask;
    }
}