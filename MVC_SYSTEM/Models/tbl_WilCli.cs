namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_WilCli
    {
        [Key]
        public int fld_ID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_ClientID { get; set; }
    }
}
