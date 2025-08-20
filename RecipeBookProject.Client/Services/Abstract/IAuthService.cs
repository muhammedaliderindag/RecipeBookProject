using RecipeBookProject.Client.Models;
namespace RecipeBookProject.Client.Services
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(LoginRequestDto loginRequest);
        Task<bool> RegisterAsync(RegisterRequestDto registerRequest);
        Task LogoutAsync();
    }
}
