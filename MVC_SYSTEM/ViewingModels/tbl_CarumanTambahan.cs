namespace MVC_SYSTEM.ViewingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_CarumanTambahan
    {
        [Key]
        public int fld_JenisCarumanID { get; set; }

        [Required]
        [StringLength(5)]
        public string fld_KodCaruman { get; set; }

        [Required]
        [StringLength(30)]
        public string fld_NamaCaruman { get; set; }

        public bool fld_Berjadual { get; set; }

        public int fld_CarumanOleh { get; set; }

        public bool fld_Default { get; set; }

        public int fld_NegaraID { get; set; }

        public int fld_SyarikatID { get; set; }

        public bool fld_Deleted { get; set; }
    }
}
