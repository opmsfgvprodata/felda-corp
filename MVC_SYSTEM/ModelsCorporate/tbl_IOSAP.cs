namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_IOSAP
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(15)]
        public string fld_IOcode { get; set; }

        [StringLength(5)]
        public string fld_PktCode { get; set; }

        [StringLength(5)]
        public string fld_SubPktCode { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LuasPkt { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LuasKawTnmn { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LuasKawBerhasil { get; set; }

        [StringLength(1)]
        public string fld_LdgIndicator { get; set; }

        [StringLength(5)]
        public string fld_LdgKod { get; set; }

        public int? fld_StatusUsed { get; set; }

        [StringLength(3)]
        public string fld_JnsLot { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }

        public DateTime? fld_DTCreated { get; set; }

        public DateTime? fld_DTModified { get; set; }

        //farahin tambah - cater integration sap
        public string fld_thnPembangunan { get; set; }
        public string fld_thnPembangunantanamsemula { get; set; }
        public string fld_busArea { get; set; }
        public string fld_IO2 { get; set; }
        public string fld_IO3 { get; set; }
        public string fld_IO4 { get; set; }
        public string fld_IO5 { get; set; }
        public string fld_IO6 { get; set; }
        public DateTime? fld_tkhTanamMulaBhsl { get; set; }
        public string fld_PktPembgnn { get; set; }
        public DateTime? fld_tkhTahapPmbgnn { get; set; }
        public DateTime? fld_tkhMulaTanam { get; set; }
        public string fld_jnsTanaman { get; set; }
        public string fld_kodBlok { get; set; }
        public string fld_indJnsKiraan { get; set; }
        public string fld_jnsBlok { get; set; }
        public string fld_jnsKawasan { get; set; }
        public int? fld_bilPenerokadlmBlok { get; set; }
        public int? fld_bilPeneroka { get; set; }
        public int? fld_bilPenerokaPkt { get; set; }
        public decimal? fld_luasKwsnBhasilFelda { get; set; }
        public decimal? fld_LuasKwsnBhasilPeneroka { get; set; }
        public decimal? fld_jumLuasLotLdgFelda { get; set; }
        public decimal? fld_jumLuasLotLdgPeneroka { get; set; }
        public int? fld_bilKwsnUtama { get; set; }
        public int? fld_bilKwsnRezab { get; set; }
        //farahin tambah - 3/8/2021
        public string fld_CreatedBy { get; set; }
        public string fld_ModifiedBy { get; set; }
        public string fld_CompanyCode { get; set; }
        public string fld_ZIOFLD { get; set; }
        public string fld_ZIOFPM { get; set; }

        //fatin added - 05/09/2023
        public string fld_WBS { get; set; }

    }
}
