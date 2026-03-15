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
    internal partial class TablixGeneral
        : UserControl
    {
        #region PrivateProperties

            RDL.DOM.DataSet DataSet { get; set; }

        #endregion

        #region Constructor

        public TablixGeneral()
        {
            InitializeComponent();
        }

        public TablixGeneral(Controls.TablixControl tablix)
        {
            InitializeComponent();
            this.DataSet = tablix.DataSet;
        }

        #endregion

        #region Public Properties
        public new System.Windows.Controls.TextBox Name
        {
            get
            {
                return this.txt_GeneralName;
            }
            set
            {
                this.txt_GeneralName = value;
            }
        }

        public new System.Windows.Controls.TextBox ToolTip
        {
            get
            {
                return this.txt_ToolTip;
            }
            set
            {
                this.txt_ToolTip = value;
            }
        }

        public System.Windows.Controls.ComboBox DataSetName
        {
            get
            {
                return this.cmb_DatasetName;
            }
            set
            {
                this.cmb_DatasetName = value;
            }
        }

        public System.Windows.Controls.CheckBox RepeatColHeaders
        {
            get
            {
                return this.chk_RepeatColHeaders;
            }
            set
            {
                this.chk_RepeatColHeaders = value;
            }
        }

        public System.Windows.Controls.CheckBox KeepColHeaderVisible
        {
            get
            {
                return this.chk_KeepColHeaderVisible;
            }
            set
            {
                this.chk_KeepColHeaderVisible = value;
            }
        }
        public System.Windows.Controls.CheckBox RepeatRowHeaders
        {
            get
            {
                return this.chk_RepeatRowHeaders;
            }
            set
            {
                this.chk_RepeatRowHeaders = value;
            }
        }

        public System.Windows.Controls.CheckBox KeepRowHeaderVisible
        {
            get
            {
                return this.chk_KeepRowHeaderVisible;
            }
            set
            {
                this.chk_KeepRowHeaderVisible = value;
            }
        }

        #endregion

        #region EventMenthods

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBlock lable = new TextBlock();
                lable.Width = 70;
                lable.Text = this.lbx_GroupExpression.Items.Count == 0 ? "Group on" : "And on";
                ComboBox cmb_Exp = new ComboBox();
                cmb_Exp.Width = 260;
                cmb_Exp.IsEditable = true;
                cmb_Exp.Margin = new Thickness(1);
                this.UpdateDataSet(cmb_Exp);

                StackPanel stpnl = new StackPanel();
                stpnl.Orientation = System.Windows.Controls.Orientation.Horizontal;
                stpnl.Children.Add(lable);
                stpnl.Children.Add(cmb_Exp);
                this.lbx_GroupExpression.Items.Add(stpnl);
                stpnl.GotFocus += new RoutedEventHandler(stpnl_GotFocus);

                if (this.lbx_GroupExpression.Items.Count > 0)
                {
                    this.btn_Delete.IsEnabled = true;
                }

                this.lbx_GroupExpression.SelectedIndex = this.lbx_GroupExpression.Items.Count - 1;
            }
            catch { }
        }

        void stpnl_GotFocus(object sender, RoutedEventArgs e)
        {
            this.lbx_GroupExpression.SelectedItem = sender;
        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.lbx_GroupExpression.SelectedIndex == 0 && this.lbx_GroupExpression.Items.Count > 1)
                {
                    UpdateLable(1, "Group on");
                }
                this.lbx_GroupExpression.Items.RemoveAt(this.lbx_GroupExpression.SelectedIndex);
                this.lbx_GroupExpression.SelectedIndex = this.lbx_GroupExpression.Items.Count - 1;
            }
            catch { }
            if (this.lbx_GroupExpression.Items.Count < 1)
            {
                this.btn_Delete.IsEnabled = false;
            }
        }

        private void UpdateDataSet(ComboBox cmb_Exp)
        {
            if (this.DataSet != null)
            {
                foreach (var field in this.DataSet.Fields)
                {
                    cmb_Exp.Items.Add("[" + field.Name + "]");
                }
            }
        }

        private void UpdateLable(int index, string text)
        {
            try
            {
                StackPanel panel = (StackPanel)this.lbx_GroupExpression.Items[index];
                TextBlock lable = panel.Children.OfType<TextBlock>().ToList().First();
                lable.Text = text;
                this.lbx_GroupExpression.Items.RemoveAt(index);
                this.lbx_GroupExpression.Items.Insert(index, panel);
            }
            catch { }
        }

        #endregion
    }
}
