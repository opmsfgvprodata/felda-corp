namespace MVC_SYSTEM.SystemModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblAgentDetail
    {
        [Key]
        public int fldAgentID { get; set; }

        [StringLength(150)]
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required.")]
        public string fldAgentFullName { get; set; }

        [StringLength(12)]
        [Display(Name = "IC No")]
        [Required(ErrorMessage = "IC No is required.")]
        public string fldICNo { get; set; }

        [StringLength(10)]
        [Display(Name = "Staff No")]
        [Required(ErrorMessage = "Staff No is required.")]
        public string fldStaffNo { get; set; }

        public DateTime? fldDatetimeCreated { get; set; }

        public int? fldCreateBy { get; set; }

        public DateTime? fldDatetimeModified { get; set; }

        public int? fldModifiedBy { get; set; }

        public int? fldDesignationID { get; set; }

        [Display(Name = "Status")]
        public int? fldStatusID { get; set; }

        public bool? fldDeleted { get; set; }

        public virtual tblAgentStatu tblAgentStatu { get; set; }

        public virtual tblDesignation tblDesignation { get; set; }
    }
}
