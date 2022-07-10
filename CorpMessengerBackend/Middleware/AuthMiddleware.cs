using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CorpMessengerBackend.Models;
using CorpMessengerBackend.Services;
using Microsoft.AspNetCore.Http;

namespace CorpMessengerBackend.Middleware;

public class AuthMiddleware : IMiddleware
{
    private readonly IAuthService _authService;
    private readonly IAppDataContext _dataContext;
    private readonly List<string> _authNotRequired = new() {"api/auth"};

    public AuthMiddleware(IAuthService authService, IAppDataContext dataContext)
    {
        _authService = authService;
        _dataContext = dataContext;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        string token = context.Request.Headers["token"];
        
        if (token == "")
            token = context.Request.Query["token"];
        
        if (token != "")
        {
            var userId = _authService.CheckUserAuth(_dataContext, token);
            var isAdmin = _authService.CheckAdminAuth(_dataContext, token);
            
            if (userId != 0)
                context.Items["UserId"] = userId;

            if (isAdmin)
                context.Items["IsAdmin"] = true;

            if (!isAdmin && userId == 0)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                // todo log ?
                return;
            }
        }
        else
        {
            if (_authNotRequired.All(path => context.Request.Path.ToString() != path))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }
        }

        await next.Invoke(context);
    }
}