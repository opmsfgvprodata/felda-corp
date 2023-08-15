namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblOptionGeneralConfigsWeb")]
    public partial class tblOptionGeneralConfigsWeb
    {
        [Key]
        public int fldID { get; set; }

        [StringLength(50)]
        public string fldGenConTitle { get; set; }

        [StringLength(100)]
        public string fldGenConDesc { get; set; }

        [StringLength(50)]
        public string fldGenConFlagToFilter1 { get; set; }

        [StringLength(50)]
        public string fldGenConFlagToFilter2 { get; set; }

        [StringLength(50)]
        public string fldGenConFlagToFilter3 { get; set; }

        [StringLength(30)]
        public string fldGenConAction { get; set; }

        [StringLength(30)]
        public string fldGenConController { get; set; }

        public bool? fldRelatedToOptConWeb { get; set; }

        [StringLength(20)]
        public string fldRole { get; set; }

        public bool? fldDeleted { get; set; }
    }
}
