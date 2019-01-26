using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace SkyLinq.Composition
{
    /// <summary>
    /// Type related helper methods
    /// </summary>
    internal static class TypeHelper
    {
        internal static Type FindGenericType(Type definition, Type type)
        {
            while (type != null && type != typeof(object))
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == definition)
                {
                    return type;
                }
                if (definition.IsInterface)
                {
                    Type[] interfaces = type.GetInterfaces();
                    for (int i = 0; i < interfaces.Length; i++)
                    {
                        Type type1 = TypeHelper.FindGenericType(definition, interfaces[i]);
                        if (type1 != null)
                        {
                            return type1;
                        }
                    }
                }
                type = type.BaseType;
            }
            return null;
        }

        internal static Type GetElementType(Type enumerableType)
        {
            Type type = TypeHelper.FindGenericType(typeof(IEnumerable<>), enumerableType);
            if (type == null)
            {
                return enumerableType;
            }
            return type.GetGenericArguments()[0];
        }

        internal static Type GetNonNullableType(Type type)
        {
            if (!TypeHelper.IsNullableType(type))
            {
                return type;
            }
            return type.GetGenericArguments()[0];
        }

        internal static bool IsEnumerableType(Type enumerableType)
        {
            return TypeHelper.FindGenericType(typeof(IEnumerable<>), enumerableType) != null;
        }

        internal static bool IsKindOfGeneric(Type type, Type definition)
        {
            return TypeHelper.FindGenericType(definition, type) != null;
        }

        internal static bool IsNullableType(Type type)
        {
            if (type == null || !type.IsGenericType)
            {
                return false;
            }
            return type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}
