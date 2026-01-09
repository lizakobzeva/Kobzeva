using gymBackendC__.WebApi.Auth;
using gymBackendC__.WebApi.Models.Entities;

using Microsoft.EntityFrameworkCore;

namespace gymBackendC__.Tests;

public class TrainingsRepositoryTests
{
    private static List<Users> GetUsersFakes() => [
        new()
        {
            Id = 1,
            Username = "test",
            Email = "test@test.com",
            Password = "test_test",
            Role = Roles.User
        },
        new()
        {
            Id = 2,
            Username = "test2",
            Email = "test2@test.com",
            Password = "test2_test2",
            Role = Roles.Manager
        },
        new()
        {
            Id = 3,
            Username = "test3",
            Email = "test3@test.com",
            Password = "test3_test3",
            Role = Roles.Admin
        },
    ];
    
    private static List<Trainings> GetTrainingsFakes(List<Users> usersFakes) =>
    [
        new()
        {
            Id = 1,
            Date = new DateTime(2020, 1, 5),
            Notes = null
        },
        new()
        {
            Id = 2,
            Date = new DateTime(2020, 1, 2),
            Notes = "idk2",
            Users = usersFakes.Where(u => new[] { 1, 3 }.Contains(u.Id)).ToList()
        },
        new()
        {
            Id = 3,
            Date = new DateTime(2020, 1, 2),
            Notes = "idk3",
            Users = usersFakes.Where(u => new[] { 1, 2 }.Contains(u.Id)).ToList()
        },
        new()
        {
            Id = 4,
            Date = new DateTime(2020, 1, 2),
            Notes = "idk4",
            Users = usersFakes.Where(u => new[] { 2, 3 }.Contains(u.Id)).ToList()
        },
        new()
        {
            Id = 5,
            Date = new DateTime(2020, 1, 1),
            Notes = "idk5",
            Users = usersFakes.Where(u => new[] { 2 }.Contains(u.Id)).ToList()
        }
    ];
    
    [Fact]
    public async Task TestGetAllAsyncWithDate()
    {
        // -- Arrange
        var (appDbContext, trainingsRepository) = DatabaseMock.GetTrainingsRepository();
        List<Users> usersFakes = GetUsersFakes();
        List<Trainings> trainingsFakes = GetTrainingsFakes(usersFakes);
        await appDbContext.Trainings.AddRangeAsync(trainingsFakes);
        await appDbContext.SaveChangesAsync();
        
        // -- Act
        var (totalTrainings, trainings) = await trainingsRepository.GetAllAsync(2, 1, 10, new DateTime(2020, 1, 2));
        
        // -- Assert
        // check if method worked
        Assert.NotNull(trainings);
        // check if correct records count selected
        Assert.Equal(2, trainings.Count());
        // check if totalTrainings correct
        Assert.Equal(2, totalTrainings);
        // check if correct data selected
        Assert.True(trainings.ToHashSet().SetEquals(trainingsFakes.Where(x => new[] { 3, 4 }.Contains(x.Id)).ToList()));
    }
    
    [Fact]
    public async Task TestGetAllAsyncWithPage()
    {
        // -- Arrange
        var (appDbContext, trainingsRepository) = DatabaseMock.GetTrainingsRepository();
        List<Users> usersFakes = GetUsersFakes();
        List<Trainings> trainingsFakes = GetTrainingsFakes(usersFakes);
        await appDbContext.Trainings.AddRangeAsync(trainingsFakes);
        await appDbContext.SaveChangesAsync();
        
        // -- Act
        var (totalTrainings, trainings) = await trainingsRepository.GetAllAsync(2, 2, 1, null);
        
        // -- Assert
        // check if method worked
        Assert.NotNull(trainings);
        // check if correct records count selected
        Assert.Equal(1, trainings.Count());
        // check if totalTrainings correct
        Assert.Equal(3, totalTrainings);
        // check if correct data selected
        Assert.Equal(trainingsFakes.Where(x => x.Id == 4).ToList(), trainings);
    }
    
    [Fact]
    public async Task TestCorrectGetByIdAsync()
    {
        // -- Arrange
        var (appDbContext, trainingsRepository) = DatabaseMock.GetTrainingsRepository();
        List<Users> usersFakes = GetUsersFakes();
        List<Trainings> trainingsFakes = GetTrainingsFakes(usersFakes);
        await appDbContext.Trainings.AddRangeAsync(trainingsFakes);
        await appDbContext.SaveChangesAsync();
        
        // -- Act
        int selectedId = 1;
        Trainings selectedTraining = await trainingsRepository.GetByIdAsync(selectedId);
        
        // -- Assert
        // check if record found
        Assert.NotNull(selectedTraining);
        // check if correct data selected
        Assert.Equal(selectedTraining, trainingsFakes.Where(x => x.Id == selectedId).ToList()[0]);
    }
    
