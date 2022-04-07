using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CorpMessengerBackend.HttpObjects;
using CorpMessengerBackend.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CorpMessengerBackend.Services
{
    public class LocalAuthService : IAuthService
    {

        public LocalAuthService()
        {
        }

        // todo DeviceId!!

        public async Task<Auth> SignInEmail(AppDataContext context, Credentials credentials)
        {
            var user = context.Users.FirstOrDefault(u => u.Email == credentials.Email);

            if (user == null) return null;

            var secret = context.UserSecrets.FirstOrDefault(
                    s => s.UserId == user.UserId)
                ?.Secret;
            

            if (secret == null)
            {
                // todo log 
                throw new Exception("No secret found for user!");
            }

            if (!CryptographyService.CheckPassword(credentials.Password, secret))
                throw new UnauthorizedAccessException("Incorrect password");

            var newToken = CryptographyService.GenerateNewToken();

            var newAuth = (await context.Auths.AddAsync(new Auth
            {
                AuthToken = newToken, 
                DeviceId = credentials.DeviceId, 
                UserId = user.UserId,
                Modified = DateTime.Now
            })).Entity;

            await context.SaveChangesAsync();

            return newAuth;
        }

        public async Task<bool> SignOut(AppDataContext context, Credentials credentials)
        {
            var authToDel = context.Auths.FirstOrDefault(a => a.AuthToken == credentials.Token);

            if (authToDel == null) return false;

            context.Auths.Remove(authToDel);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SignOutFull(AppDataContext context, long userId)
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

        public long CheckUserAuth(AppDataContext context, string token)
        {
            var auth = context.Auths.FirstOrDefault(a => a.AuthToken == token);

            if (auth == null
                ||auth.Modified.AddDays(7) < DateTime.Now
                ||!context.Users.Any(u => u.UserId == auth.UserId && !u.Deleted)) 
                return 0;
            
            return auth.UserId;
        }

        public bool CheckAdminAuth(AppDataContext context, string token)
        {
            var a = context.AdminAuths.FirstOrDefault();
            if (a == null)
            {
                // todo log
                return false;
            }

            return token == a.Token;
        }

        public async Task<Auth> RenewAuth(AppDataContext context, Credentials credentials)
        {
            var auth = context.Auths.FirstOrDefault(a => a.AuthToken == credentials.Token 
                                                         && a.DeviceId == credentials.DeviceId);

            if (auth == null 
            || !context.Users.Any(u => !u.Deleted && u.UserId == auth.UserId)) 
                return null;

            auth.AuthToken = CryptographyService.GenerateNewToken();
            auth.Modified = DateTime.Now;

            context.Auths.Update(auth);

            await context.SaveChangesAsync();

            return auth;
        }
    }
}
