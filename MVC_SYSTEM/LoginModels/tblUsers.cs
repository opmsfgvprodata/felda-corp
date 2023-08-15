using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MVC_SYSTEM.App_LocalResources;
namespace MVC_SYSTEM.LoginModels
{
    [Table("tblUsers")]
    public partial class tblUser
    {
        [Key]
        public int fldUserID { get; set; }

        //[Required(ErrorMessageResourceType = typeof(GlobalResLogin), ErrorMessageResourceName = "UsernameRequiredMsg")]
        [StringLength(50)]
        [Display(Name = "Username")]
        public string fldUserName { get; set; }

        //[Required(ErrorMessageResourceType = typeof(GlobalResLogin), ErrorMessageResourceName = "PasswordRequiredMsg")]
        [DataType(DataType.Password)]
        [StringLength(100)]
        [Display(Name = "Password")]
        public string fldUserPassword { get; set; }

        public string fldUserShortName { get; set; }

        public int? fldRoleID { get; set; }
        
        public int? fld_KmplnSyrktID { get; set; }
        
        public int? fldNegaraID { get; set; }
        
        public int? fldSyarikatID { get; set; }
        
        public int? fldWilayahID { get; set; }
        
        public int? fldLadangID { get; set; }

        [StringLength(200)]
        public string fld_TokenLadangID { get; set; }

        public bool? fldDeleted { get; set; }
        
    }
}