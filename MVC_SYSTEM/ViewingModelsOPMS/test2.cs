namespace MVC_SYSTEM.ViewingModelsOPMS
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class test2 : DbContext
    {
        public test2()
            : base("name=test2")
        {
        }

        public virtual DbSet<tbl_KumpulanKerja> tbl_KumpulanKerja { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
