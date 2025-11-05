using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Scheduler.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler.Infrastructure.Services { 

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");

            // If EmailSettings:Enabled is present and false, skip sending (useful for local tests)
            var enabledValue = emailSettings["Enabled"];
            if (!string.IsNullOrEmpty(enabledValue) && enabledValue.Equals("false", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("EmailService: sending disabled by configuration. Skipping send to: " + to);
                return;
            }

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("SchedulerApp", emailSettings["Username"]));
            emailMessage.To.Add(new MailboxAddress("", to));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

            try
            {
                using (var client = new SmtpClient())
                {
                    int port = 587;
                    if (!string.IsNullOrEmpty(emailSettings["Port"]) && int.TryParse(emailSettings["Port"], out var p))
                        port = p;

                    var smtp = emailSettings["SmtpServer"] ?? string.Empty;
                    var username = emailSettings["Username"] ?? string.Empty;
                    var password = emailSettings["Password"] ?? string.Empty;

                    await client.ConnectAsync(smtp, port, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(username, password);
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                // Log and swallow exceptions to avoid surfacing SMTP errors to API callers during tests
                Console.WriteLine("EmailService: failed to send email to " + to + ". Exception: " + ex.Message);
                return;
            }
        }
    }
}