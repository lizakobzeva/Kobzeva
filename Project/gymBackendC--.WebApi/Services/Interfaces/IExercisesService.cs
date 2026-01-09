using gymBackendC__.WebApi.Models.DTO;

namespace gymBackendC__.WebApi.Services;

public interface IExercisesService
{
    public Task<List<ExercisesResponseModel>> GetAsync(int trainingId);
    public Task<ExercisesResponseModel?> GetByIdAsync(int id);
    public Task<ExercisesStatsModel> GetStatsAsync();
    public Task<ExercisesResponseModel> CreateAsync(ExercisesRequestModel exercisesRequestModel);
    public Task UpdateAsync(int id, ExercisesRequestModel exercisesRequestModel);
    public Task DeleteAsync(int id);
}