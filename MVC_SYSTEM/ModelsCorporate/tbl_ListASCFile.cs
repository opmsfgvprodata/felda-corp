namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_ListASCFile
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(50)]
        public string fld_ASCFileName { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}