    [Fact]
    public async Task TestIncorrectGetByIdAsync()
    {
        // -- Arrange
        var (appDbContext, trainingsRepository) = DatabaseMock.GetTrainingsRepository();
        List<Users> usersFakes = GetUsersFakes();
        List<Trainings> trainingsFakes = GetTrainingsFakes(usersFakes);
        await appDbContext.Trainings.AddRangeAsync(trainingsFakes);
        await appDbContext.SaveChangesAsync();
        
        // -- Act
        int selectedId = 7;
        Trainings selectedTraining = await trainingsRepository.GetByIdAsync(selectedId);
        
        // -- Assert
        // check if record not found
        Assert.Null(selectedTraining);
    }
    
    [Fact]
    public async Task TestCreateAsyncWithoutUsers()
    {
        // -- Arrange
        var (appDbContext, trainingsRepository) = DatabaseMock.GetTrainingsRepository();
        
        // -- Act
        Trainings training = new Trainings()
        {
            Date = new DateTime(2001, 9, 11),
            Notes = "wtf"
        };
        await trainingsRepository.CreateAsync(training);
        Trainings createdTraining = await appDbContext.Trainings.SingleOrDefaultAsync(t => t.Notes == "wtf");
        
        // -- Assert
        // check if record exists
        Assert.NotNull(createdTraining);
        // check if one record wrote
        Assert.Equal(1, appDbContext.Trainings.Count());
        // check if record has correct data
        Assert.Equal(new DateTime(2001, 9, 11), createdTraining!.Date);
        Assert.Equal("wtf", createdTraining!.Notes);
        Assert.Empty(createdTraining!.Users);
        Assert.Empty(createdTraining!.Exercises);
    }
    
    [Fact]
    public async Task TestCreateAsyncWithUsers()
    {
        // -- Arrange
        var (appDbContext, trainingsRepository) = DatabaseMock.GetTrainingsRepository();
        List<Users> usersFakes = GetUsersFakes();
        
        // -- Act
        Trainings training = new Trainings()
        {
            Date = new DateTime(2001, 9, 11),
            Users = usersFakes
        };
        await trainingsRepository.CreateAsync(training);
        Trainings createdTraining = await appDbContext.Trainings.SingleOrDefaultAsync(t => t.Notes == null);
        
        // -- Assert
        // check if record exists
        Assert.NotNull(createdTraining);
        // check if one record wrote
        Assert.Equal(1, appDbContext.Trainings.Count());
        // check if record has correct data
        Assert.Equal(new DateTime(2001, 9, 11), createdTraining!.Date);
        Assert.Null(createdTraining!.Notes);
        Assert.Equal(3, createdTraining.Users.Count());
        Assert.Empty(createdTraining!.Exercises);
    }
    
    [Fact]
    public async Task TestCorrectDeleteAsync()
    {
        // -- Arrange
        var (appDbContext, trainingsRepository) = DatabaseMock.GetTrainingsRepository();
        List<Users> usersFakes = GetUsersFakes();
        List<Trainings> trainingsFakes = GetTrainingsFakes(usersFakes);
        await appDbContext.Trainings.AddRangeAsync(trainingsFakes);
        await appDbContext.SaveChangesAsync();
        
        // -- Act
        int deletedId = 1;
        await trainingsRepository.DeleteAsync(deletedId);
        Trainings deletedTraining = await appDbContext.Trainings.FirstOrDefaultAsync(t => t.Id == deletedId);
        
        // -- Assert
        // check if correct record deleted
        Assert.Null(deletedTraining);
    }
    
    [Fact]
    public async Task TestIncorrectDeleteAsync()
    {
        // -- Arrange
        var (appDbContext, trainingsRepository) = DatabaseMock.GetTrainingsRepository();
        List<Users> usersFakes = GetUsersFakes();
        List<Trainings> trainingsFakes = GetTrainingsFakes(usersFakes);
        await appDbContext.Trainings.AddRangeAsync(trainingsFakes);
        await appDbContext.SaveChangesAsync();
        
        // -- Act
        int deletedId = 5;
        await trainingsRepository.DeleteAsync(deletedId);
        Trainings deletedTraining = await appDbContext.Trainings.FirstOrDefaultAsync(t => t.Id == deletedId);
        
        // -- Assert
        // check if correct record deleted
        Assert.Null(deletedTraining);
    }
}