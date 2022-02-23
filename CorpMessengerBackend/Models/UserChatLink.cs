using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CorpMessengerBackend.Models
{
    public class UserChatLink
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public long LinkId { get; set; }

        public string UserId { get; set; }
        
        public long ChatId { get; set; }

        public bool Notifications { get; set; }
    }
}
