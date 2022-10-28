using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using QuizAPI.Models.Dtos.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizAPI.Models.Dtos
{
    public class MoldDto : BaseModel
    {
        public long? MoldId { get; set; }
        [Required]
        [StringLength(20)]
        public string? MoldSerial { get; set; }

        [Required]
        [StringLength(11)]
        public string? MoldCode { get; set; }
        public long? Model { get; set; }
        public long? MoldType { get; set; }
        public decimal? Inch { get; set; }
        public long? MachineType { get; set; }
        public decimal? MachineTon { get; set; }
        public DateTime? ETADate { get; set; }
        public int? Cabity { get; set; }
        public bool? ETAStatus { get; set; }
        public string? Remark { get; set; }

        public string? ModelName { get; set; }
        public string? MoldTypeName { get; set; }
        public string? MachineTypeName { get; set; }
        public string? ETAStatus1 { get; set; }

        public string ETAStatusName
        {
            get
            {
                if (ETAStatus == null) return "";
                return (bool)ETAStatus ? "YES" : "NO";
            }
        }
    }

}
