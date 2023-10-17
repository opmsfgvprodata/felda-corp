using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsCorporate
{
    public partial class tbl_HargaKesukaran
    {
        [Key]
        public int fld_Id { get; set; }

        [StringLength(10)]
        public string fld_KodHargaKesukaran { get; set; }
        [StringLength(50)]
        public string fld_JenisHargaKesukaran { get; set; }
        [StringLength(100)]
        public string fld_Keterangan { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_HargaKesukaran { get; set; }
        [StringLength(10)]
        public string fld_JenisAktiviti { get; set; }

        public int? fld_NegaraId { get; set; }
        public int? fld_SyarikatId { get; set; }
        public int? fld_WilayahId { get; set; }
        public int? fld_LadangId { get; set; }
        public bool? fld_Deleted { get; set; }

        [StringLength(10)]
        public string fld_CreatedBy { get; set; }
        public DateTime? fld_CreatedDate { get; set; }
        [StringLength(10)]
        public string fld_ModifiedBy { get; set; }
        public DateTime? fld_ModifiedDate { get; set; }
    }
}