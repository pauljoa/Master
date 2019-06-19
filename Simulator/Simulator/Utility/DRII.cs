using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Simulator.Interfaces;
using Simulator.Implementations;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using SharedInterfaces;

namespace Simulator.Utility
{
    public static class DRII
    {
        public static readonly BindingFlags visibilityFlags = BindingFlags.Public | BindingFlags.Instance;
        //Lookup table for already generated dynamic types
        private static Dictionary<Type, Type> Implementations = new Dictionary<Type, Type>();
        //Construction parameters for already generated dynamic types
        private static Dictionary<Type, List<TypeHelper>> Parameters = new Dictionary<Type, List<TypeHelper>>();

        /// <summary>
        /// Implements the specified Interface on a given SysComponent source. Outputs the instanced type
        /// with properties copied from the specified source
        /// </summary>
        /// <param name="Interface">Interface Type parameter, can be inherited type</param>
        /// <param name="source">The instance containing the loaded model</param>
        /// <returns></returns>
        public static object DynamicInterfaceImplementation(Type Interface, SysComponent source)
        {
            
            Type newType = null;
            //If implementation already exist
            if (Implementations.TryGetValue(Interface, out newType))
            {
                if(Parameters.TryGetValue(newType,out List<TypeHelper> parameters))
                {

                }
                var instance = Activator.CreateInstance(newType, GenerateParameters(parameters, source, ref newType).ToArray());
                if (source == null)
                {
                    return instance;
                }
                else
                {
                    return CopyValues(source, (ISysComponent)instance);
                }
            }
            else
            {
                var name = Guid.NewGuid().ToString();
                AssemblyBuilder assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(name), AssemblyBuilderAccess.Run);
                ModuleBuilder module = assembly.DefineDynamicModule(name);

                TypeBuilder type = module.DefineType(typeof(SysComponent).Name + "_" + Interface.Name, TypeAttributes.Public, typeof(SysComponent));
                ConstructorBuilder cstrBuilder = type.DefineConstructor(MethodAttributes.Public
                    | MethodAttributes.HideBySig | MethodAttributes.SpecialName
                    | MethodAttributes.RTSpecialName, CallingConventions.Standard, GetConstructorParams(Interface));
                var cstrGenerator = cstrBuilder.GetILGenerator();
                cstrGenerator.Emit(OpCodes.Ldarg_0);
                cstrGenerator.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));

                var fieldsList = new List<string>();
                var paramList = new List<TypeHelper>();
                Type[] interfaces = Interface.GetInterfaces();
                var index = 0;
                foreach (var ancestor in interfaces)
                {
                    ImplementAncestors(ancestor, ref fieldsList, ref type, ref cstrBuilder, ref cstrGenerator, ref paramList,ref index);
                }
                ImplementAncestors(Interface, ref fieldsList, ref type, ref cstrBuilder, ref cstrGenerator, ref paramList,ref index);
                cstrGenerator.Emit(OpCodes.Ret);
                newType = type.CreateType();
                Implementations.Add(Interface, newType);
                Parameters.Add(newType, paramList);
                var parameterList = new List<dynamic>();
                parameterList = GenerateParameters(paramList,source,ref newType);

