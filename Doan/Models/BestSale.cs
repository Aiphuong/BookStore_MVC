namespace Doan.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BestSale")]
    public partial class BestSale
    {
        [Key]
        public long IDPro { get; set; }

        [StringLength(250)]
        public string NamePro { get; set; }

        [StringLength(250)]
        public string Image { get; set; }

        public decimal? Price { get; set; }

        public int? Quantity { get; set; }

        public int? QuantitySucess { get; set; }

        [Required]
        [StringLength(128)]
        public string Discriminator { get; set; }

        [Required]
        [StringLength(128)]
        public string Discriminator1 { get; set; }

        [Required]
        [StringLength(128)]
        public string Discriminator2 { get; set; }

        [Required]
        [StringLength(128)]
        public string Discriminator3 { get; set; }

        [Required]
        [StringLength(128)]
        public string Discriminator4 { get; set; }

        [Required]
        [StringLength(128)]
        public string Discriminator5 { get; set; }

        [Required]
        [StringLength(128)]
        public string Discriminator6 { get; set; }
    }
}
