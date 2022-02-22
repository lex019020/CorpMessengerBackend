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
        private readonly UserContext _dbUsers;
        private readonly UserSecretContext _dbUserSecrets;
        private readonly IAuthService _auth;

        public UsersController(UserContext userContext, UserSecretContext secretContext,
            IConfiguration configuration, IAuthService authService)
        {
            _dbUsers = userContext;
            _dbUserSecrets = secretContext;
            _auth = authService;

            if (!_dbUsers.Users.Any())
            {
                var basicUser = _dbUsers.Users.Add(new User()
                {
                    DepartmentId = 0, FirstName = "Admin", Modified = DateTime.Now,
                    Patronymic = "Adminovich", SecondName = "Adminov", UserId = "0",
                    Email = "admin@admin.com"
                }).Entity;


                _dbUserSecrets.UserSecrets.Add(new UserSecret()
                {
                    UserId = basicUser.UserId,
                    Secret = CryptographyService.HashPassword("qwerty123456")
                });
            }
            else
            {
                foreach (var user in _dbUsers.Users.Where(
                    u => !u.Deleted && !_dbUserSecrets.UserSecrets.Any(
                        s => s.UserId == u.UserId)))
                {
                    _dbUserSecrets.UserSecrets.Add(new UserSecret()
                    {
                        UserId = user.UserId,
                        Secret = CryptographyService.HashPassword(user.FirstName + user.SecondName)
                    });
                }
            }


            _dbUsers.SaveChanges();
            _dbUserSecrets.SaveChanges();
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Get(string token)
        {
            if (token == "123456")    // todo check for admin token && users token
            {
                return await _dbUsers.Users.ToArrayAsync();
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
                user = _dbUsers.Users.Add(user).Entity;
            }
            catch (Exception e)
            {
                // todo log
                return BadRequest(e.Message);
            }

            if (user.UserId == "")
                return BadRequest();

            _dbUserSecrets.UserSecrets.Add(new UserSecret()
            {
                UserId = user.UserId,
                Secret = CryptographyService.HashPassword(user.FirstName + user.SecondName)
            });


            await _dbUserSecrets.SaveChangesAsync();
            await _dbUsers.SaveChangesAsync();

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
            if (!_dbUsers.Users.Any(x => x.UserId == user.UserId && !x.Deleted))
            {
                return NotFound();
            }

            //// todo change pwd somehow

            user.Modified = DateTime.Now;

            _dbUsers.Update(user);
            await _dbUsers.SaveChangesAsync();
            return Ok(user);
        }

        [HttpDelete]
        public async Task<ActionResult<User>> Delete(string userId, string token)
        {
            if (token != "123456")    // todo check for admin token
                return Unauthorized();

            var user = _dbUsers.Users.FirstOrDefault(x => x.UserId == userId
            && !x.Deleted);
            if (user == null)
            {
                return NotFound();
            }

            user.Deleted = true;
            _dbUsers.Users.Update(user);

            await _dbUsers.SaveChangesAsync();

            _auth.SignOutFull(userId);
            // todo del all chat links

            return Ok(user);
        }
    }
}
