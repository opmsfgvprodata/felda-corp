namespace MVC_SYSTEM.AuthModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Wilayah
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(50)]
        public string fld_WlyhName { get; set; }

        public int? fld_SyarikatID { get; set; }

        [StringLength(100)]
        public string fld_WlyhEmail { get; set; }

        [StringLength(400)]
        public string fld_UrlRoute { get; set; }

        [StringLength(10)]
        public string fld_ApprovalZone { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}
