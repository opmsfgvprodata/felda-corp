namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_PermohonanKewangan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long fld_ID { get; set; }

        public bool? fld_StsTtpUrsNiaga { get; set; }

        [StringLength(20)]
        public string fld_SkbNo { get; set; }

        [StringLength(20)]
        public string fld_NoAcc { get; set; }

        [StringLength(20)]
        public string fld_NoGL { get; set; }

        [StringLength(20)]
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
        //added by kamalia 24/11/21
        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahWorkerNet { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahCash { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahCheque { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahEwallet { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahCdmas { get; set; }

        //end
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

        public int? fld_Year { get; set; }

        public int? fld_Month { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        [StringLength(5)]
        public string fld_LdgCode { get; set; }

        [StringLength(50)]
        public string fld_LdgName { get; set; }

        public int? fld_WlyhID { get; set; }

        [StringLength(50)]
        public string fld_WlyhName { get; set; }
        //add by kamalia 24/12/2021
        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahKwsp { get; set; }
        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahSocso { get; set; }
        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahSip { get; set; }
        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahSbkp { get; set; }

        //Added by Shazana 3/7/2023
        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahM2U { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahM2E { get; set; }

        //Added by Shazana 1/8/2023
        [StringLength(50)]
        public string fld_CostCentre { get; set; }

        //Added by Shazana 30/10/2023
        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahEwalletTnG { get; set; }

    }
}
