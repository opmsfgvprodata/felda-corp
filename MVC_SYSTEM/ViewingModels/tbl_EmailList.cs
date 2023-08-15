namespace MVC_SYSTEM.ViewingModels
{
    using AuthModels;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;

    [Table("tblEmailList")]
    public partial class tblEmailList
    {
        [Key]
        public int fldID { get; set; }

        [StringLength(50)]
        public string fldEmail { get; set; }

        [StringLength(100)]
        public string fldName { get; set; }

        [StringLength(5)]
        public string fldCategory { get; set; }

        public bool? fldDeleted { get; set; }

        public int? fldNegaraID { get; set; }

        public int? fldSyarikatID { get; set; }
    }
}