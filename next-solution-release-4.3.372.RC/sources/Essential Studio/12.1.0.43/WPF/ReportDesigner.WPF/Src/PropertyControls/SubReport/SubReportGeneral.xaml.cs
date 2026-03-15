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
using System.Net;
using System.IO;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for SubReportGeneral.xaml
    /// </summary>
    internal partial class SubReportGeneral : UserControl
    {
        public SubReportGeneral()
        {
            InitializeComponent();
        }

        #region Public Properties

        public TextBox SubReportName
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

        public TextBox ReportFileName
        {
            get
            {
                return this.txt_ReportName;
            }
            set
            {
                this.txt_ReportName = value;
            }
        }

        public RadioButton OmitBorderYes
        {
            get
            {
                return this.rbtn_Yes;
            }
            set
            {
                this.rbtn_Yes = value;
            }
        }

        public RadioButton OmitBorderNo
        {
            get
            {
                return this.rbtn_No;
            }

            set
            {
                this.rbtn_No = value;
            }
        }

        public string selectedFileName = string.Empty;
 
        #endregion

        private void Btn_Browse_Click(object sender, RoutedEventArgs e)
        {
            SelectReport selectReport = new SelectReport(this);
            if (selectReport.ShowDialog() == true)
            {
                this.txt_ReportName.Text = selectReport.selectedFileName;
                int i = selectReport.selectedFileName.LastIndexOf("/");
                int j = selectReport.selectedFileName.Length;
                string str = selectReport.selectedFileName.Substring(0, (j - (j - i-1))-1);
                selectedFileName = str;
            }
        }
    }
}
