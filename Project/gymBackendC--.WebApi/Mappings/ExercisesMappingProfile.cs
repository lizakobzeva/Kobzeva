using gymBackendC__.WebApi.Models.DTO;
using gymBackendC__.WebApi.Models.Entities;

using AutoMapper;

namespace gymBackendC__.WebApi.Mappings;

public class ExercisesMappingProfile : Profile
{
    public ExercisesMappingProfile()
    {
        // ExercisesRequestModel -> Exercises
        CreateMap<ExercisesRequestModel, Exercises>()
            .ForMember(destination => destination.Id, options => options.Ignore());

        // Exercises -> ExercisesResponseModel
        CreateMap<Exercises, ExercisesResponseModel>();
    }
}
