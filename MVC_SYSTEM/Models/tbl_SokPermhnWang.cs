namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_SokPermhnWang
    {
        [Key]
        public long fld_ID { get; set; }

        [StringLength(5)]
        public string fld_LdgCode { get; set; }

        [StringLength(50)]
        public string fld_LdgName { get; set; }

        [StringLength(12)]
        public string fld_NoAcc { get; set; }

        [StringLength(10)]
        public string fld_NoGL { get; set; }

        [StringLength(10)]
        public string fld_NoCIT { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahPermohonan { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahPDP { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahTT { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahCIT { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahManual { get; set; }

        public int? fld_SemakWil_Status { get; set; }

        public int? fld_SemakWil_By { get; set; }

        public DateTime? fld_SemakWil_DT { get; set; }

        public int? fld_TolakWil_Status { get; set; }

        public int? fld_TolakWil_By { get; set; }

        public DateTime? fld_TolakWil_DT { get; set; }

        public int? fld_SokongWilGM_Status { get; set; }

        public int? fld_SokongWilGM_By { get; set; }

        public DateTime? fld_SokongWilGM_DT { get; set; }

        public int? fld_TolakWilGM_Status { get; set; }

        public int? fld_TolakWilGM_By { get; set; }

        public DateTime? fld_TolakWilGM_DT { get; set; }

        public int? fld_TerimaHQ_Status { get; set; }

        public int? fld_TerimaHQ_By { get; set; }

        public DateTime? fld_TerimaHQ_DT { get; set; }

        public int? fld_TolakHQ_Status { get; set; }

        public int? fld_TolakHQ_By { get; set; }

        public DateTime? fld_TolakHQ_DT { get; set; }

        [StringLength(200)]
        public string fld_Remark { get; set; }

        public int? fld_ReuploadReject { get; set; }

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
    }
}
