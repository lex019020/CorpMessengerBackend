using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CorpMessengerBackend.HttpObjects;
using CorpMessengerBackend.Models;
using CorpMessengerBackend.Services;

namespace CorpMessengerBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IAuthService _auth;
        private readonly AppDataContext _db;

        public ChatController(IAuthService authService, AppDataContext dataContext)
        {
            _auth = authService;
            _db = dataContext;
        }

        // todo get user chats

        // todo get chat info

        // todo create new chat
        [HttpPost]

        // todo change chat something

        // todo add user

        // todo kick user

        private ChatInfo GetInfoById(long chatId)
        {
            var retInfo = new ChatInfo();
            var chat = _db.Chats.FirstOrDefault(c => c.ChatId == chatId);

            if (chat == null) return null;

            retInfo.Users = new List<string>();

            foreach (var userChatLink in _db.UserChatLinks
                .Where(ucl => ucl.ChatId == chat.ChatId))
            {
                retInfo.Users.Add(userChatLink.UserId);
            }

            retInfo.ChatName = chat.ChatName;
            retInfo.IsPersonal = chat.IsPersonal;

            return retInfo;
        }
    }
}
