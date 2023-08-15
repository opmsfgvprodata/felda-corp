namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_UserIDDetail
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(50)]
        public string fldUserName { get; set; }

        [StringLength(200)]
        public string fldUserFullName { get; set; }

        [StringLength(100)]
        public string fldUserShortName { get; set; }

        [StringLength(50)]
        public string fldUserEmail { get; set; }

        [StringLength(40)]
        public string fldNama { get; set; }

        [StringLength(12)]
        public string fldNoKP { get; set; }

        [StringLength(5)]
        public string fldKdLdg { get; set; }

        [StringLength(40)]
        public string fldNamaLdg { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fldTarikh { get; set; }

        [StringLength(40)]
        public string fldJawatan { get; set; }

        [StringLength(12)]
        public string fldPassword { get; set; }

        [StringLength(1)]
        public string fldStatus { get; set; }

        public long? fldFileID { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fldTrkdload { get; set; }

        public int? fldNegaraID { get; set; }

        public int? fldSyarikatID { get; set; }

        public int? fldWilayahID { get; set; }

        public int? fldLadangID { get; set; }

        public int? fldActionBy { get; set; }

        public DateTime? fldDateTimeApprove { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int fldID { get; set; }

        [StringLength(50)]
        public string fldFileName { get; set; }
    }
}
