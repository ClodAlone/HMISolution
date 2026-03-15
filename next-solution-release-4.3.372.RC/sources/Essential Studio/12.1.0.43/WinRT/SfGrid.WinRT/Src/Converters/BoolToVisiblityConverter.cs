#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.UI.Xaml.Grid.Cells;
#if WinRT
using System.Collections;
using System.Globalization;
using Syncfusion.Data;
using Syncfusion.UI.Xaml.Controls.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#else
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using System.Linq;
using System.Windows;
using Syncfusion.Data;
using Syncfusion.Data.Extensions;
#if !WP
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;
#endif
#endif

namespace Syncfusion.UI.Xaml.Grid
{
    public class BoolToVisiblityConverter : IValueConverter
    {

#if WinRT
        /// <summary>
        /// Converts the bool value to Visiblity
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        object IValueConverter.Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter != null && parameter.Equals("InverseVisiblity") && (bool)value)
                return Visibility.Collapsed;
            else if (parameter != null && parameter.Equals("InverseVisiblity") && !(bool)value)
                return Visibility.Visible;
            else if ((bool)value)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        /// <summary>
        /// Converts the visibity value to bool
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if ((Visibility)value == Visibility.Visible)
                return true;
            return false;
        }
#else
        /// <summary>
        /// Converts the bool value to Visiblity
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter != null && parameter.Equals("InverseVisiblity") && (bool)value)
                return Visibility.Collapsed;
            else if (parameter != null && parameter.Equals("InverseVisiblity") && !(bool)value)
                return Visibility.Visible;
            else if ((bool)value)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        /// <summary>
        /// Converts the visibity value to bool
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((Visibility)value == Visibility.Visible)
                return true;
            return false;
        }
