using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Net.Http.Json; // GetFromJsonAsync için
using Blazored.SessionStorage; // SessionStorage için paketi ekleyin
using System.Text.Json;
using System.Collections.Generic;
using System;
using System.Linq;

namespace RecipeBookProject.Client.Services
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ISessionStorageService _sessionStorage;
        private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        public CustomAuthenticationStateProvider(ISessionStorageService sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var accessToken = await _sessionStorage.GetItemAsync<string>("accessToken");
                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    return new AuthenticationState(_anonymous);
                }

                var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(accessToken), "jwt"));
                return new AuthenticationState(claimsPrincipal);
            }
            catch
            {
                return new AuthenticationState(_anonymous);
            }
        }

        public async Task MarkUserAsAuthenticated(string accessToken)
        {
            await _sessionStorage.SetItemAsync("accessToken", accessToken);
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(accessToken), "jwt"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }

        public async Task MarkUserAsLoggedOut()
        {
            await _sessionStorage.RemoveItemAsync("accessToken");
            var authState = Task.FromResult(new AuthenticationState(_anonymous));
            NotifyAuthenticationStateChanged(authState);
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            if (keyValuePairs != null)
            {
                //keyValuePairs.TryGetValue(ClaimTypes.NameIdentifier, out object sub);
                //if (sub != null) claims.Add(new Claim(ClaimTypes.NameIdentifier, sub.ToString()));

                keyValuePairs.TryGetValue(ClaimTypes.Email, out object email);
                if (email != null) claims.Add(new Claim(ClaimTypes.Email, email.ToString()));

                keyValuePairs.TryGetValue("firstName", out object firstName);
                if (firstName != null) claims.Add(new Claim(ClaimTypes.Name, firstName.ToString()));

                keyValuePairs.TryGetValue("lastName", out object lastName);
                if (lastName != null) claims.Add(new Claim(ClaimTypes.Surname, lastName.ToString()));
                
                keyValuePairs.TryGetValue("userid", out object userid);
                if (userid != null) claims.Add(new Claim(ClaimTypes.NameIdentifier, userid.ToString()));
            }
            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
