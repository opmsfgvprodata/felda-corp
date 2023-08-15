namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_ServicesList
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(50)]
        public string fld_ServicesName { get; set; }

        public int? fld_ClientID { get; set; }

        [StringLength(50)]
        public string fld_SevicesActivity { get; set; }

        public bool? fld_Deleted { get; set; }

        [StringLength(10)]
        public string fld_Category { get; set; }

        public int? fldNegaraID { get; set; }

        public int? fldSyarikatID { get; set; }

        public int? fldWilayahID { get; set; }

        public int? fldLadangID { get; set; }
    }
}
