namespace MVC_SYSTEM.ViewingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_MapGL
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(10)]
        public string fld_KodAktvt { get; set; }

        [StringLength(10)]
        public string fld_KodGL { get; set; }

        [StringLength(10)]
        public string fld_Paysheet { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}
