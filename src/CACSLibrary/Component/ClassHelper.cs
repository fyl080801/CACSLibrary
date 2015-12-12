using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace CACSLibrary.Component
{
    /// <summary>
    /// 
    /// </summary>
    public class ClassHelper
    {
        private ClassHelper()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static object CreateInstance(Type t)
        {
            return Activator.CreateInstance(t);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="className"></param>
        /// <param name="lcpi"></param>
        /// <returns></returns>
        public static object CreateInstance(string className, List<CustomPropertyInfo> lcpi)
        {
            Type type = ClassHelper.BuildType(className);
            type = ClassHelper.AddProperty(type, lcpi);
            return Activator.CreateInstance(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lcpi"></param>
        /// <returns></returns>
        public static object CreateInstance(List<CustomPropertyInfo> lcpi)
        {
            return ClassHelper.CreateInstance("DefaultClass", lcpi);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classInstance"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertSetValue"></param>
        public static void SetPropertyValue(object classInstance, string propertyName, object propertSetValue)
        {
            classInstance.GetType().InvokeMember(propertyName, BindingFlags.SetProperty, null, classInstance, new object[]
			{
				Convert.ChangeType(propertSetValue, propertSetValue.GetType())
			});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classInstance"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static object GetPropertyValue(object classInstance, string propertyName)
        {
            return classInstance.GetType().InvokeMember(propertyName, BindingFlags.GetProperty, null, classInstance, new object[0]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Type BuildType()
        {
            return ClassHelper.BuildType("DefaultClass");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public static Type BuildType(string className)
        {
            AppDomain domain = Thread.GetDomain();
            AssemblyName assemblyName = new AssemblyName();
            assemblyName.Name = "MyDynamicAssembly";
            AssemblyBuilder assemblyBuilder = domain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, assemblyName.Name + ".dll");
            TypeBuilder typeBuilder = moduleBuilder.DefineType(className, TypeAttributes.Public);
            return typeBuilder.CreateType();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classType"></param>
        /// <param name="lcpi"></param>
        /// <returns></returns>
        public static Type AddProperty(Type classType, List<CustomPropertyInfo> lcpi)
        {
            ClassHelper.MergeProperty(classType, lcpi);
            return ClassHelper.AddPropertyToType(classType, lcpi);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classType"></param>
        /// <param name="cpi"></param>
        /// <returns></returns>
        public static Type AddProperty(Type classType, CustomPropertyInfo cpi)
        {
            List<CustomPropertyInfo> list = new List<CustomPropertyInfo>();
            list.Add(cpi);
            ClassHelper.MergeProperty(classType, list);
            return ClassHelper.AddPropertyToType(classType, list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classType"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static Type DeleteProperty(Type classType, string propertyName)
        {
            List<CustomPropertyInfo> lcpi = ClassHelper.SeparateProperty(classType, new List<string>
			{
				propertyName
			});
            return ClassHelper.AddPropertyToType(classType, lcpi);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classType"></param>
        /// <param name="ls"></param>
        /// <returns></returns>
        public static Type DeleteProperty(Type classType, List<string> ls)
        {
            List<CustomPropertyInfo> lcpi = ClassHelper.SeparateProperty(classType, ls);
            return ClassHelper.AddPropertyToType(classType, lcpi);
        }

        private static void MergeProperty(Type t, List<CustomPropertyInfo> lcpi)
        {
            PropertyInfo[] properties = t.GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo propertyInfo = properties[i];
                CustomPropertyInfo item = new CustomPropertyInfo(propertyInfo.PropertyType.FullName, propertyInfo.Name);
                lcpi.Add(item);
            }
        }

        private static List<CustomPropertyInfo> SeparateProperty(Type t, List<string> ls)
        {
            List<CustomPropertyInfo> list = new List<CustomPropertyInfo>();
            PropertyInfo[] properties = t.GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo propertyInfo = properties[i];
                foreach (string current in ls)
                {
                    if (propertyInfo.Name != current)
                    {
                        CustomPropertyInfo item = new CustomPropertyInfo(propertyInfo.PropertyType.FullName, propertyInfo.Name);
                        list.Add(item);
                    }
                }
            }
            return list;
        }

        private static void AddPropertyToTypeBuilder(TypeBuilder myTypeBuilder, List<CustomPropertyInfo> lcpi)
        {
            MethodAttributes attributes = MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.SpecialName;
            foreach (CustomPropertyInfo current in lcpi)
            {
                FieldBuilder fieldBuilder = myTypeBuilder.DefineField(current.FieldName, Type.GetType(current.Type), FieldAttributes.Private);
                fieldBuilder.SetConstant("11111111");
                PropertyBuilder propertyBuilder = myTypeBuilder.DefineProperty(current.PropertyName, PropertyAttributes.HasDefault, Type.GetType(current.Type), null);
                propertyBuilder.SetConstant("111111111");
                MethodBuilder methodBuilder = myTypeBuilder.DefineMethod(current.GetPropertyMethodName, attributes, Type.GetType(current.Type), Type.EmptyTypes);
                ILGenerator iLGenerator = methodBuilder.GetILGenerator();
                try
                {
                    iLGenerator.Emit(OpCodes.Ldarg_0);
                    iLGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
                    iLGenerator.Emit(OpCodes.Ret);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                MethodBuilder methodBuilder2 = myTypeBuilder.DefineMethod(current.SetPropertyMethodName, attributes, null, new Type[]
				{
					Type.GetType(current.Type)
				});
                ILGenerator iLGenerator2 = methodBuilder2.GetILGenerator();
                iLGenerator2.Emit(OpCodes.Ldarg_0);
                iLGenerator2.Emit(OpCodes.Ldarg_1);
                iLGenerator2.Emit(OpCodes.Stfld, fieldBuilder);
                iLGenerator2.Emit(OpCodes.Ret);
                propertyBuilder.SetGetMethod(methodBuilder);
                propertyBuilder.SetSetMethod(methodBuilder2);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classType"></param>
        /// <param name="lcpi"></param>
        /// <returns></returns>
        public static Type AddPropertyToType(Type classType, List<CustomPropertyInfo> lcpi)
        {
            AppDomain domain = Thread.GetDomain();
            AssemblyName assemblyName = new AssemblyName();
            assemblyName.Name = "MyDynamicAssembly";
            AssemblyBuilder assemblyBuilder = domain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, assemblyName.Name + ".dll");
            TypeBuilder typeBuilder = moduleBuilder.DefineType(classType.FullName, TypeAttributes.Public);
            ClassHelper.AddPropertyToTypeBuilder(typeBuilder, lcpi);
            return typeBuilder.CreateType();
        }
    }
}
