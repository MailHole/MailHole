namespace MailHole.Api.Models.Validation
{
    public class ValidationRequest
    {
        public string HashType { get; set; }

        public string HashToValidate { get; set; }
    }
}