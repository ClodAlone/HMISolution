#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System.Windows.Forms;
namespace Syncfusion.Windows.Forms.Tools.Controls.RibbonTabBar
{
    /// <summary>
    /// RibbonTabBar Header
    /// </summary>
   public partial class RibbonTabBarHeader
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
            this.pnlLeft = new System.Windows.Forms.ToolStrip();
            this.pnlTop = new System.Windows.Forms.ToolStrip();
            this.ribbonHeader = new Syncfusion.Windows.Forms.Tools.RibbonHeaderControl();
            this.SuspendLayout();
            // 
            // pnlLeft
            // 
            this.pnlLeft.AutoSize = false;
            this.pnlLeft.CanOverflow = false;
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlLeft.ForeColor = System.Drawing.Color.MidnightBlue;
            this.pnlLeft.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.pnlLeft.ImageScalingSize = new System.Drawing.Size(60, 60);
            this.pnlLeft.Location = new System.Drawing.Point(0, 0);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Size = new System.Drawing.Size(80, 89);
            this.pnlLeft.TabIndex = 0;
            // 
            // pnlTop
            // 
            this.pnlTop.AutoSize = false;
            this.pnlTop.ForeColor = System.Drawing.Color.MidnightBlue;
            this.pnlTop.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.pnlTop.Location = new System.Drawing.Point(80, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(815, 30);
            this.pnlTop.TabIndex = 1;
            // 
            // ribbonHeader
            // 
            this.ribbonHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ribbonHeader.FillWidthWithItems = false;
            this.ribbonHeader.Location = new System.Drawing.Point(80, 30);
            this.ribbonHeader.Name = "ribbonHeader";
            this.ribbonHeader.Size = new System.Drawing.Size(815, 59);
            this.ribbonHeader.TabIndex = 2;
            // 
            // RibbonTabBarHeader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ribbonHeader);
            this.Controls.Add(this.pnlTop);
            this.Controls.Add(this.pnlLeft);
            this.Name = "RibbonTabBarHeader";
            this.Size = new System.Drawing.Size(895, 89);
            this.ResumeLayout(false);

        }

        #endregion

        private ToolStrip pnlLeft;
        private ToolStrip pnlTop;
        private RibbonHeaderControl ribbonHeader;
    }
}
#endif
