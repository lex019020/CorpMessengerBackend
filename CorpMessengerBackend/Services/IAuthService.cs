﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CorpMessengerBackend.HttpObjects;
using CorpMessengerBackend.Models;

namespace CorpMessengerBackend.Services
{
    public interface IAuthService
    {
        // TODO ASYNC


        /// <summary>
        /// Sign in
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns>Auth object</returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public Auth SignInEmail(AppDataContext context, Credentials credentials);

        public bool SignOut(AppDataContext context, Credentials credentials);

        /// <summary>
        /// Full sign-out
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool SignOutFull(AppDataContext context, string userId);

        /// <summary>
        /// Create new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns>UserId</returns>
        /// <exception cref="Exception"></exception>
        public string CreateUser(AppDataContext context, Credentials credentials);

        /// <summary>
        /// Check if user is logged in
        /// </summary>
        /// <param name="token">Auth token</param>
        /// <returns>UserId</returns>
        public string CheckUserAuth(AppDataContext context, string token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns></returns>
        public Auth RenewAuth(AppDataContext context, Credentials credentials);
    }
}
