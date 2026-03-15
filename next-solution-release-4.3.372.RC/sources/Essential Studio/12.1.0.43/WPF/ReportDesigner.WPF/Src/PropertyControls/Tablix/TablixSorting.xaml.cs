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
using System.ComponentModel;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for Tablix.xaml
    /// </summary>
#if SyncfusionFramework4_0
    [DesignTimeVisible(false)]
#endif

    internal partial class TablixSorting : UserControl
    {
        #region private properties

        private   RDL.DOM.DataSet dataSet;

        #endregion

        #region constructor

        public TablixSorting()
        {
            InitializeComponent();
        }

        public TablixSorting(RDL.DOM.DataSet dataSet, RDL.DOM.SortExpressions sortExpressions)
        {
            InitializeComponent();
            this.dataSet = dataSet;

            if (sortExpressions != null)
            {
                foreach (var expresion in sortExpressions)
                {
                    TextBlock lable = new TextBlock();
                    lable.Width = 50;

                    if (!(this.lbx_SortingOptions.Items.Count > 1))
                    {
                        lable.Text = "Sort by";
                    }
                    else
                    {
                        lable.Text = "Then by";
                    }

                    ComboBox cmb_Column = new ComboBox();
                    cmb_Column.IsEditable = true;
                    cmb_Column.Width = 230;
                    if (!string.IsNullOrEmpty(expresion.Value))
                    {
                        this.UpdateColumnList(cmb_Column);
                        cmb_Column.Text = this.ConvertExpressionToField(expresion.Value);
                    }

                    ComboBox cmb_Order = new ComboBox();
                    cmb_Order.Width = 80;
                    cmb_Order.Margin = new Thickness(5, 0, 0, 0);
                    cmb_Order.Items.Add("A to Z");
                    cmb_Order.Items.Add("Z to A");
                    cmb_Order.Text = expresion.Direction.Equals(RDL.DOM.SortDirection.Ascending) ? "A to Z" : "Z to A";

                    StackPanel stpnl = new StackPanel();
                    stpnl.Orientation = System.Windows.Controls.Orientation.Horizontal;
                    stpnl.Children.Add(lable);
                    stpnl.Children.Add(cmb_Column);
                    stpnl.Children.Add(cmb_Order);

                    this.lbx_SortingOptions.Items.Add(stpnl);
                    stpnl.GotFocus += new RoutedEventHandler(stpnl_GotFocus);

                    if (this.lbx_SortingOptions.Items.Count > 0)
                    {
                        this.btn_Delete.IsEnabled = true;
                    }
                    if (this.lbx_SortingOptions.Items.Count > 1)
                    {
                        this.btn_UpArrow.IsEnabled = true;
                        this.btn_DownArrow.IsEnabled = true;
                    }

                    this.lbx_SortingOptions.SelectedIndex = this.lbx_SortingOptions.Items.Count - 1;
                }
            }
        }

        
        #endregion

        #region event methohds

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBlock lable = new TextBlock();
                lable.Width = 50;

                if (!(this.lbx_SortingOptions.Items.Count > 1))
                {
                    lable.Text = "Sort by";
                }
                else
                {
                    lable.Text = "Then by";
                }

                ComboBox cmb_Column = new ComboBox();
                cmb_Column.IsEditable = true;
                cmb_Column.Width = 230;
                this.UpdateColumnList(cmb_Column);

                ComboBox cmb_Order = new ComboBox();
                cmb_Order.Width = 80;
                cmb_Order.Margin = new Thickness(5, 0, 0, 0);
                cmb_Order.Items.Add("A to Z");
                cmb_Order.Items.Add("Z to A");
                cmb_Order.SelectedItem = cmb_Order.Items[0];

                StackPanel stpnl = new StackPanel();
                stpnl.Orientation = System.Windows.Controls.Orientation.Horizontal;
                stpnl.Children.Add(lable);
                stpnl.Children.Add(cmb_Column);
                stpnl.Children.Add(cmb_Order);

                this.lbx_SortingOptions.Items.Add(stpnl);
                stpnl.GotFocus += new RoutedEventHandler(stpnl_GotFocus);

                if (this.lbx_SortingOptions.Items.Count > 0)
                {
                    this.btn_Delete.IsEnabled = true;
                }
                if (this.lbx_SortingOptions.Items.Count > 1)
                {
                    this.btn_UpArrow.IsEnabled = true;
                    this.btn_DownArrow.IsEnabled = true;
                }

                this.lbx_SortingOptions.SelectedIndex = this.lbx_SortingOptions.Items.Count - 1;
            }
            catch { }
        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.lbx_SortingOptions.SelectedIndex == 1 && this.lbx_SortingOptions.Items.Count > 2)
                {
                    UpdateLable(2,"Sort by");
                }
                this.lbx_SortingOptions.Items.RemoveAt(this.lbx_SortingOptions.SelectedIndex);
                this.lbx_SortingOptions.SelectedIndex = this.lbx_SortingOptions.Items.Count - 1;
            }
            catch { }
            if (this.lbx_SortingOptions.Items.Count <= 1)
            {
                this.btn_Delete.IsEnabled = false;
            }
            if (this.lbx_SortingOptions.Items.Count <= 2)
            {
                this.btn_UpArrow.IsEnabled = false;
                this.btn_DownArrow.IsEnabled = false;
            }
        }

        private void btn_UpArrow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.lbx_SortingOptions.SelectedIndex == 2)
                {
                    UpdateLable(2, "Sort by");
                    UpdateLable(1, "Then by");
                    this.lbx_SortingOptions.SelectedIndex = 2;
                }
                if (this.lbx_SortingOptions.SelectedIndex > 1)
                {
                    int index = this.lbx_SortingOptions.SelectedIndex;
                    var tempItem = this.lbx_SortingOptions.SelectedItem;
                    this.lbx_SortingOptions.Items.RemoveAt(index);
                    this.lbx_SortingOptions.Items.Insert(index - 1, tempItem);
                    this.lbx_SortingOptions.SelectedIndex = index - 1;
                }
            }
            catch { }
        }

        private void btn_DownArrow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.lbx_SortingOptions.SelectedIndex == 1)
                {
                    UpdateLable(2, "Sort by");
                    UpdateLable(1, "Then by");
                    this.lbx_SortingOptions.SelectedIndex = 1;
                }
                if (this.lbx_SortingOptions.SelectedIndex != this.lbx_SortingOptions.Items.Count - 1)
                {
                    int index = this.lbx_SortingOptions.SelectedIndex;
                    var tempItem = this.lbx_SortingOptions.SelectedItem;
                    this.lbx_SortingOptions.Items.RemoveAt(index);
                    this.lbx_SortingOptions.Items.Insert(index + 1, tempItem);
                    this.lbx_SortingOptions.SelectedIndex = index + 1;
                }
            }
            catch { }
        }

        void stpnl_GotFocus(object sender, RoutedEventArgs e)
        {
            this.lbx_SortingOptions.SelectedItem = sender;
        }

        #endregion

        #region Helper Methods

        private void UpdateColumnList(ComboBox cmb_Column)
        {
            if (this.dataSet != null)
            {
                foreach (var field in this.dataSet.Fields)
                {
                    cmb_Column.Items.Add("["+field.Name+"]");
                }
            }
        }

        private void UpdateLable(int index,string text)
        {
            try
            {
                StackPanel panel = (StackPanel)this.lbx_SortingOptions.Items[index];
                TextBlock lable = panel.Children.OfType<TextBlock>().ToList().First();
                lable.Text = text;
                this.lbx_SortingOptions.Items.RemoveAt(index);
                this.lbx_SortingOptions.Items.Insert(index, panel);
            }
            catch { }

        }

        private string ConvertExpressionToField(string expression)
        {
            if (expression.StartsWith("=Fields!") && expression.EndsWith(".Value"))
            {
                expression = expression.Replace("=Fields!", "[");
                expression = expression.Replace(".Value", "]");
            }

            return expression;
        }

        #endregion
    }
}
