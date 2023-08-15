namespace MVC_SYSTEM.AuthFTPModels
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class MVC_SYSTEM_Auth_FTP : DbContext
    {
        public MVC_SYSTEM_Auth_FTP()
            : base("name=MVC_SYSTEM_HQFTP_CONN")
        {
        }

        public virtual DbSet<tblUser> tblUsers { get; set; }
        public virtual DbSet<tblOptionConfigsWeb> tblOptionConfigsWebs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
