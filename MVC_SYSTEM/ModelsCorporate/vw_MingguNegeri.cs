namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_MingguNegeri
    {
        [Key]
        [Column(Order = 0)]
        public int fld_MingguNegeriID { get; set; }

        public short? fld_JenisMinggu { get; set; }

        public int? fld_NegeriID { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public bool? fld_Deleted { get; set; }

        [StringLength(50)]
        public string fldOptConfValue { get; set; }

        [StringLength(50)]
        public string fldOptConfDesc { get; set; }

        [StringLength(50)]
        public string fldOptConfFlag1 { get; set; }

        [StringLength(50)]
        public string fldOptConfFlag2 { get; set; }
    }
}
