namespace MVC_SYSTEM.AuthModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

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

        [StringLength(100)]
        public string fld_CostCentre { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}
