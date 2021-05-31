namespace Doan.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Product")]
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            IVCDetails = new HashSet<IVCDetail>();
            OrderDetails = new HashSet<OrderDetail>();
        }

        [Key]
        public long IDProduct { get; set; }

        [StringLength(250)]
        public string ProductName { get; set; }

        public string MetaTitle { get; set; }

        public string Description { get; set; }

        [StringLength(250)]
        public string Image { get; set; }

        [StringLength(250)]
        public string MoreImage1 { get; set; }

        [StringLength(250)]
        public string MoreImage2 { get; set; }

        [StringLength(250)]
        public string MoreImage3 { get; set; }

        public decimal? Price { get; set; }

        public decimal? Entryprice { get; set; }

        public decimal? PromotionPrice { get; set; }

        public bool? IncludedVAT { get; set; }

        public int? Quantity { get; set; }

        public long? CategoryID { get; set; }

        public long? SupplierID { get; set; }

        [StringLength(500)]
        public string Detail { get; set; }

        public DateTime? CreatedDate { get; set; }

        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        [StringLength(50)]
        public string ModifiedBy { get; set; }

        [StringLength(25)]
        public string Status { get; set; }

        public virtual Category Category { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IVCDetail> IVCDetails { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        public virtual Supplier Supplier { get; set; }
    }
}
