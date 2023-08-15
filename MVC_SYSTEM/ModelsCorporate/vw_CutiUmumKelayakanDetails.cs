namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_CutiUmumKelayakanDetails
    {
        [Key]
        public Guid fld_CutiUmumKelayakanID { get; set; }

        public int? fld_CutiMasterID { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }

        [StringLength(100)]
        public string fld_KeteranganCuti { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_TarikhCuti { get; set; }

        public short? fld_Tahun { get; set; }

        [StringLength(5)]
        public string fld_CreatedInd { get; set; }
    }

    public partial class vw_CutiUmumKelayakanDetailsGenerate
    {
        [Key]
        public Guid fld_CutiUmumKelayakanID { get; set; }

        public short? fld_Tahun { get; set; }
    }
}
