using Microsoft.EntityFrameworkCore;
using QuizAPI.Models.Dtos.Common;
using System.ComponentModel.DataAnnotations;

namespace QuizAPI.Models.Dtos
{
    public class LocationDto : BaseModel
    {
        public long? LocationId { get; set; }
        public string? LocationCode { get; set; }
        public long? AreaId { get; set; }
        public string? AreaName { get; set; }
    }
}