                var instance = Activator.CreateInstance(newType, parameterList.ToArray());
                if (source == null)
                {
                    return instance;
                }
                else
                {
                    return CopyValues(source, (ISysComponent)instance);
                    //return BindToInstance(Interface, ret);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Interface"></param>
        /// <returns></returns>
        private static Type[] GetConstructorParams(Type Interface)
        {
            List<Type> types = new List<Type>();
            List<string> names = new List<string>();
            var ancestors = Interface.GetInterfaces();
            
            foreach(var ancestor in ancestors)
            {
                //!m.Name.StartsWith("get_") || !m.Name.StartsWith("set_")
                foreach (var m in ancestor.GetMethods())
                {
                    if (!m.Name.StartsWith("set_"))
                    {
                        if(names.Contains(m.Name))
                        {

                        }
                        else
                        {
                            types.Add(GenerateDelegate(m));
                            names.Add(m.Name);
                        }
                        
                    }
                }
            }
            foreach (var m in Interface.GetMethods())
            {
                //!m.Name.StartsWith("get_") && !m.Name.StartsWith("set_")
                if (!m.Name.StartsWith("set_"))
                {
                    if (names.Contains(m.Name))
                    {

                    }
                    else
                    {
                        types.Add(GenerateDelegate(m));
                        names.Add(m.Name);
                    }
                }
            }
            return types.ToArray();
        }
        /// <summary>
        /// Main method for dynamically implementing a given interface
        /// </summary>
        /// <param name="Interface">The specified interface to implement on the dynamically created type</param>
        /// <param name="fieldsList">reference field list containing names of previously created fields</param>
        /// <param name="type">Reference typebuilder being used by the implementation algorithm </param>
        /// <param name="ctr">Reference constructor builder used to define constructor parameters</param>
        /// <param name="ctrGenerator">Reference IL generator for the constructor</param>
        /// <param name="paramTypes">Reference list containing generated Delegates to be used in the constructor </param>
        /// <param name="index"> Reference parameter index for constructor</param>
        private static void ImplementAncestors(Type Interface, ref List<string> fieldsList, ref TypeBuilder type,ref ConstructorBuilder ctr,ref ILGenerator ctrGenerator,ref List<TypeHelper> paramTypes,ref int index)
        {
            type.AddInterfaceImplementation(Interface);
            foreach (var v in Interface.GetProperties())
            {
                //Adds property to list of already added properties
                if(fieldsList.Contains(v.Name))
                {
                    continue;
                }
                fieldsList.Add(v.Name);

                FieldBuilder field = type.DefineField("_" + v.Name.ToLower(), v.PropertyType, FieldAttributes.Private);
                PropertyBuilder property = type.DefineProperty(v.Name, PropertyAttributes.None, v.PropertyType, new Type[0]);
                if (v.GetGetMethod() != null)
                {
                    Type del = GenerateDelegate(v.GetGetMethod());
                    paramTypes.Add(new TypeHelper(del, "get_"+v.Name));
                    ctr.DefineParameter(index + 1, ParameterAttributes.None, "get_" + v.Name);
                    var delegateField = type.DefineField("del_" + v.Name, del, FieldAttributes.Private);

                    ctrGenerator.Emit(OpCodes.Ldarg_0);
                    ctrGenerator.Emit(OpCodes.Ldarg_S, index + 1);
                    ctrGenerator.Emit(OpCodes.Stfld, delegateField);


                    List<Type> parameterTypes = new List<Type>();

                    MethodBuilder newMethod = type.DefineMethod("get_"+v.Name, MethodAttributes.Public | MethodAttributes.Virtual |
                        MethodAttributes.Final | MethodAttributes.HideBySig
                | MethodAttributes.NewSlot, v.PropertyType,new Type[0]);
                    ILGenerator mGen = newMethod.GetILGenerator();
                    mGen.Emit(OpCodes.Ldarg_0);
                    mGen.Emit(OpCodes.Ldfld, delegateField);

                    mGen.EmitCall(OpCodes.Callvirt, del.GetMethod("Invoke"), null);

                    mGen.Emit(OpCodes.Ret);
                    property.SetGetMethod(newMethod);
                    type.DefineMethodOverride(newMethod,v.GetGetMethod());
                    index++;



                }
                if (v.GetSetMethod() != null)
                {
                    MethodBuilder setter = type.DefineMethod("set_" + v.Name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Virtual, null, new Type[] { v.PropertyType });
                    ILGenerator setGenerator = setter.GetILGenerator();
                    setGenerator.Emit(OpCodes.Ldarg_0);
                    setGenerator.Emit(OpCodes.Ldarg_1);
                    setGenerator.Emit(OpCodes.Stfld, field);
                    setGenerator.Emit(OpCodes.Ret);
                    property.SetSetMethod(setter);
                    type.DefineMethodOverride(setter, v.GetSetMethod());
                }
            }
            foreach(var m in Interface.GetMethods())
            {
                if(m.Name.StartsWith("get_") || m.Name.StartsWith("set_"))
                {
                    continue;
                }
                else
                {
                    Type del = GenerateDelegate(m);
                    paramTypes.Add(new TypeHelper(del,m.Name));
                    ctr.DefineParameter(index + 1, ParameterAttributes.None, "del_"+m.Name);
                    var delegateField = type.DefineField("del_" + m.Name,del,FieldAttributes.Private);

                    ctrGenerator.Emit(OpCodes.Ldarg_0);
                    ctrGenerator.Emit(OpCodes.Ldarg_S, index + 1);
                    ctrGenerator.Emit(OpCodes.Stfld, delegateField);

                    ParameterInfo[] parameters = m.GetParameters();
                    List<Type> parameterTypes = new List<Type>();
                    var callingConvention = m.CallingConvention;
                    foreach( var p in parameters)
                    {
                        parameterTypes.Add(p.ParameterType);
                    }
                    MethodBuilder newMethod = type.DefineMethod(m.Name, MethodAttributes.Public | MethodAttributes.Virtual |
                        MethodAttributes.Final | MethodAttributes.HideBySig 
                |       MethodAttributes.NewSlot, m.CallingConvention,m.ReturnType,parameterTypes.ToArray());
                    ILGenerator mGen = newMethod.GetILGenerator();
                    mGen.Emit(OpCodes.Ldarg_0);
                    mGen.Emit(OpCodes.Ldfld, delegateField);

                    byte paramIndex = 1;
                    foreach(var param in m.GetParameters())
                    {
                        mGen.Emit(OpCodes.Ldarg_S, paramIndex);
                        paramIndex++;
                    }
                    mGen.EmitCall(OpCodes.Callvirt, del.GetMethod("Invoke"), null);

                    mGen.Emit(OpCodes.Ret);
                    index++;
                }
            }
        }
        #region DelegateGenerator
        private static Type GenerateDelegate(MethodInfo info)
        {
            var parameters = info.GetParameters().Select(o => o.ParameterType).ToArray();
            if(info.ReturnType == typeof(void))
            {
                switch(parameters.Length)
                {
                    case 0:
                        return typeof(Action);
                    case 1:
                        return typeof(Action<>).MakeGenericType(parameters);
                    case 2:
                        return typeof(Action<,>).MakeGenericType(parameters);
                    case 3:
                        return typeof(Action<,,>).MakeGenericType(parameters);
                    case 4:
                        return typeof(Action<,,,>).MakeGenericType(parameters);
                    case 5:
                        return typeof(Action<,,,,>).MakeGenericType(parameters);
                    case 6:
                        return typeof(Action<,,,,,>).MakeGenericType(parameters);
                    case 7:
                        return typeof(Action<,,,,,,>).MakeGenericType(parameters);
                    case 8:
                        return typeof(Action<,,,,,,,>).MakeGenericType(parameters);
                    case 9:
                        return typeof(Action<,,,,,,,,>).MakeGenericType(parameters);
                    default:
                        return null;
                }
            }
            else
            {
                switch (parameters.Length)
                {
                    case 0:
                        return typeof(Func<>).MakeGenericType(parameters.Concat(new[] { info.ReturnType }).ToArray());
                    case 1:
                        return typeof(Func<,>).MakeGenericType(parameters.Concat(new[] { info.ReturnType }).ToArray());
                    case 2:
                        return typeof(Func<,,>).MakeGenericType(parameters.Concat(new[] { info.ReturnType }).ToArray());
                    case 3:
                        return typeof(Func<,,,>).MakeGenericType(parameters.Concat(new[] { info.ReturnType }).ToArray());
                    case 4:
                        return typeof(Func<,,,,>).MakeGenericType(parameters.Concat(new[] { info.ReturnType }).ToArray());
                    case 5:
                        return typeof(Func<,,,,,>).MakeGenericType(parameters.Concat(new[] { info.ReturnType }).ToArray());
                    case 6:
                        return typeof(Func<,,,,,,>).MakeGenericType(parameters.Concat(new[] { info.ReturnType }).ToArray());
                    case 7:
                        return typeof(Func<,,,,,,,>).MakeGenericType(parameters.Concat(new[] { info.ReturnType }).ToArray());
                    case 8:
                        return typeof(Func<,,,,,,,,>).MakeGenericType(parameters.Concat(new[] { info.ReturnType }).ToArray());
                    case 9:
                        return typeof(Func<,,,,,,,,,>).MakeGenericType(parameters.Concat(new[] { info.ReturnType }).ToArray());
                    default:
                        return null;
                }
            }





        }
        #endregion
        private static List<dynamic> GenerateParameters(List<TypeHelper> delegates,SysComponent source,ref Type newType)
        {
            List<dynamic> ctrList = new List<dynamic>();
            foreach(var d in delegates)
            {
                var del = d.Del;
                var name = d.MethodName;
                var method = newType.GetMethod(name);
                var methodParameters = method.GetParameters();
                MethodInfo InstanceMethod = source.Instance.GetType().GetMethod(name);
                if (InstanceMethod == null)
                {
                    if (name.StartsWith("get_".ToLower()))
                    {
                        var split = name.Split('_');
                        var filteredName = split[1];
                        InstanceMethod = source.Instance.GetType().GetMethod(filteredName);
                        if (method == null)
                        {
                            InstanceMethod = MethodDiscovery(source.Instance.GetType(), d);
                        }
                    }
                }
                var methodReturnType = method.ReturnType;
                List<Type> parameters = new List<Type>();
                Type delType;
                foreach(var m in methodParameters)
                {
                    parameters.Add(m.ParameterType);
                }
                if (methodReturnType == typeof(void))
                {
                    delType = Expression.GetActionType(parameters.ToArray());
                }
                else
                {
                    parameters.Add(methodReturnType);
                    delType = Expression.GetFuncType(parameters.ToArray());
                }
                Delegate deli = Delegate.CreateDelegate(delType,source.Instance,name);
                ctrList.Add(deli);
            }
            return ctrList;
        }
        //TODO: Implement method discovery function
        private static MethodInfo MethodDiscovery(Type mType, TypeHelper d)
        {
            throw new NotImplementedException();
        }

        private static K CopyValues<K>(SysComponent source, K destination)
        {
            foreach (PropertyInfo property in source.GetType().GetProperties(visibilityFlags))
            {
                var prop = destination.GetType().GetProperty(property.Name, visibilityFlags);
                if (prop != null && prop.CanWrite)
                    prop.SetValue(destination, property.GetValue(source), null);
            }

            return destination;
        }
        private static T BindToInstance<T>(Type Interface,T dest)
        {
            dynamic destProxy = dest;
            foreach(var property in Interface.GetProperties(visibilityFlags))
            {
                var prop = dest.GetType().GetProperty(property.Name, visibilityFlags);
                if(prop != null && prop.CanWrite)
                {
                    prop.SetValue(dest, property.GetValue(destProxy.Instance.GetType().GetProperty(prop.Name).GetValue(destProxy)));
                }
            }
            return dest;
        }
    }

    public class TypeHelper
    {
        public TypeHelper(Type del, string methodName)
        {
            Del = del;
            MethodName = methodName;
        }

        public Type Del { get; set; }
        public string MethodName { get; set; }

    }

}