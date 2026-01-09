using gymBackendC__.WebApi.Models.Entities;

namespace gymBackendC__.WebApi.Repositories;

public interface ITrainingsRepository : IRepository<Trainings>
{
    Task<(int, List<Trainings>)> GetAllAsync(int userId, int page, int pageSize, DateTime? searchDate);
}