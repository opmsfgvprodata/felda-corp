namespace MVC_SYSTEM.ViewingModels
{
    using AuthModels;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;

    [Table("tblUsers")]
    public partial class tblUser
    {
        [Key]
        public int fldUserID { get; set; }

        public string fldUserName { get; set; }
        
        public string fldUserFullName { get; set; }
        
        public int? fld_KmplnSyrktID { get; set; }
        
        public int? fldNegaraID { get; set; }
        
        public int? fldSyarikatID { get; set; }
        
        public int? fldWilayahID { get; set; }
        
        public int? fldLadangID { get; set; }

        public int? fldRoleID { get; set; }

        public bool? fldDeleted { get; set; }
        
    }
}