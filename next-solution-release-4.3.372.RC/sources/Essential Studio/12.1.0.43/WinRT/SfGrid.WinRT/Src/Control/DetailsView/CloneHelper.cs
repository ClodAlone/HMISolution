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
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using Syncfusion.Data;
using Syncfusion.Data.Extensions;

namespace Syncfusion.UI.Xaml.Grid
{

    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class CloneableAttribute : Attribute
    {
        public CloneableAttribute(bool isCloneable)
        {
            this.IsCloneable = isCloneable;
        }

        public bool IsCloneable { get; private set; }
    }

    public static class CloneHelper
    {
#if WPF
        private static readonly Dictionary<Type, PropertyDescriptorCollection> PropertiesDescriptorCache = new Dictionary<Type, PropertyDescriptorCollection>();
#else
        private static readonly Dictionary<Type, PropertyInfoCollection> PropertiesDescriptorCache = new Dictionary<Type, PropertyInfoCollection>();
#endif
        
        public static void CloneCollection(IList sourceList, IList destinationList, Type baseType)
        {
            foreach (var sourceItem in sourceList)
            {
                var destinationItem = CreateClonedInstance(sourceItem, baseType);
                destinationList.Add(destinationItem);
            }
        }

        public static object CreateClonedInstance(object sourceItem, Type baseType)
        {
            var destinationItem = Activator.CreateInstance(sourceItem.GetType());
            CloneProperties(sourceItem, destinationItem, baseType);
            return destinationItem;
        }
        

        public static void CloneProperties(object source, object destination, Type baseType)
        {
            var propertiesDescriptor = GetCloneableProperties(destination.GetType(), baseType);
#if WPF
            foreach (PropertyDescriptor propertyDescriptor in propertiesDescriptor)
            {
#else
            foreach (var keyValuePair in propertiesDescriptor)
            {
                var propertyDescriptor = keyValuePair.Value;
#endif
                var destinationValue = propertyDescriptor.GetValue(destination);
                var sourceValue = propertyDescriptor.GetValue(source);
                if (!object.Equals(destinationValue, sourceValue))
                {
                    propertyDescriptor.SetValue(destination, sourceValue);
                }
            }
        }

#if WPF
        public static PropertyDescriptor GetCloneableProperty(Type type, Type baseType, string property)
#else
        public static PropertyInfo GetCloneableProperty(Type type, Type baseType, string property)
#endif
        {
            var descriptor = GetCloneableProperties(type, baseType);
#if !WPF
            return descriptor.ContainsKey(property) ? descriptor[property] : null;
#else
            return descriptor[property];
#endif
        }

#if WPF
        private static PropertyDescriptorCollection GetCloneableProperties(Type destinationType, Type baseType)
        {
            PropertyDescriptorCollection descriptor;
            if (!PropertiesDescriptorCache.TryGetValue(destinationType, out descriptor))
            {
                var list = new List<PropertyDescriptor>();
                foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(destinationType))
                {
                    var item = GetCloneableProperty(propertyDescriptor, baseType);
                    if (item != null)
                        list.Add(item);
                }
                descriptor = new PropertyDescriptorCollection(list.ToArray());
                PropertiesDescriptorCache.Add(destinationType, descriptor);
            }
            return descriptor;
        }
#else
        private static PropertyInfoCollection GetCloneableProperties(Type destinationType, Type baseType)
        {
            PropertyInfoCollection descriptor;
            if (!PropertiesDescriptorCache.TryGetValue(destinationType, out descriptor))
            {
                descriptor = new PropertyInfoCollection();
                foreach (var propertyInfo in destinationType.GetProperties())
                {
                    var item = GetCloneableProperty(propertyInfo, baseType);
                    if (item != null) 
                        descriptor.Add(item.Name, item);
                }
                PropertiesDescriptorCache.Add(destinationType, descriptor);
            }
            return descriptor;
        }
#endif

#if WPF
        private static PropertyDescriptor GetCloneableProperty(PropertyDescriptor property, Type baseType)
#else
        private static PropertyInfo GetCloneableProperty(PropertyInfo property, Type baseType)
#endif
        {
#if WPF
            var attribute = property.Attributes[typeof(CloneableAttribute)] as CloneableAttribute;
#else
            var attribute = property.GetCustomAttributes(typeof(CloneableAttribute), true).FirstOrDefault() as CloneableAttribute;
#endif
            if (attribute != null && !attribute.IsCloneable)
                return null;
#if WPF
            if (property.IsReadOnly)
#else
            if (!property.CanWrite)
#endif
                return null;
            if (property.Name == "Resources")
                return null;
#if WPF
            if (!baseType.IsAssignableFrom(property.ComponentType))
#else
            if (!baseType.IsAssignableFrom(property.DeclaringType))
#endif
                return null;

            return property;
        }


        public static void EnsureCollection<T, S>(T source, T target, Func<S, S, bool> predicate) where T : IList<S>
        {
            if (source.Count != target.Count)
            {
                target.Clear();
                CloneCollection((IList)source, (IList)target, typeof(S));
                return;
            }
            if (!source.Any(sourceitem => target.All(targetitem => predicate(targetitem, sourceitem))))
                return;
            target.Clear();
            CloneCollection((IList)source, (IList)target, typeof(S));
        }
    }
}
