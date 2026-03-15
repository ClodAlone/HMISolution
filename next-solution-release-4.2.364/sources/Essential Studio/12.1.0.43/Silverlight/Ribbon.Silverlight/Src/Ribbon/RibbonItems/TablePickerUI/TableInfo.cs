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

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// 
    /// </summary>
    [TypeConverter(typeof(TableInfoXamlConverter))]
    public class TableInfo 
    {
        /// <summary>
        /// 
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return (Row + 1).ToString() + "X" + (Column +1).ToString() + " Table";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TableInfoXamlConverter : TypeConverter
    {
        /// <summary>
        /// Returns whether the type converter can convert an object from the specified type to the type of this converter.
        /// </summary>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        /// <param name="context">An object that provides a format context.</param><param name="sourceType">The type you want to convert from.</param>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Converts from the specified value to the intended conversion type of the converter.
        /// </summary>
        /// <returns>
        /// The converted value.
        /// </returns>
        /// <param name="context">An object that provides a format context. </param><param name="culture">The <see cref="T:System.Globalization.CultureInfo"/> to use as the current culture. </param><param name="value">The value to convert to the type of this converter.</param><exception cref="T:System.NotImplementedException"><see cref="M:System.ComponentModel.TypeConverter.ConvertFrom(System.ComponentModel.ITypeDescriptorContext,System.Globalization.CultureInfo,System.Object)"/> not implemented in base <see cref="T:System.ComponentModel.TypeConverter"/>.</exception>
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value == null)
            {
                return new TableInfo();
            }

            if (value is string)
            {
                string returnvalue = value as string;
                if (returnvalue.Length >= 3)
                {
                    string[] args = returnvalue.Split(' ');
                    if (args.Length == 2)
                    {
                        return new TableInfo() { Row = Convert.ToInt16(args[0]), Column = Convert.ToInt16(args[1]) };
                    }
                    else
                    {
                        throw new ArgumentException("Attribute Value should contain row and column values.");
                    }
                }
                else
                {
                    throw new ArgumentException("Attribute Value should contain row and column values.");
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Converts the specified value object to the specified type.
        /// </summary>
        /// <returns>
        /// The converted object.
        /// </returns>
        /// <param name="context">An object that provides a format context. </param><param name="culture">The <see cref="T:System.Globalization.CultureInfo"/> to use as the current culture. </param><param name="value">The object to convert.</param><param name="destinationType">The type to convert the object to.</param><exception cref="T:System.NotImplementedException"><see cref="M:System.ComponentModel.TypeConverter.ConvertTo(System.ComponentModel.ITypeDescriptorContext,System.Globalization.CultureInfo,System.Object,System.Type)"/>  not implemented in base <see cref="T:System.ComponentModel.TypeConverter"/>.</exception>
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
