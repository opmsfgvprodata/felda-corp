namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web.Mvc;

    public partial class tbl_Messaging
    {
        [Key]
        public long fld_ID { get; set; }

        [Display(Name = "Tajuk Mesej")]
        [StringLength(300)]
        public string fld_Title { get; set; }
        
        [Column(TypeName = "text")]
        [AllowHtml]
        [Display(Name = "Isi Kandungan Mesej")]
        public string fld_Msg { get; set; }

        [Display(Name = "Ditulis Oleh")]
        public int? fld_CreatedBy { get; set; }

        [Display(Name = "Tarikh Dan Masa Ditulis")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? fld_CreatedDT { get; set; }

        [Display(Name = "DiUbah Oleh")]
        public int? fld_ModifiedBy { get; set; }

        [Display(Name = "Tarikh Dan Masa Diubah")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? fld_ModifiedDT { get; set; }

        public short? fld_Purpose { get; set; }

        [Display(Name = "Aktif/Tidak")]
        public bool? fld_Active { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }
    }
}
