using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CorpMessengerBackend.Models;
using CorpMessengerBackend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using User = CorpMessengerBackend.Models.User;

namespace CorpMessengerBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDataContext _db;
        private readonly IAuthService _auth;

        public UsersController(AppDataContext dataContext, IConfiguration configuration, 
            IAuthService authService)
        {
            _db = dataContext;
            _auth = authService;

            if (!_db.Users.Any())
            {
                var basicUser = _db.Users.Add(new User()
                {
                    DepartmentId = 0, FirstName = "Admin", Modified = DateTime.Now,
                    Patronymic = "Adminovich", SecondName = "Adminov",
                    Email = "admin@admin.com"
                }).Entity;


                _db.UserSecrets.Add(new UserSecret()
                {
                    UserId = basicUser.UserId,
                    Secret = CryptographyService.HashPassword("qwerty123456")
                });
            }
            else
            {
                foreach (var user in _db.Users.Where(
                    u => !u.Deleted && !_db.UserSecrets.Any(
                        s => s.UserId == u.UserId)))
                {
                    _db.UserSecrets.Add(new UserSecret()
                    {
                        UserId = user.UserId,
                        Secret = CryptographyService.HashPassword(user.FirstName + user.SecondName)
                    });
                }
            }


            _db.SaveChanges();
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Get(string token)
        {
            if (token == "123456")    // todo check for admin token && users token
            {
                return await _db.Users.ToArrayAsync();
            }

            return Unauthorized();
        }

        [HttpPost]  // create new user
        public async Task<ActionResult<User>> Post(User user, string token)
        {
            if (token != "123456")    // todo check for admin token
                return Unauthorized();

            if (user == null)
            {
                return BadRequest();
            }

            user.Deleted = false;
            user.Modified = DateTime.Now;

            //try
            //{
            //    user.UserId = _auth.CreateUser(new Credentials
            //    {
            //        Email = user.Email,
            //        Password = user.FirstName + user.SecondName
            //    });
            //}
            //catch (Exception e)
            //{
            //    return BadRequest(e.Message);
            //}

            try
            {
                user = _db.Users.Add(user).Entity;
            }
            catch (Exception e)
            {
                // todo log
                return BadRequest(e.Message);
            }

            if (user.UserId == 0)
                return BadRequest();

            _db.UserSecrets.Add(new UserSecret()
            {
                UserId = user.UserId,
                Secret = CryptographyService.HashPassword(user.FirstName + user.SecondName)
            });


            await _db.SaveChangesAsync();

            return Ok(user);
        }

        [HttpPut]   // update user info
        public async Task<ActionResult<User>> Put(User user, string token)
        {
            if (token != "123456")    // todo check for admin token
                return Unauthorized();

            if (user == null)
            {
                return BadRequest();
            }
            if (!_db.Users.Any(x => x.UserId == user.UserId && !x.Deleted))
            {
                return NotFound();
            }

            //// todo change pwd somehow

            user.Modified = DateTime.Now;

            _db.Update(user);
            await _db.SaveChangesAsync();
            return Ok(user);
        }

        [HttpDelete]
        public async Task<ActionResult<User>> Delete(long userId, string token)
        {
            if (token != "123456")    // todo check for admin token
                return Unauthorized();

            var user = _db.Users.FirstOrDefault(x => x.UserId == userId
                                                     && !x.Deleted);
            if (user == null)
            {
                return NotFound();
            }

            user.Deleted = true;
            _db.Users.Update(user);

            await _db.SaveChangesAsync();

            _auth.SignOutFull(_db, userId);
            // todo del all chat links

            return Ok(user);
        }
    }
}
