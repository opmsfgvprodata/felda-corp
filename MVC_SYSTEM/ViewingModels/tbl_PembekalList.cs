namespace MVC_SYSTEM.ViewingModels
{
    using AuthModels;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;

    [Table("tbl_Pembekal")]
    public partial class tbl_Pembekal
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(5)]
        public string fld_KodPbkl { get; set; }

        [StringLength(50)]
        public string fld_NamaPbkl { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_NegaraID { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}