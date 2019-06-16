using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Simulator.Interfaces;
using Simulator.Implementations;
using System.Collections.Concurrent;
using System.Linq;

namespace Simulator.Utility
{
    public static class DRII
    {
        public static readonly BindingFlags visibilityFlags = BindingFlags.Public | BindingFlags.Instance;
        private static Dictionary<Type, Type> Implementations = new Dictionary<Type, Type>();

        

        public static object DynamicInterfaceImplementation(Type Interface, SysComponent source)
        {
            var name = Guid.NewGuid().ToString();
            AssemblyBuilder assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(name), AssemblyBuilderAccess.Run);
            ModuleBuilder module = assembly.DefineDynamicModule(name);

            TypeBuilder type = module.DefineType(typeof(SysComponent).Name + "_" + Interface.Name, TypeAttributes.Public, typeof(SysComponent));
            ConstructorBuilder cstrBuilder = type.DefineConstructor( MethodAttributes.Public 
                | MethodAttributes.HideBySig | MethodAttributes.SpecialName 
                | MethodAttributes.RTSpecialName, CallingConventions.Standard,GetConstructorParams(Interface));
            var cstrGenerator = cstrBuilder.GetILGenerator();
            cstrGenerator.Emit(OpCodes.Ldarg_0);
            cstrGenerator.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));


            var fieldsList = new List<string>();
            var paramList = new List<Type>();
            Type[] interfaces = Interface.GetInterfaces();
            
            foreach(var ancestor in interfaces)
            {
                ImplementAncestors(ancestor, ref fieldsList, ref type,ref cstrBuilder,ref cstrGenerator,ref paramList);
            }
            ImplementAncestors(Interface, ref fieldsList, ref type,ref cstrBuilder, ref cstrGenerator, ref paramList);
            cstrGenerator.Emit(OpCodes.Ret);
            Type newType = type.CreateType();
            //TODO: Fix parameterized constructor
            var instance = Activator.CreateInstance(newType);
            if(source == null)
            {
                return instance;
            }
            else
            {
                return CopyValues(source, (ISysComponent)instance);
            }
        }
        private static Type[] GetConstructorParams(Type Interface)
        {
            List<Type> types = new List<Type>();
            List<string> names = new List<string>();
            var ancestors = Interface.GetInterfaces();
            
            foreach(var ancestor in ancestors)
            {
                foreach(var m in ancestor.GetMethods())
                {
                    if (!m.Name.StartsWith("get_") || !m.Name.StartsWith("set_"))
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
                if (!m.Name.StartsWith("get_") && !m.Name.StartsWith("set_"))
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

        private static void ImplementAncestors(Type Interface, ref List<string> fieldsList, ref TypeBuilder type,ref ConstructorBuilder ctr,ref ILGenerator ctrGenerator,ref List<Type> paramTypes)
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
                    MethodBuilder getter = type.DefineMethod("get_" + v.Name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Virtual, v.PropertyType, new Type[0]);
                    ILGenerator getGenerator = getter.GetILGenerator();
                    getGenerator.Emit(OpCodes.Ldarg_0);
                    getGenerator.Emit(OpCodes.Ldfld, field);
                    getGenerator.Emit(OpCodes.Ret);
                    property.SetGetMethod(getter);
                    type.DefineMethodOverride(getter, v.GetGetMethod());
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
                    paramTypes.Add(del);
                    var index = 0;
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
        #region Comment
        //public static K Implement<K>(T source = default(T))
        //{


        //    var name = Guid.NewGuid().ToString();
        //    AssemblyBuilder assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(name), AssemblyBuilderAccess.Run);
        //    ModuleBuilder module = assembly.DefineDynamicModule(name);


        //    TypeBuilder type = module.DefineType(typeof(T).Name + "_" + typeof(K).Name, TypeAttributes.Public, typeof(SysComponent));
        //    var fieldsList = new List<string>();

        //    type.AddInterfaceImplementation(typeof(K));

        //    foreach (var v in typeof(K).GetProperties())
        //    {
        //        //Adds property to list of already added properties
        //        fieldsList.Add(v.Name);

        //        var field = type.DefineField("_" + v.Name.ToLower(), v.PropertyType, FieldAttributes.Private);
        //        var property = type.DefineProperty(v.Name, PropertyAttributes.None, v.PropertyType, new Type[0]);
        //        var getter = type.DefineMethod("get_" + v.Name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Virtual, v.PropertyType, new Type[0]);
        //        var setter = type.DefineMethod("set_" + v.Name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Virtual, null, new Type[] { v.PropertyType });

        //        var getGenerator = getter.GetILGenerator();
        //        var setGenerator = setter.GetILGenerator();

        //        getGenerator.Emit(OpCodes.Ldarg_0);
        //        getGenerator.Emit(OpCodes.Ldfld, field);
        //        getGenerator.Emit(OpCodes.Ret);

        //        setGenerator.Emit(OpCodes.Ldarg_0);
        //        setGenerator.Emit(OpCodes.Ldarg_1);
        //        setGenerator.Emit(OpCodes.Stfld, field);
        //        setGenerator.Emit(OpCodes.Ret);

        //        property.SetGetMethod(getter);
        //        property.SetSetMethod(setter);

        //        type.DefineMethodOverride(getter, v.GetGetMethod());
        //        type.DefineMethodOverride(setter, v.GetSetMethod());
        //    }

        //    if (source != null)
        //    {
        //        foreach (var v in source.GetType().GetProperties())
        //        {
        //            //If parent already implements property
        //            if (fieldsList.Contains(v.Name))
        //            {
        //                continue;
        //            }
        //            //Add property to list
        //            fieldsList.Add(v.Name);

        //            var field = type.DefineField("_" + v.Name.ToLower(), v.PropertyType, FieldAttributes.Private);

        //            var property = type.DefineProperty(v.Name, PropertyAttributes.None, v.PropertyType, new Type[0]);
        //            var getter = type.DefineMethod("get_" + v.Name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Virtual, v.PropertyType, new Type[0]);
        //            var setter = type.DefineMethod("set_" + v.Name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Virtual, null, new Type[] { v.PropertyType });

        //            var getGenerator = getter.GetILGenerator();
        //            var setGenerator = setter.GetILGenerator();

        //            getGenerator.Emit(OpCodes.Ldarg_0);
        //            getGenerator.Emit(OpCodes.Ldfld, field);
        //            getGenerator.Emit(OpCodes.Ret);

        //            setGenerator.Emit(OpCodes.Ldarg_0);
        //            setGenerator.Emit(OpCodes.Ldarg_1);
        //            setGenerator.Emit(OpCodes.Stfld, field);
        //            setGenerator.Emit(OpCodes.Ret);
        //            property.SetGetMethod(getter);
        //            property.SetSetMethod(setter);
        //        }
        //    }
        //    var newType = type.CreateType();
        //    var newObject = Activator.CreateInstance(type.CreateType());
        //    return source == null ? (K)newObject : CopyValues(source, (K)newObject);
        //}
        #endregion

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
    }
}