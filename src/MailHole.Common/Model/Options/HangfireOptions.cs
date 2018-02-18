namespace MailHole.Common.Model.Options
{
    public class HangfireOptions
    {
        public string RedisConnectionString { get; set; } = "redis:6379";

        public ushort RedisDatabaseIndex { get; set; } = 1;

        public string RedisPrefix { get; set; } = "Hangfire";
    }
}