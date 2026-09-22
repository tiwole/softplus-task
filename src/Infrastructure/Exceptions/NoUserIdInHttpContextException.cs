namespace Infrastructure.Exceptions;

public class NoUserIdInHttpContextException(string message) : SoftPlusException(message);
