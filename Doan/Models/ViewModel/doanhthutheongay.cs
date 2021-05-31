using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Doan.Models.ViewModel
{
    public class doanhthutheongay
    {
        [Key]
        public long? IDO { get; set; }
        public DateTime? OrderDate { get; set; }

        public long? QuantityOrder { get; set; }

        public decimal? Total { get; set; }
        public List<OrderDetail> Days { get; set; }
    }
}