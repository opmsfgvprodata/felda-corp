namespace MVC_SYSTEM.ModelsEstate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_PermitPassportDetail
    {
        [Key]
        [StringLength(10)]
        [Display(Name = "NO. PEKERJA")]
        public string fld_Nopkj { get; set; }

        [StringLength(40)]
        [Display(Name = "NAMA")]
        public string fld_Nama1 { get; set; }

        [Column(TypeName = "date")]
        [Display(Name = "TARIKH T.PERMIT")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? fld_T2prmt { get; set; }

        [StringLength(50)]
        public string fld_LdgName { get; set; }

        [StringLength(5)]
        public string fld_LdgCode { get; set; }

        [StringLength(50)]
        public string fld_WlyhName { get; set; }

        [StringLength(20)]
        [Display(Name = "NO. PERMIT")]
        public string fld_Prmtno { get; set; }

        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? fld_T1prmt { get; set; }

        [Display(Name = "BIL. BULAN TAMAT")]
        public int? fld_BilBlnTmtPrmnt { get; set; }

        [StringLength(12)]
        [Display(Name = "NO. PASSPORT")]
        public string fld_Nokp { get; set; }

        [Column(TypeName = "date")]
        [Display(Name = "TARIKH T.PASSPORT")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? fld_Ttsplp { get; set; }

        [Display(Name = "BIL. BULAN TAMAT")]
        public int? fld_BilBlnTmtPsprt { get; set; }

        public int? fld_WlyhID { get; set; }

        [StringLength(1)]
        public string fld_Kdaktf { get; set; }

        public int? fld_NegaraID { get; set; }

        public int?  fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        [StringLength(2)]
        public string fld_Kdrkyt { get; set; }
    }
}
