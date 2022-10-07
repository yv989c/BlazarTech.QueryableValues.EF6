using System.Text;

namespace BlazarTech.QueryableValues
{
    sealed class StringBuilderPool
    {
#if NET452
        public static readonly StringBuilderPool Shared = new();

        public StringBuilder Get() => new();
        public void Return(StringBuilder _) { }
#else
        public static readonly Microsoft.Extensions.ObjectPool.DefaultObjectPool<StringBuilder> Shared = new(
            new Microsoft.Extensions.ObjectPool.StringBuilderPooledObjectPolicy
            {
                InitialCapacity = 512,
                MaximumRetainedCapacity = 524288
            });
#endif
    }
}
