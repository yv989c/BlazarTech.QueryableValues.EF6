using BenchmarkDotNet.Running;

namespace BlazarTech.QueryableValues.EF6.SqlServer.Benchmarks;

internal class Program
{
    static void Main(string[] args)
    {
        BenchmarkRunner.Run<SqlServerBenchmarks>();
    }
}