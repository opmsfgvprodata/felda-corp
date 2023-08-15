namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_ClientWilayah
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int fld_ID { get; set; }

        public int? fld_WilayahID { get; set; }

        [StringLength(50)]
        public string fld_WlyhName { get; set; }

        public int? fld_ClientID { get; set; }

        [StringLength(50)]
        public string fldClientName { get; set; }

        public int? fldUserCategory { get; set; }

        public int? fldCompanyID { get; set; }
    }
}
