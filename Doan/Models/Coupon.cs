namespace Doan.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Coupon")]
    public partial class Coupon
    {
        [Key]
        public long IdCoupon { get; set; }

        [StringLength(50)]
        public string Name { get; set; }
        public decimal Price { get; set; }
        public virtual ICollection<_Order> C_Order { get; set; }
    }
}
