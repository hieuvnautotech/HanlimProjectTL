﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuizAPI.Models
{
    [Index("MaterialCode", Name = "UC_Material_MaterialCode", IsUnique = true)]
    public partial class Material
    {
        public Material()
        {
            ForecastPO = new HashSet<ForecastPO>();
            Material_Buyer = new HashSet<Material_Buyer>();
            Material_Supplier = new HashSet<Material_Supplier>();
            PODetail = new HashSet<PODetail>();
            WorkOrder = new HashSet<WorkOrder>();
        }

        [Key]
        public long MaterialId { get; set; }
        [Required]
        [StringLength(11)]
        [Unicode(false)]
        public string MaterialCode { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        public long MaterialType { get; set; }
        public long? ProductType { get; set; }
        public long? Model { get; set; }
        [Column(TypeName = "decimal(9, 3)")]
        public decimal? Inch { get; set; }
        public long Unit { get; set; }
        [Required]
        public bool? isActived { get; set; }
        [Precision(3)]
        public DateTime createdDate { get; set; }
        public long createdBy { get; set; }
        [Precision(3)]
        public DateTime? modifiedDate { get; set; }
        public long? modifiedBy { get; set; }
        public byte[] row_version { get; set; }

        [ForeignKey("MaterialType")]
        [InverseProperty("MaterialMaterialTypeNavigation")]
        public virtual sysTbl_CommonDetail MaterialTypeNavigation { get; set; }
        [ForeignKey("Unit")]
        [InverseProperty("MaterialUnitNavigation")]
        public virtual sysTbl_CommonDetail UnitNavigation { get; set; }
        [InverseProperty("Material")]
        public virtual ICollection<ForecastPO> ForecastPO { get; set; }
        [InverseProperty("Material")]
        public virtual ICollection<Material_Buyer> Material_Buyer { get; set; }
        [InverseProperty("Material")]
        public virtual ICollection<Material_Supplier> Material_Supplier { get; set; }
        [InverseProperty("Material")]
        public virtual ICollection<PODetail> PODetail { get; set; }
        [InverseProperty("Material")]
        public virtual ICollection<WorkOrder> WorkOrder { get; set; }
    }
}