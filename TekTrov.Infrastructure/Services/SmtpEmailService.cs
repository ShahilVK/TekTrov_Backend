using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using TekTrov.Application.DTOs;
using TekTrov.Application.Interfaces.Services;

namespace TekTrov.Infrastructure.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public SmtpEmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            var message = new MailMessage
            {
                From = new MailAddress(_settings.From),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(to);

            using var smtp = new SmtpClient
            {
                Host = _settings.Host,
                Port = _settings.Port,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
         _settings.Username,
         _settings.Password
     )
            };


            await smtp.SendMailAsync(message);
        }
    }
}
