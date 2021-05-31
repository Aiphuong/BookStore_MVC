namespace Doan.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Customer")]
    public partial class Customer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Customer()
        {
            C_Order = new HashSet<_Order>();
            Histories = new HashSet<History>();
        }

        [Key]
        public long CodeCus { get; set; }

        public long IDCus { get; set; }

        [StringLength(150)]
        public string Email_Cus { get; set; }

        [StringLength(200)]
        public string Address_Cus { get; set; }

        [StringLength(15)]
        public string Phone_Cus { get; set; }

        [StringLength(50)]
        public string Password { get; set; }

        [StringLength(50)]
        public string NewPassword { get; set; }

        [StringLength(50)]
        public string ConfirmPassword { get; set; }

        [StringLength(10)]
        public string FirstName { get; set; }

        [StringLength(10)]
        public string LastName { get; set; }

        public DateTime? CreatedDay { get; set; }

        public DateTime? ModifiedDay { get; set; }

        public int? scores { get; set; }

        public long? Idtype { get; set; }
        public virtual TypeCu TypeCu { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<_Order> C_Order { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<History> Histories { get; set; }
        public virtual ICollection<Comment> Products { get; set; }

    }
}
