using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Doan.Models.ViewModel
{
    public class OrderViewModel
        {
            [Key]
            public long IDOrder { get; set; }

            public DateTime? OrderDate { get; set; }

            [StringLength(500)]
            public string Descriptions { get; set; }

            public long? CodeCus { get; set; }

            [StringLength(150)]
            public string Email_Cus { get; set; }

            [StringLength(50)]
            public string Password_cus { get; set; }

            [StringLength(25)]
            public string SDT_Cus { get; set; }

            public DateTime? Delivery_date { get; set; }

            public bool? Status { get; set; }

            public bool? Paid { get; set; }

            [StringLength(50)]
            public string Cancelled { get; set; }

            [StringLength(50)]
            public bool? Deleted { get; set; }

            public virtual Customer Customer { get; set; }
        }
    }
