using System;
using System.Collections.Concurrent;

namespace VIC.CloneExtension
{
    public static class DeepCloneExtension
    {
        internal static readonly ConcurrentDictionary<Type, Func<object, object>> Cache
            = new ConcurrentDictionary<Type, Func<object, object>>();

        public static T DeepClone<T>(this T t)
        {
            var clone = Cache.GetOrAdd(typeof(T), (type) =>
            {
                var func = DeepCloneManager.CreateCloneFunc<T>();
                return (object org) => (object)func((T)org);
            });
            return (T)clone(t);
        }
    }
}