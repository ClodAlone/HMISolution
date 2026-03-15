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

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for ReportVariables.xaml
    /// </summary>
    internal partial class ReportVariables : UserControl
    {
        
        #region constructor

        public ReportVariables()
        {
            InitializeComponent();
        }

        public ReportVariables(RDL.DOM.Variables variables)
        {
            InitializeComponent();

            if (variables != null)
            {
                foreach (var variable in variables)
                {
                    TextBox txtbox_Name = new TextBox();

                    if (string.IsNullOrEmpty(variable.Name))
                    {
                        txtbox_Name.Text = string.Empty;
                    }
                    else
                    {
                        txtbox_Name.Text = variable.Name;
                    }

                    txtbox_Name.Width = 170;
                    TextBox txtbox_Value = new TextBox();

                    if (string.IsNullOrEmpty(variable.Value))
                    {
                        txtbox_Value.Text = string.Empty;
                    }
                    else
                    {
                        txtbox_Value.Text = variable.Value;
                    }

                    txtbox_Value.Width = 170;
                    StackPanel stpnl = new StackPanel();
                    stpnl.Orientation = System.Windows.Controls.Orientation.Horizontal;
                    stpnl.Children.Add(txtbox_Name);
                    stpnl.Children.Add(txtbox_Value);
                    this.lbx_Variables.Items.Add(stpnl);
                    stpnl.GotFocus += new RoutedEventHandler(stpnl_GotFocus);

                    if (this.lbx_Variables.Items.Count > 1)
                    {
                        this.btn_Delete.IsEnabled = true;
                    }
                }
            }
        }

        #endregion

        #region Event methods

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBox txtbox_Name = new TextBox();
                txtbox_Name.Width = 170;
                TextBox txtbox_Value = new TextBox();
                txtbox_Value.Width = 170;
                StackPanel stpnl = new StackPanel();
                stpnl.Orientation = System.Windows.Controls.Orientation.Horizontal;
                stpnl.Children.Add(txtbox_Name);
                stpnl.Children.Add(txtbox_Value);
                this.lbx_Variables.Items.Add(stpnl);
                stpnl.GotFocus += new RoutedEventHandler(stpnl_GotFocus);

                if (this.lbx_Variables.Items.Count > 1)
                {
                    this.btn_Delete.IsEnabled = true;
                }

                this.lbx_Variables.SelectedIndex = this.lbx_Variables.Items.Count - 1;
            }
            catch { }
        }

        void stpnl_GotFocus(object sender, RoutedEventArgs e)
        {
            this.lbx_Variables.SelectedItem = sender;
        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.lbx_Variables.Items.RemoveAt(this.lbx_Variables.SelectedIndex);
                this.lbx_Variables.SelectedIndex = this.lbx_Variables.Items.Count - 1;
            }
            catch { }

            if (this.lbx_Variables.Items.Count < 2)
            {
                this.btn_Delete.IsEnabled = false;
            }
        }

        #endregion
    }
}
