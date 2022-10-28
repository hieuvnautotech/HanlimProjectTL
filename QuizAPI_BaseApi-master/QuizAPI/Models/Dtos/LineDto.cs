using QuizAPI.Models.Dtos.Common;
using System.ComponentModel.DataAnnotations;

namespace QuizAPI.Models.Dtos
{
    public class LineDto : BaseModel
    {
        public long? LineId { get; set; }
        public string? LineName { get; set; }
        public string? Description { get; set; }
        public bool? IsUsed { get; set; }
    }
}
