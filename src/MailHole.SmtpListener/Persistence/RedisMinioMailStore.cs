using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MailHole.SmtpListener.Extensions;
using MimeKit;
using Newtonsoft.Json;
using SmtpServer;
using SmtpServer.Mail;
using SmtpServer.Protocol;
using SmtpServer.Storage;
using StackExchange.Redis;

namespace MailHole.SmtpListener.Persistence
{
    public class RedisMinioMailStore : MessageStore
    {

        private readonly int _databaseNumber;
        private readonly IConnectionMultiplexer _redis;

        public RedisMinioMailStore(string connectionString, int databaseNumber)
        {
            _databaseNumber = databaseNumber;
            _redis = ConnectionMultiplexer.Connect(connectionString);
        }

        public override async Task<SmtpResponse> SaveAsync(ISessionContext context, IMessageTransaction transaction, CancellationToken cancellationToken)
        {
            var mailGuid = Guid.NewGuid().ToString();
            var textMessage = (ITextMessage) transaction.Message;
            var message = MimeMessage.Load(textMessage.Content);
            var attachements = message.Attachments.ToList();
            foreach (var attachement in attachements)
            {
                //TODO write files to temp directory for further uploads
                //attachement.WriteToAsync()
            }
            var entity = message.ToReceivedMail();
            var db = _redis.GetDatabase(_databaseNumber);
            foreach (var mailbox in transaction.To)
            {
                await db.HashSetAsync(mailbox.AsAddress(), mailGuid, JsonConvert.SerializeObject(entity));
            }
            return SmtpResponse.Ok;
        }
    }
}