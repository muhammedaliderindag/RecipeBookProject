
using RecipeBookProject.Business.Abstract;
using RecipeBookProject.Business.Models;
using RecipeBookProject.Data.Entities;
using RecipeBookProject.DataAccess.Repositories.Abstract;



public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository; // Değişiklik

    // DbContext yerine IRefreshTokenRepository'yi enjekte edin
    public AuthService(IUserRepository userRepository,ITokenService tokenService,IRefreshTokenRepository refreshTokenRepository)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository; // Değişiklik
    }

    // LoginAsync metodu aynı kalır, çünkü token oluşturma işini zaten TokenService yapıyordu.
    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        RefreshToken refreshToken = new RefreshToken();
        var user = await _userRepository.GetUserByIdAsync(email);
        var refreshTokenRepo = await _refreshTokenRepository.GetRefreshTokenByIdAsync(user.UserId);
        if (user == null)
            return new AuthResult(false, "Kullanıcı bulunamadı.");

        // DİKKAT: Gönderdiğiniz resimde PasswordHashed varchar(50) olarak görünüyor.
        // BCrypt hash'i 60 karakterdir. Veritabanı alanınızı en az varchar(100) yapmalısınız.
        // Aksi takdirde bu kod çalışmaz!
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHashed))
              return new AuthResult(false, "Geçersiz şifre.");
       

        if (refreshTokenRepo == null)
        {
            refreshToken = await _tokenService.GenerateAndStoreRefreshTokenAsync(user.UserId);
        }
        else
        {
            if (refreshTokenRepo == null || !refreshTokenRepo.IsActive)
            {
                refreshToken = await _tokenService.GenerateAndStoreRefreshTokenAsync(user.UserId);
            }
            else
            {
                refreshToken = refreshTokenRepo;
            }
                
        }
            var accessToken = _tokenService.GenerateJwtToken(user);
        

        return new AuthResult(true, AccessToken: accessToken, NewRefreshToken: refreshToken);
    }

    public async Task<AuthResult> RefreshTokenAsync(string refreshTokenValue)
    {
        // DbContext yerine repository'i kullan
        var dbToken = await _refreshTokenRepository.GetByTokenAsync(refreshTokenValue);

        if (dbToken == null || !dbToken.IsActive)
            return new AuthResult(false, "Geçersiz token.");

        var newAccessToken = _tokenService.GenerateJwtToken(dbToken.User);
        var newRefreshToken = await _tokenService.GenerateAndStoreRefreshTokenAsync(dbToken.UserId);

        // Eski token'ı geçersiz kıl
        dbToken.Revoked = DateTime.UtcNow;
        await _refreshTokenRepository.UpdateAsync(dbToken); // Değişiklik

        return new AuthResult(true, AccessToken: newAccessToken, NewRefreshToken: newRefreshToken);
    }

    public async Task InvalidateRefreshTokenAsync(string refreshTokenValue)
    {
        // DbContext yerine repository'i kullan
        var dbToken = await _refreshTokenRepository.GetByTokenAsync(refreshTokenValue);

        if (dbToken != null && dbToken.IsActive)
        {
            dbToken.Revoked = DateTime.UtcNow;
            await _refreshTokenRepository.UpdateAsync(dbToken); // Değişiklik
        }
    }

    public async Task<GeneralResponse<NoData>> RegisterAsync(RegisterRequestDto request)
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.PasswordHashed);
        var register = new User
        {
            Email = request.Email,
            PasswordHashed = hashedPassword,
            FirstName = request.FirstName,
            LastName = request.LastName
        };
        await _userRepository.AddAsync(register);
        return GeneralResponse<NoData>.Success("Kullanıcı başarıyla oluşturuldu.",201);
    }
}