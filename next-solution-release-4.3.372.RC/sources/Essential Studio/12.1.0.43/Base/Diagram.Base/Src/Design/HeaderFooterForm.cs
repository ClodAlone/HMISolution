#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Header footer form.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class HeaderFooterForm : System.Windows.Forms.Form
    {
        private System.Windows.Forms.TextBox txtStr;
        private System.Windows.Forms.ContextMenu contextMenu1;
        private System.Windows.Forms.Button btn1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.MenuItem mnuPage;
        private System.Windows.Forms.MenuItem mnuCurrentTime;
        private System.Windows.Forms.MenuItem mnuDateShort;
        private System.Windows.Forms.MenuItem mnuDateLong;
        private System.Windows.Forms.MenuItem mnuTotalPages;
        private System.Windows.Forms.MenuItem mnuPageCount;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderFooterForm"/> class.
        /// </summary>
        public HeaderFooterForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtStr = new System.Windows.Forms.TextBox();
            this.mnuPage = new System.Windows.Forms.MenuItem();
            this.mnuTotalPages = new System.Windows.Forms.MenuItem();
            this.mnuCurrentTime = new System.Windows.Forms.MenuItem();
            this.mnuDateShort = new System.Windows.Forms.MenuItem();
            this.mnuDateLong = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.mnuPageCount = new System.Windows.Forms.MenuItem();
            this.btn1 = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.SuspendLayout();
            // 
            // txtStr
            // 
            this.txtStr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStr.Location = new System.Drawing.Point(8, 8);
            this.txtStr.Name = "txtStr";
            this.txtStr.Size = new System.Drawing.Size(178, 20);
            this.txtStr.TabIndex = 0;
            this.txtStr.Text = "";
            // 
            // mnuPage
            // 
            this.mnuPage.Index = 0;
            this.mnuPage.Text = "Page";
            this.mnuPage.Click += new System.EventHandler(this.mnuPage_Click);
            // 
            // mnuTotalPages
            // 
            this.mnuTotalPages.Index = 1;
            this.mnuTotalPages.Text = "Total Pages";
            this.mnuTotalPages.Click += new System.EventHandler(this.mnuTotalPages_Click);
            // 
            // mnuCurrentTime
            // 
            this.mnuCurrentTime.Index = 2;
            this.mnuCurrentTime.Text = "CurrentTime               - &&T";
            this.mnuCurrentTime.Click += new System.EventHandler(this.mnuCurrentTime_Click);
            // 
            // mnuDateShort
            // 
            this.mnuDateShort.Index = 3;
            this.mnuDateShort.Text = "Current Date (short) - &&d";
            this.mnuDateShort.Click += new System.EventHandler(this.mnuDateShort_Click);
            // 
            // mnuDateLong
            // 
            this.mnuDateLong.Index = 4;
            this.mnuDateLong.Text = "Current Date (long)   - &&D";
            this.mnuDateLong.Click += new System.EventHandler(this.mnuDateLong_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 5;
            this.menuItem1.Text = "-";
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 6;
            this.menuItem2.Text = "for && type it twice";
            // 
            // mnuPageCount
            // 
            this.mnuPageCount.Index = -1;
            this.mnuPageCount.Text = "";
            // 
            // btn1
            // 
            this.btn1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn1.Location = new System.Drawing.Point(194, 8);
            this.btn1.Name = "btn1";
            this.btn1.Size = new System.Drawing.Size(16, 20);
            this.btn1.TabIndex = 1;
            this.btn1.Text = ">";
            this.btn1.Click += new System.EventHandler(this.btn1_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(58, 40);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 24);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "&OK";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(138, 40);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(76, 24);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "&Cancel";
            // 
            // contextMenu1
            // 
            this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuPage,
            this.mnuTotalPages,
            this.mnuCurrentTime,
            this.mnuDateShort,
            this.mnuDateLong,
            this.menuItem1,
            this.menuItem2});
            // 
            // HeaderFooterForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(218, 69);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btn1);
            this.Controls.Add(this.txtStr);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HeaderFooterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Header/Footer Editor";
            this.ResumeLayout(false);
        }
        #endregion

        /// <summary>
        /// Handles the Click event of the button control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btn1_Click(object sender, System.EventArgs e)
        {
            ContextMenu conMenu = new ContextMenu();
            conMenu.MergeMenu(this.contextMenu1);
            Point pt = new Point();
            pt.X = this.ActiveControl.Location.X + this.ActiveControl.Width;
            pt.Y = this.ActiveControl.Location.Y;
            conMenu.Show(this, pt);
            conMenu.Dispose();
        }

        #region Context Menu Event Handlers

        private void mnuPage_Click(object sender, System.EventArgs e)
        {
            this.txtStr.Text += "&p";
        }

        // private void mnuPageCount_Click(object sender, System.EventArgs e)
        // {
        //    this.txtStr.Text += "&P";
        // }
        private void mnuTotalPages_Click(object sender, System.EventArgs e)
        {
            this.txtStr.Text += "&P";
        }

        private void mnuCurrentTime_Click(object sender, System.EventArgs e)
        {
            this.txtStr.Text += "&T";
        }

        private void mnuDateShort_Click(object sender, System.EventArgs e)
        {
            this.txtStr.Text += "&d";
        }

        private void mnuDateLong_Click(object sender, System.EventArgs e)
        {
            this.txtStr.Text += "&D";
        }

        #endregion

        /// <summary>
        /// Gets or sets the text box.
        /// </summary>
        /// <value>The text box.</value>
        public string TextBox
        {
            get
            {
                return this.txtStr.Text;
            }
            set
            {
                if (value != null)
                {
                    this.txtStr.Text = value;
                }
                else
                {
                    this.txtStr.Text = string.Empty;
                }
            }
        }
    }
}