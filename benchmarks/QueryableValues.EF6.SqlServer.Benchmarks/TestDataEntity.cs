namespace BlazarTech.QueryableValues.EF6.SqlServer.Benchmarks;

public class TestDataEntity
{
    public int Id { get; set; }
    public bool BoolValue { get; set; }
    public byte ByteValue { get; set; }
    public short Int16Value { get; set; }
    public int Int32Value { get; set; }
    public long Int64Value { get; set; }
    public decimal DecimalValue { get; set; }
    public float SingleValue { get; set; }
    public double DoubleValue { get; set; }
    public string StringValue { get; set; }
    public string StringUnicodeValue { get; set; }
    public System.DateTime DateTimeValue { get; set; }
    public System.DateTimeOffset DateTimeOffsetValue { get; set; }
    public System.Guid GuidValue { get; set; }
}
