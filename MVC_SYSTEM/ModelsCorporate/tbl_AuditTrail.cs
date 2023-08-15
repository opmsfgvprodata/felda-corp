namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_AuditTrail
    {
        [Key]
        public long fld_ID { get; set; }

        public int? fld_Bln1 { get; set; }

        public int? fld_Bln2 { get; set; }

        public int? fld_Bln3 { get; set; }

        public int? fld_Bln4 { get; set; }

        public int? fld_Bln5 { get; set; }

        public int? fld_Bln6 { get; set; }

        public int? fld_Bln7 { get; set; }

        public int? fld_Bln8 { get; set; }

        public int? fld_Bln9 { get; set; }

        public int? fld_Bln10 { get; set; }

        public int? fld_Bln11 { get; set; }

        public int? fld_Bln12 { get; set; }

        public int? fld_Thn { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}
