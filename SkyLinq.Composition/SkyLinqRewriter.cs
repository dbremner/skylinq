using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SkyLinq.Linq;

namespace SkyLinq.Composition
{
    internal sealed class SkyLinqRewriter : ExpressionVisitor
    {
        private static volatile ILookup<string, MethodInfo> _seqMethods;

        internal SkyLinqRewriter()
        {
        }

        private static bool ArgsMatch(MethodInfo m, ReadOnlyCollection<Expression> args, Type[] typeArgs)
        {
            ParameterInfo[] parameters = m.GetParameters();
            if (parameters.Length != args.Count)
            {
                return false;
            }
            if (!m.IsGenericMethod && typeArgs != null && typeArgs.Length > 0)
            {
                return false;
            }
            if (!m.IsGenericMethodDefinition && m.IsGenericMethod && m.ContainsGenericParameters)
            {
                m = m.GetGenericMethodDefinition();
            }
            if (m.IsGenericMethodDefinition)
            {
                if (typeArgs == null || typeArgs.Length == 0)
                {
                    return false;
                }
                if (m.GetGenericArguments().Length != typeArgs.Length)
                {
                    return false;
                }
                m = m.MakeGenericMethod(typeArgs);
                parameters = m.GetParameters();
            }
            int num = 0;
            int count = args.Count;
            while (num < count)
            {
                Type parameterType = parameters[num].ParameterType;
                if (parameterType == null)
                {
                    return false;
                }
                if (parameterType.IsByRef)
                {
                    parameterType = parameterType.GetElementType();
                }
                Expression item = args[num];
                if (!parameterType.IsAssignableFrom(item.Type))
                {
                    if (item.NodeType == ExpressionType.Quote)
                    {
                        item = ((UnaryExpression)item).Operand;
                    }
                    if (!parameterType.IsAssignableFrom(item.Type) && !parameterType.IsAssignableFrom(StripExpression(item.Type)))
                    {
                        return false;
                    }
                }
                num++;
            }
            return true;
        }

        private static MethodInfo FindEnumerableMethod(string name, ReadOnlyCollection<Expression> args, params Type[] typeArgs)
        {
            if (_seqMethods == null)
            {
                _seqMethods = ((IEnumerable<MethodInfo>)typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)).ToLookup((MethodInfo m) => m.Name);
            }
            MethodInfo methodInfo = _seqMethods[name].FirstOrDefault((MethodInfo m) => ArgsMatch(m, args, typeArgs));
            if (methodInfo == null)
            {
                throw new ArgumentException("Unable to find enumerable method " + name);
            }
            if (typeArgs == null)
            {
                return methodInfo;
            }
            return methodInfo.MakeGenericMethod(typeArgs);
        }

        internal static MethodInfo FindMethod(Type type, string name, ReadOnlyCollection<Expression> args, Type[] typeArgs, BindingFlags flags)
        {
            var array = (
                from m in type.GetMethods(flags)
                where m.Name == name
                select m).ToList();
            if (array.Count == 0)
            {
                throw new ArgumentException("Unable to find method " + name + " on type " + type);
            }
            MethodInfo methodInfo = array.FirstOrDefault((MethodInfo m) => ArgsMatch(m, args, typeArgs));
            if (methodInfo == null)
            {
                throw new ArgumentException("Unable to matching arguments for method " + name + " on type " + type);
            }
            if (typeArgs == null)
            {
                return methodInfo;
            }
            return methodInfo.MakeGenericMethod(typeArgs);
        }

        private ReadOnlyCollection<Expression> FixupQuotedArgs(MethodInfo mi, ReadOnlyCollection<Expression> argList)
        {
            ParameterInfo[] parameters = mi.GetParameters();
            if (parameters.Length > 0)
            {
                List<Expression> expressions = null;
                int num = 0;
                int length = parameters.Length;
                while (num < length)
                {
                    Expression item = argList[num];
                    ParameterInfo parameterInfo = parameters[num];
                    item = this.FixupQuotedExpression(parameterInfo.ParameterType, item);
                    if (expressions == null && item != argList[num])
                    {
                        expressions = new List<Expression>(argList.Count);
                        for (int i = 0; i < num; i++)
                        {
                            expressions.Add(argList[i]);
                        }
                    }
                    if (expressions != null)
                    {
                        expressions.Add(item);
                    }
                    num++;
                }
                if (expressions != null)
                {
                    argList = expressions.ToReadOnlyCollection();
                }
            }
            return argList;
        }

        private Expression FixupQuotedExpression(Type type, Expression expression)
        {
            Expression i;
            for (i = expression; !type.IsAssignableFrom(i.Type); i = ((UnaryExpression)i).Operand)
            {
                if (i.NodeType != ExpressionType.Quote)
                {
                    if (!type.IsAssignableFrom(i.Type) && type.IsArray && i.NodeType == ExpressionType.NewArrayInit && type.IsAssignableFrom(StripExpression(i.Type)))
                    {
                        Type elementType = type.GetElementType();
                        NewArrayExpression newArrayExpression = (NewArrayExpression)i;
                        List<Expression> expressions = new List<Expression>(newArrayExpression.Expressions.Count);
                        int num = 0;
                        int count = newArrayExpression.Expressions.Count;
                        while (num < count)
                        {
                            expressions.Add(this.FixupQuotedExpression(elementType, newArrayExpression.Expressions[num]));
                            num++;
                        }
                        expression = Expression.NewArrayInit(elementType, expressions);
                    }
                    return expression;
                }
            }
            return i;
        }

