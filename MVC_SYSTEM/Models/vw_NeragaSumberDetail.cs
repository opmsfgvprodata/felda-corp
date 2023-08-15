namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_NeragaSumberDetail
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int fldID { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string fldUserName { get; set; }

        [StringLength(200)]
        public string fldUserFullName { get; set; }

        [StringLength(100)]
        public string fldUserShortName { get; set; }

        [StringLength(50)]
        public string fldUserEmail { get; set; }

        public int? fldRoleID { get; set; }

        public bool? fldDeleted { get; set; }

        [StringLength(10)]
        public string fldUserCategory { get; set; }

        [StringLength(100)]
        public string fld_NamaKmplnSyrkt { get; set; }

        public int? fldUserID { get; set; }

        public int? fldKmplnSyrktID { get; set; }
    }
}
