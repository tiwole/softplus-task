namespace Core.Exceptions;

public class CategoryException : SoftPlusException
{
    public CategoryException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}
