using Donace_BE_Project.Exceptions;
using Donace_BE_Project.Interfaces.Services;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Hosting;
using MimeKit;
using System.Net.Mail;
using System.Net;

namespace Donace_BE_Project.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(string email, string subject, string body)
        {
            try
            {
                var userName = _configuration["Smtp:Username"];
                var host = _configuration["Smtp:Server"];
                var password = _configuration["Smtp:Password"];

                var smtpClient = new System.Net.Mail.SmtpClient
                {
                    Host = host, // set your SMTP server name here
                    Port = 587, // Port 
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential()
                    {
                        UserName = userName,
                        Password = password
                    }
                };

                using var message = new MailMessage(userName, email)
                {
                    Subject = subject,
                    Body = body,
                };
                message.From = new MailAddress(userName, "Donace Ticket");
                await smtpClient.SendMailAsync(message);
            }
            catch (FriendlyException ex)
            {

            }
        }
    }
}
