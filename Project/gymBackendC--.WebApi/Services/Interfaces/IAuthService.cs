using gymBackendC__.WebApi.Models.DTO;

namespace gymBackendC__.WebApi.Services;

public interface IAuthService
{
    public Task RegisterAsync(RegisterRequestModel registerRequestModel);
    public Task<LoginResponseModel> LoginAsync(LoginRequestModel loginRequestModel);
}