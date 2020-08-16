using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WICRWebApp.Model
{
    [Table("project")]
    public partial class Project
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        [StringLength(20)]
        public string Name { get; set; }
        [Column("createdby")]
        [StringLength(20)]
        public string Createdby { get; set; }
        [Column("description")]
        [StringLength(20)]
        public string Description { get; set; }
        [Column("createdon", TypeName = "date")]
        public DateTime? Createdon { get; set; }
        [Column("imagedata")]
        public byte[] Imagedata { get; set; }
    }
}
