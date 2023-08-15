namespace MVC_SYSTEM.ModelsEstate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Kepuasan
    {
        [Key]
        public int fld_ID { get; set; }

        public int? fld_UserID { get; set; }

        public int? fld_Kepuasan { get; set; }

        public string fld_Catatan { get; set; }

        public DateTime? fld_Tarikh { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }
    }
}
