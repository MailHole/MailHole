namespace MailHole.Common.Model.Options
{
    public class MinioOptions
    {
        public string Endpoint { get; set; } = "minio:9000";

        public string AccessKey { get; set; } = string.Empty;

        public string SecretKey { get; set; } = string.Empty;

        public string Region { get; set; } = string.Empty;
    }
}