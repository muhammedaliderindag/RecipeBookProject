using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RecipeBookProject.Business.Abstract;
using RecipeBookProject.Data.Entities;
using RecipeBookProject.DataAccess.Repositories.Abstract;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly IRefreshTokenRepository _refreshTokenRepository; // Değişiklik

    // DbContext yerine IRefreshTokenRepository'yi enjekte edin
    public TokenService(IConfiguration configuration, IRefreshTokenRepository refreshTokenRepository)
    {
        _configuration = configuration;
        _refreshTokenRepository = refreshTokenRepository; // Değişiklik
    }

    public string GenerateJwtToken(User user)
    {
        // Bu metodun içeriği aynı kalır...
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("firstName", user.FirstName), 
            new Claim("lastName", user.LastName),
            new Claim("userid", user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"])),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<RefreshToken> GenerateAndStoreRefreshTokenAsync(int userId)
    {
        var refreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Expires = DateTime.UtcNow.AddDays(double.Parse(_configuration["Jwt:RefreshTokenExpirationDays"])),
            Created = DateTime.UtcNow,
            UserId = userId
        };

        // DbContext yerine repository'i kullan
        await _refreshTokenRepository.AddAsync(refreshToken);

        return refreshToken;
    }
}