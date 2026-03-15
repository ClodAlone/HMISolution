#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Forms.PdfViewer
{
    partial class SearchBox
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

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.searchPreviousBtn = new SearchBoxButton();
            this.searchNextBtn = new SearchBoxButton();
            this.searchInputTxtBox = new System.Windows.Forms.TextBox();
            this.searchCloseBtn = new SearchBoxButton();
            this.topSpacingPanel = new System.Windows.Forms.Panel();
            this.leftSpacingPanel = new System.Windows.Forms.Panel();
            this.rightSpacingPanel = new System.Windows.Forms.Panel();
            this.searchPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            //
            // searchPanel
            //
            this.searchPanel.BackColor = System.Drawing.Color.FromArgb(255, 180, 203, 255);//System.Drawing.Color.Transparent;
            this.searchPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.searchPanel.Size = new System.Drawing.Size(290, 20);
            this.Controls.Add(searchPanel);
            //
            //topSpacingPanel
            //
            this.topSpacingPanel.BackColor = System.Drawing.Color.FromArgb(255, 180, 203, 255);
            this.topSpacingPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topSpacingPanel.Size = new System.Drawing.Size(280, 5);
            this.Controls.Add(topSpacingPanel);
            //
            //leftSpacingPanel
            //
            this.leftSpacingPanel.BackColor = System.Drawing.Color.FromArgb(255, 180, 203, 255);
            this.leftSpacingPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.leftSpacingPanel.Size = new System.Drawing.Size(5, 5);
            this.Controls.Add(leftSpacingPanel);
            //
            //rightSpacingPanel
            //
            this.rightSpacingPanel.BackColor = System.Drawing.Color.FromArgb(255, 180, 203, 255);
            this.rightSpacingPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.rightSpacingPanel.Size = new System.Drawing.Size(5, 5);
            // 
            // searchPreviousBtn
            // 
            this.searchPreviousBtn.FlatAppearance.BorderSize = 0;
            this.searchPreviousBtn.Image = global::Syncfusion.Windows.PdfViewer.Properties.Resources.ArrowHead_UpDisable;
            this.searchPreviousBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.searchPreviousBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.searchPreviousBtn.Location = new System.Drawing.Point(202, 0);
            this.searchPreviousBtn.Name = "searchPreviousBtn";
            this.searchPreviousBtn.Size = new System.Drawing.Size(20, 20);
            this.searchPreviousBtn.TabIndex = 1;
            this.searchPreviousBtn.Dock = System.Windows.Forms.DockStyle.Left;
            this.searchPreviousBtn.Paint += new System.Windows.Forms.PaintEventHandler(this.allButton_Paint);
            this.searchPreviousBtn.MouseHover += new System.EventHandler(searchPreviousBtn_MouseHover);
            this.searchPreviousBtn.MouseLeave += new System.EventHandler(searchPreviousBtn_MouseLeave);
            this.searchPreviousBtn.GotFocus += new System.EventHandler(searchPreviousBtn_MouseHover);
            this.searchPreviousBtn.LostFocus += new System.EventHandler(searchPreviousBtn_MouseLeave);
            // 
            // searchNextBtn
            // 
            this.searchNextBtn.FlatAppearance.BorderSize = 0;
            this.searchNextBtn.Image = global::Syncfusion.Windows.PdfViewer.Properties.Resources.ArrowHead_DownDisable;
            this.searchNextBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.searchNextBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.searchNextBtn.Location = new System.Drawing.Point(231, 0);
            this.searchNextBtn.Name = "searchNextBtn";
            this.searchNextBtn.Size = new System.Drawing.Size(20, 20);
            this.searchNextBtn.TabIndex = 2;
            this.searchNextBtn.Dock = System.Windows.Forms.DockStyle.Left;
            this.searchNextBtn.Paint += new System.Windows.Forms.PaintEventHandler(this.allButton_Paint);
            this.searchNextBtn.MouseHover += new System.EventHandler(searchNextBtn_MouseHover);
            this.searchNextBtn.MouseLeave += new System.EventHandler(searchNextBtn_MouseLeave);
            this.searchNextBtn.GotFocus += new System.EventHandler(searchNextBtn_MouseHover);
            this.searchNextBtn.LostFocus += new System.EventHandler(searchNextBtn_MouseLeave);
            // 
            // searchInputTxtBox
            //
            this.searchInputTxtBox.Location = new System.Drawing.Point(0, 0);
            this.searchInputTxtBox.Name = "searchInputTxtBox";
            this.searchInputTxtBox.Size = new System.Drawing.Size(200, 20);
            this.searchInputTxtBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.searchInputTxtBox.TabIndex = 0;
            this.searchInputTxtBox.Click += new System.EventHandler(searchInputTxtBox_Click);
            this.searchInputTxtBox.GotFocus += new System.EventHandler(searchInputTxtBox_GotFocus);
            // 
            // searchCloseBtn
            // 
            this.searchCloseBtn.FlatAppearance.BorderSize = 0;
            this.searchCloseBtn.Image = global::Syncfusion.Windows.PdfViewer.Properties.Resources.WrongCheckDisabled;
            this.searchCloseBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.searchCloseBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.searchCloseBtn.Location = new System.Drawing.Point(286, 0);
            this.searchCloseBtn.Name = "searchCloseBtn";
            this.searchCloseBtn.Size = new System.Drawing.Size(20, 20);
            this.searchCloseBtn.TabIndex = 4;
            this.searchCloseBtn.Dock = System.Windows.Forms.DockStyle.Right;
            this.searchCloseBtn.Paint += new System.Windows.Forms.PaintEventHandler(this.allButton_Paint);
            this.searchCloseBtn.Click += new System.EventHandler(this.closeButton_Click);
            this.searchCloseBtn.MouseHover += new System.EventHandler(searchCloseBtn_MouseHover);
            this.searchCloseBtn.MouseLeave += new System.EventHandler(searchCloseBtn_MouseLeave);
            this.searchCloseBtn.GotFocus += new System.EventHandler(searchCloseBtn_MouseHover);
            this.searchCloseBtn.LostFocus += new System.EventHandler(searchCloseBtn_MouseLeave);
            // 
            // SearchBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.searchPanel.Controls.Add(this.searchNextBtn);
            this.searchPanel.Controls.Add(this.searchPreviousBtn);
            this.searchPanel.Controls.Add(this.searchInputTxtBox);
            this.Controls.Add(this.searchCloseBtn);
            this.Controls.Add(rightSpacingPanel);
            this.Name = "SearchBox";
            this.Size = new System.Drawing.Size(275, 30);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.SearchBox_Paint);
            this.GotFocus += new System.EventHandler(SearchBox_GotFocus);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        void searchInputTxtBox_GotFocus(object sender, System.EventArgs e)
        {
            this.searchInputTxtBox.SelectAll();
        }

        void searchInputTxtBox_Click(object sender, System.EventArgs e)
        {
            this.searchInputTxtBox.SelectAll();
        }

        void SearchBox_GotFocus(object sender, System.EventArgs e)
        {
            this.searchInputTxtBox.Focus();
        }

        void searchPreviousBtn_MouseLeave(object sender, System.EventArgs e)
        {
            this.searchPreviousBtn.Image = global::Syncfusion.Windows.PdfViewer.Properties.Resources.ArrowHead_UpDisable;
        }
        void searchPreviousBtn_MouseHover(object sender, System.EventArgs e)
        {
            this.searchPreviousBtn.Image = global::Syncfusion.Windows.PdfViewer.Properties.Resources.ArrowHead_Up;
        }

        void searchCloseBtn_MouseLeave(object sender, System.EventArgs e)
        {
            this.searchCloseBtn.Image = global::Syncfusion.Windows.PdfViewer.Properties.Resources.WrongCheckDisabled;
        }
        void searchCloseBtn_MouseHover(object sender, System.EventArgs e)
        {
            this.searchCloseBtn.Image = global::Syncfusion.Windows.PdfViewer.Properties.Resources.Close;
        }

        void searchNextBtn_MouseLeave(object sender, System.EventArgs e)
        {
            this.searchNextBtn.Image = global::Syncfusion.Windows.PdfViewer.Properties.Resources.ArrowHead_DownDisable;
        }
        void searchNextBtn_MouseHover(object sender, System.EventArgs e)
        {
            this.searchNextBtn.Image = global::Syncfusion.Windows.PdfViewer.Properties.Resources.ArrowHead_Down;
        }
        #endregion

        internal SearchBoxButton searchPreviousBtn;
        internal SearchBoxButton searchNextBtn;
        internal System.Windows.Forms.TextBox searchInputTxtBox;
        internal SearchBoxButton searchCloseBtn;
        private System.Windows.Forms.Panel topSpacingPanel;
        private System.Windows.Forms.Panel searchPanel;
        private System.Windows.Forms.Panel leftSpacingPanel;
        private System.Windows.Forms.Panel rightSpacingPanel;
    }
}
