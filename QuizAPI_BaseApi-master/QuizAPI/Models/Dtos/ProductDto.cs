using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using QuizAPI.Models.Dtos.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QuizAPI.Models.Dtos
{
    public class ProductDto : BaseModel
    {
        [Key]
        public long MaterialId { get; set; }
        [Required]
        [StringLength(11)]
        [Unicode(false)]
        public string MaterialCode { get; set; } = string.Empty;
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;
        public long MaterialType { get; set; }
        public long ProductType { get; set; }
        public long Model { get; set; }
        [Column(TypeName = "decimal(9, 3)")]
        public decimal? Inch { get; set; }
        public long Unit { get; set; }

        //ngoại biến
        public string ModelName { get; set; } = string.Empty;
        public string ProductTypeName { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public bool showDelete { get; set; }
    }
}
