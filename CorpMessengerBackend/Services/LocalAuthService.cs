using System;
using System.Linq;
using System.Threading.Tasks;
using CorpMessengerBackend.HttpObjects;
using CorpMessengerBackend.Models;
using Microsoft.AspNetCore.Http;

namespace CorpMessengerBackend.Services;

public class LocalAuthService : IAuthService
{
    // !!!!!!!!!!!!! todo DeviceId !!!!!!!!!!!!!!
    private readonly ICriptographyProvider _cryptographyProvider;
    private readonly IDateTimeService _dateTimeService;
    private readonly IUserAuthProvider _userAuthProvider;

    public LocalAuthService(ICriptographyProvider cryptographyProvider, IDateTimeService dateTimeService, 
        IUserAuthProvider userAuthProvider)
    {
        _cryptographyProvider = cryptographyProvider;
        _dateTimeService = dateTimeService;
        _userAuthProvider = userAuthProvider;
    }

    public async Task<Auth?> SignInEmail(IAppDataContext context, Credentials credentials)
    {
        var user = context.Users.FirstOrDefault(u => u.Email == credentials.Email);

        if (user == null) return null;

        var secret = context.UserSecrets.FirstOrDefault(
                s => s.UserId == user.UserId)
            ?.Secret;


        if (secret == null)
            // todo log 
            throw new Exception($"No secret found for user {user.Email}!");

        if (!_cryptographyProvider.CheckPassword(credentials.Password!, secret))
            return null;

        var newToken = _cryptographyProvider.GenerateNewToken();

        var newAuth = (await context.Auths.AddAsync(new Auth
        {
            AuthToken = newToken,
            DeviceId = credentials.DeviceId!,
            UserId = user.UserId,
            Modified = _dateTimeService.CurrentDateTime
        })).Entity;

        await context.SaveChangesAsync();

        return newAuth;
    }

    public async Task<bool> SignOut(IAppDataContext dataContext, HttpContext httpContext)
    {
        var authToDel = dataContext.Auths.FirstOrDefault(
            a => a.AuthToken == _userAuthProvider.GetToken());

        if (authToDel == null) return false;

        dataContext.Auths.Remove(authToDel);
        await dataContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> SignOutFull(IAppDataContext context, long userId)
    {
        var authToDel = context.Auths.FirstOrDefault(a => a.UserId == userId);

        if (authToDel == null) return false;

        do
        {
            context.Auths.Remove(authToDel);

            authToDel = context.Auths.FirstOrDefault(a => a.UserId == userId);
        } while (authToDel != null);

        await context.SaveChangesAsync();

        return true;
    }

    public long CheckUserAuth(IAppDataContext context, string token)
    {
        var auth = context.Auths.FirstOrDefault(a => a.AuthToken == token);

        if (auth == null
            ||  auth.Modified < _dateTimeService.MinValidTokenDateTime
            || !context.Users.Any(u => u.UserId == auth.UserId && !u.Deleted))
            return 0;

        return auth.UserId;
    }

    public bool CheckAdminAuth(IAppDataContext context, string token)
    {
        var a = context.AdminAuths.FirstOrDefault();
        if (a == null) return false;

        return token == a.Token;
    }

    public async Task<Auth?> RenewAuth(IAppDataContext context, Credentials credentials, HttpContext httpContext)
    {
        var auth = context.Auths.FirstOrDefault(a => a.AuthToken == _userAuthProvider.GetToken()
                                                     && a.DeviceId == credentials.DeviceId);

        if (auth == null
            ||  auth.Modified < _dateTimeService.MinValidTokenDateTime
            || !context.Users.Any(u => !u.Deleted && u.UserId == auth.UserId))
            return null;

        auth.AuthToken = _cryptographyProvider.GenerateNewToken();
        auth.Modified = _dateTimeService.CurrentDateTime;

        context.Auths.Update(auth);

        await context.SaveChangesAsync();

        return auth;
    }
}