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
using Syncfusion.RDL.Data;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Syncfusion.UI.Xaml.Reports
{
    public sealed class MultiValueComboBox : ComboBox
    {
        public MultiValueComboBox()
        {
            this.DefaultStyleKey = typeof(MultiValueComboBox);
        }

        ListBox listBox = null;
        internal ComboBox rootcombo = null;
        Popup popup = null;
        Grid rootgrid = null;
        TextBlock combotext = null;
        string displayText;
        bool itemChanged;
        bool internalChange;

        public List<bool> IsSelected
        {
            get;
            set;
        }

        public string DisplayText
        {
            get
            {
                return this.displayText;
            }
            set
            {
                this.displayText = value;
                this.Tag = this.displayText;
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            listBox = GetTemplateChild("comboListBox") as ListBox;
            popup = GetTemplateChild("PART_DropDown") as Popup;
            combotext = GetTemplateChild("DropDownToggle") as TextBlock;
            rootgrid = GetTemplateChild("rootgrid") as Grid;
            rootgrid.PointerPressed += rootgrid_PointerPressed;
            popup.Closed += popup_Closed;
            listBox.SelectionChanged += new SelectionChangedEventHandler(box_SelectionChanged);
            this.DropDownOpened += MulitValueComboBox_DropDownOpened;
            this.DropDownClosed += MulitValueComboBox_DropDownClosed;
            this.listBox.Loaded += box_Loaded;
        }

        void rootgrid_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            popup.IsOpen = true;

        }

        void popup_Closed(object sender, object e)
        {
            List<ParameterReportData> dataSource = this.ItemsSource as List<ParameterReportData>;
            string text="";
            foreach (ParameterReportData data in dataSource)
            {
                if (data.IsSelected && data.DisplayField != "(SelectAll)")
                {
                    text += data.DisplayField +",";
                }
            }
            combotext.Text = this.displayText=text;
        }

        void MulitValueComboBox_DropDownClosed(object sender, object e)
        {
            listBox.SelectionChanged -= new SelectionChangedEventHandler(box_SelectionChanged);
             popup.IsOpen = false;
        }

        void MulitValueComboBox_DropDownOpened(object sender, object e)
        {
             listBox.SelectionChanged += new SelectionChangedEventHandler(box_SelectionChanged);
            this.popup.IsOpen = true;
        }

        void box_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.itemChanged && this.IsSelected != null)
            {
                List<ParameterReportData> dataSource = this.ItemsSource as List<ParameterReportData>;
                int itemsCount = 0;
                this.itemChanged = false;
                this.internalChange = true;

                foreach (var obj in dataSource)
                {
                    obj.IsSelected = this.IsSelected[itemsCount++];
                }

                this.internalChange = false;
            }
        }

        void box_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!internalChange)
            {
                List<ParameterReportData> dataSource = this.ItemsSource as List<ParameterReportData>;
                if (e.AddedItems.Count > 0)
                {
                    internalChange = true;
                    ParameterReportData data = e.AddedItems[0] as ParameterReportData;

                    if (data.IsSelected && data.DisplayField == "(SelectAll)")
                    {
                        foreach (var obj in dataSource)
                        {
                            obj.IsSelected = true;
                        }
                    }

                    internalChange = false;
                }
                else if (e.RemovedItems.Count > 0)
                {
                    internalChange = true;
                    ParameterReportData data = e.RemovedItems[0] as ParameterReportData;

                    if (!data.IsSelected && data.DisplayField == "(SelectAll)")
                    {
                        foreach (var obj in dataSource)
                        {
                            obj.IsSelected = false;
                        }
                    }
                    else
                    {
                        var value = from parm in dataSource
                                    where (parm.DisplayField == "(SelectAll)")
                                    select parm;

                        value.First().IsSelected = false;
                    }

                    internalChange = false;
                }
            }
        }
    }

    public class MultiColumnComboBoxHelper
    {
        public static bool GetEnableBinding(FrameworkElement element)
        {
            return (bool)element.GetValue(EnableBindingProperty);
        }

        public static void SetEnableBinding(FrameworkElement element, MultiColumnComboBoxHelper value)
        {
            element.SetValue(EnableBindingProperty, value);
        }

        public static readonly DependencyProperty EnableBindingProperty =
            DependencyProperty.RegisterAttached(
                "EnableBinding",
                typeof(bool),
                typeof(MultiColumnComboBoxHelper),
                new PropertyMetadata(false, EnableBindingPropertyChanged));

        private static void EnableBindingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Binding bin = new Binding();
            bin.Path = new PropertyPath("IsSelected");
            bin.Mode = BindingMode.TwoWay;
            ((FrameworkElement)d).SetBinding(ListBoxItem.IsSelectedProperty, bin);
        }

    }
}
