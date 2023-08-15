using System.ComponentModel;
using System.Web.Mvc;
using MVC_SYSTEM.App_LocalResources;

namespace MVC_SYSTEM.ViewingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tbl_KategoriAktiviti")]
    public partial class tbl_KategoriAktiviti
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(2)]
        public string fld_KodKategori { get; set; }

        [StringLength(50)]
        public string fld_Kategori { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public bool? fld_Deleted { get; set; }

        [StringLength(2)]
        public string fld_PrefixPkt { get; set; }
    }

    [Table("tbl_KategoriAktiviti")]
    public partial class tbl_KategoriAktivitiCreate
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(2)]
        public string fld_KodKategori { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(50)]
        public string fld_Kategori { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public bool? fld_Deleted { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(2)]
        public string fld_PrefixPkt { get; set; }
    }

    [Table("tbl_KategoriAktiviti")]
    public partial class tbl_KategoriAktivitiView
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(2)]
        public string fld_KodKategori { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(50)]
        public string fld_Kategori { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public bool? fld_Deleted { get; set; }

        [StringLength(2)]
        public string fld_PrefixPkt { get; set; }
    }
}
