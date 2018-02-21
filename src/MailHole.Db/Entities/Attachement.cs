using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MailHole.Db.Entities.Auth;

namespace MailHole.Db.Entities
{
    public class Attachement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AttachementGuid { get; set; }

        public string Bucket { get; set; }

        public string FileName { get; set; }

        public string MimeType { get; set; }

        public long SizeInBytes { get; set; }

        public string Md5Hash { get; set; }

        public string Sha1Hash { get; set; }

        public string Sha256Hash { get; set; }
        
        public MailHoleUser Owner { get; set; }
        
        [ForeignKey(nameof(Mail))]
        public Guid MailGuid { get; set; }
        
        [Required]
        public Mail Mail { get; set; }
    }
}