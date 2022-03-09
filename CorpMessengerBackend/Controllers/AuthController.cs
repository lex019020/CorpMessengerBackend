using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using CorpMessengerBackend.HttpObjects;
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
        private readonly AppDataContext _db;
        private readonly IAuthService _authService;

        public AuthController(AppDataContext context, IAuthService authService)
        {
            _db = context;
            _authService = authService;
        }

        [HttpGet] // check user auth
        public async Task<ActionResult<bool>> Get(Credentials credentials)
        {
            if (credentials.DeviceId == "" || credentials.Token == "")
                return BadRequest(false);

            return _authService.CheckUserAuth(_db, credentials.Token) != 0;
        }

        [HttpPost] // user auth
        public async Task<ActionResult<Credentials>> Post(Credentials credentials)
        {
            if (credentials.DeviceId == "" || credentials.Email == "" || credentials.Password == "")
                return BadRequest(new Credentials());

            var auth = _authService.SignInEmail(_db, credentials);

            if (auth == null || auth.AuthToken == "") return Unauthorized();

            credentials.Token = auth.AuthToken;

            return Ok( credentials );
        }

        [HttpDelete]
        public async Task<ActionResult<bool>> Delete(Credentials credentials)
        {
            if (credentials.Token == "" || credentials.DeviceId == "")
                return BadRequest(false);

            var ret =_authService.SignOut(_db, credentials);

            return Ok( ret );
        }

        [HttpPost] // renew user auth
        [Route("api/[controller]/renew")]
        public async Task<ActionResult<Credentials>> UpdateAuth(Credentials credentials)
        {
            if (credentials.DeviceId == "" || credentials.Email == "" || credentials.Password == "")
                return BadRequest(new Credentials());

            var auth = _authService.RenewAuth(_db, credentials);

            if (auth == null || auth.AuthToken == "") return Unauthorized();

            credentials.Token = auth.AuthToken;

            return Ok(credentials);
        }

        /*[Route("api/[controller]/refresh")]
        [HttpPost] // user renew auth 
        public async Task<ActionResult<Credentials>> Post(Credentials credentials)
        {
            if (credentials.DeviceId == "" || credentials.Email == "" || credentials.Password == "")
                return BadRequest(new Credentials());

            var auth = _authService.SignInEmail(credentials);

            if (auth.AuthToken == "") return Unauthorized();

            await _db.Auths.AddAsync(auth);
            await _db.SaveChangesAsync();

            credentials.Token = auth.AuthToken;

            return Ok(credentials);
        }*/
    }
}
