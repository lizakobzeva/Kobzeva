using gymBackendC__.WebApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace gymBackendC__.Tests;

public class ExercisesRepositoryTests
{
    private static List<Trainings> GetTrainingsFakes() =>
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
            Notes = "idk2"
        },
        new()
        {
            Id = 3,
            Date = new DateTime(2020, 1, 1),
            Notes = "idk3"
        }
    ];
    
    private static List<Exercises> GetExercisesFakes(List<Trainings> trainingsFakes) =>
    [
        new()
        {
            Id = 1,
            TrainingId = 1,
            Training = trainingsFakes.Where(t => t.Id == 1).ToList().First(),
            Name = "idk",
            Repeats = 13,
            Weight = 77,
            TimeSec = 120
        },
        new()
        {
            Id = 2,
            TrainingId = 1,
            Training = trainingsFakes.Where(t => t.Id == 1).ToList().First(),
            Name = "idk_new",
            Repeats = 5,
            Weight = 25,
        },
        new()
        {
            Id = 3,
            TrainingId = 2,
            Training = trainingsFakes.Where(t => t.Id == 2).ToList().First(),
            Name = "idk_new_new",
            Repeats = 44,
            Weight = 144,
            TimeSec = 10
        },
    ];
    
    [Fact]
    public async Task TestGetAllAsync()
    {
        // -- Arrange
        var (appDbContext, exercisesRepository) = DatabaseMock.GetExercisesRepository();
        List<Trainings> trainingsFakes = GetTrainingsFakes();
        List<Exercises> exercisesFakes = GetExercisesFakes(trainingsFakes);
        await appDbContext.Exercises.AddRangeAsync(exercisesFakes);
        await appDbContext.SaveChangesAsync();
        
        // -- Act
        int trainingId = 1;
        List<Exercises> selectedExercisesFakes = exercisesFakes.Where(e => e.TrainingId == trainingId).ToList();
        List<Exercises> selectedExercises = await exercisesRepository.GetAllAsync(1);
        
        // -- Assert
        // check if method worked
        Assert.NotNull(selectedExercises);
        // check if correct records count selected
        Assert.Equal(2, selectedExercises.Count());
        // check if correct data selected
        Assert.True(selectedExercises.ToHashSet().SetEquals(selectedExercisesFakes));
    }
    
    [Fact]
    public async Task TestCorrectGetByIdAsync()
    {
        // -- Arrange
        var (appDbContext, exercisesRepository) = DatabaseMock.GetExercisesRepository();
        List<Trainings> trainingsFakes = GetTrainingsFakes();
        List<Exercises> exercisesFakes = GetExercisesFakes(trainingsFakes);
        await appDbContext.Exercises.AddRangeAsync(exercisesFakes);
        await appDbContext.SaveChangesAsync();
        
        // -- Act
        int selectedId = 1;
        Exercises selectedExercise = await exercisesRepository.GetByIdAsync(selectedId);
        
        // -- Assert
        // check if record found
        Assert.NotNull(selectedExercise);
        // check if correct data selected
        Assert.Equal(selectedExercise, exercisesFakes.Where(x => x.Id == selectedId).ToList()[0]);
    }
    
    [Fact]
    public async Task TestIncorrectGetByIdAsync()
    {
        // -- Arrange
        var (appDbContext, exercisesRepository) = DatabaseMock.GetExercisesRepository();
        List<Trainings> trainingsFakes = GetTrainingsFakes();
        List<Exercises> exercisesFakes = GetExercisesFakes(trainingsFakes);
        await appDbContext.Exercises.AddRangeAsync(exercisesFakes);
        await appDbContext.SaveChangesAsync();
        
        // -- Act
        int selectedId = 7;
        Exercises selectedExercise = await exercisesRepository.GetByIdAsync(selectedId);
        
        // -- Assert
        // check if record not found
        Assert.Null(selectedExercise);
    }
    
    [Fact]
    public async Task TestCreateAsync()
    {
        // -- Arrange
        var (appDbContext, exercisesRepository) = DatabaseMock.GetExercisesRepository();
        List<Trainings> trainingsFakes = GetTrainingsFakes();
        
        // -- Act
        Exercises exercise = new Exercises()
        {
            TrainingId = 1,
            Training = trainingsFakes.Where(t => t.Id == 1).ToList().First(),
            Name = "idk",
            Repeats = 13,
            Weight = 77,
            TimeSec = 120
        };
        await exercisesRepository.CreateAsync(exercise);
        Exercises createdExercise = await appDbContext.Exercises.SingleOrDefaultAsync(u => u.Name == "idk");
        
        // -- Assert
        // check if record exists
        Assert.NotNull(createdExercise);
        // check if one record wrote
        Assert.Equal(1, appDbContext.Exercises.Count());
        // check if record has correct data
        Assert.Equal(1, createdExercise!.TrainingId);
        Assert.Equal(trainingsFakes.Where(t => t.Id == 1).ToList().First(), createdExercise!.Training);
        Assert.Equal("idk", createdExercise!.Name);
        Assert.Equal(13, createdExercise!.Repeats);
        Assert.Equal(77, createdExercise!.Weight);
        Assert.Equal(120, createdExercise!.TimeSec);
    }

    [Fact]
    public async Task TestCorrectUpdateAsync()
    {
        // -- Arrange
        var (appDbContext, exercisesRepository) = DatabaseMock.GetExercisesRepository();
        List<Trainings> trainingsFakes = GetTrainingsFakes();
        List<Exercises> exercisesFakes = GetExercisesFakes(trainingsFakes);
        await appDbContext.Exercises.AddRangeAsync(exercisesFakes);
        await appDbContext.SaveChangesAsync();
        
        // -- Act
        int updatedId = 1;
        Exercises exercise = exercisesFakes.Where(t => t.Id == updatedId).ToList().First();
        exercise.TrainingId = 2;
        exercise.Training = trainingsFakes.Where(t => t.Id == 2).ToList().First();
        exercise.Name = "idk_upd";
        exercise.Repeats = 18;
        exercise.Weight = 60;
        exercise.TimeSec = null;
        await exercisesRepository.UpdateAsync(exercise);
        Exercises updatedExercise = await appDbContext.Exercises.SingleOrDefaultAsync(u => u.Id == updatedId);
        
        // -- Assert
        // check if record exists
        Assert.NotNull(updatedExercise);
        // check if record has correct data
        Assert.Equal(2, updatedExercise!.TrainingId);
        Assert.Equal(trainingsFakes.Where(t => t.Id == 2).ToList().First(), updatedExercise!.Training);
        Assert.Equal("idk_upd", updatedExercise!.Name);
        Assert.Equal(18, updatedExercise!.Repeats);
        Assert.Equal(60, updatedExercise!.Weight);
        Assert.Null(updatedExercise!.TimeSec);
    }

    [Fact]
    public async Task TestCorrectDeleteAsync()
    {
        // -- Arrange
        var (appDbContext, exercisesRepository) = DatabaseMock.GetExercisesRepository();
        List<Trainings> trainingsFakes = GetTrainingsFakes();
        List<Exercises> exercisesFakes = GetExercisesFakes(trainingsFakes);
        await appDbContext.Exercises.AddRangeAsync(exercisesFakes);
        await appDbContext.SaveChangesAsync();
        
        // -- Act
        int deletedId = 1;
        await exercisesRepository.DeleteAsync(deletedId);
        Exercises deletedExercise = await appDbContext.Exercises.FirstOrDefaultAsync(t => t.Id == deletedId);
        
        // -- Assert
        // check if correct record deleted
        Assert.Null(deletedExercise);
    }
    
    [Fact]
    public async Task TestIncorrectDeleteAsync()
    {
        // -- Arrange
        var (appDbContext, exercisesRepository) = DatabaseMock.GetExercisesRepository();
        List<Trainings> trainingsFakes = GetTrainingsFakes();
        List<Exercises> exercisesFakes = GetExercisesFakes(trainingsFakes);
        await appDbContext.Exercises.AddRangeAsync(exercisesFakes);
        await appDbContext.SaveChangesAsync();
        
        // -- Act
        int deletedId = 5;
        await exercisesRepository.DeleteAsync(deletedId);
        Exercises deletedExercise = await appDbContext.Exercises.FirstOrDefaultAsync(t => t.Id == deletedId);
        
        // -- Assert
        // check if correct record deleted
        Assert.Null(deletedExercise);
    }
}