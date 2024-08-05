using EventMaster.BLL.DTOs.Implementations.Requests.User;
using FluentValidation;

namespace EventMaster.BLL.Infrastructure.Validators;

public class UserDTOValidator : AbstractValidator<UserDTO>
{
    public UserDTOValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty().WithMessage("Login is required")
            .MaximumLength(50).WithMessage("Login can't be longer than 50 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long")
            .MaximumLength(100).WithMessage("Password can't be longer than 100 characters");
    }
}