using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace VIC.CloneExtension
{
    internal static class TypeHelper
    {
        internal static readonly Type Int32Type = typeof(int);
        internal static readonly Type CollectionGenericType = typeof(ICollection<>);
        internal static readonly Type IEnumerableGenericType = typeof(IEnumerable<>);
        internal static readonly Type IEnumeratorGenericType = typeof(IEnumerator<>);
        internal static readonly Type IEnumeratorType = typeof(IEnumerator);
        internal static readonly Type DelegateType = typeof(Delegate);
        internal static readonly Type NullableGenericType = typeof(Nullable<>);

        internal static readonly HashSet<Type> ImmutableTypes = new HashSet<Type>() {
            typeof(string), typeof(DateTime), typeof(TimeSpan)
        };

        internal static readonly HashSet<Type> TupleTypes = new HashSet<Type>() {
            typeof(Tuple<>)
            ,typeof(Tuple<,>)
            ,typeof(Tuple<,,>)
            ,typeof(Tuple<,,,>)
            ,typeof(Tuple<,,,,>)
            ,typeof(Tuple<,,,,,>)
            ,typeof(Tuple<,,,,,,>)
            ,typeof(Tuple<,,,,,,,>)
        };

        internal static MethodInfo CreateCloneFuncMI = typeof(DeepCloneManager).GetMethod("CreateCloneFunc", BindingFlags.Public | BindingFlags.Static);
    }
}