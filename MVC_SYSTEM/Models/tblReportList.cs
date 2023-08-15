namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblReportList
    {
        [Key]
        public int fldReportListID { get; set; }

        [StringLength(100)]
        public string fldReportListName { get; set; }

        [StringLength(30)]
        public string fldReportListAction { get; set; }

        [StringLength(30)]
        public string fldReportListController { get; set; }

        [StringLength(20)]
        public string fldLevelAccess { get; set; }

        public int? fldWidthReport { get; set; }

        public int? fldHeightReport { get; set; }

        public bool? fldSubReport { get; set; }

        public bool? fldDeleted { get; set; }
    }
}
