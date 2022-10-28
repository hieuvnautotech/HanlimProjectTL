using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using QuizAPI.Models.Dtos.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QuizAPI.Models.Dtos
{
    public class StandardQCDto : BaseModel
    {
        [Key]
        public long QCId { get; set; }
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public string QCCode { get; set; } = string.Empty;
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;
        public bool showDelete { get; set; }
    }
}
