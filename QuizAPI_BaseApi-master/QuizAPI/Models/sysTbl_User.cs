﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuizAPI.Models
{
    [Index("userName", Name = "UC_sysTbl_User_userName", IsUnique = true)]
    public partial class sysTbl_User
    {
        public sysTbl_User()
        {
            sysTbl_User_Role = new HashSet<sysTbl_User_Role>();
        }

        [Key]
        public long userId { get; set; }
        [Required]
        [StringLength(100)]
        [Unicode(false)]
        public string userName { get; set; }
        [Required]
        [StringLength(100)]
        public string userPassword { get; set; }
        public bool? isOnline { get; set; }
        [Required]
        public bool? isActived { get; set; }
        [Precision(3)]
        public DateTime createdDate { get; set; }
        public long? createdBy { get; set; }
        [Precision(3)]
        public DateTime? modifiedDate { get; set; }
        public long? modifiedBy { get; set; }
        [Precision(3)]
        public DateTime? lastLoginOnWeb { get; set; }
        [Precision(3)]
        public DateTime? lastLoginOnApp { get; set; }
        public byte[] row_version { get; set; }

        [InverseProperty("user")]
        public virtual ICollection<sysTbl_User_Role> sysTbl_User_Role { get; set; }
    }
}