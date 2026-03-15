//-------------------------------------------------------------------------------------------------
// <copyright file="GridBorderConverter.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.ComponentModel.Design.Serialization;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    ///      GridBorderConverter is a class that can be used to convert
    ///      border from one data type to another. Access this
    ///      class through the TypeDescriptor.
    /// </summary>
    public class GridBorderConverter : TypeConverter
    {
        /// <summary>
        /// Initializes a <see cref="GridBorderConverter"/> class.
        /// </summary>
        public GridBorderConverter()
        {
        }

        /// <override/>
        /// <summary>
        /// Returns whether this converter can convert the object to the specified type,
        /// using the specified context.
        /// </summary>
        /// <param name="context">An <see
        /// cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format
        /// context. </param>
        /// <param name="destinationType">A <see cref="T:System.Type" /> that represents the
        /// type you want to convert to. </param>       
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        /// <override/>
        /// <summary>
        /// Converts the given value object to the specified type, using the specified
        /// context and culture information.
        /// </summary>
        /// <param name="context">An <see
        /// cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format
        /// context. </param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo" />. If
        /// null is passed, the current culture is assumed. </param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert. </param>
        /// <param name="destinationType">The <see cref="T:System.Type" /> to convert the
        /// value parameter to. </param>       
        /// <returns>
        /// An <see cref="T:System.Object" /> that represents the converted value.
        /// </returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor) && value is GridBorder)
            {
                GridBorder border = value as GridBorder;

                ConstructorInfo constructorInfo = typeof(Syncfusion.Windows.Forms.Grid.GridBorder).GetConstructor(new Type[] { typeof(GridBorderStyle), typeof(Color), typeof(GridBorderWeight) });

                return new InstanceDescriptor(constructorInfo, new object[] { border.Style, border.Color, border.Weight }, false);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <override/>
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the
        /// type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see
        /// cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format
        /// context. </param>
        /// <param name="sourceType">A <see cref="T:System.Type" /> that represents the type
        /// you want to convert from. </param>       
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(
            ITypeDescriptorContext context,
            Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        /// <override/>
        /// <summary>
        /// Converts the given object to the type of this converter, using the specified
        /// context and culture information.
        /// </summary>
        /// <param name="context">An <see
        /// cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format
        /// context. </param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo" /> to
        /// use as the current culture. </param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert. </param>        
        /// <returns>
        /// An <see cref="T:System.Object" /> that represents the converted value.
        /// </returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                string stringValue = (string)value;
                return GridBorder.Parse(stringValue);
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <override/>
        /// <summary>
        /// Creates an instance of the type that this <see
        /// cref="T:System.ComponentModel.TypeConverter" /> is associated with, using the
        /// specified context, given a set of property values for the object.
        /// </summary>
        /// <param name="context">An <see
        /// cref="ITypeDescriptorContext" /> that provides a format
        /// context. </param>
        /// <param name="propertyValues">An <see cref="T:System.Collections.IDictionary" />
        /// of new property values. </param>
        /// <returns>
        /// An <see cref="Object" /> representing the given <see
        /// cref="T:System.Collections.IDictionary" />, or null if the object cannot be
        /// created. This method always returns null.
        /// </returns>
        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            Color color = Color.Black;
            GridBorderStyle style = GridBorderStyle.NotSet;
            GridBorderWeight weight = GridBorderWeight.Thin;

            if (propertyValues.Contains("Style"))
            {
                style = (GridBorderStyle)propertyValues["Style"];
            }

            if (propertyValues.Contains("Weight"))
            {
                weight = (GridBorderWeight)propertyValues["Weight"];
            }

            if (propertyValues.Contains("Color"))
            {
                color = (Color)propertyValues["Color"];
            }

            if (context != null && (style == GridBorderStyle.NotSet || style == GridBorderStyle.None || style == GridBorderStyle.Standard))
            {
                // Adjust Style setting if user has changed pattern or backcolor.
                object o = context.PropertyDescriptor.GetValue(context.Instance);
                if (o is GridBorder)
                {
                    GridBorder oldBorder = (GridBorder)o;
                    if (oldBorder.Style == style)
                    {
                        style = GridBorderStyle.Solid;
                    }
                }
            }

            if (style != GridBorderStyle.NotSet &&
                style != GridBorderStyle.None &&
                style != GridBorderStyle.Standard &&
                color == Color.Empty)
            {
                color = SystemColors.WindowFrame;
            }

            return new GridBorder(style, color, weight);
        }

        /// <override/>
        /// <summary>
        /// Returns whether changing a value on this object requires a call to <see
        /// cref="TypeConverter.CreateInstance(System.Collections.IDictionary)"
        /// /> to create a new value, using the specified context.
        /// </summary>
        /// <param name="context">An <see
        /// cref="ITypeDescriptorContext" /> that provides a format
        /// context. </param>
        /// <returns>
        /// true if changing a property on this object requires a call to <see
        /// cref="TypeConverter.CreateInstance(System.Collections.IDictionary)"
        /// /> to create a new value; otherwise, false. Always returns true.
        /// </returns>
        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <override/>
        /// <summary>
        /// Returns a collection of preoperties for the type specified.
        /// </summary>
        /// <param name="context">Format context.</param>
        /// <param name="value">The value Type.</param>
        /// <param name="attributes">An array of System.Attribute that will be used as a filter.</param>
        /// <returns>Property descriptor collection.</returns>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            System.ComponentModel.PropertyDescriptorCollection pds = TypeDescriptor.GetProperties(typeof(GridBorder), attributes);

            string[] atts = new string[]
            {
                "Style",
                "Color",
                "Weight",
            };

            return pds.Sort(atts);
        }

        /// <override/>
        /// <summary>
        /// Returns whether this object supports properties, using the specified context.
        /// </summary>
        /// <param name="context">Format context. </param>
        /// <returns>
        /// true if this method should be called to find the properties of this object; otherwise, false.
        /// </returns>
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
