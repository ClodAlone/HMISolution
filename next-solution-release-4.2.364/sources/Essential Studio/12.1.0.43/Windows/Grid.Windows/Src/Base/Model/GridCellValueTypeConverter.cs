//-------------------------------------------------------------------------------------------------
// <copyright file="GridCellValueTypeConverter.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Implements a <see cref="TypeConverter"/> for the <see cref="GridStyleInfo.CellValueType"/> property in
    /// <see cref="GridStyleInfo"/>.
    /// </summary>
    public class GridCellValueTypeConverter: TypeConverter
    {
        /// <override/>
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the
        /// type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">Format context. </param>
        /// <param name="sourceType">The type
        /// you want to convert from. </param>       
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)  
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
        /// <param name="context">Format context. </param>
        /// <param name="culture">Current culture information. </param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert. </param>        
        /// <returns>
        /// An <see cref="T:System.Object" /> that represents the converted value.
        /// </returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)  
        {
            if (value is string)
            {
                if (((string)value).Length > 0)
                {
                    return Type.GetType((string)value);
                }
                else
                {
                    return null;
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

        //// no string conversion

        /// <override/>
        /// <summary>
        /// Converts the given value object to the specified type, using the specified
        /// context and culture information.
        /// </summary>
        /// <param name="context">Format context. </param>
        /// <param name="culture">Current culture information. </param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert. </param>
        /// <param name="destinationType">The type to convert the
        /// value parameter to. </param>        
        /// <returns>
        /// An <see cref="T:System.Object" /> that represents the converted value.
        /// </returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)  
        {
            if (destinationType == typeof(string))
            {
                Type type = (Type) value;
                if (type == null)
                {
                    return String.Empty;
                }
                else if (type.Namespace == "System")
                {
                    return type.ToString();
                }
                else
                {
                    return String.Concat(type.FullName, ",", type.AssemblyQualifiedName.Split(',')[1]);
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <override/>
        /// <summary>
        /// Returns a collection of standard values for the data type this type converter is
        /// designed for when provided with a format context.
        /// </summary>
        /// <param name="context">An <see
        /// cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format
        /// context that can be used to extract additional information about the environment
        /// from which this converter is invoked. This parameter or properties of this
        /// parameter can be null. </param>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.TypeConverter.StandardValuesCollection" />
        /// that holds a standard set of valid values, or null if the data type does not
        /// support a standard set of values.
        /// </returns>
        public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)  
        {
            return svc;
        }

        /// <override/>
        /// <summary>
        /// Returns whether the collection of standard values returned from <see
        /// cref="M:System.ComponentModel.TypeConverter.GetStandardValues" /> is an
        /// exclusive list of possible values, using the specified context.
        /// </summary>
        /// <param name="context">Format context. </param>
        /// <returns>returns False. </returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)  
        {
            return false;
        }

        /// <override/>
        /// <summary>
        /// Returns whether this object supports a standard set of values that can be picked
        /// from a list, using the specified context.
        /// </summary>
        /// <param name="context">Format context. </param>
        /// <returns>returns True. </returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)  
        {
            return true;
        }

        /// <summary>
        /// Static Constructor.
        /// </summary>
        static GridCellValueTypeConverter()
        {
            values = new string[] 
                {
                    "System.String",
                    "System.Double",
                    "System.Int32",
                    "System.Boolean",
                    ////"System.Drawing.Color, System.Drawing",
                    "System.DateTime",
                    "System.Int16",
                    "System.Int64",
                    "System.Single",
                    "System.Byte",
                    "System.Char",
                    "System.Decimal",
                    "System.UInt16",
                    "System.UInt32",
                    "System.UInt64",
                    ////"System.Windows.Forms.DockStyle, System.Windows.Forms",
                /*typeof(System.String),
                    typeof(System.Double),
                    typeof(System.Int32),
                    typeof(System.Boolean),
                    typeof(System.Drawing.Color),
                    typeof(System.DateTime),
                    typeof(System.Int16),
                    typeof(System.Int64),
                    typeof(System.Single),
                    typeof(System.SByte),
                    typeof(System.Byte),
                    typeof(System.Char),
                    typeof(System.Decimal),
                    typeof(System.DBNull),
                    typeof(System.UInt16),
                    typeof(System.UInt32),
                    typeof(System.UInt64),*/
            };
            Array.Sort(values);
            Type[] types = new Type[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                types[i] = Type.GetType(values[i]);
            }

            svc = new TypeConverter.StandardValuesCollection(types);
        }

        private static string[] values;
        private static TypeConverter.StandardValuesCollection svc;
    }
}