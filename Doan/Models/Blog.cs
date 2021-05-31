namespace Doan.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Blog")]
    public partial class Blog
    {
        public long Id { get; set; }

        [StringLength(250)]
        public string Image { get; set; }

        [StringLength(250)]
        public string MoreImage { get; set; }

        public string Content { get; set; }

        [StringLength(100)]
        public string Title { get; set; }

        public string Content2 { get; set; }

        public string Content3 { get; set; }

        [StringLength(50)]
        public string CreateBy { get; set; }

        public DateTime? CreateDate { get; set; }

        [StringLength(10)]
        public string ChangesBy { get; set; }

        public DateTime? ChangeDate { get; set; }
    }
}
