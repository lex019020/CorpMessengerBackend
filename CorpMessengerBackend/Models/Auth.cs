using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CorpMessengerBackend.Models;

[Index("AuthToken")]
public class Auth
{
    [Key] public Guid AuthId { get; set; }

    [Required] public long UserId { get; set; }

    [Required] public string AuthToken { get; set; }

    [Required] public string DeviceId { get; set; }

    [Required] public DateTime Modified { get; set; }

    [ForeignKey("UserId")] public virtual User User { get; set; }
}