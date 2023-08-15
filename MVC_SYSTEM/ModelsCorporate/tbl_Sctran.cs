namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Sctran
    {
        [Key]
        public Guid fld_UniqueID { get; set; }

        [StringLength(1)]
        public string fld_KdCaj { get; set; }

        public int? fld_Tahun { get; set; }

        [StringLength(2)]
        public string fld_Bulan { get; set; }

        [StringLength(2)]
        public string fld_KdTran { get; set; }

        [StringLength(3)]
        public string fld_Lejar { get; set; }

        [StringLength(3)]
        public string fld_KdLdg { get; set; }

        [StringLength(5)]
        public string fld_Pkt { get; set; }

        [StringLength(4)]
        public string fld_Akt { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Amt { get; set; }

        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? fld_Tarikh { get; set; }

        [StringLength(10)]
        public string fld_NoDkmn { get; set; }

        [StringLength(10)]
        public string fld_NoCtr { get; set; }

        [StringLength(12)]
        public string fld_NoKP { get; set; }

        [StringLength(10)]
        public string fld_Nop23i { get; set; }

        [StringLength(10)]
        public string fld_NoAset { get; set; }

        [StringLength(6)]
        public string fld_KdStok { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Qty { get; set; }

        [StringLength(8)]
        public string fld_NoCek { get; set; }

        [StringLength(60)]
        public string fld_NoSkb { get; set; }

        public int? fld_Extsts { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_Trkdload { get; set; }

        public int? fld_Year { get; set; }

        public int? fld_Month { get; set; }

        [StringLength(50)]
        public string fld_ServicesName { get; set; }

        public int? fld_UploadBy { get; set; }

        public DateTime? fld_UploadDate { get; set; }

        [StringLength(5)]
        public string fld_UploadCdLdg { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public long? fld_ProcessID { get; set; }

        [StringLength(50)]
        public string fld_ASCFileName { get; set; }
    }
}
