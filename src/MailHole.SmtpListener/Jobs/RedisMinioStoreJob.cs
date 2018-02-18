using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailHole.Common.Model;
using MailHole.SmtpListener.Extensions;
using MimeKit;
using Newtonsoft.Json;
using SmtpServer;
using SmtpServer.Mail;
using StackExchange.Redis;

namespace MailHole.SmtpListener.Jobs
{
    public class RedisMinioStoreJob
    {
        private readonly IDatabase _redisDb;

        public RedisMinioStoreJob(IDatabase redisDb)
        {
            _redisDb = redisDb;
        }

        public async Task StoreMail(List<string> to, string mailGuid, ReceivedMail receivedMail)
        {
            try
            {
                Console.Out.WriteLine("storing the mail...not!");
            
                foreach (var mailbox in to)
                {
                    await _redisDb.HashSetAsync(mailbox, mailGuid, JsonConvert.SerializeObject(receivedMail));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}