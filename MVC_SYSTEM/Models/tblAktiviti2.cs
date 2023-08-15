namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblAktiviti2
    {
        [Key]
        public int fldID { get; set; }

        [StringLength(5)]
        public string fldLejar { get; set; }

        [StringLength(5)]
        public string fldKodAktvti { get; set; }

        [StringLength(100)]
        public string fldKtrangan { get; set; }

        [StringLength(5)]
        public string fldMainGroup { get; set; }

        [StringLength(5)]
        public string fldGroup { get; set; }

        public bool? fldDeleted { get; set; }
    }
}
