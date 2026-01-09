using gymBackendC__.WebApi.Data;
using gymBackendC__.WebApi.Models.DTO;
using gymBackendC__.WebApi.Models.Entities;

using AutoMapper;

namespace gymBackendC__.WebApi.Mappings;

public class TrainingsMappingProfile : Profile
{
    public TrainingsMappingProfile()
    {
        // TrainingsRequestModel -> Trainings
        CreateMap<TrainingsRequestModel, Trainings>()
            .ForMember(destination => destination.Id, options => options.Ignore())
            .ForMember(destination => destination.Users, options => options.Ignore());
        
        // Trainings -> TrainingsResponseModel
        CreateMap<Trainings, TrainingsResponseModel>()
            .ForMember(
                destination => destination.Date,
                options => options.MapFrom(source => $"{source.Date:MMMM d, yyyy}")
            )
            .ForMember(
                destination => destination.Users, 
                options => options.MapFrom(source => source.Users.Select(u => u.Id).ToList())
            );
    }
}