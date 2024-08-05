using EventMaster.BLL.DTOs.Responses.User;
using FluentValidation;

namespace EventMaster.BLL.Infrastructure.Validators;

public class TokenDTOValidator : AbstractValidator<TokenDTO>
{
    public TokenDTOValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty().WithMessage("AccessToken is required");

        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("RefreshToken is required");
    }
}