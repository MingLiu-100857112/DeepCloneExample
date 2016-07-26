using System;
using System.Linq.Expressions;

namespace VIC.CloneExtension.DeepCloneCreators
{
    internal class TupleCloneFuncCreator : ICloneFuncCreator
    {
        public bool CanClone(Type type)
        {
            return type.IsGenericType && TypeHelper.TupleTypes.Contains(type.GetGenericTypeDefinition());
        }

        public Func<T, T> CreateCloneFunc<T>()
        {
            var type = typeof(T);
            var genericTypes = type.GetGenericArguments();
            var constructorInfo = type.GetConstructors()[0];
            var org = Expression.Parameter(type, "org");
            var itemsCloneExpressions = new Expression[genericTypes.Length];
            for (int i = 0; i < genericTypes.Length; i++)
            {
                var func = TypeHelper.CreateCloneFuncMI.MakeGenericMethod(genericTypes[i]).Invoke(null, new object[0]);
                itemsCloneExpressions[i] = Expression.Invoke(Expression.Constant(func),
                    Expression.Property(org, i == 7 ? "Rest" : "Item" + (i + 1).ToString()));
            }

            return Expression.Lambda<Func<T, T>>(Expression.New(constructorInfo, itemsCloneExpressions), org).Compile();
        }
    }
}