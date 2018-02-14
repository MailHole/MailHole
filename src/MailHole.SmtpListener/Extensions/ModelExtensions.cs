using System.Linq;
using MailHole.Common.Model;
using MimeKit;

namespace MailHole.SmtpListener.Extensions
{
    public static class ModelExtensions
    {
        public static ReceivedMail ToReceivedMail(this MimeMessage mimeMessage)
        {
            return new ReceivedMail
            {
                Sender = mimeMessage.From.FirstOrDefault()?.ToString(),
                Subject = mimeMessage.Subject,
                HtmlBody = mimeMessage.HtmlBody,
                TextBody = mimeMessage.TextBody,
                Headers = mimeMessage.Headers.ToDictionary(header => header.Field, header => header.Value),
                UtcReceivedTime = mimeMessage.Date.UtcDateTime,
                Bcc = mimeMessage.Bcc.Select(inetAddr => inetAddr.Name).ToList()
            };
        }
    }
}