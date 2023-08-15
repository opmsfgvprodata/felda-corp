﻿
namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;
    using System.Web;
    public class vw_LadangDetails
    {
        [Key]
        public int fld_ID { get; set; }

        public int fld_ID2 { get; set; }

        [StringLength(50)]
        public string fld_WlyhName { get; set; }

        [StringLength(5)]
        public string fld_LdgCode { get; set; }

        [StringLength(50)]
        public string fld_LdgName { get; set; }

        public int? fld_WlyhID { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        [StringLength(100)]
        public string fld_LdgEmail { get; set; }

        public bool? fld_Deleted { get; set; }

        [StringLength(50)]
        public string fld_Pengurus { get; set; }

        [StringLength(50)]
        public string fld_Adress { get; set; }

        [StringLength(2)]
        public string fld_KodNegeri { get; set; }

        [StringLength(50)]
        public string fld_Tel { get; set; }

        [StringLength(50)]
        public string fld_Fax { get; set; }

        [StringLength(12)]
        public string fld_NoAcc { get; set; }

        [StringLength(10)]
        public string fld_NoGL { get; set; }

        [StringLength(10)]
        public string fld_NoCIT { get; set; }

        [StringLength(5)]
        public string fld_BankCode { get; set; }

        public DateTime? fld_BankCreatedDate { get; set; }

        [StringLength(50)]
        public string fld_BankCreatedBy { get; set; }

        public int? fld_BranchCode { get; set; }

        [StringLength(50)]
        public string fld_BranchName { get; set; }

        [StringLength(50)]
        public string fld_PengurusSblm { get; set; }

        [StringLength(50)]
        public string fld_AdressSblm { get; set; }

        [StringLength(50)]
        public string fld_TelSblm { get; set; }

        [StringLength(50)]
        public string fld_FaxSblm { get; set; }

        [StringLength(100)]
        public string fld_LdgEmailSblm { get; set; }

        public int? fld_OriginatorID { get; set; }

        [StringLength(50)]
        public string fld_OriginatorName { get; set; }

        [StringLength(50)]
        public string fld_CostCentre { get; set; }

        [StringLength(150)]
        public string fld_NamaPndkSyarikat { get; set; }
    }
}