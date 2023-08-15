namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblASCApprovalFileDetail")]
    public partial class tblASCApprovalFileDetail
    {
        [Key]
        public int fldID { get; set; }

        [StringLength(50)]
        public string fldFileName { get; set; }

        [StringLength(5)]
        public string fldCodeLadang { get; set; }

        public int? fldNegaraID { get; set; }

        public int? fldSyarikatID { get; set; }

        public int? fldWilayahID { get; set; }

        public int? fldLadangID { get; set; }

        public int? fldGenStatus { get; set; }

        public int? fldASCFileStatus { get; set; }

        public int? fldPurpose { get; set; }

        public DateTime? fldDateApplied { get; set; }
    }
}
