namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblMaintenanceList")]
    public partial class tblMaintenanceList
    {
        [Key]
        public int fldID { get; set; }

        [StringLength(50)]
        public string fldName { get; set; }

        [StringLength(30)]
        public string fldAction { get; set; }

        [StringLength(30)]
        public string fldController { get; set; }

        [StringLength(20)]
        public string fldLevelAccess { get; set; }

        public bool? fldSubList { get; set; }

        public bool? fldDeleted { get; set; }
    }
}
