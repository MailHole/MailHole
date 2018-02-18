using System;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Redis;
using MailHole.Common.Extensions;
using MailHole.Common.HangfireExtensions;
using MailHole.Common.Model.Options;
using MailHole.SmtpListener.Jobs;
using MailHole.SmtpListener.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
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
                .SetBasePath(PlatformServices.Default.Application.ApplicationBasePath)
                .AddJsonFile($"appsettings.{Environment.UserName}.json", true)
                .AddJsonFile("appsettings.Development.json", true)
                .AddJsonFile("appsettings.json")
                .Build();

            var serviceProvider = BootstrapDependencyInjection(config);
            var hangfireOptions = serviceProvider.GetHangfireOptionsOrDefault();
            var hangfireJobStorage = CreateRedisJobStorage(hangfireOptions);

            using (var server = CreateHangfireBackgroundJobServer(hangfireJobStorage, new HangfireActivator(serviceProvider)))
            {
                var smtpOptions = serviceProvider.GetSmtpOptionsOrDefault();
                var backgroundJobClient = new BackgroundJobClient(hangfireJobStorage);
                var options = new OptionsBuilder()
                    .ServerName(smtpOptions.HostName)
                    .Port(smtpOptions.Port)
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
                .ConfigureMailHoleOptions(configuration)
                .AddSingleton(provider =>
                    ConnectionMultiplexer.Connect(provider.GetRedisOptionsOrDefault().ConnectionString)
                )
                .AddTransient(provider =>
                    provider.GetService<ConnectionMultiplexer>()
                        .GetDatabase(provider.GetRedisOptionsOrDefault().DatabaseIndex)
                )
                .AddTransient(provider => new RedisMinioStoreJob(provider.GetService<IDatabase>()))
                .BuildServiceProvider();
        }

        private static JobStorage CreateRedisJobStorage(HangfireOptions hangfireOptions)
        {
            return new RedisStorage(hangfireOptions.RedisConnectionString,
                new RedisStorageOptions
                {
                    Db = hangfireOptions.RedisDatabaseIndex,
                    Prefix = hangfireOptions.RedisPrefix
                });
        }

        private static BackgroundJobServer CreateHangfireBackgroundJobServer(JobStorage storage, JobActivator activator)
        {
            return new BackgroundJobServer(new BackgroundJobServerOptions
            {
                Activator = activator
            }, storage);
        }
    }
}