﻿using System;
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

        public Auth SignInEmail(AppDataContext context, Credentials credentials)
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

        public bool SignOut(AppDataContext context, Credentials credentials)
        {
            var authToDel = context.Auths.FirstOrDefault(a => a.AuthToken == credentials.Token);

            if (authToDel == null) return false;

            context.Auths.Remove(authToDel);
            context.SaveChanges();

            return true;
        }

        public bool SignOutFull(AppDataContext context, string userId)
        {
            var authToDel = context.Auths.FirstOrDefault(a => a.UserId == userId);

            if (authToDel == null) return false;

            do
            {
                context.Auths.Remove(authToDel);

                authToDel = context.Auths.FirstOrDefault(a => a.UserId == userId);
            } while (authToDel != null);

            context.SaveChanges();

            return true;
        }

        public string CreateUser(AppDataContext context, Credentials credentials)
        {
            throw new NotImplementedException();
        }

        public string CheckUserAuth(AppDataContext context, string token)
        {
            var auth = context.Auths.FirstOrDefault(a => a.AuthToken == token);

            if (auth == null
                ||auth.Modified.AddDays(7) < DateTime.Now
                ||!context.Users.Any(u => u.UserId == auth.UserId && !u.Deleted)) 
                return "";
            
            return auth.UserId;
        }

        public Auth RenewAuth(AppDataContext context, Credentials credentials)
        {
            var auth = context.Auths.FirstOrDefault(a => a.AuthToken == credentials.Token 
                                                         && a.DeviceId == credentials.DeviceId);

            if (auth == null 
            || !context.Users.Any(u => !u.Deleted && u.UserId == auth.UserId)) 
                return null;

            auth.AuthToken = CryptographyService.GenerateNewToken();
            auth.Modified = DateTime.Now;

            context.Auths.Update(auth);

            return auth;
        }
    }
}
