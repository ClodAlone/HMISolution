#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Page size control class designer
    /// </summary>
    partial class PageSizeControl
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
            this.grpPageSizePageSize = new System.Windows.Forms.GroupBox();
            this.comboPageSizePaper = new System.Windows.Forms.ComboBox();
            this.comboPageSizeStandart = new System.Windows.Forms.ComboBox();
            this.txtPageSizeCustomHeight = new Syncfusion.Windows.Forms.Diagram.MeasureTextBox();
            this.txtPageSizeCustomWidth = new Syncfusion.Windows.Forms.Diagram.MeasureTextBox();
            this.rdPageSizeSameAsPrinterSize = new System.Windows.Forms.RadioButton();
            this.rdPageSizePreDefinedSize = new System.Windows.Forms.RadioButton();
            this.rdPageSizeCustomSize = new System.Windows.Forms.RadioButton();
            this.rdPageSizeToFitDrawingContent = new System.Windows.Forms.RadioButton();
            this.lblPageSizeCustomSize = new System.Windows.Forms.Label();
            this.grpPageSizePageOrientation = new System.Windows.Forms.GroupBox();
            this.rdPageSizePortrait = new System.Windows.Forms.RadioButton();
            this.rdPageSizeLandscape = new System.Windows.Forms.RadioButton();
            this.grpPageSizePageSize.SuspendLayout();
            this.grpPageSizePageOrientation.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpPageSizePageSize
            // 
            this.grpPageSizePageSize.Controls.Add(this.comboPageSizePaper);
            this.grpPageSizePageSize.Controls.Add(this.comboPageSizeStandart);
            this.grpPageSizePageSize.Controls.Add(this.txtPageSizeCustomHeight);
            this.grpPageSizePageSize.Controls.Add(this.txtPageSizeCustomWidth);
            this.grpPageSizePageSize.Controls.Add(this.rdPageSizeSameAsPrinterSize);
            this.grpPageSizePageSize.Controls.Add(this.rdPageSizePreDefinedSize);
            this.grpPageSizePageSize.Controls.Add(this.rdPageSizeCustomSize);
            this.grpPageSizePageSize.Controls.Add(this.rdPageSizeToFitDrawingContent);
            this.grpPageSizePageSize.Controls.Add(this.lblPageSizeCustomSize);
            this.grpPageSizePageSize.Location = new System.Drawing.Point(3, 2);
            this.grpPageSizePageSize.Name = "grpPageSizePageSize";
            this.grpPageSizePageSize.Size = new System.Drawing.Size(215, 208);
            this.grpPageSizePageSize.TabIndex = 0;
            this.grpPageSizePageSize.TabStop = false;
            this.grpPageSizePageSize.Text = "Page Size";
            // 
            // comboPageSizePaper
            // 
            this.comboPageSizePaper.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboPageSizePaper.Enabled = false;
            this.comboPageSizePaper.FormattingEnabled = true;
            this.comboPageSizePaper.Location = new System.Drawing.Point(51, 92);
            this.comboPageSizePaper.Name = "comboPageSizePaper";
            this.comboPageSizePaper.Size = new System.Drawing.Size(152, 21);
            this.comboPageSizePaper.TabIndex = 3;
            this.comboPageSizePaper.SelectedIndexChanged += new System.EventHandler(this.ComboPageSizePaper_SelectedIndexChanged);
            // 
            // comboPageSizeStandart
            // 
            this.comboPageSizeStandart.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboPageSizeStandart.Enabled = false;
            this.comboPageSizeStandart.FormattingEnabled = true;
            this.comboPageSizeStandart.Location = new System.Drawing.Point(29, 65);
            this.comboPageSizeStandart.Name = "comboPageSizeStandart";
            this.comboPageSizeStandart.Size = new System.Drawing.Size(174, 21);
            this.comboPageSizeStandart.TabIndex = 2;
            this.comboPageSizeStandart.SelectedIndexChanged += new System.EventHandler(this.ComboPageSizeStandart_SelectedIndexChanged);
            // 
            // txtPageSizeCustomHeight
            // 
            this.txtPageSizeCustomHeight.DefaultMeasureUnits = Syncfusion.Windows.Forms.Diagram.MeasureUnits.Pixel;
            this.txtPageSizeCustomHeight.DefaultValue = 0F;
            this.txtPageSizeCustomHeight.Enabled = false;
            this.txtPageSizeCustomHeight.Location = new System.Drawing.Point(129, 141);
            this.txtPageSizeCustomHeight.MeasureUnits = Syncfusion.Windows.Forms.Diagram.MeasureUnits.Pixel;
            this.txtPageSizeCustomHeight.Name = "txtPageSizeCustomHeight";
            this.txtPageSizeCustomHeight.Size = new System.Drawing.Size(75, 20);
            this.txtPageSizeCustomHeight.TabIndex = 7;
            this.txtPageSizeCustomHeight.Value = 0F;
            this.txtPageSizeCustomHeight.ValueChanged += new System.EventHandler(this.PageSizeCustomSize_TextChanged);
            // 
            // txtPageSizeCustomWidth
            // 
            this.txtPageSizeCustomWidth.DefaultMeasureUnits = Syncfusion.Windows.Forms.Diagram.MeasureUnits.Pixel;
            this.txtPageSizeCustomWidth.DefaultValue = 0F;
            this.txtPageSizeCustomWidth.Enabled = false;
            this.txtPageSizeCustomWidth.Location = new System.Drawing.Point(29, 141);
            this.txtPageSizeCustomWidth.MaxLength = 8;
            this.txtPageSizeCustomWidth.MeasureUnits = Syncfusion.Windows.Forms.Diagram.MeasureUnits.Pixel;
            this.txtPageSizeCustomWidth.Name = "txtPageSizeCustomWidth";
            this.txtPageSizeCustomWidth.Size = new System.Drawing.Size(75, 20);
            this.txtPageSizeCustomWidth.TabIndex = 5;
            this.txtPageSizeCustomWidth.Value = 0F;
            this.txtPageSizeCustomWidth.ValueChanged += new System.EventHandler(this.PageSizeCustomSize_TextChanged);
            // 
            // rdPageSizeSameAsPrinterSize
            // 
            this.rdPageSizeSameAsPrinterSize.AutoSize = true;
            this.rdPageSizeSameAsPrinterSize.Checked = true;
            this.rdPageSizeSameAsPrinterSize.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.rdPageSizeSameAsPrinterSize.Location = new System.Drawing.Point(9, 19);
            this.rdPageSizeSameAsPrinterSize.Name = "rdPageSizeSameAsPrinterSize";
            this.rdPageSizeSameAsPrinterSize.Size = new System.Drawing.Size(149, 17);
            this.rdPageSizeSameAsPrinterSize.TabIndex = 0;
            this.rdPageSizeSameAsPrinterSize.TabStop = true;
            this.rdPageSizeSameAsPrinterSize.Text = "Same as printer paper size";
            this.rdPageSizeSameAsPrinterSize.UseVisualStyleBackColor = true;
            this.rdPageSizeSameAsPrinterSize.CheckedChanged += new System.EventHandler(this.PageSize_CheckedChanged);
            // 
            // rdPageSizePreDefinedSize
            // 
            this.rdPageSizePreDefinedSize.AutoSize = true;
            this.rdPageSizePreDefinedSize.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.rdPageSizePreDefinedSize.Location = new System.Drawing.Point(9, 42);
            this.rdPageSizePreDefinedSize.Name = "rdPageSizePreDefinedSize";
            this.rdPageSizePreDefinedSize.Size = new System.Drawing.Size(103, 17);
            this.rdPageSizePreDefinedSize.TabIndex = 1;
            this.rdPageSizePreDefinedSize.Text = "Pre-defined size:";
            this.rdPageSizePreDefinedSize.UseVisualStyleBackColor = true;
            this.rdPageSizePreDefinedSize.CheckedChanged += new System.EventHandler(this.PageSize_CheckedChanged);
            // 
            // rdPageSizeCustomSize
            // 
            this.rdPageSizeCustomSize.AutoSize = true;
            this.rdPageSizeCustomSize.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.rdPageSizeCustomSize.Location = new System.Drawing.Point(9, 119);
            this.rdPageSizeCustomSize.Name = "rdPageSizeCustomSize";
            this.rdPageSizeCustomSize.Size = new System.Drawing.Size(86, 17);
            this.rdPageSizeCustomSize.TabIndex = 4;
            this.rdPageSizeCustomSize.Text = "Custom Size:";
            this.rdPageSizeCustomSize.UseVisualStyleBackColor = true;
            this.rdPageSizeCustomSize.CheckedChanged += new System.EventHandler(this.PageSize_CheckedChanged);
            // 
            // rdPageSizeToFitDrawingContent
            // 
            this.rdPageSizeToFitDrawingContent.AutoSize = true;
            this.rdPageSizeToFitDrawingContent.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.rdPageSizeToFitDrawingContent.Location = new System.Drawing.Point(9, 174);
            this.rdPageSizeToFitDrawingContent.Name = "rdPageSizeToFitDrawingContent";
            this.rdPageSizeToFitDrawingContent.Size = new System.Drawing.Size(152, 17);
            this.rdPageSizeToFitDrawingContent.TabIndex = 8;
            this.rdPageSizeToFitDrawingContent.Text = "Size to fit drawing contents";
            this.rdPageSizeToFitDrawingContent.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.rdPageSizeToFitDrawingContent.UseVisualStyleBackColor = true;
            this.rdPageSizeToFitDrawingContent.CheckedChanged += new System.EventHandler(this.PageSize_CheckedChanged);
            // 
            // lblPageSizeCustomSize
            // 
            this.lblPageSizeCustomSize.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblPageSizeCustomSize.Location = new System.Drawing.Point(109, 142);
            this.lblPageSizeCustomSize.Name = "lblPageSizeCustomSize";
            this.lblPageSizeCustomSize.Size = new System.Drawing.Size(14, 20);
            this.lblPageSizeCustomSize.TabIndex = 6;
            this.lblPageSizeCustomSize.Text = "X";
            this.lblPageSizeCustomSize.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // grpPageSizePageOrientation
            // 
            this.grpPageSizePageOrientation.Controls.Add(this.rdPageSizePortrait);
            this.grpPageSizePageOrientation.Controls.Add(this.rdPageSizeLandscape);
            this.grpPageSizePageOrientation.Location = new System.Drawing.Point(3, 217);
            this.grpPageSizePageOrientation.Name = "grpPageSizePageOrientation";
            this.grpPageSizePageOrientation.Size = new System.Drawing.Size(215, 47);
            this.grpPageSizePageOrientation.TabIndex = 1;
            this.grpPageSizePageOrientation.TabStop = false;
            this.grpPageSizePageOrientation.Text = "Page Orientation";
            // 
            // rdPageSizePortrait
            // 
            this.rdPageSizePortrait.Checked = true;
            this.rdPageSizePortrait.Enabled = false;
            this.rdPageSizePortrait.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rdPageSizePortrait.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.rdPageSizePortrait.Location = new System.Drawing.Point(20, 18);
            this.rdPageSizePortrait.Name = "rdPageSizePortrait";
            this.rdPageSizePortrait.Size = new System.Drawing.Size(64, 24);
            this.rdPageSizePortrait.TabIndex = 0;
            this.rdPageSizePortrait.TabStop = true;
            this.rdPageSizePortrait.Text = "Portrait";
            this.rdPageSizePortrait.CheckedChanged += new System.EventHandler(this.PageSizeOrientation_CheckedChanged);
            // 
            // rdPageSizeLandscape
            // 
            this.rdPageSizeLandscape.Enabled = false;
            this.rdPageSizeLandscape.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rdPageSizeLandscape.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.rdPageSizeLandscape.Location = new System.Drawing.Point(113, 18);
            this.rdPageSizeLandscape.Name = "rdPageSizeLandscape";
            this.rdPageSizeLandscape.Size = new System.Drawing.Size(88, 24);
            this.rdPageSizeLandscape.TabIndex = 1;
            this.rdPageSizeLandscape.Text = "Landscape";
            this.rdPageSizeLandscape.CheckedChanged += new System.EventHandler(this.PageSizeOrientation_CheckedChanged);
            // 
            // PageSizeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpPageSizePageSize);
            this.Controls.Add(this.grpPageSizePageOrientation);
            this.Name = "PageSizeControl";
            this.Size = new System.Drawing.Size(221, 267);
            this.grpPageSizePageSize.ResumeLayout(false);
            this.grpPageSizePageSize.PerformLayout();
            this.grpPageSizePageOrientation.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpPageSizePageSize;
        private System.Windows.Forms.Label lblPageSizeCustomSize;
        private System.Windows.Forms.GroupBox grpPageSizePageOrientation;
        public System.Windows.Forms.ComboBox comboPageSizePaper;
        public System.Windows.Forms.ComboBox comboPageSizeStandart;
        public MeasureTextBox txtPageSizeCustomHeight;
        public MeasureTextBox txtPageSizeCustomWidth;
        public System.Windows.Forms.RadioButton rdPageSizeSameAsPrinterSize;
        public System.Windows.Forms.RadioButton rdPageSizePreDefinedSize;
        public System.Windows.Forms.RadioButton rdPageSizeCustomSize;
        public System.Windows.Forms.RadioButton rdPageSizeToFitDrawingContent;
        public System.Windows.Forms.RadioButton rdPageSizePortrait;
        public System.Windows.Forms.RadioButton rdPageSizeLandscape;
    }
}
