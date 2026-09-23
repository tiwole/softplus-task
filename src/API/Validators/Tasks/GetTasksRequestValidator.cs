using API.Validators.Models;
using FluentValidation;
using Shared.Tasks;

namespace API.Validators.Tasks;

public sealed class GetTasksRequestValidator : PaginatedRequestValidator<GetTasksRequest>
{
    public GetTasksRequestValidator()
    {
        RuleFor(request => request.Search)
            .MaximumLength(256);
    }
}
