using ExerciseTracker.Core.DTOs;
using FluentValidation;

namespace ExerciseTracker.Domain.Validation;

public class CreateExerciseRequestValidator : AbstractValidator<CreateExerciseRequest>
{
    public CreateExerciseRequestValidator()
    {
        RuleFor(x => x.ExerciserId)
            .GreaterThan(0).WithMessage("Exerciser ID must be greater than 0.");

        RuleFor(x => x.StartTime)
            .LessThanOrEqualTo(DateTime.Now).WithMessage("Start time cannot be in the future.");

        RuleFor(x => x.EndTime)
            .LessThanOrEqualTo(DateTime.Now).WithMessage("End time cannot be in the future.")
            .GreaterThan(x => x.StartTime).WithMessage("End time must be after start time.");

        RuleFor(x => x)
            .Must(x => (x.EndTime - x.StartTime).TotalSeconds > 0)
            .WithMessage("Exercise duration must be positive.")
            .When(x => x.StartTime != default && x.EndTime != default);

        RuleFor(x => x.ExerciseType)
            .IsInEnum().WithMessage("Invalid exercise type provided. Must be one of: Weights, Cardio, Yoga, Calisthenics, Other.");

        RuleFor(x => x.Comments)
            .NotEmpty().When(x => x.Comments != null).WithMessage("Comments cannot be empty if provided.")
            .MaximumLength(200).WithMessage("Comments cannot exceed 200 characters if provided.");
    }
}