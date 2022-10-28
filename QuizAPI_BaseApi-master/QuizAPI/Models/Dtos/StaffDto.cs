using Microsoft.EntityFrameworkCore;
using QuizAPI.Models.Dtos.Common;
using System.ComponentModel.DataAnnotations;

namespace QuizAPI.Models.Dtos
{
    public class StaffDto : BaseModel
    {
        public long StaffId { get; set; }
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public string StaffCode { get; set; }
        [StringLength(100)]
        public string StaffName { get; set; }
    }
}
