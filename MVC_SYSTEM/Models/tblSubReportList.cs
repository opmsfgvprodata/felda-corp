namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblSubReportList
    {
        [Key]
        public int fldSubReportListID { get; set; }

        [StringLength(50)]
        public string fldSubReportListName { get; set; }

        [StringLength(30)]
        public string fldSubReportListAction { get; set; }

        [StringLength(30)]
        public string fldSubReportListController { get; set; }

        [StringLength(20)]
        public string fldSubLevelAccess { get; set; }

        public int? fldSubWidthReport { get; set; }

        public int? fldSubHeightReport { get; set; }

        public int? fldMainReportID { get; set; }

        public bool? fldDeleted { get; set; }
    }
}
