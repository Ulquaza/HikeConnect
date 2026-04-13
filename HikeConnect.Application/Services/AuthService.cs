using HikeConnect.Core.Dtos;
using HikeConnect.Core.Entities;
using HikeConnect.Core.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace HikeConnect.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;

        public AuthService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public Task<User?> GetUserByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetUserByUserNameAsync(string userName)
        {
            throw new NotImplementedException();
        }

        public Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            throw new NotImplementedException();
        }

        public Task LogoutAsync(string? refreshToken)
        {
            throw new NotImplementedException();
        }

        public Task<RefreshTokenResponse> RefreshTokenAsync(string? refreshToken)
        {
            throw new NotImplementedException();
        }

        public Task<RegisterResponse> RegisterAsync(RegisterRequest registerRequest)
        {
            throw new NotImplementedException();
        }
    }
}
