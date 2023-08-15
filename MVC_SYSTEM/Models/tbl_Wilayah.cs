namespace MVC_SYSTEM.Models
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

        public int? fld_NegaraID { get; set; }

        [StringLength(100)]
        public string fld_WlyhEmail { get; set; }

        public bool? fld_Deleted { get; set; }

        [StringLength(400)]
        public string fld_UrlRoute { get; set; }

    }
}
