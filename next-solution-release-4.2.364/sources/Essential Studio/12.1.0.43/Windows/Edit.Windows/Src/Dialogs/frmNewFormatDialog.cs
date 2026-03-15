#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using Syncfusion.Windows.Forms.Edit;
using Syncfusion.Windows.Forms.Edit.Enums;
using Syncfusion.Windows.Forms.Edit.Implementation;
using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Implementation.Formatting;
using Syncfusion.Windows.Forms.Edit.Implementation.Config;

namespace Syncfusion.Windows.Forms.Edit.Dialogs
{
	/// <summary>
	/// Dialog for new format.
	/// </summary>
	public class frmNewFormatDialog
		: System.Windows.Forms.Form
	{
		#region Fields
		/// <summary>
		/// Format manager.
		/// </summary>
		private Format m_format;
		#endregion

		#region Controls
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Label lblName;
		private System.Windows.Forms.Label lblSettings;
		private System.Windows.Forms.ComboBox txtName;
		private Syncfusion.Windows.Forms.Edit.Dialogs.ControlFormatsSettings ctrlFormat;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		#endregion

		#region Properties
		/// <summary>
		/// Gets newly created format.
		/// </summary>
		public Format NewFormat
		{
			get
			{
				return m_format;
			}
		}
		#endregion

		#region Initialization/Finalization
		/// <summary>
		/// Creates new form instance.
		/// </summary>
		public frmNewFormatDialog()
		{
			m_format = new Format();
			InitializeComponent();

			txtName.BeginUpdate();
			txtName.Items.Clear();

			string[] strNames = Enum.GetNames( typeof( FormatType ) );

			foreach( string name in strNames )
			{
				txtName.Items.Add( name );
			}

			txtName.EndUpdate();
			txtName.Text = m_format.Name;

			ctrlFormat.Formats.Clear();
			ctrlFormat.Formats.Add( m_format );
		}
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( components != null )
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager( typeof( frmNewFormatDialog ) );
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOk = new System.Windows.Forms.Button();
			this.lblName = new System.Windows.Forms.Label();
			this.lblSettings = new System.Windows.Forms.Label();
			this.txtName = new System.Windows.Forms.ComboBox();
			this.ctrlFormat = new Syncfusion.Windows.Forms.Edit.Dialogs.ControlFormatsSettings();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.AccessibleDescription = resources.GetString( "btnCancel.AccessibleDescription" );
			this.btnCancel.AccessibleName = resources.GetString( "btnCancel.AccessibleName" );
			this.btnCancel.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "btnCancel.Anchor" ) ) );
			this.btnCancel.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "btnCancel.BackgroundImage" ) ) );
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "btnCancel.Dock" ) ) );
			this.btnCancel.Enabled = ( ( bool )( resources.GetObject( "btnCancel.Enabled" ) ) );
			this.btnCancel.FlatStyle = ( ( System.Windows.Forms.FlatStyle )( resources.GetObject( "btnCancel.FlatStyle" ) ) );
			this.btnCancel.Font = ( ( System.Drawing.Font )( resources.GetObject( "btnCancel.Font" ) ) );
			this.btnCancel.Image = ( ( System.Drawing.Image )( resources.GetObject( "btnCancel.Image" ) ) );
			this.btnCancel.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnCancel.ImageAlign" ) ) );
			this.btnCancel.ImageIndex = ( ( int )( resources.GetObject( "btnCancel.ImageIndex" ) ) );
			this.btnCancel.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "btnCancel.ImeMode" ) ) );
			this.btnCancel.Location = ( ( System.Drawing.Point )( resources.GetObject( "btnCancel.Location" ) ) );
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "btnCancel.RightToLeft" ) ) );
			this.btnCancel.Size = ( ( System.Drawing.Size )( resources.GetObject( "btnCancel.Size" ) ) );
			this.btnCancel.TabIndex = ( ( int )( resources.GetObject( "btnCancel.TabIndex" ) ) );
			this.btnCancel.Text = resources.GetString( "btnCancel.Text" );
			this.btnCancel.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnCancel.TextAlign" ) ) );
			this.btnCancel.Visible = ( ( bool )( resources.GetObject( "btnCancel.Visible" ) ) );
			// 
			// btnOk
			// 
			this.btnOk.AccessibleDescription = resources.GetString( "btnOk.AccessibleDescription" );
			this.btnOk.AccessibleName = resources.GetString( "btnOk.AccessibleName" );
			this.btnOk.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "btnOk.Anchor" ) ) );
			this.btnOk.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "btnOk.BackgroundImage" ) ) );
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "btnOk.Dock" ) ) );
			this.btnOk.Enabled = ( ( bool )( resources.GetObject( "btnOk.Enabled" ) ) );
			this.btnOk.FlatStyle = ( ( System.Windows.Forms.FlatStyle )( resources.GetObject( "btnOk.FlatStyle" ) ) );
			this.btnOk.Font = ( ( System.Drawing.Font )( resources.GetObject( "btnOk.Font" ) ) );
			this.btnOk.Image = ( ( System.Drawing.Image )( resources.GetObject( "btnOk.Image" ) ) );
			this.btnOk.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnOk.ImageAlign" ) ) );
			this.btnOk.ImageIndex = ( ( int )( resources.GetObject( "btnOk.ImageIndex" ) ) );
			this.btnOk.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "btnOk.ImeMode" ) ) );
			this.btnOk.Location = ( ( System.Drawing.Point )( resources.GetObject( "btnOk.Location" ) ) );
			this.btnOk.Name = "btnOk";
			this.btnOk.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "btnOk.RightToLeft" ) ) );
			this.btnOk.Size = ( ( System.Drawing.Size )( resources.GetObject( "btnOk.Size" ) ) );
			this.btnOk.TabIndex = ( ( int )( resources.GetObject( "btnOk.TabIndex" ) ) );
			this.btnOk.Text = resources.GetString( "btnOk.Text" );
			this.btnOk.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnOk.TextAlign" ) ) );
			this.btnOk.Visible = ( ( bool )( resources.GetObject( "btnOk.Visible" ) ) );
			// 
			// lblName
			// 
			this.lblName.AccessibleDescription = resources.GetString( "lblName.AccessibleDescription" );
			this.lblName.AccessibleName = resources.GetString( "lblName.AccessibleName" );
			this.lblName.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "lblName.Anchor" ) ) );
			this.lblName.AutoSize = ( ( bool )( resources.GetObject( "lblName.AutoSize" ) ) );
			this.lblName.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "lblName.Dock" ) ) );
			this.lblName.Enabled = ( ( bool )( resources.GetObject( "lblName.Enabled" ) ) );
			this.lblName.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.lblName.Font = ( ( System.Drawing.Font )( resources.GetObject( "lblName.Font" ) ) );
			this.lblName.Image = ( ( System.Drawing.Image )( resources.GetObject( "lblName.Image" ) ) );
			this.lblName.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblName.ImageAlign" ) ) );
			this.lblName.ImageIndex = ( ( int )( resources.GetObject( "lblName.ImageIndex" ) ) );
			this.lblName.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "lblName.ImeMode" ) ) );
			this.lblName.Location = ( ( System.Drawing.Point )( resources.GetObject( "lblName.Location" ) ) );
			this.lblName.Name = "lblName";
			this.lblName.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "lblName.RightToLeft" ) ) );
			this.lblName.Size = ( ( System.Drawing.Size )( resources.GetObject( "lblName.Size" ) ) );
			this.lblName.TabIndex = ( ( int )( resources.GetObject( "lblName.TabIndex" ) ) );
			this.lblName.Text = resources.GetString( "lblName.Text" );
			this.lblName.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblName.TextAlign" ) ) );
			this.lblName.Visible = ( ( bool )( resources.GetObject( "lblName.Visible" ) ) );
			// 
			// lblSettings
			// 
			this.lblSettings.AccessibleDescription = resources.GetString( "lblSettings.AccessibleDescription" );
			this.lblSettings.AccessibleName = resources.GetString( "lblSettings.AccessibleName" );
			this.lblSettings.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "lblSettings.Anchor" ) ) );
			this.lblSettings.AutoSize = ( ( bool )( resources.GetObject( "lblSettings.AutoSize" ) ) );
			this.lblSettings.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "lblSettings.Dock" ) ) );
			this.lblSettings.Enabled = ( ( bool )( resources.GetObject( "lblSettings.Enabled" ) ) );
			this.lblSettings.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.lblSettings.Font = ( ( System.Drawing.Font )( resources.GetObject( "lblSettings.Font" ) ) );
			this.lblSettings.Image = ( ( System.Drawing.Image )( resources.GetObject( "lblSettings.Image" ) ) );
			this.lblSettings.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblSettings.ImageAlign" ) ) );
			this.lblSettings.ImageIndex = ( ( int )( resources.GetObject( "lblSettings.ImageIndex" ) ) );
			this.lblSettings.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "lblSettings.ImeMode" ) ) );
			this.lblSettings.Location = ( ( System.Drawing.Point )( resources.GetObject( "lblSettings.Location" ) ) );
			this.lblSettings.Name = "lblSettings";
			this.lblSettings.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "lblSettings.RightToLeft" ) ) );
			this.lblSettings.Size = ( ( System.Drawing.Size )( resources.GetObject( "lblSettings.Size" ) ) );
			this.lblSettings.TabIndex = ( ( int )( resources.GetObject( "lblSettings.TabIndex" ) ) );
			this.lblSettings.Text = resources.GetString( "lblSettings.Text" );
			this.lblSettings.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblSettings.TextAlign" ) ) );
			this.lblSettings.Visible = ( ( bool )( resources.GetObject( "lblSettings.Visible" ) ) );
			// 
			// txtName
			// 
			this.txtName.AccessibleDescription = resources.GetString( "txtName.AccessibleDescription" );
			this.txtName.AccessibleName = resources.GetString( "txtName.AccessibleName" );
			this.txtName.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "txtName.Anchor" ) ) );
			this.txtName.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "txtName.BackgroundImage" ) ) );
			this.txtName.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "txtName.Dock" ) ) );
			this.txtName.Enabled = ( ( bool )( resources.GetObject( "txtName.Enabled" ) ) );
			this.txtName.Font = ( ( System.Drawing.Font )( resources.GetObject( "txtName.Font" ) ) );
			this.txtName.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "txtName.ImeMode" ) ) );
			this.txtName.IntegralHeight = ( ( bool )( resources.GetObject( "txtName.IntegralHeight" ) ) );
			this.txtName.ItemHeight = ( ( int )( resources.GetObject( "txtName.ItemHeight" ) ) );
			this.txtName.Location = ( ( System.Drawing.Point )( resources.GetObject( "txtName.Location" ) ) );
			this.txtName.MaxDropDownItems = ( ( int )( resources.GetObject( "txtName.MaxDropDownItems" ) ) );
			this.txtName.MaxLength = ( ( int )( resources.GetObject( "txtName.MaxLength" ) ) );
			this.txtName.Name = "txtName";
			this.txtName.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "txtName.RightToLeft" ) ) );
			this.txtName.Size = ( ( System.Drawing.Size )( resources.GetObject( "txtName.Size" ) ) );
			this.txtName.TabIndex = ( ( int )( resources.GetObject( "txtName.TabIndex" ) ) );
			this.txtName.Text = resources.GetString( "txtName.Text" );
			this.txtName.Visible = ( ( bool )( resources.GetObject( "txtName.Visible" ) ) );
			this.txtName.SelectionChangeCommitted += new System.EventHandler( this.txtName_SelectionChangeCommitted );
			// 
			// ctrlFormat
			// 
			this.ctrlFormat.AccessibleDescription = resources.GetString( "ctrlFormat.AccessibleDescription" );
			this.ctrlFormat.AccessibleName = resources.GetString( "ctrlFormat.AccessibleName" );
			this.ctrlFormat.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "ctrlFormat.Anchor" ) ) );
			this.ctrlFormat.AutoScroll = ( ( bool )( resources.GetObject( "ctrlFormat.AutoScroll" ) ) );
			this.ctrlFormat.AutoScrollMargin = ( ( System.Drawing.Size )( resources.GetObject( "ctrlFormat.AutoScrollMargin" ) ) );
			this.ctrlFormat.AutoScrollMinSize = ( ( System.Drawing.Size )( resources.GetObject( "ctrlFormat.AutoScrollMinSize" ) ) );
			this.ctrlFormat.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "ctrlFormat.BackgroundImage" ) ) );
			this.ctrlFormat.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "ctrlFormat.Dock" ) ) );
			this.ctrlFormat.Enabled = ( ( bool )( resources.GetObject( "ctrlFormat.Enabled" ) ) );
			this.ctrlFormat.Font = ( ( System.Drawing.Font )( resources.GetObject( "ctrlFormat.Font" ) ) );
			this.ctrlFormat.FormatsSelector = null;
			this.ctrlFormat.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "ctrlFormat.ImeMode" ) ) );
			this.ctrlFormat.Location = ( ( System.Drawing.Point )( resources.GetObject( "ctrlFormat.Location" ) ) );
			this.ctrlFormat.Name = "ctrlFormat";
			this.ctrlFormat.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "ctrlFormat.RightToLeft" ) ) );
			this.ctrlFormat.Size = ( ( System.Drawing.Size )( resources.GetObject( "ctrlFormat.Size" ) ) );
			this.ctrlFormat.TabIndex = ( ( int )( resources.GetObject( "ctrlFormat.TabIndex" ) ) );
			this.ctrlFormat.Visible = ( ( bool )( resources.GetObject( "ctrlFormat.Visible" ) ) );
			// 
			// frmNewFormatDialog
			// 
			this.AcceptButton = this.btnOk;
			this.AccessibleDescription = resources.GetString( "$this.AccessibleDescription" );
			this.AccessibleName = resources.GetString( "$this.AccessibleName" );
			this.AutoScaleBaseSize = ( ( System.Drawing.Size )( resources.GetObject( "$this.AutoScaleBaseSize" ) ) );
			this.AutoScroll = ( ( bool )( resources.GetObject( "$this.AutoScroll" ) ) );
			this.AutoScrollMargin = ( ( System.Drawing.Size )( resources.GetObject( "$this.AutoScrollMargin" ) ) );
			this.AutoScrollMinSize = ( ( System.Drawing.Size )( resources.GetObject( "$this.AutoScrollMinSize" ) ) );
			this.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "$this.BackgroundImage" ) ) );
			this.CancelButton = this.btnCancel;
			this.ClientSize = ( ( System.Drawing.Size )( resources.GetObject( "$this.ClientSize" ) ) );
			this.Controls.Add( this.ctrlFormat );
			this.Controls.Add( this.txtName );
			this.Controls.Add( this.lblSettings );
			this.Controls.Add( this.lblName );
			this.Controls.Add( this.btnOk );
			this.Controls.Add( this.btnCancel );
			this.Enabled = ( ( bool )( resources.GetObject( "$this.Enabled" ) ) );
			this.Font = ( ( System.Drawing.Font )( resources.GetObject( "$this.Font" ) ) );
			this.Icon = ( ( System.Drawing.Icon )( resources.GetObject( "$this.Icon" ) ) );
			this.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "$this.ImeMode" ) ) );
			this.Location = ( ( System.Drawing.Point )( resources.GetObject( "$this.Location" ) ) );
			this.MaximumSize = ( ( System.Drawing.Size )( resources.GetObject( "$this.MaximumSize" ) ) );
			this.MinimumSize = ( ( System.Drawing.Size )( resources.GetObject( "$this.MinimumSize" ) ) );
			this.Name = "frmNewFormatDialog";
			this.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "$this.RightToLeft" ) ) );
			this.RightToLeftLayout = true;
			this.StartPosition = ( ( System.Windows.Forms.FormStartPosition )( resources.GetObject( "$this.StartPosition" ) ) );
			this.Text = resources.GetString( "$this.Text" );
			this.ResumeLayout( false );
		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// Sets name of the format.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void txtName_TextChanged( object sender, System.EventArgs e )
		{
			m_format.Name = txtName.Text;
		}
		/// <summary>
		/// Changes format name.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void txtName_SelectionChangeCommitted( object sender, System.EventArgs e )
		{
			txtName.Text = ( string )txtName.SelectedItem;
			m_format.Name = txtName.Text;
		}
		#endregion
	}
}
