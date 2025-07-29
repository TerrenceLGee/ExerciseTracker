using AutoMapper;
using ExerciseTracker.Core.DTOs;
using ExerciseTracker.Core.Extensions;
using ExerciseTracker.Core.Models;
using ExerciseTracker.Core.Results;
using ExerciseTracker.DataAccess.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ExerciseTracker.Domain.Services;

public class ExerciserService : IExerciserService
{
    private readonly IExerciserRepository _repository;
    private readonly ILogger<ExerciserService> _logger;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateExerciserRequest> _createExerciserValidator;
    private readonly IValidator<UpdateExerciserRequest> _updateExerciserValidator;

    public ExerciserService(
        IExerciserRepository repository,
        ILogger<ExerciserService> logger,
        IMapper mapper,
        IValidator<CreateExerciserRequest> createExerciserValidator,
        IValidator<UpdateExerciserRequest> updateExerciserValidator)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
        _createExerciserValidator = createExerciserValidator;
        _updateExerciserValidator = updateExerciserValidator;
    }


    public async Task<Result> CreateExerciserAsync(CreateExerciserRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _createExerciserValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errorMessage = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            return _logger.LogErrorAndReturnFail(errorMessage);
        }

        var exerciser = _mapper.Map<Exerciser>(request);

        return await _repository.CreateExerciserAsync(exerciser, cancellationToken);
    }

    public async Task<Result> UpdateExerciserAsync(UpdateExerciserRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _updateExerciserValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errorMessage = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            return _logger.LogErrorAndReturnFail(errorMessage);
        }

        if (request.Id <= 0)
            return _logger.LogErrorAndReturnFail("Exerciser id is required for update.");


        var existingExerciserResult = await _repository.GetExerciserByIdAsync(request.Id, cancellationToken);

        if (!existingExerciserResult.IsSuccess)
           return _logger.LogErrorAndReturnFail($"{existingExerciserResult.ErrorMessage}");

        var existingExerciser = existingExerciserResult.Value!;

        _mapper.Map(request, existingExerciser);

        return await _repository.UpdateExerciserAsync(existingExerciser, cancellationToken);
    }

    public async Task<Result> DeleteExerciserAsync(int id, CancellationToken cancellationToken = default)
    {
        var validExerciserResult = await _repository.GetExerciserByIdAsync(id, cancellationToken);

        if (!validExerciserResult.IsSuccess)
            return _logger.LogErrorAndReturnFail($"{validExerciserResult.ErrorMessage}");

        return await _repository.DeleteExerciserAsync(id, cancellationToken);
    }

    public async Task<Result<ExerciserDto>> GetExerciserByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var validExerciserResult = await _repository.GetExerciserByIdAsync(id, cancellationToken);

        if (!validExerciserResult.IsSuccess)
            return _logger.LogErrorAndReturnFail<ExerciserDto>($"{validExerciserResult.ErrorMessage}");

        var exerciser = validExerciserResult.Value;

        var exerciserDto = _mapper.Map<ExerciserDto>(exerciser);

        return Result<ExerciserDto>.Ok(exerciserDto);
    }

    public async Task<Result<IReadOnlyList<ExerciserDto>>> GetExercisersAsync(CancellationToken cancellationToken)
    {
        var exercisersResult = await _repository.GetExercisersAsync(cancellationToken);

        if (!exercisersResult.IsSuccess)
            return _logger.LogErrorAndReturnFail<IReadOnlyList<ExerciserDto>>($"{exercisersResult.ErrorMessage}");

        var exercisers = exercisersResult.Value;

        var exerciserDtos = _mapper.Map<IReadOnlyList<ExerciserDto>>(exercisers);

        return Result<IReadOnlyList<ExerciserDto>>.Ok(exerciserDtos);
    }
}