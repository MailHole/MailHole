using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MailHole.Common.Model
{
    public class ReceivedMail
    {
        public string Sender { get; set; }

        public IReadOnlyList<string> Bcc { get; set; } = new List<string>();

        public IReadOnlyList<string> Cc { get; set; } = new List<string>();

        public string Subject { get; set; }

        public string HtmlBody { get; set; }

        public string TextBody { get; set; }

        public IReadOnlyDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        public DateTime UtcReceivedTime { get; set; }

        public IReadOnlyList<string> Attachements { get; set; } = new List<string>();

        [JsonIgnore] public bool HasAttachements => Attachements.Count > 0;
    }
}