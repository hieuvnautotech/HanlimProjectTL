using Microsoft.EntityFrameworkCore;
using QuizAPI.Models.Dtos.Common;
using System.ComponentModel.DataAnnotations;

namespace QuizAPI.Models.Dtos
{
    public class BuyerDto: BaseModel
    {
        public long BuyerId { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string BuyerCode { get; set; } = string.Empty;
        [StringLength(200)]
        public string BuyerName { get; set; } = string.Empty;
        public string Contact { get; set; } = string.Empty;
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;
    }
}
