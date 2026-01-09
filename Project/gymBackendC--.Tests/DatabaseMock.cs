using gymBackendC__.WebApi.Data;
using gymBackendC__.WebApi.Repositories;

using System.Data.Common;
using System.Data.SQLite;
using Microsoft.EntityFrameworkCore;

namespace gymBackendC__.Tests;

public class DatabaseMock
{
    private static AppDbContext _getAppDbContext()
    {
        DbContextOptions<AppDbContext> options;
        var builder = new DbContextOptionsBuilder<AppDbContext>();
        builder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        options = builder.Options;
        
        AppDbContext appDbContext = new AppDbContext(options);
        appDbContext.Database.EnsureDeleted();
        appDbContext.Database.EnsureCreated();
        return appDbContext;
    }

    public static (AppDbContext, IAuthRepository) GetAuthRepository()
    {
        AppDbContext appDbContext = _getAppDbContext();
        return new (appDbContext, new AuthRepository(appDbContext));
    }

    public static (AppDbContext, IExercisesRepository) GetExercisesRepository()
    {
        AppDbContext appDbContext = _getAppDbContext();
        DbConnection dbConnection = new SQLiteConnection("Data Source=:memory:;Version=3;New=True;");
        return new (appDbContext, new ExercisesRepository(appDbContext, dbConnection));
    }

    public static (AppDbContext, ITrainingsRepository) GetTrainingsRepository()
    {
        AppDbContext appDbContext = _getAppDbContext();
        return new (appDbContext, new TrainingsRepository(appDbContext));
    }
}