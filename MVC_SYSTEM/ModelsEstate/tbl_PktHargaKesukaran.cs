using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace MVC_SYSTEM.ModelsEstate
{
    public partial class tbl_PktHargaKesukaran
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(10)]
        public string fld_PktUtama { get; set; }

        [StringLength(10)]
        public string fld_KodJenisHargaKesukaran { get; set; }

        [StringLength(50)]
        public string fld_JenisHargaKesukaran { get; set; }

        [StringLength(10)]
        public string fld_KodHargaKesukaran { get; set; }

        [StringLength(100)]
        public string fld_KeteranganHargaKesukaran { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_HargaKesukaran { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public DateTime? fld_CreatedDate { get; set; }

        [StringLength(50)]
        public string fld_CreatedBy { get; set; }

        public DateTime? fld_ModifiedDate { get; set; }

        [StringLength(50)]
        public string fld_ModifiedBy { get; set; }

        public bool? fld_Deleted { get; set; }

    }
}