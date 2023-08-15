namespace MVC_SYSTEM.ViewingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Kwsp
    {
        [Key]
        public int fld_ID { get; set; }

        public decimal? fld_KdrLower { get; set; }

        public decimal? fld_KdrUpper { get; set; }

        public decimal? fld_Mjkn { get; set; }

        public decimal? fld_Pkj { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_NegaraID { get; set; }

        [StringLength(5)]
        public string fld_KodCaruman { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}
