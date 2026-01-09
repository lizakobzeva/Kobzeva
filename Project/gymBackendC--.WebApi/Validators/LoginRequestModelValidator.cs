using gymBackendC__.WebApi.Models.DTO;

using FluentValidation;

namespace gymBackendC__.WebApi.Validators;

public class LoginRequestModelValidator : AbstractValidator<LoginRequestModel>
{
    public LoginRequestModelValidator()
    {
        RuleFor(user => user.Username).NotEmpty().WithMessage("Username is required");
        RuleFor(user => user.Password).NotEmpty().WithMessage("Password is required");
    }
}