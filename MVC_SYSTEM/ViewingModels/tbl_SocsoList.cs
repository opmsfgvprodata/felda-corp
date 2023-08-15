namespace MVC_SYSTEM.ViewingModels
{
    //using AuthModels;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;

    [Table("tbl_Socso")]
    public partial class tbl_Socso
    {
        [Key]
        public int fld_ID { get; set; }

        public decimal? fld_KdrLower { get; set; }

        public decimal? fld_KdrUpper { get; set; }

        public decimal? fld_SocsoMjkn { get; set; }

        public decimal? fld_SocsoPkj { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_NegaraID { get; set; }

        [StringLength(5)]
        public string fld_KodCaruman { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}