using ExerciseTracker.Core.Models;
using ExerciseTracker.Core.Results;
using ExerciseTracker.Core.Extensions;
using ExerciseTracker.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ExerciseTracker.DataAccess.Repositories;

public class ExerciserRepository : IExerciserRepository
{
    private readonly ExerciseTrackerDbContext _context;
    private readonly ILogger<ExerciserRepository> _logger;

    public ExerciserRepository(ExerciseTrackerDbContext context, ILogger<ExerciserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    public async Task<Result> CreateExerciserAsync(Exerciser exerciser, CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Exercisers
                .AddAsync(exerciser, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
        catch (DbUpdateException ex)
        {
            return _logger.LogErrorAndReturnFail($"Database error during exerciser creation: {ex.Message}", ex);
        }
        catch (OperationCanceledException ex)
        {
            return _logger.LogErrorAndReturnFail($"Operation canceled during exerciser creation: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            return _logger.LogErrorAndReturnFail($"An unexpected error has occurred during exerciser creation: {ex.Message}", ex);
        }

    }

    public async Task<Result> UpdateExerciserAsync(Exerciser exerciser, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Exercisers.Update(exerciser);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
        catch (DbUpdateException ex)
        {
            return _logger.LogErrorAndReturnFail($"Database error during exerciser update: {ex.Message}", ex);
        }
        catch (OperationCanceledException ex)
        {
            return _logger.LogErrorAndReturnFail($"Operation canceled during exerciser update: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            return _logger.LogErrorAndReturnFail($"An unexpected error has occurred during exerciser update: {ex.Message}", ex);
        }
    }

    public async Task<Result> DeleteExerciserAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var exerciserToDelete = await _context.Exercisers
                .FirstOrDefaultAsync(ex => ex.Id == id, cancellationToken);

            if (exerciserToDelete is null)
                return _logger.LogErrorAndReturnFail($"No exerciser with id: {id} found. Nothing deleted");

            _context.Exercisers.Remove(exerciserToDelete);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
        catch (DbUpdateException ex)
        {
            return _logger.LogErrorAndReturnFail($"Database error during deletion of exerciser: {id}: {ex.Message}", ex);
        }
        catch (OperationCanceledException ex)
        {
            return _logger.LogErrorAndReturnFail($"Operation canceled during deletion of exerciser: {id}: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            return _logger.LogErrorAndReturnFail($"An unexpected error has occurred during deletion of exerciser: {id}: {ex.Message}", ex);
        }

    }

    public async Task<Result<Exerciser>> GetExerciserByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var exerciser = await _context.Exercisers
                .Include(ex => ex.Exercises)
                .FirstOrDefaultAsync(ex => ex.Id == id, cancellationToken);


            return exerciser is null 
                ? _logger.LogErrorAndReturnFail<Exerciser>($"No exerciser with id: {id} found")
                : Result<Exerciser>.Ok(exerciser);
        }
        catch (OperationCanceledException ex)
        {
            return _logger.LogErrorAndReturnFail<Exerciser>($"Operation canceled during retrieval of exerciser: {id}: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            return _logger.LogErrorAndReturnFail<Exerciser>($"An unexpected error has occurred during retrieval of exerciser: {id}: {ex.Message}", ex);
        }
    }

    public async Task<Result<IReadOnlyList<Exerciser>>> GetExercisersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var exercisers = await _context.Exercisers
                .Include(ex => ex.Exercises)
                .ToListAsync();

            return Result<IReadOnlyList<Exerciser>>.Ok(exercisers);
        }
        catch (OperationCanceledException ex)
        {
            return _logger.LogErrorAndReturnFail<IReadOnlyList<Exerciser>>($"Operation canceled during retrieval of all exercisers: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            return _logger.LogErrorAndReturnFail<IReadOnlyList<Exerciser>>($"An unexpected error has occurred during retrieval of all exercisers: {ex.Message}", ex);
        }
    }
}