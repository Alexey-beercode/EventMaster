using EventMaster.BLL.DTOs.Implementations.Requests.Participant;
using FluentValidation;

namespace EventMaster.BLL.Infrastructure.Validators;

public class CreateParticipantDTOValidator : AbstractValidator<CreateParticipantDTO>
{
    public CreateParticipantDTOValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("FirstName is required")
            .MaximumLength(50).WithMessage("FirstName can't be longer than 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("LastName is required")
            .MaximumLength(50).WithMessage("LastName can't be longer than 50 characters");

        RuleFor(x => x.BirthDate)
            .LessThan(DateTime.UtcNow).WithMessage("BirthDate must be in the past");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid Email format");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");

        RuleFor(x => x.EventId)
            .NotEmpty().WithMessage("EventId is required");
    }
}