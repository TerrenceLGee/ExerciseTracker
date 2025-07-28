using ExerciseTracker.Core.Models;
using ExerciseTracker.Core.Results;

namespace ExerciseTracker.DataAccess.Repositories;

public interface IExerciserRepository
{
    Task<Result> CreateExerciserAsync(Exerciser exerciser, CancellationToken cancellationToken = default);
    Task<Result> UpdateExerciserAsync(Exerciser exerciser, CancellationToken cancellationToken = default);
    Task<Result> DeleteExerciserAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<Exerciser>> GetExerciserByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<Exerciser>>> GetExercisersAsync(CancellationToken cancellationToken = default);
}