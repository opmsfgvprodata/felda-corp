namespace MVC_SYSTEM.ViewingModelsOPMS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblUser
    {
        [Key]
        public int fldUserID { get; set; }

        [Required]
        [StringLength(50)]
        public string fldUserName { get; set; }

        [StringLength(200)]
        public string fldUserFullName { get; set; }

        [StringLength(100)]
        public string fldUserShortName { get; set; }

        [StringLength(50)]
        public string fldUserEmail { get; set; }

        [Required]
        [StringLength(100)]
        public string fldUserPassword { get; set; }

        public int? fldRoleID { get; set; }

        public int? fld_KmplnSyrktID { get; set; }

        public int? fldNegaraID { get; set; }

        public int? fldSyarikatID { get; set; }

        public int? fldWilayahID { get; set; }

        public int? fldLadangID { get; set; }

        public int? fldFirstTimeLogin { get; set; }

        public int? fldClientID { get; set; }

        [StringLength(10)]
        public string fldUserCategory { get; set; }

        public bool? fldDeleted { get; set; }

        public int? fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }

        public int? fld_ModifiedBy { get; set; }

        public DateTime? fld_ModifiedDT { get; set; }

        [StringLength(200)]
        public string fld_TokenLadangID { get; set; }

        [StringLength(20)]
        public string fldICNo { get; set; }

        [StringLength(30)]
        public string fldPosition { get; set; }
    }
}
