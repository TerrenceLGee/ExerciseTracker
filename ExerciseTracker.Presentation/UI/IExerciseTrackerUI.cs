namespace ExerciseTracker.Presentation.UI;

public interface IExerciseTrackerUI
{
    Task RunAsync(CancellationToken cancellationToken = default);
    Task CreateExerciserAsync(CancellationToken cancellationToken = default);
    Task UpdateExerciserAsync(CancellationToken cancellationToken = default);
    Task DeleteExerciserAsync(CancellationToken cancellationToken = default);
    Task ViewExerciserByIdAsync(CancellationToken cancellationToken = default);
    Task ViewAllExercisersAsync(CancellationToken cancellationToken = default);
    Task TrackExerciseSessionAsync(CancellationToken cancellationToken = default);
    Task UpdateTrackedExerciseSessionAsync(CancellationToken cancellationToken = default);
    Task DeleteTrackedExerciseSessionAsync(CancellationToken cancellationToken = default);
    Task ViewTrackedExerciseSessionByIdAsync(CancellationToken cancellationToken = default);
    Task ViewTrackedExerciseSessionsByExerciserIdAsync(CancellationToken cancellationToken = default);
    Task ViewAllTrackedExerciseSessionsAsync(CancellationToken cancellationToken = default);
}