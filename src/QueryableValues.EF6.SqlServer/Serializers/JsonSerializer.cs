using System;
using System.Collections.Generic;

namespace BlazarTech.QueryableValues.Serializers
{
    internal sealed class JsonSerializer : ISerializer
    {
        public SerializationFormat Format => SerializationFormat.Json;

        private static string SerializePrivate<T>(T values)
        {
#if NET452
            return Newtonsoft.Json.JsonConvert.SerializeObject(values);
#else
            return System.Text.Json.JsonSerializer.Serialize(values);
#endif
        }

        public string Serialize(IEnumerable<byte> values)
        {
            return SerializePrivate(values);
        }

        public string Serialize(IEnumerable<short> values)
        {
            return SerializePrivate(values);
        }

        public string Serialize(IEnumerable<int> values)
        {
            return SerializePrivate(values);
        }

        public string Serialize(IEnumerable<long> values)
        {
            return SerializePrivate(values);
        }

        public string Serialize(IEnumerable<decimal> values)
        {
            return SerializePrivate(values);
        }

        public string Serialize(IEnumerable<float> values)
        {
            return SerializePrivate(values);
        }

        public string Serialize(IEnumerable<double> values)
        {
            return SerializePrivate(values);
        }

        public string Serialize(IEnumerable<DateTime> values)
        {
            return SerializePrivate(values);
        }

        public string Serialize(IEnumerable<DateTimeOffset> values)
        {
            return SerializePrivate(values);
        }

        public string Serialize(IEnumerable<Guid> values)
        {
            return SerializePrivate(values);
        }

        public string Serialize(IEnumerable<char> values)
        {
            return SerializePrivate(values);
        }

        public string Serialize(IEnumerable<string> values)
        {
            return SerializePrivate(values);
        }
    }
}
