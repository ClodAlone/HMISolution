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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Syncfusion.Windows.Shared;
using System.ComponentModel;
using Syncfusion.Windows.Reports.Designer.Controls;
using System.Reflection;
using Syncfusion.Windows.ReportDesigner.Resources;
using System.Globalization;

namespace Syncfusion.Windows.Reports.Designer.Editors
{
    internal class ListBrush
    {
        public Brush Brush { get; set; }

        public string Name { get; set; }
    }

    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    internal partial class CustomUIEditorDropDown : UserControl
    {
        ExpressionDialog dialog;

        private ReportingConvertorUtil convertor;

        private ReportDesignView reportDesignView;

        internal static readonly DependencyProperty ExpressionVisiblityProperty = DependencyProperty.Register("ExpressionVisiblity", typeof(Visibility), typeof(CustomUIEditorDropDown), new UIPropertyMetadata(Visibility.Visible, OnExpressionVisiblityPropertyChanged));

        internal Visibility ExpressionVisiblity
        {
            get { return (Visibility)GetValue(ExpressionVisiblityProperty); }
            set { SetValue(ExpressionVisiblityProperty, value); }
        }

        public static void OnExpressionVisiblityPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            CustomUIEditorDropDown customUIEditorDropDown = dependencyObject as CustomUIEditorDropDown;

            if (customUIEditorDropDown != null)
            {
                Visibility expressionVisiblity = (Visibility)e.NewValue;

                if (expressionVisiblity == Visibility.Collapsed)
                {
                    customUIEditorDropDown.hyperLinkBlock.Visibility = Visibility.Collapsed;
                }
                else if (expressionVisiblity == Visibility.Hidden)
                {
                    customUIEditorDropDown.hyperLinkBlock.Visibility = Visibility.Hidden;
                }
                else
                {
                    customUIEditorDropDown.hyperLinkBlock.Visibility = Visibility.Visible;
                }
            }          
        }

