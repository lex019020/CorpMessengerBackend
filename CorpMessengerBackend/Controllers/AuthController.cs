using System.Threading.Tasks;
using CorpMessengerBackend.HttpObjects;
using CorpMessengerBackend.Models;
using CorpMessengerBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace CorpMessengerBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAppDataContext _db;
    private readonly IUserAuthProvider _authProvider;
    private readonly IAuthService _authService;

    public AuthController(IAppDataContext context, IAuthService authService, IUserAuthProvider authProvider)
    {
        _db = context;
        _authService = authService;
        _authProvider = authProvider;
    }

    [HttpGet] // check user auth
    public Task<ActionResult<long>> Get(Credentials credentials)
    {
        if (   string.IsNullOrEmpty(credentials.DeviceId) 
            || string.IsNullOrEmpty(_authProvider.GetToken()))
            return Task.FromResult<ActionResult<long>>(BadRequest(0));

        return Task.FromResult<ActionResult<long>>(_authProvider.GetUserAuth().UserId);
    }

    [HttpPost] // user auth
    public async Task<ActionResult<Credentials>> Post(Credentials credentials)
    {
        if (credentials.DeviceId == "" || credentials.Email == "" || credentials.Password == "")
            return BadRequest();

        var newAuth = await _authService.SignInEmail(_db, credentials);///////////////////

        if (newAuth == null || newAuth.AuthToken == "") return Unauthorized();
        
        credentials.Token = newAuth.AuthToken;

        return Ok(credentials);
    }

    [HttpDelete]
    public Task<ActionResult<bool>> Delete(Credentials credentials)
    {
        if (string.IsNullOrEmpty(_authProvider.GetToken()) 
            || string.IsNullOrEmpty(credentials.DeviceId))
            return Task.FromResult<ActionResult<bool>>(BadRequest(false));

        var signOutResult = _authService.SignOut(_db);

        return Task.FromResult<ActionResult<bool>>(Ok(signOutResult));
    }

    [HttpPost("renew")] // renew user auth
    public async Task<ActionResult<Credentials>> UpdateAuth(Credentials credentials)
    {
        if (string.IsNullOrEmpty(_authProvider.GetToken()) 
            || string.IsNullOrEmpty(credentials.DeviceId))
            return BadRequest();

        var renewedAuth = await _authService.RenewAuth(_db, credentials);

        if (renewedAuth == null || renewedAuth.AuthToken == "") return Unauthorized();

        credentials.Token = renewedAuth.AuthToken;

        return Ok(credentials);
    }
}