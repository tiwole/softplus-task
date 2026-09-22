using Core.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Middleware;

public class ExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var statusCode = exception switch
        {
            SoftPlusException x => x.StatusCode,
            _ => StatusCodes.Status500InternalServerError
        };

        httpContext.Response.StatusCode = statusCode;

        var error = new
        {
            Id = Guid.NewGuid(),
            StatusCode = statusCode,
            ErrorMessage = exception.Message
        };

        await httpContext.Response.WriteAsJsonAsync(error, cancellationToken);

        return true;
    }
}
