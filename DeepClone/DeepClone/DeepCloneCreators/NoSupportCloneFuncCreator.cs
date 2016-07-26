using System;

namespace VIC.CloneExtension.DeepCloneCreators
{
    public class NoSupportCloneFuncCreator : ICloneFuncCreator
    {
        public bool CanClone(Type type)
        {
            return type.IsAbstract || type.IsInterface;
        }

        public Func<T, T> CreateCloneFunc<T>()
        {
            return (T t) => default(T);
        }
    }
}