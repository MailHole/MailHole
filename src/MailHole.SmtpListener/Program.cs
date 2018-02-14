using System.Threading;
using System.Threading.Tasks;
using MailHole.Common.Extensions;
using MailHole.SmtpListener.Persistence;
using Microsoft.Extensions.Configuration;
using SmtpServer;

namespace MailHole.SmtpListener
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            var options = new OptionsBuilder()
                .ServerName(config.GetSmtpHostName())
                .Port(config.GetSmtpPorts())
                .MessageStore(new RedisMinioMailStore(config.GetRedisConnectionString(), config.GetRedisDatabaseNumber()))
                .Build();

            var smtpServer = new SmtpServer.SmtpServer(options);
            await smtpServer.StartAsync(CancellationToken.None);
        }
    }
}