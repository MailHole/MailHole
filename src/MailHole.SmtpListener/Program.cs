using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Redis;
using MailHole.Common.Extensions;
using MailHole.Common.HangfireExtensions;
using MailHole.SmtpListener.Jobs;
using MailHole.SmtpListener.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmtpServer;
using StackExchange.Redis;

namespace MailHole.SmtpListener
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            var serviceProvider = BootstrapDependencyInjection(config);

            var hangfireJobStorage = new RedisStorage(serviceProvider.GetService<ConnectionMultiplexer>(),
                new RedisStorageOptions
                {
                    Db = config.GetRedisDatabaseNumber(),
                    Prefix = "Hangfire"
                });

            using (var server = new BackgroundJobServer(new BackgroundJobServerOptions
            {
                Activator = new HangfireActivator(serviceProvider),
                WorkerCount = 5
            }, hangfireJobStorage))
            {

                var backgroundJobClient = new BackgroundJobClient(hangfireJobStorage);
                Console.WriteLine($"Listening on Ports {string.Join(", ", config.GetSmtpPorts())}");
                var options = new OptionsBuilder()
                    .ServerName(config.GetSmtpHostName())
                    .Port(config.GetSmtpPorts())
                    .MessageStore(new RedisMinioMailStore(backgroundJobClient))
                    .Build();

                var smtpServer = new SmtpServer.SmtpServer(options);
                await smtpServer.StartAsync(CancellationToken.None);
                server.SendStop();
            }
        }

        private static IServiceProvider BootstrapDependencyInjection(IConfiguration configuration)
        {
            return new ServiceCollection()
                .AddSingleton(provider => ConnectionMultiplexer.Connect(configuration.GetRedisConnectionString()))
                .AddTransient(provider =>
                    provider.GetService<ConnectionMultiplexer>().GetDatabase(configuration.GetRedisDatabaseNumber()))
                .AddTransient(provider => new RedisMinioStoreJob(provider.GetService<IDatabase>()))
                .BuildServiceProvider();
        }
    }
}