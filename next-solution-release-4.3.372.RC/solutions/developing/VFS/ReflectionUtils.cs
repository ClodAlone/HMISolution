using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;

namespace VFS
{
    public static class ReflectionUtils
    {
        private static Dictionary<TypePropertyPair, PropertyDescriptor> propertyDescriptorsCache =
            new Dictionary<TypePropertyPair, PropertyDescriptor>();
        public static Type GetNonPublicTypeFromAssembly(string assemblyName, string typeName)
        {
            Assembly assembly = DevExpress.Data.Utils.Helpers.LoadWithPartialName(assemblyName);
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                if (type.Name == typeName)
                    return type;
            }
            return null;
        }
        public static T GetNonPublicInstanceFieldValue<T>(object source, string fieldName)
        {
            return (T)GetNonPublicInstanceFieldValue(source, fieldName);
        }
        public static PropertyDescriptorCollection GetProperties(object source)
        {
            return TypeDescriptor.GetProperties(source);
        }
        public static object GetNonPublicInstancePropertyValue(object source, string propertyName)
        {
            BindingFlags binding = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            PropertyInfo propertyInfo = source.GetType().GetProperty(propertyName, binding);
            return propertyInfo.GetValue(source, null);
        }
        public static object GetPropertyValue(object source, string propertyName)
        {
            object value;
            TryToGetPropertyValue(source, propertyName, out value);
            return value;
        }
        public static object GetPropertyValue(PropertyDescriptorCollection props, object source, string propertyName)
        {
            PropertyDescriptor p = props.Find(propertyName, true);
            return p == null ? null : p.GetValue(source);
        }
        public static void SetNonPublicInstanceFieldValue(object target, string fieldName, object fieldValue)
        {
            FindFieldInfo(target.GetType(), fieldName, BindingFlags.SetField | BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, fieldValue);
        }
        public static void SetNonPublicStaticFieldValue(Type type, string fieldName, object fieldValue)
        {
            FindFieldInfo(type, fieldName, BindingFlags.SetField | BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, fieldValue);
        }
        public static object GetNonPublicInstanceFieldValue(object target, string fieldName)
        {
            return FindFieldInfo(target.GetType(), fieldName, BindingFlags.SetField | BindingFlags.Instance | BindingFlags.NonPublic).GetValue(target);
        }
        public static void SetPropertyValue(object target, string propertyName, object propertyValue)
        {
            PropertyDescriptor descriptor = GetPropertyDescriptor(target, propertyName);
            descriptor.SetValue(target, propertyValue);
        }
        public static bool IsPropertyExist(object obj, string propertyName)
        {
            return GetPropertyDescriptor(obj, propertyName) != null;
        }
        public static bool IsPropertyExist(PropertyDescriptorCollection props, string propertyName)
        {
            return props.Find(propertyName, true) != null;
        }
        public static object InvokeStaticMethod(Type type, string methodName, params object[] parameters)
        {
            return InvokeMethod(type, null, methodName, true, parameters);
        }
        public static object InvokeInstanceMethod(object target, string methodName, params object[] parameters)
        {
            return InvokeMethod(null, target, methodName, false, parameters);
        }
        private static object InvokeMethod(Type type, object target, string methodName, bool isStatic, params object[] parameters)
        {
            if (target == null && !isStatic)
                throw new NullReferenceException("target");
            Type targetType = isStatic ? type : target.GetType();
            BindingFlags bindingFlags = (isStatic ? BindingFlags.Static : BindingFlags.Instance) | BindingFlags.Public | BindingFlags.NonPublic;
            MethodInfo method = FindMethodInfo(targetType, methodName, bindingFlags, parameters);
            if (method == null)
                throw new InvalidOperationException("Method not found");
            return method.Invoke(isStatic ? null : target, parameters);
        }
        private static MethodInfo FindMethodInfo(Type targetType, string methodName, BindingFlags bindingFlags, object[] parameters)
        {
            MethodInfo[] candidateMethods = Array.FindAll(targetType.GetMethods(bindingFlags), delegate(MethodInfo candidate)
            {
                if (candidate.Name != methodName)
                    return false;
                ParameterInfo[] cndidateParameters = candidate.GetParameters();
                if (cndidateParameters.Length != parameters.Length)
                    return false;
                for (int i = 0; i < cndidateParameters.Length; i++)
                    if (!cndidateParameters[i].ParameterType.IsAssignableFrom(parameters[i].GetType()))
                        return false;
                return true;
            });
            if (candidateMethods.Length > 1)
                throw new AmbiguousMatchException();
            return candidateMethods.Length == 0 ? null : candidateMethods[0];
        }
        public static bool TryToGetPropertyValue(object source, string propertyName, out object value)
        {
            if (IsObjectTypeNonCacheable(source))
                return TryToGetPropertyValueNonCacheable(source, propertyName, out value);
            return TryToExtractValueFromDescriptor(source, GetPropertyDescriptor(source, propertyName), out value);
        }
        private static FieldInfo FindFieldInfo(Type type, string fieldName, BindingFlags binding)
        {
            FieldInfo fieldInfo = null;
            while (fieldInfo == null && type != typeof(object))
            {
                fieldInfo = type.GetField(fieldName, binding);
                type = type.BaseType;
            }
            return fieldInfo;
        }
        private static bool IsObjectTypeNonCacheable(object obj)
        {
            return obj is ICustomTypeDescriptor;
        }
        private static PropertyDescriptor GetPropertyDescriptor(object obj, string propertyName)
        {
            if (IsObjectTypeNonCacheable(obj))
                return GetPropertyDescriptorNonCacheable(obj, propertyName);
            TypePropertyPair key = new TypePropertyPair(obj != null ? obj.GetType() : null, propertyName);
            if (key.IsEmpty)
                return null;
            if (propertyDescriptorsCache.ContainsKey(key))
                return propertyDescriptorsCache[key];
            else
            {
                PropertyDescriptor descriptor = GetPropertyDescriptorNonCacheable(obj, propertyName);
                lock (propertyDescriptorsCache)
                {
                    if (!propertyDescriptorsCache.ContainsKey(key))
                        propertyDescriptorsCache.Add(key, descriptor);
                }
                return descriptor;
            }
        }
        private static bool TryToExtractValueFromDescriptor(object source, PropertyDescriptor descriptor, out object value)
        {
            if (descriptor != null)
            {
                value = descriptor.GetValue(source);
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }
        private static PropertyDescriptor GetPropertyDescriptorNonCacheable(object obj, string propertyName)
        {
            if (obj != null && !string.IsNullOrEmpty(propertyName))
            {
                if (propertyName.Contains("."))
                    return new DevExpress.Data.Access.ComplexPropertyDescriptorReflection(obj, propertyName);
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(obj);
                if (properties != null)
                    return properties.Find(propertyName, true);
            }
            return null;
        }
        private static bool IsPropertyExistNonCacheable(object obj, string propertyName)
        {
            return GetPropertyDescriptorNonCacheable(obj, propertyName) != null;
        }
        private static bool TryToGetPropertyValueNonCacheable(object source, string propertyName, out object value)
        {
            return TryToExtractValueFromDescriptor(source, GetPropertyDescriptorNonCacheable(source, propertyName), out value);
        }
        public static Type StripNullableType(Type type)
        {
            if (type == null)
                return null;
            Type underlying = Nullable.GetUnderlyingType(type);
            if (underlying != null)
                return underlying;
            return type;
        }
        #region Nested types
        private struct TypePropertyPair
        {
            private Type type;
            private string propertyName;
            public TypePropertyPair(Type type, string propertyName)
            {
                this.type = type;
                this.propertyName = propertyName;
            }
            public bool IsEmpty
            {
                get { return this.type == null || this.propertyName == null; }
            }
            public string PropertyName
            {
                get { return this.propertyName; }
            }
            public Type Type
            {
                get { return this.type; }
            }
            public override bool Equals(object obj)
            {
                if (obj is TypePropertyPair)
                {
                    TypePropertyPair pair = (TypePropertyPair)obj;
                    if (pair.Type.Equals(this.Type) && pair.PropertyName == this.PropertyName)
                    {
                        return pair.GetType().Equals(base.GetType());
                    }
                }
                return false;
            }
            public override int GetHashCode()
            {
                return type.GetHashCode() ^ propertyName.GetHashCode();
            }
        }
        #endregion
    }
}
