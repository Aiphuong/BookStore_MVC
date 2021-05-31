

namespace Doan.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Shipping_unit")]
    public class Shipping_unit
    {
        [Key]
        public long ID_Ship { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Shipping> Shippings { get; set; }
        public virtual ICollection<_Order> Orders { get; set; }

    }
   
}