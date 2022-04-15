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
        public async Task<ActionResult<long>> Get(Credentials credentials)
        {
            if (credentials.DeviceId == "" || credentials.Token == "")
                return BadRequest(0);

            return _authService.CheckUserAuth(_db, credentials.Token);
        }

        [HttpPost] // user auth
        public async Task<ActionResult<Credentials>> Post(Credentials credentials)
        {
            if (credentials.DeviceId == "" || credentials.Email == "" || credentials.Password == "")
                return BadRequest();

            var auth = await _authService.SignInEmail(_db, credentials);

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

        [HttpPost("renew")] // renew user auth
        //[Route("api/[controller]/renew")]
        public async Task<ActionResult<Credentials>> UpdateAuth(Credentials credentials)
        {
            if (credentials.DeviceId == "" || credentials.Token == "")
                return BadRequest();

            var auth = await _authService.RenewAuth(_db, credentials);

            if (auth == null || auth.AuthToken == "") return Unauthorized();

            credentials.Token = auth.AuthToken;

            return Ok(credentials);
        }
    }
}
