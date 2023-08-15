namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Pkjmast
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public Guid fld_UniqueID { get; set; }

        [Required]
        [StringLength(10)]
        public string fld_Nopkj { get; set; }

        [Required]
        [StringLength(12)]
        public string fld_Nokp { get; set; }

        [Required]
        [StringLength(40)]
        public string fld_Nama { get; set; }

        [Required]
        [StringLength(30)]
        public string fld_Almt1 { get; set; }

        [Required]
        [StringLength(30)]
        public string fld_Daerah { get; set; }

        [Required]
        [StringLength(2)]
        public string fld_Neg { get; set; }

        [Required]
        [StringLength(2)]
        public string fld_Negara { get; set; }

        [Required]
        [StringLength(5)]
        public string fld_Poskod { get; set; }

        [StringLength(10)]
        public string fld_Notel { get; set; }

        [StringLength(10)]
        public string fld_Nofax { get; set; }

        [Required]
        [StringLength(1)]
        public string fld_Kdjnt { get; set; }

        [Required]
        [StringLength(2)]
        public string fld_Kdbgsa { get; set; }

        [Required]
        [StringLength(1)]
        public string fld_Kdagma { get; set; }

        [Required]
        [Column(TypeName = "date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? fld_Trlhr { get; set; }

        [Required]
        [StringLength(2)]
        public string fld_Kdrkyt { get; set; }

        [Required]
        [StringLength(1)]
        public string fld_Kdkwn { get; set; }

        [Required]
        [StringLength(1)]
        public string fld_Kpenrka { get; set; }

        [StringLength(1)]
        public string fld_Kdaktf { get; set; }

        [Column(TypeName = "date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? fld_Trtakf { get; set; }

        [StringLength(60)]
        public string fld_Sbtakf { get; set; }

        [Required]
        [Column(TypeName = "date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? fld_Trmlkj { get; set; }

        [Required]
        [Column(TypeName = "date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? fld_Trshjw { get; set; }

        [Required]
        [StringLength(2)]
        public string fld_Ktgpkj { get; set; }

        [Required]
        [StringLength(2)]
        public string fld_Jenispekerja { get; set; }

        [StringLength(3)]
        public string fld_Kodbkl { get; set; }


        [StringLength(5)]
        public string fld_KodSocso { get; set; }

        [StringLength(12)]
        public string fld_Noperkeso { get; set; }

        [StringLength(5)]
        public string fld_KodKWSP { get; set; }

        [StringLength(12)]
        public string fld_Nokwsp { get; set; }

        [StringLength(50)]
        public string fld_Kdbank { get; set; }

        [StringLength(50)]
        public string fld_NoAkaun { get; set; }

        [StringLength(13)]
        public string fld_Visano { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_T1visa { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_T2visa { get; set; }

        [StringLength(13)]
        public string fld_Nogilr { get; set; }

        [StringLength(20)]
        public string fld_Prmtno { get; set; }

        [Column(TypeName = "date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? fld_T1prmt { get; set; }

        [Column(TypeName = "date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? fld_T2prmt { get; set; }

        [StringLength(20)]
        public string fld_Psptno { get; set; }

        [Column(TypeName = "date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? fld_T1pspt { get; set; }

        [Column(TypeName = "date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? fld_T2pspt { get; set; }

        [StringLength(3)]
        public string fld_Kdldg { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public DateTime? fld_DateApply { get; set; }

        [StringLength(50)]
        public string fld_AppliedBy { get; set; }

        public int? fld_StatusApproved { get; set; }

        [StringLength(50)]
        public string fld_ActionBy { get; set; }

        public DateTime? fld_ActionDate { get; set; }

        [StringLength(50)]
        public string fld_Batch { get; set; }

        [StringLength(10)]
        public string fld_IDpkj { get; set; }

        public int? fld_KumpulanID { get; set; }

        public string fld_StatusKwspSocso { get; set; }

        public string fld_StatusAkaun { get; set; }
    }
}
