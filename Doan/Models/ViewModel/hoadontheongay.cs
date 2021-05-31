using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Doan.Models.ViewModel
{
    public class hoadontheongay
    {
        [Key]
        public long ? IDOrder { get; set; }
        public DateTime? OrderDate { get; set; }
        public long? CodeCus { get; set; }

        public int? QuantitySale { get; set; }

        public decimal? UnitPriceSale { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}