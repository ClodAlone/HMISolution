#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Progress form used to display the visio palette conversion.
    /// </summary>
    [
    Syncfusion.Documentation.DocumentationExclude()
    ]
    public class ProgressDialog : Form, IStop
    {
        private ProgressBar progressBar1;
        private System.Windows.Forms.Label label1;
        private bool m_bIncrementing = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressDialog"/> class.
        /// </summary>
        public ProgressDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(8, 32);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(248, 24);
            this.progressBar1.Step = 5;
            this.progressBar1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(248, 24);
            this.label1.TabIndex = 1;
            this.label1.Text = "Converting...";
            // 
            // ProgressDialog
            // 
#if SyncfusionFramework2_0
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
#else
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);    
#endif
            this.ClientSize = new System.Drawing.Size(266, 64);
            this.ControlBox = false;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressBar1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ProgressDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Essential Diagram VSSConverter";
            this.Load += new System.EventHandler(this.ProgressDialog_Load);
            this.ResumeLayout(false);

        }
        #endregion

        private void Timer_Tick(object sender, System.EventArgs e)
        {
            int nIncrement = 5;
            if (!m_bIncrementing)
                nIncrement = -5;

            if ((progressBar1.Value + nIncrement) < progressBar1.Minimum && !m_bIncrementing)
            {
                m_bIncrementing = true;
                nIncrement = 5;
            }
            else if ((progressBar1.Value + nIncrement) > progressBar1.Maximum && m_bIncrementing)
            {
                m_bIncrementing = false;
                nIncrement = -5;
            }

            progressBar1.Increment(nIncrement);

            this.label1.Invalidate();
        }

        private void ProgressDialog_Load(object sender, System.EventArgs e)
        {
            Timer timer1 = new Timer();
            timer1.Interval = 50;
            timer1.Enabled = true;
            timer1.Tick += new System.EventHandler(Timer_Tick);
            progressBar1.Show();
            timer1.Start();
        }

        /// <summary>
        /// Closes the form.
        /// </summary>
        public void CloseForm()
        {
            this.CloseForm();
        }
    }

    /// <summary>
    /// Interface to stop the progress form.
    /// </summary>
    public interface IStop
    {
        /// <summary>
        /// Closes the form.
        /// </summary>
        void CloseForm();
    }
}
