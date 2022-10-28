using Microsoft.EntityFrameworkCore;
using QuizAPI.Models.Dtos.Common;
using System.ComponentModel.DataAnnotations;

namespace QuizAPI.Models.Dtos
{
    public class TrayDto : BaseModel
    {
        public long? TrayId { get; set; }
        public string? TrayCode { get; set; }
        public long? TrayType { get; set; }
        public string? TrayTypeName { get; set; }
        public bool? IsReuse { get; set; }
    }
}
