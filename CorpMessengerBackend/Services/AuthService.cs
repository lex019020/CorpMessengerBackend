﻿using System;
using CorpMessengerBackend.Models;
using Firebase.Auth;
using FirebaseAdmin;
using User = CorpMessengerBackend.Models.User;

namespace CorpMessengerBackend.Services
{
    public class AuthService : IAuthService
    {
        private FirebaseAuthProvider _auth;
        //Firebase.Auth.FirebaseAuth auth;// = Firebase.Auth.FirebaseAuth.DefaultInstance;


        public AuthService(FirebaseConfig config)
        {
            _auth = new FirebaseAuthProvider(config);
            
            var defaultApp = FirebaseApp.Create();
        }

        public Auth SignInEmail(Credentials credentials)
        {
            var resAuth = new Auth();

            _auth.SignInWithEmailAndPasswordAsync(credentials.Email, credentials.Password)
                .ContinueWith(task =>
                {
                    if (task.IsCanceled)
                    {
                        // todo log
                        return;
                    }

                    if (task.IsFaulted)
                    {
                        // todo log
                        if (task.Exception != null) throw task.Exception;
                        return;
                    }
                    // todo log

                    var newUser = task.Result.User;
                    var token = task.Result.FirebaseToken;
                    resAuth = new Auth
                    {
                        AuthToken = token,
                        DeviceId = credentials.DeviceId,
                        UserId = newUser.LocalId,
                        Modified = DateTime.Now
                    };
                });

            return resAuth;
        }

        public bool SignOut(Credentials credentials)
        {
            //throw new NotImplementedException();
            return false;
        }

        public string CreateUser(Credentials credentials)
        {
            var userId = "";

            _auth.CreateUserWithEmailAndPasswordAsync(credentials.Email, credentials.Password).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    //todo log
                    return;
                }
                if (task.IsFaulted)
                {
                    //todo log
                    if (task.Exception != null) throw task.Exception;
                    return;
                }
                
                var newUser = task.Result;
                // todo log

                userId = newUser.User.LocalId;

            });

            return userId;
        }

        public string CheckUserAuth(string token)
        {
            try
            {
                return _auth.GetUserAsync(token).Result.LocalId;
            }
            catch (FirebaseAuthException e)
            {
                Console.WriteLine(e);
                return "";
            }
            //throw new NotImplementedException();
        }
    }
}
