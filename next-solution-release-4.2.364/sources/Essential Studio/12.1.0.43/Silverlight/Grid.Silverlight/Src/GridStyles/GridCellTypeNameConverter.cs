#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using Syncfusion.Windows.Styles;
using Syncfusion.Windows.Controls.Grid;

namespace Syncfusion.Windows.Controls.Grid
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

        /// <override/>
        public override bool CanConvertFrom(
            ITypeDescriptorContext context,
            Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        /// <override/>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                return (string)value;
            }

            return base.ConvertFrom(context, culture, value);
        }
      
    }
}
