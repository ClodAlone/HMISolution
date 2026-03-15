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
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Collections;

namespace Syncfusion.Windows.Forms.Edit.Dialogs
{
	/// <summary>
	/// Editor for list items.
	/// </summary>
	[ToolboxItem( false )]
	public class ListItemsEditor
		: System.Windows.Forms.UserControl
	{
		#region Constants
		/// <summary>
		///
		/// </summary>
		private const RegexOptions DEF_REGEX = Syncfusion.Windows.Forms.Edit.Implementation.Config.Config.DEF_COMPILED_REGEX;
		/// <summary>
		///
		/// </summary>
		private const string DEF_VALIDATION = ".*";
		/// <summary>
		///
		/// </summary>
		private const string DEF_EXAMPLE = "";
		/// <summary>
		/// Step between controls
		/// </summary>
		private const int DEF_STEP = 8;
		#endregion

		#region Fields
		/// <summary>
		///
		/// </summary>
		private ArrayList m_arrItems = new ArrayList();
		/// <summary>
		///
		/// </summary>
		private Regex m_validation = new Regex( DEF_VALIDATION, DEF_REGEX );
		/// <summary>
		///
		/// </summary>
		private string m_strExample = DEF_EXAMPLE;
		/// <summary>
		/// Reverse validation.
		/// </summary>
		private bool m_bReverse;
		#endregion

		#region Properties
		/// <summary>
		/// Gets list of items.
		/// </summary>
		[Browsable( false )]
		public string[] ItemsList
		{
			get
			{
				return ( string[] )m_arrItems.ToArray( typeof( string ) );
			}
		}
		/// <summary>
		/// Gets or sets Regular expression which validate input control data
		/// </summary>
		[Category( "Behavior" )
	 , TypeConverter( typeof( RegexConverter ) )
	 , RefreshProperties( RefreshProperties.All )
	 , Description( "GET or SET Regular expression which validate input control data" )]
		public Regex Validator
		{
			get
			{
				return m_validation;
			}
			set
			{
				m_validation = value;
			}
		}
		/// <summary>
		///Gets or sets example string.
		/// </summary>
		[Category( "Appearance" )
	 , Browsable( true )
	 , DefaultValue( "" )
	 , Description( "Get or Set example of items" )]
		public string Example
		{
			get
			{
				return m_strExample;
			}
			set
			{
				if( value != m_strExample )
				{
					m_strExample = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets value which indicates if validation is reverse.
		/// </summary>
		[Category( "Appearance" )
	 , Browsable( true )
	 , Description( "Get or Set reverse validation." )]
		public bool ReverseValidation
		{
			get
			{
				return m_bReverse;
			}
			set
			{
				if( m_bReverse != value )
				{
					m_bReverse = value;
				}
			}
		}
		#endregion

		#region Events
		/// <summary>
		/// On Add button click
		/// </summary>
		[Category( "Actions" )]
		public event EventHandler OnAddClick;
		/// <summary>
		/// On Remove Button click
		/// </summary>
		[Category( "Actions" )]
		public event EventHandler OnRemoveClick;
		#endregion

		#region Form controls
        /// <summary>
        /// Defines a new Button
        /// </summary>
		protected System.Windows.Forms.Button btnRemove;
        /// <summary>
        /// Defines a new Button
        /// </summary>
		protected System.Windows.Forms.Button btnAdd;
        /// <summary>
        /// Defines a new listbox
        /// </summary>
		protected System.Windows.Forms.ListBox lstItems;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		#endregion

		#region Initialize/Finalize methods
		/// <summary>
		/// Creates new instance of ListItemsEditor.
		/// </summary>
		public ListItemsEditor()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			EnableDoubleBuffering();
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager( typeof( ListItemsEditor ) );
			this.btnRemove = new System.Windows.Forms.Button();
			this.btnAdd = new System.Windows.Forms.Button();
			this.lstItems = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
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
			// lstItems
			// 
			this.lstItems.AccessibleDescription = resources.GetString( "lstItems.AccessibleDescription" );
			this.lstItems.AccessibleName = resources.GetString( "lstItems.AccessibleName" );
			this.lstItems.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "lstItems.Anchor" ) ) );
			this.lstItems.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "lstItems.BackgroundImage" ) ) );
			this.lstItems.ColumnWidth = ( ( int )( resources.GetObject( "lstItems.ColumnWidth" ) ) );
			this.lstItems.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "lstItems.Dock" ) ) );
			this.lstItems.Enabled = ( ( bool )( resources.GetObject( "lstItems.Enabled" ) ) );
			this.lstItems.Font = ( ( System.Drawing.Font )( resources.GetObject( "lstItems.Font" ) ) );
			this.lstItems.HorizontalExtent = ( ( int )( resources.GetObject( "lstItems.HorizontalExtent" ) ) );
			this.lstItems.HorizontalScrollbar = ( ( bool )( resources.GetObject( "lstItems.HorizontalScrollbar" ) ) );
			this.lstItems.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "lstItems.ImeMode" ) ) );
			this.lstItems.IntegralHeight = ( ( bool )( resources.GetObject( "lstItems.IntegralHeight" ) ) );
			this.lstItems.ItemHeight = ( ( int )( resources.GetObject( "lstItems.ItemHeight" ) ) );
			this.lstItems.Location = ( ( System.Drawing.Point )( resources.GetObject( "lstItems.Location" ) ) );
			this.lstItems.Name = "lstItems";
			this.lstItems.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "lstItems.RightToLeft" ) ) );
			this.lstItems.ScrollAlwaysVisible = ( ( bool )( resources.GetObject( "lstItems.ScrollAlwaysVisible" ) ) );
			this.lstItems.Size = ( ( System.Drawing.Size )( resources.GetObject( "lstItems.Size" ) ) );
			this.lstItems.TabIndex = ( ( int )( resources.GetObject( "lstItems.TabIndex" ) ) );
			this.lstItems.Visible = ( ( bool )( resources.GetObject( "lstItems.Visible" ) ) );
			// 
			// ListItemsEditor
			// 
			this.AccessibleDescription = resources.GetString( "$this.AccessibleDescription" );
			this.AccessibleName = resources.GetString( "$this.AccessibleName" );
			this.AutoScroll = ( ( bool )( resources.GetObject( "$this.AutoScroll" ) ) );
			this.AutoScrollMargin = ( ( System.Drawing.Size )( resources.GetObject( "$this.AutoScrollMargin" ) ) );
			this.AutoScrollMinSize = ( ( System.Drawing.Size )( resources.GetObject( "$this.AutoScrollMinSize" ) ) );
			this.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "$this.BackgroundImage" ) ) );
			this.Controls.Add( this.btnRemove );
			this.Controls.Add( this.btnAdd );
			this.Controls.Add( this.lstItems );
			this.Enabled = ( ( bool )( resources.GetObject( "$this.Enabled" ) ) );
			this.Font = ( ( System.Drawing.Font )( resources.GetObject( "$this.Font" ) ) );
			this.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "$this.ImeMode" ) ) );
			this.Location = ( ( System.Drawing.Point )( resources.GetObject( "$this.Location" ) ) );
			this.Name = "ListItemsEditor";
			this.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "$this.RightToLeft" ) ) );
			this.Size = ( ( System.Drawing.Size )( resources.GetObject( "$this.Size" ) ) );
			this.Resize += new System.EventHandler( this.ListItemsEditor_Resize );
			this.ResumeLayout( false );

		}
		#endregion

		#region Event Handlers
		/// <summary>
		///
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnAdd_Click( object sender, System.EventArgs e )
		{
			if( OnAddClick != null )
			{
				OnAddClick( sender, e );
			}

			using( frmSimpleAdd form = new frmSimpleAdd( "", this.Example, this.Validator ) )
			{
				form.ReverseValidation = this.ReverseValidation;

				if( DialogResult.OK == form.ShowDialog() )
				{
					m_arrItems.Add( ( form.Value == null ) ? "" : form.Value );
					UpdateItems();
				}
			}
		}
		/// <summary>
		///
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnRemove_Click( object sender, System.EventArgs e )
		{
			if( OnRemoveClick != null )
			{
				OnRemoveClick( sender, e );
			}

			foreach( string item in lstItems.SelectedItems )
			{
				m_arrItems.Remove( item );
			}

			UpdateItems();
		}
		/// <summary>
		///
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ListItemsEditor_Resize( object sender, System.EventArgs e )
		{
			btnAdd.Left = this.Width - DEF_STEP - btnAdd.Width;
			btnRemove.Left = this.Width - DEF_STEP - btnRemove.Width;
			lstItems.Width = btnAdd.Left - DEF_STEP * 2;
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Adds new value.
		/// </summary>
		/// <param name="value">String value to add.</param>
		public void Add( string value )
		{
			if( value == null ) throw new ArgumentNullException( "value" );
			if( value.Length == 0 ) throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_118 );

			m_arrItems.Add( value );
			UpdateItems();
		}
		/// <summary>
		/// Adds range of values.
		/// </summary>
		/// <param name="value">Range of string values to add.</param>
		public void AddRange( string[] value )
		{
			if( value == null ) throw new ArgumentNullException( "value" );

			m_arrItems.AddRange( value );
			UpdateItems();
		}
		/// <summary>
		/// Removes item from list.
		/// </summary>
		/// <param name="value">Item to remove.</param>
		public void Remove( string value )
		{
			if( value == null ) throw new ArgumentNullException( "value" );
			if( value.Length == 0 ) throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_118 );

			m_arrItems.Remove( value );
			UpdateItems();
		}
		/// <summary>
		/// Deletes all items from the list.
		/// </summary>
		public void Clear()
		{
			m_arrItems.Clear();
			UpdateItems();
		}
		#endregion

		#region Utility Methods
		/// <summary>
		/// Updates items.
		/// </summary>
		protected void UpdateItems()
		{
			m_arrItems.Sort();

			lstItems.BeginUpdate();
			lstItems.Items.Clear();
			lstItems.Items.AddRange( m_arrItems.ToArray() );
			lstItems.EndUpdate();
		}
		/// <summary>
		/// Enables double buffering.
		/// </summary>
		public void EnableDoubleBuffering()
		{
			// Set the value of the double-buffering style bits to true.
			this.SetStyle( ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true );
			this.UpdateStyles();
		}
		#endregion
	}
}