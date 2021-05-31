namespace Doan.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TypeCu
    {
        [Key]
        public long IdType { get; set; }

        [StringLength(100)]
        public string NameType { get; set; }

        public int scores { get; set; }

        public virtual ICollection<Customer> Customers { get; set; }
    }
}
