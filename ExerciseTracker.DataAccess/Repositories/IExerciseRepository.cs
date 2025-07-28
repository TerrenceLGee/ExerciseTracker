using ExerciseTracker.Core.Results;
using ExerciseTracker.Core.Models;

namespace ExerciseTracker.DataAccess.Repositories;

public interface IExerciseRepository
{
    Task<Result> CreateExerciseAsync(Exercise exercise, CancellationToken cancellationToken = default);
    Task<Result> UpdateExerciseAsync(Exercise exercise, CancellationToken cancellationToken = default);
    Task<Result> DeleteExerciseAsync(int id, CancellationToken cancellationToken = default);

    Task<Result<Exercise>> GetExerciseByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<Exercise>>> GetExercisesByExerciserIdAsync(int exerciserId,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<Exercise>>> GetExercisesAsync(CancellationToken cancellationToken = default);
}