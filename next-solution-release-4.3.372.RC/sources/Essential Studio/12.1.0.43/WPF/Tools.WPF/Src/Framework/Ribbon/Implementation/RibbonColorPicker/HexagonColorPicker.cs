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
using Syncfusion.Windows.Tools.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Threading;

namespace Syncfusion.Windows.Tools.Controls
{
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    /// <summary>
    /// Class represent the color picker dialog
    /// </summary>
    class ColorPickerDialog : Window
    {
        /// <summary>
        /// Represents the color picker dock panel
        /// </summary>
        public ColorPickerDockPanel cpdp;

        /// <summary>
        /// Represents the selected color
        /// </summary>
        public Color selectedColor = new Color();

        /// <summary>
        /// Represents the ok flag
        /// </summary>
        public bool ok;

        /// <summary>
        /// Represents the int flag flag
        /// </summary>
        public int flag = 0;

        /// <summary>
        /// Represents the hsv color
        /// </summary>
        public xColorEdit HSVColorPicker;

        /// <summary>
        /// Represents the  new label
        /// </summary>
        public Label lblNew;

        /// <summary>
        /// Represents the current label
        /// </summary>
        public Label lblCurrent;

        /// <summary>
        /// Represents the tabitem hsv
        /// </summary>
        internal TabItem tab_HSV;

        /// <summary>
        /// Represents the tabitem rgb
        /// </summary>
        internal TabItem tab_RGB;

        /// <summary>
        /// Represents the tab control
        /// </summary>
        internal TabControl tabcontrol1;

        /// <summary>
        /// Represents the ok button
        /// </summary>
        Button BtnOk;

        /// <summary>
        /// Represents the cancel button
        /// </summary>
        Button BtnCancel;

        /// <summary>
        /// Represents the color combobox
        /// </summary>
        ComboBox colorMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorPickerDialog"/> class.
        /// </summary>
        public ColorPickerDialog()
        {
            cpdp = new ColorPickerDockPanel();
            ColorPickerPanel = new StackPanel();
            ok = false;
            this.Title = "Colors";
            this.Content = cpdp;
            this.Width = 400;
            this.MinWidth = 400;
            this.Height = 300;
            this.MinHeight = 300;
            
            //this.Dispatcher.BeginInvoke(
            //        System.Windows.Threading.DispatcherPriority.SystemIdle,
            //        new Initialize(this.findRes));
            
        }

        /// <summary>
        /// HSVs the color picker_ color changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        void HSVColorPicker_ColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Color c = (Color)e.NewValue;
            
            lblNew.Background = new SolidColorBrush(c);
        }

        /// <summary>
        /// Represents the color picker stack panel
        /// </summary>
        StackPanel ColorPickerPanel;

        /// <summary>
        /// Fills the color picker panel.
        /// </summary>
        private void fillColorPickerPanel()
        {
            ColorPickerPanel.Children.Add((xColorEdit)this.FindResource("ColorPicker"));
            tab_HSV.Content = ColorPickerPanel;
        }

        /// <summary>
        /// Changes the mode.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ChangeMode(object sender, RoutedEventArgs e)
        {
           
            xColorEdit ce = this.HSVColorPicker;
            RadioButton rb = new RadioButton();
            
            rb = (RadioButton)ce.Template.FindName("ButtomH", ce);
            rb.Content = "QQQQQ";
            ce.UpdateLayout();
        }

        /// <summary>
        /// Oks the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Ok(object sender, RoutedEventArgs e)
        {
            this.selectedColor = ((SolidColorBrush)lblNew.Background).Color;
            this.ok = true;
            this.Close();
        }

        /// <summary>
        /// Cancels the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Cancel(object sender, RoutedEventArgs e)
        {
           
            this.selectedColor = ((SolidColorBrush)lblCurrent.Background).Color;
            this.Close();
        }

        /// <summary>
        /// Handles the Resized event of the Dialog control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> instance containing the event data.</param>
        private void Dialog_Resized(object sender, SizeChangedEventArgs e)
        {
            
            tabcontrol1.Width = ((Window)sender).ActualWidth - 150;
            HSVColorPicker.Height = ((Window)sender).ActualHeight - 60;

        }


