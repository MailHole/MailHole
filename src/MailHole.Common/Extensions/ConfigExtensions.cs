using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;

namespace MailHole.Common.Extensions
{
    public static class ConfigExtensions
    {
        private const string SmtpHostKey = "SMTP_HOST_NAME";
        private const string SmtpPortsKey = "SMTP_PORTS";
        private const string RedisConnectionStringKey = "REDIS_CONNECTION";
        private const string RedisDatabaseNumberKey = "REDIS_DB_INDEX";
        
        private static readonly Regex NumberRegex = new Regex("^\\d+$");

        public static string GetSmtpHostName(this IConfiguration configurationRoot) =>
            configurationRoot[SmtpHostKey];

        public static int[] GetSmtpPorts(this IConfiguration configurationRoot)
        {
            var portsString = configurationRoot[SmtpPortsKey];
            if (string.IsNullOrEmpty(portsString)) return new[] {25};
            return portsString.Split(",")
                .Where(s => NumberRegex.IsMatch(s))
                .Select(int.Parse)
                .ToArray();
        }

        public static string GetRedisConnectionString(this IConfiguration configurationRoot) =>
            configurationRoot[RedisConnectionStringKey];

        public static int GetRedisDatabaseNumber(this IConfiguration configurationRoot)
        {
            var numberString = configurationRoot[RedisDatabaseNumberKey];
            if (!string.IsNullOrEmpty(numberString) && int.TryParse(numberString, out var number)) return number;
            return 0;
        }
    }
}