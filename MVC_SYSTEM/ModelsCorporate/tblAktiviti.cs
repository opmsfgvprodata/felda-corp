namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblAktiviti")]
    public partial class tblAktiviti
    {
        [Key]
        public int fldID { get; set; }

        [StringLength(5)]
        public string fldLejar { get; set; }

        [StringLength(5)]
        public string fldKodAktvti { get; set; }

        [StringLength(100)]
        public string fldKtrangan { get; set; }

        public bool? fldDeleted { get; set; }
    }
}
