using System.Text;

#if NETSTANDARD2_1_OR_GREATER || NET472_OR_GREATER || NET
using Microsoft.Extensions.ObjectPool;
#endif

namespace BlazarTech.QueryableValues
{
    sealed class StringBuilderPool
    {
#if NETSTANDARD2_1_OR_GREATER || NET472_OR_GREATER || NET
        public static readonly DefaultObjectPool<StringBuilder> Shared = new DefaultObjectPool<StringBuilder>(
            new StringBuilderPooledObjectPolicy
            {
                InitialCapacity = 512,
                MaximumRetainedCapacity = 524288
            });
#else
        public static readonly StringBuilderPool Shared = new StringBuilderPool();
#endif

#if NET452
        public StringBuilder Get() => new StringBuilder();
        public void Return(StringBuilder _) { }
#endif
    }
}
