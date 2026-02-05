using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ExternalServices.Email
{
    public class MailtrapService : IEmailService
    {
        private readonly MailtrapOptions _options;
        private readonly ILogger<MailtrapService> _logger;
        public MailtrapService(IOptions<MailtrapOptions> options,ILogger<MailtrapService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task SendEmailConfirmationAsync(string toEmail, string userName, string confirmationLink)
        {
            var subject = "🌍 Подтверди свой email в TravelApp";

            var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); 
                   color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .button {{ display: inline-block; padding: 15px 30px; background: #FF6B6B; 
                   color: white; text-decoration: none; border-radius: 5px; 
                   font-weight: bold; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 30px; font-size: 12px; color: #999; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🌍 TravelApp</h1>
        </div>
        <div class='content'>
            <h2>Привет, {userName}! 👋</h2>
            <p>Спасибо за регистрацию в TravelApp!</p>
            <p>Чтобы начать планировать путешествия и делиться впечатлениями, 
               подтверди свой email адрес:</p>
            
            <div style='text-align: center;'>
                <a href='{confirmationLink}' class='button'>
                    Подтвердить Email
                </a>
            </div>
            
            <p style='margin-top: 30px; font-size: 14px; color: #666;'>
                Или скопируй эту ссылку в браузер:<br>
                <code style='background: #eee; padding: 10px; display: inline-block; 
                             margin-top: 10px; word-break: break-all;'>{confirmationLink}</code>
            </p>
            
            <p style='margin-top: 30px; font-size: 14px; color: #666;'>
                ⏰ Ссылка действительна 24 часа.
            </p>
        </div>
        <div class='footer'>
            <p>Если ты не регистрировался в TravelApp, просто игнорируй это письмо.</p>
            <p>© 2025 TravelApp. Все права защищены.</p>
        </div>
    </div>
</body>
</html>";

            await SendEmailAsync(toEmail, subject, htmlBody);
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string userName)
        {
            var subject = "🎉 Добро пожаловать в TravelApp!";

            var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); 
                   color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .button {{ display: inline-block; padding: 15px 30px; background: #FF6B6B; 
                   color: white; text-decoration: none; border-radius: 5px; 
                   font-weight: bold; margin: 20px 0; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🎉 Добро пожаловать!</h1>
        </div>
        <div class='content'>
            <h2>Привет, {userName}!</h2>
            <p>Твой email подтвержден! Теперь ты можешь:</p>
            <ul>
                <li>📸 Публиковать посты о своих путешествиях</li>
                <li>🗺️ Планировать маршруты с AI-помощником</li>
                <li>👥 Общаться с другими путешественниками</li>
                <li>🔖 Создавать коллекции любимых мест</li>
            </ul>
            <p style='margin-top: 30px; text-align: center;'>
                <a href='http://localhost:3000/login' class='button'>
                    Начать путешествие
                </a>
            </p>
        </div>
    </div>
</body>
</html>";

            await SendEmailAsync(toEmail, subject, htmlBody);
        }

        private async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            try
            {
                using var smtpClient = new SmtpClient(_options.Host, _options.Port)
                {
                    Credentials = new NetworkCredential(_options.Username, _options.Password),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_options.FromEmail, _options.FromName),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);

                _logger.LogInformation($"✅ Email sent to {toEmail}: {subject}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Failed to send email to {toEmail}");
                throw;
            }
        }
    }
}
