#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Forms.PdfViewer
{
    partial class DocumentToolbar
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DocumentToolbar));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnPrint = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnGoToFirstPage = new System.Windows.Forms.ToolStripButton();
            this.btnGoToPreviousPage = new System.Windows.Forms.ToolStripButton();
            this.txtCurrentPageIndex = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.lblTotalPageCount = new System.Windows.Forms.ToolStripLabel();
            this.btnGoToNextPage = new System.Windows.Forms.ToolStripButton();
            this.btnGoToLastPage = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnZoomIn = new System.Windows.Forms.ToolStripButton();
            this.btnZoomOut = new System.Windows.Forms.ToolStripButton();
            this.cmbCurrentZoomLevel = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnFitWidth = new System.Windows.Forms.ToolStripButton();
            this.btnFitPage = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.btnSave,
            this.toolStripSeparator2,
            this.btnPrint,
            this.toolStripSeparator1,
            this.btnGoToFirstPage,
            this.btnGoToPreviousPage,
            this.txtCurrentPageIndex,
            this.toolStripLabel1,
            this.lblTotalPageCount,
            this.btnGoToNextPage,
            this.btnGoToLastPage,
            this.toolStripButton3,
            this.btnZoomIn,
            this.btnZoomOut,
            this.cmbCurrentZoomLevel,
            this.toolStripSeparator3,
            this.btnFitWidth,
            this.btnFitPage});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(471, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = global::Syncfusion.Windows.PdfViewer.Properties.Resources.OpenFile;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "btnOpenFile";
            this.toolStripButton1.ToolTipText = "Click to open a PDF Document";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnPrint
            // 
            this.btnPrint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPrint.Image = global::Syncfusion.Windows.PdfViewer.Properties.Resources.PrintDocument;
            this.btnPrint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(23, 22);
            this.btnPrint.Text = "Click to Print this PDF file or pages from it.";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnGoToFirstPage
            // 
            this.btnGoToFirstPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnGoToFirstPage.Image = global::Syncfusion.Windows.PdfViewer.Properties.Resources.GoToFirstPage_Disabled;
            this.btnGoToFirstPage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnGoToFirstPage.Name = "btnGoToFirstPage";
            this.btnGoToFirstPage.Size = new System.Drawing.Size(23, 22);
            this.btnGoToFirstPage.Text = "toolStripButton2";
            this.btnGoToFirstPage.ToolTipText = "Click to go to first page in the document.";
            // 
            // btnGoToPreviousPage
            // 
            this.btnGoToPreviousPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnGoToPreviousPage.Image = global::Syncfusion.Windows.PdfViewer.Properties.Resources.GoToPreviousPage_Disabled;
            this.btnGoToPreviousPage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnGoToPreviousPage.Name = "btnGoToPreviousPage";
            this.btnGoToPreviousPage.Size = new System.Drawing.Size(23, 22);
            this.btnGoToPreviousPage.Text = "toolStripButton3";
            this.btnGoToPreviousPage.ToolTipText = "Click to go to previous page in the document.";
            // 
            // txtCurrentPageIndex
            // 
            this.txtCurrentPageIndex.Name = "txtCurrentPageIndex";
            this.txtCurrentPageIndex.Size = new System.Drawing.Size(30, 25);
            this.txtCurrentPageIndex.Text = "1";
            this.txtCurrentPageIndex.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtCurrentPageIndex.Click += new System.EventHandler(this.txtCurrentPageIndex_Click);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.toolStripLabel1.Size = new System.Drawing.Size(17, 22);
            this.toolStripLabel1.Text = "/";
            // 
            // lblTotalPageCount
            // 
            this.lblTotalPageCount.Name = "lblTotalPageCount";
            this.lblTotalPageCount.Size = new System.Drawing.Size(25, 22);
            this.lblTotalPageCount.Text = "100";
            // 
            // btnGoToNextPage
            // 
            this.btnGoToNextPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnGoToNextPage.Image = global::Syncfusion.Windows.PdfViewer.Properties.Resources.GoToNextPage_Enabled;
            this.btnGoToNextPage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnGoToNextPage.Name = "btnGoToNextPage";
            this.btnGoToNextPage.Size = new System.Drawing.Size(23, 22);
            this.btnGoToNextPage.Text = "toolStripButton4";
            this.btnGoToNextPage.ToolTipText = "Click to go to next page in the document.";
            // 
            // btnGoToLastPage
            // 
            this.btnGoToLastPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnGoToLastPage.Image = global::Syncfusion.Windows.PdfViewer.Properties.Resources.GoToLastPage_Enabled;
            this.btnGoToLastPage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnGoToLastPage.Name = "btnGoToLastPage";
            this.btnGoToLastPage.Size = new System.Drawing.Size(23, 22);
            this.btnGoToLastPage.Text = "toolStripButton5";
            this.btnGoToLastPage.ToolTipText = "Click to go to last page in the document.";
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(6, 25);
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnZoomIn.Image = global::Syncfusion.Windows.PdfViewer.Properties.Resources.ZoomIn;
            this.btnZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(23, 22);
            this.btnZoomIn.Text = "toolStripButton4";
            this.btnZoomIn.ToolTipText = "Click to increase the magnification of the entire page.";
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnZoomOut.Image = global::Syncfusion.Windows.PdfViewer.Properties.Resources.ZoomOut;
            this.btnZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(23, 22);
            this.btnZoomOut.ToolTipText = "Click to decrease the magnification of the entire page.";
            // 
            // cmbCurrentZoomLevel
            // 
            this.cmbCurrentZoomLevel.Items.AddRange(new object[] {
            "50%",
            "75%",
            "100%",
            "125%",
            "150%",
            "200%",
            "400%"});
            this.cmbCurrentZoomLevel.Name = "cmbCurrentZoomLevel";
            this.cmbCurrentZoomLevel.Size = new System.Drawing.Size(75, 25);
            this.cmbCurrentZoomLevel.Text = "100%";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // btnFitWidth
            // 
            this.btnFitWidth.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnFitWidth.Image = global::Syncfusion.Windows.PdfViewer.Properties.Resources.side;
            this.btnFitWidth.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFitWidth.Name = "btnFitWidth";
            this.btnFitWidth.Size = new System.Drawing.Size(23, 22);
            this.btnFitWidth.Text = "toolStripButton1";
            this.btnFitWidth.ToolTipText = "Click to fill the window with each page and scroll through pages continuously.";
            // 
            // btnFitPage
            // 
            this.btnFitPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnFitPage.Image = global::Syncfusion.Windows.PdfViewer.Properties.Resources.FitPage;
            this.btnFitPage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFitPage.Name = "btnFitPage";
            this.btnFitPage.Size = new System.Drawing.Size(23, 22);
            this.btnFitPage.Text = "toolStripButton2";
            this.btnFitPage.ToolTipText = "Click to show one page at a time.";
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSave.Image = global::Syncfusion.Windows.PdfViewer.Properties.Resources.save;
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(23, 22);
            this.btnSave.ToolTipText = "Click to save the document in the local disk";
            this.btnSave.Text = "Save";
            // 
            // DocumentToolbar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.toolStrip1);
            this.Name = "DocumentToolbar";
            this.Size = new System.Drawing.Size(471, 30);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnPrint;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnGoToFirstPage;
        private System.Windows.Forms.ToolStripButton btnGoToPreviousPage;
        private System.Windows.Forms.ToolStripTextBox txtCurrentPageIndex;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripLabel lblTotalPageCount;
        private System.Windows.Forms.ToolStripButton btnGoToNextPage;
        private System.Windows.Forms.ToolStripButton btnGoToLastPage;
        internal System.Windows.Forms.ToolStripComboBox cmbCurrentZoomLevel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton btnFitWidth;
        private System.Windows.Forms.ToolStripButton btnFitPage;
        private System.Windows.Forms.ToolStripSeparator toolStripButton3;
        private System.Windows.Forms.ToolStripButton btnZoomIn;
        private System.Windows.Forms.ToolStripButton btnZoomOut;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton btnSave;
    }
}
