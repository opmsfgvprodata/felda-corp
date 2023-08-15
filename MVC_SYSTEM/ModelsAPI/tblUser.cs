namespace MVC_SYSTEM.ModelsAPI
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

        [Required]
        [StringLength(100)]
        public string fldUserPassword { get; set; }

        public int? fld_KmplnSyrktID { get; set; }

        public int? fldNegaraID { get; set; }

        public int? fldSyarikatID { get; set; }

        public int? fldWilayahID { get; set; }

        public int? fldLadangID { get; set; }

        public bool? fldDeleted { get; set; }
        
    }
}
