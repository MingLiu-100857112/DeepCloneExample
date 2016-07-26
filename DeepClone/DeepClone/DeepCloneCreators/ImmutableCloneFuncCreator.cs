using System;

namespace VIC.CloneExtension.DeepCloneCreators
{
    internal class ImmutableCloneFuncCreator : ICloneFuncCreator
    {
        public Func<T, T> CreateCloneFunc<T>()
        {
            return (T t) => t;
        }

        public bool CanClone(Type type)
        {
            return type.IsPrimitive
                || TypeHelper.ImmutableTypes.Contains(type)
                || TypeHelper.DelegateType.IsAssignableFrom(type);
        }
    }
}