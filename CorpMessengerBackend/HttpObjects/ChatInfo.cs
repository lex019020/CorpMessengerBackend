using System.Collections.Generic;

namespace CorpMessengerBackend.HttpObjects;

public class ChatInfo
{
    public long ChatId { get; set; }
    public string? ChatName { get; set; }
    public bool IsPersonal { get; set; }
    public List<long> Users { get; set; }
}