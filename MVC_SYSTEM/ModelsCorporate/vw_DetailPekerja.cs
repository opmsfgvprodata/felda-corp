namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_DetailPekerja
    {
        [StringLength(10)]
        public string fld_Nopkj { get; set; }

        [StringLength(40)]
        public string fld_Nama1 { get; set; }

        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? fld_T2prmt { get; set; }

        [StringLength(50)]
        public string fld_LdgName { get; set; }

        [StringLength(5)]
        public string fld_LdgCode { get; set; }

        [StringLength(50)]
        public string fld_WlyhName { get; set; }

        public int? fld_BilBlnTmtPrmnt { get; set; }

        [StringLength(12)]
        public string fld_Nokp { get; set; }

        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? fld_Ttsplp { get; set; }

        public int? fld_BilBlnTmtPsprt { get; set; }

        public int? fld_WlyhID { get; set; }

        [Key]
        [Column(Order = 0)]
        [StringLength(1)]
        public string fld_Kdaktf { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid fld_UniqueID { get; set; }

        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? fld_Trlhr { get; set; }

        [StringLength(2)]
        public string fld_Kdrkyt { get; set; }

        [StringLength(1)]
        public string fld_Kdjnt { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(2)]
        public string fld_Jenispekerja { get; set; }

        [StringLength(3)]
        public string fld_Kodbkl { get; set; }

        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? fld_Trmlkj { get; set; }

        [StringLength(2)]
        public string fld_Kodtakf { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(2)]
        public string fld_Ktgpkj { get; set; }
    }
}