#endif

    }

    public class DisplayMemberConverter : IValueConverter
    {
        public DisplayMemberConverter()
        {

        }

        public DisplayMemberConverter(GridColumn column)
        {
            cachedColumn = column;
        }

        private GridColumn cachedColumn;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return Convert(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public object Convert(object selectedValue)
        {
            IEnumerable list = null;
            var displayMemberPath = string.Empty;
            var valueMemberPath = string.Empty;
            if (cachedColumn is GridComboBoxColumn)
            {
                var column = cachedColumn as GridComboBoxColumn;
                cachedColumn = column;
                list = column.ItemsSource as IEnumerable;
                displayMemberPath = column.DisplayMemberPath;
                valueMemberPath = column.SelectedValuePath;
            }
            else if (cachedColumn is GridMultiColumnDropDownList)
            {
                var column = cachedColumn as GridMultiColumnDropDownList;
                cachedColumn = column;
                list = column.ItemsSource as IEnumerable;
                displayMemberPath = column.DisplayMember;
                valueMemberPath = column.ValueMember;
            }

            if (selectedValue == null)
                return null;

            if (!string.IsNullOrEmpty(valueMemberPath) && list!=null)
            {
                var enumerator = list.GetEnumerator();
#if WPF
                PropertyDescriptorCollection pdc = null;
#else
                PropertyInfoCollection pdc = null;
#endif
                while (enumerator.MoveNext())
                {
                    if (pdc == null)
#if WPF
                        pdc = TypeDescriptor.GetProperties(enumerator.Current.GetType());
#else
                        pdc = new PropertyInfoCollection(enumerator.Current.GetType());
#endif
                    if (selectedValue.Equals(pdc.GetValue(enumerator.Current, valueMemberPath)))
                    {
                        if (!string.IsNullOrEmpty(displayMemberPath))
                            return pdc.GetValue(enumerator.Current, displayMemberPath);
                        return enumerator.Current;
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(displayMemberPath) && list!=null)
                {
#if WPF
                    var pdc = TypeDescriptor.GetProperties(selectedValue.GetType());
#else
                var pdc = new PropertyInfoCollection(selectedValue.GetType());
#endif
                    return pdc.GetValue(selectedValue, displayMemberPath);
                }
                return selectedValue;
            }
            return null;
        }
    }
#if !WP
    public class CultureFormatConverter : IValueConverter
    {
        private GridColumn cachedColumn;
        public CultureFormatConverter(GridColumn column)
        {
            cachedColumn = column;
        }
#if !WinRT
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string formatValue = string.Empty;
            decimal columnValue;
            if (value == null)
            {                                                                                      
                if (cachedColumn is GridTimeSpanColumn)
                {
                    var column = (cachedColumn as GridTimeSpanColumn);
                    TimeSpan _columnValue;
                    if (column.AllowNull && column.NullText != string.Empty)
                        return column.NullText;
                    else if (column.AllowNull && column.NullText == string.Empty && column.MinValue == System.TimeSpan.MinValue
                        || (column.AllowNull && column.NullText == string.Empty && column.MinValue != System.TimeSpan.MinValue))
                        return null;

                    TimeSpan.TryParse(column.MinValue != System.TimeSpan.MinValue &&  !column.AllowNull? column.MinValue.ToString() : new TimeSpan(0, 0, 0, 0, 0).ToString(), out _columnValue);

                    return GridCellTimeSpanRenderer.DisplayText(_columnValue, column.Format);                    
                }
                else if (cachedColumn is GridDateTimeColumn)
                {
                    var column = cachedColumn as GridDateTimeColumn;
                    if (column.AllowNullValue && column.MaxDateTime != System.DateTime.MaxValue && column.NullText == string.Empty)
                        return column.MaxDateTime;
                    if (column.AllowNullValue && column.NullValue != null)
                        return column.NullValue;
                    else if (column.AllowNullValue && column.NullText != string.Empty)
                        return column.NullText;
                    if (column.MaxDateTime != System.DateTime.MaxValue)
                        return column.MaxDateTime;
                }
                else if (cachedColumn is GridEditorColumn)
                {
                    var column = cachedColumn as GridEditorColumn;
                    if (column.AllowNullValue && column.NullValue != null)
                    {
                        decimal.TryParse(column.NullValue.ToString(), out columnValue);
                        return ConvertToFormat(column,columnValue);
                    }
                    else if (column.AllowNullValue && column.NullText != string.Empty)
                        return column.NullText;
                }                   
                    return null;
            }           
            var _value = value.ToString();           

            if (cachedColumn is GridCurrencyColumn)
            {
                var column = cachedColumn as GridCurrencyColumn;                
              decimal.TryParse(value.ToString(), out columnValue);          

                if (columnValue < column.MinValue)
                    columnValue = column.MinValue;
                if (columnValue > column.MaxValue)
                    columnValue = column.MaxValue;
                formatValue = ConvertToFormat(column, columnValue);
            }
            else if (cachedColumn is GridPercentColumn)
            {
                var column = cachedColumn as GridPercentColumn;
                decimal.TryParse(value.ToString(), out columnValue);

                if (columnValue < column.MinValue)
                    columnValue = column.MinValue;
                if (columnValue > column.MaxValue)
                    columnValue = column.MaxValue;             
                formatValue = ConvertToFormat(column, columnValue);
            }
            else if (cachedColumn is GridNumericColumn)
            {
                var column = cachedColumn as GridNumericColumn;
                decimal.TryParse(value.ToString(), out columnValue);

                if (columnValue < column.MinValue)
                    columnValue = column.MinValue;
                if (columnValue > column.MaxValue)
                    columnValue = column.MaxValue;
                formatValue = ConvertToFormat(column, columnValue);
            }
            else if (cachedColumn is GridDateTimeColumn)
            {
                DateTime _columnValue;
                var column = (GridDateTimeColumn) cachedColumn;
                DateTime.TryParse(_value, (cachedColumn as GridDateTimeColumn).DateTimeFormat,
                                    DateTimeStyles.AdjustToUniversal, out _columnValue);
#if !WinRT
                if (_columnValue < column.MinDateTime)
                    _columnValue = column.MinDateTime;
                if (_columnValue > column.MaxDateTime)
                    _columnValue = column.MaxDateTime;
#endif
                return DateTimeFormatString(_columnValue, column);
            }
            else if (cachedColumn is GridMaskColumn)
            {
                var column = cachedColumn as GridMaskColumn;
                Decimal.TryParse(_value, NumberStyles.Any, NumberFormatInfo.CurrentInfo, out columnValue);
                if (column.IsNumeric)
                    return columnValue.ToString(column.Mask, NumberFormatInfo.CurrentInfo);
                else
                {                   
#if WPF
                    var dateSeparator = string.IsNullOrEmpty(column.DateSeparator) ? DateTimeFormatInfo.CurrentInfo.DateSeparator : column.DateSeparator;
                    var timeSeparator = string.IsNullOrEmpty(column.TimeSeparator) ? DateTimeFormatInfo.CurrentInfo.TimeSeparator : column.TimeSeparator;
                    var decimalSeparator = string.IsNullOrEmpty(column.DecimalSeparator) ? NumberFormatInfo.CurrentInfo.NumberDecimalSeparator : column.DecimalSeparator;
#elif SILVERLIGHT
                    var dateSeparator = string.IsNullOrEmpty(column.DateSeparator) ? "/" : column.DateSeparator;
                    var timeSeparator = string.IsNullOrEmpty(column.TimeSeparator) ? ":": column.TimeSeparator;
                    var decimalSeparator = string.IsNullOrEmpty(column.DecimalSeparator) ? NumberFormatInfo.CurrentInfo.NumberDecimalSeparator : column.DecimalSeparator;
#endif
#if SyncfusionFramework3_5 && SILVERLIGHT
                    string maskValue = MaskedEditorModel.GetMaskedText(column.Mask, _value,
                                                             DateTimeFormatInfo.CurrentInfo.DateSeparator,
                                                             NumberFormatInfo.CurrentInfo.NumberDecimalSeparator,
                                                             NumberFormatInfo.CurrentInfo.NumberDecimalSeparator,
                                                             NumberFormatInfo.CurrentInfo.NumberGroupSeparator, 
                                                             '0',
                                                             NumberFormatInfo.CurrentInfo.CurrencySymbol);
#else
                    string maskValue = MaskedEditorModel.GetMaskedText(column.Mask, _value,
                                                             dateSeparator,
                                                             timeSeparator,
                                                             decimalSeparator,
                                                             NumberFormatInfo.CurrentInfo.NumberGroupSeparator, 
                                                             column.PromptChar,
                                                             NumberFormatInfo.CurrentInfo.CurrencySymbol);
#endif
                    return maskValue;
                }
            }
            else if (cachedColumn is GridTimeSpanColumn)
            {
                TimeSpan _columnValue;
                var column = cachedColumn as GridTimeSpanColumn;
                TimeSpan.TryParse(_value, out _columnValue);
                if (_columnValue < column.MinValue)
                    _columnValue = column.MinValue;
                if (_columnValue > column.MaxValue)
                    _columnValue = column.MaxValue;
                return GridCellTimeSpanRenderer.DisplayText(_columnValue, column.Format);
            }
            return formatValue;
        }

        private string ConvertToFormat(GridEditorColumn column, decimal columnValue)
        {
            if (column is GridCurrencyColumn)
            {
                var currencyColumn = cachedColumn as GridCurrencyColumn;
                var currencyNumberFormatInfo = new NumberFormatInfo
                {
                    CurrencyDecimalDigits = currencyColumn.CurrencyDecimalDigits,
                    CurrencyDecimalSeparator = currencyColumn.CurrencyDecimalSeparator,
                    CurrencyGroupSeparator = currencyColumn.CurrencyGroupSeparator,
                    CurrencyNegativePattern = currencyColumn.CurrencyNegativePattern,
                    CurrencyPositivePattern = currencyColumn.CurrencyPositivePattern,
                    CurrencySymbol = currencyColumn.CurrencySymbol,
                    CurrencyGroupSizes = currencyColumn.CurrencyGroupSizes.ToArray(),
                };                
                return columnValue.ToString("C" , currencyNumberFormatInfo);
            }        
            else if(column is GridPercentColumn)
            {
                var percentColumn = cachedColumn as GridPercentColumn;

                if (percentColumn.PercentEditMode == PercentEditMode.DoubleMode)
                    columnValue /= 100;

                var percentFormatInfo = new NumberFormatInfo
                {
                    PercentDecimalDigits = percentColumn.PercentDecimalDigits,
                    PercentDecimalSeparator = percentColumn.PercentDecimalSeparator,
                    PercentGroupSeparator = percentColumn.PercentGroupSeparator,
                    PercentNegativePattern = percentColumn.PercentNegativePattern,
                    PercentPositivePattern = percentColumn.PercentPositivePattern,
                    PercentSymbol = percentColumn.PercentSymbol,
                    PercentGroupSizes = percentColumn.PercentGroupSizes.ToArray(),
                };
                return columnValue.ToString("P", percentFormatInfo);
            }
            else if(column is GridNumericColumn)
            {
                var numericColumn = cachedColumn as GridNumericColumn;
                var numericNumberFormatInfo = new NumberFormatInfo
                {
                    NumberDecimalDigits = numericColumn.NumberDecimalDigits,
                    NumberDecimalSeparator = numericColumn.NumberDecimalSeparator,
                    NumberGroupSeparator = numericColumn.NumberGroupSeparator,
                    NumberNegativePattern = numericColumn.NumberNegativePattern,
                    NumberGroupSizes = numericColumn.NumberGroupSizes.ToArray(),
                };
                return columnValue.ToString("N", numericNumberFormatInfo);
            }
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        private string DateTimeFormatString(DateTime columnValue, GridDateTimeColumn column)
        {
            switch (column.Pattern)
            {
                case DateTimePattern.ShortDate:
                    return columnValue.ToString("d", column.DateTimeFormat);
                case DateTimePattern.LongDate:
                    return columnValue.ToString("D", column.DateTimeFormat);
                case DateTimePattern.LongTime:
                    return columnValue.ToString("T", column.DateTimeFormat);
                case DateTimePattern.ShortTime:
                    return columnValue.ToString("t", column.DateTimeFormat);
                case DateTimePattern.FullDateTime:
                    return columnValue.ToString("F", column.DateTimeFormat);
                case DateTimePattern.RFC1123:
                    return columnValue.ToString("R", column.DateTimeFormat);
                case DateTimePattern.SortableDateTime:
                    return columnValue.ToString("s", column.DateTimeFormat);
                case DateTimePattern.UniversalSortableDateTime:
                    return columnValue.ToString("u", column.DateTimeFormat);
                case DateTimePattern.YearMonth:
                    return columnValue.ToString("Y", column.DateTimeFormat);
                case DateTimePattern.MonthDay:
                    return columnValue.ToString("M", column.DateTimeFormat);
                case DateTimePattern.CustomPattern:                                          
                    return columnValue.ToString(column.CustomPattern, column.DateTimeFormat);                                           
                default:
                    return columnValue.ToString("MMMM", column.DateTimeFormat);
            }
        }
#endif

#if WinRT
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return null;
            var _value = value.ToString();
            string formatValue = string.Empty;
            Double columnValue;

            if (cachedColumn is GridNumericColumn)
            {
                Decimal columnvalue;
                var column = cachedColumn as GridNumericColumn;
                if (_value == null)
                {
                    if (!column.AllowNullInput)
                        _value = decimal.MinValue.ToString();
                }
                if (column.ParsingMode == Parsers.Double)
                {
                    Double.TryParse(_value, out columnValue);
                    if (string.Equals(column.FormatString, "P"))
                        columnValue /= 100;
                    formatValue = columnValue.ToString(column.FormatString, CultureInfo.CurrentUICulture);
                }
                else
                {
                    Decimal.TryParse(_value, out columnvalue);
                    if (string.Equals(column.FormatString, "P"))
                        columnvalue /= 100;
                    formatValue = columnvalue.ToString(column.FormatString, CultureInfo.CurrentUICulture);
                }
            }
            else if (cachedColumn is GridDateTimeColumn)
            {
                DateTime _columnValue;
                DateTime.TryParse(_value, CultureInfo.CurrentUICulture, DateTimeStyles.AdjustToUniversal, out _columnValue);
                formatValue = _columnValue.ToString((cachedColumn as GridDateTimeColumn).FormatString, CultureInfo.CurrentUICulture);
            }
            else if (cachedColumn is GridUpDownColumn)
            {
                var column = cachedColumn as GridUpDownColumn;
#if WinRT
                double _columnValue = double.MinValue;
                Double.TryParse(_value, NumberStyles.Float, CultureInfo.CurrentUICulture, out _columnValue);

                if (_columnValue < column.MinValue)
                    _columnValue = column.MinValue;
                if (_columnValue > column.MaxValue)
                    _columnValue = column.MaxValue;
                formatValue = _columnValue.ToString();
#else
                Decimal.TryParse(_value, NumberStyles.Float, column.Culture, out columnValue);

                if (columnValue < column.MinValue)
                    columnValue = column.MinValue;
                if (columnValue > column.MaxValue)
                    columnValue = column.MaxValue;
                formatValue = columnValue.ToString(column.FormatString, column.Culture);
#endif
            }
            return formatValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
#endif
    }
#endif

#if !WP
    public class TextAlignmentToHorizontalAlignmentConverter : IValueConverter
    {
#if !WinRT
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#else
        public object Convert(object value, Type targetType, object parameter, string language)
#endif
        {
            var textAlignment = value is TextAlignment ? (TextAlignment) value : TextAlignment.Left;
            HorizontalAlignment horizontalAlignment;
            switch (textAlignment)
            {
                case TextAlignment.Right:
                    horizontalAlignment = HorizontalAlignment.Right;
                    break;

                case TextAlignment.Center:
                    horizontalAlignment = HorizontalAlignment.Center;
                    break;

                case TextAlignment.Justify:
                    horizontalAlignment = HorizontalAlignment.Stretch;
                    break;
                default:
                    horizontalAlignment = HorizontalAlignment.Left;
                    break;
            }
            return horizontalAlignment;
        }

#if !WinRT
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
#else
        public object ConvertBack(object value, Type targetType, object parameter, string language)
#endif
        {
            return null;
        }
    }
#endif
}
