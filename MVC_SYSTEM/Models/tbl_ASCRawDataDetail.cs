namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_ASCRawDataDetail
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(50)]
        public string fld_ASCFileName { get; set; }

        public int? fld_CountData { get; set; }

        [StringLength(5)]
        public string fld_LdgCode { get; set; }
    }
}
