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
using Syncfusion.Windows.Forms.Edit.Implementation;
using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Implementation.Formatting;
using Syncfusion.Windows.Forms.Edit.Implementation.Config;
using Syncfusion.Windows.Forms.Localization;


namespace Syncfusion.Windows.Forms.Edit.Dialogs
{
	/// <summary>
	/// List of formats of EditControl.
	/// </summary>
	[ToolboxItem( false )]
	public class ControlFormatsList
		: BaseControlEditControlConfigurator
	{
		#region Controls
		private System.Windows.Forms.ListBox lstFormats;
		private System.Windows.Forms.Panel pnlBottom;
		private System.Windows.Forms.Button btnRemove;
		private System.Windows.Forms.Button btnAdd;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		#endregion

		#region Fields
		/// <summary>
		/// Currently edited configuration language.
		/// </summary>
		private IConfigLanguage m_Language;
		/// <summary>
		/// Language selector.
		/// </summary>
		private ControlLanguageSelector m_languageSelector;
		/// <summary>
		/// Specifies whether Add/Remove buttons should be visible.
		/// </summary>
		private bool m_bCanAddRemove = true;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets selected language.
		/// </summary>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public IConfigLanguage Language
		{
			get
			{
				if( LanguageSelector != null )
				{
					return LanguageSelector.SelectedLanguage;
				}

				return m_Language;
			}
			set
			{
				if( m_Language != value )
				{
					m_Language = value;

					OnLanguageChanged();
				}
			}
		}
		/// <summary>
		/// Gets or sets currently selected format.
		/// </summary>
		[Browsable( false )]
		public ISnippetFormat[] SelectedFormats
		{
			get
			{
				if( lstFormats.SelectedIndices.Count == 0 ) return null;

				ISnippetFormat[] result = new ISnippetFormat[ lstFormats.SelectedIndices.Count ];

				for( int i = 0; i < lstFormats.SelectedIndices.Count; i++ )
				{
					result[ i ] = ( ISnippetFormat )Language[ lstFormats.SelectedIndices[ i ] ];
				}

				return result;
			}
			set
			{
				lstFormats.BeginUpdate();
				lstFormats.ClearSelected();

				if( value != null )
				{
					foreach( ISnippetFormat format in value )
					{
						int index = Language.KnownFormats.IndexOf( format );
						lstFormats.SetSelected( index, true );
					}
				}

				lstFormats.EndUpdate();
			}
		}
		/// <summary>
		/// Gets or sets language selector control.
		/// </summary>
		[Browsable( true )]
		[TypeConverter( typeof( ComponentConverter ) )]
		[Category( "Data" )]
		public ControlLanguageSelector LanguageSelector
		{
			get
			{
				return m_languageSelector;
			}
			set
			{
				if( m_languageSelector != value )
				{
					if( m_languageSelector != null )
						m_languageSelector.SelectedLanguageChanged -= new EventHandler( LanguageSelectorLanguageChanged );

					m_languageSelector = value;

					if( m_languageSelector != null )
						m_languageSelector.SelectedLanguageChanged += new EventHandler( LanguageSelectorLanguageChanged );
				}
			}
		}
		/// <summary>
		/// Gets or sets value that indicates whether Add and Remove 
		/// buttons should be visible.
		/// </summary>
		[Browsable( true )]
		[Category( "Appearance" )]
		[DefaultValue( true )]
		public bool ShowAddRemoveButtons
		{
			get
			{
				return m_bCanAddRemove;
			}
			set
			{
				if( m_bCanAddRemove != value )
				{
					m_bCanAddRemove = value;
					pnlBottom.Visible = m_bCanAddRemove;
				}
			}
		}
		#endregion

		#region Events
		/// <summary>
		/// Event that is raised when user selects some language.
		/// </summary>
		[Category( "Data" )]
		public event EventHandler LanguageChanged;
		/// <summary>
		/// Event that is raised when format selection has changed.
		/// </summary>
		[Category( "Data" )]
		public event EventHandler SelectedFormatChanged;
		#endregion

		#region Initialization/Finalization
		/// <summary>
		/// Creates and initializes new instance of the class.
		/// </summary>
		public ControlFormatsList()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			// TODO: Add any initialization after the InitializeComponent call
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager( typeof( ControlFormatsList ) );
			this.lstFormats = new System.Windows.Forms.ListBox();
			this.pnlBottom = new System.Windows.Forms.Panel();
			this.btnAdd = new System.Windows.Forms.Button();
			this.btnRemove = new System.Windows.Forms.Button();
			this.pnlBottom.SuspendLayout();
			this.SuspendLayout();
			// 
			// lstFormats
			// 
			this.lstFormats.AccessibleDescription = resources.GetString( "lstFormats.AccessibleDescription" );
			this.lstFormats.AccessibleName = resources.GetString( "lstFormats.AccessibleName" );
			this.lstFormats.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "lstFormats.Anchor" ) ) );
			this.lstFormats.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "lstFormats.BackgroundImage" ) ) );
			this.lstFormats.ColumnWidth = ( ( int )( resources.GetObject( "lstFormats.ColumnWidth" ) ) );
			this.lstFormats.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "lstFormats.Dock" ) ) );
			this.lstFormats.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.lstFormats.Enabled = ( ( bool )( resources.GetObject( "lstFormats.Enabled" ) ) );
			this.lstFormats.Font = ( ( System.Drawing.Font )( resources.GetObject( "lstFormats.Font" ) ) );
			this.lstFormats.HorizontalExtent = ( ( int )( resources.GetObject( "lstFormats.HorizontalExtent" ) ) );
			this.lstFormats.HorizontalScrollbar = ( ( bool )( resources.GetObject( "lstFormats.HorizontalScrollbar" ) ) );
			this.lstFormats.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "lstFormats.ImeMode" ) ) );
			this.lstFormats.IntegralHeight = ( ( bool )( resources.GetObject( "lstFormats.IntegralHeight" ) ) );
			this.lstFormats.ItemHeight = ( ( int )( resources.GetObject( "lstFormats.ItemHeight" ) ) );
			this.lstFormats.Items.AddRange( new object[] {
                                                    resources.GetString("lstFormats.Items"),
                                                    resources.GetString("lstFormats.Items1")} );
			this.lstFormats.Location = ( ( System.Drawing.Point )( resources.GetObject( "lstFormats.Location" ) ) );
			this.lstFormats.Name = "lstFormats";
			this.lstFormats.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "lstFormats.RightToLeft" ) ) );
			this.lstFormats.ScrollAlwaysVisible = ( ( bool )( resources.GetObject( "lstFormats.ScrollAlwaysVisible" ) ) );
			this.lstFormats.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.lstFormats.Size = ( ( System.Drawing.Size )( resources.GetObject( "lstFormats.Size" ) ) );
			this.lstFormats.TabIndex = ( ( int )( resources.GetObject( "lstFormats.TabIndex" ) ) );
			this.lstFormats.Visible = ( ( bool )( resources.GetObject( "lstFormats.Visible" ) ) );
			this.lstFormats.DrawItem += new System.Windows.Forms.DrawItemEventHandler( this.lstFormats_DrawItem );
			this.lstFormats.SelectedIndexChanged += new System.EventHandler( this.lstFormats_SelectedIndexChanged );
			// 
			// pnlBottom
			// 
			this.pnlBottom.AccessibleDescription = resources.GetString( "pnlBottom.AccessibleDescription" );
			this.pnlBottom.AccessibleName = resources.GetString( "pnlBottom.AccessibleName" );
			this.pnlBottom.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "pnlBottom.Anchor" ) ) );
			this.pnlBottom.AutoScroll = ( ( bool )( resources.GetObject( "pnlBottom.AutoScroll" ) ) );
			this.pnlBottom.AutoScrollMargin = ( ( System.Drawing.Size )( resources.GetObject( "pnlBottom.AutoScrollMargin" ) ) );
			this.pnlBottom.AutoScrollMinSize = ( ( System.Drawing.Size )( resources.GetObject( "pnlBottom.AutoScrollMinSize" ) ) );
			this.pnlBottom.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "pnlBottom.BackgroundImage" ) ) );
			this.pnlBottom.Controls.Add( this.btnAdd );
			this.pnlBottom.Controls.Add( this.btnRemove );
			this.pnlBottom.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "pnlBottom.Dock" ) ) );
			this.pnlBottom.Enabled = ( ( bool )( resources.GetObject( "pnlBottom.Enabled" ) ) );
			this.pnlBottom.Font = ( ( System.Drawing.Font )( resources.GetObject( "pnlBottom.Font" ) ) );
			this.pnlBottom.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "pnlBottom.ImeMode" ) ) );
			this.pnlBottom.Location = ( ( System.Drawing.Point )( resources.GetObject( "pnlBottom.Location" ) ) );
			this.pnlBottom.Name = "pnlBottom";
			this.pnlBottom.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "pnlBottom.RightToLeft" ) ) );
			this.pnlBottom.Size = ( ( System.Drawing.Size )( resources.GetObject( "pnlBottom.Size" ) ) );
			this.pnlBottom.TabIndex = ( ( int )( resources.GetObject( "pnlBottom.TabIndex" ) ) );
			this.pnlBottom.Text = resources.GetString( "pnlBottom.Text" );
			this.pnlBottom.Visible = ( ( bool )( resources.GetObject( "pnlBottom.Visible" ) ) );
			// 
			// btnAdd
			// 
			this.btnAdd.AccessibleDescription = resources.GetString( "btnAdd.AccessibleDescription" );
			this.btnAdd.AccessibleName = resources.GetString( "btnAdd.AccessibleName" );
			this.btnAdd.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "btnAdd.Anchor" ) ) );
			this.btnAdd.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "btnAdd.BackgroundImage" ) ) );
			this.btnAdd.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "btnAdd.Dock" ) ) );
			this.btnAdd.Enabled = ( ( bool )( resources.GetObject( "btnAdd.Enabled" ) ) );
			this.btnAdd.FlatStyle = ( ( System.Windows.Forms.FlatStyle )( resources.GetObject( "btnAdd.FlatStyle" ) ) );
			this.btnAdd.Font = ( ( System.Drawing.Font )( resources.GetObject( "btnAdd.Font" ) ) );
			this.btnAdd.Image = ( ( System.Drawing.Image )( resources.GetObject( "btnAdd.Image" ) ) );
			this.btnAdd.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnAdd.ImageAlign" ) ) );
			this.btnAdd.ImageIndex = ( ( int )( resources.GetObject( "btnAdd.ImageIndex" ) ) );
			this.btnAdd.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "btnAdd.ImeMode" ) ) );
			this.btnAdd.Location = ( ( System.Drawing.Point )( resources.GetObject( "btnAdd.Location" ) ) );
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "btnAdd.RightToLeft" ) ) );
			this.btnAdd.Size = ( ( System.Drawing.Size )( resources.GetObject( "btnAdd.Size" ) ) );
			this.btnAdd.TabIndex = ( ( int )( resources.GetObject( "btnAdd.TabIndex" ) ) );
			this.btnAdd.Text = resources.GetString( "btnAdd.Text" );
			this.btnAdd.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnAdd.TextAlign" ) ) );
			this.btnAdd.Visible = ( ( bool )( resources.GetObject( "btnAdd.Visible" ) ) );
			this.btnAdd.Click += new System.EventHandler( this.btnAdd_Click );
			// 
			// btnRemove
			// 
			this.btnRemove.AccessibleDescription = resources.GetString( "btnRemove.AccessibleDescription" );
			this.btnRemove.AccessibleName = resources.GetString( "btnRemove.AccessibleName" );
			this.btnRemove.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "btnRemove.Anchor" ) ) );
			this.btnRemove.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "btnRemove.BackgroundImage" ) ) );
			this.btnRemove.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "btnRemove.Dock" ) ) );
			this.btnRemove.Enabled = ( ( bool )( resources.GetObject( "btnRemove.Enabled" ) ) );
			this.btnRemove.FlatStyle = ( ( System.Windows.Forms.FlatStyle )( resources.GetObject( "btnRemove.FlatStyle" ) ) );
			this.btnRemove.Font = ( ( System.Drawing.Font )( resources.GetObject( "btnRemove.Font" ) ) );
			this.btnRemove.Image = ( ( System.Drawing.Image )( resources.GetObject( "btnRemove.Image" ) ) );
			this.btnRemove.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnRemove.ImageAlign" ) ) );
			this.btnRemove.ImageIndex = ( ( int )( resources.GetObject( "btnRemove.ImageIndex" ) ) );
			this.btnRemove.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "btnRemove.ImeMode" ) ) );
			this.btnRemove.Location = ( ( System.Drawing.Point )( resources.GetObject( "btnRemove.Location" ) ) );
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "btnRemove.RightToLeft" ) ) );
			this.btnRemove.Size = ( ( System.Drawing.Size )( resources.GetObject( "btnRemove.Size" ) ) );
			this.btnRemove.TabIndex = ( ( int )( resources.GetObject( "btnRemove.TabIndex" ) ) );
			this.btnRemove.Text = resources.GetString( "btnRemove.Text" );
			this.btnRemove.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnRemove.TextAlign" ) ) );
			this.btnRemove.Visible = ( ( bool )( resources.GetObject( "btnRemove.Visible" ) ) );
			this.btnRemove.Click += new System.EventHandler( this.btnRemove_Click );
			// 
			// ControlFormatsList
			// 
			this.AccessibleDescription = resources.GetString( "$this.AccessibleDescription" );
			this.AccessibleName = resources.GetString( "$this.AccessibleName" );
			this.AutoScroll = ( ( bool )( resources.GetObject( "$this.AutoScroll" ) ) );
			this.AutoScrollMargin = ( ( System.Drawing.Size )( resources.GetObject( "$this.AutoScrollMargin" ) ) );
			this.AutoScrollMinSize = ( ( System.Drawing.Size )( resources.GetObject( "$this.AutoScrollMinSize" ) ) );
			this.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "$this.BackgroundImage" ) ) );
			this.Controls.Add( this.lstFormats );
			this.Controls.Add( this.pnlBottom );
			this.Enabled = ( ( bool )( resources.GetObject( "$this.Enabled" ) ) );
			this.Font = ( ( System.Drawing.Font )( resources.GetObject( "$this.Font" ) ) );
			this.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "$this.ImeMode" ) ) );
			this.Location = ( ( System.Drawing.Point )( resources.GetObject( "$this.Location" ) ) );
			this.Name = "ControlFormatsList";
			this.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "$this.RightToLeft" ) ) );
			this.Size = ( ( System.Drawing.Size )( resources.GetObject( "$this.Size" ) ) );
			this.pnlBottom.ResumeLayout( false );
			this.ResumeLayout( false );
		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// Draws item.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lstFormats_DrawItem( object sender, System.Windows.Forms.DrawItemEventArgs e )
		{
			if( e.Index == -1 ) return;

			e.DrawBackground();

			if( ( e.State & DrawItemState.Selected ) == DrawItemState.Selected )
			{
				e.Graphics.DrawString( lstFormats.Items[ e.Index ].ToString(), Font, SystemBrushes.Window, e.Bounds );
			}
			else
			{
				e.Graphics.DrawString( lstFormats.Items[ e.Index ].ToString(), Font, SystemBrushes.WindowText, e.Bounds );
			}

			if( ( e.State & DrawItemState.Focus ) == DrawItemState.Focus )
			{
				e.DrawFocusRectangle();
			}
		}
		/// <summary>
		/// Calls OnFormatSelectionChanged method. 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lstFormats_SelectedIndexChanged( object sender, System.EventArgs e )
		{
			OnFormatSelectionChanged();
		}
		/// <summary>
		/// Removes selected format from collection.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnRemove_Click( object sender, System.EventArgs e )
		{
			if( Language != null && SelectedFormats != null && SelectedFormats.Length > 0 )
			{
				foreach( ISnippetFormat format in SelectedFormats )
				{
					Language.Remove( format );
				}

				Language.ResetCaches();
			}
		}
		/// <summary>
		/// Updates formats list.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void EditControl_ConfigurationChanged( object sender, EventArgs e )
		{
			OnLanguageChanged();
		}
		/// <summary>
		/// Adds new format to language.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnAdd_Click( object sender, System.EventArgs e )
		{
			if( Language == null ) return;

			frmNewFormatDialog formNew = new frmNewFormatDialog();

			DialogResult result = formNew.ShowDialog();

			if( result != DialogResult.OK ) return;

			if( formNew.NewFormat == null )
				throw new NullReferenceException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_13 );

			FormatManager formats = ( FormatManager )Language;
			formats.Add( formNew.NewFormat );
			Language.ResetCaches();
		}

		#endregion

		#region Virtual Methods
		/// <summary>
		/// Invalidates control.
		/// </summary>
		protected override void OnEditControlChanged()
		{
			base.OnEditControlChanged();

			Configuration.ConfigurationChanged += new EventHandler( EditControl_ConfigurationChanged );
			Configuration.FormatsChanged += new EventHandler( EditControl_ConfigurationChanged );

			Language = ( Configuration.KnownLanguages.Count > 0 )
				? ( IConfigLanguage )Configuration.KnownLanguages[ 0 ]
				: null;

			Invalidate();
		}
		/// <summary>
		/// Raises LanguageChanged event.
		/// </summary>
		protected virtual void OnLanguageChanged()
		{
			ArrayList selection = new ArrayList( lstFormats.SelectedItems );

			lstFormats.BeginUpdate();
			lstFormats.Items.Clear();

			try
			{
				if( Language != null )
				{
					foreach( ISnippetFormat format in ( IEnumerable )Language )
					{
						lstFormats.Items.Add( format.Name );
					}
				}

				if( LanguageChanged != null )
				{
					LanguageChanged( this, EventArgs.Empty );
				}

				lstFormats.SelectedIndex = -1;

				for( int i = 0; i < lstFormats.Items.Count; i++ )
				{
					string formatName = lstFormats.Items[ i ].ToString();

					if( selection.Contains( formatName ) )
						lstFormats.SetSelected( i, true );
				}
			}
			finally
			{
				lstFormats.EndUpdate();
			}
		}
		/// <summary>
		/// Raises SelectedFormatChanged event.
		/// </summary>
		protected virtual void OnFormatSelectionChanged()
		{
			btnRemove.Enabled = ( Language != null ) && ( SelectedFormats != null && SelectedFormats.Length > 0 );
			btnAdd.Enabled = ( Language != null );

			if( SelectedFormatChanged != null )
			{
				SelectedFormatChanged( this, EventArgs.Empty );
			}
		}
		/// <summary>
		/// Calls OnFormatSelectionChanged() method.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LanguageSelectorLanguageChanged( object sender, EventArgs e )
		{
			OnLanguageChanged();
		}
		#endregion
	}
}
