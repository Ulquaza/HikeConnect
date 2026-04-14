using HikeConnect.Core.Dtos;
using HikeConnect.Core.Entities;
using HikeConnect.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HikeConnect.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;

        public AuthService(UserManager<User> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _userManager.Users.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<User?> GetUserByUserNameAsync(string userName)
        {
            return await _userManager.Users.FirstOrDefaultAsync(e => e.UserName == userName);
        }

        public async Task<User?> UpdateBioAsync(Guid userId, string? bio)
        {
            if (userId == Guid.Empty)
            {
                return null;
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user is null)
            {
                return null;
            }

            user.Bio = string.IsNullOrWhiteSpace(bio) ? null : bio.Trim();
            var updateResult = await _userManager.UpdateAsync(user);
            return updateResult.Succeeded ? user : null;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            User? user;

            if (loginRequest.EmailOrUsername.Contains('@'))
                user = await _userManager.FindByEmailAsync(loginRequest.EmailOrUsername);
            else
                user = await _userManager.FindByNameAsync(loginRequest.EmailOrUsername);

            if (user is null || !await _userManager.CheckPasswordAsync(user, loginRequest.Password))
                return new() { ErrorMessage = "Неверные имя пользователя и/или пароль." };

            List<Claim> claims = await GetClaimsAsync(user);
            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();
            var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(_tokenService.GetRefreshTokenExpirationDays());

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiresAt = refreshTokenExpiresAt;
            await _userManager.UpdateAsync(user);

            return new()
            {
                IsLoggedIn = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                RefreshTokenExpiresAt = refreshTokenExpiresAt
            };
        }

        public async Task LogoutAsync(string? refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken)) return;

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
            if (user is null) return;

            user.RefreshToken = null;
            user.RefreshTokenExpiresAt = null;
            await _userManager.UpdateAsync(user);
        }

        public async Task<RefreshTokenResponse> RefreshTokenAsync(string? refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return new() { ErrorMessage = "Refresh token отсутствует." };

            User? user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user is null || user.RefreshTokenExpiresAt <= DateTime.UtcNow)
                return new() { ErrorMessage = "Refresh token просрочен." };

            var claims = await GetClaimsAsync(user);
            var newAccessToken = _tokenService.GenerateAccessToken(claims);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var newRefreshTokenExpirationDate = DateTime.UtcNow.AddDays(_tokenService.GetRefreshTokenExpirationDays());

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiresAt = newRefreshTokenExpirationDate;
            await _userManager.UpdateAsync(user);

            return new()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                RefreshTokenExpiresAt = newRefreshTokenExpirationDate
            };
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest registerRequest)
        {
            if (await _userManager.FindByEmailAsync(registerRequest.Email) is not null)
                return new() { ErrorMessage = "Пользователь с такой почтой уже существует." };

            if (await _userManager.FindByNameAsync(registerRequest.Username) is not null)
                return new() { ErrorMessage = "Пользователь с таким именем уже существует." };

            var user = new User
            {
                FullName = registerRequest.FullName,
                UserName = registerRequest.Username,
                Email = registerRequest.Email,
                RegisteredAt = DateTime.UtcNow
            };

            var createResult = await _userManager.CreateAsync(user, registerRequest.Password);
            if (!createResult.Succeeded)
            {
                var errors = createResult.Errors.Select(e => e.Code);
                var translatedErrors = string.Join("; ", errors.Select(TranslateErrorCode));
                return new() { ErrorMessage = translatedErrors };
            }

            string defaultRole = "Пользователь";   
            await _userManager.AddToRoleAsync(user, defaultRole);   // предполагается, что роль по умолчанию уже существует

            return new() { IsRegistered = true };
        }

        private static string TranslateErrorCode(string code)
        {
            string translatedError = code switch
            {
                "PasswordTooShort" => "Пароль слишком короткий.",
                "PasswordRequiresNonAlphanumeric" => "Пароль должен содержать хотя бы один специальный символ.",
                "PasswordRequiresDigit" => "Пароль должен содержать хотя бы одну цифру.",
                "PasswordRequiresLower" => "Пароль должен содержать хотя бы одну строчную букву.",
                "PasswordRequiresUpper" => "Пароль должен содержать хотя бы одну заглавную букву.",
                "InvalidUserName" => "Имя пользователя содержит запрещённые символы",
                _ => $"Необработанная ошибка: {code}"
            };

            return translatedError;
        }

        private async Task<List<Claim>> GetClaimsAsync(User user)
        {
            var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "Пользователь";
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Role, role)
            };
            return claims;
        }
    }
}
