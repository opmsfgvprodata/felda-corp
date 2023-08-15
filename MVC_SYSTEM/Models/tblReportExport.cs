namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblReportExport")]
    public partial class tblReportExport
    {
        [Key]
        public int fldID { get; set; }

        [StringLength(100)]
        public string fldFileName { get; set; }

        [StringLength(200)]
        public string fldPath { get; set; }

        [StringLength(100)]
        public string fldReportName { get; set; }

        public int? fldUserID { get; set; }
    }
}
