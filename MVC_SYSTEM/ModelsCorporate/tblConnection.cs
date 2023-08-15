namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblConnection")]
    public partial class tblConnection
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string ConnectionName { get; set; }

        [StringLength(50)]
        public string DataSource { get; set; }

        [StringLength(50)]
        public string DataSourceInternal { get; set; }

        [StringLength(50)]
        public string InitialCatalog { get; set; }

        [StringLength(50)]
        public string userID { get; set; }

        [StringLength(50)]
        public string Password { get; set; }

        public int? wilayahID { get; set; }

        public int? syarikatID { get; set; }

        public int? negaraID { get; set; }

        public bool? deleted { get; set; }
    }
}
