using CloneExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace VIC.CloneExtension.DeepCloneCreators
{
    internal class ComplexCloneFuncCreator : ICloneFuncCreator
    {
        public bool CanClone(Type type)
        {
            return true;
        }

        public Func<T, T> CreateCloneFunc<T>()
        {
            var type = typeof(T);
            var constructorInfo = type.GetConstructors().FirstOrDefault(i => i.GetParameters().Length == 0);
            var collectionType = type.GetInterfaces()
                                      .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == TypeHelper.CollectionGenericType);
            var temp = Expression.Variable(type, "temp");
            var org = Expression.Parameter(type, "org");
            Expression tempAssign = null;
            if (collectionType == null)
            {
                tempAssign = Expression.Assign(temp, Expression.New(constructorInfo));
            }
            else
            {
                var func = CreateCloneListFunc<T>(type);
                tempAssign = Expression.Assign(temp, Expression.Invoke(Expression.Constant(func), org));
            }
            var setters = type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(i => !i.IsInitOnly && !i.GetCustomAttributes<NonClonedAttribute>(true).Any())
                .Select(i =>
                {
                    var tempProperty = Expression.Field(temp, i);
                    var orgProperty = Expression.Field(org, i);
                    var pType = i.FieldType;
                    var func = TypeHelper.CreateCloneFuncMI.MakeGenericMethod(pType).Invoke(null, new object[0]);
                    var clone = Expression.Invoke(Expression.Constant(func), orgProperty);
                    return Expression.Assign(tempProperty, clone);
                }).ToList<Expression>();
            setters.AddRange(type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(i => i.CanRead && i.CanWrite && !i.GetCustomAttributes<NonClonedAttribute>(true).Any())
                .Select(i =>
                {
                    var tempProperty = Expression.Property(temp, i);
                    var orgProperty = Expression.Property(org, i);
                    var pType = i.PropertyType;
                    var func = TypeHelper.CreateCloneFuncMI.MakeGenericMethod(pType).Invoke(null, new object[0]);
                    var clone = Expression.Invoke(Expression.Constant(func), orgProperty);
                    return Expression.Assign(tempProperty, clone);
                }));

            setters.Insert(0, tempAssign);
            var setter = Expression.IfThen(Expression.NotEqual(org, Expression.Constant(null, type)), Expression.Block(setters));
            var block = Expression.Block(new List<ParameterExpression> { temp }, new List<Expression> { setter, temp });
            return Expression.Lambda<Func<T, T>>(block, org).Compile();
        }

        private Func<T, T> CreateCloneListFunc<T>(Type type)
        {
            var elementType = type.GetGenericArguments()[0];
            var add = TypeHelper.CollectionGenericType.MakeGenericType(elementType).GetMethod("Add");
            var func = TypeHelper.CreateCloneFuncMI.MakeGenericMethod(elementType).Invoke(null, new object[0]);
            var enumerableType = TypeHelper.IEnumerableGenericType.MakeGenericType(elementType);
            var enumeratorType = TypeHelper.IEnumeratorGenericType.MakeGenericType(elementType);
            var org = Expression.Parameter(type, "org");
            var tempArray = Expression.Variable(type, "temp");
            var enumeratorVar = Expression.Variable(enumeratorType, "enumerator");
            var getEnumeratorCall = Expression.Call(org, enumerableType.GetMethod("GetEnumerator"));
            var enumeratorAssign = Expression.Assign(enumeratorVar, getEnumeratorCall);
            var moveNextCall = Expression.Call(enumeratorVar, TypeHelper.IEnumeratorType.GetMethod("MoveNext"));
            var tempArrayNew = Expression.New(type.GetConstructor(new Type[1] { TypeHelper.Int32Type }),
                Expression.Property(org, "Count"));
            var tempArrayAssign = Expression.Assign(tempArray, tempArrayNew);
            var exitLabel = Expression.Label();
            var array = Expression.Block(new ParameterExpression[1] { enumeratorVar },
                new Expression[3] {
                    tempArrayAssign
                    ,enumeratorAssign
                    , Expression.Loop(Expression.IfThenElse(Expression.NotEqual(moveNextCall, Expression.Constant(true)), Expression.Goto(exitLabel),
                        Expression.Call(tempArray,add, Expression.Property(enumeratorVar, "Current"))), exitLabel)
            });
            var setter = Expression.IfThen(Expression.NotEqual(org, Expression.Constant(null, type)), array);
            var block = Expression.Block(new List<ParameterExpression> { tempArray }, new List<Expression> { setter, tempArray });
            return Expression.Lambda<Func<T, T>>(block, org).Compile();
        }
    }
}