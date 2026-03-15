#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Provides a way to show list of font family names in subproperties.
    /// </summary>
    public class FontFamilyConverter : CollectionConverter
    {
        #region Class overrides
        /// <summary>
        /// Returns whether this object
        /// supports a standard set of values that can be picked
        /// from a list, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <returns>
        /// <see langword="true"/> if <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues"/> should be
        /// called to find a common set of values the object supports; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Returns whether the collection of standard values returned from
        /// <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues"/> is an exclusive
        /// list of possible values, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <returns>
        /// <see langword="true "/>if the
        /// <see cref="T:System.ComponentModel.TypeConverter.StandardValuesCollection"/>
        /// returned from <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues"/> is an exhaustive list of
        /// possible values; <see langword="false "/>if other values are possible.
        /// </returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Returns a collection of standard values for the data type this type converter is designed for when provided with a
        /// format context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context that can be used to extract additional information about the environment from which this converter is invoked. This parameter or properties of this parameter can be <see langword="null"/> .</param>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.TypeConverter.StandardValuesCollection"/> that holds a standard set
        /// of valid values, or <see langword="null"/> if the data type does not support a
        /// standard set of values.
        /// </returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            ICollection families = GetAvailableFontFamilies(context.Instance as FontStyle);
            return new StandardValuesCollection(families);
        }

        /// <summary>
        /// Returns whether the given value object is valid for this type and for the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to test for validity.</param>
        /// <returns>
        /// <see langword="true "/>if the specified value is valid
        /// for this object; otherwise, <see langword="false"/>.
        /// </returns>
        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            ArrayList list = GetAvailableFontFamilies(context.Instance as FontStyle);
            return list.Contains(value);
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Get the available font families for current font style.
        /// </summary>
        /// <param name="fontStyle" >The font style <see cref="Syncfusion.Windows.Forms.Diagram.FontStyle"/>.</param>
        /// <returns>Collection of available font familie names.</returns>
        private ArrayList GetAvailableFontFamilies(FontStyle fontStyle)
        {
            ArrayList lstFontFamilies = new ArrayList();
            FontFamily[] arrFontFamilies = FontFamily.Families;
            int nLength = arrFontFamilies.Length;

            for (int i = 0; i < nLength; i++)
            {
                if (fontStyle == null || arrFontFamilies[i].IsStyleAvailable(fontStyle.Style))
                {
                    lstFontFamilies.Add(arrFontFamilies[i].Name);
                }
            }

            return lstFontFamilies;
        }
        #endregion
    }
}
