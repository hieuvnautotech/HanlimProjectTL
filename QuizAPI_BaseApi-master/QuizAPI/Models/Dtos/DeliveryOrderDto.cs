using Microsoft.EntityFrameworkCore;
using QuizAPI.Models.Dtos.Common;
using System.ComponentModel.DataAnnotations;

namespace QuizAPI.Models.Dtos
{
    public class DeliveryOrderDto : BaseModel
    {
        public long DoId { get; set; }
        public string DoCode { get; set; }
        public long PoId { get; set; }
        public long MaterialId { get; set; }
        public int? OrderQty { get; set; }
        public int? RemainQty { get; set; }
        public string PackingNote { get; set; }
        public string InvoiceNo { get; set; }
        public string Dock { get; set; }
        public DateTime? ETDLoad { get; set; }
        public DateTime? DeliveryTime { get; set; }
        public string Remark { get; set; }
        public string Truck { get; set; }

        //Required Properties
        public string PoCode { get; set; }
        public string MaterialCode { get; set; }

        public DeliveryOrderDto()
        {
            DoCode = string.Empty;
            PackingNote = string.Empty;
            InvoiceNo = string.Empty;
            Dock = string.Empty;
            Truck = string.Empty;
            Remark = string.Empty;
            PoCode = string.Empty;
            MaterialCode = string.Empty;
        }
    }
}
