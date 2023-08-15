namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblClient
    {
        [Key]
        public int fldClientID { get; set; }

        [StringLength(50)]
        public string fldClientName { get; set; }

        public int? fldUserCategory { get; set; }

        public int? fldCompanyID { get; set; }

        public bool? fldDeleted { get; set; }
    }
}
