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
    /// Drawing scale control designer.
    /// </summary>
    partial class DrawingScaleControl
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
            this.grpMeasurementUnits = new System.Windows.Forms.GroupBox();
            this.lblDrawingScalePageSize = new System.Windows.Forms.Label();
            this.comboMeasureUnits = new System.Windows.Forms.ComboBox();
            this.txtDrawingScalePageWidth = new Syncfusion.Windows.Forms.Diagram.MeasureTextBox();
            this.txtDrawingScalePageHeight = new Syncfusion.Windows.Forms.Diagram.MeasureTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grpDrawingScale = new System.Windows.Forms.GroupBox();
            this.comboDrawingPreDefinedPaper = new System.Windows.Forms.ComboBox();
            this.comboDrawingPreDefinedStandart = new System.Windows.Forms.ComboBox();
            this.txtDrawingCustomHeight = new Syncfusion.Windows.Forms.Diagram.MeasureTextBox();
            this.txtDrawingCustomWidth = new Syncfusion.Windows.Forms.Diagram.MeasureTextBox();
            this.rdDrawingNoScale = new System.Windows.Forms.RadioButton();
            this.rdDrawingPreDefinedScale = new System.Windows.Forms.RadioButton();
            this.rdDrawingCustomScale = new System.Windows.Forms.RadioButton();
            this.lblDrawingCustomScale = new System.Windows.Forms.Label();
            this.grpMeasurementUnits.SuspendLayout();
            this.grpDrawingScale.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpMeasurementUnits
            // 
            this.grpMeasurementUnits.Controls.Add(this.lblDrawingScalePageSize);
            this.grpMeasurementUnits.Controls.Add(this.comboMeasureUnits);
            this.grpMeasurementUnits.Controls.Add(this.txtDrawingScalePageWidth);
            this.grpMeasurementUnits.Controls.Add(this.txtDrawingScalePageHeight);
            this.grpMeasurementUnits.Controls.Add(this.label1);
            this.grpMeasurementUnits.Location = new System.Drawing.Point(3, 171);
            this.grpMeasurementUnits.Name = "grpMeasurementUnits";
            this.grpMeasurementUnits.Size = new System.Drawing.Size(215, 93);
            this.grpMeasurementUnits.TabIndex = 4;
            this.grpMeasurementUnits.TabStop = false;
            this.grpMeasurementUnits.Text = "Measurement Units";
            // 
            // lblDrawingScalePageSize
            // 
            this.lblDrawingScalePageSize.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDrawingScalePageSize.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblDrawingScalePageSize.Location = new System.Drawing.Point(9, 42);
            this.lblDrawingScalePageSize.Name = "lblDrawingScalePageSize";
            this.lblDrawingScalePageSize.Size = new System.Drawing.Size(194, 20);
            this.lblDrawingScalePageSize.TabIndex = 2;
            this.lblDrawingScalePageSize.Text = "Page size ( in measurement units ):";
            this.lblDrawingScalePageSize.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // comboMeasureUnits
            // 
            this.comboMeasureUnits.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboMeasureUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboMeasureUnits.FormattingEnabled = true;
            this.comboMeasureUnits.Location = new System.Drawing.Point(9, 18);
            this.comboMeasureUnits.Name = "comboMeasureUnits";
            this.comboMeasureUnits.Size = new System.Drawing.Size(194, 21);
            this.comboMeasureUnits.TabIndex = 3;
            this.comboMeasureUnits.SelectedIndexChanged += new System.EventHandler(this.ComboMeasureUnits_SelectedIndexChanged);
            // 
            // txtDrawingScalePageWidth
            // 
            this.txtDrawingScalePageWidth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDrawingScalePageWidth.DefaultMeasureUnits = Syncfusion.Windows.Forms.Diagram.MeasureUnits.Pixel;
            this.txtDrawingScalePageWidth.DefaultValue = 0F;
            this.txtDrawingScalePageWidth.Enabled = false;
            this.txtDrawingScalePageWidth.Location = new System.Drawing.Point(28, 65);
            this.txtDrawingScalePageWidth.MeasureUnits = Syncfusion.Windows.Forms.Diagram.MeasureUnits.Pixel;
            this.txtDrawingScalePageWidth.Name = "txtDrawingScalePageWidth";
            this.txtDrawingScalePageWidth.Size = new System.Drawing.Size(75, 20);
            this.txtDrawingScalePageWidth.TabIndex = 1;
            this.txtDrawingScalePageWidth.Value = 1F;
            this.txtDrawingScalePageWidth.ValueChanged += new System.EventHandler(this.DrawingScalePageSize_ValueChanged);
            // 
            // txtDrawingScalePageHeight
            // 
            this.txtDrawingScalePageHeight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDrawingScalePageHeight.DefaultMeasureUnits = Syncfusion.Windows.Forms.Diagram.MeasureUnits.Pixel;
            this.txtDrawingScalePageHeight.DefaultValue = 0F;
            this.txtDrawingScalePageHeight.Enabled = false;
            this.txtDrawingScalePageHeight.Location = new System.Drawing.Point(128, 65);
            this.txtDrawingScalePageHeight.MeasureUnits = Syncfusion.Windows.Forms.Diagram.MeasureUnits.Pixel;
            this.txtDrawingScalePageHeight.Name = "txtDrawingScalePageHeight";
            this.txtDrawingScalePageHeight.Size = new System.Drawing.Size(75, 20);
            this.txtDrawingScalePageHeight.TabIndex = 4;
            this.txtDrawingScalePageHeight.Value = 1F;
            this.txtDrawingScalePageHeight.ValueChanged += new System.EventHandler(this.DrawingScalePageSize_ValueChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(108, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "X";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // grpDrawingScale
            // 
            this.grpDrawingScale.Controls.Add(this.comboDrawingPreDefinedPaper);
            this.grpDrawingScale.Controls.Add(this.comboDrawingPreDefinedStandart);
            this.grpDrawingScale.Controls.Add(this.txtDrawingCustomHeight);
            this.grpDrawingScale.Controls.Add(this.txtDrawingCustomWidth);
            this.grpDrawingScale.Controls.Add(this.rdDrawingNoScale);
            this.grpDrawingScale.Controls.Add(this.rdDrawingPreDefinedScale);
            this.grpDrawingScale.Controls.Add(this.rdDrawingCustomScale);
            this.grpDrawingScale.Controls.Add(this.lblDrawingCustomScale);
            this.grpDrawingScale.Location = new System.Drawing.Point(3, 2);
            this.grpDrawingScale.Name = "grpDrawingScale";
            this.grpDrawingScale.Size = new System.Drawing.Size(215, 168);
            this.grpDrawingScale.TabIndex = 3;
            this.grpDrawingScale.TabStop = false;
            this.grpDrawingScale.Text = "Drawing Scale";
            // 
            // comboDrawingPreDefinedPaper
            // 
            this.comboDrawingPreDefinedPaper.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboDrawingPreDefinedPaper.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboDrawingPreDefinedPaper.Enabled = false;
            this.comboDrawingPreDefinedPaper.FormattingEnabled = true;
            this.comboDrawingPreDefinedPaper.Location = new System.Drawing.Point(51, 92);
            this.comboDrawingPreDefinedPaper.Name = "comboDrawingPreDefinedPaper";
            this.comboDrawingPreDefinedPaper.Size = new System.Drawing.Size(152, 21);
            this.comboDrawingPreDefinedPaper.TabIndex = 3;
            this.comboDrawingPreDefinedPaper.SelectedIndexChanged += new System.EventHandler(this.ComboDrawingPreDefinedPaper_SelectedIndexChanged);
            // 
            // comboDrawingPreDefinedStandart
            // 
            this.comboDrawingPreDefinedStandart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboDrawingPreDefinedStandart.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboDrawingPreDefinedStandart.Enabled = false;
            this.comboDrawingPreDefinedStandart.FormattingEnabled = true;
            this.comboDrawingPreDefinedStandart.Location = new System.Drawing.Point(29, 65);
            this.comboDrawingPreDefinedStandart.Name = "comboDrawingPreDefinedStandart";
            this.comboDrawingPreDefinedStandart.Size = new System.Drawing.Size(174, 21);
            this.comboDrawingPreDefinedStandart.TabIndex = 3;
            this.comboDrawingPreDefinedStandart.SelectedIndexChanged += new System.EventHandler(this.ComboDrawingPreDefinedStandart_SelectedIndexChanged);
            // 
            // txtDrawingCustomHeight
            // 
            this.txtDrawingCustomHeight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDrawingCustomHeight.DefaultMeasureUnits = Syncfusion.Windows.Forms.Diagram.MeasureUnits.Pixel;
            this.txtDrawingCustomHeight.DefaultValue = 0F;
            this.txtDrawingCustomHeight.Enabled = false;
            this.txtDrawingCustomHeight.Location = new System.Drawing.Point(128, 143);
            this.txtDrawingCustomHeight.MeasureUnits = Syncfusion.Windows.Forms.Diagram.MeasureUnits.Pixel;
            this.txtDrawingCustomHeight.Name = "txtDrawingCustomHeight";
            this.txtDrawingCustomHeight.Size = new System.Drawing.Size(75, 20);
            this.txtDrawingCustomHeight.TabIndex = 4;
            this.txtDrawingCustomHeight.Value = 1F;
            this.txtDrawingCustomHeight.ValueChanged += new System.EventHandler(this.DrawingCustom_TextChanged);
            // 
            // txtDrawingCustomWidth
            // 
            this.txtDrawingCustomWidth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtDrawingCustomWidth.DefaultMeasureUnits = Syncfusion.Windows.Forms.Diagram.MeasureUnits.Pixel;
            this.txtDrawingCustomWidth.DefaultValue = 0F;
            this.txtDrawingCustomWidth.Enabled = false;
            this.txtDrawingCustomWidth.Location = new System.Drawing.Point(28, 143);
            this.txtDrawingCustomWidth.MeasureUnits = Syncfusion.Windows.Forms.Diagram.MeasureUnits.Pixel;
            this.txtDrawingCustomWidth.Name = "txtDrawingCustomWidth";
            this.txtDrawingCustomWidth.Size = new System.Drawing.Size(75, 20);
            this.txtDrawingCustomWidth.TabIndex = 1;
            this.txtDrawingCustomWidth.Value = 1F;
            this.txtDrawingCustomWidth.ValueChanged += new System.EventHandler(this.DrawingCustom_TextChanged);
            // 
            // rdDrawingNoScale
            // 
            this.rdDrawingNoScale.AutoSize = true;
            this.rdDrawingNoScale.Checked = true;
            this.rdDrawingNoScale.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.rdDrawingNoScale.Location = new System.Drawing.Point(9, 19);
            this.rdDrawingNoScale.Name = "rdDrawingNoScale";
            this.rdDrawingNoScale.Size = new System.Drawing.Size(91, 17);
            this.rdDrawingNoScale.TabIndex = 0;
            this.rdDrawingNoScale.TabStop = true;
            this.rdDrawingNoScale.Text = "No scale (1:1)";
            this.rdDrawingNoScale.UseVisualStyleBackColor = true;
            this.rdDrawingNoScale.CheckedChanged += new System.EventHandler(this.DrawingScale_CheckedChanged);
            // 
            // rdDrawingPreDefinedScale
            // 
            this.rdDrawingPreDefinedScale.AutoSize = true;
            this.rdDrawingPreDefinedScale.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.rdDrawingPreDefinedScale.Location = new System.Drawing.Point(9, 42);
            this.rdDrawingPreDefinedScale.Name = "rdDrawingPreDefinedScale";
            this.rdDrawingPreDefinedScale.Size = new System.Drawing.Size(110, 17);
            this.rdDrawingPreDefinedScale.TabIndex = 0;
            this.rdDrawingPreDefinedScale.Text = "Pre-defined scale:";
            this.rdDrawingPreDefinedScale.UseVisualStyleBackColor = true;
            this.rdDrawingPreDefinedScale.CheckedChanged += new System.EventHandler(this.DrawingScale_CheckedChanged);
            // 
            // rdDrawingCustomScale
            // 
            this.rdDrawingCustomScale.AutoSize = true;
            this.rdDrawingCustomScale.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.rdDrawingCustomScale.Location = new System.Drawing.Point(9, 119);
            this.rdDrawingCustomScale.Name = "rdDrawingCustomScale";
            this.rdDrawingCustomScale.Size = new System.Drawing.Size(91, 17);
            this.rdDrawingCustomScale.TabIndex = 0;
            this.rdDrawingCustomScale.Text = "Custom scale:";
            this.rdDrawingCustomScale.UseVisualStyleBackColor = true;
            this.rdDrawingCustomScale.CheckedChanged += new System.EventHandler(this.DrawingScale_CheckedChanged);
            // 
            // lblDrawingCustomScale
            // 
            this.lblDrawingCustomScale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblDrawingCustomScale.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblDrawingCustomScale.Location = new System.Drawing.Point(109, 141);
            this.lblDrawingCustomScale.Name = "lblDrawingCustomScale";
            this.lblDrawingCustomScale.Size = new System.Drawing.Size(14, 20);
            this.lblDrawingCustomScale.TabIndex = 2;
            this.lblDrawingCustomScale.Text = "=";
            this.lblDrawingCustomScale.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DrawingScaleControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpMeasurementUnits);
            this.Controls.Add(this.grpDrawingScale);
            this.Name = "DrawingScaleControl";
            this.Size = new System.Drawing.Size(221, 267);
            this.grpMeasurementUnits.ResumeLayout(false);
            this.grpMeasurementUnits.PerformLayout();
            this.grpDrawingScale.ResumeLayout(false);
            this.grpDrawingScale.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpMeasurementUnits;
        private System.Windows.Forms.Label lblDrawingScalePageSize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox grpDrawingScale;
        private System.Windows.Forms.Label lblDrawingCustomScale;
        public System.Windows.Forms.ComboBox comboMeasureUnits;
        public MeasureTextBox txtDrawingScalePageWidth;
        public MeasureTextBox txtDrawingScalePageHeight;
        public System.Windows.Forms.ComboBox comboDrawingPreDefinedPaper;
        public System.Windows.Forms.ComboBox comboDrawingPreDefinedStandart;
        public MeasureTextBox txtDrawingCustomHeight;
        public MeasureTextBox txtDrawingCustomWidth;
        public System.Windows.Forms.RadioButton rdDrawingNoScale;
        public System.Windows.Forms.RadioButton rdDrawingPreDefinedScale;
        public System.Windows.Forms.RadioButton rdDrawingCustomScale;
    }
}
