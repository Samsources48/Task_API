using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configuration.Email
{
    public class EmailSettings
    {
        public const string SectionName = "Email";
        public string SmtpHost { get; set; } = "smtp.office365.com";
        public int SmtpPort { get; set; } = 587;
        public string SmtpUser { get; set; } = string.Empty;
        public string SmtpPassword { get; set; } = string.Empty;
        public bool UseSsl { get; set; } = true;
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = "RPA Tracking";
        public List<string> Recipients { get; set; } = new();
        public List<string> CcRecipients { get; set; } = new();
        public bool Enabled { get; set; } = true;
    }
}
