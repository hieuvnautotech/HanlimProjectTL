using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace QuizAPI.Models.Dtos.Common
{
    public class PermissionDto : BaseModel
    {
        public long permissionId { get; set; }

        [StringLength(50)]
        public string? permissionName { get; set; } = default;

        public long? commonDetailId { get; set; } = default;
        public string? commonDetailName { get; set; } = default;

        public bool forRoot { get; set; } = true;
    }
}
