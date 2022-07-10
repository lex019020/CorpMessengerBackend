using CorpMessengerBackend.Models;
using Microsoft.AspNetCore.Http;

namespace CorpMessengerBackend.Services;

public class HttpContextUserAuthProvider : IUserAuthProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextUserAuthProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public (bool IsAuthorized, long UserId) GetUserAuth()
    {
        var reqUserId = _httpContextAccessor.HttpContext?.Items["UserId"];
        return reqUserId is long userId && userId != 0
            ? (true, userId) 
            : (false, 0);
    }

    public bool GetAdminAuth()
    {
        var reqIsAdmin = _httpContextAccessor.HttpContext?.Items["IsAdmin"];
        return reqIsAdmin is true;
    }

    public string GetToken()
    {
        var reqToken = _httpContextAccessor.HttpContext?.Request.Headers["token"];
        return reqToken.ToString() ?? "";
    }
}