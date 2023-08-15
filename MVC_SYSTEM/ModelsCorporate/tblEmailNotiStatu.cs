namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblEmailNotiStatu
    {
        [Key]
        public int fldID { get; set; }

        [StringLength(100)]
        public string fldEmailNotiDesc { get; set; }

        [StringLength(30)]
        public string fldEmailNotiFlag { get; set; }

        [StringLength(30)]
        public string fldEmailNotiSource { get; set; }

        public int? fldEmailNotiStatus { get; set; }

        public int? fldNegaraID { get; set; }

        public int? fldSyarikatID { get; set; }

        public int? fldWilayahID { get; set; }

        public int? fldLadangID { get; set; }

        public DateTime? fldDateTimeStamp { get; set; }
    }
}
