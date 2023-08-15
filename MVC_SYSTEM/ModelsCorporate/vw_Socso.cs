namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_Socso
    {
        public decimal? fld_KdrLower { get; set; }

        public decimal? fld_KdrUpper { get; set; }

        public decimal? fld_SocsoMjkn { get; set; }

        public decimal? fld_SocsoPkj { get; set; }

        public decimal? fld_MjknShj { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_NegaraID { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int fld_ID { get; set; }

        public bool? fld_Deleted { get; set; }

    }
}
