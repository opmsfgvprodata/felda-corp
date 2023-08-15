namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_NSWL
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int fld_NegaraID { get; set; }

        [StringLength(100)]
        public string fld_NamaNegara { get; set; }

        public bool? fld_Deleted_N { get; set; }

        public int? fld_SyarikatID { get; set; }

        [StringLength(100)]
        public string fld_NamaSyarikat { get; set; }

        public bool? fld_Deleted_S { get; set; }

        public int? fld_WilayahID { get; set; }

        [StringLength(50)]
        public string fld_NamaWilayah { get; set; }

        public bool? fld_Deleted_W { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int fld_LadangID { get; set; }

        [StringLength(5)]
        public string fld_LdgCode { get; set; }

        [StringLength(50)]
        public string fld_NamaLadang { get; set; }

        public bool? fld_Deleted_L { get; set; }

        [StringLength(100)]
        public string fld_WlyhEmail { get; set; }

        [StringLength(100)]
        public string fld_LdgEmail { get; set; }

        [StringLength(100)]
        public string fld_SyarikatEmail { get; set; }

        public int? fld_KmplnSyrktID { get; set; }

        [StringLength(10)]
        public string fld_FrstNmeUsrNme { get; set; }

        [StringLength(10)]
        public string fld_RequestCode { get; set; }

        public string fld_KodNegeri { get; set; }
        public string fld_CostCentre { get; set; }

        //Added by Shazana 3/4/2023
        public string fld_BranchName { get; set; }
        public string fld_NoAcc { get; set; }

    }
}
