#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Controls.Spreadsheet.Resources;

namespace Syncfusion.Windows.Controls.Spreadsheet
{

    public class DocumentPropertiesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
#if SILVERLIGHT
            if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
#endif
                return value;
#if SILVERLIGHT
            else
                return string.Empty;
#endif
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
#if SILVERLIGHT
            if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
#endif
                return value;
#if SILVERLIGHT
            else
                return string.Empty;
#endif
        }
    }


    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SolidColorBrush SolidBrush = new SolidColorBrush((Color)value);
            return SolidBrush;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class HeightToFormulaBarVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility val = (Visibility)value;
            if (val.Equals(Visibility.Visible))
                return 10;
            else
                return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double val = 0;
            double.TryParse(value.ToString(), out val);
            if (val > 0)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }
    }


    public class CurrentCellStyleToReadOnlyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            GridStyleInfo CurrentCellStyle = (GridStyleInfo)value;
            if (CurrentCellStyle != null)
            {
                if (CurrentCellStyle.CellType == "Static")
                    return true;
                else if (CurrentCellStyle.CellType == "HeaderCell")
                    return true;
                else if (CurrentCellStyle.CellType == "GridDataControlCell")
                    return true;
                else if (CurrentCellStyle.CellType == "GridDataBoundTemplate")
                    return true;
                else if (CurrentCellStyle.CellType == "RichText")
                    return true;

                else if (CurrentCellStyle.CellType == "FormulaCell" || CurrentCellStyle.CellType == "DoubleEdit" || CurrentCellStyle.CellType == "DateTimeEdit" || CurrentCellStyle.CellType == "CurrencyEdit" ||
                    CurrentCellStyle.CellType == "IntegerEdit" || CurrentCellStyle.CellType == "DoubleEdit" || CurrentCellStyle.CellType == "ComboBox" ||
                    CurrentCellStyle.CellType == "MaskEdit" || CurrentCellStyle.CellType == "ImageCell" || CurrentCellStyle.CellType == "Hyperlink" ||
                    CurrentCellStyle.CellType == "TextBox")
                    return CurrentCellStyle.ReadOnly;
                else return true;
            }
            else
                return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                bool val = (bool)value;
                if (val)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
            else
            {
                Visibility val = (Visibility)value;
                if (val.Equals(Visibility.Visible))
                    return true;
                else
                    return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                bool val = (bool)value;
                if (val)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
            else
            {
                Visibility val = (Visibility)value;
                if (val.Equals(Visibility.Visible))
                    return true;
                else
                    return false;
            }
        }
    }

    public class InverseBoolConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)value;
        }
        #endregion
    }


    public class GridCellBoldToBoolConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (FontWeight)value == FontWeights.Bold;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool val = (bool)value;
            if (val)
                return FontWeights.Bold;
            else
                return FontWeights.Normal;
        }
        #endregion
    }

    public class GridCellItalicToBoolConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (FontStyle)value == FontStyles.Italic;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool val = (bool)value;
            if (val)
                return FontStyles.Italic;
            else
                return FontStyles.Normal;
        }
        #endregion
    }

    public class GridCellUnderlineToBoolConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (TextDecorationCollection)value == TextDecorations.Underline;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool val = (bool)value;
            if (val)
                return TextDecorations.Underline;
#if !SILVERLIGHT
            else
                return TextDecorations.Baseline;
#else
            else
                return value;
#endif


        }
        #endregion
    }

    public class TopAlignToBoolConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (VerticalAlignment)value == VerticalAlignment.Top;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return VerticalAlignment.Top;
        }
        #endregion
    }

    public class MiddleAlignToBoolConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (VerticalAlignment)value == VerticalAlignment.Center;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return VerticalAlignment.Center;
        }
        #endregion
    } 
    
    public class BottomAlignToBoolConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (VerticalAlignment)value == VerticalAlignment.Bottom;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return VerticalAlignment.Bottom;
        }
        #endregion
    }

    public class LeftAlignToBoolConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (HorizontalAlignment)value == HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return HorizontalAlignment.Left;
        }
        #endregion
    }

    public class CenterAlignToBoolConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (HorizontalAlignment)value == HorizontalAlignment.Center;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return HorizontalAlignment.Center;
        }
        #endregion
    } 
    
    public class RightAlignToBoolConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (HorizontalAlignment)value == HorizontalAlignment.Right;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return HorizontalAlignment.Right;
        }
        #endregion
    }

    public class GridCellFontSizeToIndexConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int[] fontSizes = new int[] { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
            for (int i = 0; i < fontSizes.Length; i++)
            {
                if (int.Parse(value.ToString()) == fontSizes[i])
                    return i;
            }
            return 3;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
        #endregion
    }

    public class GridCellFontNameToIndexConverter : IValueConverter
    {
        string[] fontNames = new string[] { "Arial", "Arial Black", "Calibri(Body)", "Comic Sans MS", "Georgia", "Lucida Sans Unicode", "Portable User Interface", "Times New Roman", "Trebuchet MS", "Verdana", "Webdings" };
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //FontFamily font = (FontFamily)value;
            //for (int i = 0; i < fontNames.Length; i++)
            //{
            //    if (font == new FontFamily(fontNames[i].ToString()))
            //        return i;
            //}
            return 3;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
        #endregion
    }

    public class GridCellCommentToNewCommentVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        #endregion
    }
      public class GridCellCommentToDeleteCommentEnableConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return false;
            else
                return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        #endregion
    }
  
    public class GridCellCommentToEditCommentVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        #endregion
    }

    public class BackStageWidthToBorderWidthConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && value is double && (double)value > 400)
#if SILVERLIGHT

                return (double)value / 1.9;
#else
                return (double)value / 1.5;
#endif
            else
                return 200;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        #endregion
    }

    public class BackStageHeightToBorderHeightConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && value is double && (double)value > 400)
                return (double)value / 1.1;
            else
                return 200;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        #endregion
    }


    public class ProtectSheetVisibleConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }


        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        #endregion
    }
    public class UnProtectSheetVisibleConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        #endregion
    }

    public class FreezePaneTextConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value)
            {
#if !SILVERLIGHT
                return SpreadsheetResourceWrapper.UnFreezePanes;
#else
                return SpreadsheetResourceWrapper.UnFreezePanes;
#endif

            }
            else
            {
#if !SILVERLIGHT

                return SpreadsheetResourceWrapper.FreezePanes;
#else
                return SpreadsheetResourceWrapper.FreezePanes;
#endif
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        #endregion
    }


    public class ExcelNumberFormatToNumberFormat : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        #endregion
    }

    public class TextWrappingToBoolConverter:IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                if ((TextWrapping)value == TextWrapping.Wrap)
                    return true;
                else
                    return false;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                if ((bool)value)
                    return TextWrapping.Wrap;
                else
                    return TextWrapping.NoWrap;
            }
            return TextWrapping.NoWrap;
        }
    }

}