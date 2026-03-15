//-------------------------------------------------------------------------------------------------
// <copyright file="CustomTypeDescriptorConverter.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Globalization;

namespace Syncfusion.Grouping
{
    /// <summary>
    /// A ExpandableObjectConverter that lets users expand collections at design-time and list
    /// collection items similar to nested properties of a class.
    /// </summary>
    public class CustomTypeDescriptorConverter : ExpandableObjectConverter
    {
        /// <summary>Gets a collection of properties for the specified object type.</summary>
        /// <param name="context">Format context.</param>
        /// <param name="value">Array type for which to get the properties.</param>
        /// <param name="attributes">An array of type System.Attribute that will be used as a filter.</param>
        /// <returns>Property descriptor collection.</returns>
        /// <override/>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return ((ICustomTypeDescriptor)value).GetProperties(attributes);
        }

        /// <summary>Gets a value indicating whether this object supports properties using the specified context.</summary>
        /// <param name="context">Format context.</param>
        /// <returns>True when the properties are supported; False otherwise.</returns>
        /// <override/>
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return context == null || context.PropertyDescriptor == null
                || (context.Instance != null && GetCount(context.PropertyDescriptor.GetValue(context.Instance)) > 0);
        }

        /// <summary>Determines whether this object can be converted to the specified type, using the given format.</summary>
        /// <param name="context">Format context.</param>
        /// <param name="destinationType">Target type.</param>
        /// <returns>True if this conversion is supported; False otherwise.</returns>
        /// <override/>
        public override /*TypeConverter*/ bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }
            else
            {
                return base.CanConvertTo(context, destinationType);
            }
        }

        int GetCount(object value)
        {
            if (value == null)
            {
                return 0;
            }

            return ((ICustomTypeDescriptor)value).GetProperties(null).Count;
        }

        /// <summary>Converts the given value to the specified type, using the given format and culture.</summary>
        /// <param name="context">Format context.</param>
        /// <param name="culture">The CultureInfo.</param>
        /// <param name="value">The value.</param>
        /// <param name="destinationType">The Target type.</param>
        /// <returns>Converted object.</returns>
        /// <override/>
        public override /*TypeConverter*/ object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                int count = GetCount(value);
                return count > 0 ? String.Format("Count = {0}", count) : string.Empty;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        } // end of method ConvertTo
    }
}
