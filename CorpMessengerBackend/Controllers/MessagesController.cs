﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CorpMessengerBackend.Models;
using CorpMessengerBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CorpMessengerBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MessagesController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IAppDataContext _db;
    private readonly IDateTimeService _dateTimeService;

    public MessagesController(IAuthService authService, IAppDataContext dataContext, IDateTimeService dateTimeService)
    {
        _authService = authService;
        _db = dataContext;
        _dateTimeService = dateTimeService;
    }

    // get messages by datetime
    [HttpGet]
    public async Task<ActionResult<List<Message>>> Get(string token, DateTime dateAfter)
    {
        var userId = _authService.CheckUserAuth(_db, token);

        if (userId != 0)
            return Ok(await _db.Messages
                .Where(m =>
                    m.Sent >= dateAfter
                    && _db.UserChatLinks.Any(ucl =>
                        ucl.UserId == userId
                        && ucl.ChatId == m.ChatId))
                .ToListAsync());


        if (!_authService.CheckAdminAuth(_db, token))
            return Unauthorized();

        return Ok(await _db.Messages.Where(m => m.Sent >= dateAfter).ToListAsync());
    }

    // get messages by datetime and chat
    [HttpGet("chat")]
    public async Task<ActionResult<List<Message>>> Get(string token, DateTime dateAfter,
        DateTime? dateBefore, long chatId)
    {
        //var date = new DateTime(datetime);
        var userId = _authService.CheckUserAuth(_db, token);

        if (userId == 0) return Unauthorized();

        // есть ли юзер в чате
        if (!_db.UserChatLinks.Any(ucl => ucl.UserId == userId
                                          && ucl.ChatId == chatId))
            return BadRequest();

        return Ok(await _db.Messages
            .Where(m => m.ChatId == chatId
                        && m.Sent >= dateAfter
                        && (dateBefore == null || m.Sent <= dateBefore))
            .ToListAsync());
    }

    // send message
    [HttpPost]
    public async Task<ActionResult<Message>> Post(string token, long chatId, string text)
    {
        var userId = _authService.CheckUserAuth(_db, token);
        if (userId == 0) return Unauthorized();

        // todo admin things??

        // есть ли юзер в чате
        if (!_db.UserChatLinks.Any(ucl => ucl.UserId == userId
                                          && ucl.ChatId == chatId))
            return BadRequest();

        var newMessage = _db.Messages.Add(new Message
        {
            ChatId = chatId,
            UserId = userId,
            Text = text,
            Sent = _dateTimeService.CurrentDateTime
        });

        // todo notifications

        await _db.SaveChangesAsync();

        return Ok(newMessage.Entity);
    }

    // generate random data in DB
    [HttpPost("generate")]
    public async Task<ActionResult> Post(string token)
    {
        var admin = _authService.CheckAdminAuth(_db, token);
        if (!admin) return Unauthorized();

        var rnd = new Random(Environment.TickCount);

        var lst1 = new List<EntityEntry<Department>>(); // list of added department entities
        for (var i = 0; i < 20; ++i)
            lst1.Add(_db.Departments.Add(new Department
            {
                DepartmentName = $"Тестовый отдел №{rnd.Next(1000)} test"
            }));

        await _db.SaveChangesAsync();

        var addedDepartments = lst1.Select(ent => ent.Entity?.DepartmentId).ToList(); // list of added depId's

        var lst2 = new List<EntityEntry<User>>(); // list of added user entities
        foreach (var val in addedDepartments.Where(val => val != null))
            for (var i = 0; i < 20; ++i)
            {
                lst2.Add(_db.Users.Add(new User
                {
                    DepartmentId = (long)val,
                    FirstName = $"Имя {rnd.Next(10000)}",
                    SecondName = $"Фамилия {rnd.Next(1000)}",
                    Email = $"testuser{rnd.Next(100_000_000)}@example.com",
                    Deleted = false
                }));
                ;
            }

        await _db.SaveChangesAsync();

        var addedUsers = lst2.Select(ent => ent.Entity?.UserId).ToList(); // list of added UserId's
        var curUserId = 1; //

        var lst3 = new List<EntityEntry<Chat>>(); // list of added chat entities
        for (var i = 0; i < 15; ++i) lst3.Add(_db.Chats.Add(new Chat { ChatName = $"Test chat #{rnd.Next(1000)}" }));
        await _db.SaveChangesAsync();

        var addedChats = lst3.Select(el => el.Entity.ChatId).ToList();

        foreach (var chat in addedChats)
        {
            foreach (var p in addedUsers.OrderBy(x => rnd.Next()).Take(rnd.Next(5, 20)))
                await _db.UserChatLinks.AddAsync(new UserChatLink
                {
                    ChatId = chat,
                    Notifications = true,
                    UserId = p ?? curUserId + 5
                });

            await _db.UserChatLinks.AddAsync(new UserChatLink
            {
                ChatId = chat,
                Notifications = true,
                UserId = curUserId
            });
        }

        await _db.SaveChangesAsync();

        foreach (var chat in addedChats)
        {
            var count = rnd.Next(50, 250);
            for (var i = 0; i < count; ++i)
                _db.Messages.Add(new Message
                {
                    ChatId = chat,
                    UserId = rnd.Next(4) == 1
                        ? curUserId
                        : addedUsers[rnd.Next(0, addedUsers.Count)],
                    Text = $"Тестовое сообщение {rnd.Next(100_000_000)} test",
                    Sent = _dateTimeService.CurrentDateTime.AddSeconds(
                        rnd.Next(-7_000_000, -10))
                });
        }

        await _db.SaveChangesAsync();

        return Ok();
    }

    // todo connect to chat or sth?????
}