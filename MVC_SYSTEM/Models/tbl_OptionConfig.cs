namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_OptionConfig
    {
        [Key]
        public int fldOptConfID { get; set; }

        [Required]
        [StringLength(500)]
        public string fldOptConfValue { get; set; }

        [Required]
        [StringLength(50)]
        public string fldOptConfDesc { get; set; }

        [Required]
        [StringLength(50)]
        public string fldOptConfFlag1 { get; set; }

        [StringLength(50)]
        public string fldOptConfFlag2 { get; set; }

        [StringLength(50)]
        public string fldOptConfFlag3 { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public bool? fldDeleted { get; set; }
    }
}
