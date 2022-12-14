// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuizAPI.Models
{
    [Index("PoCode", Name = "UC_PurchaseOrder_PoCode", IsUnique = true)]
    public partial class PurchaseOrder
    {
        public PurchaseOrder()
        {
            PODetail = new HashSet<PODetail>();
        }

        [Key]
        public long PoId { get; set; }
        public long? FPOId { get; set; }
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public string PoCode { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        public int? TotalQty { get; set; }
        public int? RemainQty { get; set; }
        [Required]
        public bool? isActived { get; set; }
        [Precision(3)]
        public DateTime createdDate { get; set; }
        public long createdBy { get; set; }
        [Precision(3)]
        public DateTime? modifiedDate { get; set; }
        public long? modifiedBy { get; set; }
        public byte[] row_version { get; set; }

        [InverseProperty("Po")]
        public virtual ICollection<PODetail> PODetail { get; set; }
    }
}