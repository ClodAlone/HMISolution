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
using Syncfusion.Windows.ReportDesigner.Resources;
using System.Globalization;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for PanelGeneral.xaml
    /// </summary>
    internal partial class PanelGeneral : UserControl
    {
        public PanelGeneral()
        {
            InitializeComponent();
            this.DatasetName.SelectionChanged += new SelectionChangedEventHandler(DatasetName_SelectionChanged);
        }

        void DatasetName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selection = (sender as ComboBox);

            if (!string.IsNullOrEmpty(selection.SelectionBoxItem.ToString()) && !(selection.SelectionBoxItem.Equals(selection.SelectedValue)))
            {
                MessageBoxResult result = MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture,"msgBoxChangeDataSet"),
                       SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner"), MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.No)
                {
                    this.cmb_datasetName.Text = selection.Text;
                    this.cmb_datasetName.SelectedItem = selection.SelectionBoxItem;
                    this.cmb_datasetName.SelectedValue = selection.SelectionBoxItem;
                }
            }
        }

        #region Public Properties

        public new TextBox Name
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

        public ComboBox DatasetName
        {
            get
            {
                return this.cmb_datasetName;
            }
            set
            {
                this.cmb_datasetName = value;
            }
        }

        public CheckBox AddPageBreakBefore
        {
            get
            {
                return this.chk_AddPageBreakBefore;
            }
            set
            {
                this.chk_AddPageBreakBefore = value;
            }
        }

        public CheckBox AddPageBreakAfter
        {
            get
            {
                return this.chk_AddPageBreakAfter;
            }
            set
            {
                this.chk_AddPageBreakAfter = value;
            }
        }
        #endregion
    }
}
