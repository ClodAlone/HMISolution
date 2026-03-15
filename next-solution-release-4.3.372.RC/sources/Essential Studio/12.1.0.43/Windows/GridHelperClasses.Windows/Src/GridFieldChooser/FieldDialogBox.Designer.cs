//-------------------------------------------------------------------------------------------------
// <copyright file="FieldDialogBox.Designer.cs" company="Syncfusion">
// Copyright (c) Syncfusion.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.GridHelperClasses
{
    /// <summary>
    /// For internal use.
    /// </summary>
    partial class FieldDialogBox
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
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
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
            this.directorySearcher1 = new System.DirectoryServices.DirectorySearcher();
            this.FieldChooserGridList = new Syncfusion.Windows.Forms.Grid.GridControl();
            ((System.ComponentModel.ISupportInitialize)(this.FieldChooserGridList)).BeginInit();
            this.SuspendLayout();
            // 
            // directorySearcher1
            // 
            this.directorySearcher1.ClientTimeout = System.TimeSpan.Parse("-00:00:01");
            this.directorySearcher1.ServerPageTimeLimit = System.TimeSpan.Parse("-00:00:01");
            this.directorySearcher1.ServerTimeLimit = System.TimeSpan.Parse("-00:00:01");
            // 
            // FieldChooserGridList
            // 
            this.FieldChooserGridList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FieldChooserGridList.Location = new System.Drawing.Point(0, 0);
            this.FieldChooserGridList.Name = "FieldChooserGridList";
            this.FieldChooserGridList.SerializeCellsBehavior = Syncfusion.Windows.Forms.Grid.GridSerializeCellsBehavior.SerializeAsRangeStylesIntoCode;
            this.FieldChooserGridList.Size = new System.Drawing.Size(this.MinimumWidth, 257);
            this.FieldChooserGridList.SmartSizeBox = false;
            this.FieldChooserGridList.TabIndex = 2;
            this.FieldChooserGridList.Text = "gridControl1";
            this.FieldChooserGridList.UseRightToLeftCompatibleTextBox = true;
            // 
            // FieldDialogBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(181, 257);
            this.Controls.Add(this.FieldChooserGridList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FieldDialogBox";
            this.ShowInTaskbar = false;
            this.Text = SR.GetString("FieldDialogBox");
            //this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.FieldChooserGridList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.DirectoryServices.DirectorySearcher directorySearcher1;
        /// <summary>
        /// For internal use.
        /// </summary>
        public Syncfusion.Windows.Forms.Grid.GridControl FieldChooserGridList;
    }
}
