#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !(SyncfusionFramework1_0 || SyncfusionFramework1_1)

namespace Syncfusion.Windows.Forms
{
	partial class MessageBoxFormAdv
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing && (components != null) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.m_ok = new Syncfusion.Windows.Forms.ButtonAdv();
			this.m_yes = new Syncfusion.Windows.Forms.ButtonAdv();
			this.m_abort = new Syncfusion.Windows.Forms.ButtonAdv();
			this.m_retry = new Syncfusion.Windows.Forms.ButtonAdv();
			this.m_ignore = new Syncfusion.Windows.Forms.ButtonAdv();
			this.m_no = new Syncfusion.Windows.Forms.ButtonAdv();
			this.m_cancel = new Syncfusion.Windows.Forms.ButtonAdv();
			this.m_text = new System.Windows.Forms.Label();
			this.m_image = new System.Windows.Forms.PictureBox();
			this.m_contentPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.m_buttonsPanel = new System.Windows.Forms.FlowLayoutPanel();
			((System.ComponentModel.ISupportInitialize)(this.m_image)).BeginInit();
			this.m_contentPanel.SuspendLayout();
			this.m_buttonsPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_ok
			// 
			this.m_ok.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Office2007;
			this.m_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.m_ok.Location = new System.Drawing.Point( 515, 5 );
			this.m_ok.Margin = new System.Windows.Forms.Padding( 5, 5, 5, 8 );
			this.m_ok.Name = "m_ok";
			this.m_ok.Size = new System.Drawing.Size( 75, 23 );
			this.m_ok.TabIndex = 0;
			this.m_ok.Text = "&OK";
			this.m_ok.UseVisualStyle = true;
			this.m_ok.Visible = false;
			// 
			// m_yes
			// 
			this.m_yes.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Office2007;
			this.m_yes.DialogResult = System.Windows.Forms.DialogResult.Yes;
			this.m_yes.Location = new System.Drawing.Point( 430, 5 );
			this.m_yes.Margin = new System.Windows.Forms.Padding( 5, 5, 5, 8 );
			this.m_yes.Name = "m_yes";
			this.m_yes.Size = new System.Drawing.Size( 75, 23 );
			this.m_yes.TabIndex = 0;
			this.m_yes.Text = "&Yes";
			this.m_yes.UseVisualStyle = true;
			this.m_yes.Visible = false;
			// 
			// m_abort
			// 
			this.m_abort.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Office2007;
			this.m_abort.DialogResult = System.Windows.Forms.DialogResult.Abort;
			this.m_abort.Location = new System.Drawing.Point( 345, 5 );
			this.m_abort.Margin = new System.Windows.Forms.Padding( 5, 5, 5, 8 );
			this.m_abort.Name = "m_abort";
			this.m_abort.Size = new System.Drawing.Size( 75, 23 );
			this.m_abort.TabIndex = 0;
			this.m_abort.Text = "&Abort";
			this.m_abort.UseVisualStyle = true;
			this.m_abort.Visible = false;
			// 
			// m_retry
			// 
			this.m_retry.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Office2007;
			this.m_retry.DialogResult = System.Windows.Forms.DialogResult.Retry;
			this.m_retry.Location = new System.Drawing.Point( 5, 5 );
			this.m_retry.Margin = new System.Windows.Forms.Padding( 5, 5, 5, 8 );
			this.m_retry.Name = "m_retry";
			this.m_retry.Size = new System.Drawing.Size( 75, 23 );
			this.m_retry.TabIndex = 0;
			this.m_retry.Text = "&Retry";
			this.m_retry.UseVisualStyle = true;
			this.m_retry.Visible = false;
			// 
			// m_ignore
			// 
			this.m_ignore.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Office2007;
			this.m_ignore.DialogResult = System.Windows.Forms.DialogResult.Ignore;
			this.m_ignore.Location = new System.Drawing.Point( 260, 5 );
			this.m_ignore.Margin = new System.Windows.Forms.Padding( 5, 5, 5, 8 );
			this.m_ignore.Name = "m_ignore";
			this.m_ignore.Size = new System.Drawing.Size( 75, 23 );
			this.m_ignore.TabIndex = 0;
			this.m_ignore.Text = "&Ignore";
			this.m_ignore.UseVisualStyle = true;
			this.m_ignore.Visible = false;
			// 
			// m_no
			// 
			this.m_no.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Office2007;
			this.m_no.DialogResult = System.Windows.Forms.DialogResult.No;
			this.m_no.Location = new System.Drawing.Point( 175, 5 );
			this.m_no.Margin = new System.Windows.Forms.Padding( 5, 5, 5, 8 );
			this.m_no.Name = "m_no";
			this.m_no.Size = new System.Drawing.Size( 75, 23 );
			this.m_no.TabIndex = 0;
			this.m_no.Text = "&No";
			this.m_no.UseVisualStyle = true;
			this.m_no.Visible = false;
			// 
			// m_cancel
			// 
			this.m_cancel.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Office2007;
			this.m_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.m_cancel.Location = new System.Drawing.Point( 90, 5 );
			this.m_cancel.Margin = new System.Windows.Forms.Padding( 5, 5, 5, 8 );
			this.m_cancel.Name = "m_cancel";
			this.m_cancel.Size = new System.Drawing.Size( 75, 23 );
			this.m_cancel.TabIndex = 0;
			this.m_cancel.Text = "&Cancel";
			this.m_cancel.UseVisualStyle = true;
			this.m_cancel.Visible = false;
			// 
			// m_text
			// 
			this.m_text.BackColor = System.Drawing.Color.Transparent;
			this.m_text.Location = new System.Drawing.Point( 73, 10 );
			this.m_text.Margin = new System.Windows.Forms.Padding( 10, 10, 5, 5 );
			this.m_text.Name = "m_text";
			this.m_text.Size = new System.Drawing.Size( 513, 48 );
			this.m_text.TabIndex = 0;
			this.m_text.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// m_image
			// 
			this.m_image.BackColor = System.Drawing.Color.Transparent;
			this.m_image.Location = new System.Drawing.Point( 5, 10 );
			this.m_image.Margin = new System.Windows.Forms.Padding( 5, 10, 10, 5 );
			this.m_image.Name = "m_image";
			this.m_image.Size = new System.Drawing.Size( 48, 48 );
			this.m_image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.m_image.TabIndex = 2;
			this.m_image.TabStop = false;
			this.m_image.Visible = false;
			// 
			// m_contentPanel
			// 
			this.m_contentPanel.AutoSize = true;
			this.m_contentPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.m_contentPanel.BackColor = System.Drawing.Color.Transparent;
			this.m_contentPanel.Controls.Add( this.m_image );
			this.m_contentPanel.Controls.Add( this.m_text );
			this.m_contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_contentPanel.Location = new System.Drawing.Point( 5, 0 );
			this.m_contentPanel.Name = "m_contentPanel";
			this.m_contentPanel.Size = new System.Drawing.Size( 591, 67 );
			this.m_contentPanel.TabIndex = 0;
			this.m_contentPanel.WrapContents = false;
			// 
			// m_buttonsPanel
			// 
			this.m_buttonsPanel.AutoSize = true;
			this.m_buttonsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.m_buttonsPanel.BackColor = System.Drawing.Color.Transparent;
			this.m_buttonsPanel.Controls.Add( this.m_retry );
			this.m_buttonsPanel.Controls.Add( this.m_cancel );
			this.m_buttonsPanel.Controls.Add( this.m_no );
			this.m_buttonsPanel.Controls.Add( this.m_ignore );
			this.m_buttonsPanel.Controls.Add( this.m_abort );
			this.m_buttonsPanel.Controls.Add( this.m_yes );
			this.m_buttonsPanel.Controls.Add( this.m_ok );
			this.m_buttonsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.m_buttonsPanel.Location = new System.Drawing.Point( 5, 67 );
			this.m_buttonsPanel.Name = "m_buttonsPanel";
			this.m_buttonsPanel.Size = new System.Drawing.Size( 591, 36 );
			this.m_buttonsPanel.TabIndex = 0;
			this.m_buttonsPanel.WrapContents = false;
			// 
			// MessageBoxFormAdv
			// 
			this.ClientSize = new System.Drawing.Size( 601, 103 );
			this.Controls.Add( this.m_contentPanel );
			this.Controls.Add( this.m_buttonsPanel );
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MessageBoxFormAdv";
			this.Padding = new System.Windows.Forms.Padding( 5, 0, 5, 0 );
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.UseOffice2007SchemeBackColor = true;
			((System.ComponentModel.ISupportInitialize)(this.m_image)).EndInit();
			this.m_contentPanel.ResumeLayout( false );
			this.m_buttonsPanel.ResumeLayout( false );
			this.ResumeLayout( false );
			this.PerformLayout();

		}

		#endregion
	}
}

#endif