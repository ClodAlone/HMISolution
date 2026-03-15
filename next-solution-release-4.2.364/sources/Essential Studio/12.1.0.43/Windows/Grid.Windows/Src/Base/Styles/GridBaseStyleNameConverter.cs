//-------------------------------------------------------------------------------------------------
// <copyright file="GridBaseStyleNameConverter.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.ComponentModel;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// This is a <see cref="TypeConverter"/> for the <see cref="GridStyleInfo.BaseStyle"/>
    /// property of the <see cref="GridStyleInfo"/> class.
    /// </summary>
    /// <remarks>
    /// This calls for registered base styles in the <see cref="GridModel.BaseStylesMap"/>
    /// and displayed them in a drop-down list in a <see cref="PropertyGrid"/>.
    /// </remarks>
    public class GridBaseStyleNameConverter : TypeConverter
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridBaseStyleNameConverter()
            : base()
        {
        }

        /// <override/>
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the
        /// type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see
        /// cref="ITypeDescriptorContext" /> that provides a format
        /// context. </param>
        /// <param name="sourceType">A <see cref="Type" /> that represents the type
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
        /// <param name="context">Format context. </param>
        /// <param name="culture">Current culture information. </param>
        /// <param name="value">The object to convert. </param>     
        /// <returns>
        /// Converted value.
        /// </returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                string stringValue = (string)value;
                return stringValue;
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
        public override TypeConverter.StandardValuesCollection GetStandardValues(
            ITypeDescriptorContext context)
        {
            if (context != null)
            {
                GridStyleInfo style = context.Instance as GridStyleInfo;

                if (style != null)
                {
                    GridBaseStylesMap gsim = null;
                    GridStyleInfoIdentity gsid = style.Identity as GridStyleInfoIdentity;
                    if (gsid != null)
                    {
                        gsim = gsid.Data.BaseStylesMap;
                    }

                    GridBaseStyleIdentity gbsi = style.Identity as GridBaseStyleIdentity;
                    if (gbsi != null)
                    {
                        gsim = gbsi.BaseStylesMap;
                    }

                    if (gsim != null)
                    {
                        return new TypeConverter.StandardValuesCollection(gsim.baseStyles.Keys);
                    }
                }
            }

            return base.GetStandardValues(context);
        }

        /// <override/>
        /// <summary>
        /// Returns whether the collection of standard values returned from <see
        /// cref="M:System.ComponentModel.TypeConverter.GetStandardValues" /> is an
        /// exclusive list of possible values, using the specified context.
        /// </summary>
        /// <param name="context">An <see
        /// cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format
        /// context. </param>
        /// <returns>
        /// true if the <see
        /// cref="T:System.ComponentModel.TypeConverter.StandardValuesCollection" />
        /// returned from <see
        /// cref="M:System.ComponentModel.TypeConverter.GetStandardValues" /> is an
        /// exhaustive list of possible values; false if other values are possible.
        /// </returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }

        /// <override/>
        /// <summary>
        /// Returns whether this object supports a standard set of values that can be picked
        /// from a list, using the specified context.
        /// </summary>
        /// <param name="context">An <see
        /// cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format
        /// context. </param>
        /// <returns>
        /// true if <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues" />
        /// should be called to find a common set of values the object supports; otherwise,
        /// false.
        /// </returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