        /// <summary>
        /// Tabchangeds the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void tabchanged(object sender, SelectionChangedEventArgs e)
        {
           
            temp = ((SolidColorBrush)lblNew.Background).Color;
                this.Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.SystemIdle,
                    new NextPrimeDelegate(this.DoTabChanged));
            
        }

        /// <summary>
        /// Represents the next prine delegate
        /// </summary>
        public delegate void NextPrimeDelegate();

        /// <summary>
        /// Represents the temp color
        /// </summary>
        Color temp;

        /// <summary>
        /// Does the tab changed.
        /// </summary>
        public void DoTabChanged()
        {
            xColorEdit ce = this.HSVColorPicker;
            RadioButton rb = new RadioButton();
            rb = (RadioButton)ce.Template.FindName("ButtomS", ce);

            if (rb == null)
            {
                this.Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.SystemIdle,
                    new NextPrimeDelegate(this.DoTabChanged));
            }
            else
            {
                
                ce.VisualizationStyle = xColorSelectionMode.HSV;
                ce.Color = temp;
                rb.IsChecked = true;
                ce.ApplyTemplate();
                ce.UpdateLayout();
            }
            

        }

        /// <summary>
        /// Represents the initialize delegate
        /// </summary>
        public delegate void Initialize();

        /// <summary>
        /// Finds the res.
        /// </summary>
        public void findRes()
        {
            HSVColorPicker= (xColorEdit)cpdp.Template.FindName("HSVColorPicker",cpdp);

            if (HSVColorPicker == null)
            {
                this.Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.SystemIdle,
                    new Initialize(this.findRes));
            }
            else
            {
                HSVColorPicker.ColorChanged += new PropertyChangedCallback(HSVColorPicker_ColorChanged);
                lblNew = (Label)cpdp.Template.FindName("lblNew", cpdp);
                tab_HSV = (TabItem)cpdp.Template.FindName("tab_HSV", cpdp);
                lblCurrent = (Label)cpdp.Template.FindName("lblCurrent", cpdp);
                lblCurrent.Background = new SolidColorBrush(this.selectedColor);
                tabcontrol1 = (TabControl)cpdp.Template.FindName("tabcontrol1", cpdp);
                this.SizeChanged += new SizeChangedEventHandler(Dialog_Resized);
                HSVColorPicker.ApplyTemplate();
                HSVColorPicker.UpdateLayout();

                colorMode = (ComboBox)HSVColorPicker.Template.FindName("ColorModeListBox", HSVColorPicker);
                colorMode.SelectionChanged += new SelectionChangedEventHandler(colorMode_SelectionChanged);

                Button UpDown;
                UpDown = (Button)HSVColorPicker.Template.FindName("HUp", HSVColorPicker);
                UpDown.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(UpDown_MouseDown);
                UpDown = (Button)HSVColorPicker.Template.FindName("SUp", HSVColorPicker);
                UpDown.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(UpDown_MouseDown);
                UpDown = (Button)HSVColorPicker.Template.FindName("VUp", HSVColorPicker);
                UpDown.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(UpDown_MouseDown);
                UpDown = (Button)HSVColorPicker.Template.FindName("HDown", HSVColorPicker);
                UpDown.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(UpDown_MouseDown);
                UpDown = (Button)HSVColorPicker.Template.FindName("SDown", HSVColorPicker);
                UpDown.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(UpDown_MouseDown);
                UpDown = (Button)HSVColorPicker.Template.FindName("VDown", HSVColorPicker);
                UpDown.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(UpDown_MouseDown);

                UpDown = (Button)HSVColorPicker.Template.FindName("RUp", HSVColorPicker);
                UpDown.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(UpDown_MouseDown);
                UpDown.Click += new RoutedEventHandler(UpDown_Click);
                UpDown = (Button)HSVColorPicker.Template.FindName("GUp", HSVColorPicker);
                UpDown.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(UpDown_MouseDown);
                UpDown = (Button)HSVColorPicker.Template.FindName("BUp", HSVColorPicker);
                UpDown.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(UpDown_MouseDown);
                UpDown = (Button)HSVColorPicker.Template.FindName("RDown", HSVColorPicker);
                UpDown.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(UpDown_MouseDown);
                UpDown = (Button)HSVColorPicker.Template.FindName("GDown", HSVColorPicker);
                UpDown.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(UpDown_MouseDown);
                UpDown = (Button)HSVColorPicker.Template.FindName("BDown", HSVColorPicker);
                UpDown.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(UpDown_MouseDown);

                tabcontrol1.Width = this.ActualWidth - 150;
                HSVColorPicker.Height = this.ActualHeight - 60;
                HSVColorPicker.Color = this.selectedColor;

                tabcontrol1.SelectionChanged+=new SelectionChangedEventHandler(tabchanged);
                temp = ((SolidColorBrush)lblNew.Background).Color;
                this.Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.SystemIdle,
                    new NextPrimeDelegate(this.DoTabChanged));
                BtnOk=(Button)cpdp.Template.FindName("BtnOk", cpdp);
                BtnOk.Click+=new RoutedEventHandler(Ok);
                BtnCancel = (Button)cpdp.Template.FindName("BtnCancel", cpdp);
                BtnCancel.Click+=new RoutedEventHandler(Cancel);
            }
           
        }

        /// <summary>
        /// Handles the MouseDown event of the UpDown control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void UpDown_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            
                Button b = (Button)sender;
                switch (b.Name)
                {
                    case "HUp":
                        if (HSVColorPicker.H <= 359)
                            HSVColorPicker.H += 1f;
                        break;
                    case "SUp":
                        if (HSVColorPicker.S < 1)
                            HSVColorPicker.S += 0.01f;
                        break;
                    case "VUp":
                        if (HSVColorPicker.V < 1)
                            HSVColorPicker.V += 0.01f;
                        break;
                    case "HDown":
                        if (HSVColorPicker.H >1)
                            HSVColorPicker.H -= 1f;
                        break;
                    case "SDown":
                        if (HSVColorPicker.S > 0.01)
                            HSVColorPicker.S -= 0.01f;
                        break;
                    case "VDown":
                        if (HSVColorPicker.V>0.01)
                            HSVColorPicker.V -= 0.01f;
                        break;

                    case "RUp":
                        if (HSVColorPicker.R <1)
                        HSVColorPicker.R += 0.00392f;
                        break;
                    case "GUp":
                        if (HSVColorPicker.G <1)
                            HSVColorPicker.G += 0.00392f;
                        break;
                    case "BUp":
                        if (HSVColorPicker.B <1)
                            HSVColorPicker.B += 0.00392f;
                        break;
                    case "RDown":
                        if (HSVColorPicker.R > 0.003)
                            HSVColorPicker.R -= 0.00392f;
                        break;
                    case "GDown":
                        if (HSVColorPicker.G > 0.003)
                            HSVColorPicker.G -= 0.00392f;
                        break;
                    case "BDown":
                        if (HSVColorPicker.B > 0.003)
                            HSVColorPicker.B -= 0.00392f;
                        break;
                }
        }

        /// <summary>
        /// Handles the Click event of the UpDown control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void UpDown_Click(object sender, RoutedEventArgs e)
        {
            

        }

        /// <summary>
        /// Handles the SelectionChanged event of the colorMode control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        void colorMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox lb = (ComboBox)sender;
            ColumnDefinition cd;
            
            if (lb.SelectedValue.ToString().Contains("HSV"))
            {
                cd=(ColumnDefinition) HSVColorPicker.Template.FindName("HSVLabel", HSVColorPicker);
                cd.MaxWidth = double.PositiveInfinity;
                cd = (ColumnDefinition)HSVColorPicker.Template.FindName("HSVText", HSVColorPicker);
                cd.MaxWidth = double.PositiveInfinity;
                cd = (ColumnDefinition)HSVColorPicker.Template.FindName("RGBLabel", HSVColorPicker);
                cd.MaxWidth = 0d;
                cd = (ColumnDefinition)HSVColorPicker.Template.FindName("RGBText", HSVColorPicker);
                cd.MaxWidth = 0d;
            }
            else
            {
                cd = (ColumnDefinition)HSVColorPicker.Template.FindName("HSVLabel", HSVColorPicker);
                cd.MaxWidth = 0d;
                cd = (ColumnDefinition)HSVColorPicker.Template.FindName("HSVText", HSVColorPicker);
                cd.MaxWidth = 0d;
                cd = (ColumnDefinition)HSVColorPicker.Template.FindName("RGBLabel", HSVColorPicker);
                cd.MaxWidth = double.PositiveInfinity;
                cd = (ColumnDefinition)HSVColorPicker.Template.FindName("RGBText", HSVColorPicker);
                cd.MaxWidth = double.PositiveInfinity;
            }
            
           
        }
    }
}
