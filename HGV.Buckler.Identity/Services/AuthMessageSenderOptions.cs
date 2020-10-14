using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HGV.Buckler.Identity.Services
{
    public class AuthMessageSenderOptions
    {
        public const string Prefix = "SendGrid";

        public string Sender { get; set; }
        public string User { get; set; }
        public string Key { get; set; }
    }
}
