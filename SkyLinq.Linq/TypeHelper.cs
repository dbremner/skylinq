using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace SkyLinq.Linq
{
    /// <summary>
    /// Type related helper methods
    /// </summary>
    public static class TypeHelper
    {
        public static Type FindIEnumerable(Type seqType)
        {
            if (seqType == null || seqType == typeof(string))
                return null;
            if (seqType.IsArray)
                return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());

            TypeInfo seqTypeInfo = seqType.GetTypeInfo();

            if (seqTypeInfo.IsGenericType)
            {
                foreach (Type arg in seqTypeInfo.GenericTypeArguments)
                {
                    Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                    if (ienum.GetTypeInfo().IsAssignableFrom(seqTypeInfo))
                    {
                        return ienum;
                    }
                }
            }
            var ifaces = seqTypeInfo.ImplementedInterfaces;
            if (ifaces != null)
            {
                foreach (Type iface in ifaces)
                {
                    Type ienum = FindIEnumerable(iface);
                    if (ienum != null) return ienum;
                }
            }
            if (seqTypeInfo.BaseType != null && seqTypeInfo.BaseType != typeof(object))
            {
                return FindIEnumerable(seqTypeInfo.BaseType);
            }
            return null;
        }

        public static Type GetSequenceType(Type elementType)
        {
            return typeof(IEnumerable<>).MakeGenericType(elementType);
        }

        public static Type GetElementType(Type seqType)
        {
            Type ienum = FindIEnumerable(seqType);
            if (ienum == null) return seqType;
            return ienum.GetTypeInfo().GenericTypeArguments.First();
        }

        public static bool IsNullableType(Type type)
        {
            return type != null && type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static bool IsNullAssignable(Type type)
        {
            return !type.GetTypeInfo().IsValueType || IsNullableType(type);
        }

        public static Type GetNonNullableType(Type type)
        {
            if (IsNullableType(type))
            {
                return type.GetTypeInfo().GenericTypeArguments.First();
            }
            return type;
        }

        public static Type FindGenericType(Type definition, Type type)
        {
            while ((type != null) && (type != typeof(object)))
            {
                if (type.GetTypeInfo().IsGenericType && (type.GetGenericTypeDefinition() == definition))
                {
                    return type;
                }
                if (definition.GetTypeInfo().IsInterface)
                {
                    foreach (Type type2 in type.GetTypeInfo().ImplementedInterfaces)
                    {
                        Type type3 = FindGenericType(definition, type2);
                        if (type3 != null)
                        {
                            return type3;
                        }
                    }
                }
                type = type.GetTypeInfo().BaseType;
            }
            return null;
        }
    }
}
