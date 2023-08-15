namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_UpahMenuai
    {
        [Key]
        public int fld_ID { get; set; }

        public decimal? fld_HasilLower { get; set; }

        public decimal? fld_HasilUpper { get; set; }

        public decimal? fld_Kadar { get; set; }

        public int? fld_Catitan { get; set; }

        public bool? fld_Deleted { get; set; }

        [StringLength(50)]
        public string fld_Jadual { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }
    }
}
