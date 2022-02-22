using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CorpMessengerBackend.Models;

namespace CorpMessengerBackend.Services
{
    public interface IAuthService
    {
        /// <summary>
        /// Sign in
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns>Auth object</returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public Auth SignInEmail(Credentials credentials);

        public bool SignOut(Credentials credentials);

        /// <summary>
        /// Full sign-out
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool SignOutFull(string userId);

        /// <summary>
        /// Create new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns>UserId</returns>
        /// <exception cref="Exception"></exception>
        public string CreateUser(Credentials credentials);

        /// <summary>
        /// Check if user is logged in
        /// </summary>
        /// <param name="token">Auth token</param>
        /// <returns>UserId</returns>
        public string CheckUserAuth(string token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns></returns>
        public Auth RenewAuth(Credentials credentials);
    }
}
