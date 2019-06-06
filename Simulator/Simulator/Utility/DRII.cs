using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Simulator.Interfaces;
using Simulator.Implementations;
using System.Collections.Concurrent;

namespace Simulator.Utility
{
    public static class DRII<ISysComponent>
    {
        public static readonly BindingFlags visibilityFlags = BindingFlags.Public | BindingFlags.Instance;
        private static Dictionary<Type, Type> Implementations = new Dictionary<Type, Type>();

        public static object DynamicInterfaceImplementation(Type Interface, SysComponent source)
        {
            var name = Guid.NewGuid().ToString();
            AssemblyBuilder assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(name), AssemblyBuilderAccess.Run);
            ModuleBuilder module = assembly.DefineDynamicModule(name);

            TypeBuilder type = module.DefineType(typeof(SysComponent).Name + "_" + Interface.Name, TypeAttributes.Public, typeof(SysComponent));
            var fieldsList = new List<string>();

            type.AddInterfaceImplementation(Interface);
            foreach (var v in Interface.GetProperties())
            {
                //Adds property to list of already added properties
                fieldsList.Add(v.Name);

                var field = type.DefineField("_" + v.Name.ToLower(), v.PropertyType, FieldAttributes.Private);
                var property = type.DefineProperty(v.Name, PropertyAttributes.None, v.PropertyType, new Type[0]);
                var getter = type.DefineMethod("get_" + v.Name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Virtual, v.PropertyType, new Type[0]);
                var setter = type.DefineMethod("set_" + v.Name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.Virtual, null, new Type[] { v.PropertyType });

                var getGenerator = getter.GetILGenerator();
                var setGenerator = setter.GetILGenerator();

                getGenerator.Emit(OpCodes.Ldarg_0);
                getGenerator.Emit(OpCodes.Ldfld, field);
                getGenerator.Emit(OpCodes.Ret);

                setGenerator.Emit(OpCodes.Ldarg_0);
                setGenerator.Emit(OpCodes.Ldarg_1);
                setGenerator.Emit(OpCodes.Stfld, field);
                setGenerator.Emit(OpCodes.Ret);

                property.SetGetMethod(getter);
                property.SetSetMethod(setter);

                if(v.GetGetMethod() != null)
                { 
                type.DefineMethodOverride(getter, v.GetGetMethod());
                }
                if (v.GetSetMethod() != null)
                    {
                    type.DefineMethodOverride(setter, v.GetSetMethod());
                    }
            }
            Type newType = type.CreateType();
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