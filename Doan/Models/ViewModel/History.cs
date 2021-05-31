using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Doan.Models.ViewModel
{
    public class History
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long? ID { get; set; }
        public long? IDCus { get; set; }
        public long? IDPro { get; set; }
        public DateTime? OrderDate { get; set; }
        public string ProductName { get; set; }
        public long? Quantity { get; set; }
        public decimal? Price { get; set; }

        public string Image { get; set; }
    }
}