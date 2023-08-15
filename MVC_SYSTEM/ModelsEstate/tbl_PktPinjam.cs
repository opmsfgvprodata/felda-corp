namespace MVC_SYSTEM.ModelsEstate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_PktPinjam
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(20)]
        public string fld_KodPkt { get; set; }

        [StringLength(50)]
        public string fld_NamaPkt { get; set; }

        public short? fld_JenisPkt { get; set; }

        [StringLength(30)]
        public string fld_SAPCode { get; set; }

        [StringLength(5)]
        public string fld_JnsLot { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public int? fld_DivisionID { get; set; }

        public int? fld_NegaraIDAsal { get; set; }

        public int? fld_SyarikatIDAsal { get; set; }

        public int? fld_WilayahIDAsal { get; set; }

        public int? fld_LadangIDAsal { get; set; }

        public int? fld_DivisionIDAsal { get; set; }

        public int? fld_OriginPktID { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_EndDT { get; set; }

        public int? fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }
    }
}
