namespace MVC_SYSTEM.ViewingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_CutiPeruntukan
    {
        [Key]
        public int fld_CutiPeruntukanID { get; set; }

        [StringLength(50)]
        public string fld_KodCuti { get; set; }

        [StringLength(50)]
        public string fld_NoPkj { get; set; }

        public short? fld_Tahun { get; set; }

        public int? fld_JumlahCuti { get; set; }

        public int? fld_JumlahCutiDiambil { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}
