#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
namespace Syncfusion.GridHelperClasses
{
    partial class OptimizedFilterDropDownControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code
        private bool isSearchOption = false;
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            if (isSearchOption)
            {
                this.components = new System.ComponentModel.Container();
                this.okButton = new Syncfusion.Windows.Forms.ButtonAdv();
                this.cancelButton = new Syncfusion.Windows.Forms.ButtonAdv();
                this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
                this.searchPanel1 = new System.Windows.Forms.Panel();
                this.searchLabel1 = new System.Windows.Forms.Label();
                this.searchBox1 = new System.Windows.Forms.TextBox();
                this.searchPanel1.SuspendLayout();
                this.SuspendLayout();
                // 
                // okButton
                // 
                this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
                this.okButton.BeforeTouchSize = new System.Drawing.Size(64, 28);
                this.okButton.IsBackStageButton = false;
                this.okButton.Location = new System.Drawing.Point(55, 199);
                this.okButton.Name = "okButton";
                this.okButton.Size = new System.Drawing.Size(64, 28);
                this.okButton.TabIndex = 0;
                this.okButton.Text = SR.GetString(SR.Office2007FilterOK);
                this.okButton.UseVisualStyle = true;
                this.okButton.Click += new System.EventHandler(this.button1_Click);
                // 
                // cancelButton
                // 
                this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
                this.cancelButton.BeforeTouchSize = new System.Drawing.Size(67, 28);
                this.cancelButton.IsBackStageButton = false;
                this.cancelButton.Location = new System.Drawing.Point(124, 199);
                this.cancelButton.Name = "cancelButton";
                this.cancelButton.Size = new System.Drawing.Size(67, 28);
                this.cancelButton.TabIndex = 0;
                this.cancelButton.Text = SR.GetString(SR.Office2007FilterCancel);
                this.cancelButton.UseVisualStyle = true;
                this.cancelButton.Click += new System.EventHandler(this.button2_Click);
                // 
                // checkedListBox1
                // 
                this.checkedListBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                            | System.Windows.Forms.AnchorStyles.Left)
                            | System.Windows.Forms.AnchorStyles.Right)));
                this.checkedListBox1.FormattingEnabled = true;
                this.checkedListBox1.Location = new System.Drawing.Point(3, 38);
                this.checkedListBox1.Name = "checkedListBox1";
                this.checkedListBox1.Size = new System.Drawing.Size(185, 154);
                this.checkedListBox1.TabIndex = 0;
                // 
                // searchPanel1
                // 
                this.searchPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                            | System.Windows.Forms.AnchorStyles.Right)));
                this.searchPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                this.searchPanel1.Controls.Add(this.searchLabel1);
                this.searchPanel1.Controls.Add(this.searchBox1);
                this.searchPanel1.Location = new System.Drawing.Point(3, 7);
                this.searchPanel1.Name = "searchPanel1";
                this.searchPanel1.Size = new System.Drawing.Size(185, 23);
                this.searchPanel1.TabIndex = 1;
                // 
                // searchLabel1
                // 
                this.searchLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                            | System.Windows.Forms.AnchorStyles.Right)));
                this.searchLabel1.BackColor = System.Drawing.Color.Transparent;
                this.searchLabel1.Location = new System.Drawing.Point(159, 3);
                this.searchLabel1.Name = "searchLabel1";
                this.searchLabel1.Size = new System.Drawing.Size(24, 17);
                this.searchLabel1.TabIndex = 1;
                this.searchLabel1.Text = "  ";
                // 
                // searchBox1
                // 
                this.searchBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                            | System.Windows.Forms.AnchorStyles.Left)
                            | System.Windows.Forms.AnchorStyles.Right)));
                this.searchBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
                this.searchBox1.Location = new System.Drawing.Point(3, 4);
                this.searchBox1.Name = "searchBox1";
                this.searchBox1.Size = new System.Drawing.Size(155, 13);
                this.searchBox1.TabIndex = 0;
                // 
                // OptimizedFilterDropDownControl
                // 
                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
                this.Controls.Add(this.searchPanel1);
                this.Controls.Add(this.checkedListBox1);
                this.Controls.Add(this.okButton);
                this.Controls.Add(this.cancelButton);
                this.Margin = new System.Windows.Forms.Padding(4);
                this.Name = "OptimizedFilterDropDownControl";
                this.Size = new System.Drawing.Size(191, 239);

                this.searchPanel1.ResumeLayout(false);
                this.searchPanel1.PerformLayout();
                this.ResumeLayout(false);
            }
            else
            {
                //OLD SETTINGS//

                this.components = new System.ComponentModel.Container();
                this.okButton = new Syncfusion.Windows.Forms.ButtonAdv();
                this.cancelButton = new Syncfusion.Windows.Forms.ButtonAdv();
                this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
                this.searchPanel1 = new System.Windows.Forms.Panel();
                this.searchLabel1 = new System.Windows.Forms.Label();
                this.searchBox1 = new System.Windows.Forms.TextBox();
                this.searchPanel1.SuspendLayout();
                this.SuspendLayout();
                // 
                // okButton
                // 
                this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
                this.okButton.BeforeTouchSize = new System.Drawing.Size(64, 28);
                this.okButton.IsBackStageButton = false;
                this.okButton.Location = new System.Drawing.Point(55, 164);
                this.okButton.Name = "okButton";
                this.okButton.Size = new System.Drawing.Size(64, 28);
                this.okButton.TabIndex = 0;
                this.okButton.Text = SR.GetString(SR.Office2007FilterOK);
                this.okButton.UseVisualStyle = true;
                this.okButton.Click += new System.EventHandler(this.button1_Click);
                // 
                // cancelButton
                // 
                this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
                this.cancelButton.BeforeTouchSize = new System.Drawing.Size(67, 28);
                this.cancelButton.IsBackStageButton = false;
                this.cancelButton.Location = new System.Drawing.Point(124, 164);
                this.cancelButton.Name = "cancelButton";
                this.cancelButton.Size = new System.Drawing.Size(67, 28);
                this.cancelButton.TabIndex = 0;
                this.cancelButton.Text = SR.GetString(SR.Office2007FilterCancel);
                this.cancelButton.UseVisualStyle = true;
                this.cancelButton.Click += new System.EventHandler(this.button2_Click);
                // 
                // checkedListBox1
                // 
                this.checkedListBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                            | System.Windows.Forms.AnchorStyles.Left)
                            | System.Windows.Forms.AnchorStyles.Right)));
                this.checkedListBox1.FormattingEnabled = true;
                this.checkedListBox1.Location = new System.Drawing.Point(3, 3);
                this.checkedListBox1.Name = "checkedListBox1";
                this.checkedListBox1.Size = new System.Drawing.Size(185, 154);
                this.checkedListBox1.TabIndex = 0;
                // 
                // searchPanel1
                // 
                this.searchPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                            | System.Windows.Forms.AnchorStyles.Right)));
                this.searchPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                this.searchPanel1.Controls.Add(this.searchLabel1);
                this.searchPanel1.Controls.Add(this.searchBox1);
                this.searchPanel1.Location = new System.Drawing.Point(3, 7);
                this.searchPanel1.Name = "searchPanel1";
                this.searchPanel1.Size = new System.Drawing.Size(185, 23);
                this.searchPanel1.TabIndex = 1;
                // 
                // searchLabel1
                // 
                this.searchLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                            | System.Windows.Forms.AnchorStyles.Right)));
                this.searchLabel1.BackColor = System.Drawing.Color.Transparent;
                this.searchLabel1.Location = new System.Drawing.Point(159, 3);
                this.searchLabel1.Name = "searchLabel1";
                this.searchLabel1.Size = new System.Drawing.Size(24, 17);
                this.searchLabel1.TabIndex = 1;
                this.searchLabel1.Text = "  ";
                // 
                // searchBox1
                // 
                this.searchBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                            | System.Windows.Forms.AnchorStyles.Left)
                            | System.Windows.Forms.AnchorStyles.Right)));
                this.searchBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
                this.searchBox1.Location = new System.Drawing.Point(3, 4);
                this.searchBox1.Name = "searchBox1";
                this.searchBox1.Size = new System.Drawing.Size(155, 13);
                this.searchBox1.TabIndex = 0;
                this.searchBox1.Hide();
                this.searchLabel1.Hide();
                this.searchPanel1.Hide();
                this.searchPanel1.Enabled = false;
                this.searchLabel1.Enabled = false;
                this.searchBox1.Enabled = false;
                // 
                // OptimizedFilterDropDownControl
                // 
                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
                this.Controls.Add(this.searchPanel1);
                this.Controls.Add(this.checkedListBox1);
                this.Controls.Add(this.okButton);
                this.Controls.Add(this.cancelButton);
                this.Margin = new System.Windows.Forms.Padding(4);
                this.Name = "OptimizedFilterDropDownControl";
                this.Size = new System.Drawing.Size(191, 204);

                this.searchPanel1.ResumeLayout(false);
                this.searchPanel1.PerformLayout();
                this.ResumeLayout(false);
            }
        }
        #endregion

        public Syncfusion.Windows.Forms.ButtonAdv okButton;
        public Syncfusion.Windows.Forms.ButtonAdv cancelButton;
        public System.Windows.Forms.CheckedListBox checkedListBox1;
        
        internal System.Windows.Forms.Panel searchPanel1;
        internal System.Windows.Forms.TextBox searchBox1;
        internal System.Windows.Forms.Label searchLabel1;
    }
}
