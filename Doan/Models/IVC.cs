namespace Doan.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("IVC")]
    public partial class IVC
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public IVC()
        {
            IVCDetails = new HashSet<IVCDetail>();
        }

        public long Id { get; set; }

        public DateTime? Date { get; set; }

        [StringLength(250)]
        public string Status { get; set; }

        public long? Supplier_SupplierID { get; set; }

        public virtual Supplier Supplier { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IVCDetail> IVCDetails { get; set; }
    }
}
