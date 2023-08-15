namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Lejar
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(5)]
        public string fld_KodLejar { get; set; }

        [StringLength(50)]
        public string fld_Desc { get; set; }

        [StringLength(1)]
        public string fld_KodCaj { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}
