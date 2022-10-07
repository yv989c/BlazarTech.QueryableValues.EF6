using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazarTech.QueryableValues.EF6.SqlServer.Benchmarks;

[SimpleJob(RunStrategy.Monitoring, RuntimeMoniker.Net60, warmupCount: 1, targetCount: 10, invocationCount: 12)]
[GcServer(true)]
[GroupBenchmarksBy(BenchmarkDotNet.Configs.BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
//[MemoryDiagnoser]
public class SqlServerBenchmarks
{
    private DbContextFixture _dbContextFixture = default!;

    public TestDbContext Db => _dbContextFixture.Db;

    [Params(2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768, 65536, 131072)]
    public int NumberOfValues { get; set; }

    [GlobalSetup]
    public async Task GlobalSetupAsync()
    {
        _dbContextFixture = new DbContextFixture();
        await _dbContextFixture.InitializeAsync();
    }

    private IEnumerable<int> GetIntValues()
    {
        var random = new Random(1);

        for (int i = 0; i < NumberOfValues; i++)
        {
            yield return random.Next(int.MaxValue);
        }
    }

    private IEnumerable<string> GetStringValues()
    {
        var random = new Random(1);
        var sb = new StringBuilder();

        for (int i = 0; i < NumberOfValues; i++)
        {
            sb.Clear();
            var length = random.Next(0, 50);
            for (int x = 0; x < length; x++)
            {
                sb.Append((char)random.Next(32, 65));
            }
            yield return sb.ToString();
        }
    }

    private IEnumerable<Guid> GetGuidValues()
    {
        var random = new Random(1);
        var buffer = new byte[16];

        for (int i = 0; i < NumberOfValues; i++)
        {
            random.NextBytes(buffer);
            yield return new Guid(buffer);
        }
    }

    private async Task Int32ValuesAsync()
    {
        _ = await (
            from e in Db.TestData
            join v in Db.AsQueryableValues(GetIntValues()) on e.Int32Value equals v
            select e.Id
            )
            .ToListAsync();
    }

    private async Task StringValuesAsync()
    {
        _ = await (
            from e in Db.TestData
            join v in Db.AsQueryableValues(GetStringValues()) on e.StringValue equals v
            select e.Id
            )
            .ToListAsync();
    }

    private async Task GuidValuesAsync()
    {
        _ = await (
            from e in Db.TestData
            join v in Db.AsQueryableValues(GetGuidValues()) on e.GuidValue equals v
            select e.Id
            )
            .ToListAsync();
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Int32Values")]
    public async Task Int32ValuesXmlAsync()
    {
        QueryableValuesConfigurator.Configure()
            .Serialization(SerializationOptions.UseXml);

        await Int32ValuesAsync();
    }

    [Benchmark]
    [BenchmarkCategory("Int32Values")]
    public async Task Int32ValuesJsonAsync()
    {
        QueryableValuesConfigurator.Configure()
            .Serialization(SerializationOptions.UseJson);

        await Int32ValuesAsync();
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("StringValues")]
    public async Task StringValuesXmlAsync()
    {
        QueryableValuesConfigurator.Configure()
            .Serialization(SerializationOptions.UseXml);

        await StringValuesAsync();
    }

    [Benchmark]
    [BenchmarkCategory("StringValues")]
    public async Task StringValuesJsonAsync()
    {
        QueryableValuesConfigurator.Configure()
            .Serialization(SerializationOptions.UseJson);

        await StringValuesAsync();
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("GuidValues")]
    public async Task GuidValuesXmlAsync()
    {
        QueryableValuesConfigurator.Configure()
            .Serialization(SerializationOptions.UseXml);

        await GuidValuesAsync();
    }

    [Benchmark]
    [BenchmarkCategory("GuidValues")]
    public async Task GuidValuesJsonAsync()
    {
        QueryableValuesConfigurator.Configure()
            .Serialization(SerializationOptions.UseJson);

        await GuidValuesAsync();
    }
}
