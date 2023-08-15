using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsCustom
{
    public class UserIDApplication
    {
        [Key]
        public int ID { get; set; }

        public string batchno { get; set; }

        public string kdldng { get; set; }

        public string kdprmhnan { get; set; }

        public string Username { get; set; }

        public string Name { get; set; }

        public string IC { get; set; }

        public string PositionList { get; set; }

        public string shortname { get; set; }

        public string email { get; set; }

    }
}