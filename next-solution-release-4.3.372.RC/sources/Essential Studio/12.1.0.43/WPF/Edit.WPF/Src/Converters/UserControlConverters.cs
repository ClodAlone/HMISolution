#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Syncfusion.Windows.Edit
{
#if SyncfusionFramework4_0

    using System.ComponentModel;

    [DesignTimeVisible(false)]
#endif
    /// <summary>
    ///
    /// </summary>
    public class Int32ToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value == 0)
            {
                return "Quick Find";
            }
            else
            {
                return "Find Symbol";
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IValueConverter Members
    }

#if SyncfusionFramework4_0

    [DesignTimeVisible(false)]
#endif
    /// <summary>
    ///
    /// </summary>
    public class BooleanToBackgroundBrushConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return Brushes.AliceBlue;
            }
            else
            {
                return Brushes.Transparent;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IValueConverter Members
    }

#if SyncfusionFramework4_0

    [DesignTimeVisible(false)]
#endif
    /// <summary>
    ///
    /// </summary>
    public class BooleanToBorderBrushConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return Brushes.Blue;
            }
            else
            {
                return Brushes.Transparent;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IValueConverter Members
    }

#if SyncfusionFramework4_0

    [DesignTimeVisible(false)]
#endif
    /// <summary>
    ///
    /// </summary>
    public class InvertedBooleanToBorderBrushConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return Brushes.Transparent;
            }
            else
            {
                return Brushes.Blue;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IValueConverter Members
    }

#if SyncfusionFramework4_0

    [DesignTimeVisible(false)]
#endif
    /// <summary>
    ///
    /// </summary>
    public class InvertedBooleanToBackgroundBrushConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return Brushes.Transparent;
            }
            else
            {
                return Brushes.AliceBlue;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IValueConverter Members
    }

#if SyncfusionFramework4_0

    [DesignTimeVisible(false)]
#endif
    /// <summary>
    ///
    /// </summary>
    public class FindResultToTextConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is FindResult)
            {
                FindResult result = value as FindResult;
                return string.Format(@"({0},{1}) {2}", result.LineNumber, result.Index, result.Text);
            }
            return string.Empty;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IValueConverter Members
    }

#if SyncfusionFramework4_0

    [DesignTimeVisible(false)]
#endif
    /// <summary>
    ///
    /// </summary>
    public class TabVisibilityToVisibilityConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        /// <summary>
        ///
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] != DependencyProperty.UnsetValue)
            {
                TabVisibility visiblity = (TabVisibility)Enum.Parse(typeof(TabVisibility), values[0].ToString());
                switch (visiblity)
                {
                    case TabVisibility.Auto:
                        if (values[1] != DependencyProperty.UnsetValue && values[1] != null && values[2] != DependencyProperty.UnsetValue && !(bool)values[2])
                        {
                            return Visibility.Visible;
                        }
                        return Visibility.Collapsed;
                    case TabVisibility.Visible:
                        return Visibility.Visible;
                    case TabVisibility.Collapsed:
                        return Visibility.Collapsed;
                    default:
                        return Visibility.Visible;
                }
            }
            return Visibility.Collapsed;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IMultiValueConverter Members
    }

#if SyncfusionFramework4_0

    [DesignTimeVisible(false)]
