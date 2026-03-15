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
using System.Globalization;
using System.Windows.Media.Imaging;
using Syncfusion.Windows.Shared.Resources;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class SizeToDoubleConverter : IValueConverter
    {
        #region Implementation

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
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double result = 0;
            Size size = (Size)value;
            string strParemeter = parameter.ToString();
            if (strParemeter == "width")
            {
                result = size.Width;
            }
            else
            {
                result = size.Height;
            }

            return result;
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
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class EnumToVisibilityConverter : IValueConverter
    {

        #region IValueConverter Members

        /// <summary>
        /// 
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
            {
                if ((DialogIcon)value == DialogIcon.None)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
            else
            {
                if (parameter.ToString() == "2")
                {
                    if ((DialogButton)value == DialogButton.OK)
                    {
                        return Visibility.Collapsed;
                    }
                    else
                    {
                        return Visibility.Visible;
                    }
                }
                else if (parameter.ToString() == "3")
                {
                    if ((DialogButton)value == DialogButton.OKCancelApply || (DialogButton)value == DialogButton.YesNoCancel)
                    {
                        return Visibility.Visible;
                    }
                    else
                    {
                        return Visibility.Collapsed;
                    }
                }

                else if (parameter.ToString() == "4")
                {
                    if (value.ToString() != string.Empty)
                    {
                        return Visibility.Visible;
                    }
                    else
                    {
                        return Visibility.Collapsed;
                    }
                }

                else if (parameter.ToString() == "5")
                {
                    if (value.ToString() != string.Empty)
                    {
                        return Visibility.Visible;
                    }
                    else
                    {
                        return Visibility.Collapsed;
                    }
                }

                else if (parameter.ToString() == "6")
                {
                    if (value != null)
                    {
                        return Visibility.Visible;
                    }
                    else
                    {
                        return Visibility.Collapsed;
                    }
                }
                else
                {
                    return Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class EnumToImageConverter : IValueConverter
    {

        #region IValueConverter Members

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        /// <param name="value">The source data being passed to the target.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (parameter.ToString() == "1")
            {
                System.Windows.Resources.StreamResourceInfo sr=null;
                BitmapImage bmp = new BitmapImage();
                if ((DialogIcon)value == DialogIcon.Information)
                {
                    sr= Application.GetResourceStream(new Uri("/Syncfusion.Shared.Silverlight;Component/System.Windows.Controls/Window/Images/Info.png", UriKind.RelativeOrAbsolute)); 
                    bmp.SetSource(sr.Stream);
                    return bmp;
                    //return new BitmapImage(new Uri(@"/Syncfusion.Shared.Silverlight;Component/System.Windows.Controls/Window/Images/Info.png", UriKind.RelativeOrAbsolute));
                }
                else if ((DialogIcon)value == DialogIcon.Warning)
                {
                    sr = Application.GetResourceStream(new Uri("/Syncfusion.Shared.Silverlight;Component/System.Windows.Controls/Window/Images/Warning.png", UriKind.RelativeOrAbsolute));
                    bmp.SetSource(sr.Stream);
                    return bmp;
                    //return new BitmapImage(new Uri("/Syncfusion.Shared.Silverlight;Component/System.Windows.Controls/Window/Images/Warning.png", UriKind.RelativeOrAbsolute));
                }
                else if ((DialogIcon)value == DialogIcon.Error)
                {
                    sr = Application.GetResourceStream(new Uri("/Syncfusion.Shared.Silverlight;Component/System.Windows.Controls/Window/Images/Error.png", UriKind.RelativeOrAbsolute));
                    bmp.SetSource(sr.Stream);
                    return bmp;
                    //return new BitmapImage(new Uri("/Syncfusion.Shared.Silverlight;Component/System.Windows.Controls/Window/Images/Error.png", UriKind.RelativeOrAbsolute));
                }
                else if ((DialogIcon)value == DialogIcon.Exclamation)
                {
                    sr = Application.GetResourceStream(new Uri("/Syncfusion.Shared.Silverlight;Component/System.Windows.Controls/Window/Images/Exclamation.png", UriKind.RelativeOrAbsolute));
                    bmp.SetSource(sr.Stream);
                    return bmp;
                    //return new BitmapImage(new Uri("/Syncfusion.Shared.Silverlight;Component/System.Windows.Controls/Window/Images/Exclamation.png", UriKind.RelativeOrAbsolute));
                }
                else if ((DialogIcon)value == DialogIcon.Question)
                {
                    sr = Application.GetResourceStream(new Uri("/Syncfusion.Shared.Silverlight;Component/System.Windows.Controls/Window/Images/Question.png", UriKind.RelativeOrAbsolute));
                    bmp.SetSource(sr.Stream);
                    return bmp;
                    //return new BitmapImage(new Uri("/Syncfusion.Shared.Silverlight;Component/System.Windows.Controls/Window/Images/Question.png", UriKind.RelativeOrAbsolute));
                }
                else
                {
                    return null;
                }
            }
            else if (parameter.ToString() == "2")
            {
                return value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        /// <param name="value">The target data being passed to the source.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class EnumToTextConverter : IValueConverter
    {
        private ResourceDictionary _mgeneric = new ResourceDictionary() { Source = new Uri("/Syncfusion.Shared.Silverlight;Component/System.Windows.Controls/Window/Themes/Default.xaml", UriKind.RelativeOrAbsolute) };

        #region IValueConverter Members

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        /// <param name="value">The source data being passed to the target.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ResourceWrapper wrapper = _mgeneric["ResourceWrapperKey"] as ResourceWrapper;

            if (parameter.ToString() == "1")
            {
                if ((DialogButton)value == DialogButton.OK || (DialogButton)value == DialogButton.OKCancel || (DialogButton)value == DialogButton.OKCancelApply)
                {
                    return wrapper.Ok;
                }
                else if ((DialogButton)value == DialogButton.YesNo || (DialogButton)value == DialogButton.YesNoCancel)
                {
                    return wrapper.Yes;
                }
                else if ((DialogButton)value == DialogButton.AbortRetry)
                {
                    return wrapper.Abort;
                }
            }
            else if (parameter.ToString() == "2")
            {
                if ((DialogButton)value == DialogButton.OKCancel || (DialogButton)value == DialogButton.OKCancelApply)
                {
                    return wrapper.Cancel;
                }
                else if ((DialogButton)value == DialogButton.YesNo || (DialogButton)value == DialogButton.YesNoCancel)
                {
                    return wrapper.No;
                }
                else if ((DialogButton)value == DialogButton.AbortRetry)
                {
                    return wrapper.Retry;
                }
            }
            else
            {
                if ((DialogButton)value == DialogButton.YesNoCancel)
                {
                    return wrapper.Cancel;
                }
                else if ((DialogButton)value == DialogButton.OKCancelApply)
                {
                    return wrapper.Apply;
                }
            }
            return String.Empty;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        /// <param name="value">The target data being passed to the source.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}