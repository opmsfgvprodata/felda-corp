namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblNgrSmbrSyrkt")]
    public partial class tblNgrSmbrSyrkt
    {
        [Key]
        public int fldID { get; set; }

        public int? fldUserID { get; set; }

        public int? fldKmplnSyrktID { get; set; }
    }
}
