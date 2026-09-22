namespace Core.Exceptions;

public class SoftPlusException(string message) : Exception(message)
{
    public int StatusCode = 500;
}
