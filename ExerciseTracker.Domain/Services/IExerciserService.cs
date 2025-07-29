using ExerciseTracker.Core.DTOs;
using ExerciseTracker.Core.Results;

namespace ExerciseTracker.Domain.Services;

public interface IExerciserService
{
    Task<Result> CreateExerciserAsync(CreateExerciserRequest request, CancellationToken cancellationToken = default);

    Task<Result> UpdateExerciserAsync(UpdateExerciserRequest request, CancellationToken cancellationToken = default);

    Task<Result> DeleteExerciserAsync(int id, CancellationToken cancellationToken = default);

    Task<Result<ExerciserDto>> GetExerciserByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<ExerciserDto>>> GetExercisersAsync(CancellationToken cancellationToken);
}