using System.Data.Entity;

namespace BlazarTech.QueryableValues.EF6.SqlServer.Tests.CodeFirst
{
    public class TestDbContext : DbContext, ITestDbContext, ITestDbContextWithSauce
    {
        public DbSet<DatabaseFirst.TestDataEntity> TestData { get; set; } = null!;

        public TestDbContext(string connectionString) : base(connectionString)
        {
            Database.SetInitializer<TestDbContext>(null);
        }

        public static TestDbContext Create()
        {
            return new TestDbContext(DbUtil.GetConnectionString(false));
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Conventions.Remove(new System.Data.Entity.ModelConfiguration.Conventions.PluralizingEntitySetNameConvention());
            //modelBuilder.Conventions.Remove(new System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention());

            modelBuilder.HasDefaultSchema("dbo");
              
            var entity = modelBuilder.Entity<DatabaseFirst.TestDataEntity>()
                .ToTable("TestData")
                .HasKey(k => k.Id);

            //entity.Property(p => p.CharValue).HasColumnType("char");
            //entity.Property(p => p.CharUnicodeValue).HasColumnType("nchar");
            entity.Property(p => p.StringValue).HasColumnType("varchar").HasMaxLength(50).IsUnicode(false);
            entity.Property(p => p.StringUnicodeValue).HasColumnType("nvarchar").HasMaxLength(50).IsUnicode(true);
            entity.Property(p => p.DateTimeValue).HasColumnType("datetime2");
        }
    }
}
