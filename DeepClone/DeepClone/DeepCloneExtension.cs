using System;

namespace VIC.CloneExtension
{
    public static class DeepCloneExtension<T>
    {
        internal static readonly Func<T, T> Cache
            = DeepCloneManager.CreateCloneFunc<T>();

        public static T DeepClone(T t)
        {
            return Cache(t);
        }
    }

    public static class DeepCloneExtension
    {
        public static T DeepClone<T>(this T t)
        {
            return DeepCloneExtension<T>.DeepClone(t);
        }
    }
}