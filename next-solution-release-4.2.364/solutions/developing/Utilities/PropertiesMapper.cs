using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
#if !NET_STANDARD
using System.Windows.Data;
#endif
using System.Windows;

namespace Utilities
{
    public static class PropetiesMapper
    {
        /// <summary>
        /// Copies all the properties of the "from" object to this object if they exist.
        /// </summary>
        /// <param name="to">The object in which the properties are copied</param>
        /// <param name="from">The object which is used as a source</param>
        /// <param name="excludedProperties">Exclude these properties from the copy</param>
        public static void copyPropertiesFrom(this object to, object from, string[] excludedProperties)
        {
            Type targetType = to.GetType();
            Type sourceType = from.GetType();

            var list = new List<String>();
            PropertyInfo[] sourceProps = sourceType.GetProperties();
            foreach (var propInfo in sourceProps)
            {
                //filter the properties
                if (list.Contains(propInfo.Name) || excludedProperties != null &&  
                    excludedProperties.Contains(propInfo.Name))
                    continue;

                #if !NET_STANDARD
                if (from is DependencyObject)
                {
                    try
                    {
                        var depobj = from as DependencyObject;
                        var pdc = System.ComponentModel.TypeDescriptor.GetProperties(depobj);
                        var dpd = DependencyPropertyDescriptor.FromProperty(pdc[propInfo.Name]);
                        if (dpd != null)
                        {
                            var depValue = depobj.ReadLocalValue(dpd.DependencyProperty);
                            if (depValue is BindingExpression)
                                continue;
                        }
                    }
                    catch
                    {

                    }
                }
                #endif

                list.Add(propInfo.Name);

                //Get the matching property from the target
                PropertyInfo toProp =
                  (targetType == sourceType) ? propInfo : targetType.GetProperty(propInfo.Name);

                //If it exists and it's writeable
                if (toProp != null && toProp.CanWrite)
                {
                    //Copy the value from the source to the target
                    Object value = propInfo.GetValue(from, null);
                    try
                    {
                        toProp.SetValue(to, value, null);
                        System.Diagnostics.Debug.WriteLine(String.Format("Copy Properties of {0}", propInfo.Name));
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }
            }
        }

        /// <summary>
        /// Copies all the properties of the "from" object to this object if they exist.
        /// </summary>
        /// <param name="to">The object in which the properties are copied</param>
        /// <param name="from">The object which is used as a source</param>
        public static void copyPropertiesFrom(this object to, object from)
        {
            to.copyPropertiesFrom(from, null);
        }

        /// <summary>
        /// Copies all the properties of this object to the "to" object
        /// </summary>
        /// <param name="to">The object in which the properties are copied</param>
        /// <param name="from">The object which is used as a source</param>
        public static void copyPropertiesTo(this object from, object to)
        {
            to.copyPropertiesFrom(from, null);
        }

        /// <summary>
        /// Copies all the properties of this object to the "to" object
        /// </summary>
        /// <param name="to">The object in which the properties are copied</param>
        /// <param name="from">The object which is used as a source</param>
        /// <param name="excludedProperties">Exclude these properties from the copy</param>
        public static void copyPropertiesTo(this object from, object to, string[] excludedProperties)
        {
            to.copyPropertiesFrom(from, excludedProperties);
        }
    }
}
