using Microsoft.EntityFrameworkCore;
using QuizAPI.Models.Dtos.Common;
using System.ComponentModel.DataAnnotations;

namespace QuizAPI.Models.Dtos
{
    public class SupplierDto: BaseModel
    {
        public long SupplierId { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string SupplierCode { get; set; } = string.Empty;
        [StringLength(200)]
        public string SupplierName { get; set; } = string.Empty;
        public string SupplierContact { get; set; } = string.Empty;
    }
}
