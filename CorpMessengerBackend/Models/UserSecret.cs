using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CorpMessengerBackend.Models
{
    public class UserSecret
    {
        [Key]
        public string UserId { get; set; }

        [Required]
        public string Secret { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
