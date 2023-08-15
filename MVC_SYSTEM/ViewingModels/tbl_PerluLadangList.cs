namespace MVC_SYSTEM.ViewingModels
{
    using AuthModels;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;

    [Table("tbl_PerluLadang")]
    public partial class tbl_PerluLadang
    {
        [Key]
        public int fld_ID { get; set; }

        public int? fld_LadangID { get; set; }

        public string fld_LadangCode { get; set; }

        public string fld_LadangName { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_NegaraID { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Luas { get; set; }

        public int? fld_Perlu { get; set; }

        //public int? fld_PerluSemasa { get; set; }

        //public int? fld_Nisbah { get; set; }

        public int? fld_Tahun { get; set; }
    }
}