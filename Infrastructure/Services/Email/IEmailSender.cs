namespace Infrastructure.Services.Notification.Email
{

    public interface IEmailSender
    {

        Task SendEmailAsync(
            string subject,
            string htmlBody,
            IEnumerable<string> recipients,
            IEnumerable<string>? ccRecipients = null,
            IEnumerable<EmailAttachment>? attachments = null,
            CancellationToken ct = default);
    }

    public class EmailAttachment
    {
        public string FileName { get; set; } = string.Empty;
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = "application/octet-stream";
    }
}
