﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuizAPI.Models
{
    public partial class ForecastPO
    {
        [Key]
        public long FPOId { get; set; }
        public long? FPoMasterId { get; set; }
        public long MaterialId { get; set; }
        public long LineId { get; set; }
        public byte Week { get; set; }
        public int Year { get; set; }
        public int? Amount { get; set; }
        public int? OrderQty { get; set; }
        [Required]
        public bool? isActived { get; set; }
        [Precision(3)]
        public DateTime createdDate { get; set; }
        public long createdBy { get; set; }
        [Precision(3)]
        public DateTime? modifiedDate { get; set; }
        public long? modifiedBy { get; set; }
        public byte[] row_version { get; set; }

        [ForeignKey("FPoMasterId")]
        [InverseProperty("ForecastPO")]
        public virtual ForecastPOMaster FPoMaster { get; set; }
        [ForeignKey("LineId")]
        [InverseProperty("ForecastPO")]
        public virtual Line Line { get; set; }
        [ForeignKey("MaterialId")]
        [InverseProperty("ForecastPO")]
        public virtual Material Material { get; set; }
    }
}