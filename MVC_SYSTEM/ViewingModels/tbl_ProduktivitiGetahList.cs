namespace MVC_SYSTEM.ViewingModels
{
    //using AuthModels;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;

    [Table("tbl_ProduktivitiGetah")]
    public partial class tbl_ProduktivitiGetah
    {
        [Key]
        public int fld_ID { get; set; }

        public int? fld_PrdktvtCuplumpLower { get; set; }

        public int? fld_PrdktvtCuplumpUpper { get; set; }

        public decimal? fld_KadarUpah { get; set; }

        public decimal? fld_InsentifKuantiti { get; set; }

        public int? fld_TahunToreh { get; set; }

        public bool? fld_Deleted { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }
        public int? fld_Nombor { get; set; }
    }
}