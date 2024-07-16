using MVC_SYSTEM.App_LocalResources;

namespace MVC_SYSTEM.ModelsEstate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tbl_Kerjahdr")]
    public partial class tbl_Kerjahdr
    {
        [Key]
        public Guid fld_UniqueID { get; set; }

        [StringLength(20)]
        public string fld_NoPkj { get; set; }

        [StringLength(50)]
        public string fld_Kum { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_Tarikh { get; set; }

        public int? fld_Hujan { get; set; }

        [Column("fld_Kdhdct")]
        public string fld_Kdhdct { get; set; }

        [MaxLength(1)]
        public char? fld_DataSource { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public int? fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }

        [StringLength(20)]
        public string fld_SAPChargeCode { get; set; }
    }
}
