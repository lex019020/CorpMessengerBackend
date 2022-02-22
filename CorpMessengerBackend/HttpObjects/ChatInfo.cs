using System.Collections.Generic;

namespace CorpMessengerBackend.HttpObjects
{
    public class ChatInfo
    {
        public string ChatName { get; set; }
        public bool IsPersonal { get; set; }
        public List<string> Users { get; set; }
    }
}
