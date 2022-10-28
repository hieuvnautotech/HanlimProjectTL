using Microsoft.EntityFrameworkCore;
using QuizAPI.Models.Dtos.Common;
using System.ComponentModel.DataAnnotations;

namespace QuizAPI.Models.Dtos
{
    public class PurchaseOrderDto : BaseModel
    {
        public long? PoId { get; set; }
        public string? PoCode { get; set; }
        public string? Description { get; set; }
        public int? TotalQty { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public int? RemainQty { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
