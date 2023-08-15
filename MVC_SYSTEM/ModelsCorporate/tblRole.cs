namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblRole
    {
        [Key]
        public int fldRoleID { get; set; }

        [StringLength(30)]
        public string fldRoleName { get; set; }

        public int? fldRangeLevel { get; set; }

        public bool? fldDeleted { get; set; }

        public string fldDescriptionRole { get; set; }
    }
}
