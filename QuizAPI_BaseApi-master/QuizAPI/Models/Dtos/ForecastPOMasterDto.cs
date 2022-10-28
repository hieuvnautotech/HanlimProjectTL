using Microsoft.EntityFrameworkCore;
using QuizAPI.Models.Dtos.Common;
using System.ComponentModel.DataAnnotations;

namespace QuizAPI.Models.Dtos
{
    public class ForecastPOMasterDto : BaseModel
    {
        public long? FPoMasterId { get; set; }
        public string? FPoMasterCode { get; set; }
        public int? TotalOrderQty { get; set; }
    }
}
