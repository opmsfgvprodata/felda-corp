using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class tbl_SAPLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid fld_Oid { get; set; }
        public string fld_type { get; set; }
        public string fld_id { get; set; }
        public string fld_number { get; set; }
        public string fld_logno { get; set; }
        public string fld_logmsgno { get; set; }
        public string fld_message { get; set; }
        public string fld_msg1 { get; set; }
        public string fld_msg2 { get; set; }
        public string fld_msg3 { get; set; }
        public string fld_msg4 { get; set; }
        public string fld_parameter { get; set; }
        public string fld_row { get; set; }
        public string fld_field { get; set; }
        public string fld_system { get; set; }
        public string fld_syarikatID { get; set; }
        public string fld_negaraID { get; set; }
        public DateTime fld_logDate { get; set; }
    }
}