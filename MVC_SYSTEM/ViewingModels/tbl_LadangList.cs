namespace MVC_SYSTEM.ViewingModels
{
    using AuthModels;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;

    [Table("tbl_Ladang")]
    public partial class tbl_Ladang
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(5)]
        public string fld_LdgCode { get; set; }

        [StringLength(50)]
        public string fld_LdgName { get; set; }

        public int? fld_WlyhID { get; set; }

        [StringLength(100)]
        public string fld_LdgEmail { get; set; }

        public bool? fld_Deleted { get; set; }

        [StringLength(12)]
        public string fld_NoAcc { get; set; }

        [StringLength(10)]
        public string fld_NoGL { get; set; }

        [StringLength(10)]
        public string fld_NoCIT { get; set; }
    }
}