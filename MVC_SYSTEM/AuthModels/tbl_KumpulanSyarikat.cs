namespace MVC_SYSTEM.AuthModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_KumpulanSyarikat
    {
        [Key]
        public int fld_KmplnSyrktID { get; set; }

        [StringLength(100)]
        public string fld_NamaKmplnSyrkt { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}
