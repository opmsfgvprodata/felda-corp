namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblPkjmastApp")]
    public partial class tblPkjmastApp
    {
        [Key]
        public long fldID { get; set; }

        [StringLength(20)]
        public string fldNoPkj { get; set; }

        [StringLength(50)]
        public string fldNama1 { get; set; }

        [StringLength(15)]
        public string fldNoKP { get; set; }

        [StringLength(2)]
        public string fldKdJnsPkj { get; set; }

        [StringLength(2)]
        public string fldKdRkyt { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fldTrshjw { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fldTtplks { get; set; }

        [StringLength(5)]
        public string fldKdLdg { get; set; }

        public int? fldStatus { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fldJumPjm { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fldTrkDload { get; set; }

        [StringLength(30)]
        public string fldSbbMsk { get; set; }

        [StringLength(40)]
        public string fldAlsnMsk { get; set; }

        public DateTime? fldDateTimeApprove { get; set; }

        public long? fldFileID { get; set; }

        public int? fldNegaraID { get; set; }

        public int? fldSyarikatID { get; set; }

        public int? fldWilayahID { get; set; }

        public int? fldLadangID { get; set; }

        public int? fldActionBy { get; set; }
        
        public string fldSbbTolak { get; set; }

        public int? fldWilayahAsal { get; set; }

        public int? fldLadangAsal { get; set; }

        public int? fldNegaraAsal { get; set; }

        public int? fldSyarikatAsal { get; set; }

        [StringLength(20)]
        public string fldNoPkjAsal { get; set; }
    }
}
