#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
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
using System.Collections;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class MultiLineConverter : IValueConverter
    {

        #region IValueConverter Members

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        /// <param name="value">The source data being passed to the target.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(!(String.IsNullOrEmpty(value as string)))
            {
                var items = new List<FrameworkElement>();
                var sender = parameter as IRibbonItem;
                string label = value.ToString(), text1 = String.Empty, text2 = String.Empty;

                int index = label.IndexOf(' ');
                if (index != -1)
                {
                    text1 = label.Substring(0, index);
                    text2 = label.Substring(index, label.Length - index);
                }
                else
                {
                    text1 = label.Substring(0, label.Length);
                }
                
                if (sender != null)
                {
                    if (sender.IsMultiLine && index != -1)
                    {
                        var text_1 = new TextBlock() { HorizontalAlignment = HorizontalAlignment.Center };
                        text_1.Text = text1.TrimStart().TrimEnd();
                        items.Add(text_1);

                        var text_2 = new TextBlock();

                        text_2.Text = text2.Trim();
                       
                        if (!(sender is RibbonDropDownButton))
                        {
                            text_1.Foreground = ((RibbonButton)sender).Foreground;
                            text_1.FontFamily = ((RibbonButton)sender).FontFamily;
                            text_1.FontSize = ((RibbonButton)sender).FontSize;
                            items.Add(text_2);
                        }
                        else
                        {
                            if (sender is RibbonDropDownButton)
                            {
                                var dropdown = sender as RibbonDropDownButton;
                                StackPanel panel = new StackPanel() { Orientation = Orientation.Horizontal };
                                panel.Children.Add(text_2);
                                TextBlock text_arrow = new TextBlock() { Text = "6", FontFamily = new FontFamily("Webdings"), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, FontSize = 14};
                                text_arrow.Foreground = ((RibbonDropDownButton)sender).Foreground;
                                text_arrow.FontSize = ((RibbonDropDownButton)sender).FontSize;
                                panel.Children.Add(text_arrow);
                                items.Add(panel);
                            }
                        }
                    }
                    else
                    {
                        var text = new TextBlock();
                        text.Text = label;
                        items.Add(text);
                        if (sender is RibbonDropDownButton)
                        {
                            TextBlock text_arrow = new TextBlock() { Text = "6", FontFamily = new FontFamily("Webdings"), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, FontSize = 14 };
                            text_arrow.Foreground = ((RibbonDropDownButton)sender).Foreground;
                            text_arrow.FontSize = ((RibbonDropDownButton)sender).FontSize;
                            items.Add(text_arrow);
                        }
                        else
                        {
                            var text_2 = new TextBlock() { Text = " " };
                            items.Add(text_2);
                        }
                    }
                }
                return items;
            }
            return null;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        /// <param name="value">The target data being passed to the source.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
