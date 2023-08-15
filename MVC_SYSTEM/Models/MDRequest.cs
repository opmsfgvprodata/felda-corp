using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.Models
{
    public class MDRequest
    {
        [Key]
        public int ID { get; set; }
        public string dateStart { get; set; }

        public string dateEnd { get; set; }

        public string glStart { get; set; }

        public string glEnd { get; set; }

        public string ccStart { get; set; }

        public string ccEnd { get; set; }

        public string vdStart { get; set; }

        public string vdEnd { get; set; }

        public string cmStart { get; set; }

        public string cmEnd { get; set; }
    }
}