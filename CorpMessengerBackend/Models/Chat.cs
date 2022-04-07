using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CorpMessengerBackend.Models
{
    public class Chat
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public long ChatId { get; set; }

        [StringLength(255)]
        public string? ChatName { get; set; }
        
        [Required]
        public bool IsPersonal { get; set; }

        public DateTime Modified { get; set; }

    }
}
