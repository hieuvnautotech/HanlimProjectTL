﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using QuizAPI.Models.Dtos.Common;

namespace QuizAPI.Models.Dtos
{
    public partial class BomDto : BaseModel
    {
        public long? BomId { get; set; }
        public string BomCode { get; set; }
        public string Remark { get; set; }
        public long? ParentId { get; set; }
        public string ParentCode { get; set; }
        public int? BomLevel { get; set; }
        public string BomLevelGroup { get; set; }
        public long? MaterialId { get; set; }
        public decimal? Amount { get; set; }
        public string MaterialCode { get; set; }
        public string MaterialType { get; set; }
        public string Version { get; set; }
        public string MaterialUnit { get; set; }
    }
}