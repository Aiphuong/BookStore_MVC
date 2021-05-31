namespace Doan.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Colletion")]
    public partial class Colletion
    {
        public long Id { get; set; }

        [StringLength(250)]
        public string Image { get; set; }

        [StringLength(250)]
        public string MoreImage { get; set; }

        [StringLength(150)]
        public string Title { get; set; }

        public string Content { get; set; }

        public long? IdColl { get; set; }
    }
}
