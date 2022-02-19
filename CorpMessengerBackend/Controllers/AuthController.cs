using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using CorpMessengerBackend.Models;
using CorpMessengerBackend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CorpMessengerBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthContext _db;
        private readonly IAuthService _authService;

        public AuthController(AuthContext context, IConfiguration configuration, IAuthService authService)
        {
            _db = context;
            _authService = authService;
        }

        [HttpGet] // check user auth
        public async Task<ActionResult<bool>> Get(Credentials credentials)
        {
            if (credentials.DeviceId == "" || credentials.Token == "")
                return BadRequest(false);

            return await _db.Auths.AnyAsync(a => a.AuthToken == credentials.Token
                                                && a.DeviceId == credentials.DeviceId);
        }

        [HttpPost] // user auth //todo recheck this siht
        public async Task<ActionResult<Credentials>> Post(Credentials credentials)
        {
            if (credentials.DeviceId == "" || credentials.Email == "" || credentials.Password == "")
                return BadRequest(new Credentials());

            var auth = new Auth();

            /*await _auth.SignInWithEmailAndPasswordAsync(credentials.Email, credentials.Password)
                .ContinueWith(task =>
                {
                    if (task.IsCanceled)
                    {
                        // todo
                    }

                    if (task.IsFaulted)
                    {
                        // todo
                    }

                    var newUser = task.Result.User;
                    var token = task.Result.FirebaseToken;
                    auth = new Auth
                    {
                        AuthToken = token,
                        DeviceId = credentials.DeviceId,
                        UserId = newUser.LocalId,
                        Modified = DateTime.Now
                    };
                    // todo
                });*/

            if (auth.AuthToken == "") return Unauthorized();
            
            await _db.Auths.AddAsync(auth);
            await _db.SaveChangesAsync();

            credentials.Token = auth.AuthToken;

            return Ok( credentials );

        }

        [HttpDelete]
        public async Task<ActionResult<bool>> Delete(Credentials credentials)
        {
            if (credentials.Token == "" || credentials.DeviceId == "")
                return BadRequest(false);
        }
    }
}
