#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Forms.PivotAnalysis
{
    partial class FilterDropDown
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
            this.btnOk = new Syncfusion.Windows.Forms.ButtonAdv();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.parentNode = new Syncfusion.Windows.Forms.Tools.TreeViewAdv();
            this.BtnCancel = new Syncfusion.Windows.Forms.ButtonAdv();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.parentNode)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Office2007;
            this.btnOk.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.None;
            this.btnOk.Location = new System.Drawing.Point(40, 201);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(60, 27);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = PivotAnalysis.SR.GetString(PivotAnalysis.SR.OK);
            this.btnOk.UseVisualStyle = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.parentNode);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(199, 195);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            // 
            // parentNode
            // 
            this.parentNode.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.parentNode.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // 
            // 
            this.parentNode.HelpTextControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.parentNode.HelpTextControl.Location = new System.Drawing.Point(0, 0);
            this.parentNode.HelpTextControl.Name = "helpText";
            this.parentNode.HelpTextControl.Size = new System.Drawing.Size(49, 15);
            this.parentNode.HelpTextControl.TabIndex = 0;
            this.parentNode.HelpTextControl.Text = "help text";
            this.parentNode.Location = new System.Drawing.Point(3, 16);
            this.parentNode.Name = "parentNode";
            this.parentNode.Size = new System.Drawing.Size(193, 176);
            this.parentNode.TabIndex = 0;
            this.parentNode.Text = "treeViewAdv1";
            // 
            // 
            // 
            this.parentNode.ToolTipControl.BackColor = System.Drawing.SystemColors.Info;
            this.parentNode.ToolTipControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.parentNode.ToolTipControl.Location = new System.Drawing.Point(0, 0);
            this.parentNode.ToolTipControl.Name = "toolTip";
            this.parentNode.ToolTipControl.Size = new System.Drawing.Size(41, 15);
            this.parentNode.ToolTipControl.TabIndex = 1;
            this.parentNode.ToolTipControl.Text = "toolTip";
            this.parentNode.SelectedNodes.CollectionChanged += new System.ComponentModel.CollectionChangeEventHandler(SelectedNodes_CollectionChanged);
            // 
            // BtnCancel
            // 
            this.BtnCancel.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Office2007;
            this.BtnCancel.Location = new System.Drawing.Point(120, 201);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 27);
            this.BtnCancel.TabIndex = 4;
            this.BtnCancel.Text = PivotAnalysis.SR.GetString(PivotAnalysis.SR.Cancel);
            this.BtnCancel.UseVisualStyle = true;
            this.BtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // FilterDropDown
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnOk);
            this.Name = "FilterDropDown";
            this.Size = new System.Drawing.Size(201, 233);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.parentNode)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Syncfusion.Windows.Forms.ButtonAdv btnOk;
        private System.Windows.Forms.GroupBox groupBox1;
        private Syncfusion.Windows.Forms.Tools.TreeViewAdv parentNode;
        private ButtonAdv BtnCancel;
    }
}
