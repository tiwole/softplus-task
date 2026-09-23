using FluentValidation;
using Shared.Models;

namespace API.Validators.Models;

public class PaginatedRequestValidator<TRequest> : AbstractValidator<TRequest>
    where TRequest : PaginatedRequest
{
    public PaginatedRequestValidator()
    {
        RuleFor(request => request.Start)
            .GreaterThanOrEqualTo(0);

        RuleFor(request => request.Limit)
            .InclusiveBetween(1, 100);
    }
}
