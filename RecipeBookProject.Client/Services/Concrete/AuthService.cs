using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using RecipeBookProject.Client.Models;
namespace RecipeBookProject.Client.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public AuthService(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<bool> LoginAsync(LoginRequestDto loginRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginRequest);
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            await ((CustomAuthenticationStateProvider)_authenticationStateProvider)
                .MarkUserAsAuthenticated(authResponse.AccessToken);

            return true;
        }
        public async Task<bool> RegisterAsync(RegisterRequestDto registerRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerRequest);
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            return true;
        }

        public async Task LogoutAsync()
        {
            await _httpClient.PostAsync("api/auth/logout", null);
            await ((CustomAuthenticationStateProvider)_authenticationStateProvider)
                .MarkUserAsLoggedOut();
        }
    }
}