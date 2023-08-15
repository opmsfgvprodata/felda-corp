namespace MVC_SYSTEM.ViewingModels
{
    using AuthModels;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;

    [Table("tbl_Wilayah")]
    public partial class tbl_Wilayah
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(50)]
        public string fld_WlyhName { get; set; }

        public int fld_SyarikatID { get; set; }

        public string fld_WlyhEmail { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}