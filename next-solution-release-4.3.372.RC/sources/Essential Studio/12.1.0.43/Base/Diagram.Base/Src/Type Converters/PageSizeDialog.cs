#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Class containing Page size dialog
    /// </summary>
    public partial class PageSizeDialog : Form
    {
        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>The size of the page.</value>
        public PageSize PageSize
        {
            get { return pageSizeControl1.PageSize; }
            set { pageSizeControl1.PageSize = value; }
        }

        /// <summary>
        /// Gets or sets the printer settings.
        /// </summary>
        /// <value>The printer settings.</value>
        public PageSettings PrinterSettings
        {
            get { return pageSizeControl1.PrinterSettings; }
            set { pageSizeControl1.PrinterSettings = value; }
        }

        /// <summary>
        /// Gets or sets the size of the model content.
        /// </summary>
        /// <value>The size of the model content.</value>
        public SizeF ModelContentSize
        {
            get { return pageSizeControl1.ModelContentSize; }
            set { pageSizeControl1.ModelContentSize = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageSizeDialog"/> class.
        /// </summary>
        public PageSizeDialog()
        {
            InitializeComponent();
        }
    }
}