using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace VIC.CloneExtension.DeepCloneCreators
{
    internal class NullableCloneFuncCreator : ICloneFuncCreator
    {
        public bool CanClone(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == TypeHelper.NullableGenericType;
        }

        public Func<T, T> CreateCloneFunc<T>()
        {
            var type = typeof(T);
            var valueType = type.GetGenericArguments()[0];
            var constructorInfo = type.GetConstructors().FirstOrDefault(i => i.GetParameters().Length == 0);
            var temp = Expression.Variable(type, "temp");
            var tempAssign = Expression.Assign(temp, Expression.New(constructorInfo));
            var org = Expression.Parameter(type, "org");
            var func = TypeHelper.CreateCloneFuncMI.MakeGenericMethod(valueType).Invoke(null, new object[0]);
            var clone = Expression.Invoke(Expression.Constant(func), Expression.Property(org, "Value"));
            var block = Expression.Block(new List<ParameterExpression> { temp },
                new List<Expression>
                {
                    tempAssign,
                    Expression.IfThenElse(
                        Expression.Equal(Expression.Property(org, "HasValue"),Expression.Constant(false)),
                        Expression.Assign(temp, Expression.Constant(null, type)),
                        Expression.Assign(temp, clone)),
                    temp
                });

            return Expression.Lambda<Func<T, T>>(block, org).Compile();
        }
    }
}