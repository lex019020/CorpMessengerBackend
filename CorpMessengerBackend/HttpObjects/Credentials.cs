using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CorpMessengerBackend.HttpObjects
{
    public class Credentials
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? DeviceId { get; set; }
        public string? Token { get; set; }
    }
}
