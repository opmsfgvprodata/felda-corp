namespace MVC_SYSTEM.AuthModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblRoles")]
    public partial class tblRole
    {
        [Key]
        [Display(Name = "Role")]
        public int fldRoleID { get; set; }

        [StringLength(30)]
        [Display(Name = "Role")]
        public string fldRoleName { get; set; }

        public int ? fldRangeLevel { get; set; }

        public string fldDescriptionRole { get; set; }

        public bool? fldDeleted { get; set; }
    }
}
