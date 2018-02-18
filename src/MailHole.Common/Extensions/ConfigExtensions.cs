using System;
using System.Text.RegularExpressions;
using MailHole.Common.Model.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MailHole.Common.Extensions
{
    public static class ConfigExtensions
    {
        private const string RedisConfigSectionKey = "REDIS";
        private const string SmtpConfigSectionKey = "SMTP";
        private const string HangfireConfigSectionKey = "HANGFIRE";
        private const string MinioConfigSectionKey = "MINIO";

        public static IServiceCollection ConfigureMailHoleOptions(this IServiceCollection serviceCollection,
            IConfiguration configuration)
        {
            return serviceCollection
                .AddOptions()
                .Configure<RedisOptions>(redisOpts =>
                    configuration.GetSection(RedisConfigSectionKey).Bind(redisOpts)
                )
                .Configure<SmtpOptions>(smtpOpts =>
                    configuration.GetSection(SmtpConfigSectionKey).Bind(smtpOpts)
                )
                .Configure<HangfireOptions>(hangfireOpts => configuration.GetSection(HangfireConfigSectionKey).Bind(hangfireOpts))
                .Configure<MinioOptions>(minioOpts => configuration.GetSection(MinioConfigSectionKey).Bind(minioOpts));
        }

        public static SmtpOptions GetSmtpOptionsOrDefault(this IServiceProvider serviceProvider)
        {
            var iOptions = serviceProvider.GetService<IOptions<SmtpOptions>>();
            return iOptions.Value ?? new SmtpOptions();
        }

        public static SmtpOptions BindSmtpOptions(this IConfiguration configuration, SmtpOptions smtpOptions = null)
        {
            var options = smtpOptions ?? new SmtpOptions();
            configuration.GetSection(SmtpConfigSectionKey).Bind(options);
            return options;
        }

        public static RedisOptions GetRedisOptionsOrDefault(this IServiceProvider serviceProvider)
        {
            var iOptions = serviceProvider.GetService<IOptions<RedisOptions>>();
            return iOptions.Value ?? new RedisOptions();
        }

        public static RedisOptions BindRedisOptions(this IConfiguration configuration, RedisOptions redisOptions = null)
        {
            var options = redisOptions ?? new RedisOptions();
            configuration.GetSection(RedisConfigSectionKey).Bind(options);
            return options;
        }

        public static HangfireOptions GetHangfireOptionsOrDefault(this IServiceProvider serviceProvider)
        {
            var iOptions = serviceProvider.GetService<IOptions<HangfireOptions>>();
            return iOptions.Value ?? new HangfireOptions();
        }

        public static HangfireOptions BindHangfireOptions(this IConfiguration configuration,
            HangfireOptions existingOptions = null)
        {
            var options = existingOptions ?? new HangfireOptions();
            configuration.GetSection(HangfireConfigSectionKey).Bind(options);
            return options;
        }

        public static MinioOptions GetMinioOptionsOrDefault(this IServiceProvider serviceProvider)
        {
            var iOptions = serviceProvider.GetService<IOptions<MinioOptions>>();
            return iOptions.Value ?? new MinioOptions();
        }

        public static MinioOptions BindMinioOptions(this IConfiguration configuration, MinioOptions minioOptions = null)
        {
            var options = minioOptions ?? new MinioOptions();
            configuration.GetSection(MinioConfigSectionKey).Bind(options);
            return options;
        }
    }
}