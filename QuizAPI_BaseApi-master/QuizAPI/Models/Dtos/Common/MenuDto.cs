using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace QuizAPI.Models.Dtos.Common
{
    public class MenuDto : BaseModel
    {
        public long menuId { get; set; }

        public long? parentId { get; set; } = default(long?);

       
        [StringLength(100)]
        public string? menuName { get; set; } = string.Empty;

        public string? parentMenuName { get; set; }

        public byte? menuLevel { get; set; } = byte.MinValue;

        public byte? sortOrder { get; set; } = byte.MinValue;

        [StringLength(50)]
        [Unicode(false)]
        public string? menuIcon { get; set; } = string.Empty;

        [StringLength(100)]
        [Unicode(false)]
        public string? languageKey { get; set; } = string.Empty;

        [StringLength(50)]
        [Unicode(false)]
        public string? menuComponent { get; set; } = string.Empty;

        [StringLength(200)]
        [Unicode(false)]
        public string? navigateUrl { get; set; } = string.Empty;

        public bool forRoot { get; set; } = false;
        public bool? forApp { get; set; } = false;

    }
}
