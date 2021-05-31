namespace Doan.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OrderDetail")]
    public partial class OrderDetail
    {
        public long ID { get; set; }

        public int? QuantitySale { get; set; }

        public long? IDOrder { get; set; }

        public long IDProduct { get; set; }

        public decimal? UnitPriceSale { get; set; }

        public DateTime? CreadeDay { get; set; }

        public long? IdCoupon { get; set; }

        public virtual _Order C_Order { get; set; }

        public virtual Product Product { get; set; }
    }
}
