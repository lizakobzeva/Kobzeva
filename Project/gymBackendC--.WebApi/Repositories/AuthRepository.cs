using gymBackendC__.WebApi.Data;
using gymBackendC__.WebApi.Models.Entities;

using Microsoft.EntityFrameworkCore;

namespace gymBackendC__.WebApi.Repositories;

public class AuthRepository(AppDbContext dbContext) : IAuthRepository
{
    public async ValueTask<Users?> GetByUsernameAsync(string username) => await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
    
    public async Task<List<Users>> GetByIdsAsync(List<int> userIds) => await dbContext.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
    
    public async Task CreateAsync(Users user)
    {
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
    }
}