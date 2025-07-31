using ExerciseTracker.Core.Extensions;
using ExerciseTracker.Core.Models;
using ExerciseTracker.Core.Results;
using ExerciseTracker.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ExerciseTracker.DataAccess.Repositories;

public class ExerciseRepository : IExerciseRepository
{
    private readonly ExerciseTrackerDbContext _context;
    private readonly ILogger<ExerciseRepository> _logger;

    public ExerciseRepository(ExerciseTrackerDbContext context, ILogger<ExerciseRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result> CreateExerciseAsync(Exercise exercise, CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Exercises
                .AddAsync(exercise, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
        catch (DbUpdateException ex)
        {
            return _logger.LogErrorAndReturnFail($"Database error during exercise session creation: {ex.Message}", ex);
        }
        catch (OperationCanceledException ex)
        {
            return _logger.LogErrorAndReturnFail($"Operation canceled during exercise session creation: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            return _logger.LogErrorAndReturnFail($"An unexpected error has occurred during exercise session creation: {ex.Message}", ex);
        }
    }

    public async Task<Result> UpdateExerciseAsync(Exercise exercise, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Exercises.Update(exercise);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
        catch (DbUpdateException ex)
        {
            return _logger.LogErrorAndReturnFail($"Database error during exercise session update: {ex.Message}", ex);
        }
        catch (OperationCanceledException ex)
        {
            return _logger.LogErrorAndReturnFail($"Operation canceled during exercise session update: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            return _logger.LogErrorAndReturnFail($"An unexpected error has occurred during exercise session update: {ex.Message}", ex);
        }
    }

    public async Task<Result> DeleteExerciseAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var exerciseToDelete = await _context.Exercises
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            if (exerciseToDelete is null)
                return _logger.LogErrorAndReturnFail($"No exercise session with id: {id} found. Nothing deleted");

            _context.Exercises.Remove(exerciseToDelete);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
        catch (DbUpdateException ex)
        {
            return _logger.LogErrorAndReturnFail($"Database error during deletion of exercise session: {id}: {ex.Message}", ex);
        }
        catch (OperationCanceledException ex)
        {
            return _logger.LogErrorAndReturnFail($"Operation canceled during deletion of exercise session: {id}: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            return _logger.LogErrorAndReturnFail($"An unexpected error has occurred during deletion of exercise session: {id}: {ex.Message}", ex);
        }
    }

    public async Task<Result<Exercise>> GetExerciseByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var exercise = await _context.Exercises
                .Include(e => e.Exerciser)
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            return exercise is null
                ? _logger.LogErrorAndReturnFail<Exercise>($"Exercise session with id: {id} not found.")
                : Result<Exercise>.Ok(exercise);

        }
        catch (OperationCanceledException ex)
        {
            return _logger.LogErrorAndReturnFail<Exercise>($"Operation canceled during retrieval of exercise session: {id}: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            return _logger.LogErrorAndReturnFail<Exercise>($"An unexpected error has occurred during retrieval of exercise session: {id}: {ex.Message}", ex);
        }
    }

    public async Task<Result<IReadOnlyList<Exercise>>> GetExercisesByExerciserIdAsync(int exerciserId, CancellationToken cancellationToken = default)
    {
        try
        {
            var exercises = await _context.Exercises
                .Where(e => e.ExerciserId == exerciserId)
                .Include(e => e.Exerciser)
                .ToListAsync(cancellationToken);

            return Result<IReadOnlyList<Exercise>>.Ok(exercises);
        }
        catch (OperationCanceledException ex)
        {
            return _logger.LogErrorAndReturnFail<IReadOnlyList<Exercise>>($"Operation canceled during retrieval of exercise sessions for exerciser: {exerciserId}: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            return _logger.LogErrorAndReturnFail<IReadOnlyList<Exercise>>($"An unexpected error has occurred during retrieval of exercise sessions for exerciser: {exerciserId}: {ex.Message}", ex);
        }
    }

    public async Task<Result<IReadOnlyList<Exercise>>> GetExercisesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var exercises = await _context.Exercises
                .Include(e => e.Exerciser)
                .ToListAsync(cancellationToken);

            return Result<IReadOnlyList<Exercise>>.Ok(exercises);
        }
        catch (OperationCanceledException ex)
        {
            return _logger.LogErrorAndReturnFail<IReadOnlyList<Exercise>>($"Operation canceled during retrieval of all exercises: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            return _logger.LogErrorAndReturnFail<IReadOnlyList<Exercise>>($"An unexpected error has occurred during retrieval of all exercises: {ex.Message}", ex);
        }
    }
}