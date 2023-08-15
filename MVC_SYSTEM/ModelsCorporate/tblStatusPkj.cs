namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblStatusPkj")]
    public partial class tblStatusPkj
    {
        [Key]
        public long fldID { get; set; }

        [StringLength(10)]
        public string fldNoPkjLama { get; set; }

        [StringLength(50)]
        public string fldNama { get; set; }

        [StringLength(12)]
        public string fldNoKP { get; set; }

        public int? fldStatusAktif { get; set; }

        [StringLength(10)]
        public string fldNoPkjBaru { get; set; }

        [StringLength(50)]
        public string fldAppliedBy { get; set; }

        public DateTime? fldAppliedDate { get; set; }

        public int? fldStatusApprove { get; set; }

        [StringLength(50)]
        public string fldApprovedBy { get; set; }

        public DateTime? fldApprovedDate { get; set; }

        [StringLength(5)]
        public string fldCodeLadang { get; set; }

        public int? fldNegaraID { get; set; }

        public int? fldSyarikatID { get; set; }

        public int? fldWilayahID { get; set; }

        public int? fldLadangID { get; set; }
    }
}
