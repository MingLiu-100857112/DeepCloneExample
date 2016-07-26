//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Reflection;

//namespace DeepClone
//{
//    public static class DeepCloneHelper
//    {
//        private static Type m_Int32Type = typeof(int);
//        private static Type m_IListType = typeof(IList);
//        private static Type m_IEnumerableGenericType = typeof(IEnumerable<>);
//        private static Type m_IEnumeratorGenericType = typeof(IEnumerator<>);
//        private static Type m_IEnumeratorType = typeof(IEnumerator);
//        private static MethodInfo m_CreateCloneFuncMI = typeof(DeepCloneHelper).GetMethod("CreateCloneFunc", BindingFlags.Public | BindingFlags.Static);

//        public static Func<T, T> CreateCloneFunc<T>()
//        {
//            var type = typeof(T);
//            if (Type.GetTypeCode(type) != TypeCode.Object)
//            {
//                return (T i) => i;
//            }
//            else if (type.IsArray)
//            {
//                return CreateCloneArrayFunc<T>(type);
//            }
//            else if (m_IListType.IsAssignableFrom(type))
//            {
//                return CreateCloneListFunc<T>(type);
//            }
//            else
//            {
//                return CreateCloneClassFunc<T>();
//            }
//        }

//        private static Func<T, T> CreateCloneListFunc<T>(Type type)
//        {
//            var elementType = type.GetGenericArguments()[0];
//            var add = m_IListType.GetMethod("Add");
//            var func = m_CreateCloneFuncMI.MakeGenericMethod(elementType).Invoke(null, new object[0]);
//            var enumerableType = m_IEnumerableGenericType.MakeGenericType(elementType);
//            var enumeratorType = m_IEnumeratorGenericType.MakeGenericType(elementType);
//            var org = Expression.Parameter(type, "org");
//            var tempArray = Expression.Variable(type, "temp");
//            var enumeratorVar = Expression.Variable(enumeratorType, "enumerator");
//            var getEnumeratorCall = Expression.Call(org, enumerableType.GetMethod("GetEnumerator"));
//            var enumeratorAssign = Expression.Assign(enumeratorVar, getEnumeratorCall);
//            var moveNextCall = Expression.Call(enumeratorVar, m_IEnumeratorType.GetMethod("MoveNext"));
//            var tempArrayNew = Expression.New(type.GetConstructor(new Type[1] { m_Int32Type }),
//                Expression.Property(org, "Count"));
//            var tempArrayAssign = Expression.Assign(tempArray, tempArrayNew);
//            var exitLabel = Expression.Label();
//            var array = Expression.Block(new ParameterExpression[1] { enumeratorVar },
//                new Expression[3] {
//                    tempArrayAssign
//                    ,enumeratorAssign
//                    , Expression.Loop(Expression.IfThenElse(Expression.NotEqual(moveNextCall, Expression.Constant(true)), Expression.Goto(exitLabel),
//                        Expression.Call(tempArray,add, Expression.Property(enumeratorVar, "Current"))), exitLabel)
//            });
//            var setter = Expression.IfThen(Expression.NotEqual(org, Expression.Constant(null, type)), array);
//            var block = Expression.Block(new List<ParameterExpression> { tempArray }, new List<Expression> { setter, tempArray });
//            return Expression.Lambda<Func<T, T>>(block, org).Compile();
//        }

//        private static Func<T, T> CreateCloneArrayFunc<T>(Type type)
//        {
//            var func = m_CreateCloneFuncMI.MakeGenericMethod(type.GetElementType()).Invoke(null, new object[0]);
//            var org = Expression.Parameter(type, "org");
//            var tempArray = Expression.Variable(type, "temp");
//            var tempArrayNew = Expression.New(type.GetConstructor(new Type[1] { m_Int32Type }),
//                Expression.ArrayLength(org));
//            var tempArrayAssign = Expression.Assign(tempArray, tempArrayNew);
//            var index = Expression.Variable(m_Int32Type, "index");
//            var arrayAccess = Expression.ArrayAccess(tempArray, index);
//            var orgArrayAccess = Expression.ArrayAccess(org, index);
//            var exitLabel = Expression.Label();
//            var array = Expression.Block(new ParameterExpression[1] { index },
//                new Expression[3] {Expression.Assign(index, Expression.Constant(0))
//                    , tempArrayAssign
//                    , Expression.Loop(Expression.IfThenElse(Expression.Equal(index,
//                        Expression.ArrayLength(tempArray)), Expression.Goto(exitLabel),
//                        Expression.Block(Expression.Assign(arrayAccess, Expression.Invoke(Expression.Constant(func),orgArrayAccess)),
//                            Expression.AddAssign(index, Expression.Constant(1)))), exitLabel)
//            });
//            var setter = Expression.IfThen(Expression.NotEqual(org, Expression.Constant(null, type)), array);
//            var block = Expression.Block(new List<ParameterExpression> { tempArray }, new List<Expression> { setter, tempArray });
//            return Expression.Lambda<Func<T, T>>(block, org).Compile();
//        }

//        private static Func<T, T> CreateCloneClassFunc<T>()
//        {
//            var type = typeof(T);
//            var constructorInfo = type.GetConstructors().FirstOrDefault(i => i.GetParameters().Length == 0);
//            var temp = Expression.Variable(type, "temp");
//            var tempAssign = Expression.Assign(temp, Expression.New(constructorInfo));
//            var org = Expression.Parameter(type, "org");
//            var setters = type.GetFields()
//                .Select(i =>
//                {
//                    var tempProperty = Expression.Field(temp, i);
//                    var orgProperty = Expression.Field(org, i);
//                    var pType = i.FieldType;
//                    var func = m_CreateCloneFuncMI.MakeGenericMethod(pType).Invoke(null, new object[0]);
//                    var clone = Expression.Invoke(Expression.Constant(func), orgProperty);
//                    return Expression.Assign(tempProperty, clone);
//                }).ToList();
//            setters.AddRange(type.GetProperties().Where(i => i.CanRead && i.CanWrite)
//                .Select(i =>
//                {
//                    var tempProperty = Expression.Property(temp, i);
//                    var orgProperty = Expression.Property(org, i);
//                    var pType = i.PropertyType;
//                    var func = m_CreateCloneFuncMI.MakeGenericMethod(pType).Invoke(null, new object[0]);
//                    var clone = Expression.Invoke(Expression.Constant(func), orgProperty);
//                    return Expression.Assign(tempProperty, clone);
//                }));

//            setters.Insert(0, tempAssign);
//            var setter = Expression.IfThen(Expression.NotEqual(org, Expression.Constant(null, type)), Expression.Block(setters));
//            var block = Expression.Block(new List<ParameterExpression> { temp }, new List<Expression> { setter, temp });
//            return Expression.Lambda<Func<T, T>>(block, org).Compile();
//        }

//        public static T DeepClone<T>(this T t) where T : class
//        {
//            return CreateCloneFunc<T>()(t);
//        }
//    }
//}