﻿using System;
using System.Threading.Tasks;
using CorpMessengerBackend.HttpObjects;
using CorpMessengerBackend.Models;

namespace CorpMessengerBackend.Services;

public interface IAuthService
{
    /// <summary>
    ///     Sign in
    /// </summary>
    /// <param name="credentials"></param>
    /// <returns>Auth object</returns>
    /// <exception cref="UnauthorizedAccessException"></exception>
    public Task<Auth?> SignInEmail(IAppDataContext context, Credentials credentials);

    public Task<bool> SignOut(IAppDataContext context, Credentials credentials);

    /// <summary>
    ///     Full sign-out
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public Task<bool> SignOutFull(IAppDataContext context, long userId);

    /// <summary>
    ///     Check if user is logged in
    /// </summary>
    /// <param name="token">Auth token</param>
    /// <returns>UserId</returns>
    public long CheckUserAuth(IAppDataContext context, string token);

    /// <summary>
    ///     Check for admin token
    /// </summary>
    /// <param name="token">Auth token</param>
    /// <returns>Is auth correct</returns>
    public bool CheckAdminAuth(IAppDataContext context, string token);

    /// <summary>
    /// </summary>
    /// <param name="credentials"></param>
    /// <returns></returns>
    public Task<Auth?> RenewAuth(IAppDataContext context, Credentials credentials);
}