namespace MVC_SYSTEM.ViewingModels
{
    using AuthModels;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;

    [Table("tblOptionGeneralConfigsWeb")]
    public partial class tblOptionGeneralConfigsWeb
    {
        [Key]
        public int fldID { get; set; }

        public string fldGenConTitle { get; set; }
        
        public string fldGenConDesc { get; set; }

        public bool? fldDeleted { get; set; }
        
    }
}