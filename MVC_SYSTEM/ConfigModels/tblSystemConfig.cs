namespace MVC_SYSTEM.ConfigModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblSystemConfigs")]
    public partial class tblSystemConfig
    {
        [Key]
        public int fldConfigID { get; set; }

        [Display(Name = "Content")]
        [StringLength(50)]
        public string fldConfigValue { get; set; }

        [Display(Name = "Description")]
        [StringLength(100)]
        public string fldConfigDesc { get; set; }

        [Display(Name = "Flag 1")]
        [StringLength(50)]
        public string fldFlag1 { get; set; }

        [Display(Name = "Flag 2")]
        [StringLength(50)]
        public string fldFlag2 { get; set; }

        [Display(Name = "Status")]
        public bool? fldDeleted { get; set; }
    }
}
