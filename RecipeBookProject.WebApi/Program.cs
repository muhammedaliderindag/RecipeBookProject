using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.StaticFiles;
using RecipeBookProject.Business.Abstract;
using RecipeBookProject.Business.Concrete;
using RecipeBookProject.Business.Middleware;
using RecipeBookProject.Data.Context;
using RecipeBookProject.DataAccess.Repositories.Abstract;
using RecipeBookProject.DataAccess.Repositories.Concrete;
using RecipeBookProject.WebApi.Services;
using Swashbuckle.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// 1. Veritabanı bağlantısı
builder.Services.AddDbContext<RecipeBookProjectDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

// 2. JWT Authentication eklenmesi
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
            ClockSkew = TimeSpan.Zero // Süre toleransını kaldırır, token biter bitmez geçersiz olur
        };
    });

// CORS ayarı (Blazor uygulamasının API'ye erişebilmesi için)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp",
        policy =>
        {
            policy.WithOrigins(
                "https://localhost:7014",  // Blazor app'in adresi
                "https://localhost:5000",  // Alternatif port
                "http://localhost:5000",   // HTTP alternatifi
                "http://localhost:7014"    // HTTP alternatifi
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // Refresh token cookie'si için bu gerekli
        });
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "My API", Version = "v1" });
    // XML yorumlarını dahil etmek için:
    // var xmlPath = Path.Combine(AppContext.BaseDirectory, "MyApi.xml");
    // c.IncludeXmlComments(xmlPath);
});

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<IPendingProductRepository, PendingProductRepository>();
builder.Services.AddScoped<IAdminPendingProductsService, AdminPendingProductsService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IIngredientRepository, IngredientRepository>();
builder.Services.AddScoped<IIngredientService, IngredientService>();
builder.Services.AddScoped<IFileService, FileService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  //  app.MapOpenApi();
    // JSON spec → /swagger/v1/swagger.json
    app.UseSwagger();
    // UI → /swagger/index.html
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        // c.RoutePrefix = ""; // kök dizine açmak için
    });
}

app.UseHttpsRedirection();

// CORS'u en başta kullan (Authentication'dan önce)
app.UseCors("AllowBlazorApp");

// Static dosyaları serve et (uploads klasörü için)
app.UseStaticFiles();

Console.WriteLine($"DEBUG: Default static files configured for wwwroot: {builder.Environment.WebRootPath}");

// Uploads klasörü için özel static file serving yapılandırması (sadece gerekirse)
var uploadsPath = Path.Combine(builder.Environment.WebRootPath, "uploads");
Console.WriteLine($"DEBUG: Uploads path: {uploadsPath}");
Console.WriteLine($"DEBUG: Uploads path exists: {Directory.Exists(uploadsPath)}");

if (Directory.Exists(uploadsPath))
{
    Console.WriteLine($"DEBUG: Uploads directory exists, files: {string.Join(", ", Directory.GetFiles(uploadsPath, "*", SearchOption.AllDirectories).Select(f => Path.GetFileName(f)))}");
}

// Middleware sırasını kontrol et
Console.WriteLine($"DEBUG: Middleware order - After static files configuration");

// Debug için path'leri logla
Console.WriteLine($"DEBUG: WebRootPath: {builder.Environment.WebRootPath}");
Console.WriteLine($"DEBUG: Uploads path: {Path.Combine(builder.Environment.WebRootPath, "uploads")}");
Console.WriteLine($"DEBUG: Uploads path exists: {Directory.Exists(Path.Combine(builder.Environment.WebRootPath, "uploads"))}");

// Middleware sırasını logla
Console.WriteLine($"DEBUG: Middleware order - CORS and StaticFiles configured");

app.UseExceptionHandlerMiddleware();
app.UseAuthentication(); // Önce kimlik doğrulama
app.UseAuthorization();  // Sonra yetkilendirme
app.MapControllers();

app.Run();