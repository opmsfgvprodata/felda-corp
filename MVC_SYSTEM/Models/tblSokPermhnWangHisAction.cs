namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblSokPermhnWangHisAction")]
    public partial class tblSokPermhnWangHisAction
    {
        [Key]
        public long fldID { get; set; }

        [StringLength(50)]
        public string fldHisDesc { get; set; }

        public int? fldHisUserID { get; set; }

        public DateTime? fldHisDT { get; set; }

        public long? fldHisSPWID { get; set; }

        public int? fldHisAppLevel { get; set; }
    }
}
