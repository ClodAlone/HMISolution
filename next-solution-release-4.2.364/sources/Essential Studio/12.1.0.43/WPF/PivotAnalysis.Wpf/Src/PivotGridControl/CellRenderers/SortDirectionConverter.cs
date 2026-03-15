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
using System.Windows;
#if SILVERLIGHT

namespace Syncfusion.Silverlight.Controls.PivotGrid
#else 
namespace Syncfusion.Windows.Controls.PivotGrid
#endif
{
    /// <summary>
    /// Converter class for sorting indicator direction
    /// </summary>
    public class SortDirectionConverter : IValueConverter
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
            PivotSortHeaderCell grid = value as PivotSortHeaderCell;
            if ((!grid.GridControlBase.GridControl.PivotEngine.IsColumnSorted(grid.CellIdentity.ColumnIndex)))
                return null;
            ListSortDirection dir = ListSortDirection.Ascending;
            if (grid.GridControlBase.GridControl.PivotEngine.UseIndexedEngine)
                dir = grid.GridControlBase.GridControl.PivotEngine.IndexEngine.SortDirection;
            else
                dir = grid.GridControlBase.GridControl.PivotEngine.GetSortDirection(grid.CellIdentity.ColumnIndex);
            if (dir == ListSortDirection.Ascending)
            {
#if SILVERLIGHT
                return " F1 M 181.926,180.872L 181.926,180.872C 182.055,180.872 182.16,180.671 182.16,180.419C 182.16,180.299 182.134,180.182 182.091,180.101L 178.264,174.316C 178.215,174.246 178.152,174.203 178.086,174.203C 178.019,174.203 177.957,174.246 177.908,174.316L 177.015,175.665L 174.081,180.101C 174.038,180.182 174.012,180.299 174.012,180.419C 174.012,180.671 174.116,180.872 174.245,180.872L 175.74,180.872L 180.429,180.872L 181.926,180.872 Z";
#else
                return "M 0 4 L 4 0 L 8 4 Z";
             
                
#endif
            }
            else if (dir == ListSortDirection.Descending)
            {
#if SILVERLIGHT
                return " F1 M 174.248,174.201L 174.248,174.201C 174.119,174.201 174.014,174.403 174.014,174.654C 174.014,174.774 174.039,174.891 174.082,174.972L 177.906,180.759C 177.956,180.83 178.018,180.873 178.084,180.873C 178.151,180.873 178.213,180.83 178.263,180.759L 179.156,179.411L 182.092,174.976C 182.135,174.896 182.161,174.778 182.161,174.659C 182.161,174.407 182.057,174.205 181.929,174.205L 180.434,174.204L 175.744,174.202L 174.248,174.201 Z";
#else
                return "M 0 0 L 4 4 L 8 0 Z";
                
#endif
            }
            return null;

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

    /// <summary>
    /// Converter class for filter checkbox visibility
    /// </summary>
    public class FilterCheckBoxVisibilityConverter: IValueConverter
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
            if (value == null)
               return Visibility.Collapsed;
            return Visibility.Visible;
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
