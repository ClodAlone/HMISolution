#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Drawing;
namespace Syncfusion.Diagram.Base.Wizard
{
    partial class DiagramLoadBaseWizard
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
            this.btnLoadEDD = new System.Windows.Forms.Button();
            this.btnLoadEDP = new System.Windows.Forms.Button();
            this.btnDiagramBuilder = new System.Windows.Forms.Button();
            this.btnSymbolDesigner = new System.Windows.Forms.Button();
            this.chbStartup = new System.Windows.Forms.CheckBox();
            this.dEDD = new System.Windows.Forms.OpenFileDialog();
            this.dEDP = new System.Windows.Forms.OpenFileDialog();
            this.btnOk = new Syncfusion.Windows.Forms.Diagram.ImageButton();
            this.btnCancel = new Syncfusion.Windows.Forms.Diagram.ImageButton();
            this.lblEdd = new System.Windows.Forms.Label();
            this.lblEdp = new System.Windows.Forms.Label();
            this.lblDiagramBuilder = new System.Windows.Forms.Label();
            this.lblSymbolDesigner = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnLoadEDD
            // 
            this.btnLoadEDD.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnLoadEDD.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLoadEDD.Font = new System.Drawing.Font("Corbel", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoadEDD.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(66)))), ((int)(((byte)(139)))));
            this.btnLoadEDD.Location = new System.Drawing.Point(12, 170);
            this.btnLoadEDD.Name = "btnLoadEDD";
            this.btnLoadEDD.Size = new System.Drawing.Size(150, 40);
            this.btnLoadEDD.TabIndex = 0;
            this.btnLoadEDD.Text = "Load EDD File";
            this.btnLoadEDD.UseVisualStyleBackColor = true;
            this.btnLoadEDD.Click += new System.EventHandler(this.LoadEDD_Click);
            // 
            // btnLoadEDP
            // 
            this.btnLoadEDP.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnLoadEDP.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLoadEDP.Font = new System.Drawing.Font("Corbel", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoadEDP.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(66)))), ((int)(((byte)(139)))));
            this.btnLoadEDP.Location = new System.Drawing.Point(12, 258);
            this.btnLoadEDP.Name = "btnLoadEDP";
            this.btnLoadEDP.Size = new System.Drawing.Size(150, 40);
            this.btnLoadEDP.TabIndex = 1;
            this.btnLoadEDP.Text = "Load EDP File";
            this.btnLoadEDP.UseVisualStyleBackColor = true;
            this.btnLoadEDP.Click += new System.EventHandler(this.LoadEDP_Click);
            // 
            // btnDiagramBuilder
            // 
            this.btnDiagramBuilder.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnDiagramBuilder.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDiagramBuilder.Location = new System.Drawing.Point(336, 170);
            this.btnDiagramBuilder.Name = "btnDiagramBuilder";
            this.btnDiagramBuilder.Size = new System.Drawing.Size(260, 40);
            this.btnDiagramBuilder.TabIndex = 2;
            this.btnDiagramBuilder.UseVisualStyleBackColor = true;
            this.btnDiagramBuilder.Click += new System.EventHandler(this.DiagramBuilder_Click);
            // 
            // btnSymbolDesigner
            // 
            this.btnSymbolDesigner.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnSymbolDesigner.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSymbolDesigner.Location = new System.Drawing.Point(336, 257);
            this.btnSymbolDesigner.Name = "btnSymbolDesigner";
            this.btnSymbolDesigner.Size = new System.Drawing.Size(260, 40);
            this.btnSymbolDesigner.TabIndex = 3;
            this.btnSymbolDesigner.UseVisualStyleBackColor = true;
            this.btnSymbolDesigner.Click += new System.EventHandler(this.SymbolDesigner_Click);
            // 
            // chbStartup
            // 
            this.chbStartup.AutoSize = true;
            this.chbStartup.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chbStartup.ForeColor = System.Drawing.Color.MidnightBlue;
            this.chbStartup.Location = new System.Drawing.Point(27, 360);
            this.chbStartup.Name = "chbStartup";
            this.chbStartup.Size = new System.Drawing.Size(119, 19);
            this.chbStartup.TabIndex = 4;
            this.chbStartup.Text = "Show on startup";
            this.chbStartup.UseVisualStyleBackColor = true;
            this.chbStartup.CheckedChanged += new System.EventHandler(this.Startup_CheckedChanged);
            // 
            // dEDD
            // 
            this.dEDD.FileName = "dEDD";
            this.dEDD.Filter = "Diagram files|*.edd";
            // 
            // dEDP
            // 
            this.dEDP.FileName = "dEDP";
            this.dEDP.Filter = "Palettes|*.edp";
            // 
            // btnOk
            // 
            this.btnOk.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnOk.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.FlatAppearance.BorderSize = 0;
            this.btnOk.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnOk.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOk.Font = new System.Drawing.Font("Corbel", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOk.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(66)))), ((int)(((byte)(139)))));
            this.btnOk.HoverImage = null;
            this.btnOk.Location = new System.Drawing.Point(355, 348);
            this.btnOk.Name = "btnOk";
            this.btnOk.NormalImage = null;
            this.btnOk.Size = new System.Drawing.Size(105, 36);
            this.btnOk.TabIndex = 8;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Corbel", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(66)))), ((int)(((byte)(139)))));
            this.btnCancel.HoverImage = null;
            this.btnCancel.Location = new System.Drawing.Point(487, 348);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.NormalImage = null;
            this.btnCancel.Size = new System.Drawing.Size(105, 36);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblEdd
            // 
            this.lblEdd.AutoSize = true;
            this.lblEdd.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEdd.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(66)))), ((int)(((byte)(139)))));
            this.lblEdd.Location = new System.Drawing.Point(12, 125);
            this.lblEdd.Name = "lblEdd";
            this.lblEdd.Size = new System.Drawing.Size(165, 42);
            this.lblEdd.TabIndex = 10;
            this.lblEdd.BackColor = Color.Transparent;
            this.lblEdd.Text = "Click to load a Diagram file to the \r\nDiagram control. Enabled when \r\nDiagram con" +
                "trol is available.\r\n";
            // 
            // lblEdp
            // 
            this.lblEdp.AutoSize = true;
            this.lblEdp.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEdp.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(66)))), ((int)(((byte)(139)))));
            this.lblEdp.Location = new System.Drawing.Point(9, 213);
            this.lblEdp.Name = "lblEdp";
            this.lblEdp.Size = new System.Drawing.Size(212, 42);
            this.lblEdp.TabIndex = 11;
            this.lblEdp.BackColor = Color.Transparent;
            this.lblEdp.Text = "Click to load a Palette file to the Palette \r\nGroupView control. Enabled when Pal" +
                "ette \r\nGroupView control is available.\r\n";
            // 
            // lblDiagramBuilder
            // 
            this.lblDiagramBuilder.AutoSize = true;
            this.lblDiagramBuilder.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDiagramBuilder.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(66)))), ((int)(((byte)(139)))));
            this.lblDiagramBuilder.Location = new System.Drawing.Point(336, 133);
            this.lblDiagramBuilder.Name = "lblDiagramBuilder";
            this.lblDiagramBuilder.Size = new System.Drawing.Size(278, 28);
            this.lblDiagramBuilder.TabIndex = 12;
            this.lblDiagramBuilder.BackColor = Color.Transparent;
            this.lblDiagramBuilder.Text = "Click to start a Diagram Builder application. Helps design \r\ndiagrams with user-f" +
                "riendly tools.";
            // 
            // lblSymbolDesigner
            // 
            this.lblSymbolDesigner.AutoSize = true;
            this.lblSymbolDesigner.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSymbolDesigner.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(66)))), ((int)(((byte)(139)))));
            this.lblSymbolDesigner.Location = new System.Drawing.Point(336, 223);
            this.lblSymbolDesigner.Name = "lblSymbolDesigner";
            this.lblSymbolDesigner.Size = new System.Drawing.Size(274, 28);
            this.lblSymbolDesigner.TabIndex = 13;
            this.lblSymbolDesigner.BackColor = Color.Transparent;
            this.lblSymbolDesigner.Text = "Click to start a Symbol Builder application. Helps design \r\ncustom symbols and sh" +
                "apes.";
            // 
            // DiagramLoadBaseWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(652, 397);
            this.Controls.Add(this.lblSymbolDesigner);
            this.Controls.Add(this.lblDiagramBuilder);
            this.Controls.Add(this.btnLoadEDP);
            this.Controls.Add(this.lblEdp);
            this.Controls.Add(this.btnLoadEDD);
            this.Controls.Add(this.lblEdd);
            this.Controls.Add(this.btnDiagramBuilder);
            this.Controls.Add(this.btnSymbolDesigner);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.chbStartup);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(5)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DiagramLoadBaseWizard";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Diagram Wizard";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        /// <summary>
        /// Load EDD button.
        /// </summary>
        protected System.Windows.Forms.Button btnLoadEDD;
        /// <summary>
        /// Load EDP button.
        /// </summary>
        protected System.Windows.Forms.Button btnLoadEDP;
        private System.Windows.Forms.CheckBox chbStartup;
        private System.Windows.Forms.OpenFileDialog dEDD;
        /// <summary>
        /// Open file dialog.
        /// </summary>
        protected System.Windows.Forms.OpenFileDialog dEDP;
        /// <summary>
        /// Load diagram builder button.
        /// </summary>
        protected System.Windows.Forms.Button btnDiagramBuilder;
        /// <summary>
        /// Load Symbol designer button.
        /// </summary>
        protected System.Windows.Forms.Button btnSymbolDesigner;
        /// <summary>
        /// OK button.
        /// </summary>
        protected Syncfusion.Windows.Forms.Diagram.ImageButton btnOk;
        /// <summary>
        /// Cancel button.
        /// </summary>
        protected Syncfusion.Windows.Forms.Diagram.ImageButton btnCancel;
        private System.Windows.Forms.Label lblEdd;
        private System.Windows.Forms.Label lblEdp;
        private System.Windows.Forms.Label lblDiagramBuilder;
        private System.Windows.Forms.Label lblSymbolDesigner;
    }
}
