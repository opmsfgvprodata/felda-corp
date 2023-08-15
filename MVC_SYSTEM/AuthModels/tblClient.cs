namespace MVC_SYSTEM.AuthModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblClients")]
    public partial class tblClient
    {
        [Key]
        [Display(Name = "Client")]
        public int fldClientID { get; set; }

        [StringLength(50)]
        [Display(Name = "Client")]
        public string fldClientName { get; set; }

        public int? fldUserCategory { get; set; }

        public int? fldCompanyID { get; set; }

        public bool? fldDeleted { get; set; }
    }
}
