using AutoMapper;
using EgitimPortalFinal.Models;
using EgitimPortalFinal.ViewModels;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Course, CourseModel>()
            .ForMember(dest => dest.PhotoFile, opt => opt.Ignore())
            .ReverseMap()
            .ForMember(dest => dest.Lessons, opt => opt.Ignore());

        CreateMap<CourseLesson, CourseLessonModel>()
            .ForMember(dest => dest.VideoFile, opt => opt.Ignore())
            .ForMember(dest => dest.ThumbnailFile, opt => opt.Ignore())
            .ForMember(dest => dest.Course, opt => opt.MapFrom(src => src.Course));

        CreateMap<CourseLessonModel, CourseLesson>()
            .ForMember(dest => dest.Course, opt => opt.Ignore())
            .ForMember(dest => dest.Video, opt => opt.MapFrom(src => src.Video))
            .ForMember(dest => dest.Thumbnail, opt => opt.MapFrom(src => src.Thumbnail));

        // Create için özel mapping
        CreateMap<CourseLessonsCreateModel, CourseLesson>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Course, opt => opt.Ignore())
            .ForMember(dest => dest.Video, opt => opt.Ignore())
            .ForMember(dest => dest.Thumbnail, opt => opt.Ignore());

      
    }
}
