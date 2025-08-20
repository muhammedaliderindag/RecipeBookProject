using Blazored.SessionStorage;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using RecipeBookProject.Client.Models;
using System.Net.Http.Json;
namespace RecipeBookProject.Client.Services
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly ISessionStorageService _sessionStorage;

        public AuthHeaderHandler(ISessionStorageService sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // İstek gönderilmeden önce token'ı headera ekle
            var token = await _sessionStorage.GetItemAsync<string>("accessToken");
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await base.SendAsync(request, cancellationToken);

            // Eğer response 401 (Unauthorized) ise ve bu bir refresh isteği değilse
            if (response.StatusCode == HttpStatusCode.Unauthorized && !request.RequestUri.AbsolutePath.Contains("/refresh"))
            {
                // Yeni bir HttpClient oluşturarak sonsuz döngüyü engelle
                var refreshClient = new HttpClient
                {
                    BaseAddress = new Uri(request.RequestUri.GetLeftPart(UriPartial.Authority))
                };
                var refreshResponse = await refreshClient.PostAsync("/api/auth/refresh", null, cancellationToken);

                if (refreshResponse.IsSuccessStatusCode)
                {
                    var newAuthResponse = await refreshResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
                    await _sessionStorage.SetItemAsync("accessToken", newAuthResponse.AccessToken);

                    // Başarısız olan orijinal isteğin header'ını yeni token ile güncelle
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newAuthResponse.AccessToken);

                    // Ve isteği tekrar gönder
                    return await base.SendAsync(request, cancellationToken);
                }
            }

            return response;
        }
    }

}
