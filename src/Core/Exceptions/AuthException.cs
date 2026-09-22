namespace Core.Exceptions;

public class AuthException : SoftPlusException
{
    public AuthException(string message) : base(message)
    {
        StatusCode = 400;
    }
}
