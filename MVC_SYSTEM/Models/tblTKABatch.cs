namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblTKABatch")]
    public partial class tblTKABatch
    {
        [Key]
        public int fldID { get; set; }

        [StringLength(30)]
        public string fldNoBatch { get; set; }

        public int? fldCreatedBy { get; set; }

        public DateTime? fldDTCreated { get; set; }

        public int? fldModifiedBy { get; set; }

        public DateTime? fldDTModified { get; set; }

        public int? fldKmplnSyrktID { get; set; }

        public int? fldSyrktID { get; set; }
    }
}
