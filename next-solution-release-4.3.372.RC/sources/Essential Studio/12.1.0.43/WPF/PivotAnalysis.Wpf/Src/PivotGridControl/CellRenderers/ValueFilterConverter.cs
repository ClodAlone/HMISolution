#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows.Controls;
#if SILVERLIGHT

namespace Syncfusion.Silverlight.Controls.PivotGrid
#else 
namespace Syncfusion.Windows.Controls.PivotGrid
#endif
{
    /// <summary>
    /// Class used to draw the filter icon both filter Icon and filtered icon according to the given condition
    /// </summary>
    public class ValueFilterConverter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
           
#if SILVERLIGHT
            return " F1 M 181.926,180.872L 181.926,180.872C 182.055,180.872 182.16,180.671 182.16,180.419C 182.16,180.299 182.134,180.182 182.091,180.101L 178.264,174.316C 178.215,174.246 178.152,174.203 178.086,174.203C 178.019,174.203 177.957,174.246 177.908,174.316L 177.015,175.665L 174.081,180.101C 174.038,180.182 174.012,180.299 174.012,180.419C 174.012,180.671 174.116,180.872 174.245,180.872L 175.74,180.872L 180.429,180.872L 181.926,180.872 Z";
#else
            Button b = value as Button;
            var g1 = System.Windows.Media.VisualTreeHelper.GetParent(b);
            var g2 = System.Windows.Media.VisualTreeHelper.GetParent(g1);
            PivotSortHeaderCell grid = System.Windows.Media.VisualTreeHelper.GetParent(g2) as PivotSortHeaderCell;
            if (!grid.GridControlBase.GridControl.PivotEngine.CanFilterColumn(grid.CellIdentity.ColumnIndex))
                return null;
            //if(grid.GridControlBase.GridControl.RowPivotsOnly && grid.CellIdentity.ColumnIndex < grid.GridControlBase.GridControl.PivotRows.Count - 1)
            //    return null;
            string name = grid.GridControlBase.GridControl.PivotEngine.GetFieldNameAtIndex(grid.CellIdentity.ColumnIndex);
            if (ColumnFilterPopup.FilterPopUpCollection.ContainsKey(name))
            {
                return "M2.1299944,9.9798575L55.945994,9.9798575 35.197562,34.081179 35.197562,62.672859 23.428433,55.942383 23.428433,33.52121z M1.3001332,0L56.635813,0C57.355887,0,57.935946,0.5891428,57.935946,1.3080959L57.935946,2.8258877C57.935946,3.5448422,57.355887,4.133985,56.635813,4.133985L1.3001332,4.133985C0.58005941,4.133985,-2.3841858E-07,3.5448422,0,2.8258877L0,1.3080959C-2.3841858E-07,0.5891428,0.58005941,0,1.3001332,0z";
            }
            else
            {
                return "F1 M 383.146,268.922L 383.146,269.806L 384.029,269.806L 384.029,270.691L 384.932,270.691L 384.932,271.556L 385.798,271.556L 385.798,272.439L 386.7,272.439L 386.7,273.324L 387.565,273.324L 387.565,277.744L 391.12,277.744L 391.12,273.333L 392.004,273.333L 392.004,272.458L 392.868,272.458L 392.868,271.574L 393.754,271.574L 393.754,270.691L 394.637,270.691L 394.637,269.788L 395.521,269.788L 395.521,268.922L 383.146,268.922 Z ";
            }
 #endif
        }
  
        /// <summary>
        /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
