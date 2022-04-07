using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CorpMessengerBackend.Models;
using CorpMessengerBackend.Services;
using Microsoft.EntityFrameworkCore;

namespace CorpMessengerBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IAuthService _auth;
        private readonly AppDataContext _db;

        public MessagesController(IAuthService authService, AppDataContext dataContext)
        {
            _auth = authService;
            _db = dataContext;
        }

        // get messages by datetime
        [HttpGet]
        public async Task<ActionResult<List<Message>>> Get(string token, long datetime)
        {
            var userId = _auth.CheckUserAuth(_db, token);

            var date = new DateTime(datetime);

            if (userId != 0)
                return Ok(await _db.Messages
                    .Where(m =>
                        m.Sent >= date
                        && _db.UserChatLinks.Any(ucl =>
                            ucl.UserId == userId
                            && ucl.ChatId == m.ChatId))
                    .ToListAsync());
            

            if (!_auth.CheckAdminAuth(_db, token)) 
                return Unauthorized();

            return Ok(await _db.Messages.Where(m => m.Sent >= date).ToListAsync() );
        }

        // get messages by datetime and chat
        [HttpGet("chat")]
        public async Task<ActionResult<List<Message>>> Get(string token, long datetime, long chatId)
        {
            var date = new DateTime(datetime);
            var userId = _auth.CheckUserAuth(_db, token);

            if (userId == 0) return Unauthorized();

            // есть ли юзер в чате
            if (!_db.UserChatLinks.Any(ucl => ucl.UserId == userId 
                                              && ucl.ChatId == chatId))
                return BadRequest();

            return Ok(await _db.Messages
                .Where(m => m.ChatId == chatId
                            && m.Sent >= date)
                .ToListAsync());
        }

        // todo send message
        [HttpPost]
        public async Task<ActionResult<Message>> Post(string token, long chatId, string text)
        {
            var userId = _auth.CheckUserAuth(_db, token);
            if (userId == 0) return Unauthorized();

            // todo admin things??

            // есть ли юзер в чате
            if (!_db.UserChatLinks.Any(ucl => ucl.UserId == userId
                                              && ucl.ChatId == chatId))
                return BadRequest();

            var mess = _db.Messages.Add(new Message
            {
                ChatId = chatId,
                UserId = userId,
                Text = text,
                Sent = DateTime.UtcNow
            }).Entity;

            // todo notifications

            await _db.SaveChangesAsync();

            return Ok(mess);
        }

        // todo connect to chat or sth?????
    }
}
