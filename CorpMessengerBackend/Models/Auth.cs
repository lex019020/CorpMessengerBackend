using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CorpMessengerBackend.Models
{
    public class Auth
    {
        [Key]
        public string AuthId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string AuthToken { get; set; }

        [Required]
        public string DeviceId { get; set; }
        
        [Required]
        public DateTime Modified { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

    }
}
