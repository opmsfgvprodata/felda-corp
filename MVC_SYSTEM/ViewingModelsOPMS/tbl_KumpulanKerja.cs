namespace MVC_SYSTEM.ViewingModelsOPMS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_KumpulanKerja
    {
        [Key]
        public int fld_KumpulanID { get; set; }

        [StringLength(50)]
        public string fld_KodKumpulan { get; set; }

        [StringLength(50)]
        public string fld_KodKerja { get; set; }

        [StringLength(50)]
        public string fld_Keterangan { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_deleted { get; set; }
    }
}
