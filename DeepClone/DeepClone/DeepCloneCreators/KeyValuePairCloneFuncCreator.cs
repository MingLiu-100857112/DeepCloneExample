using System;
using System.Linq;
using System.Linq.Expressions;

namespace VIC.CloneExtension.DeepCloneCreators
{
    internal class KeyValuePairCloneFuncCreator : ICloneFuncCreator
    {
        public bool CanClone(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == TypeHelper.KeyValuePairGenericType;
        }

        public Func<T, T> CreateCloneFunc<T>()
        {
            var type = typeof(T);
            var argumentTypes = type.GetGenericArguments();
            var keyType = argumentTypes[0];
            var valueType = argumentTypes[1];
            var constructorInfo = type.GetConstructors().FirstOrDefault(i => i.GetParameters().Length == 2);
            var org = Expression.Parameter(type, "org");
            var func = TypeHelper.CreateCloneFuncMI.MakeGenericMethod(keyType).Invoke(null, new object[0]);
            var cloneKey = Expression.Invoke(Expression.Constant(func), Expression.Property(org, "Key"));
            func = TypeHelper.CreateCloneFuncMI.MakeGenericMethod(valueType).Invoke(null, new object[0]);
            var cloneValue = Expression.Invoke(Expression.Constant(func), Expression.Property(org, "Value"));
            return Expression.Lambda<Func<T, T>>(Expression.New(constructorInfo, cloneKey, cloneValue), org).Compile();
        }
    }
}