namespace MVC_SYSTEM.SystemModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblAgentStatu
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblAgentStatu()
        {
            tblAgentDetails = new HashSet<tblAgentDetail>();
        }

        [Key]
        public int fldStatusID { get; set; }

        [StringLength(50)]
        [Display(Name = "Status")]
        public string fldStatusName { get; set; }

        public bool? fldDeleted { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblAgentDetail> tblAgentDetails { get; set; }
    }
}
