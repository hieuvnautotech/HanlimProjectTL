using Microsoft.EntityFrameworkCore;
using QuizAPI.Models.Dtos.Common;
using System.ComponentModel.DataAnnotations;

namespace QuizAPI.Models.Dtos
{
    public class WorkOrderDto : BaseModel
    {
        public long WoId { get; set; }
        public string? WoCode { get; set; }
        public long? FPoMasterId { get; set; }
        public long? MaterialId { get; set; }
        public long? LineId { get; set; }
        public int? OrderQty { get; set; }
        public DateTime? StartDate { get; set; }
        public int? ActualQty { get; set; }

        //Required Properties
        public string? FPoMasterCode { get; set; }
        public string? MaterialCode { get; set; }
        public string? LineName { get; set; }
        public DateTime? StartSearchingDate { get; set; }
        public DateTime? EndSearchingDate { get; set; }
    }
}
