using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
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

        public AuthController(AuthContext context, IAuthService authService)
        {
            _db = context;
            _authService = authService;
        }

        [HttpGet] // check user auth
        public async Task<ActionResult<bool>> Get(Credentials credentials)
        {
            if (credentials.DeviceId == "" || credentials.Token == "")
                return BadRequest(false);

            // todo check with auth service

            return await _db.Auths.AnyAsync(a => a.AuthToken == credentials.Token
                                                && a.DeviceId == credentials.DeviceId);
        }

        [HttpPost] // user auth //todo recheck this siht
        public async Task<ActionResult<Credentials>> Post(Credentials credentials)
        {
            if (credentials.DeviceId == "" || credentials.Email == "" || credentials.Password == "")
                return BadRequest(new Credentials());

            var auth = _authService.SignInEmail(credentials);

            if (auth == null || auth.AuthToken == "") return Unauthorized();
            
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

            var authToDel = _db.Auths.First(
                au => au.AuthToken == credentials.Token
                      && au.DeviceId == credentials.DeviceId);

            if (authToDel == null) return Ok( false );

            _db.Auths.Remove( authToDel );
            await _db.SaveChangesAsync();

            return Ok( true );
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
