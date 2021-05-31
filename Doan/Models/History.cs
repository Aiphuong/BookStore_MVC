namespace Doan.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("History")]
    public partial class History
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }

        public long? IdUser { get; set; }

        public DateTime? Date { get; set; }

        public string Content { get; set; }

        public long? Customer_CodeCus { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual User User { get; set; }
    }
}
