using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RecipeBookProject.Business.Abstract;
using RecipeBookProject.Business.Concrete;
using RecipeBookProject.Business.Middleware;
using RecipeBookProject.Data.Context;
using RecipeBookProject.DataAccess.Repositories.Abstract;
using RecipeBookProject.DataAccess.Repositories.Concrete;
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
            policy.WithOrigins(configuration["Jwt:Audience"]) // Blazor app'in adresi
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
app.UseCors("AllowBlazorApp"); // CORS'u aktif et
app.UseExceptionHandlerMiddleware();
app.UseAuthentication(); // Önce kimlik doğrulama
app.UseAuthorization();  // Sonra yetkilendirme
app.MapControllers();

app.Run();