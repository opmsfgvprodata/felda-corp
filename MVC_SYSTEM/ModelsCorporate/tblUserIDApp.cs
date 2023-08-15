namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblUserIDApp")]
    public partial class tblUserIDApp
    {
        [Key]
        public int fldID { get; set; }

        [StringLength(50)]
        public string fldUserid { get; set; }

        [StringLength(200)]
        public string fldNama { get; set; }

        [StringLength(20)]
        public string fldNoKP { get; set; }

        [StringLength(50)]
        public string fldKdLdg { get; set; }

        [StringLength(50)]
        public string fldNamaLdg { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fldTarikh { get; set; }

        [StringLength(50)]
        public string fldJawatan { get; set; }

        [StringLength(50)]
        public string fldPassword { get; set; }

        [StringLength(50)]
        public string fldStatus { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fldTrkdload { get; set; }

        public long? fldFileID { get; set; }

        public int? fldNegaraID { get; set; }

        public int? fldSyarikatID { get; set; }

        public int? fldWilayahID { get; set; }

        public int? fldLadangID { get; set; }

        public int? fldActionBy { get; set; }

        public DateTime? fldDateTimeApprove { get; set; }
    }
}
