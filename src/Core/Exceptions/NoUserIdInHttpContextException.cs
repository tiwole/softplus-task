namespace Core.Exceptions;

public class NoUserIdInHttpContextException : SoftPlusException
{
    public NoUserIdInHttpContextException(string message) : base(message)
    {
        StatusCode = 500;
    }
}
