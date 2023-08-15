namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_KelayakanInsentifLdg
    {
        [Key]
        public Guid fld_KelayakanInsentifLdgID { get; set; }

        [StringLength(50)]
        public string fld_KodInsentif { get; set; }

        [StringLength(50)]
        public string fld_Keterangan { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }

        public string fld_JenisInsentif { get; set; }
    }
}
