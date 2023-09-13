using Donace_BE_Project.Interfaces;
using MimeKit;
using MailKit.Net.Smtp;

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
                var server = _configuration["Smtp:Server"];
                var password = _configuration["Smtp:Password"];

                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("Donace Ticks", userName));

                emailMessage.To.Add(MailboxAddress.Parse(email));
                emailMessage.Subject = subject;

                var textPart = new TextPart("plain")
                {
                    Text = body
                };

                var multipart = new Multipart("mixed");
                multipart.Add(textPart);

                emailMessage.Body = multipart;

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(server, 587, false);
                    await client.AuthenticateAsync(userName, password);
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
