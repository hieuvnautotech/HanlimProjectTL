namespace QuizAPI.Models.Dtos.Common
{
    public class AuthorizationResponse
    {
        public string? accessToken { get; set; } = default;
        public string? refreshToken { get; set; } = default;
        public bool isSuccess { get; set; } = true;
        public string? reason { get; set; } = default; 
    }
}
