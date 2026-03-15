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
using System.Windows.Shapes;
using System.ComponentModel;
using System.Reflection;
using Syncfusion.Windows.ReportDesigner.Resources;
using System.Globalization;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for ReportReferences.xaml
    /// </summary>
#if SyncfusionFramework4_0
        [DesignTimeVisible(false)]
#endif
    internal partial class ReportReferences : UserControl
    {
        #region Members

        #endregion

        #region Constructor

        public ReportReferences()
        {
            InitializeComponent();
        }

        public ReportReferences(RDL.DOM.ReportDefinition reportDefinition)
        {
            InitializeComponent();

            if (reportDefinition.Classes != null)
            {
                if (reportDefinition.Classes.Count > 0)
                {
                    foreach (var _class in reportDefinition.Classes)
                    {
                        TextBox txtbox_className = new TextBox();
                        txtbox_className.Text = _class.ClassName;
                        txtbox_className.Width = 170;
                        TextBox txtbox_InstanceName = new TextBox();
                        txtbox_InstanceName.Text = _class.InstanceName;
                        txtbox_InstanceName.Width = 170;

                        StackPanel stpnl = new StackPanel();
                        stpnl.Orientation = System.Windows.Controls.Orientation.Horizontal;
                        stpnl.Children.Add(txtbox_className);
                        stpnl.Children.Add(txtbox_InstanceName);
                        this.lbx_Classes.Items.Add(stpnl);
                        stpnl.GotFocus += new RoutedEventHandler(stpnl_GotFocus);

                        if (this.lbx_Classes.Items.Count > 1)
                        {
                            this.btn_Class_Delete.IsEnabled = true;
                        }
                        if (this.lbx_Classes.Items.Count > 2)
                        {
                            this.btn_Class_UpArrow.IsEnabled = true;
                            this.btn_Class_DownArrow.IsEnabled = true;
                        }
                    }

                    this.lbx_Classes.SelectedIndex = this.lbx_Classes.Items.Count - 1;
                }
            }
            if (reportDefinition.CodeModules != null)
            {
                if (reportDefinition.CodeModules.Count > 0)
                {
                    foreach (var _module in reportDefinition.CodeModules)
                    {
                        TextBox textBox_Assembly = new TextBox();
                        textBox_Assembly.Text = _module.Value;
                        textBox_Assembly.Width = 300;
                        Button btn = new Button();
                        btn.Width = 30;
                        btn.Margin = new Thickness(10, 0, 0, 0);
                        btn.Content = "....";
                        
                        StackPanel stpnl = new StackPanel();
                        stpnl.Orientation = System.Windows.Controls.Orientation.Horizontal;
                        stpnl.Children.Add(textBox_Assembly);
                        stpnl.Children.Add(btn);
                        this.lbx_Assemblies.Items.Add(stpnl);
                        stpnl.GotFocus += new RoutedEventHandler(stpnl_GotFocus);

                        if (this.lbx_Assemblies.Items.Count > 0)
                        {
                            this.btn_Delete.IsEnabled = true;
                        }
                        if (this.lbx_Assemblies.Items.Count >= 2)
                        {
                            this.btn_UpArrow.IsEnabled = true;
                            this.btn_DownArrow.IsEnabled = true;
                        }
                    }

                    this.lbx_Assemblies.SelectedIndex = this.lbx_Assemblies.Items.Count - 1;
                }
            }
        }        

        #endregion

        #region event methods

        #region grd_AddAssembly

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBox textBox_Assembly = new TextBox();
                textBox_Assembly.Width = 300;

                Button btn = new Button();
                btn.Width = 30;
                btn.Margin = new Thickness(10, 0, 0, 0);
                btn.Content = "....";
                StackPanel stpnl = new StackPanel();
                stpnl.Orientation = System.Windows.Controls.Orientation.Horizontal;
                stpnl.Children.Add(textBox_Assembly);
                stpnl.Children.Add(btn);
                this.lbx_Assemblies.Items.Add(stpnl);
                stpnl.GotFocus += new RoutedEventHandler(stpnl_GotFocus);

                if (this.lbx_Assemblies.Items.Count > 0)
                {
                    this.btn_Delete.IsEnabled = true;
                }
                if (this.lbx_Assemblies.Items.Count >= 2)
                {
                    this.btn_UpArrow.IsEnabled = true;
                    this.btn_DownArrow.IsEnabled = true;
                }

                this.lbx_Assemblies.SelectedIndex = this.lbx_Assemblies.Items.Count - 1;
            }
            catch { }
        }       

        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.lbx_Assemblies.Items.RemoveAt(this.lbx_Assemblies.SelectedIndex);
                this.lbx_Assemblies.SelectedIndex = this.lbx_Assemblies.Items.Count - 1;
            }
            catch { }
            if (this.lbx_Assemblies.Items.Count < 1)
            {
                this.btn_Delete.IsEnabled = false;
            }
            if (this.lbx_Assemblies.Items.Count < 2)
            {
                this.btn_UpArrow.IsEnabled = false;
                this.btn_DownArrow.IsEnabled = false;
            }
        } 
        private void btn_UpArrow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.lbx_Assemblies.SelectedIndex != 0)
                {
                    int index = this.lbx_Assemblies.SelectedIndex;
                    var tempItem = this.lbx_Assemblies.SelectedItem;
                    this.lbx_Assemblies.Items.RemoveAt(index);
                    this.lbx_Assemblies.Items.Insert(index - 1, tempItem);
                    this.lbx_Assemblies.SelectedIndex = index - 1;
                }                
            }
            catch { }
        }

        private void btn_DownArrow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.lbx_Assemblies.SelectedIndex != this.lbx_Assemblies.Items.Count-1)
                {
                    int index = this.lbx_Assemblies.SelectedIndex;
                    var tempItem = this.lbx_Assemblies.SelectedItem;
                    this.lbx_Assemblies.Items.RemoveAt(index);
                    this.lbx_Assemblies.Items.Insert(index + 1, tempItem);
                    this.lbx_Assemblies.SelectedIndex = index + 1;
                }
            }
            catch { }
        }      

        void stpnl_GotFocus(object sender, RoutedEventArgs e)
        {
            this.lbx_Assemblies.SelectedItem = sender;

            if (e.Source is Button)
            {
                System.Windows.Forms.OpenFileDialog openDialog = new System.Windows.Forms.OpenFileDialog();
                openDialog.Filter = "(*.dll)|*.dll|All files (*.*)|*.*";
                openDialog.Multiselect = false;

                if (openDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        Assembly assembly = Assembly.LoadFile(openDialog.FileName);
                        System.Windows.Controls.TextBox textbox = (sender as StackPanel).Children.OfType<System.Windows.Controls.TextBox>().ToList().First();
                        textbox.Text = assembly.FullName;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner"), MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        #endregion

        #region grd_AddClasses

        private void btn_Class_Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBox textBox_Class = new TextBox();
                textBox_Class.Width = 170;
                TextBox textBox_Instance = new TextBox();
                textBox_Instance.Width = 170;
                StackPanel stpnl_Class = new StackPanel();
                stpnl_Class.Orientation = System.Windows.Controls.Orientation.Horizontal;
                stpnl_Class.Children.Add(textBox_Class);
                stpnl_Class.Children.Add(textBox_Instance);
                this.lbx_Classes.Items.Add(stpnl_Class);
                stpnl_Class.GotFocus += new RoutedEventHandler(stpnl_Class_GotFocus);

                if (this.lbx_Classes.Items.Count > 1)
                {
                    this.btn_Class_Delete.IsEnabled = true;
                }
                if (this.lbx_Classes.Items.Count > 2)
                {
                    this.btn_Class_UpArrow.IsEnabled = true;
                    this.btn_Class_DownArrow.IsEnabled = true;
                }

                this.lbx_Classes.SelectedIndex = this.lbx_Classes.Items.Count - 1;
            }
            catch { }
        }

        void stpnl_Class_GotFocus(object sender, RoutedEventArgs e)
        {
            this.lbx_Classes.SelectedItem = sender;
        }

        private void btn_Class_Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.lbx_Classes.Items.RemoveAt(this.lbx_Classes.SelectedIndex);
                this.lbx_Classes.SelectedIndex = this.lbx_Classes.Items.Count - 1;
            }
            catch { }
            if (this.lbx_Classes.Items.Count <= 1)
            {
                this.btn_Class_Delete.IsEnabled = false;
            }
            if (this.lbx_Classes.Items.Count <= 2)
            {
                this.btn_Class_UpArrow.IsEnabled = false;
                this.btn_Class_DownArrow.IsEnabled = false;
            }
        }
        private void btn_Class_UpArrow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.lbx_Classes.SelectedIndex > 1)
                {
                    int index = this.lbx_Classes.SelectedIndex;
                    var tempItem = this.lbx_Classes.SelectedItem;
                    this.lbx_Classes.Items.RemoveAt(index);
                    this.lbx_Classes.Items.Insert(index - 1, tempItem);
                    this.lbx_Classes.SelectedIndex = index - 1;
                }
            }
            catch { }
        }

        private void btn_Class_DownArrow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.lbx_Classes.SelectedIndex != this.lbx_Classes.Items.Count-1)
                {
                    int selectedIndex = this.lbx_Classes.SelectedIndex;
                    var temp = this.lbx_Classes.SelectedItem;
                    this.lbx_Classes.Items.RemoveAt(selectedIndex);
                    this.lbx_Classes.Items.Insert(selectedIndex + 1, temp);
                    this.lbx_Classes.SelectedIndex = selectedIndex + 1;
                }
            }
            catch { }
        }

        #endregion        
         
        #endregion
    }
}
