namespace HikeConnect.Core.Dtos
{
    public class LoginResponse
    {
        public bool IsLoggedIn { get; set; }
        public string? ErrorMessage { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; } // в контроллере уходит в куки
        public DateTime RefreshTokenExpiresAt { get; set; }
    }
}
