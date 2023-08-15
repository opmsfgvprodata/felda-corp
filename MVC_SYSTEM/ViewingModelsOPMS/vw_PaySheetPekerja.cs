namespace MVC_SYSTEM.ViewingModelsOPMS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_PaySheetPekerja
    {
        [Key]
        [Column(Order = 0)]
        public Guid fld_ID { get; set; }

        [StringLength(10)]
        public string fld_Nopkj { get; set; }

        [StringLength(12)]
        public string fld_Nokp { get; set; }

        [StringLength(40)]
        public string fld_Nama { get; set; }

        [StringLength(1)]
        public string fld_Kdjnt { get; set; }

        [StringLength(1)]
        public string fld_Kdaktf { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(2)]
        public string fld_Ktgpkj { get; set; }

        [StringLength(12)]
        public string fld_Noperkeso { get; set; }

        [StringLength(12)]
        public string fld_Nokwsp { get; set; }

        public int? fld_KumpulanID { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_ByrKerja { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_KWSPPkj { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_KWSPMjk { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_SocsoPkj { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_SocsoMjk { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LainInsentif { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_OT { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_ByrCuti { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_BonusHarian { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LainPotongan { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_TargetProd { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_CapaiProd { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_ProdInsentif { get; set; }

        public short? fld_KuaTarget { get; set; }

        public short? fld_KuaCapai { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_KuaInsentif { get; set; }

        public int? fld_HdrTarget { get; set; }

        public int? fld_HdrCapai { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_HdrInsentif { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_AIPS { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_GajiKasar { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_GajiBersih { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_PurataGaji { get; set; }

        public int? fld_Year { get; set; }

        public int? fld_Month { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public int? fld_CreatedBy { get; set; }

        public DateTime? fld_DTCreated { get; set; }

        [StringLength(50)]
        public string fld_NoAkaun { get; set; }
        //added by kamalia 24/11/21
        [StringLength(15)]
        public string fld_PaymentMode { get; set; }

        [StringLength(4)]
        public string fld_Last4Pan { get; set; }

        [StringLength(15)]
        public string fld_Notel { get; set; }
    }
}
