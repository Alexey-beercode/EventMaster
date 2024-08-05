using EventMaster.BLL.DTOs.Implementations.Requests.Event;
using FluentValidation;

namespace EventMaster.BLL.Infrastructure.Validators;

public class UpdateEventDTOValidator : AbstractValidator<UpdateEventDTO>
{
    public UpdateEventDTOValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name can't be longer than 100 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required");

        RuleFor(x => x.Date)
            .GreaterThan(DateTime.UtcNow).WithMessage("Date must be in the future");

        RuleFor(x => x.Location)
            .NotNull().WithMessage("Location is required");

        RuleFor(x => x.MaxParticipants)
            .GreaterThan(0).WithMessage("MaxParticipants must be greater than 0");

        RuleFor(x => x.Image)
            .NotNull().WithMessage("Image is required");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("CategoryId is required");
    }
}