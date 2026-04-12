namespace HikeConnect.Core.Dtos
{
    public class RefreshTokenResponse
    {
        public string? ErrorMessage { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiresAt { get; set; }
    }
}
