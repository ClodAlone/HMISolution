//-------------------------------------------------------------------------------------------------
// <copyright file="GridCellTypeNameConverter.cs" company="syncfusion">
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

using Syncfusion.Styles;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// <see cref="GridCellTypeNameConverter"/> implements a <see cref="TypeConverter"/>
    /// for <see cref="GridStyleInfo.CellType"/> of the <see cref="GridStyleInfo"/> class.
    /// </summary>
    /// <remarks>
    /// In its current implementation, the cell type name converter loads all cell
    /// types from the <see cref="GridModel.BaseStylesMap"/>. So, in order to make
    /// new cell types appear in a property grid, <see cref="GridModel.BaseStylesMap"/>
    /// provides a mechanism to register cell type names. TODO: Is this correct?
    /// <para/>
    /// <see cref="GridCellTypeNameConverter"/> returns a list of cell types. If you display
    /// a <see cref="GridStyleInfo"/> in a <see cref="PropertyGrid"/>, you will be able
    /// to drop-down a choicelist with cell types for the <see cref="GridStyleInfo.CellType"/> 
    /// property.
    /// </remarks>
    public class GridCellTypeNameConverter : TypeConverter
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridCellTypeNameConverter()
            : base()
        {
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
                return (string)value;
            }

            return base.ConvertFrom(context, culture, value);
        }

        string[] cellTypes = new string[] 
                {
                    "Header", "Static", "TextBox", "CheckBox",
                    "PushButton", "NumericUpDown", "DropDownGrid", 
                    "GridListControl", "ComboBox", "ColorEdit", "MonthCalendar", "FormulaCell",
                    "MaskEdit", "Currency", "Image",
                    "RichText", "Control", "OriginalTextBox", "ProgressBar", "RadioButton"
                };

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
                    GridBaseStylesMap gsim = this.GetStylesMap(style.Identity);

                    ArrayList al = new ArrayList();
                    if (gsim != null)
                    {
                        al.AddRange(gsim.CellTypes);
                    }
                    else
                    {
                        al.AddRange(cellTypes);
                    }

                    al.Sort();
                    return new TypeConverter.StandardValuesCollection(al);
                }
            }

            return base.GetStandardValues(context);
        }

        /// <summary>
        /// To get the base styles map.
        /// </summary>
        /// <param name="identity">cell style identifier.</param>
        /// <returns>An object of type GridBaseStylesMap.</returns>
        protected virtual GridBaseStylesMap GetStylesMap(StyleInfoIdentityBase identity)
        {
            GridBaseStylesMap gsim = null;
            GridStyleInfoIdentity gsid = identity as GridStyleInfoIdentity;
            if (gsid != null)
            {
                gsim = gsid.Data.BaseStylesMap;
            }

            GridBaseStyleIdentity gbsi = identity as GridBaseStyleIdentity;
            if (gbsi != null)
            {
                gsim = gbsi.BaseStylesMap;
            }

            return gsim;
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
        /// <returns>returns False.</returns>
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
        /// <returns>returns True.</returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
