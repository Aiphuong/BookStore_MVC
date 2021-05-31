

namespace Doan.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Shipping")]
    public class Shipping
    {
        [Key]
        public long IDShip { get; set; }
        public decimal Total { get; set; }
        public bool Status { get; set; }
        public decimal Shipping_fee { get; set; }
        public long ID_Ship { get; set; }
        public virtual Shipping_unit Shipping_Unit { get; set; }

        public virtual ICollection<_Order> Order { get; set; }

    }
}