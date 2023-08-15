namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_HariBekerjaLadang
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int fld_ID { get; set; }

        [StringLength(5)]
        public string fld_LdgCode { get; set; }

        [StringLength(50)]
        public string fld_LdgName { get; set; }

        public int? fld_WlyhID { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public bool? LadangDeleted { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid fld_HariBekerjaLadangID { get; set; }

        public int? fld_BilHariBekerja { get; set; }

        public int? fld_Month { get; set; }

        public int? fld_Year { get; set; }

        public bool? HariBekerjaLadangDeleted { get; set; }
    }
}
