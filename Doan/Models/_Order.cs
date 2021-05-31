namespace Doan.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("_Order")]
    public partial class _Order
    {
     
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public _Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        [Key]
        public long IDOrder { get; set; }

        public DateTime? OrderDate { get; set; }

        public DateTime? DeletedDay { get; set; }

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

        public decimal Total { get; set; }

        public decimal TotalSuccess { get; set; }


        public int? scores { get; set; }
        public bool? Status { get; set; }
        public decimal Collected { get; set; }

        public bool? Paid { get; set; }

        public bool? Cancelled { get; set; }

        public bool? Deleted { get; set; }

        public long? IdPayment { get; set; }

        public long? IdCoupon { get; set; }
        public long? IDShip { get; set; }
        public long? ID_Ship { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Coupon Coupon { get; set; }
        public virtual Shipping Shipping { get; set; }
        public virtual Shipping_unit Shipping_Unit { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
