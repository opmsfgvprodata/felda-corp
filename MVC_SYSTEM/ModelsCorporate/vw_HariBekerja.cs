namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_HariBekerja
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short fld_HariBekerjaID { get; set; }

        public short? fld_Month { get; set; }

        public short? fld_Year { get; set; }

        public short? fld_BilanganHariBekerja { get; set; }

        public short? fld_JenisMinggu { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_NegeriID { get; set; }

        public int? fld_SyarikatID { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string fldOptConfDesc { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}
