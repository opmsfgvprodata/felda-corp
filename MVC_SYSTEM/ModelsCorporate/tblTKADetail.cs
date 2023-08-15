namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblTKADetail")]
    public partial class tblTKADetail
    {
        [Key]
        public long fldID { get; set; }

        [StringLength(100)]
        public string fldWorkerName { get; set; }

        [StringLength(20)]
        public string fldPassNo { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fldPassExpDT { get; set; }

        [StringLength(1)]
        public string fldGender { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fldBOD { get; set; }

        [StringLength(20)]
        public string fldNationality { get; set; }

        [StringLength(20)]
        public string fldNatureWork { get; set; }

        [StringLength(100)]
        public string fldDpdntName { get; set; }

        [StringLength(10)]
        public string fldDpdntRelationship { get; set; }

        [StringLength(20)]
        public string fldDpdntTelNo { get; set; }

        [StringLength(150)]
        public string fldDpdntAdd { get; set; }

        public int? fldTKABatchID { get; set; }

        public int? fldStatusArrive { get; set; }
    }
}
