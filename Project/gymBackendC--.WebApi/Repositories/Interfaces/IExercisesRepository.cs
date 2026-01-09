using gymBackendC__.WebApi.Models.Entities;
using gymBackendC__.WebApi.Models.DTO;

namespace gymBackendC__.WebApi.Repositories;

public interface IExercisesRepository : IRepository<Exercises>
{
    Task<List<Exercises>> GetAllAsync(int trainingId);
    Task<IEnumerable<ExercisesStatsModel>> GetStatsAsync();
}