using gymBackendC__.WebApi.Models.DTO;

using FluentValidation;

namespace gymBackendC__.WebApi.Validators;

public class TrainingsRequestModelValidator : AbstractValidator<TrainingsRequestModel>
{
    public TrainingsRequestModelValidator()
    {
        RuleFor(training => training.Date).NotEmpty().WithMessage("Date is required");
        RuleFor(training => training.Users).NotEmpty().WithMessage("Users is required");
    }
}
