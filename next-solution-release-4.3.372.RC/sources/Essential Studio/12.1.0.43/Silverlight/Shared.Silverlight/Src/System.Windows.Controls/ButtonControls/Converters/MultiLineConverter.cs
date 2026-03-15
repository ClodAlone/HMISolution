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
        /// 
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(String.IsNullOrEmpty(value as string)))
            {
                var items = new List<FrameworkElement>();
                var sender = parameter as IButtonAdv;
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
                        var horizontalTextAlignement = HorizontalAlignment.Center;                     

                        if (parameter is ButtonAdv)
                        {
                            horizontalTextAlignement = (parameter as ButtonAdv).HorizontalAlignment;                           
                        }

                        var text_1 = new TextBlock() { HorizontalAlignment = HorizontalAlignment.Center };
                        text_1.Text = text1.TrimStart().TrimEnd();
                        items.Add(text_1);

                        var text_2 = new TextBlock() { VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = horizontalTextAlignement };

                        text_2.Text = text2.Trim();

                        if (!(sender is DropDownButtonAdv))
                        {
                            text_1.Foreground = ((ButtonAdv)sender).Foreground;
                        text_1.FontFamily = ((ButtonAdv)sender).FontFamily;

                           text_1.FontSize = ((ButtonAdv)sender).FontSize;  
                            text_2.Foreground = ((ButtonAdv)sender).Foreground;
                            text_2.FontFamily = ((ButtonAdv)sender).FontFamily;
                            text_2.FontSize = ((ButtonAdv)sender).FontSize;

                          ButtonAdv button = sender as ButtonAdv;      
                            ItemsControl multilinepresenter = button._multilineTextLarge;
                           Binding bind = new Binding("Foreground");

                           bind.Source = multilinepresenter;
                            text_1.SetBinding(TextBlock.ForegroundProperty, bind);    
                            text_2.SetBinding(TextBlock.ForegroundProperty, bind);
                                                
                           text_1.FontFamily = button.FontFamily;     
                            text_1.FontSize = button.FontSize;
                            text_2.FontFamily = button.FontFamily;
                            text_2.FontSize = button.FontSize;
                            items.Add(text_2);
                        }
                        else
                        {
                            if (sender is DropDownButtonAdv)
                            {
                                     var dropdown = sender as DropDownButtonAdv;
                                ItemsControl multilinepresenter = dropdown._multilineTextLarge;

                                Binding bind = new Binding("Foreground");
                                bind.Source = multilinepresenter;
                                 StackPanel panel = new StackPanel() { Orientation = Orientation.Horizontal };
                                text_2.Foreground = ((DropDownButtonAdv)sender).Foreground;
                                text_1.Foreground = ((DropDownButtonAdv)sender).Foreground;                               
                                text_2.SetBinding(TextBlock.ForegroundProperty, bind);
                                text_1.SetBinding(TextBlock.ForegroundProperty, bind);                              
                                 panel.Children.Add(text_2);
                                 TextBlock text_arrow = new TextBlock() { Text = "6", FontFamily = new FontFamily("Webdings"), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, FontSize = 14 };
                                text_arrow.Foreground = ((DropDownButtonAdv)sender).Foreground;
                                text_arrow.SetBinding(TextBlock.ForegroundProperty, bind);       
                         
                                text_arrow.FontSize = 9.0;
                                panel.Children.Add(text_arrow);
                                items.Add(panel);
                            }
                        }
                    }
                    else
                    {
                        var text = new TextBlock();
                        text.Text = label;
                        if (sender is DropDownButtonAdv)
                         {
                           DropDownButtonAdv dropdownbutton1 = sender as DropDownButtonAdv;
                            ItemsControl multilinepresenter = dropdownbutton1._multilineTextLarge;

                           Binding bind = new Binding("Foreground");
                           bind.Source = multilinepresenter;

                           text.SetBinding(TextBlock.ForegroundProperty, bind);
                            items.Add(text);
                             TextBlock text_arrow = new TextBlock() { Text = "6", FontFamily = new FontFamily("Webdings"), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, FontSize = 14 };
                            text_arrow.Foreground = ((DropDownButtonAdv)sender).Foreground;
                            text_arrow.SetBinding(TextBlock.ForegroundProperty, bind);
                             text_arrow.FontSize = 9.0;
                             items.Add(text_arrow);
                         }
                         else
                         {
                            ButtonAdv dropdownbutton1 = sender as ButtonAdv;
                            ItemsControl multilinepresenter = dropdownbutton1._multilineTextLarge;
 
                            Binding bind = new Binding("Foreground");
                            bind.Source = multilinepresenter;


                            text.SetBinding(TextBlock.ForegroundProperty, bind);
                            items.Add(text);
                           
                             var text_2 = new TextBlock() { Text = " " };
                            text_2.Foreground = ((ButtonAdv)sender).Foreground;
                            text_2.SetBinding(TextBlock.ForegroundProperty, bind);
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
