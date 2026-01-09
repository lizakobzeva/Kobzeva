using gymBackendC__.WebApi.Models.DTO;

using FluentValidation;

namespace gymBackendC__.WebApi.Validators;

public class RegisterRequestModelValidator : AbstractValidator<RegisterRequestModel>
{
    public RegisterRequestModelValidator()
    {
        RuleFor(user => user.Username).NotEmpty().WithMessage("Username is required.");
        RuleFor(user => user.Email).NotEmpty().WithMessage("Email is required.");
        RuleFor(user => user.Password).NotEmpty().WithMessage("Password is required.");
    }
}