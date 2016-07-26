using System;

namespace VIC.CloneExtension.DeepCloneCreators
{
    internal interface ICloneFuncCreator
    {
        bool CanClone(Type type);

        Func<T, T> CreateCloneFunc<T>();
    }
}