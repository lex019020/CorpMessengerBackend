﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CorpMessengerBackend.Models
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long MessageId { get; set; }

        [Required]
        public long ChatId { get; set; }

        public string UserId { get; set; }

        [Required]
        public string Text { get; set; }

        public long AttachmentId { get; set; }

        [Required]
        public DateTime Sent { get; set; }

        [ForeignKey("ChatId")]
        public virtual Chat Chat { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

}
}
