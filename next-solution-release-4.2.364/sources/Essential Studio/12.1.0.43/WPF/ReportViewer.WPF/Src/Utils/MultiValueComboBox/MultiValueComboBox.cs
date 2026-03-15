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
using Syncfusion.RDL.Data;

namespace Syncfusion.Windows.Reports.Viewer.Utils
{
    class MulitValueComboBox : ComboBox
    {
        ListBox listBox = null;
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

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            this.itemChanged = true;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            listBox = this.GetTemplateChild("PART_comboListBox") as ListBox;
            this.DropDownOpened += new EventHandler(MulitValueComboBox_DropDownOpened);
            this.DropDownClosed += new EventHandler(MulitValueComboBox_DropDownClosed);
            this.listBox.Loaded += new RoutedEventHandler(box_Loaded);
        }

        void box_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.itemChanged && this.IsSelected!=null)
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

        void MulitValueComboBox_DropDownClosed(object sender, EventArgs e)
        {
            listBox.SelectionChanged -= new SelectionChangedEventHandler(box_SelectionChanged);
        }

        void MulitValueComboBox_DropDownOpened(object sender, EventArgs e)
        {
            listBox.SelectionChanged += new SelectionChangedEventHandler(box_SelectionChanged);
        }

        void box_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!internalChange)
            {
                List<ParameterReportData> dataSource = this.ItemsSource as List<ParameterReportData>;
                if (e.AddedItems.Count > 0 )
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
            bin.BindsDirectlyToSource = true;
            bin.Mode = BindingMode.TwoWay;
            ((FrameworkElement)d).SetBinding(ListBoxItem.IsSelectedProperty, bin);
        }
    }
}