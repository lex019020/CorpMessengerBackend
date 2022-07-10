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
    private readonly IUserAuthProvider _authProvider;
    private readonly IAppDataContext _db;
    private readonly IDateTimeService _dateTimeService;

    public ChatController(IUserAuthProvider authProvider, IAppDataContext dataContext, IDateTimeService dateTimeService)
    {
        _authProvider = authProvider;
        _db = dataContext;
        _dateTimeService = dateTimeService;
    }

    // get user chats
    [HttpGet]
    public async Task<ActionResult<List<ChatInfo>>> Get()
    {
        // проверить что польз авторизован
        var userAuth = _authProvider.GetUserAuth();
        var adminAuth = _authProvider.GetAdminAuth();

        var userChatsList = new List<ChatInfo?>();

        if (adminAuth)
        {
            userChatsList.AddRange(_db.Chats.Select(c => GetInfoById(c.ChatId)));

            return Ok(userChatsList);
        }

        var chatLinks = await _db.UserChatLinks
            .Where(ucl => ucl.UserId == userAuth.UserId).ToListAsync();

        // собираем список чатов
        chatLinks.ForEach(ucl => userChatsList.Add(GetInfoById(ucl.ChatId)));

        if (userChatsList.Contains(null)) return BadRequest();

        return Ok(userChatsList);
    }

    // get single chat info
    public Task<ActionResult<ChatInfo>> Get(long chatId)
    {
        // проверить что польз авторизован
        var userAuth = _authProvider.GetUserAuth();
        var adminAuth = _authProvider.GetAdminAuth();

        if (!adminAuth)
            // проверка что пользователь есть в чате
            if (!_db.UserChatLinks.Any(ucl => ucl.ChatId == chatId
                                              && ucl.UserId == userAuth.UserId))
                return Task.FromResult<ActionResult<ChatInfo>>(BadRequest());

        var chatInfo = GetInfoById(chatId);

        return Task.FromResult<ActionResult<ChatInfo>>(chatInfo == null
            ? NotFound()
            : Ok(chatInfo));
    }

    // create new chat
    [HttpPost]
    public async Task<ActionResult<long>> Post(ChatInfo chatInfo)
    {
        // проверить что польз авторизован
        var userAuth = _authProvider.GetUserAuth();

        // проверить что он в списке
        var userList = chatInfo.Users;
        if (!userList.Contains(userAuth.UserId))
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
            Modified = _dateTimeService.CurrentDateTime
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

        // сообщение о создании
        if (!chatInfo.IsPersonal)
        {
            var creator = _db.Users.First(u => u.UserId == userAuth.UserId);
            
            await _db.Messages.AddAsync(new Message
            {
                ChatId = newChat.Entity.ChatId,
                Sent = _dateTimeService.CurrentDateTime,
                Text = $"{creator.FirstName} {creator.SecondName} создал(а) чат"
            });
        }

        await _db.SaveChangesAsync();

        return Ok(newChat.Entity);
    }

    // change chat something
    [HttpPut]
    public async Task<ActionResult<Chat>> Put(Chat newChat)
    {
        // проверить что польз авторизован
        var userAuth = _authProvider.GetUserAuth();

        // todo admin thing

        // проверка существования чата
        var chat = _db.Chats.FirstOrDefault(ch => ch.ChatId == newChat.ChatId);
        if (chat == null)
            return BadRequest();

        // проверка что пользователь есть в чате
        if (!_db.UserChatLinks.Any(ucl => ucl.ChatId == chat.ChatId
                                          && ucl.UserId == userAuth.UserId))
            return BadRequest();

        chat.ChatName = newChat.ChatName;
        chat.Modified = _dateTimeService.CurrentDateTime;

        var changedChat = _db.Chats.Update(chat);
        await _db.SaveChangesAsync();

        return changedChat.Entity;
    }

    // add user to chat
    [HttpPost("addUser")]
    public async Task<ActionResult<bool>> AddUser(long chatId, long userToAdd)
    {
        var userAuth = _authProvider.GetUserAuth();

        // todo admin thing

        // проверка существования чата
        var chat = _db.Chats.FirstOrDefault(ch => ch.ChatId == chatId);
        if (chat == null)
            return BadRequest();

        // проверка что пользователь есть в чате
        if (!_db.UserChatLinks.Any(ucl => ucl.ChatId == chat.ChatId
                                          && ucl.UserId == userAuth.UserId))
            return BadRequest();

        // проверка второго пользователя на существование
        var added = _db.Users.FirstOrDefault(u => u.UserId == userToAdd);
        if (added is null)
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
        });
        await _db.SaveChangesAsync();

        var adder = _db.Users.FirstOrDefault(u => u.UserId == userAuth.UserId);
        if (adder is null)
        {
            // todo log
            return Problem();
        }

        await _db.Messages.AddAsync(new Message
        {
            ChatId = chat.ChatId,
            Sent = _dateTimeService.CurrentDateTime,
            Text = $"{adder.FirstName} {adder.SecondName} добавил(а) пользователя {added.FirstName} {added.SecondName}"
        });
        
        await _db.SaveChangesAsync();

        return Ok(true);
    }

    // kick user
    [HttpPost("kickUser")]
    public async Task<ActionResult<bool>> KickUser(long chatId, long userToKick)
    {
        var userAuth = _authProvider.GetUserAuth();

        var kicker = _db.Users.First(u => u.UserId == userAuth.UserId);

        // todo admin thing

        // проверка существования чата
        var chat = _db.Chats.FirstOrDefault(ch => ch.ChatId == chatId);
        if (chat == null)
            return BadRequest();

        // проверка второго пользователя на существование
        var kicked = _db.Users.FirstOrDefault(u => u.UserId == userToKick);
        if (kicked is null)
            return BadRequest();
        
        // проверка что пользователь есть в чате
        if (!_db.UserChatLinks.Any(ucl => ucl.ChatId == chat.ChatId
                                          && ucl.UserId == userAuth.UserId))
            return BadRequest();

        

        // проверка что кикаемый есть в чате
        var link = _db.UserChatLinks.FirstOrDefault(
            ucl => ucl.ChatId == chatId && ucl.UserId == userToKick);

        if (link == null)
            return BadRequest();

        // удаление
        _db.UserChatLinks.Remove(link);
        
        // системное сообщение
        await _db.Messages.AddAsync(new Message
        {
            ChatId = chat.ChatId,
            Sent = _dateTimeService.CurrentDateTime,
            Text = $"{kicker.FirstName} {kicker.SecondName} удалил(а) пользователя {kicked.FirstName} {kicked.SecondName}"
        });

        await _db.SaveChangesAsync();

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