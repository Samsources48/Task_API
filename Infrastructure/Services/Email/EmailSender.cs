using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Infrastructure.Configuration.Email;

namespace Infrastructure.Services.Notification.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IOptions<EmailSettings> settings, ILogger<EmailSender> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(
            string subject,
            string htmlBody,
            IEnumerable<string> recipients,
            IEnumerable<string>? ccRecipients = null,
            IEnumerable<EmailAttachment>? attachments = null,
            CancellationToken ct = default)
        {
            if (!_settings.Enabled)
            {
                _logger.LogInformation("Email deshabilitado por configuracion. Asunto omitido: {Subject}", subject);
                return;
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));

            foreach (var to in recipients)
                message.To.Add(MailboxAddress.Parse(to));

            if (ccRecipients != null)
            {
                foreach (var cc in ccRecipients)
                    message.Cc.Add(MailboxAddress.Parse(cc));
            }

            message.Subject = subject;

            // Construir cuerpo con adjuntos usando BodyBuilder de MimeKit
            var builder = new BodyBuilder { HtmlBody = htmlBody };

            if (attachments is not null)
            {
                foreach (var attachment in attachments)
                {
                    builder.Attachments.Add(
                        attachment.FileName,
                        attachment.Content,
                        ContentType.Parse(attachment.ContentType));
                }
            }

            message.Body = builder.ToMessageBody();

            // Enviar vía SMTP con MailKit
            using var client = new SmtpClient();
            try
            {
                var secureOption = _settings.UseSsl
                    ? SecureSocketOptions.StartTls
                    : SecureSocketOptions.Auto;

                await client.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, secureOption, ct);
                await client.AuthenticateAsync(_settings.SmtpUser, _settings.SmtpPassword, ct);
                await client.SendAsync(message, ct);

                _logger.LogInformation(
                    "Email enviado exitosamente: {Subject} -> {Recipients}",
                    subject, string.Join(", ", recipients));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando email: {Subject}", subject);
                throw;
            }
            finally
            {
                await client.DisconnectAsync(true, ct);
            }
        }
    }
}
