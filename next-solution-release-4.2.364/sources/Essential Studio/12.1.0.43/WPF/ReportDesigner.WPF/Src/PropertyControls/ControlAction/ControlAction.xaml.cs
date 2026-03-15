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
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Reports.Designer.Editors;
using Syncfusion.Windows.Reports.Designer.Wizard;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for ImageAction.xaml
    /// </summary>
    internal partial class ControlAction : UserControl
    {
        #region properties

        RDL.DOM.DataSets Datasets { get; set; }

        #endregion

        #region public properties

        public string ServerUrl
        {
            get;
            set;
        }

        #endregion

        #region constructor

        public ControlAction()
        {
            InitializeComponent();
        }

        public ControlAction(RDL.DOM.Action action,RDL.DOM.DataSets Datasets)
        {
            InitializeComponent();
            this.Datasets = Datasets;
            this.rbtn_None.Checked += new RoutedEventHandler(rbtn_None_Checked);
            this.rbtn_goToReport.Checked += new RoutedEventHandler(rbtn_goToReport_Checked);
            this.rbtn_goToUrl.Checked += new RoutedEventHandler(rbtn_goToUrl_Checked);
            this.btn_browse.Click += new RoutedEventHandler(btn_browse_Click);

            if (action != null && action.Drillthrough != null)
            {
                RDL.DOM.Parameters parameter = (RDL.DOM.Parameters)action.Drillthrough.Parameters;

                foreach (var param in parameter)
                {
                    TextBox txt_Name = new TextBox();
                    txt_Name.Text = param.Name;
                    txt_Name.Width = 170;
                    ComboBox cmb_Value = new ComboBox();
                    cmb_Value.Width = 170;
                    cmb_Value.IsEditable = true;
                    cmb_Value.Margin = new Thickness(5, 0, 0, 0);

                    if (!string.IsNullOrEmpty(param.Value))
                    {
                        cmb_Value.Items.Add(param.Value);
                        cmb_Value.SelectedItem = param.Value;
                    }

                    UpdateFieldValues(cmb_Value);

                    StackPanel stpnl = new StackPanel();
                    stpnl.Orientation = System.Windows.Controls.Orientation.Horizontal;
                    stpnl.Children.Add(txt_Name);
                    stpnl.Children.Add(cmb_Value);
                    this.lbx_Parameters.Items.Add(stpnl);
                    stpnl.GotFocus += new RoutedEventHandler(stpnl_GotFocus);

                    if (this.lbx_Parameters.Items.Count > 1)
                    {
                        this.btn_Delete.IsEnabled = true;
                    }
                }
            }
        }

        #endregion

        #region Event methods

        void btn_browse_Click(object sender, RoutedEventArgs e)
        {
            ReportOpenDialog openReport = new ReportOpenDialog();
            openReport.Owner = Window.GetWindow(this);
            SkinStorage.SetVisualStyle(openReport, SkinStorage.GetVisualStyle(openReport.Owner));
            openReport.DialogMode = DialogMode.Server;
            openReport.Title = "Select report";
            
            if (openReport.ShowDialog() == true)
            {
                this.txt_reportName.Text = openReport.FilePath;
                this.ServerUrl = openReport.ReportServerURL;
            }
        }        

        void rbtn_None_Checked(object sender, RoutedEventArgs e)
        {
            this.stpnl_Url.Visibility = System.Windows.Visibility.Hidden;
            this.stpnl_Report.Visibility = System.Windows.Visibility.Collapsed;
        }

        void rbtn_goToUrl_Checked(object sender, RoutedEventArgs e)
        {
            this.stpnl_Url.Visibility = System.Windows.Visibility.Visible;
            this.stpnl_Report.Visibility = System.Windows.Visibility.Collapsed;
        }

        void rbtn_goToReport_Checked(object sender, RoutedEventArgs e)
        {
            this.stpnl_Report.Visibility = System.Windows.Visibility.Visible;
            this.stpnl_Url.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBox txt_Name = new TextBox();
                txt_Name.Width = 170;
                ComboBox cmb_Value = new ComboBox();
                cmb_Value.Width = 170;
                cmb_Value.IsEditable = true;
                cmb_Value.Margin = new Thickness(5, 0, 0, 0);
                UpdateFieldValues(cmb_Value);

                StackPanel stpnl = new StackPanel();
                stpnl.Orientation = System.Windows.Controls.Orientation.Horizontal;
                stpnl.Children.Add(txt_Name);
                stpnl.Children.Add(cmb_Value);
                this.lbx_Parameters.Items.Add(stpnl);
                stpnl.GotFocus += new RoutedEventHandler(stpnl_GotFocus);

                if (this.lbx_Parameters.Items.Count > 1)
                {
                    this.btn_Delete.IsEnabled = true;
                }

                this.lbx_Parameters.SelectedIndex = this.lbx_Parameters.Items.Count - 1;
            }
            catch { }
        }

        void stpnl_GotFocus(object sender, RoutedEventArgs e)
        {
            this.lbx_Parameters.SelectedItem = sender;
        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.lbx_Parameters.Items.RemoveAt(this.lbx_Parameters.SelectedIndex);
                this.lbx_Parameters.SelectedIndex = this.lbx_Parameters.Items.Count - 1;
            }
            catch { }

            if (this.lbx_Parameters.Items.Count < 2)
            {
                this.btn_Delete.IsEnabled = false;
            }
        }

        private void UpdateFieldValues(ComboBox cmb_Value)
        {
            foreach (var set in this.Datasets)
            {
                foreach (var field in set.Fields)
                {
                    cmb_Value.Items.Add("=First(Fields!" + field.Name + ".Value,\"" + set.Name + "\")");
                }
            }
        }

        #endregion
    }
}
