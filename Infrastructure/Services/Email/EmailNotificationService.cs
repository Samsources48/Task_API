using Infrastructure.Configuration;
using Infrastructure.Configuration.Email;
using Infrastructure.Services.Notification.Reports;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.Notification.Email
{
    public class EmailNotificationService : IEmailNotificationService
    {
        private readonly IEmailSender _emailSender;
        private readonly IExcelReportGenerator _reportGenerator;
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailNotificationService> _logger;

        public EmailNotificationService(
            IEmailSender emailSender,
            IExcelReportGenerator reportGenerator,
            IOptions<EmailSettings> settings,
            ILogger<EmailNotificationService> logger)
        {
            _emailSender = emailSender;
            _reportGenerator = reportGenerator;
            _settings = settings.Value;
            _logger = logger;
        }


        public async Task NotifyExpiringTaskAsync(
            string recipientEmail,
            string userName,
            string taskTitle,
            DateTime? dueDate,
            CancellationToken ct = default)
        {
            if (!_settings.Enabled) return;

            try
            {
                var htmlBody = $@"
                <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; max-width: 600px; margin: 20px auto; border: 1px solid #e0e0e0; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.05);'>
                    <div style='background-color: #2c3e50; color: #ffffff; padding: 25px; text-align: center;'>
                        <h1 style='margin: 0; font-size: 24px; font-weight: 300;'>Recordatorio de Tarea</h1>
                    </div>
                    <div style='padding: 30px; line-height: 1.6; color: #333;'>
                        <p style='font-size: 16px;'>Hola <strong>{userName}</strong>,</p>
                        <p>Te informamos que una de tus tareas está próxima a vencer.</p>
                        
                        <div style='background-color: #f8f9fa; border-left: 4px solid #3498db; padding: 20px; margin: 25px 0;'>
                            <p style='margin: 0 0 10px 0;'><strong>Tarea:</strong> {taskTitle}</p>
                            <p style='margin: 0;'><strong>Fecha de vencimiento:</strong> {dueDate:dd/MM/yyyy HH:mm}</p>
                        </div>
                        
                        <p>Por favor, asegúrate de completarla a tiempo para mantener tu flujo de trabajo al día.</p>
                        
                        <div style='text-align: center; margin-top: 35px;'>
                            <a href='#' style='background-color: #3498db; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block;'>Ver mis tareas</a>
                        </div>
                    </div>
                    <div style='background-color: #f1f1f1; color: #7f8c8d; padding: 15px; text-align: center; font-size: 12px;'>
                        Este es un mensaje automático del sistema Task_API. No responda a este correo.
                    </div>
                </div>";

                await _emailSender.SendEmailAsync(
                    subject: $" Tarea próxima a vencer: {taskTitle}",
                    htmlBody: htmlBody,
                    recipients: new[] { recipientEmail },
                    ct: ct);

                _logger.LogInformation("Notificación de vencimiento enviada a {Email} para la tarea {Task}", recipientEmail, taskTitle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando notificación de vencimiento para {Task}", taskTitle);
            }
        }
    }
}
