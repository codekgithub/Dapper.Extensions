﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Dapper.Extensions
{
    public static class ReflectionHelper
    {
        private static readonly List<Type> SimpleTypes = new List<Type>
                               {
                                   typeof(byte),
                                   typeof(sbyte),
                                   typeof(short),
                                   typeof(ushort),
                                   typeof(int),
                                   typeof(uint),
                                   typeof(long),
                                   typeof(ulong),
                                   typeof(float),
                                   typeof(double),
                                   typeof(decimal),
                                   typeof(bool),
                                   typeof(string),
                                   typeof(char),
                                   typeof(Guid),
                                   typeof(DateTime),
                                   typeof(DateTimeOffset),
                                   typeof(byte[])
                               };

        public static MemberInfo GetProperty(LambdaExpression lambda)
        {
            Expression expr = lambda;
            for (; ; )
            {
                switch (expr.NodeType)
                {
                    case ExpressionType.Lambda:
                        expr = ((LambdaExpression)expr).Body;
                        break;
                    case ExpressionType.Convert:
                        expr = ((UnaryExpression)expr).Operand;
                        break;
                    case ExpressionType.MemberAccess:
                        MemberExpression memberExpression = (MemberExpression)expr;
                        MemberInfo mi = memberExpression.Member;
                        return mi;
                    default:
                        return null;
                }
            }
        }

        public static IDictionary<string, object> GetObjectValues(object obj)
        {
            IDictionary<string, object> result = new Dictionary<string, object>();
            if (obj == null)
            {
                return result;
            }
            PropertyInfo[] propertyInfos = obj.GetType().GetProperties();
            foreach (var propertyInfo in propertyInfos)
            {
                string name = propertyInfo.Name;
                object value = propertyInfo.GetValue(obj, null);
                result[name] = value;
            }
            return result;
        }

        public static string AppendStrings(this IEnumerable<string> list, string seperator = ", ")
        {
            return list.Aggregate(
                new StringBuilder(),
                (sb, s) => (sb.Length == 0 ? sb : sb.Append(seperator)).Append(s),
                sb => sb.ToString());
        }

        public static bool IsSimpleType(Type type)
        {
            Type actualType = type;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                actualType = type.GetGenericArguments()[0];
            }

            return SimpleTypes.Contains(actualType);
        }

    }
}