using HikeConnect.Core.Dtos;
using HikeConnect.Core.Entities;

namespace HikeConnect.Core.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest registerRequest);
        Task<LoginResponse> LoginAsync(LoginRequest loginRequest);
        Task LogoutAsync(string? refreshToken);
        Task<RefreshTokenResponse> RefreshTokenAsync(string? refreshToken);

        Task<User?> GetUserByIdAsync(Guid id);
        Task<User?> GetUserByUserNameAsync(string userName);
        Task<User?> UpdateBioAsync(Guid userId, string? bio);
    }
}
