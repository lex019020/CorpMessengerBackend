using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CorpMessengerBackend.Models;

public class Chat
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long ChatId { get; set; }

    [StringLength(255)] public string? ChatName { get; set; }

    [Required] public bool IsPersonal { get; set; }

    public DateTime Modified { get; set; }
}