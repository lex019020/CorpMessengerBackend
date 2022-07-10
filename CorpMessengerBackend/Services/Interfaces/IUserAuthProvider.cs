using CorpMessengerBackend.Models;
using Microsoft.AspNetCore.Http;

namespace CorpMessengerBackend.Services;

public interface IUserAuthProvider
{
    /// <summary>
    /// Checks for user authorization
    /// </summary>
    /// <returns>Marker of authorization, UserId if auth is ok</returns>
    public (bool IsAuthorized, long UserId) GetUserAuth();
    
    /// <summary>
    /// Check for admin authorisation
    /// </summary>
    /// <returns>True if request is admin-authorized</returns>
    public bool GetAdminAuth();

    /// <summary>
    /// Get request token
    /// </summary>
    /// <returns>Token or empty string</returns>
    public string GetToken();
}