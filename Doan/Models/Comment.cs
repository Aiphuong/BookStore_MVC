

namespace Doan.Models
{
   

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Comment")]
    public class Comment
    {
        [Key]
        public long Id { get; set; }
        public bool Deleted { get; set; }
        public string Content { get; set; }
        public long CodeCus { get; set; }
        public virtual Customer Customer { get; set; }

    }
}