using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace QuizAPI.Models.Dtos.Common
{
    public class RoleDto : BaseModel
    {
        public long roleId { get; set; }

        [Required]
        [StringLength(50)]
        public string? roleName { get; set; } = default;

        public IEnumerable<PermissionDto>? Permissions { get; set; } = default;
        public IEnumerable<MenuDto>? Menus { get; set; } = default;

        public IEnumerable<long>? PermissionIds { get; set; } = default;
        public IEnumerable<long>? MenuIds { get; set; } = default;
    }

    public class RoleDeleteDto
    {
        public long roleId { get; set; }
        public IEnumerable<PermissionDto>? Permissions { get; set; } = default;
        public IEnumerable<MenuDto>? Menus { get; set; } = default;
        public long? createdBy { get; set; } = default;
        public List<long>? permissionIds { get; set; }
        public List<long>? menuIds { get; set; }
    }
}
