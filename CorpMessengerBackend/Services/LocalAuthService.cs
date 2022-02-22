using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CorpMessengerBackend.Models;

namespace CorpMessengerBackend.Services
{
    public class LocalAuthService : IAuthService
    {
        private readonly AuthContext _dbAuth;
        private readonly UserContext _dbUsers;
        private readonly UserSecretContext _dbUsersSecrets;

        public LocalAuthService(AuthContext authContext, UserContext userContext, 
            UserSecretContext secretContext)
        {
            _dbAuth = authContext;
            _dbUsers = userContext;
            _dbUsersSecrets = secretContext;
        }

        public Auth SignInEmail(Credentials credentials)
        {
            var user = _dbUsers.Users.FirstOrDefault(u => u.Email == credentials.Email);

            if (user == null) return null;

            var secret = _dbUsersSecrets.UserSecrets.FirstOrDefault(
                s => s.UserId == user.UserId)
                ?.Secret;

            if (secret == null)
            {
                // todo log 
                throw new Exception("No secret found for user!");
            }

            if (CryptographyService.HashPassword(credentials.Password) != secret)
                throw new UnauthorizedAccessException("Incorrect password");

            var newToken = CryptographyService.GenerateNewToken();

            var newAuth = new Auth
            {
                AuthToken = newToken, 
                DeviceId = credentials.DeviceId, 
                UserId = user.UserId,
                Modified = DateTime.Now
            };

            return newAuth;
        }

        public string CreateUser(Credentials credentials)
        {
            throw new NotImplementedException();
        }

        public string CheckUserAuth(string token)
        {
            var auth = _dbAuth.Auths.FirstOrDefault(a => a.AuthToken == token);

            if (auth == null
                ||auth.Modified.AddDays(7) < DateTime.Now
                ||!_dbUsers.Users.Any(u => u.UserId == auth.UserId && !u.Deleted)) 
                return "";
            
            return auth.UserId;
        }

        public Auth RenewAuth(Credentials credentials)
        {
            var auth = _dbAuth.Auths.FirstOrDefault(a => a.AuthToken == credentials.Token 
                                                         && a.DeviceId == credentials.DeviceId);

            if (auth == null 
            || !_dbUsers.Users.Any(u => !u.Deleted && u.UserId == auth.UserId)) 
                return null;

            auth.AuthToken = CryptographyService.GenerateNewToken();
            auth.Modified = DateTime.Now;

            _dbAuth.Auths.Update(auth);

            return auth;
        }
    }
}
