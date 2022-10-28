using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace QuizAPI.Models.Dtos.Common
{
    public class LoginModelDto
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        [Unicode(false)]
        public string? userName { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        [Unicode(false)]
        public string? userPassword { get; set; }

        public bool? isOnApp { get; set; } = false;

        public LoginModelDto()
        {
            isOnApp = false;
        }
    }
}
