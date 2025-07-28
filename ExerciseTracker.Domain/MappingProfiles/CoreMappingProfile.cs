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
            .ForMember(dest => dest.Exercises, opt => opt.MapFrom(src => src.Exercises))
            .ForMember(dest => dest.TotalExerciseDuration,
                opt => opt.MapFrom(src =>
                    src.Exercises.Any()
                        ? src.Exercises.Aggregate(TimeSpan.Zero, (current, ex) => current + ex.Duration)
                        : TimeSpan.Zero))
            .ForMember(dest => dest.NumberOfSessions, opt => opt.MapFrom(src => src.Exercises.Count));

        CreateMap<Exercise, ExerciseDto>()
            .ForMember(dest => dest.ExerciserId, opt => opt.MapFrom(src => src.Exerciser.Id))
            .ForMember(dest => dest.ExerciserName, opt => opt.MapFrom(src => src.Exerciser.Name))
            .ForMember(dest => dest.ExerciserAge,
                opt => opt.MapFrom(src => ExerciseTrackerHelper.CalculateValidAge(src.Exerciser.BirthDate)))
            .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.EndTime - src.StartTime));
    }
}