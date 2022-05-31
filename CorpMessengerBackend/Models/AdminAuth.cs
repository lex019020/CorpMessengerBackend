using System.ComponentModel.DataAnnotations;

namespace CorpMessengerBackend.Models;

public class AdminAuth
{
    [Key] public long AdminAuthId { get; set; }

    [Required] public string Token { get; set; }

    [Required] public long UserId { get; set; }
}