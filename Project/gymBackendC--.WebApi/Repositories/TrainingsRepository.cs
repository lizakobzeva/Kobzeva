using gymBackendC__.WebApi.Data;
using gymBackendC__.WebApi.Models.Entities;

using Microsoft.EntityFrameworkCore;

namespace gymBackendC__.WebApi.Repositories;

public class TrainingsRepository(AppDbContext dbContext) : Repository<Trainings>(dbContext), ITrainingsRepository
{
    public async Task<(int, List<Trainings>)> GetAllAsync(int userId, int page, int pageSize, DateTime? searchDate)
    {
        IQueryable<Trainings> query;
        
        if (userId == 0)
        {
            query = dbContext.Trainings.Where(t => true);
        }
        else
        {
            query = dbContext.Trainings
                .Where(t => t.Users.Any(u => u.Id == userId));
        }
        
        if (searchDate.HasValue)
        {
            query = query.Where(t => t.Date.Date == DateTime.SpecifyKind(searchDate.Value.Date, DateTimeKind.Utc));
        }

        int total = await query.CountAsync();
        
        var trainings = await query
            .OrderByDescending(t => t.Date)
            .ThenBy(t => t.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(t => t.Users)
            .ToListAsync();
        
        return new (total, trainings);
    }

    public new async ValueTask<Trainings?> GetByIdAsync(int id) => await dbContext.Trainings.Include(t => t.Users).FirstOrDefaultAsync(t => t.Id == id);
    
    public new async Task UpdateAsync(Trainings training) => await dbContext.SaveChangesAsync();
}