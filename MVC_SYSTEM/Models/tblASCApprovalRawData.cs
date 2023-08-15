namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblASCApprovalRawData")]
    public partial class tblASCApprovalRawData
    {
        [Key]
        public int fldID { get; set; }

        [StringLength(200)]
        public string fldRawData { get; set; }

        public int? fldUserID { get; set; }

        [StringLength(50)]
        public string fldASCFileName { get; set; }

        public int? fldNegaraID { get; set; }

        public int? fldSyarikatID { get; set; }

        public int? fldLadangID { get; set; }

        public int? fldWilayahID { get; set; }
    }
}
