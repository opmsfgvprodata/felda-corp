namespace MVC_SYSTEM.ViewingModels
{
    //using AuthModels;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;

    [Table("tbl_UpahAktiviti")]
    public partial class tbl_UpahAktiviti
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(4)]
        public string fld_KodAktvt { get; set; }

        [StringLength(100)]
        public string fld_Desc { get; set; }

        [StringLength(10)]
        public string fld_Unit { get; set; }

        public decimal? fld_Harga { get; set; }

        [StringLength(2)]
        public string fld_KodJenisAktvt { get; set; }

        public decimal? fld_Produktvt { get; set; }

        public bool? fld_DisabledFlag { get; set; }

        [StringLength(1)]
        public string fld_KdhByr { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }

        [StringLength(1)]
        public string fld_Kategori { get; set; }

        public decimal? fld_BonusProduktvt { get; set; }

        public decimal? fld_BonusKehadiran { get; set; }

        public decimal? fld_BonusKualiti { get; set; }
    }
}