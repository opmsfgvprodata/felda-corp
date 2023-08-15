namespace MVC_SYSTEM.ViewingModels
{
    //using AuthModels;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;

    [Table("tbl_HasilSawit")]
    public partial class tbl_HasilSawit
    {
        [Key]
        public int fld_ID { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_hsltan { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_jumlah { get; set; }

        [StringLength(10)]
        public string fld_kum { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_bulan { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_tahun { get; set; }

        [StringLength(10)]
        public string fld_pkt { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_luas_hek { get; set; }

        [StringLength(3)]
        public string fld_kdldg { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }
    }
}