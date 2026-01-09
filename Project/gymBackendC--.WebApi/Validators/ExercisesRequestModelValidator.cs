using gymBackendC__.WebApi.Models.DTO;

using FluentValidation;

namespace gymBackendC__.WebApi.Validators;

public class ExercisesRequestModelValidator : AbstractValidator<ExercisesRequestModel>
{
    public ExercisesRequestModelValidator()
    {
        RuleFor(exercise => exercise.TrainingId).NotEmpty().WithMessage("TrainingId is required.");
        RuleFor(exercise => exercise.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(exercise => exercise.Repeats).NotEmpty().WithMessage("Repeats is required.");
        RuleFor(exercise => exercise.Weight).NotEmpty().WithMessage("Weight is required.");
    }
}