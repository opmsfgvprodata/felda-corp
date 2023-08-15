namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblTaskRemainder")]
    public partial class tblTaskRemainder
    {
        [Key]
        public long fldID { get; set; }

        [StringLength(50)]
        public string fldFileName { get; set; }

        [StringLength(5)]
        public string fldCodeLadang { get; set; }

        public int? fldNegaraID { get; set; }

        public int? fldSyarikatID { get; set; }

        public int? fldWilayahID { get; set; }

        public int? fldLadangID { get; set; }

        public int? fldStatus { get; set; }

        [StringLength(2)]
        public string fldPurpose { get; set; }

        public DateTime? fldDateTimeStamp { get; set; }
    }
}
