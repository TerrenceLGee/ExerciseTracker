using ExerciseTracker.Core.DTOs;
using ExerciseTracker.Core.Models;
using FluentValidation;

namespace ExerciseTracker.Domain.Validation;

public class UpdateExerciseRequestValidator : AbstractValidator<UpdateExerciseRequest>
{
    public UpdateExerciseRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Exercise id must be greater than 0");

        RuleFor(x => x.StartTime)
            .LessThanOrEqualTo(DateTime.Now)
            .When(x => x.StartTime.HasValue)
            .WithMessage("Start time cannot be in the future if provided.");

        RuleFor(x => x.EndTime)
            .LessThanOrEqualTo(DateTime.Now)
            .When(x => x.EndTime.HasValue)
            .WithMessage("End time cannot be in the future if provided.");

        RuleFor(x => x)
            .Must(x => (x.EndTime!.Value - x.StartTime!.Value).TotalSeconds > 0)
            .When(x => x.StartTime.HasValue && x.EndTime.HasValue)
            .WithMessage("Exercise duration must be positive if start and end times are provided.");

        RuleFor(x => x.ExerciseType)
            .IsInEnum()
            .When(x => x.ExerciseType.HasValue)
            .WithMessage(
                $"Invalid exercise type. If provided must be one of: {string.Join(", ", Enum.GetNames(typeof(ExerciseType)))}.");

        RuleFor(x => x.Comments)
            .NotEmpty().When(x => x.Comments != null).WithMessage("Comments cannot be empty if provided.")
            .MaximumLength(200).WithMessage("Comments cannot exceed 200 characters if provided.");

        RuleFor(x => x)
            .Must(request => request.StartTime.HasValue ||
                             request.EndTime.HasValue ||
                             request.ExerciseType.HasValue ||
                             request.Comments != null)
            .WithMessage(
                "At least one field (StartTime, EndTime, ExerciseType, Comments) must be provided for update.");
    }
}