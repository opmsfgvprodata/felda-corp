namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_ApplicationInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int fldID { get; set; }

        [StringLength(50)]
        public string fldFileName { get; set; }

        [StringLength(5)]
        public string fldCodeLadang { get; set; }

        public int? fldNegaraID { get; set; }

        public int? fldSyarikatID { get; set; }

        public int? fldLadangID { get; set; }

        public int? fldWilayahID { get; set; }

        public int? fldGenStatus { get; set; }

        public int? fldASCFileStatus { get; set; }

        public int? tblASCApprovalFileDetailfldPurpose { get; set; }

        [StringLength(100)]
        public string fldEmailNotiDesc { get; set; }

        [StringLength(30)]
        public string fldEmailNotiSource { get; set; }

        public int? fldEmailNotiStatus { get; set; }

        public DateTime? tblEmailNotiStatusfldDateTimeStamp { get; set; }

        public int? fldStatus { get; set; }

        [StringLength(2)]
        public string tblTaskRemainderfldPurpose { get; set; }

        public DateTime? tblTaskRemainderfldDateTimeStamp { get; set; }
    }
}
