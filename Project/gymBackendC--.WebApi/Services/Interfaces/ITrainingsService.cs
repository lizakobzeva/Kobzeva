using gymBackendC__.WebApi.Models.DTO;

namespace gymBackendC__.WebApi.Services;

public interface ITrainingsService
{
    public Task<TrainingsSearchModel> GetAsync(int page, int pageSize, DateTime? searchDate);
    public Task<TrainingsResponseModel?> GetByIdAsync(int id);
    public Task<TrainingsResponseModel> CreateAsync(TrainingsRequestModel trainingsRequestModel);
    public Task UpdateAsync(int id, TrainingsRequestModel trainingsRequestModel);
    public Task DeleteAsync(int id);
}