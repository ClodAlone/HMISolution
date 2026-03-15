#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.GridHelperClasses.Zoom
{
    /// <summary>
    /// Enables to perform zooming in Grouping Grid
    /// </summary>
    partial class ZoomGroupingGrid
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.popupControlContainer1 = new Syncfusion.Windows.Forms.PopupControlContainer();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();

            ((System.ComponentModel.ISupportInitialize)(this.popupControlContainer1)).BeginInit();
            this.popupControlContainer1.SuspendLayout();

            this.SuspendLayout();
            // 
            // ZoomGroupingGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(428, 332);
            this.Name = "ZoomGroupingGrid";
            this.Text = "Form1";
            this.ResumeLayout(false);

            this.popupControlContainer1.Controls.Add(this.pictureBox1);
            this.popupControlContainer1.Location = new System.Drawing.Point(9, 9);
            this.popupControlContainer1.Name = "popupControlContainer1";
            this.popupControlContainer1.Size = new System.Drawing.Size(86, 139);
            this.popupControlContainer1.TabIndex = 1;

            this.pictureBox1.Location = new System.Drawing.Point(2, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(84, 134);
            this.pictureBox1.TabIndex = 0;

        }

        #endregion

        private Syncfusion.Windows.Forms.PopupControlContainer popupControlContainer1;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}