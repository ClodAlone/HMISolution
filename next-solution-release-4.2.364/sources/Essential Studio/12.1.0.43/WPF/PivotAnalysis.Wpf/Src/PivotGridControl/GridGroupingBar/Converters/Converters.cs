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
using System.Windows;

#if SILVERLIGHT
using Syncfusion.PivotAnalysis.Base.Silverlight;
using System.Windows.Media;
namespace Syncfusion.Silverlight.Controls.PivotGrid
#else
using Syncfusion.PivotAnalysis.Base;
using Syncfusion.Windows.Controls.PivotSchemaDesigner;
namespace Syncfusion.Windows.Controls.PivotGrid
#endif
{

    #if !SILVERLIGHT
    /// <summary>
    /// Converter class for ToggleButton 
    /// </summary>
    public class ToggleButtonCheckConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value is FilterItemElement)
            {
                FilterItemElement element = value as FilterItemElement;
                if (element.Key ==  PivotGridConstants.AllString)
                {
                    return true;
                }
                else
                    return false;
            }
            else
            {
                PivotItem pivotItem = value as PivotItem;
                if (pivotItem != null)
                {
                    if (pivotItem.Comparer == null || (!(pivotItem.Comparer is ReverseCustomComparer) && !(pivotItem.Comparer is ReverseOrderComparer)))
                    {
                        return false;
                    }
                    else
                        return true;
                }
            }
            return true;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

#endif
    /// <summary>
    /// Converter  class for Path
    /// </summary>
    public class PathConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            PivotItem item = value as PivotItem;

            if (item != null)
            {
                if (item.Comparer == null)
                {
                    return Common.GetPathGeometry(" F1 M 181.926,180.872L 181.926,180.872C 182.055,180.872 182.16,180.671 182.16,180.419C 182.16,180.299 182.134,180.182 182.091,180.101L 178.264,174.316C 178.215,174.246 178.152,174.203 178.086,174.203C 178.019,174.203 177.957,174.246 177.908,174.316L 177.015,175.665L 174.081,180.101C 174.038,180.182 174.012,180.299 174.012,180.419C 174.012,180.671 174.116,180.872 174.245,180.872L 175.74,180.872L 180.429,180.872L 181.926,180.872 Z");
                }
                else
                    return Common.GetPathGeometry(" F1 M 174.248,174.201L 174.248,174.201C 174.119,174.201 174.014,174.403 174.014,174.654C 174.014,174.774 174.039,174.891 174.082,174.972L 177.906,180.759C 177.956,180.83 178.018,180.873 178.084,180.873C 178.151,180.873 178.213,180.83 178.263,180.759L 179.156,179.411L 182.092,174.976C 182.135,174.896 182.161,174.778 182.161,174.659C 182.161,174.407 182.057,174.205 181.929,174.205L 180.434,174.204L 175.744,174.202L 174.248,174.201 Z");
            }
            return null;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    /// <summary>
    /// Converter class for dragging indicator
    /// </summary>
    public class DragIndicatorConverter : IValueConverter
    {
        #region IValueConverter Members

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
            if (value is PivotItem && targetType == typeof(string))
            {
                return ((PivotItem)value).FieldHeader;
            }

            else if (value is PivotItem && targetType == typeof(Visibility))
            {
                return Visibility.Visible;
            }

            else if (value is PivotComputationInfo && targetType == typeof(string))
            {
#if SILVERLIGHT
                return ((PivotComputationInfo)value).FieldHeader;
#else
                return ((PivotComputationInfo)value).FieldName;
#endif
            }

            else if (value is PivotComputationInfo && targetType == typeof(Visibility))
            {
                return Visibility.Collapsed;
            }

            else if (value is FilterItemsCollection && targetType == typeof(string))
            {
#if SILVERLIGHT
                return ((FilterItemsCollection)value).DisplayHeader;
#else
                return ((FilterItemsCollection)value).FilterProperty.Name;
#endif
            }

            else if (value is FilterItemsCollection && targetType == typeof(Visibility))
            {
                if (parameter != null && parameter.ToString() == "Filter")
                {
                    return Visibility.Visible;
                }
                else
                    return Visibility.Collapsed;
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

        #endregion
    }
    /// <summary>
    /// Converter class for filter and filtered image
    /// </summary>
    public class ImageConverter : IValueConverter
    {
        #region IValueConverter Members
        private string path;
        private static Dictionary<string,bool> _checkFilter;
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageConverter">ImageConverter</see> class. 
        /// </summary>
        public ImageConverter()
        {
            
        }
        /// <summary>
        /// Get the dictionary from the FilterPopup class, handles when the filtering can be done
        /// </summary>
        /// <param name="filter">contains the FieldNames/FilterList names when filtering</param>
        public void GetDictionary(Dictionary<string, bool> filter)
        {
            _checkFilter = filter;
        }
        /// <summary>
        /// Handles the convert the images while filtering
        /// </summary>
        /// <param name="value">The source object being passed to the target.</param>
        /// <param name="targetType">The type to convert to source of Images.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>string contains the path of image source</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((value as FilterItemsCollection) != null && (value as FilterItemsCollection)[0].IsSelected == true)
            {
                path = "/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Resources/Filter.png";
            }
            else if ((value as FilterItemsCollection) != null && (value as FilterItemsCollection)[0].IsSelected == null)
            {
                path = "/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Resources/Filtered.png";
            }
            else if ((value as FilterItemsCollection) == null)
            {
                path = "/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Resources/Filter.png";
                PivotItem pivotitem = value as PivotItem;

                if (_checkFilter == null)
                {
                    _checkFilter = new Dictionary<string, bool>();
                }

                if (pivotitem != null)
                {
                    if (_checkFilter.Keys.Contains(pivotitem.FieldMappingName))
                    {
                        path = "/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Resources/Filtered.png";
                        return path;
                    }
                }
            }
            return path;
        }
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
#if SILVERLIGHT

    public class BrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new SolidColorBrush(Colors.Green);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class VisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
           // throw new NotImplementedException();
           
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

#endif
}