#endif
    /// <summary>
    ///
    /// </summary>
    public class LinePropertiesToStyleConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        /// <summary>
        ///
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool containsLines = (bool)values[0];
            LineItem item = values[1] as LineItem;
            EditControl control = parameter as EditControl;
            ResourceDictionary dictionary = new ResourceDictionary();
            dictionary.Source = new Uri(@"/Syncfusion.Edit.Wpf;component/Themes/Generic.xaml", UriKind.Relative);
            bool showLine = (bool)values[4];
            if (containsLines)
            {
                return dictionary["ExpandButtonStyle"];
            }
            else
            {
                if (showLine && item.ParentLineNumber > 0 && control.Lines.Count > item.ParentLineNumber - 1 && control.Lines[item.ParentLineNumber - 1].ContainsLines && control.Lines[item.ParentLineNumber - 1].EndLine == control.Lines.IndexOf(item) + 1)
                {
                    return dictionary["EndLineButtonStyle"];
                }
                else if (item.ParentLineNumber > 0 && showLine)
                {
                    return dictionary["LineButtonStyle"];
                }
                else
                {
                    return dictionary["EmpyLineButtonStyle"];
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IMultiValueConverter Members
    }

#if SyncfusionFramework4_0

    [DesignTimeVisible(false)]
#endif
    /// <summary>
    ///
    /// </summary>
    public class EditTypeInfoToImageSourceConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EditTypeInfo)
            {
                string imageUri = "/Syncfusion.Edit.Wpf;component/Resources/";
                EditTypeInfo item = value as EditTypeInfo;
                if (item.IsClass)
                {
                    imageUri += "class.png";
                }
                else if (item.IsEnum)
                {
                    imageUri += "enum.png";
                }
                else if (item.IsInterface)
                {
                    imageUri += "interface.png";
                }
                else if (item.IsInstance)
                {
                    imageUri += "field.png";
                }
                else if (item.IsLexem)
                {
                    imageUri += "keyword.png";
                }
                else if (item.IsProperty)
                {
                    imageUri += "property.png";
                }
                else if (item.IsEvent)
                {
                    imageUri += "event.png";
                }
                else if (item.IsMethod)
                {
                    imageUri += "method.png";
                }
                else
                {
                    imageUri += "ns.png";
                }

                return new System.Windows.Media.Imaging.BitmapImage(new Uri(imageUri, UriKind.Relative));
            }
            return null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IValueConverter Members
    }

    /// <summary>
    ///
    /// </summary>
    public class TabSelectionConverter : IMultiValueConverter
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool findTab = (bool)values[0];
            bool symbolTab = (bool)values[1];
            return findTab || symbolTab;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class OrBasedVisibilityConverter : IMultiValueConverter
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool findTab = values[0] is bool ? (bool)values[0] : false;
            bool symbolTab = values[1] is bool ? (bool)values[1] : false;
            if (findTab || symbolTab)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class TabSelectionToTextConverter : IMultiValueConverter
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool findTab = (bool)values[0];
            bool symbolTab = (bool)values[1];
            string txt = values[2].ToString();
            if (findTab)
            {
                return "Quick Find";
            }

            if (symbolTab)
            {
                return "Find Symbol";
            }

            if (txt != string.Empty)
            {
                return txt;
            }
            else
            {
                return "Quick Find";
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class EndLineToLineHeightConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        /// <summary>
        ///
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[2] == null)
                return 0;
            LineItem item = values[2] as LineItem;

            double val = 0;
            if (values[0] != DependencyProperty.UnsetValue)
            {
                val = (double)values[0];
            }
            else
            {
                val = item.ParentControl != null ? item.ParentControl.LineHeight : 0;
            }

            EditControl control = null;
            if (values[1] != DependencyProperty.UnsetValue)
            {
                control = values[1] as EditControl;
            }
            else
            {
                control = item.ParentControl != null ? item.ParentControl : null;
            }

            if (control == null)
            {
                return val;
            }

            if (item != null && control != null)
            {
                int index = control.Lines.IndexOf(item);
                if (item.ParentLineNumber > 0 && control.Lines.Count > item.ParentLineNumber - 1)
                {
                    LineItem parentLineItem = control.Lines[item.ParentLineNumber - 1];
                    if (parentLineItem.EndLine == index + 1 && parentLineItem.ParentLineNumber <= 0 && parameter.ToString() == "Bottom")
                    {
                        return 0;
                    }
                }
            }
            return val / 2;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IMultiValueConverter Members
    }

    internal class LineStateToBackgroundConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            LineModificationState state = (LineModificationState)(Enum.Parse(typeof(LineModificationState), values[0].ToString()));
            switch (state)
            {
                case LineModificationState.Unchanged:
                    return Brushes.Transparent;
                case LineModificationState.Modified:
                    return (Brush)values[3];
                case LineModificationState.Saved:
                    return (Brush)values[2];
                default:
                    return Brushes.Transparent;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IMultiValueConverter Members
    }
}