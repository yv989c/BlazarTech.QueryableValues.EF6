using System.Data.Entity;

namespace BlazarTech.QueryableValues.EF6.SqlServer.Benchmarks;

public class TestDbContext : DbContext
{
    public DbSet<TestDataEntity> TestData { get; set; } = null!;

    public TestDbContext(string connectionString) : base(connectionString)
    {
        Database.SetInitializer<TestDbContext>(null);
    }

    public static TestDbContext Create(bool useDatabaseNullSemantics = false, bool useCompat120 = false)
    {
        var cs = useCompat120 ?
            DbUtil.GetConnectionString(false, databaseNameSuffix: "TestsCompatLevel120") :
            DbUtil.GetConnectionString(false);

        return new TestDbContext(cs)
        {
            Configuration =
            {
                UseDatabaseNullSemantics = useDatabaseNullSemantics
            }
        };
    }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("dbo");
          
        var entity = modelBuilder.Entity<TestDataEntity>()
            .ToTable("TestData")
            .HasKey(k => k.Id);

        entity.Property(p => p.StringValue).HasColumnType("varchar").HasMaxLength(50).IsUnicode(false);
        entity.Property(p => p.StringUnicodeValue).HasColumnType("nvarchar").HasMaxLength(50).IsUnicode(true);
        entity.Property(p => p.DateTimeValue).HasColumnType("datetime2");
    }
}
