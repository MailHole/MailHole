namespace MailHole.Common.Model.Options
{
    public class RedisOptions
    {
        public string ConnectionString { get; set; } = "redis:6379";

        public ushort DatabaseIndex { get; set; }
    }
}