        internal static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(CustomUIEditorDropDown), new UIPropertyMetadata(string.Empty, OnTextPropertyChanged));

        internal string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static void OnTextPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            CustomUIEditorDropDown customUIEditorDropDown = dependencyObject as CustomUIEditorDropDown;

            if (customUIEditorDropDown != null)
            {
                string textValue = (string)e.NewValue;
                customUIEditorDropDown.backGroundColorBox.Background = customUIEditorDropDown.convertor.GetBackGroundColor(textValue);

                if (textValue != null && !textValue.Equals(customUIEditorDropDown.colorBox.Text))
                {
                    customUIEditorDropDown.colorBox.Text = textValue;
                }
            }
        }

        public CustomUIEditorDropDown()
        {
            InitializeComponent();
            convertor = new ReportingConvertorUtil();
            IntializeColors();
            WireEvents();
        }

        public CustomUIEditorDropDown(string name,string name1,ReportDesignView designview)
        {
            InitializeComponent();
            convertor = new ReportingConvertorUtil();
            this.reportDesignView = designview;
            IntializeColors();
            WireEvents();
        }


        public void IntializeColors()
        {
            List<ListBrush> brushes = new List<ListBrush>();
            System.Type brush = typeof(Brushes);
            PropertyInfo[] m = brush.GetProperties();

            foreach (PropertyInfo item in m)
            {
                Brush brush1 = (Brush)(new System.Windows.Media.BrushConverter().ConvertFromString(item.Name));
                brushes.Add(new ListBrush() { Brush = brush1, Name = item.Name });
            }

            this.colorListBox.ItemsSource = brushes;
        }

        void WireEvents()
        {
            this.popupButton.Checked += new RoutedEventHandler(popupButton_Checked);
            this.popupButton.Unchecked += new RoutedEventHandler(popupButton_Unchecked);
            this.Expression.Click += new RoutedEventHandler(Expression_Click);
            this.colorBox.PreviewKeyDown += new KeyEventHandler(colorBox_PreviewKeyDown);
            this.popupButton.GotFocus += new RoutedEventHandler(popupButton_GotFocus);
            this.popupContainer.Closed += new EventHandler(popupContainer_Closed);
            this.colorBox.TextChanged += new TextChangedEventHandler(colorBox_TextChanged);
            this.colorBox.LostFocus += new RoutedEventHandler(colorBox_LostFocus);
            this.colorBox.KeyDown += new System.Windows.Input.KeyEventHandler(colorBox_KeyDown);
            //this.dropDownControl.LostFocus += new RoutedEventHandler(dropDownControl_LostFocus);
        }

        void dropDownControl_LostFocus(object sender, RoutedEventArgs e)
        {
            this.popupButton.IsChecked = false;
        }

        void colorBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.ValidateAndUpdate();
            }
        }

        void colorBox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.ValidateAndUpdate();
        }

        void colorBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            System.Type brush = typeof(Brushes);
            PropertyInfo[] m = brush.GetProperties();
            foreach (PropertyInfo item in m)
            {
                if (item.Name.Equals((string)this.colorBox.Text,StringComparison.InvariantCultureIgnoreCase))
                {
                    this.Text = this.colorBox.Text;
                }
            }
        }

        void popupButton_Unchecked(object sender, RoutedEventArgs e)
        {
            this.popupContainer.IsOpen = false;
        }

        void popupButton_Checked(object sender, RoutedEventArgs e)
        {
            this.popupContainer.IsOpen = true;
            this.colorListBox.SelectionChanged += new SelectionChangedEventHandler(colorListBox_SelectionChanged);
        }

        void popupContainer_Closed(object sender, EventArgs e)
        {
            this.colorListBox.SelectionChanged -= new SelectionChangedEventHandler(colorListBox_SelectionChanged);
            if (!(this.dropDownControl.IsStylusOver || this.dropDownControl.IsMouseOver))
            {
                this.popupButton.IsChecked = false;
            }
        }

        void colorListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1)
            {
                ListBrush brush = this.colorListBox.SelectedItem as ListBrush;
                this.Text = brush.Name;
                this.popupButton.IsChecked = false;
            }
        }

        void popupButton_GotFocus(object sender, RoutedEventArgs e)
        {
            Color colorBoxValue;

            if (colorBox.Text.StartsWith("#"))
            {
                colorBoxValue = (Color)ColorConverter.ConvertFromString(colorBox.Text);
            }
            else
            {
                System.Drawing.Color colorValue = System.Drawing.Color.FromName(colorBox.Text);
                colorBoxValue = Color.FromRgb(colorValue.R, colorValue.G, colorValue.B);
            }
        }

        void colorBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                System.Drawing.Color colorValue = System.Drawing.Color.FromName(colorBox.Text);
                System.Windows.Media.Color colorBoxValue = System.Windows.Media.Color.FromRgb(colorValue.R, colorValue.G, colorValue.B);
            }
        }

        void Expression_Click(object sender, RoutedEventArgs e)
        {
            dialog = new ExpressionDialog(ValueType.FillStyle,"Color");
            Binding binding = new Binding();
            binding.Path = new PropertyPath("Text");
            binding.Source = this;
            binding.Mode = BindingMode.TwoWay;
            dialog.SetBinding(ExpressionDialog.TextProperty, binding);
            if (this.reportDesignView != null)
            {
                this.reportDesignView.UpdateOwnerWindow(dialog);
            }
            else
            {
                this.dialog.Owner = Window.GetWindow(this);
                SkinStorage.SetVisualStyle(this.dialog, SkinStorage.GetVisualStyle(this.dialog.Owner));
            }
            dialog.ShowDialog();
            BindingOperations.ClearBinding(dialog, ExpressionDialog.TextProperty);
        }

        private void ValidateAndUpdate()
        {
            try
            {
                if (!colorBox.Text.StartsWith("="))
                {
                    var colorValue = ColorConverter.ConvertFromString(colorBox.Text);
                }
            }
            catch
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxNotValidProperty"), SR.GetString(CultureInfo.CurrentUICulture, "titleControlProperty"), MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                colorBox.Text = this.Text;
            }
        }

    }
}
