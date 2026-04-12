namespace HikeConnect.Core.Dtos
{
    public class LoginResponse
    {
        public bool IsLoggedIn { get; set; }
        public string? ErrorMessage { get; set; }
        public string? AccessToken { get; set; }
        // RefreshToken в куки
    }
}
