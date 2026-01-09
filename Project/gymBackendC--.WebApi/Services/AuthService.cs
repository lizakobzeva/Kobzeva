using gymBackendC__.WebApi.Auth;
using gymBackendC__.WebApi.Models.DTO;
using gymBackendC__.WebApi.Models.Entities;
using gymBackendC__.WebApi.Repositories;

using System.Security.Cryptography;
using System.Text;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace gymBackendC__.WebApi.Services;

public class AuthService(
    IAuthRepository authRepository,
    IJWTHelper jwtHelper,
    ILogger<AuthService> logger
) : IAuthService
{

    public async Task RegisterAsync(RegisterRequestModel registerRequestModel)
    {
        var existingUser = await authRepository.GetByUsernameAsync(registerRequestModel.Username);
        if (existingUser != null)
            throw new DbUpdateConcurrencyException("User with this username already exists");

        var user = new Users
        {
            Username = registerRequestModel.Username,
            Email = registerRequestModel.Email,
            Password = HashPassword(registerRequestModel.Password),
            Role = Roles.User
        };

        logger.LogInformation("User with username {username} registered successfully", registerRequestModel.Username);
        await authRepository.CreateAsync(user);
    }

    public async Task<LoginResponseModel> LoginAsync(LoginRequestModel loginRequestModel)
    {
        var user = await authRepository.GetByUsernameAsync(loginRequestModel.Username);
        if (user == null || user.Password != HashPassword(loginRequestModel.Password))
            throw new ValidationException("Incorrect username or password");

        var token = jwtHelper.GenerateToken(user.Id, user.Role);
        
        logger.LogInformation("User with username {username} logged in successfully", loginRequestModel.Username);
        return new LoginResponseModel() { Token = token };
    }

    private static string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
