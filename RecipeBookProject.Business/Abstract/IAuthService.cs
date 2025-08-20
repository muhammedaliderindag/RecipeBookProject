using RecipeBookProject.Business.Models; 
using System.Threading.Tasks;
using RecipeBookProject.Data.Entities;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RecipeBookProject.Business.Abstract
{
    // Sonuçları sarmalamak için bir DTO
    public record AuthResult(bool Succeeded, string ErrorMessage = null, string AccessToken = null, RefreshToken NewRefreshToken = null);

    public interface IAuthService
    {
        Task<AuthResult> LoginAsync(string email, string password);
        Task<GeneralResponse<NoData>> RegisterAsync(RegisterRequestDto request);
        Task<AuthResult> RefreshTokenAsync(string refreshTokenValue);
        Task InvalidateRefreshTokenAsync(string refreshTokenValue);
    }
}