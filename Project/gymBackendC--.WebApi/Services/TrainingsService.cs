using gymBackendC__.WebApi.Auth;
using gymBackendC__.WebApi.Models.DTO;
using gymBackendC__.WebApi.Models.Entities;
using gymBackendC__.WebApi.Repositories;

using AutoMapper;
using Microsoft.IdentityModel.Tokens;

namespace gymBackendC__.WebApi.Services;

public class TrainingsService (
    ITrainingsRepository trainingsRepository, 
    IAuthRepository authRepository,
    IMapper mapper,
    ILogger<TrainingsService> logger,
    ICurrentUser currentUser
) : ITrainingsService
{
    public async Task<TrainingsSearchModel> GetAsync(int page, int pageSize, DateTime? searchDate)
    {
        if (!currentUser.UserPermissions.Contains(Permissions.Read))
        {
            throw new SecurityTokenException("Authorization failed");
        }
        
        var (total, trainings) = await trainingsRepository.GetAllAsync(
            currentUser.UserId, page, pageSize, searchDate
        );
        if (trainings.Count == 0) {
            throw new KeyNotFoundException("No one trainings found");
        }
        var trainingsResponseModels = mapper.Map<List<TrainingsResponseModel>>(trainings);
        logger.LogInformation("Trainings for user with ID {userId} and date {searchDate} got from database successfully", currentUser.UserId, searchDate);
        return new TrainingsSearchModel()
        {
            Items = trainingsResponseModels,
            Page = page,
            PageSize = pageSize,
            Total = total
        };
    }
    
    public async Task<TrainingsResponseModel?> GetByIdAsync(int id)
    {
        if (!currentUser.UserPermissions.Contains(Permissions.Read))
        {
            throw new SecurityTokenException("Authorization failed");
        }
        
        var training = await trainingsRepository.GetByIdAsync(id);
        if (training is null)
        {
            throw new KeyNotFoundException("Training not found");
        }
        logger.LogInformation("Training with ID {trainingId} got from database successfully", id);
        return mapper.Map<TrainingsResponseModel>(training);
    }
    
    public async Task<TrainingsResponseModel> CreateAsync(TrainingsRequestModel trainingRequestModel)
    {
        if (!currentUser.UserPermissions.Contains(Permissions.Create))
        {
            throw new SecurityTokenException("Authorization failed");
        }
        
        var users = await authRepository.GetByIdsAsync(trainingRequestModel.Users);
        var training = mapper.Map<Trainings>(trainingRequestModel);
        training.Users = users;
        await trainingsRepository.CreateAsync(training);
        logger.LogInformation("Training with ID {trainingId} created successfully", training.Id);
        return mapper.Map<TrainingsResponseModel>(training);
    }
    
    public async Task UpdateAsync(int id, TrainingsRequestModel trainingRequestModel)
    {
        if (!currentUser.UserPermissions.Contains(Permissions.Update))
        {
            throw new SecurityTokenException("Authorization failed");
        }
        
        var training = await trainingsRepository.GetByIdAsync(id);
        if (training is null)
        {
            throw new KeyNotFoundException("Training not found");
        }
        var currentUserIds = training.Users.Select(u => u.Id).ToList();
        mapper.Map(trainingRequestModel, training);
        
        // обновление many-to-many
        var usersToAdd = await authRepository.GetByIdsAsync(trainingRequestModel.Users.Except(currentUserIds).ToList());
        foreach (var user in usersToAdd)
        {
            training.Users.Add(user);
        }
        var usersToRemove = training.Users.Where(u => !trainingRequestModel.Users.Contains(u.Id)).ToList();
        foreach (var user in usersToRemove)
        {
            training.Users.Remove(user);
        }
        
        await trainingsRepository.UpdateAsync(training);
        logger.LogInformation("Training with ID {trainingId} updated successfully", id);
    }
    
    public async Task DeleteAsync(int id)
    {
        if (!currentUser.UserPermissions.Contains(Permissions.Delete))
        {
            throw new SecurityTokenException("Authorization failed");
        }
        
        var training = await trainingsRepository.GetByIdAsync(id);
        if (training is null)
        {
            throw new KeyNotFoundException("Training not found");
        }
        await trainingsRepository.DeleteAsync(training.Id);
        logger.LogInformation("Training with ID {trainingId} deleted successfully", id);
    }
}