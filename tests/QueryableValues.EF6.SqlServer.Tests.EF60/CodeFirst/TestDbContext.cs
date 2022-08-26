using System.Data.Entity;

namespace BlazarTech.QueryableValues.EF6.SqlServer.Tests.CodeFirst
{
    public class TestDbContext : DbContext, ITestDbContext
    {
        public TestDbContext() : base("data source=.\\SQLEXPRESS;initial catalog=Test;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Conventions.Remove(new System.Data.Entity.ModelConfiguration.Conventions.PluralizingEntitySetNameConvention());
            //modelBuilder.Conventions.Remove(new System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention());

            modelBuilder.HasDefaultSchema("dbo");

            modelBuilder.Entity<DatabaseFirst.MyEntity>()
                .ToTable("MyEntity")
                .HasKey(k => k.MyEntityID);
        }

        public DbSet<DatabaseFirst.MyEntity> MyEntity { get; set; } = default!;
    }

    //public class MyEntity2 : IMyEntity
    //{
    //    public int MyEntityID { get; set; }
    //    public int PropA { get; set; }
    //    public long PropB { get; set; }
    //    public string PropC { get; set; } = default!;
    //    public System.DateTime PropD { get; set; }
    //    public Nullable<System.DateTime> PropE { get; set; }
    //}
}
