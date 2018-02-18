using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using MailHole.SmtpListener.Extensions;
using MailHole.SmtpListener.Jobs;
using MimeKit;
using SmtpServer;
using SmtpServer.Mail;
using SmtpServer.Protocol;
using SmtpServer.Storage;

using static MailHole.Common.MailHoleConstants;

namespace MailHole.SmtpListener.Persistence
{
    public class RedisMinioMailStore : MessageStore
    {

        private readonly IBackgroundJobClient _backgroundJobClient;

        public RedisMinioMailStore(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
        }

        public override async Task<SmtpResponse> SaveAsync(ISessionContext context, IMessageTransaction transaction, CancellationToken cancellationToken)
        {
            try
            {
                var textMessage = (ITextMessage) transaction.Message;
                var message = MimeMessage.Load(textMessage.Content);
                var entity = message.ToReceivedMail();
                var mailGuid = entity.Headers.ContainsKey(MailGuidHeader)
                    ? entity.Headers[MailGuidHeader]
                    : Guid.NewGuid().ToString();
                foreach (var mimeEntity in message.Attachments)
                {
                    /* TODO store attachements as tmp files and load them to minio */
                }
                
                
                _backgroundJobClient.Enqueue<RedisMinioStoreJob>(rmsj => rmsj.StoreMail(transaction.To.Select(im => im.AsAddress()).ToList(), mailGuid, entity));
                return SmtpResponse.Ok;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return SmtpResponse.MailboxUnavailable;
            }
        }
    }
}