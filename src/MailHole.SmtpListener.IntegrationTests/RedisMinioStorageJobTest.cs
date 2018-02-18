using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Xunit;

namespace MailHole.SmtpListener.IntegrationTests
{
    public class RedisMinioStorageJobTest
    {
        [Fact]
        public async Task SendTestMail()
        {
            var message = new MimeMessage
            {
                Subject = "Testmail",
                Body = new TextPart("plain")
                {
                    Text = "Test content bla gna bla"
                }
            };
            
            message.From.Add(new MailboxAddress("Donald Duck", "donald.duck@mailhole.com"));
            message.To.Add(new MailboxAddress("Mickey Mouse", "mickey.mouse@waltdisney.com"));
            message.Headers["X-MAIL-GUID"] = Guid.NewGuid().ToString();

            using (var smtpClient = new SmtpClient())
            {
                await smtpClient.ConnectAsync("127.0.0.1", 1025, SecureSocketOptions.None);
                await smtpClient.SendAsync(message);
                await smtpClient.DisconnectAsync(true);
            }
        }
    }
}