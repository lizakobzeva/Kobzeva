using gymBackendC__.WebApi.Models.DTO;
using gymBackendC__.WebApi.Services;

using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace gymBackendC__.WebApi.Controllers;

[ApiController]
[Route("auth")]
[Produces("application/json")]
public class AuthController(
    IAuthService authService, 
    IValidator<RegisterRequestModel> registerValidator,
    IValidator<LoginRequestModel> loginValidator
) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ExceptionModel>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ExceptionModel>(StatusCodes.Status409Conflict)]
    public async Task<IResult> Register([FromBody] RegisterRequestModel request)
    {
        var validation = await registerValidator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors);
        }
        
        await authService.RegisterAsync(request);
        return TypedResults.Ok();
    }

    [HttpPost("login")]
    [ProducesResponseType<LoginResponseModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<ExceptionModel>(StatusCodes.Status400BadRequest)]
    public async Task<IResult> Login([FromBody] LoginRequestModel request)
    {
        var validation = await loginValidator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors);
        }
        
        var result = await authService.LoginAsync(request);
        return TypedResults.Ok(result);
    }
}
