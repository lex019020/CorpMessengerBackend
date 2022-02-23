﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CorpMessengerBackend.HttpObjects;
using CorpMessengerBackend.Models;
using CorpMessengerBackend.Services;
using Microsoft.EntityFrameworkCore;

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

        // get user chats
        [HttpGet]
        public async Task<ActionResult<List<ChatInfo>>> Get(string token)
        {
            // проверить что польз авторизован
            var userId = _auth.CheckUserAuth(_db, token);
            if (userId == "") return Unauthorized();

            var retList = new List<ChatInfo>();

            // собираем список чатов
            foreach (var userChatLink in _db.UserChatLinks.Where( 
                ucl => ucl.UserId == userId))
            {
                var chatInfo = GetInfoById(userChatLink.ChatId);

                if (chatInfo == null)
                    return BadRequest();

                retList.Add(chatInfo);
            }

            // todo get all chats (admin)

            return Ok(retList);
        }

        // get single chat info
        public async Task<ActionResult<ChatInfo>> Get(string token, long chatId)
        {
            // проверить что польз авторизован
            var userId = _auth.CheckUserAuth(_db, token);
            if (userId == "") return Unauthorized();

            // todo admin thing

            // проверка что пользователь есть в чате
            if (!_db.UserChatLinks.Any(ucl => ucl.ChatId == chatId 
                                              && ucl.UserId == userId))
                return BadRequest();

            var ci = GetInfoById(chatId);

            return ci == null 
                ? NotFound() 
                : Ok(ci);
        }

        // create new chat
        [HttpPost]
        public async Task<ActionResult<long>> Post(string token, ChatInfo chatInfo)
        {
            // проверить что польз авторизован
            var userId = _auth.CheckUserAuth(_db, token);
            if (userId == "") return Unauthorized();

            // проверить что он в списке
            var userList = chatInfo.Users;
            if (!userList.Contains(userId))
                return BadRequest();

            // проверить корректность списка
            if (userList.Any(uid => !_db.Users.Any(us => us.UserId == uid && !us.Deleted)))
            {
                return BadRequest();
            }

            // проверить корректна ли галка
            if ((userList.Count == 2) != chatInfo.IsPersonal)
                return BadRequest();

            // если персональный - запретить если есть дубликат
            if (chatInfo.IsPersonal && _db.Chats.Any(ch => ch.IsPersonal && _db.UserChatLinks.Where(
                ucl => ucl.ChatId == ch.ChatId).All(
                ucl => chatInfo.Users.Contains(ucl.UserId))))
                return BadRequest();

            // создать чат
            var newCh =_db.Chats.Add(new Chat
            {
                ChatName = chatInfo.ChatName, 
                IsPersonal = chatInfo.IsPersonal, 
                Modified = DateTime.Now
            }).Entity;

            // создать связи
            foreach (var uid in userList)
            {
                await _db.UserChatLinks.AddAsync(new UserChatLink
                {
                    ChatId = newCh.ChatId,
                    Notifications = true,
                    UserId = uid
                });
            }
            
            await _db.SaveChangesAsync();

            return Ok(newCh.ChatId);
        }

        // change chat something
        [HttpPut]
        public async Task<ActionResult<Chat>> Put(string token, Chat newChat)
        {
            // проверить что польз авторизован
            var userId = _auth.CheckUserAuth(_db, token);
            if (userId == "") return Unauthorized();

            // todo admin thing

            // проверка существования чата
            var chat = _db.Chats.FirstOrDefault(ch => ch.ChatId == newChat.ChatId);
            if (chat == null)
                return BadRequest();

            // проверка что пользователь есть в чате
            if (!_db.UserChatLinks.Any(ucl => ucl.ChatId == chat.ChatId
                                              && ucl.UserId == userId))
                return BadRequest();

            chat.ChatName = newChat.ChatName;
            chat.Modified = DateTime.Now;

            var ret = _db.Chats.Update(chat);
            await _db.SaveChangesAsync();

            return ret.Entity;
        }

        // add user to chat
        [HttpPost]
        [Route("api/[controller]/addUser")]
        public async Task<ActionResult<bool>> AddUser(string token, long chatId, string userToAdd)
        {
            var userId = _auth.CheckUserAuth(_db, token);
            if (userId == "") return Unauthorized();

            // todo admin thing

            // проверка существования чата
            var chat = _db.Chats.FirstOrDefault(ch => ch.ChatId == chatId);
            if (chat == null)
                return BadRequest();

            // проверка что пользователь есть в чате
            if (!_db.UserChatLinks.Any(ucl => ucl.ChatId == chat.ChatId
                                              && ucl.UserId == userId))
                return BadRequest();

            // проверка второго пользователя на существование
            if (!_db.Users.Any(u => u.UserId == userToAdd))
                return BadRequest();

            // проверка что его нет в чате
            if (_db.UserChatLinks.Any(ucl => ucl.ChatId == chatId
                                             && ucl.UserId == userToAdd))
                return BadRequest();

            // добавление
            await _db.UserChatLinks.AddAsync(new UserChatLink
            {
                ChatId = chat.ChatId,
                UserId = userToAdd,
                Notifications = true
            }).AsTask().ContinueWith(async _ => await _db.SaveChangesAsync());

            return Ok(true);
        }

        // kick user
        [HttpPost]
        [Route("api/[controller]/kickUser")]
        public async Task<ActionResult<bool>> KickUser(string token, long chatId, string userToKick)
        {
            var userId = _auth.CheckUserAuth(_db, token);
            if (userId == "") return Unauthorized();

            // todo admin thing

            // проверка существования чата
            var chat = _db.Chats.FirstOrDefault(ch => ch.ChatId == chatId);
            if (chat == null)
                return BadRequest();

            // проверка что пользователь есть в чате
            if (!_db.UserChatLinks.Any(ucl => ucl.ChatId == chat.ChatId
                                              && ucl.UserId == userId))
                return BadRequest();

            // проверка второго пользователя на существование
            if (!_db.Users.Any(u => u.UserId == userToKick))
                return BadRequest();

            // проверка что кикаемый есть в чате
            var link = _db.UserChatLinks.FirstOrDefault(
                ucl => ucl.ChatId == chatId && ucl.UserId == userToKick);

            if (link == null)
                return BadRequest();

            // удаление
            _db.UserChatLinks.Remove(link);

            await _db.SaveChangesAsync();

            return Ok(true);
        }

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

            retInfo.ChatId = chat.ChatId;
            retInfo.ChatName = chat.ChatName;
            retInfo.IsPersonal = chat.IsPersonal;

            return retInfo;
        }
    }
}
