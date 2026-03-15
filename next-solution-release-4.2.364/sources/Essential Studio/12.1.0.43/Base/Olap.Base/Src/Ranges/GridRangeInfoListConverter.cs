#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Globalization;
using System.ComponentModel;

namespace Syncfusion.Olap.Engine
{
    /// <summary>
    ///      GridRangeInfoListConverter is a class that can be used to convert
    ///      a range list to a string or vice versa. Access this
    ///      class through the TypeDescriptor.
    /// </summary>
    public class GridRangeInfoListConverter : TypeConverter 
    {
        // TypeConvert.ConvertTo is implemented in base class (and calls object.ToString()).


        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type"/> that represents the type you want to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(
            ITypeDescriptorContext context, 
            Type sourceType) 
        {
            return (sourceType == typeof(string));
        }


        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"/> to use as the current culture.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)  
        {
            if (value is string) 
            {
                string stringValue = (string)value;
                return GridRangeInfoList.FromString(stringValue.Trim());
            }

            return base.ConvertFrom(context, culture, value);
        }
    } 
}
