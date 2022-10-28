using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace QuizAPI.Models.Dtos.Common
{
    public class UserDto : BaseModel
    {
        public long userId { get; set; }

        [StringLength(100)]
        [Unicode(false)]
        public string? userName { get; set; }

        [StringLength(100)]
        public string? userPassword { get; set; }

        [StringLength(100)]
        public string? newPassword { get; set; }

        [Precision(3)]
        public DateTime? lastLoginOnWeb { get; set; }

        [Precision(3)]
        public DateTime? lastLoginOnApp { get; set; }

        //// Role list
        public IEnumerable<RoleDto>? Roles { get; set; }
        public IEnumerable<string>? RoleNames { get; set; }
        public string? RoleNameList { get; set; }

        /// <summary>
        /// UserPermission
        /// </summary>
        public IEnumerable<string>? PermissionNames { get; set; }

        //Menu List
        public IEnumerable<MenuDto>? Menus { get; set; }

        //Menu Tree
        public IEnumerable<MenuTreeDto>? MenuTree { get; set; }

        //Token
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }

        //Response Message
        //public string? ResponseMessage { get; set; }
        
    }
}
