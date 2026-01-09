using System.ComponentModel.DataAnnotations;
using gymBackendC__.WebApi.Auth;
using gymBackendC__.WebApi.Models.DTO;
using gymBackendC__.WebApi.Models.Entities;
using gymBackendC__.WebApi.Repositories;

using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

namespace gymBackendC__.WebApi.Services;

public class ExercisesService (
    IExercisesRepository exercisesRepository, 
    ITrainingsRepository trainingsRepository, 
    IMapper mapper,
    ICurrentUser currentUser,
    ILogger<ExercisesService> logger,
    IDistributedCache distributedCache
) : IExercisesService
{
    private static string CacheKey(int trainingId) => nameof(GetAsync) + ":" + trainingId;
    
    public async Task<List<ExercisesResponseModel>> GetAsync(int trainingId)
    {
        if (!currentUser.UserPermissions.Contains(Permissions.Read))
        {
            throw new SecurityTokenException("Authorization failed");
        }
        
        var exercisesString = await distributedCache.GetStringAsync(CacheKey(trainingId));
        if (exercisesString != null)
        {
            logger.LogInformation("Exercises for training with ID {trainingId} got from cache successfully", trainingId);
            return JsonSerializer.Deserialize<List<ExercisesResponseModel>>(exercisesString)!;
        }
        
        var exercises = await exercisesRepository.GetAllAsync(trainingId);
        if (exercises.Count == 0) {
            throw new KeyNotFoundException("No one exercises found");
        } 
        
        var exercisesResponseModels = mapper.Map<List<ExercisesResponseModel>>(exercises);
        exercisesString = JsonSerializer.Serialize(exercisesResponseModels);
        await distributedCache.SetStringAsync(
            CacheKey(trainingId), 
            exercisesString, 
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
            }
        );
        
        logger.LogInformation("Exercises for training with ID {trainingId} got from database successfully", trainingId);
        return exercisesResponseModels;
    }
    
    public async Task<ExercisesResponseModel?> GetByIdAsync(int id)
    {
        if (!currentUser.UserPermissions.Contains(Permissions.Read))
        {
            throw new SecurityTokenException("Authorization failed");
        }
        
        var exercise = await exercisesRepository.GetByIdAsync(id);
        if (exercise is null)
        {
            throw new KeyNotFoundException("Exercise not found");
        }
        logger.LogInformation("Exercise with ID {exerciseId} got from database successfully", id);
        return mapper.Map<ExercisesResponseModel>(exercise);
    }
    
    public async Task<ExercisesStatsModel> GetStatsAsync()
    {
        if (!currentUser.UserPermissions.Contains(Permissions.Read))
        {
            throw new SecurityTokenException("Authorization failed");
        }
        
        var exercisesStats = (await exercisesRepository.GetStatsAsync()).ToList();
        if (exercisesStats.Count == 0) {
            throw new KeyNotFoundException("Exercises stats not found");
        } 
        logger.LogInformation("Exercises stats got from database successfully");
        return exercisesStats[0];
    }
    
    public async Task<ExercisesResponseModel> CreateAsync(ExercisesRequestModel exercisesRequestModel)
    {
        if (!currentUser.UserPermissions.Contains(Permissions.Create))
        {
            throw new SecurityTokenException("Authorization failed");
        }
        
        if (await trainingsRepository.GetByIdAsync(exercisesRequestModel.TrainingId) is null)
        {
            throw new ValidationException("Training not found");
        }
        var exercise = mapper.Map<Exercises>(exercisesRequestModel);
        await exercisesRepository.CreateAsync(exercise);
        await distributedCache.RemoveAsync(CacheKey(exercise.TrainingId));
        logger.LogInformation("Exercise with ID {exerciseId} created successfully", exercise.Id);
        return mapper.Map<ExercisesResponseModel>(exercise);
    }
    
    public async Task UpdateAsync(int id, ExercisesRequestModel exercisesRequestModel)
    {
        if (!currentUser.UserPermissions.Contains(Permissions.Update))
        {
            throw new SecurityTokenException("Authorization failed");
        }
        
        var exercise = await exercisesRepository.GetByIdAsync(id);
        if (exercise is null)
        {
            throw new KeyNotFoundException("Exercise not found");
        }
        if (await trainingsRepository.GetByIdAsync(exercisesRequestModel.TrainingId) is null)
        {
            throw new KeyNotFoundException("Training not found");
        }
        mapper.Map(exercisesRequestModel, exercise);
        await exercisesRepository.UpdateAsync(exercise);
        await distributedCache.RemoveAsync(CacheKey(exercise.TrainingId));
        logger.LogInformation("Exercise with ID {exerciseId} updated successfully", exercise.Id);
    }
    
    public async Task DeleteAsync(int id)
    {
        if (!currentUser.UserPermissions.Contains(Permissions.Delete))
        {
            throw new SecurityTokenException("Authorization failed");
        }
        
        var exercise = await exercisesRepository.GetByIdAsync(id);
        if (exercise is null)
        {
            throw new KeyNotFoundException("Exercise not found");
        }
        await exercisesRepository.DeleteAsync(exercise.Id);
        await distributedCache.RemoveAsync(CacheKey(exercise.TrainingId));
        logger.LogInformation("Exercise with ID {exerciseId} deleted successfully", exercise.Id);
    }
}