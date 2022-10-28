namespace QuizAPI.Models.Dtos.Common
{
    public class UserAuthorizationDto
    {
        public long userId { get; set; }
        public string userName { get; set; } = string.Empty;
        public string roleName { get; set; } = string.Empty;
        public string permissionName { get; set; } = string.Empty;
    }
}
