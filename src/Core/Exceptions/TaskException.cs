namespace Core.Exceptions;

public class TaskException : SoftPlusException
{
    public TaskException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}