        private static Type GetPublicType(Type t)
        {
            if (t.IsGenericType && typeof(IGrouping<,>).IsAssignableFrom(t.GetGenericTypeDefinition()))
            {
                return typeof(IGrouping<,>).MakeGenericType(t.GetGenericArguments());
            }
            if (!t.IsNestedPrivate)
            {
                return t;
            }
            Type[] interfaces = t.GetInterfaces();
            for (int i = 0; i < interfaces.Length; i++)
            {
                Type type = interfaces[i];
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    return type;
                }
            }
            if (!typeof(IEnumerable).IsAssignableFrom(t))
            {
                return t;
            }
            return typeof(IEnumerable);
        }

        private static Type StripExpression(Type type)
        {
            bool isArray = type.IsArray;
            Type genericArguments = (isArray ? type.GetElementType() : type);
            Type type1 = TypeHelper.FindGenericType(typeof(Expression<>), genericArguments);
            if (type1 != null)
            {
                genericArguments = type1.GetGenericArguments()[0];
            }
            if (!isArray)
            {
                return type;
            }
            int arrayRank = type.GetArrayRank();
            if (arrayRank == 1)
            {
                return genericArguments.MakeArrayType();
            }
            return genericArguments.MakeArrayType(arrayRank);
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            SkyLinqQuery value = c.Value as SkyLinqQuery;
            if (value == null)
            {
                return c;
            }
            if (value.Enumerable == null)
            {
                return this.Visit(value.Expression);
            }
            Type publicType = GetPublicType(value.Enumerable.GetType());
            return Expression.Constant(value.Enumerable, publicType);
        }

        private ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            List<Expression> expressions = null;
            int num = 0;
            int count = original.Count;
            while (num < count)
            {
                Expression expression = this.Visit(original[num]);
                if (expressions != null)
                {
                    expressions.Add(expression);
                }
                else if (expression != original[num])
                {
                    expressions = new List<Expression>(count);
                    for (int i = 0; i < num; i++)
                    {
                        expressions.Add(original[i]);
                    }
                    expressions.Add(expression);
                }
                num++;
            }
            if (expressions == null)
            {
                return original;
            }
            return expressions.ToReadOnlyCollection();
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            Type[] genericArguments;
            Expression expression = this.Visit(m.Object);
            ReadOnlyCollection<Expression> expressions = this.VisitExpressionList(m.Arguments);
            if (expression == m.Object && expressions == m.Arguments)
            {
                return m;
            }
            expressions.ToList();
            if (m.Method.IsGenericMethod)
            {
                genericArguments = m.Method.GetGenericArguments();
            }
            else
            {
                genericArguments = null;
            }
            Type[] typeArray = genericArguments;
            if ((m.Method.IsStatic || m.Method.DeclaringType.IsAssignableFrom(expression.Type)) && ArgsMatch(m.Method, expressions, typeArray))
            {
                return Expression.Call(expression, m.Method, expressions);
            }
            if (m.Method.DeclaringType == typeof(Queryable))
            {
                MethodInfo methodInfo = FindEnumerableMethod(m.Method.Name, expressions, typeArray);
                expressions = this.FixupQuotedArgs(methodInfo, expressions);

                //Try to optimize calls
                switch (m.Method.Name)
                {
                    case "Take":
                    case "First":
                        //If we have Order followed Take or First, we optimize it by calling the Top or Bottom functions
                        Expression innerCall = expressions[0];
                        Expression nexp = m.Method.Name == "Take" ? expressions[1] : Expression.Constant(1);
                        if (innerCall.NodeType == ExpressionType.Call)
                        {
                            MethodInfo innerMI = ((MethodCallExpression)innerCall).Method;
                            if (innerMI.DeclaringType == typeof(Enumerable) &&
                                (innerMI.Name == "OrderBy" || innerMI.Name == "OrderByDescending"))
                            {
                                //Map the new method name
                                string newMethodName = innerMI.Name == "OrderBy" ? "Bottom" : "Top";
                                //Construct the new expressions
                                expressions = new ReadOnlyCollection<Expression>(((MethodCallExpression)innerCall).Arguments.Concat(new Expression[] {nexp}).ToList());
                                methodInfo = FindMethod(typeof(LinqExt), newMethodName,
                                    expressions,
                                    innerMI.GetGenericArguments(),
                                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                            }
                        }
                        break;
                    //case "GroupBy":
                    //    int n = Array.FindIndex(methodInfo.GetParameters(), p => p.Name == "resultSelector");
                    //    if (n > -1)
                    //    {
                    //        Expression resultSelector = expressions[n];
                    //        if (resultSelector.NodeType == ExpressionType.Lambda)
                    //        {
                    //            Expression resultSelectorBody = ((LambdaExpression)resultSelector).Body;
                    //        }
                    //    }
                    //    break;
                }

                return Expression.Call(expression, methodInfo, expressions);
            }
            BindingFlags bindingFlag = (BindingFlags)(8 | (m.Method.IsPublic ? 16 : 32));
            MethodInfo methodInfo1 = FindMethod(m.Method.DeclaringType, m.Method.Name, expressions, typeArray, bindingFlag);
            expressions = this.FixupQuotedArgs(methodInfo1, expressions);
            return Expression.Call(expression, methodInfo1, expressions);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            return p;
        }
    }
}
