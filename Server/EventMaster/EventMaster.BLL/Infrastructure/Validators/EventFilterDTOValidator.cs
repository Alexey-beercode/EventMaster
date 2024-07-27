using EventMaster.BLL.DTOs.Requests.Event;
using FluentValidation;

namespace EventMaster.BLL.Infrastructure.Validators;

public class EventFilterDTOValidator : AbstractValidator<EventFilterDto>
{
    public EventFilterDTOValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("PageNumber must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be greater than 0");
    }
}