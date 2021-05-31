namespace Doan.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("IVCDetail")]
    public partial class IVCDetail
    {
        public long Id { get; set; }

        public decimal? Price { get; set; }

        public int? Quantity { get; set; }

        public long? Ivc_Id { get; set; }

        public long? Product_IDProduct { get; set; }

        public virtual IVC IVC { get; set; }

        public virtual Product Product { get; set; }
    }
}
