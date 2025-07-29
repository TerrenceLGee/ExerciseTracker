using ExerciseTracker.Core.DTOs;
using FluentValidation;

namespace ExerciseTracker.Domain.Validation;

public class UpdateExerciserRequestValidator : AbstractValidator<UpdateExerciserRequest>
{
    public UpdateExerciserRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Exerciser id must be greater than 0");

        RuleFor(x => x.Name)
            .NotEmpty().When(x => x.Name != null).WithMessage("Name cannot be empty if provided.")
            .Length(2, 50).When(x => x.Name != null).WithMessage("Name must between 2 and 50 characters if provided.");

        RuleFor(x => x.BirthDate)
            .LessThan(DateTime.Today).When(x => x.BirthDate.HasValue)
            .WithMessage("Birth date must be in the past if provided.")
            .Must(BeAValidBirthDateForUpdate).When(x => x.BirthDate.HasValue).WithMessage("Must be a valid birth date if provided.");

        RuleFor(x => x.BodyWeight)
            .GreaterThan(0).When(x => x.BodyWeight.HasValue)
            .WithMessage("Body weight must be greater than 0 if provided.");

        RuleFor(x => x.FitnessGoal)
            .MaximumLength(200).When(x => x.Name != null).WithMessage("Fitness goal cannot exceed 200 characters if provided.");

        RuleFor(x => x)
            .Must(request => request.Name != null ||
                             request.BirthDate.HasValue ||
                             request.BodyWeight.HasValue ||
                             request.FitnessGoal != null)
            .WithMessage("At least one field (Name, BirthDate, BodyWeight, FitnessGoal) must be provided for update.");
    }

    private bool BeAValidBirthDateForUpdate(DateTime? birthDate)
    {
        if (!birthDate.HasValue)
        {
            return false;
        }

        return birthDate.Value <= DateTime.Today && birthDate.Value >= DateTime.Today.AddYears(-120);
    }
}