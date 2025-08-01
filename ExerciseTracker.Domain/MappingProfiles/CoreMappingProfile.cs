using AutoMapper;
using ExerciseTracker.Core.Models;
using ExerciseTracker.Core.DTOs;
using ExerciseTracker.Domain.Helpers;

namespace ExerciseTracker.Domain.MappingProfiles;

public class CoreMappingProfile : Profile
{
    public CoreMappingProfile()
    {
        CreateMap<Exerciser, ExerciserDto>()
            .ForMember(dest => dest.Age,
                opt => opt.MapFrom(src => ExerciseTrackerHelper.CalculateValidAge(src.BirthDate)))
            .ForMember(dest => dest.TotalExerciseDuration, opt => opt.MapFrom(src =>
                src.Exercises != null && src.Exercises.Any()
                    ? src.Exercises.Aggregate(TimeSpan.Zero, (current, ex) => current + ex.Duration)
                    : TimeSpan.Zero))
            .ForMember(dest => dest.NumberOfSessions,
                opt => opt.MapFrom(src => src.Exercises != null ? src.Exercises.Count : 0));

        CreateMap<Exercise, ExerciseDto>()
            .ForMember(dest => dest.ExerciserId,
                opt => opt.MapFrom(src => src.Exerciser != null ? src.Exerciser.Id : 0))
            .ForMember(dest => dest.ExerciserName,
                opt => opt.MapFrom(src => src.Exerciser != null ? src.Exerciser.Name : "Unknown"))
            .ForMember(dest => dest.ExerciserAge,
                opt => opt.MapFrom(src =>
                    src.Exerciser != null ? ExerciseTrackerHelper.CalculateValidAge(src.Exerciser.BirthDate) : 0));

        CreateMap<CreateExerciserRequest, Exerciser>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Exercises, opt => opt.Ignore());

        CreateMap<CreateExerciseRequest, Exercise>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Exerciser, opt => opt.Ignore());

        CreateMap<UpdateExerciserRequest, Exerciser>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Exercises, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.Condition(src => src.Name != null))
            .ForMember(dest => dest.BirthDate, opt => opt.Condition(src =>  src.BirthDate.HasValue))
            .ForMember(dest => dest.BodyWeight, opt => opt.Condition(src => src.BodyWeight != null))
            .ForMember(dest => dest.FitnessGoal, opt => opt.Condition(src => src.FitnessGoal != null));

        CreateMap<UpdateExerciseRequest, Exercise>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ExerciserId, opt => opt.Ignore())
            .ForMember(dest => dest.Exerciser, opt => opt.Ignore())
            .ForMember(dest => dest.StartTime, opt => opt.Condition(src => src.StartTime.HasValue))
            .ForMember(dest => dest.EndTime, opt => opt.Condition(src => src.EndTime.HasValue))
            .ForMember(dest => dest.ExerciseType, opt => opt.Condition(src => src.ExerciseType != null))
            .ForMember(dest => dest.Comments, opt => opt.Condition(src => src.Comments != null));
    }
}