using HikeConnect.Api.Extensions;
using HikeConnect.Core.Dtos;
using HikeConnect.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HikeConnect.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authManager;

        public AuthController(IAuthService authManager)
        {
            _authManager = authManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authManager.LoginAsync(request);
            LoginResponse response = new()
            {
                IsLoggedIn = result.IsLoggedIn,
                ErrorMessage = result.ErrorMessage,
                AccessToken = result.AccessToken
            };

            if (!response.IsLoggedIn) return Unauthorized(response);

            HttpContext.Response.Cookies.Append("refreshToken", result.RefreshToken!, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = result.RefreshTokenExpiresAt
            });

            return Ok(response);
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = HttpContext.Request.Cookies["refreshToken"];
            await _authManager.LogoutAsync(refreshToken);
            HttpContext.Response.Cookies.Delete("refreshToken");

            return Ok();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authManager.RegisterAsync(request);
            RegisterResponse response = new()
            {
                IsRegistered = result.IsRegistered,
                ErrorMessage = result.ErrorMessage
            };

            if (!result.IsRegistered) return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("refresh")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = HttpContext.Request.Cookies["refreshToken"];
            var result = await _authManager.RefreshTokenAsync(refreshToken);
            
            RefreshTokenResponse response = new()
            {
                ErrorMessage = result.ErrorMessage,
                AccessToken = result.AccessToken
            };

            if (response.AccessToken is null || response.RefreshToken is null) return Unauthorized(response);

            HttpContext.Response.Cookies.Append("refreshToken", result.RefreshToken!, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = result.RefreshTokenExpiresAt
            });

            return Ok(response);
        }

        [HttpGet("user/{id:guid}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _authManager.GetUserByIdAsync(id);
            return user is null
                ? NotFound()
                : Ok(user);
        }

        [HttpGet("user/username/{userName}")]
        public async Task<IActionResult> GetUserByUserName(string userName)
        {
            var user = await _authManager.GetUserByUserNameAsync(userName);
            return user is null
                ? NotFound()
                : Ok(user);
        }

        [Authorize]
        [HttpPatch("user/bio")]
        public async Task<IActionResult> UpdateBio([FromBody] UpdateUserBioRequest request)
        {
            if (!User.TryGetUserId(out var userId))
            {
                return Unauthorized();
            }

            var user = await _authManager.UpdateBioAsync(userId, request.Bio);
            return user is null
                ? BadRequest()
                : Ok(user);
        }
    }
}
