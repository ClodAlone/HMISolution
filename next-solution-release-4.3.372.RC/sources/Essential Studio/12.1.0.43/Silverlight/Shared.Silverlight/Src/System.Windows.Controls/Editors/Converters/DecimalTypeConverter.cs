#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Globalization;


namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the Decimal Type Converters
    /// </summary>
    public class DecimalTypeConverter:TypeConverter
    {
        /// <summary>
        /// Returns whether the type converter can convert an object from the specified type to the type of this converter.
        /// </summary>
        /// <param name="context">An object that provides a format context.</param>
        /// <param name="sourceType">The type you want to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return Type.GetTypeCode(sourceType) == TypeCode.String;
        }
        /// <summary>
        /// Returns whether the type converter can convert an object to the specified type.
        /// </summary>
        /// <param name="context">An object that provides a format context.</param>
        /// <param name="destinationType">The type you want to convert to.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return Type.GetTypeCode(destinationType) == TypeCode.String ? true : TypeConverters.CanConvertTo<decimal?>(destinationType);
        }
        /// <summary>
        /// Converts from.
        /// </summary>
        /// <param name="typeDescriptorContext">The type descriptor context.</param>
        /// <param name="cultureInfo">The culture info.</param>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object source)
        {
            if (source == null)
            {
                return null;
            }

            string text = source as string;

            //if (text == null)
            //{
            //    string invalidCastMessage = string.Format(
            //        CultureInfo.CurrentCulture,
            //        Properties.Resources.TypeConverters_Convert_CannotConvert,
            //        GetType().Name,
            //        source,
            //        typeof(DateTime).Name);
            //    throw new InvalidCastException(invalidCastMessage);
            //}

            if (string.IsNullOrEmpty(text))
            {
                return null;
            }
            decimal result;
            if(decimal.TryParse(text,out result))
            {
              result = decimal.Parse(text);
            }
            return result;
       }
        /// <summary>
        /// Converts to.
        /// </summary>
        /// <param name="typeDescriptorContext">The type descriptor context.</param>
        /// <param name="cultureInfo">The culture info.</param>
        /// <param name="value">The value.</param>
        /// <param name="destinationType">Type of the destination.</param>
        /// <returns></returns>
        public override object ConvertTo(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value == null)
                {
                    return String.Empty;
                }
                else if (value is decimal)
                {
                    decimal val = (decimal)value;
                    return val.ToString();
                }
            }

            return TypeConverters.ConvertTo(this, value, destinationType);
        }
    }
}
