namespace MVC_SYSTEM.ViewingModels
{
    //using AuthModels;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;

    [Table("tbl_HargaGetahSMR")]
    public partial class tbl_HargaGetahSMR
    {
        [Key]
        public int fld_ID { get; set; }

        public decimal? fld_HargaGetah_Lower { get; set; }

        public decimal? fld_HargaGetah_Upper { get; set; }

        public decimal? fld_Upah_Tahun12 { get; set; }

        public decimal? fld_Upah_Tahun322 { get; set; }

        public bool? fld_Deleted { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_Nombor { get; set; }
    }
}