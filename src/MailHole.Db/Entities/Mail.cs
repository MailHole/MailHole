using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MailHole.Db.Entities
{
    public class Mail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid MailGuid { get; set; }
        
        public string Sender { get; set; }

        public string BccJson { get; set; }

        public string CcJson { get; set; }

        public string Subject { get; set; }

        public string HtmlBody { get; set; }

        public string TextBody { get; set; }

        public string HeadersJson { get; set; }

        public DateTime UtcReceivedTime { get; set; }
        
        [NotMapped]
        public IReadOnlyList<string> Bcc { get; }

        [NotMapped]
        public IReadOnlyList<string> Cc { get; }
        
        [NotMapped]
        public IReadOnlyDictionary<string, string> Headers { get; set; }
    }
}