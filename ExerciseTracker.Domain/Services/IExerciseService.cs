using ExerciseTracker.Core.DTOs;
using ExerciseTracker.Core.Results;

namespace ExerciseTracker.Domain.Services;

public interface IExerciseService
{
    Task<Result> CreateExerciseAsync(CreateExerciseRequest request, CancellationToken cancellationToken = default);

    Task<Result> UpdateExerciseAsync(UpdateExerciseRequest request, CancellationToken cancellationToken = default);

    Task<Result> DeleteExerciseAsync(int id, CancellationToken cancellationToken = default);

    Task<Result<ExerciseDto>> GetExerciseByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<ExerciseDto>>> GetExercisesByExerciserIdAsync(int exerciserId,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<ExerciseDto>>> GetExercisesAsync(CancellationToken cancellationToken = default);
}