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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.OlapSilverlight.Reports;
using Syncfusion.Windows.Tools.Controls;
using System.ComponentModel;
using Syncfusion.Silverlight.Client.Olap.Resources;
using System.Globalization;
using Syncfusion.Silverlight.Client.Olap;
using Syncfusion.Windows.Controls.Theming;

namespace Syncfusion.Silverlight.Tools.Olap
{
    [DesignTimeVisible(false)]
    public partial class ReportNameGetter : WindowControl
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportNameGetter"/> class.
        /// </summary>
        public ReportNameGetter()
        {
            InitializeComponent();
        }

        private bool _dialogResult;

        #endregion 

        #region Properties

        /// <summary>
        /// Gets or sets the report list.
        /// </summary>
        /// <value>The report list.</value>
        internal OlapReportCollection ReportList
        {
           private get;
           set;
        }

        /// <summary>
        /// Gets or sets the new name of the report.
        /// </summary>
        /// <value>The new name of the report.</value>
        public string NewReportName { get; private set; }

        /// <summary>
        /// Gets or sets the type of the dialog.
        /// </summary>
        /// <value>The type of the dialog.</value>
        public ReportDialogType DialogType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [dialog result].
        /// </summary>
        public bool DialogResult
        {
            get
            {
                return _dialogResult;
            }
            set
            {
                _dialogResult = value;
                this.Close();
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the Click event of the OKButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.reportName.Text.Trim()))
            {
                if (!this.CheckForExistance(this.reportName.Text))
                {
                    this.NewReportName = this.reportName.Text;
                    this.reportName.Text = string.Empty;
                    this.DialogResult = true;
                }
                else
                {
                    WindowControl.ShowAlert(SR.GetString(CultureInfo.CurrentUICulture, "Message_ReportNameAlreadyExists"), SR.GetString(CultureInfo.CurrentUICulture, "OlapClient_ReportNameGetter_ReportName"), DialogIcon.Exclamation, DialogButton.OK, null, AnimationType.Zoom);
                    this.reportName.Text = string.Empty;
                    this.reportName.Focus();
                }
            }
            else
            {
                WindowControl.ShowAlert(SR.GetString(CultureInfo.CurrentUICulture, "Message_ReportNameCouldNotBeEmpty"), SR.GetString(CultureInfo.CurrentUICulture, "OlapClient_ReportNameGetter_ReportName"), DialogIcon.Exclamation, DialogButton.OK, null, AnimationType.Zoom);
                this.reportName.Text = string.Empty;
                this.reportName.Focus();
            }
        }

        /// <summary>
        /// Checks for existance of a report name
        /// </summary>
        /// <param name="reportName">Name of the report.</param>
        /// <returns></returns>
        private bool CheckForExistance(string reportName)
        {
            if (this.ReportList == null || this.ReportList.Count == 0)
            {
                return false;
            }
            var r = from report in this.ReportList where report.Name.Equals(reportName) select report;

            if (r != null && r.Count() > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Handles the Click event of the CancelButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        #endregion
    }

    /// <summary>
    /// Dialog Types
    /// </summary>
    public enum ReportDialogType
    {
        /// <summary>
        /// New report dialog
        /// </summary>
        NewReport,
        /// <summary>
        /// Add report dialog
        /// </summary>
        AddReport,
        /// <summary>
        /// Rename report dialog
        /// </summary>
        RenameReport
    }
}

