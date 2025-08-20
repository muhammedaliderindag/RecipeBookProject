using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using RecipeBookProject.Business.Abstract;
using RecipeBookProject.Business.Models;
using RecipeBookProject.Data.Entities;
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request.Email, request.Password);

        if (!result.Succeeded)
            return Unauthorized(result.ErrorMessage);

        SetRefreshTokenCookie(result.NewRefreshToken);
        return Ok(new AuthResponseDto(result.AccessToken));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody]RegisterRequestDto request)
    {
        var result = await _authService.RegisterAsync(request);
        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshTokenValue = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshTokenValue))
            return Unauthorized("Refresh token bulunamadı.");

        var result = await _authService.RefreshTokenAsync(refreshTokenValue);

        if (!result.Succeeded)
            return Unauthorized(result.ErrorMessage);

        SetRefreshTokenCookie(result.NewRefreshToken);
        return Ok(new AuthResponseDto(result.AccessToken));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshTokenValue = Request.Cookies["refreshToken"];
        if (!string.IsNullOrEmpty(refreshTokenValue))
        {
            await _authService.InvalidateRefreshTokenAsync(refreshTokenValue);
        }

        Response.Cookies.Delete("refreshToken");
        return Ok();
    }

    private void SetRefreshTokenCookie(RefreshToken refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = refreshToken.Expires,
            Secure = true,
            SameSite = SameSiteMode.Strict
        };
        Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
    }
}