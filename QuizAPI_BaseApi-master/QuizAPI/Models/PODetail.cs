﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuizAPI.Models
{
    public partial class PODetail
    {
        [Key]
        public long PoDetailId { get; set; }
        public long PoId { get; set; }
        public long MaterialId { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        public int Qty { get; set; }
        public int? RemainQty { get; set; }
        [Precision(0)]
        public DateTime? DeliveryDate { get; set; }
        [Precision(0)]
        public DateTime? DueDate { get; set; }
        [Required]
        public bool? isActived { get; set; }
        [Precision(3)]
        public DateTime createdDate { get; set; }
        public long createdBy { get; set; }
        [Precision(3)]
        public DateTime? modifiedDate { get; set; }
        public long? modifiedBy { get; set; }
        public byte[] row_version { get; set; }

        [ForeignKey("MaterialId")]
        [InverseProperty("PODetail")]
        public virtual Material Material { get; set; }
        [ForeignKey("PoId")]
        [InverseProperty("PODetail")]
        public virtual PurchaseOrder Po { get; set; }
    }
}