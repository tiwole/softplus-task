using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters;

public sealed class ValidationFilter(IServiceProvider serviceProvider) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var failures = new Dictionary<string, string[]>();
        string? firstErrorMessage = null;

        foreach (var argument in context.ActionArguments.Values.Where(value => value is not null))
        {
            var argumentType = argument!.GetType();
            var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);

            if (serviceProvider.GetService(validatorType) is not IValidator validator)
                continue;

            var validationContext = new ValidationContext<object>(argument);
            var result = await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);

            if (result.IsValid)
                continue;

            firstErrorMessage ??= result.Errors.FirstOrDefault()?.ErrorMessage;

            foreach (var group in result.Errors.GroupBy(error => error.PropertyName))
            {
                failures[group.Key] = group
                    .Select(error => error.ErrorMessage)
                    .Distinct()
                    .ToArray();
            }
        }

        if (failures.Count > 0)
        {
            context.Result = new BadRequestObjectResult(new
            {
                Id = Guid.NewGuid(),
                StatusCode = StatusCodes.Status400BadRequest,
                ErrorMessage = firstErrorMessage ?? "Validation failed",
                Errors = failures
            });

            return;
        }

        await next();
    }
}
