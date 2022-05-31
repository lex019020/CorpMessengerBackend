using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CorpMessengerBackend.HttpObjects;
using CorpMessengerBackend.Models;
using CorpMessengerBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CorpMessengerBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChatController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IAppDataContext _db;

    public ChatController(IAuthService authService, IAppDataContext dataContext)
    {
        _authService = authService;
        _db = dataContext;
    }

    // get user chats
    [HttpGet]
    public async Task<ActionResult<List<ChatInfo>>> Get(string token)
    {
        // проверить что польз авторизован
        var userId = _authService.CheckUserAuth(_db, token);
        var isAdmin = _authService.CheckAdminAuth(_db, token);
        if (userId == 0 && !isAdmin) return Unauthorized();

        var userChatsList = new List<ChatInfo?>();

        if (isAdmin)
        {
            userChatsList.AddRange(_db.Chats.Select(c => GetInfoById(c.ChatId)));

            return Ok(userChatsList);
        }

        var chatLinks = await _db.UserChatLinks
            .Where(ucl => ucl.UserId == userId).ToListAsync();

        // собираем список чатов
        chatLinks.ForEach(ucl => userChatsList.Add(GetInfoById(ucl.ChatId)));

        if (userChatsList.Contains(null)) return BadRequest();

        return Ok(userChatsList);
    }

    // get single chat info
    public Task<ActionResult<ChatInfo>> Get(string token, long chatId)
    {
        // проверить что польз авторизован
        var userId = _authService.CheckUserAuth(_db, token);
        var isAdmin = _authService.CheckAdminAuth(_db, token);
        if (userId == 0 && !isAdmin) return Task.FromResult<ActionResult<ChatInfo>>(Unauthorized());

        if (!isAdmin)
            // проверка что пользователь есть в чате
            if (!_db.UserChatLinks.Any(ucl => ucl.ChatId == chatId
                                              && ucl.UserId == userId))
                return Task.FromResult<ActionResult<ChatInfo>>(BadRequest());

        var chatInfo = GetInfoById(chatId);

        return Task.FromResult<ActionResult<ChatInfo>>(chatInfo == null
            ? NotFound()
            : Ok(chatInfo));
    }

    // create new chat
    [HttpPost]
    public async Task<ActionResult<long>> Post(string token, ChatInfo chatInfo)
    {
        // проверить что польз авторизован
        var userId = _authService.CheckUserAuth(_db, token);
        if (userId == 0) return Unauthorized();

        // проверить что он в списке
        var userList = chatInfo.Users;
        if (!userList.Contains(userId))
            return BadRequest();

        // проверить корректность списка
        if (userList.Any(uid => !_db.Users.Any(us => us.UserId == uid && !us.Deleted))) return BadRequest();

        // проверить корректна ли галка
        if (userList.Count == 2 != chatInfo.IsPersonal)
            return BadRequest();

        // если персональный - запретить если есть дубликат
        if (chatInfo.IsPersonal && _db.Chats.Any(ch => ch.IsPersonal && _db.UserChatLinks.Where(
                ucl => ucl.ChatId == ch.ChatId).All(
                ucl => chatInfo.Users.Contains(ucl.UserId))))
            return BadRequest();

        // создать чат
        var newChat = _db.Chats.Add(new Chat
        {
            ChatName = chatInfo.ChatName ?? "",
            IsPersonal = chatInfo.IsPersonal,
            Modified = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();

        // создать связи
        foreach (var uid in userList)
            await _db.UserChatLinks.AddAsync(new UserChatLink
            {
                ChatId = newChat.Entity.ChatId,
                Notifications = true,
                UserId = uid
            });

        await _db.SaveChangesAsync();

        return Ok(newChat.Entity);
    }

    // change chat something
    [HttpPut]
    public async Task<ActionResult<Chat>> Put(string token, Chat newChat)
    {
        // проверить что польз авторизован
        var userId = _authService.CheckUserAuth(_db, token);
        if (userId == 0) return Unauthorized();

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
        chat.Modified = DateTime.UtcNow;

        var changedChat = _db.Chats.Update(chat);
        await _db.SaveChangesAsync();

        return changedChat.Entity;
    }

    // add user to chat
    [HttpPost("addUser")]
    public async Task<ActionResult<bool>> AddUser(string token, long chatId, long userToAdd)
    {
        var userId = _authService.CheckUserAuth(_db, token);
        if (userId == 0) return Unauthorized();

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

        // todo в чат системное сообщение

        return Ok(true);
    }

    // kick user
    [HttpPost("kickUser")]
    public async Task<ActionResult<bool>> KickUser(string token, long chatId, long userToKick)
    {
        var userId = _authService.CheckUserAuth(_db, token);
        if (userId == 0) return Unauthorized();

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

        // todo в чат системное сообщение

        return Ok(true);
    }

    private ChatInfo? GetInfoById(long chatId)
    {
        var retInfo = new ChatInfo();
        var chat = _db.Chats.FirstOrDefault(c => c.ChatId == chatId);

        if (chat == null) return null;

        retInfo.Users = new List<long>();

        foreach (var userChatLink in _db.UserChatLinks
                     .Where(ucl => ucl.ChatId == chat.ChatId))
            retInfo.Users.Add(userChatLink.UserId);

        retInfo.ChatId = chat.ChatId;
        retInfo.ChatName = chat.ChatName;
        retInfo.IsPersonal = chat.IsPersonal;

        return retInfo;
    }
}