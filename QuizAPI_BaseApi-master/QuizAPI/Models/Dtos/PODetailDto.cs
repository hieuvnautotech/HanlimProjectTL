using Microsoft.EntityFrameworkCore;
using QuizAPI.Models.Dtos.Common;
using System.ComponentModel.DataAnnotations;

namespace QuizAPI.Models.Dtos
{
    public class PODetailDto : BaseModel
    {
        public long? PoDetailId { get; set; }
        public long? PoId { get; set; }
        public string? PoCode { get; set; }
        public long? MaterialId { get; set; }
        public string? Description { get; set; }
        public string? MaterialCode { get; set; }
        public int? Qty { get; set; }
        public int? RemainQty { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string? DeliveryDateFormat { get; set; }
        public string? DueDateFormat { get; set; }
    }
}
