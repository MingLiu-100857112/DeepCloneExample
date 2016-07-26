using System;
using System.Linq.Expressions;
using System.Linq;
using System.Collections.Generic;

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
            var constructorInfo = type.GetConstructors().FirstOrDefault(i => i.GetParameters().Length == 0);
            var temp = Expression.Variable(type, "temp");
            var tempAssign = Expression.Assign(temp, Expression.New(constructorInfo));
            var org = Expression.Parameter(type, "org");

            Expression.Property(org, "Value");
            var block = Expression.Block(new List<ParameterExpression> { temp },
                new List<Expression> 
                { 
                    tempAssign,
                    Expression.IfThen(
                        Expression.Equal(Expression.Property(org, "HasValue"),Expression.Constant(false)),
                        Expression.Assign(temp, Expression.Constant(null, type)))
                });

             return Expression.Lambda<Func<T, T>>(block, org).Compile();
        }
    }
}