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
using System.Windows.Controls;
using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Input;
using Syncfusion.Windows.ReportDesigner.Resources;
using System.Globalization;

namespace Syncfusion.Windows.Reports.Designer.Editors
{
    internal class DesignerTextBox:TextBox
    {
        public static readonly DependencyProperty TextValueProperty = DependencyProperty.Register("TextValue", typeof(string), typeof(DesignerTextBox), new UIPropertyMetadata(string.Empty));

        public string TextValue
        {
            get { return (string)GetValue(TextValueProperty); }
            set { SetValue(TextValueProperty, value); }
        }

        public static readonly DependencyProperty ValueTypeProperty = DependencyProperty.Register("ValueType", typeof(ValueType), typeof(DesignerTextBox), new UIPropertyMetadata(ValueType.None));

        public ValueType ValueType
        {
            get { return (ValueType)GetValue(ValueTypeProperty); }
            set { SetValue(ValueTypeProperty, value); }
        }

        public DesignerTextBox() : base()
        {
            this.SetResourceReference(StyleProperty, typeof(TextBox));
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("TextValue");
            binding.Mode = BindingMode.OneWay;
            this.SetBinding(DesignerTextBox.TextProperty, binding);
            this.BorderBrush = System.Windows.Media.Brushes.Transparent;
            this.BorderThickness = new Thickness(0);
            this.LostFocus += new RoutedEventHandler(DesignerTextBox_LostFocus);
            this.KeyDown += new System.Windows.Input.KeyEventHandler(DesignerTextBox_KeyDown);
        }

        void DesignerTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.ValidateAndUpdate(this.Text);
            }
        }
        
        void DesignerTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.ValidateAndUpdate(this.Text);
        }

        bool IsMessageBoxShow;

        void ValidateAndUpdate(string value)
        {
            IsMessageBoxShow = false;

            switch (this.ValueType)
            {
                case ValueType.Name:

                    if (this.Text != this.TextValue)
                    {
                        bool available = ((from name in ReportDesignView.CurrentPanel.reportItems
                                            where name.ItemName.Equals(value)
                                            select name).Count()) > 0 ? true : false;

                        IsMessageBoxShow = available;
                    }
                    if (string.IsNullOrEmpty(this.Text))
                    {
                        IsMessageBoxShow = true;
                    }

                    break;

                case ValueType.Horizontal:
                case ValueType.Vertical:
                case ValueType.PageHeight:
                case ValueType.BodyHeight:
                case ValueType.ReportWidth:
                case ValueType.PageWidth:
                case ValueType.FooterHeight:
                case ValueType.HeaderHeight:
                case ValueType.Height:
                case ValueType.Width:
                case ValueType.Left:
                case ValueType.Top:
                    try
                    {
                        Regex _regexChar = new Regex("[a-zA-Z]+");
                        Match m = _regexChar.Match(value);

                        if (string.IsNullOrEmpty(m.Value))
                        {
                            if (Dialogs.ControlProperties.UnitType == RDL.DOM.ReportUnitType.In)
                            {
                                value = new RDL.DOM.Size(value + "in").size;
                            }
                            else
                            {
                                value = new RDL.DOM.Size(value + "cm").size;
                            }
                        }
                        else
                        {
                            value = new RDL.DOM.Size(value).size;
                        }
                    }
                    catch
                    {
                        IsMessageBoxShow = true;
                    }
                    break;

                case ValueType.ListLevel:
                    if (Convert.ToInt64(value) > 9)
                        value = "9";
                    else if (Convert.ToInt64(value) < 0)
                        value = "0";
                    break;
            }

            if (IsMessageBoxShow == true)
            {
                this.Text = this.TextValue;
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxNotValidProperty"), SR.GetString(CultureInfo.CurrentUICulture, "titleControlProperty"), MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            }
            else
            {
                this.SetValue(DesignerTextBox.TextValueProperty, value);
            }
        }
    }
}
