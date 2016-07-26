using System;
using System.Collections.Generic;
using System.Linq;
using VIC.CloneExtension.DeepCloneCreators;

namespace VIC.CloneExtension
{
    public class DeepCloneManager
    {
        internal static readonly IReadOnlyList<ICloneFuncCreator> Creators
            = new List<ICloneFuncCreator>()
            {
                new NoSupportCloneFuncCreator(),
                new NullableCloneFuncCreator(),
                new ImmutableCloneFuncCreator(),
                new ArrayCloneFuncCreator(),
                new TupleCloneFuncCreator(),
                new KeyValuePairCloneFuncCreator(),
                new ComplexCloneFuncCreator()
            };

        public static Func<T, T> CreateCloneFunc<T>()
        {
            var type = typeof(T);
            var creator = Creators.FirstOrDefault(i => i.CanClone(type));
            return creator == null ? null : creator.CreateCloneFunc<T>();
        }
    }
}