#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Reflection;
#if !SILVERLIGHT && !WP7
using Syncfusion.Dynamic;
#endif
using System.ComponentModel;
#if !WP7
using Syncfusion.Dynamic;
#endif

namespace Syncfusion.Data.Extensions
{
    public static class ReflectionExtensions
    {
        public static IEnumerable<PropertyInfo> GetProperties(this Type type)
        {
#if WinRT
            return type.GetRuntimeProperties();
#else
            return type.GetProperties();
#endif
        }

        public static bool IsAssignableFrom(this Type type, Type typeInfo)
        {
#if WinRT
            return type.GetTypeInfo().IsAssignableFrom(typeInfo.GetTypeInfo());
#else
            return type.IsAssignableFrom(typeInfo);
#endif
        }

        public static Type BaseType(this Type type)
        {
#if WinRT
            return type.GetTypeInfo().BaseType;
#else
            return type.BaseType;
#endif
        }

        public static bool IsPrimitive(this Type type)
        {
#if WinRT
            return type.GetTypeInfo().IsPrimitive;
#else
            return type.IsPrimitive;
#endif
        }

        public static bool IsGenericType(this Type type)
        {
#if WinRT
            return type.GetTypeInfo().IsGenericType;
#else
            return type.IsGenericType;
#endif
        }

        public static bool IsInterface(this Type type)
        {
#if WinRT
            return type.GetTypeInfo().IsInterface;
#else
            return type.IsInterface;
#endif
        }

        public static bool IsAbstract(this Type type)
        {
#if WinRT
            return type.GetTypeInfo().IsAbstract;
#else
            return type.IsAbstract;
#endif
        }

        public static bool IsValueType(this Type type)
        {
#if WinRT
            return type.GetTypeInfo().IsValueType;
#else
            return type.IsValueType;
#endif
        }

        public static bool IsEnum(this Type type)
        {
#if WinRT
            return type.GetTypeInfo().IsEnum;
#else
            return type.IsEnum;
#endif
        }

        public static bool IsGenericTypeDefinition(this Type type)
        {
#if WinRT
            return type.GetTypeInfo().IsGenericTypeDefinition;
#else
            return type.IsGenericTypeDefinition;
#endif
        }

#if WinRT
        public static string GetCulture(this CultureInfo info)
        {
            return info.ToString();
        }
#else
        public static CultureInfo GetCulture(this CultureInfo info)
        {
            return info;
        }
#endif

#if WinRT

        public static IEnumerable<MethodInfo> GetMethods(this Type type)
        {
            return type.GetTypeInfo().DeclaredMethods;
        }

        public static IEnumerable<MemberInfo> GetMember(this Type type, string name)
        {
            return type.GetTypeInfo().DeclaredMembers;
        }

        public static MethodInfo GetMethod(this Type type, string name, Type[] types)
        {
            var results = from m in type.GetTypeInfo().DeclaredMethods
                          where m.Name == name
                          let methodParameters = m.GetParameters().Select(_ => _.ParameterType).ToArray()
                          where methodParameters.Length == types.Length &&
                                !methodParameters.Except(types).Any() &&
                                !types.Except(methodParameters).Any()
                          select m;

            return results.FirstOrDefault();
        }

        public static PropertyInfo GetProperty(this Type type, string name)
        {
            return type.GetRuntimeProperty(name);
            //return type.GetTypeInfo().GetRuntimeProperty(name);
        }

        public static Type[] GetGenericArguments(this Type type)
        {
            return type.GetTypeInfo().GenericTypeArguments;
        }
#endif

#if WPF || WinRT || SILVERLIGHT
        public static bool GetIsDynamicBound(this IEnumerable collection)
        {
            var enumerator = collection.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                return false;
            }

            var record = enumerator.Current;

            if (record != null)
            {
                var isDynamicBound = DynamicHelper.CheckIsDynamicObject(record.GetType());
                return isDynamicBound;
            }
            return false;
        }
#endif

#if WPF
        public static PropertyDescriptorCollection GetItemProperties(this IEnumerable dataSource, out bool ItemPropertiesSet)
#else
        public static PropertyInfoCollection GetItemProperties(this IEnumerable dataSource, out bool ItemPropertiesSet)
#endif
        {
            ItemPropertiesSet = false;
#if WPF
            PropertyDescriptorCollection itemProperties = null;
#else
            PropertyInfoCollection itemProperties = null;
#endif
            if (dataSource == null)
            {
#if WPF
                itemProperties = new PropertyDescriptorCollection(new PropertyDescriptor[0]);
#else
                itemProperties = PropertyInfoCollection.Empty;
#endif
            }
            else
            {
                var list = dataSource;
#if WPF
                if (dataSource is IListSource)
                    list = ((IListSource) dataSource).GetList();

                if (dataSource is ITypedList)
                {
                    itemProperties = ((ITypedList) (dataSource)).GetItemProperties(null);
                    ItemPropertiesSet = true;
                }
                else if (list is ITypedList)
                {
                    itemProperties = ((ITypedList) (list)).GetItemProperties(null);
                    ItemPropertiesSet = true;
                }
                else if (list != null)
                {
#endif
                var enumerator = list.GetEnumerator();
                if (enumerator == null)
                {
                    return null;
                }

                if (enumerator.MoveNext() && enumerator.Current!=null)
                {
#if WPF
                    itemProperties = TypeDescriptor.GetProperties(enumerator.Current);
#else
                    itemProperties = new PropertyInfoCollection(enumerator.Current.GetType());
#endif
                    ItemPropertiesSet = true;
                }
                else
                {
                    var prop = list.GetType().GetProperty("Item");
                    if (prop != null)
                    {
#if WPF
                        itemProperties = TypeDescriptor.GetProperties(prop.PropertyType);
#else
                        itemProperties = new PropertyInfoCollection(prop.PropertyType);
#endif
                        ItemPropertiesSet = true;
                    }
                }
            }
#if WPF
			}
#endif
            return itemProperties;
        }

    }
}