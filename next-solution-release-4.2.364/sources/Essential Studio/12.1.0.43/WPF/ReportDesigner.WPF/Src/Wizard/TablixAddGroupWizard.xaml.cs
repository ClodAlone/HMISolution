//-------------------------------------------------------------------------------------------------
// <copyright file="TablixAddGroupWizard.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

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
using Syncfusion.Windows.Shared;
using System.ComponentModel;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Reports.Designer.Controls;
using Syncfusion.Windows.Reports.Designer.Dialogs;
using System.IO;
using RESX = Syncfusion.Windows.Reports.Designer.Properties.Resources;
using Syncfusion.Windows.ReportDesigner.Resources;
using System.Globalization;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for TablixAddGroupWizard.xaml
    /// </summary>
    internal partial class TablixAddGroupWizard : ChromelessWindow
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TablixAddGroupWizard"/> class.
        /// </summary>
        public TablixAddGroupWizard()
        {
            InitializeComponent();
            this.WindowStyle = WindowStyle.ToolWindow;
            this.Title = Syncfusion.Windows.ReportDesigner.Resources.SR.GetString(System.Globalization.CultureInfo.CurrentUICulture, "titleTablixAddGroupWizard");
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public System.Windows.Controls.ComboBox Value
        {
            get
            {
                return this.cmb_Value;
            }
            set
            {
                this.cmb_Value = value;
            }
        }
        #endregion

        #region Helper Methods
        private void Cancel_button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Ok_button_Click(object sender, RoutedEventArgs e)
        {
            if (this.cmb_Value.Text != null && this.cmb_Value.Text.Trim().ToString() != string.Empty)
            {
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxSelectValidExpression"), SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner"), MessageBoxButton.OK);
            }
        }
        #endregion
    }
}
