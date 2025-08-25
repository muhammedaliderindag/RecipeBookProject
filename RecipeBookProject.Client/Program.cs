using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization; // Bu using �nemli
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RecipeBookProject.Client; // Projenizin ad�
using RecipeBookProject.Client.Services;
using System.Net.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// 1. Authorization Core servislerini ekle
builder.Services.AddAuthorizationCore();

builder.Services.AddBlazoredSessionStorage();
// 2. Kendi yazd���m�z AuthenticationStateProvider'� DI container'a ekle
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

// 3. API ile ileti�im kuracak olan Auth servisini ekle
builder.Services.AddScoped<IAuthService, AuthService>();

// 4. Token yenileme mant���n� y�netecek olan HttpMessageHandler'� ekle
builder.Services.AddScoped<AuthHeaderHandler>();
builder.Services.AddScoped<AdminProductsService>();
// 5. HttpClient'� API adresini ve AuthHeaderHandler'� kullanacak �ekilde yap�land�r
builder.Services.AddHttpClient("Api", client =>
{
    // API'nizin adresini buraya yaz�n
    client.BaseAddress = new Uri("https://localhost:7129/");
})
.AddHttpMessageHandler<AuthHeaderHandler>();

// HttpClient'� kolayca enjekte etmek i�in bir yard�mc�
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api"));

await builder.Build().RunAsync();
