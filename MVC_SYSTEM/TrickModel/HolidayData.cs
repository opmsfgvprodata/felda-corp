namespace MVC_SYSTEM.TrickModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class HolidayData
    {
        [Key]
        public int ID { get; set; }

        public DateTime Date { get; set; }

        public string Desc { get; set; }

        public int Region { get; set; }

        public short Year { get; set; }
    }
}
