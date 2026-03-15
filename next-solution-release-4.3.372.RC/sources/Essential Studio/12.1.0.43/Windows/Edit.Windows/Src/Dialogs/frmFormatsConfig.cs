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
using System.IO;

using Syncfusion.Windows.Forms.Edit.Implementation.Config;

namespace Syncfusion.Windows.Forms.Edit.Dialogs
{
	/// <summary>
	/// Configuration for formats.
	/// </summary>
	public class frmFormatsConfig
		: System.Windows.Forms.Form
	{
		#region Fields
		/// <summary>
		/// Edit control with configurations.
		/// </summary>
		private EditControl m_editcontrol;
		/// <summary>
		/// Currently edited configuration.
		/// </summary>
		private Config m_config;
		#endregion

		#region Form Controls
		private Syncfusion.Windows.Forms.Edit.Dialogs.ControlLanguageSelector controlLanguageSelector1;
		private Syncfusion.Windows.Forms.Edit.Dialogs.ControlFormatsSettings controlFormatsSettings1;
		private Syncfusion.Windows.Forms.Edit.Dialogs.ControlFormatsList controlFormatsList1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnApply;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		#endregion

		#region Properties
		/// <summary>
		/// Editcontrol, dialog is attached to.
		/// </summary>
		public EditControl EditControl
		{
			get
			{
				return m_editcontrol;
			}
			set
			{
				if( m_editcontrol != value )
				{
					m_editcontrol = value;

					if( value != null )
					{
						LoadConfiguration( m_editcontrol.Configurator );
						controlLanguageSelector1.SelectedLanguage = m_editcontrol.Language;
					}
					else
					{
						controlLanguageSelector1.EditControl =
							controlFormatsList1.EditControl = null;
					}
				}
			}
		}
		#endregion

		#region Initialization/Finalization
		/// <summary>
		/// Creates and initializes form.
		/// </summary>
		public frmFormatsConfig()
		{
			InitializeComponent();
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager( typeof( frmFormatsConfig ) );
			this.controlLanguageSelector1 = new Syncfusion.Windows.Forms.Edit.Dialogs.ControlLanguageSelector();
			this.controlFormatsSettings1 = new Syncfusion.Windows.Forms.Edit.Dialogs.ControlFormatsSettings();
			this.controlFormatsList1 = new Syncfusion.Windows.Forms.Edit.Dialogs.ControlFormatsList();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnApply = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// controlLanguageSelector1
			// 
			this.controlLanguageSelector1.AccessibleDescription = resources.GetString( "controlLanguageSelector1.AccessibleDescription" );
			this.controlLanguageSelector1.AccessibleName = resources.GetString( "controlLanguageSelector1.AccessibleName" );
			this.controlLanguageSelector1.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "controlLanguageSelector1.Anchor" ) ) );
			this.controlLanguageSelector1.AutoScroll = ( ( bool )( resources.GetObject( "controlLanguageSelector1.AutoScroll" ) ) );
			this.controlLanguageSelector1.AutoScrollMargin = ( ( System.Drawing.Size )( resources.GetObject( "controlLanguageSelector1.AutoScrollMargin" ) ) );
			this.controlLanguageSelector1.AutoScrollMinSize = ( ( System.Drawing.Size )( resources.GetObject( "controlLanguageSelector1.AutoScrollMinSize" ) ) );
			this.controlLanguageSelector1.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "controlLanguageSelector1.BackgroundImage" ) ) );
			this.controlLanguageSelector1.Configuration = null;
			this.controlLanguageSelector1.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "controlLanguageSelector1.Dock" ) ) );
			this.controlLanguageSelector1.EditControl = null;
			this.controlLanguageSelector1.Enabled = ( ( bool )( resources.GetObject( "controlLanguageSelector1.Enabled" ) ) );
			this.controlLanguageSelector1.Font = ( ( System.Drawing.Font )( resources.GetObject( "controlLanguageSelector1.Font" ) ) );
			this.controlLanguageSelector1.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "controlLanguageSelector1.ImeMode" ) ) );
			this.controlLanguageSelector1.Location = ( ( System.Drawing.Point )( resources.GetObject( "controlLanguageSelector1.Location" ) ) );
			this.controlLanguageSelector1.Name = "controlLanguageSelector1";
			this.controlLanguageSelector1.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "controlLanguageSelector1.RightToLeft" ) ) );
			this.controlLanguageSelector1.Size = ( ( System.Drawing.Size )( resources.GetObject( "controlLanguageSelector1.Size" ) ) );
			this.controlLanguageSelector1.TabIndex = ( ( int )( resources.GetObject( "controlLanguageSelector1.TabIndex" ) ) );
			this.controlLanguageSelector1.Visible = ( ( bool )( resources.GetObject( "controlLanguageSelector1.Visible" ) ) );
			// 
			// controlFormatsSettings1
			// 
			this.controlFormatsSettings1.AccessibleDescription = resources.GetString( "controlFormatsSettings1.AccessibleDescription" );
			this.controlFormatsSettings1.AccessibleName = resources.GetString( "controlFormatsSettings1.AccessibleName" );
			this.controlFormatsSettings1.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "controlFormatsSettings1.Anchor" ) ) );
			this.controlFormatsSettings1.AutoScroll = ( ( bool )( resources.GetObject( "controlFormatsSettings1.AutoScroll" ) ) );
			this.controlFormatsSettings1.AutoScrollMargin = ( ( System.Drawing.Size )( resources.GetObject( "controlFormatsSettings1.AutoScrollMargin" ) ) );
			this.controlFormatsSettings1.AutoScrollMinSize = ( ( System.Drawing.Size )( resources.GetObject( "controlFormatsSettings1.AutoScrollMinSize" ) ) );
			this.controlFormatsSettings1.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "controlFormatsSettings1.BackgroundImage" ) ) );
			this.controlFormatsSettings1.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "controlFormatsSettings1.Dock" ) ) );
			this.controlFormatsSettings1.Enabled = ( ( bool )( resources.GetObject( "controlFormatsSettings1.Enabled" ) ) );
			this.controlFormatsSettings1.Font = ( ( System.Drawing.Font )( resources.GetObject( "controlFormatsSettings1.Font" ) ) );
			this.controlFormatsSettings1.Formats = null;
			this.controlFormatsSettings1.FormatsSelector = this.controlFormatsList1;
			this.controlFormatsSettings1.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "controlFormatsSettings1.ImeMode" ) ) );
			this.controlFormatsSettings1.Location = ( ( System.Drawing.Point )( resources.GetObject( "controlFormatsSettings1.Location" ) ) );
			this.controlFormatsSettings1.Name = "controlFormatsSettings1";
			this.controlFormatsSettings1.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "controlFormatsSettings1.RightToLeft" ) ) );
			this.controlFormatsSettings1.Size = ( ( System.Drawing.Size )( resources.GetObject( "controlFormatsSettings1.Size" ) ) );
			this.controlFormatsSettings1.TabIndex = ( ( int )( resources.GetObject( "controlFormatsSettings1.TabIndex" ) ) );
			this.controlFormatsSettings1.Visible = ( ( bool )( resources.GetObject( "controlFormatsSettings1.Visible" ) ) );
			// 
			// controlFormatsList1
			// 
			this.controlFormatsList1.AccessibleDescription = resources.GetString( "controlFormatsList1.AccessibleDescription" );
			this.controlFormatsList1.AccessibleName = resources.GetString( "controlFormatsList1.AccessibleName" );
			this.controlFormatsList1.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "controlFormatsList1.Anchor" ) ) );
			this.controlFormatsList1.AutoScroll = ( ( bool )( resources.GetObject( "controlFormatsList1.AutoScroll" ) ) );
			this.controlFormatsList1.AutoScrollMargin = ( ( System.Drawing.Size )( resources.GetObject( "controlFormatsList1.AutoScrollMargin" ) ) );
			this.controlFormatsList1.AutoScrollMinSize = ( ( System.Drawing.Size )( resources.GetObject( "controlFormatsList1.AutoScrollMinSize" ) ) );
			this.controlFormatsList1.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "controlFormatsList1.BackgroundImage" ) ) );
			this.controlFormatsList1.Configuration = null;
			this.controlFormatsList1.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "controlFormatsList1.Dock" ) ) );
			this.controlFormatsList1.EditControl = null;
			this.controlFormatsList1.Enabled = ( ( bool )( resources.GetObject( "controlFormatsList1.Enabled" ) ) );
			this.controlFormatsList1.Font = ( ( System.Drawing.Font )( resources.GetObject( "controlFormatsList1.Font" ) ) );
			this.controlFormatsList1.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "controlFormatsList1.ImeMode" ) ) );
			this.controlFormatsList1.LanguageSelector = this.controlLanguageSelector1;
			this.controlFormatsList1.Location = ( ( System.Drawing.Point )( resources.GetObject( "controlFormatsList1.Location" ) ) );
			this.controlFormatsList1.Name = "controlFormatsList1";
			this.controlFormatsList1.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "controlFormatsList1.RightToLeft" ) ) );
			this.controlFormatsList1.SelectedFormats = null;
			this.controlFormatsList1.ShowAddRemoveButtons = false;
			this.controlFormatsList1.Size = ( ( System.Drawing.Size )( resources.GetObject( "controlFormatsList1.Size" ) ) );
			this.controlFormatsList1.TabIndex = ( ( int )( resources.GetObject( "controlFormatsList1.TabIndex" ) ) );
			this.controlFormatsList1.Visible = ( ( bool )( resources.GetObject( "controlFormatsList1.Visible" ) ) );
			// 
			// label1
			// 
			this.label1.AccessibleDescription = resources.GetString( "label1.AccessibleDescription" );
			this.label1.AccessibleName = resources.GetString( "label1.AccessibleName" );
			this.label1.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "label1.Anchor" ) ) );
			this.label1.AutoSize = ( ( bool )( resources.GetObject( "label1.AutoSize" ) ) );
			this.label1.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "label1.Dock" ) ) );
			this.label1.Enabled = ( ( bool )( resources.GetObject( "label1.Enabled" ) ) );
			this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label1.Font = ( ( System.Drawing.Font )( resources.GetObject( "label1.Font" ) ) );
			this.label1.Image = ( ( System.Drawing.Image )( resources.GetObject( "label1.Image" ) ) );
			this.label1.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "label1.ImageAlign" ) ) );
			this.label1.ImageIndex = ( ( int )( resources.GetObject( "label1.ImageIndex" ) ) );
			this.label1.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "label1.ImeMode" ) ) );
			this.label1.Location = ( ( System.Drawing.Point )( resources.GetObject( "label1.Location" ) ) );
			this.label1.Name = "label1";
			this.label1.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "label1.RightToLeft" ) ) );
			this.label1.Size = ( ( System.Drawing.Size )( resources.GetObject( "label1.Size" ) ) );
			this.label1.TabIndex = ( ( int )( resources.GetObject( "label1.TabIndex" ) ) );
			this.label1.Text = resources.GetString( "label1.Text" );
			this.label1.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "label1.TextAlign" ) ) );
			this.label1.Visible = ( ( bool )( resources.GetObject( "label1.Visible" ) ) );
			// 
			// label2
			// 
			this.label2.AccessibleDescription = resources.GetString( "label2.AccessibleDescription" );
			this.label2.AccessibleName = resources.GetString( "label2.AccessibleName" );
			this.label2.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "label2.Anchor" ) ) );
			this.label2.AutoSize = ( ( bool )( resources.GetObject( "label2.AutoSize" ) ) );
			this.label2.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "label2.Dock" ) ) );
			this.label2.Enabled = ( ( bool )( resources.GetObject( "label2.Enabled" ) ) );
			this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label2.Font = ( ( System.Drawing.Font )( resources.GetObject( "label2.Font" ) ) );
			this.label2.Image = ( ( System.Drawing.Image )( resources.GetObject( "label2.Image" ) ) );
			this.label2.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "label2.ImageAlign" ) ) );
			this.label2.ImageIndex = ( ( int )( resources.GetObject( "label2.ImageIndex" ) ) );
			this.label2.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "label2.ImeMode" ) ) );
			this.label2.Location = ( ( System.Drawing.Point )( resources.GetObject( "label2.Location" ) ) );
			this.label2.Name = "label2";
			this.label2.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "label2.RightToLeft" ) ) );
			this.label2.Size = ( ( System.Drawing.Size )( resources.GetObject( "label2.Size" ) ) );
			this.label2.TabIndex = ( ( int )( resources.GetObject( "label2.TabIndex" ) ) );
			this.label2.Text = resources.GetString( "label2.Text" );
			this.label2.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "label2.TextAlign" ) ) );
			this.label2.Visible = ( ( bool )( resources.GetObject( "label2.Visible" ) ) );
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
			// btnOK
			// 
			this.btnOK.AccessibleDescription = resources.GetString( "btnOK.AccessibleDescription" );
			this.btnOK.AccessibleName = resources.GetString( "btnOK.AccessibleName" );
			this.btnOK.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "btnOK.Anchor" ) ) );
			this.btnOK.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "btnOK.BackgroundImage" ) ) );
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "btnOK.Dock" ) ) );
			this.btnOK.Enabled = ( ( bool )( resources.GetObject( "btnOK.Enabled" ) ) );
			this.btnOK.FlatStyle = ( ( System.Windows.Forms.FlatStyle )( resources.GetObject( "btnOK.FlatStyle" ) ) );
			this.btnOK.Font = ( ( System.Drawing.Font )( resources.GetObject( "btnOK.Font" ) ) );
			this.btnOK.Image = ( ( System.Drawing.Image )( resources.GetObject( "btnOK.Image" ) ) );
			this.btnOK.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnOK.ImageAlign" ) ) );
			this.btnOK.ImageIndex = ( ( int )( resources.GetObject( "btnOK.ImageIndex" ) ) );
			this.btnOK.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "btnOK.ImeMode" ) ) );
			this.btnOK.Location = ( ( System.Drawing.Point )( resources.GetObject( "btnOK.Location" ) ) );
			this.btnOK.Name = "btnOK";
			this.btnOK.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "btnOK.RightToLeft" ) ) );
			this.btnOK.Size = ( ( System.Drawing.Size )( resources.GetObject( "btnOK.Size" ) ) );
			this.btnOK.TabIndex = ( ( int )( resources.GetObject( "btnOK.TabIndex" ) ) );
			this.btnOK.Text = resources.GetString( "btnOK.Text" );
			this.btnOK.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnOK.TextAlign" ) ) );
			this.btnOK.Visible = ( ( bool )( resources.GetObject( "btnOK.Visible" ) ) );
			this.btnOK.Click += new System.EventHandler( this.ApplyClick );
			// 
			// btnApply
			// 
			this.btnApply.AccessibleDescription = resources.GetString( "btnApply.AccessibleDescription" );
			this.btnApply.AccessibleName = resources.GetString( "btnApply.AccessibleName" );
			this.btnApply.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "btnApply.Anchor" ) ) );
			this.btnApply.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "btnApply.BackgroundImage" ) ) );
			this.btnApply.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "btnApply.Dock" ) ) );
			this.btnApply.Enabled = ( ( bool )( resources.GetObject( "btnApply.Enabled" ) ) );
			this.btnApply.FlatStyle = ( ( System.Windows.Forms.FlatStyle )( resources.GetObject( "btnApply.FlatStyle" ) ) );
			this.btnApply.Font = ( ( System.Drawing.Font )( resources.GetObject( "btnApply.Font" ) ) );
			this.btnApply.Image = ( ( System.Drawing.Image )( resources.GetObject( "btnApply.Image" ) ) );
			this.btnApply.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnApply.ImageAlign" ) ) );
			this.btnApply.ImageIndex = ( ( int )( resources.GetObject( "btnApply.ImageIndex" ) ) );
			this.btnApply.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "btnApply.ImeMode" ) ) );
			this.btnApply.Location = ( ( System.Drawing.Point )( resources.GetObject( "btnApply.Location" ) ) );
			this.btnApply.Name = "btnApply";
			this.btnApply.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "btnApply.RightToLeft" ) ) );
			this.btnApply.Size = ( ( System.Drawing.Size )( resources.GetObject( "btnApply.Size" ) ) );
			this.btnApply.TabIndex = ( ( int )( resources.GetObject( "btnApply.TabIndex" ) ) );
			this.btnApply.Text = resources.GetString( "btnApply.Text" );
			this.btnApply.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnApply.TextAlign" ) ) );
			this.btnApply.Visible = ( ( bool )( resources.GetObject( "btnApply.Visible" ) ) );
			this.btnApply.Click += new System.EventHandler( this.ApplyClick );
			// 
			// frmFormatsConfig
			// 
			this.AcceptButton = this.btnOK;
			this.AccessibleDescription = resources.GetString( "$this.AccessibleDescription" );
			this.AccessibleName = resources.GetString( "$this.AccessibleName" );
			this.AutoScaleBaseSize = ( ( System.Drawing.Size )( resources.GetObject( "$this.AutoScaleBaseSize" ) ) );
			this.AutoScroll = ( ( bool )( resources.GetObject( "$this.AutoScroll" ) ) );
			this.AutoScrollMargin = ( ( System.Drawing.Size )( resources.GetObject( "$this.AutoScrollMargin" ) ) );
			this.AutoScrollMinSize = ( ( System.Drawing.Size )( resources.GetObject( "$this.AutoScrollMinSize" ) ) );
			this.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "$this.BackgroundImage" ) ) );
			this.CancelButton = this.btnCancel;
			this.ClientSize = ( ( System.Drawing.Size )( resources.GetObject( "$this.ClientSize" ) ) );
			this.Controls.Add( this.btnApply );
			this.Controls.Add( this.btnOK );
			this.Controls.Add( this.btnCancel );
			this.Controls.Add( this.label2 );
			this.Controls.Add( this.label1 );
			this.Controls.Add( this.controlFormatsList1 );
			this.Controls.Add( this.controlFormatsSettings1 );
			this.Controls.Add( this.controlLanguageSelector1 );
			this.Enabled = ( ( bool )( resources.GetObject( "$this.Enabled" ) ) );
			this.Font = ( ( System.Drawing.Font )( resources.GetObject( "$this.Font" ) ) );
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ( ( System.Drawing.Icon )( resources.GetObject( "$this.Icon" ) ) );
			this.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "$this.ImeMode" ) ) );
			this.Location = ( ( System.Drawing.Point )( resources.GetObject( "$this.Location" ) ) );
			this.MaximumSize = ( ( System.Drawing.Size )( resources.GetObject( "$this.MaximumSize" ) ) );
			this.MinimumSize = ( ( System.Drawing.Size )( resources.GetObject( "$this.MinimumSize" ) ) );
			this.Name = "frmFormatsConfig";
			this.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "$this.RightToLeft" ) ) );
			this.RightToLeftLayout = true;
			this.StartPosition = ( ( System.Windows.Forms.FormStartPosition )( resources.GetObject( "$this.StartPosition" ) ) );
			this.Text = resources.GetString( "$this.Text" );
			this.ResumeLayout( false );

		}
		#endregion

		#region Helper Methods
		/// <summary>
		/// Creates copy of the configuration.
		/// </summary>
		/// <param name="config">Configuration to clone.</param>
		/// <returns>Cloned copy.</returns>
		protected Config CreateConfigClone( Config config )
		{
			if( config == null )
				throw new ArgumentNullException( "config" );

			MemoryStream stream = new MemoryStream();
			config.Save( stream );
			stream.Position = 0;

			Config configResult = new Config();
			configResult.Open( stream );

			return configResult;
		}
		/// <summary>
		/// Creates copy of the configuration and loads it for editing.
		/// </summary>
		/// <param name="config">Configuration to load.</param>
		protected void LoadConfiguration( Config config )
		{
			m_config = CreateConfigClone( config );
			controlLanguageSelector1.Configuration = m_config;
			controlFormatsList1.Configuration = m_config;
		}
		/// <summary>
		/// Applies current configuration to edit control.
		/// </summary>
		protected void ApplyConfiguration()
		{
			Config config = CreateConfigClone( m_config );
			EditControl.Configurator = config;

			for( int i = 0; i < 5; i++ )
			{
				GC.WaitForPendingFinalizers();
				GC.Collect();
			}
		}
		/// <summary>
		/// Applies all changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ApplyClick( object sender, System.EventArgs e )
		{
			ApplyConfiguration();
		}
		#endregion
	}
}
