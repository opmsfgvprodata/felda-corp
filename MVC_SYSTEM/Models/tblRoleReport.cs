namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblRoleReport")]
    public partial class tblRoleReport
    {
        [Key]
        public int fldID { get; set; }

        public int? fldRoleID { get; set; }

        public int? fldReportID { get; set; }
    }
}
