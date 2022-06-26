using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CorpMessengerBackend.Models;
using CorpMessengerBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CorpMessengerBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IAppDataContext _db;
    private readonly ICriptographyProvider _cryptographyProvider;

    public UsersController(IAppDataContext dataContext,
        IAuthService authService, ICriptographyProvider cryptographyProvider)
    {
        _db = dataContext;
        _authService = authService;
        _cryptographyProvider = cryptographyProvider;

        if (!_db.Users.Any())
        {
            var basicUser = _db.Users.Add(new User
            {
                DepartmentId = _db.Departments.First().DepartmentId,
                FirstName = "Admin",
                Patronymic = "Adminovich",
                SecondName = "Adminov",
                Email = "admin@admin.com",
                Modified = DateTime.UtcNow
            });

            _db.SaveChanges();

            _db.UserSecrets.Add(new UserSecret
            {
                UserId = basicUser.Entity.UserId,
                Secret = _cryptographyProvider.HashPassword("qwerty")
            });
        }
        else
        {
            foreach (var user in _db.Users.Where(
                         u => !u.Deleted && !_db.UserSecrets.Any(
                             s => s.UserId == u.UserId)))
                _db.UserSecrets.Add(new UserSecret
                {
                    UserId = user.UserId,
                    Secret = _cryptographyProvider.HashPassword(user.FirstName + user.SecondName)
                });
        }


        _db.SaveChanges();
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> Get(string token)
    {
        if (_authService.CheckUserAuth(_db, token) != 0 || _authService.CheckAdminAuth(_db, token))
            return await _db.Users.ToArrayAsync();

        return Unauthorized();
    }

    [HttpPost] // create new user
    public async Task<ActionResult<User>> Post(User user, string token)
    {
        if (!_authService.CheckAdminAuth(_db, token))
            return Unauthorized();

        user.Deleted = false;
        user.Modified = DateTime.UtcNow;

        try
        {
            user = (await _db.Users.AddAsync(user)).Entity;
        }
        catch (Exception e)
        {
            // todo log
            return BadRequest(e.Message);
        }


        await _db.SaveChangesAsync();

        if (user.UserId == 0)
            return BadRequest();

        _db.UserSecrets.Add(new UserSecret
        {
            UserId = user.UserId,
            Secret = _cryptographyProvider.HashPassword(user.FirstName + user.SecondName)
        });


        await _db.SaveChangesAsync();

        return Ok(user);
    }

    [HttpPut] // update user info
    public async Task<ActionResult<User>> Put(User user, string token)
    {
        if (!_authService.CheckAdminAuth(_db, token))
            return Unauthorized();

        // if (user == null)
        // {
        //     return BadRequest();
        // }

        if (!_db.Users.Any(x => x.UserId == user.UserId && !x.Deleted)) return NotFound();

        //// todo change pwd somehow

        user.Modified = DateTime.UtcNow;

        _db.Update(user);
        await _db.SaveChangesAsync();
        return Ok(user);
    }

    [HttpDelete]
    public async Task<ActionResult<User>> Delete(long userId, string token)
    {
        if (!_authService.CheckAdminAuth(_db, token))
            return Unauthorized();

        var user = _db.Users.FirstOrDefault(x => x.UserId == userId
                                                 && !x.Deleted);
        if (user == null) return NotFound();

        user.Deleted = true;
        user.Modified = DateTime.UtcNow;
        _db.Users.Update(user);

        await _db.SaveChangesAsync();

        _authService.SignOutFull(_db, userId);
        // todo del all chat links

        return Ok(user);
    }
}