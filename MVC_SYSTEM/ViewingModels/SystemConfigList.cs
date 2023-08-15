namespace MVC_SYSTEM.ViewingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;


    [Table("tblSystemConfigs")]
    public partial class tblSystemConfig
    {
        [Key]
        public int fldConfigID { get; set; }

        [Display(Name = "Content")]
        public string fldConfigValue { get; set; }

        [Display(Name = "Description")]
        public string fldConfigDesc { get; set; }

        [Display(Name = "Flag 1")]
        public string fldFlag1 { get; set; }

        [Display(Name = "Flag 2")]
        public string fldFlag2 { get; set; }

        [Display(Name = "Status")]
        public bool? fldDeleted { get; set; }
    }
}