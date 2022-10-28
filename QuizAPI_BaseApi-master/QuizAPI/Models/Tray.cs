﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuizAPI.Models
{
    public partial class Tray
    {
        [Key]
        public long TrayId { get; set; }
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public string TrayCode { get; set; }
        public long TrayType { get; set; }
        public bool? IsReuse { get; set; }
        [Required]
        public bool? isActived { get; set; }
        [Precision(3)]
        public DateTime createdDate { get; set; }
        public long createdBy { get; set; }
        [Precision(3)]
        public DateTime? modifiedDate { get; set; }
        public long? modifiedBy { get; set; }
        public byte[] row_version { get; set; }

        [ForeignKey("TrayType")]
        [InverseProperty("Tray")]
        public virtual sysTbl_CommonDetail TrayTypeNavigation { get; set; }
    }
}