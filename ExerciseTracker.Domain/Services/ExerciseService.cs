using AutoMapper;
using ExerciseTracker.Core.DTOs;
using ExerciseTracker.Core.Extensions;
using ExerciseTracker.Core.Models;
using ExerciseTracker.Core.Results;
using ExerciseTracker.DataAccess.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ExerciseTracker.Domain.Services;

public class ExerciseService : IExerciseService
{
    private readonly IExerciseRepository _repository;
    private readonly IExerciserRepository _exerciserRepository;
    private readonly ILogger<ExerciseService> _logger;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateExerciseRequest> _createExerciseValidator;
    private readonly IValidator<UpdateExerciseRequest> _updateExerciseValidator;

    public ExerciseService(
        IExerciseRepository repository,
        IExerciserRepository exerciserRepository,
        ILogger<ExerciseService> logger,
        IMapper mapper,
        IValidator<CreateExerciseRequest> createExerciseValidator,
        IValidator<UpdateExerciseRequest> updateExerciseValidator)
    {
        _repository = repository;
        _exerciserRepository = exerciserRepository;
        _logger = logger;
        _mapper = mapper;
        _createExerciseValidator = createExerciseValidator;
        _updateExerciseValidator = updateExerciseValidator;
    }

    public async Task<Result> CreateExerciseAsync(CreateExerciseRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _createExerciseValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errorMessage = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            return _logger.LogErrorAndReturnFail(errorMessage);
        }

        var exerciserExistsResult =
            await _exerciserRepository.GetExerciserByIdAsync(request.ExerciserId, cancellationToken);

        if (!exerciserExistsResult.IsSuccess)
            return _logger.LogErrorAndReturnFail($"Exerciser with Id {request.ExerciserId} does not exist.");

        var exercise = _mapper.Map<Exercise>(request);

        return await _repository.CreateExerciseAsync(exercise, cancellationToken);
    }

    public async Task<Result> UpdateExerciseAsync(UpdateExerciseRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _updateExerciseValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errorMessage = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            return _logger.LogErrorAndReturnFail(errorMessage);
        }

        if (request.Id <= 0)
            return _logger.LogErrorAndReturnFail("Exercise id is required for update.");

        var existingExerciseResult = await _repository.GetExerciseByIdAsync(request.Id, cancellationToken);

        if (!existingExerciseResult.IsSuccess)
            return _logger.LogErrorAndReturnFail($"{existingExerciseResult.ErrorMessage}");

        var existingExercise = existingExerciseResult.Value!;

        _mapper.Map(request, existingExercise);

        return await _repository.UpdateExerciseAsync(existingExercise, cancellationToken);
    }

    public async Task<Result> DeleteExerciseAsync(int id, CancellationToken cancellationToken = default)
    {
        var validExerciseResult = await _repository.GetExerciseByIdAsync(id, cancellationToken);

        if (!validExerciseResult.IsSuccess)
            return _logger.LogErrorAndReturnFail($"{validExerciseResult.ErrorMessage}");

        return await _repository.DeleteExerciseAsync(id, cancellationToken);
    }

    public async Task<Result<ExerciseDto>> GetExerciseByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var validExerciseResult = await _repository.GetExerciseByIdAsync(id, cancellationToken);

        if (!validExerciseResult.IsSuccess)
            return _logger.LogErrorAndReturnFail<ExerciseDto>($"{validExerciseResult.ErrorMessage}");

        var exercise = validExerciseResult.Value!;

        var exerciseDto = _mapper.Map<ExerciseDto>(exercise);

        return Result<ExerciseDto>.Ok(exerciseDto);
    }

    public async Task<Result<IReadOnlyList<ExerciseDto>>> GetExercisesByExerciserIdAsync(int exerciserId, CancellationToken cancellationToken = default)
    {
        var validExercisesResult = await _repository.GetExercisesByExerciserIdAsync(exerciserId, cancellationToken);

        if (!validExercisesResult.IsSuccess)
           return _logger.LogErrorAndReturnFail<IReadOnlyList<ExerciseDto>>($"{validExercisesResult.ErrorMessage}");

        var exercises = validExercisesResult.Value!;

        var exerciseDtos = _mapper.Map<IReadOnlyList<ExerciseDto>>(exercises);

        return Result<IReadOnlyList<ExerciseDto>>.Ok(exerciseDtos);
    }

    public async Task<Result<IReadOnlyList<ExerciseDto>>> GetExercisesAsync(CancellationToken cancellationToken = default)
    {
        var validExercisesResult = await _repository.GetExercisesAsync(cancellationToken);

        if (!validExercisesResult.IsSuccess)
            return _logger.LogErrorAndReturnFail<IReadOnlyList<ExerciseDto>>($"{validExercisesResult.ErrorMessage}");

        var exercises = validExercisesResult.Value!;

        var exerciseDtos = _mapper.Map<IReadOnlyList<ExerciseDto>>(exercises);

        return Result<IReadOnlyList<ExerciseDto>>.Ok(exerciseDtos);
    }
}