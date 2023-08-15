namespace MVC_SYSTEM.ModelsEstate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_PktUtama
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(10)]
        public string fld_PktUtama { get; set; }

        [StringLength(50)]
        public string fld_NamaPktUtama { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LsPktUtama { get; set; }

        [StringLength(1)]
        public string fld_JnsTnmn { get; set; }

        [StringLength(1)]
        public string fld_StatusTnmn { get; set; }

        public DateTime? fld_CreateDate { get; set; }

        public DateTime? fld_EndDate { get; set; }

        public int? fld_Level { get; set; }

        [StringLength(50)]
        public string fld_RefKey { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LsPktUtama_Sblm { get; set; }

        [StringLength(1)]
        public string fld_JnsTnmn_Sblm { get; set; }

        [StringLength(1)]
        public string fld_StatusTnmn_Sblm { get; set; }

        public DateTime? fld_CreateDate_Sblm { get; set; }

        public DateTime? fld_EndDate_Sblm { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LuasKawTnman { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LuasBerhasil { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LuasBlmBerhasil { get; set; }

        public int? fld_BilPokok { get; set; }

        public int? fld_DirianPokok { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LuasKawTiadaTanaman { get; set; }

        [StringLength(50)]
        public string fld_IOcode { get; set; }

        [StringLength(1)]
        public string fld_IOtype { get; set; }

        [StringLength(50)]
        public string fld_IOref { get; set; }

        [StringLength(10)]
        public string fld_KesukaranMenuaiPktUtama { get; set; }

        [StringLength(10)]
        public string fld_KesukaranMembajaPktUtama { get; set; }

        [StringLength(5)]
        public string fld_JnsLot { get; set; }

        [StringLength(10)]
        public string fld_KesukaranMemunggahPktUtama { get; set; }

        //[StringLength(10)]
        //public string fld_SAPType { get; set; }


    }
}
