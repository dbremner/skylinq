using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace SkyLinq.Composition
{
    public class DuckTypeProxyFactory
    {
        private static readonly IDictionary<Tuple<Type, Type>, Type> _typeCache = new Dictionary<Tuple<Type, Type>, Type>();
        private static readonly string _assemblyName = "SkyLinq.DuckTypingProxies.Generated";
        private static readonly AssemblyBuilder _assemblyBuilder;
        private static readonly ModuleBuilder _moduleBuilder;
        private static readonly ReaderWriterLockSlim _cacheLock = new ReaderWriterLockSlim();

        static DuckTypeProxyFactory()
        {
            _assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName(_assemblyName),
#if DEBUG
                    AssemblyBuilderAccess.RunAndSave
#else
                    AssemblyBuilderAccess.Run
#endif
            );

#if DEBUG
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule(_assemblyName,
                _assemblyName + ".dll", true);
#else
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(_assemblyName);
#endif
        }

        public TProxy GenerateProxy<TProxy>(Type proxyInterfaceType, object obj) where TProxy : class
        {
            ValidateParams(proxyInterfaceType, obj);

            //If obj already implements TProxy, simply return it
            TProxy o = obj as TProxy;

            if (o != null) return o;

            Type proxyType = null;
            _cacheLock.EnterUpgradeableReadLock();
            try
            { 
                if (!_typeCache.TryGetValue(new Tuple<Type,Type>(proxyInterfaceType, obj.GetType()), out proxyType))
                {
                    //Generate the proxyType here
                    if (!CanBeDuckTypedTo(proxyInterfaceType, obj))
                        throw new ArgumentException("Object cannot be duck typed by the interface.");

                    proxyType = GenerateProxyType(proxyInterfaceType, obj.GetType());

                    _cacheLock.EnterWriteLock();
                    try
                    { 
                        _typeCache.Add(new Tuple<Type, Type>(proxyInterfaceType, obj.GetType()), proxyType);
                    }
                    finally
                    { 
                        _cacheLock.ExitWriteLock();
                    }
                }
                return (TProxy)Activator.CreateInstance(proxyType, obj);
            }
            finally
            {
                _cacheLock.ExitUpgradeableReadLock();
            }
        }

        private static void ValidateParams(Type proxyInterfaceType, object obj)
        {
            if (!proxyInterfaceType.IsInterface)
                throw new ArgumentException("proxyInterfaceType must be a type of an interface.");

            if (obj == null)
                throw new ArgumentNullException("Object to be wrapped cannot be null");
        }

        public bool CanBeDuckTypedTo(Type proxyInterfaceType, object obj)
        {
            ValidateParams(proxyInterfaceType, obj);
            Type t = obj.GetType();
            return proxyInterfaceType.GetMembers().All(m =>
                {
                    //Interface member can be either method or property
                    if (m.MemberType == MemberTypes.Method)
                    {
                        MethodInfo mi = (MethodInfo)m;
                        MethodInfo mi2 = t.GetMethod(m.Name, mi.GetParameters().Select(pi => pi.ParameterType).ToArray());
                        return (mi2 != null) && mi2.IsPublic && !mi2.IsAbstract && !mi2.IsStatic && mi.ReturnType == mi2.ReturnType;
                    }
                    else //Property
                    {
                        PropertyInfo pi = (PropertyInfo)m;
                        PropertyInfo pi2 = t.GetProperty(m.Name, pi.PropertyType, pi.GetIndexParameters().Select(param => param.ParameterType).ToArray());
                        return pi2 != null;
                    }
                }
            );
        }

        public Type GenerateProxyType(Type proxyInterfaceType, Type typeToIntercept)
        {
            TypeAttributes newAttributes = TypeAttributes.Public | TypeAttributes.Class;
            TypeBuilder typeBuilder = _moduleBuilder.DefineType(typeToIntercept.Name + "_DuckTypingProxy" + new Guid().ToString(), newAttributes);
            // Add interface implementation
            typeBuilder.AddInterfaceImplementation(proxyInterfaceType);

            FieldBuilder targetField = typeBuilder.DefineField("target", typeToIntercept, FieldAttributes.Private);

            foreach(MethodInfo mi in proxyInterfaceType.GetMethods())
            {
                CreateDelegateImplementation(typeBuilder, targetField, mi);
            }

            foreach(PropertyInfo pi in proxyInterfaceType.GetProperties())
            {
                PropertyBuilder pb = typeBuilder.DefineProperty(
                    pi.Name,
                    pi.Attributes,
                    pi.PropertyType,
                    pi.GetIndexParameters().Select(param => param.ParameterType).ToArray()
                );
                MethodInfo getMi = pi.GetGetMethod();
                if (getMi != null)
                { 
                    MethodBuilder getMethod = CreateDelegateImplementation(typeBuilder, targetField, getMi);
                    pb.SetGetMethod(getMethod);
                }

                MethodInfo setMi = pi.GetSetMethod();
                if (setMi != null)
                { 
                    MethodBuilder setMethod = CreateDelegateImplementation(typeBuilder, targetField, setMi);
                    pb.SetSetMethod(setMethod);
                }
            }

            //Constructor
            ConstructorBuilder ctorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                CallingConventions.HasThis,
                new Type[] { typeToIntercept });
            ctorBuilder.DefineParameter(1, ParameterAttributes.None, "target");
            ILGenerator il = ctorBuilder.GetILGenerator();

            // Call base class constructor
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, CodeGenUtil.GetConstructorInfo(() => new object()));

            // Initialize the target field
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, targetField);
            il.Emit(OpCodes.Ret);

            Type result = typeBuilder.CreateType();
#if DEBUG
            _assemblyBuilder.Save(_assemblyName + ".dll");
#endif
            return result;
        }

        private MethodBuilder CreateDelegateImplementation(TypeBuilder typeBuilder, FieldBuilder targetField, MethodInfo mi)
        {
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(mi.Name,
                MethodAttributes.Public | MethodAttributes.Virtual,
                mi.ReturnType,
                mi.GetParameters().Select(param => param.ParameterType).ToArray());

            ILGenerator il = methodBuilder.GetILGenerator();

            #region forwarding implementation

            LocalBuilder baseReturn = null;

            if (mi.ReturnType != typeof(void))
            {
                baseReturn = il.DeclareLocal(mi.ReturnType);
            }

            // Call the target method
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, targetField);

            // Load the call parameters
            for(int i = 0; i < mi.GetParameters().Length; i++)
            {
                CodeGenUtil.EmitLoadArgument(il, i);
            }

            // Make the call
            MethodInfo callTarget = targetField.FieldType.GetMethod(mi.Name, mi.GetParameters().Select(pi => pi.ParameterType).ToArray());
            il.Emit(OpCodes.Callvirt, callTarget);

            if (mi.ReturnType != typeof(void))
            {
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloc_0);
            }

            il.Emit(OpCodes.Ret);

            #endregion

            return methodBuilder;
        }

    }
}
