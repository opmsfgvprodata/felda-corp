namespace MVC_SYSTEM.ModelsCorporate
{

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    public partial class tbl_CostCentre
    {

        [Key]
        public int fld_ID { get; set; }

        [StringLength(15)]
        [DisplayName("Cost Center") ]
        public string fld_CostCentre { get; set; }

        [StringLength(3)]
        [DisplayName("Kategori")]
        public string fld_KodKtgri { get; set; }

        [DisplayName("Negara")]
        public int? fld_NegaraID { get; set; }

        [DisplayName("Syarikat")]
        public int? fld_SyarikatID { get; set; }

        [DisplayName("Wilayah")]
        public int? fld_WilayahID { get; set; }

        [DisplayName("Ladang")]
        public int? fld_LadangID { get; set; }

        [DisplayName("Delete")]
        public bool? fld_Deleted { get; set; }

        public DateTime fld_DTCreated { get; set; }
    }
}