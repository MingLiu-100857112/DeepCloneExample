using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace VIC.CloneExtension.DeepCloneCreators
{
    internal class ArrayCloneFuncCreator : ICloneFuncCreator
    {
        public bool CanClone(Type type)
        {
            return type.IsArray;
        }

        public Func<T, T> CreateCloneFunc<T>()
        {
            var type = typeof(T);
            var func = TypeHelper.CreateCloneFuncMI.MakeGenericMethod(type.GetElementType()).Invoke(null, new object[0]);
            var org = Expression.Parameter(type, "org");
            var tempArray = Expression.Variable(type, "temp");
            var tempArrayNew = Expression.New(type.GetConstructor(new Type[1] { TypeHelper.Int32Type }),
                Expression.ArrayLength(org));
            var tempArrayAssign = Expression.Assign(tempArray, tempArrayNew);
            var index = Expression.Variable(TypeHelper.Int32Type, "index");
            var arrayAccess = Expression.ArrayAccess(tempArray, index);
            var orgArrayAccess = Expression.ArrayAccess(org, index);
            var exitLabel = Expression.Label();
            var array = Expression.Block(new ParameterExpression[1] { index },
                new Expression[3] {Expression.Assign(index, Expression.Constant(0))
                    , tempArrayAssign
                    , Expression.Loop(Expression.IfThenElse(Expression.Equal(index,
                        Expression.ArrayLength(tempArray)), Expression.Goto(exitLabel),
                        Expression.Block(Expression.Assign(arrayAccess, Expression.Invoke(Expression.Constant(func),orgArrayAccess)),
                            Expression.AddAssign(index, Expression.Constant(1)))), exitLabel)
            });
            var setter = Expression.IfThen(Expression.NotEqual(org, Expression.Constant(null, type)), array);
            var block = Expression.Block(new List<ParameterExpression> { tempArray }, new List<Expression> { setter, tempArray });
            return Expression.Lambda<Func<T, T>>(block, org).Compile();
        }
    }
}