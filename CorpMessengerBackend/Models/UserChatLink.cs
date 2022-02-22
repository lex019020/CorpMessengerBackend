using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CorpMessengerBackend.Models
{
    public class UserChatLink
    {
        [Key]
        public string UserId { get; set; }

        [Key]
        public long ChatId { get; set; }

        public bool Notifications { get; set; }
    }
}
