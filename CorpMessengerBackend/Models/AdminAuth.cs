using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CorpMessengerBackend.Models
{
    public class AdminAuth
    {
        [Key]
        public long AdminAuthId { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public long UserId { get; set; }
    }
}
