namespace MVC_SYSTEM.SystemModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblDesignation")]
    public partial class tblDesignation
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblDesignation()
        {
            tblAgentDetails = new HashSet<tblAgentDetail>();
        }

        [Key]
        public int fldDesignationID { get; set; }

        [StringLength(10)]
        [Display(Name = "Designation")]
        public string fldDesignationShortName { get; set; }

        [StringLength(50)]
        public string fldDesignationName { get; set; }

        public bool? fldDeleted { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblAgentDetail> tblAgentDetails { get; set; }
    }
}
