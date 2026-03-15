#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// SizeF Converter.
    /// </summary>
    public class SizeFConverter : System.ComponentModel.TypeConverter
    {
        #region Class overrides
        /// <summary>
        /// Converts the given value object
        /// to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"/> object. If <see langword="null"/> is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
        /// <param name="destinationType">The <see cref="T:System.Type"/> to convert the <paramref name="value"/> parameter to.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents
        /// the converted value.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="destinationType"/> parameter is <see langword="null"/>.</exception>
        /// <exception cref="T:System.NotSupportedException">The conversion could not be performed.</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is SizeF)
            {
                SizeF sz = (SizeF)value;
                return sz.Width.ToString() + "; " + sz.Height.ToString();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"/> to use as the current culture.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents
        /// the converted value.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The conversion could not be performed.</exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            // Convert from custom string to SizeF
            SizeF sizeResult = SizeF.Empty;

            if (value is string)
            {
                Regex regex = new Regex("(; |[;])", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string[] matches = regex.Split((string)value);
               
                // Mathches: "100.0; 200.0" -> [0]"100.0", [1]"; ", [2]"200.0"
                if (matches.Length >= 3)
                {
                    sizeResult.Width = float.Parse(matches[0]);
                    sizeResult.Height = float.Parse(matches[2]);
                }
                else
                {
                    // throw exeption on fail convert string to SizeF.
                    this.GetConvertFromException(value);
                }
            }
            else
            {
                sizeResult = (SizeF)base.ConvertFrom(context, culture, value);
            }

            return sizeResult;
        }

        /// <summary>
        /// Returns whether this object supports properties, using the
        /// specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <returns>
        /// <see langword="true "/>if <see cref="M:System.ComponentModel.TypeConverter.GetProperties(System.Object)"/> should be called to find the properties
        /// of this object; otherwise, <see langword="false"/>.
        /// </returns>
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Returns a collection of properties for
        /// the type of array specified by the value parameter, using the specified context and
        /// attributes.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="value">An <see cref="T:System.Object"/> that specifies the type of array for which to get properties.</param>
        /// <param name="attributes">An array of type <see cref="T:System.Attribute"/> that is used as a filter.</param>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.PropertyDescriptorCollection"/> with the properties that are exposed for
        /// this data type, or <see langword="null "/>if there are no properties.
        /// </returns>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            System.Attribute[] attrs = new System.Attribute[]
            {
                new System.ComponentModel.BrowsableAttribute(true)
            };
            PropertyDescriptorCollection propDesc = TypeDescriptor.GetProperties(value, attrs);

            return propDesc;
        }

        /// <summary>
        /// Returns
        /// whether this converter can convert an object of the given type to the type of this converter, using
        /// the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type"/> that represents the type you want to convert from.</param>
        /// <returns>
        /// <see langword="true "/>if this converter can perform the conversion; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }
        #endregion
    }
}
