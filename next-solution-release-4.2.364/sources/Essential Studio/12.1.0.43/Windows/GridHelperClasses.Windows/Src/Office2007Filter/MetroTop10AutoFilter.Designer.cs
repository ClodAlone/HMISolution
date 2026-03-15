#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.GridHelperClasses
{
    partial class MetroTop10AutoFilter
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cancelButton = new Syncfusion.Windows.Forms.ButtonAdv();
            this.okButton = new Syncfusion.Windows.Forms.ButtonAdv();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.itemscomboBoxAdv1 = new Syncfusion.Windows.Forms.Tools.ComboBoxAdv();
            this.topComboBox1 = new Syncfusion.Windows.Forms.Tools.ComboBoxAdv();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.itemscomboBoxAdv1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.topComboBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cancelButton);
            this.groupBox1.Controls.Add(this.okButton);
            this.groupBox1.Controls.Add(this.numericUpDown1);
            this.groupBox1.Controls.Add(this.itemscomboBoxAdv1);
            this.groupBox1.Controls.Add(this.topComboBox1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(271, 85);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Show";
            // 
            // cancelButton
            // 
            this.cancelButton.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.cancelButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(165)))), ((int)(((byte)(220)))));
            this.cancelButton.BeforeTouchSize = new System.Drawing.Size(71, 24);
            this.cancelButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.ForeColor = System.Drawing.Color.White;
            this.cancelButton.IsBackStageButton = false;
            this.cancelButton.Location = new System.Drawing.Point(188, 52);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(71, 24);
            this.cancelButton.TabIndex = 8;
            this.cancelButton.UseVisualStyle = true;
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Text = SR.GetString(SR.CustomAutoFilterCancel);
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.okButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(165)))), ((int)(((byte)(220)))));
            this.okButton.BeforeTouchSize = new System.Drawing.Size(73, 24);
            this.okButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.okButton.ForeColor = System.Drawing.Color.White;
            this.okButton.IsBackStageButton = false;
            this.okButton.Location = new System.Drawing.Point(106, 52);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(73, 24);
            this.okButton.TabIndex = 7;
            this.okButton.UseVisualStyle = true;
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Text = SR.GetString(SR.CustomAutoFilterOK);
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.numericUpDown1.Location = new System.Drawing.Point(106, 20);
            this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(56, 20);
            this.numericUpDown1.TabIndex = 4;
            this.numericUpDown1.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // itemscomboBoxAdv1
            // 
            this.itemscomboBoxAdv1.BackColor = System.Drawing.Color.White;
            this.itemscomboBoxAdv1.BeforeTouchSize = new System.Drawing.Size(89, 21);
            this.itemscomboBoxAdv1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.itemscomboBoxAdv1.Location = new System.Drawing.Point(170, 20);
            this.itemscomboBoxAdv1.Name = "itemscomboBoxAdv1";
            this.itemscomboBoxAdv1.Size = new System.Drawing.Size(89, 21);
            this.itemscomboBoxAdv1.Style = Syncfusion.Windows.Forms.VisualStyle.Metro;
            this.itemscomboBoxAdv1.TabIndex = 3;
            // 
            // topComboBox1
            // 
            this.topComboBox1.BackColor = System.Drawing.Color.White;
            this.topComboBox1.BeforeTouchSize = new System.Drawing.Size(80, 21);
            this.topComboBox1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.topComboBox1.Location = new System.Drawing.Point(18, 19);
            this.topComboBox1.Name = "topComboBox1";
            this.topComboBox1.Size = new System.Drawing.Size(80, 21);
            this.topComboBox1.Style = Syncfusion.Windows.Forms.VisualStyle.Metro;
            this.topComboBox1.TabIndex = 2;
            // 
            // MetroTop10AutoFilter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(271, 85);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MetroTop10AutoFilter";
            this.ShowIcon = false;
            this.Text = SR.GetString(SR.CustomTop10AutoFilterDialogBox);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.itemscomboBoxAdv1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.topComboBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private Windows.Forms.Tools.ComboBoxAdv itemscomboBoxAdv1;
        private Windows.Forms.Tools.ComboBoxAdv topComboBox1;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private Windows.Forms.ButtonAdv cancelButton;
        private Windows.Forms.ButtonAdv okButton;
    }
}