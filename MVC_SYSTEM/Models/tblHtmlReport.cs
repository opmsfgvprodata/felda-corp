namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblHtmlReport")]
    public partial class tblHtmlReport
    {
        [Key]
        public long fldID { get; set; }

        public string fldHtlmCode { get; set; }

        [StringLength(50)]
        public string fldReportName { get; set; }

        [StringLength(50)]
        public string fldFileName { get; set; }
    }
}
