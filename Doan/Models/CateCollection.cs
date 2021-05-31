namespace Doan.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CateCollection")]
    public partial class CateCollection
    {
        public long Id { get; set; }

        [StringLength(250)]
        public string Name { get; set; }
    }
}
