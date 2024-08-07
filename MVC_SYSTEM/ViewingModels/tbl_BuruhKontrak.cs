namespace MVC_SYSTEM.ViewingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_BuruhKontrak
    {
        [Key]
        public int fld_ID { get; set; }

        public int? fld_LadangID { get; set; }

        [StringLength(5)]
        public string fld_LadangCode { get; set; }

        [StringLength(50)]
        public string fld_LadangName { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_NegaraID { get; set; }

        [StringLength(50)]
        public string fld_Designation { get; set; }

        public int? fld_JumlahBuruh { get; set; }
        public int? fld_Tahun { get; set; }

        public int? fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }

        public int? fld_ModifiedBy { get; set; }

        public DateTime? fld_ModifiedDT { get; set; }
    }
}
