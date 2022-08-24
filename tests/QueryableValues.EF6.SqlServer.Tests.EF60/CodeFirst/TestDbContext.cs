using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazarTech.QueryableValues.EF6.SqlServer.Tests.CodeFirst
{
    public class TestDbContext : DbContext
    {
        public TestDbContext() : base("data source=.\\SQLEXPRESS;initial catalog=Test;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Conventions.Remove(new System.Data.Entity.ModelConfiguration.Conventions.PluralizingEntitySetNameConvention());
            //modelBuilder.Conventions.Remove(new System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention());

            modelBuilder.HasDefaultSchema("dbo");

            modelBuilder.Entity<MyEntity2>()
                .ToTable("MyEntity")
                .HasKey(k => k.MyEntityID);
        }

        public DbSet<MyEntity2> MyEntity { get; set; }
    }

    public class MyEntity2
    {
        public int MyEntityID { get; set; }
        public int PropA { get; set; }
        public long PropB { get; set; }
        public string PropC { get; set; }
        public System.DateTime PropD { get; set; }
        public Nullable<System.DateTime> PropE { get; set; }
    }
}
