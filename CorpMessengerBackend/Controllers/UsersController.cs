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
    private readonly IAppDataContext _db;
    private readonly ICriptographyProvider _cryptographyProvider;
    private readonly IUserAuthProvider _authProvider;
    private readonly IAuthService _authService;

    public UsersController(IAppDataContext dataContext, ICriptographyProvider cryptographyProvider, 
        IUserAuthProvider authProvider, IAuthService authService)
    {
        _db = dataContext;
        _cryptographyProvider = cryptographyProvider;
        _authProvider = authProvider;
        _authService = authService;

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
    public async Task<ActionResult<IEnumerable<User>>> Get()
    {
        return await _db.Users.ToArrayAsync();
    }

    [HttpPost] // create new user
    public async Task<ActionResult<User>> Post(User user)
    {
        if (!_authProvider.GetAdminAuth())
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
    public async Task<ActionResult<User>> Put(User user)
    {
        if (!_authProvider.GetAdminAuth())
            return Unauthorized();
        
        if (!_db.Users.Any(x => x.UserId == user.UserId && !x.Deleted)) return NotFound();

        //// todo change pwd somehow

        user.Modified = DateTime.UtcNow;

        _db.Update(user);
        await _db.SaveChangesAsync();
        return Ok(user);
    }

    [HttpDelete]
    public async Task<ActionResult<User>> Delete(long userId)
    {
        if (!_authProvider.GetAdminAuth())
            return Unauthorized();

        var user = _db.Users.FirstOrDefault(x => x.UserId == userId
                                                 && !x.Deleted);
        if (user == null) return NotFound();

        user.Deleted = true;
        user.Modified = DateTime.UtcNow;
        _db.Users.Update(user);

        await _db.SaveChangesAsync();

        await _authService.SignOutFull(_db, userId);

        return Ok(user);
    }
}