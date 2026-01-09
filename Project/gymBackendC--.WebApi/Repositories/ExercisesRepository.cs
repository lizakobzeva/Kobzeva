using System.Data.Common;
using Dapper;
using gymBackendC__.WebApi.Data;
using gymBackendC__.WebApi.Models.Entities;
using gymBackendC__.WebApi.Models.DTO;

using Npgsql;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace gymBackendC__.WebApi.Repositories;

public class ExercisesRepository(AppDbContext dbContext, DbConnection connection) : Repository<Exercises>(dbContext), IExercisesRepository
{
    private readonly DbConnection _connection = connection;
    
    public async Task<List<Exercises>> GetAllAsync(int trainingId) => await dbContext.Exercises.Where(e => e.TrainingId == trainingId).ToListAsync();
    
    public async Task<IEnumerable<ExercisesStatsModel>> GetStatsAsync()
    {
        await _connection.OpenAsync();
        await using var transaction = await _connection.BeginTransactionAsync();
        
        try
        {
            var result = await _connection.QueryAsync<ExercisesStatsModel>(
                "SELECT count(*) as Count, avg(repeats) as AvgRepeats, avg(weight) as AvgWeight FROM exercises"
            );
            await transaction.CommitAsync();
            return result;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Ошибка: {ex.Message}");
            return new List<ExercisesStatsModel>();
        }
    }
}