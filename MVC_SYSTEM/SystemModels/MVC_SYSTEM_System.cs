namespace MVC_SYSTEM.SystemModels
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class MVC_SYSTEM_System : DbContext
    {
        public MVC_SYSTEM_System()
            : base("name=MVC_SYSTEM_CONN")
        {
        }

        public virtual DbSet<tblAgentDetail> tblAgentDetails { get; set; }
        public virtual DbSet<tblAgentStatu> tblAgentStatus { get; set; }
        public virtual DbSet<tblDesignation> tblDesignations { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tblAgentDetail>()
                .Property(e => e.fldICNo)
                .IsFixedLength();

            modelBuilder.Entity<tblAgentDetail>()
                .Property(e => e.fldStaffNo)
                .IsFixedLength();
        }
    }
}
