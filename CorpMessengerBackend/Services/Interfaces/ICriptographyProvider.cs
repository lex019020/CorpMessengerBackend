namespace CorpMessengerBackend.Services;

public interface ICriptographyProvider
{
    public string GenerateNewToken();
    public string HashPassword(string password, byte[]? salt = null);
    public bool CheckPassword(string password, string secret);
}