using gymBackendC__.WebApi.Models.Entities;

namespace gymBackendC__.WebApi.Repositories;

public interface IAuthRepository
{
    ValueTask<Users?> GetByUsernameAsync(string username);
    Task<List<Users>> GetByIdsAsync(List<int> userIds);
    Task CreateAsync(Users user);
}