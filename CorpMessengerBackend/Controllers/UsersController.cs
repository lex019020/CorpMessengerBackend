using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using CorpMessengerBackend.Models;
using Firebase.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using User = CorpMessengerBackend.Models.User;

namespace CorpMessengerBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private UserContext _db;
        private readonly FirebaseAuthProvider _auth;

        public UsersController(UserContext context, IConfiguration configuration)
        {
            _db = context;
            _auth = new FirebaseAuthProvider(
                new FirebaseConfig(configuration["FirebaseApiString:DefaultKey"]));

            if (!_db.Users.Any())
            {
                _db.Users.Add(new User()
                {
                    DepartmentId = 0, FirstName = "Admin", Modified = DateTime.Now,
                    Patronymic = "Adminovich", SecondName = "Adminov", UserId = "0",
                    Email = "admin@admin.com"
                });
                _db.SaveChanges();
            }
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

            // todo add user auth to firebase

            _db.Users.Add(user);
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
            if (!_db.Users.Any(x => x.UserId == user.UserId))
            {
                return NotFound();
            }

            user.Modified = DateTime.Now;

            _db.Update(user);
            await _db.SaveChangesAsync();
            return Ok(user);
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult<User>> Delete(string userId, string token)
        {
            if (token != "123456")    // todo check for admin token
                return Unauthorized();

            var user = _db.Users.FirstOrDefault(x => x.UserId == userId);
            if (user == null)
            {
                return NotFound();
            }

            user.Deleted = true;
            _db.Users.Update(user);

            // todo del all auth & chat links

            await _db.SaveChangesAsync();
            return Ok(user);
        }
    }
}
