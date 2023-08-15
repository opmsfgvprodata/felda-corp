namespace MVC_SYSTEM.ModelsEstate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Blok
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(10)]
        public string fld_KodPktutama { get; set; }

        [StringLength(10)]
        public string fld_KodPkt { get; set; }

        [StringLength(10)]
        public string fld_Blok { get; set; }

        [StringLength(50)]
        public string fld_NamaBlok { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LsBlok { get; set; }

        public DateTime? fld_CreateDate { get; set; }

        public DateTime? fld_EndDate { get; set; }

        [StringLength(5)]
        public string fld_Blok_Sblm { get; set; }

        [StringLength(50)]
        public string fld_NamaBlok_Sblm { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LsBlok_Sblm { get; set; }

        public DateTime? fld_CreateDate_Sblm { get; set; }

        public DateTime? fld_EndDate_Sblm { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LuasKawTnmanBlok { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LuasBerhasilBlok { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LuasBlmBerhasilBlok { get; set; }

        public int? fld_BilPokokBlok { get; set; }

        public int? fld_DirianPokokBlok { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LuasKawTiadaTanamanBlok { get; set; }

        [StringLength(10)]
        public string fld_KesukaranMenuaiBlok { get; set; }

        [StringLength(10)]
        public string fld_KesukaranMembajaBlok { get; set; }
    }
}
