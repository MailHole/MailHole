namespace MailHole.Common.Model.Options
{
    public class SmtpOptions
    {
        public string HostName { get; set; } = "localhost";
        public ushort Port { get; set; } = 25;
    }
}