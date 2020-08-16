using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WICRWebApp.Model
{
    public partial class Materials
    {
        [Column("ProjectID")]
        public int? ProjectId { get; set; }
        [StringLength(255)]
        public string MaterialName { get; set; }
        [Column(TypeName = "decimal(18, 0)")]
        public decimal? MaterialPrice { get; set; }
        public int? Coverage { get; set; }
        [Column(TypeName = "decimal(18, 0)")]
        public decimal? MaterialWeight { get; set; }
        [Column(TypeName = "decimal(18, 0)")]
        public decimal? HorProdRate { get; set; }
        [Column(TypeName = "decimal(18, 0)")]
        public decimal? VertProdRate { get; set; }
        [Column(TypeName = "decimal(18, 0)")]
        public decimal? StairProdRate { get; set; }
        [Column(TypeName = "decimal(18, 0)")]
        public decimal? MinChargePerHr { get; set; }
        [Column("MaterialID")]
        public int MaterialId { get; set; }
    }
}
