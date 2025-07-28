using ExerciseTracker.Core.DTOs;
using FluentValidation;

namespace ExerciseTracker.Domain.Validation;

public class CreateExerciserRequestValidator : AbstractValidator<CreateExerciserRequest>
{
    public CreateExerciserRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name cannot be empty.")
            .Length(2, 50).WithMessage("Name must be between 2 and 50 characters");

        RuleFor(x => x.BirthDate)
            .LessThan(DateTime.Today).WithMessage("Birth date must be in the past.")
            .Must(BeAValidBirthDate).WithMessage("Birth date is not valid.")
            .NotNull().WithMessage("Birth date is required");

        RuleFor(x => x.BodyWeight)
            .GreaterThan(0).When(x => x.BodyWeight.HasValue)
            .WithMessage("Body weight must be greater than 0 if provided.");

        RuleFor(x => x.FitnessGoal)
            .MaximumLength(200).WithMessage("Fitness goal cannot exceed 200 characters if provided.");
    }

    private bool BeAValidBirthDate(DateTime birthDate)
    {
        return birthDate <= DateTime.Today && birthDate >= DateTime.Today.AddYears(-120);
    }
}