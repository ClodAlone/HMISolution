//-------------------------------------------------------------------------------------------------
// <copyright file="GridStyleInfoConverter.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.ComponentModel.Design.Serialization;
using Syncfusion.Styles;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    ///    <para>Provides
    ///       a type converter to convert expandable objects to and from various
    ///       other representations.</para>
    /// </summary>
    [TypeConverter(typeof(GridStyleInfoConverter))]
    public class GridStyleInfoConverter :
        StyleInfoBaseConverter
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridStyleInfoConverter()
            : base()
        {
        }

        /// <override/>
        /// <summary>
        /// Indicates whether this converter can convert an object to the specified destination type.
        /// </summary>
        /// <param name="context">Format context.</param>
        /// <param name="destinationType">The type you want to convert to.</param>
        /// <returns>True if this conversion is possible; False otherwise.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
            {
                return true;
            }
            else
            {
                return base.CanConvertTo(context, destinationType);
            }
        } // end of method CanConvertTo

        /// <override/>
        /// <summary>
        /// Converts the givne value object to the specified destination type.
        /// </summary>
        /// <param name="context">Format context.</param>
        /// <param name="culture">Current culture information.</param>
        /// <param name="value">The object to convert.</param>
        /// <param name="destinationType">Type to convert the value parameter to.</param>
        /// <returns>Converted object.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value != null && destinationType == typeof(InstanceDescriptor))
            {
                ConstructorInfo constructorInfo = value.GetType().GetConstructor(new Type[] { });
                if (constructorInfo != null)
                {
                    return new InstanceDescriptor(constructorInfo, null, false);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        } // end of method ConvertTo
    }
}
