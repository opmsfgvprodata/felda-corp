namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_TKADetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long fldID { get; set; }

        [StringLength(30)]
        public string fldNoBatch { get; set; }

        public int? fldCreatedBy { get; set; }

        public DateTime? fldDTCreated { get; set; }

        public int? fldModifiedBy { get; set; }

        public DateTime? fldDTModified { get; set; }

        public int? fldKmplnSyrktID { get; set; }

        public int? fldSyrktID { get; set; }

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
