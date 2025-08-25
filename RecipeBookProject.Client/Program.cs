using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization; // Bu using önemli
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RecipeBookProject.Client; // Projenizin adý
using RecipeBookProject.Client.Services;
using System.Net.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// 1. Authorization Core servislerini ekle
builder.Services.AddAuthorizationCore();

builder.Services.AddBlazoredSessionStorage();
// 2. Kendi yazdýðýmýz AuthenticationStateProvider'ý DI container'a ekle
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

// 3. API ile iletiþim kuracak olan Auth servisini ekle
builder.Services.AddScoped<IAuthService, AuthService>();

// 4. Token yenileme mantýðýný yönetecek olan HttpMessageHandler'ý ekle
builder.Services.AddScoped<AuthHeaderHandler>();
builder.Services.AddScoped<AdminProductsService>();
// 5. HttpClient'ý API adresini ve AuthHeaderHandler'ý kullanacak þekilde yapýlandýr
builder.Services.AddHttpClient("Api", client =>
{
    // API'nizin adresini buraya yazýn
    client.BaseAddress = new Uri("https://localhost:7129/");
})
.AddHttpMessageHandler<AuthHeaderHandler>();

// HttpClient'ý kolayca enjekte etmek için bir yardýmcý
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api"));

await builder.Build().RunAsync();
