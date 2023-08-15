namespace MVC_SYSTEM.ModelsAPI
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class MVC_SYSTEM_Models_API : DbContext
    {
        public MVC_SYSTEM_Models_API()
            : base("name=MVC_SYSTEM_CONN")
        {
        }

        public virtual DbSet<tblUser> tblUsers { get; set; }
     
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
