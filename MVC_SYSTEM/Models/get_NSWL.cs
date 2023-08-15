namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class get_NSWL
    {
        public int NegaraID { get; set; }
        
        public int SyarikatID { get; set; }

        public int WilayahID { get; set; }

        public int LadangID { get; set; }
    }
}
