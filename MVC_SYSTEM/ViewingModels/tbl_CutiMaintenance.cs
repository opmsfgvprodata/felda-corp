namespace MVC_SYSTEM.ViewingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tbl_CutiMaintenance")]
    public partial class tbl_CutiMaintenance
    {
        [Key]
        public int fld_CutiMaintenanceID { get; set; }

        [StringLength(50)]
        public string fld_Keterangan { get; set; }

        [StringLength(30)]
        public string fld_JenisCuti { get; set; }

        public int? fld_LowerLimit { get; set; }

        public int? fld_UpperLimit { get; set; }

        public int? fld_PeruntukkanCuti { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_NegaraID { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}
