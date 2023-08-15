namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblDataEntryList")]
    public partial class tblDataEntryList
    {
        [Key]
        public int fldDataEntryListID { get; set; }

        [StringLength(50)]
        public string fldDataEntryListName { get; set; }

        [StringLength(30)]
        public string fldDataEntryListAction { get; set; }

        [StringLength(30)]
        public string fldDataEntryListController { get; set; }

        [StringLength(20)]
        public string fldLevelAccess { get; set; }

        public bool? fldSubDataEntry { get; set; }

        public bool? fldDeleted { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }
    }
}
