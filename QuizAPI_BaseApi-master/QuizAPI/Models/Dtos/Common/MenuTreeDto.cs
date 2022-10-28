namespace QuizAPI.Models.Dtos.Common
{
    public class MenuTreeDto
    {
        public long menuId { get; set; }
        public long? parentId { get; set; } = default;
        public string? menuName { get; set; } = default;
        public string? menuIcon { get; set; } = default;
        public string? languageKey { get; set; } = default;
        public string? menuComponent { get; set; } = default;
        public string? navigateUrl { get; set; } = default;
        //public bool? Actived { get; set; }
        //public bool? ForRoot { get; set; }
        //public long? ModifiedBy { get; set; }
        public List<MenuTreeDto>? subMenus { get; set; } = default;
    }
}
