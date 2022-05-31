using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CorpMessengerBackend.Models;

public class UserChatLink
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid LinkId { get; set; }

    public long UserId { get; set; }

    public long ChatId { get; set; }

    public bool Notifications { get; set; }

    [ForeignKey("ChatId")] public virtual Chat Chat { get; set; }

    [ForeignKey("UserId")] public virtual User User { get; set; }
}