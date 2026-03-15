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
using System.Windows.Forms;
using System.Collections;
using System.Diagnostics;

using Syncfusion.Windows.Forms.Localization;
using Syncfusion.Shared.Utils.KeyBinding.Implementation;
using Syncfusion.Shared.Utils.KeyBinding;

namespace Syncfusion.Windows.Forms.Edit.Dialogs
{
	/// <summary>
	/// Form for key bindings.
	/// </summary>
	public class frmKeysBinding
		: System.Windows.Forms.Form
	{
		#region Fields
		/// <summary>
		/// 
		/// </summary>
		private KeyProcessor m_parent;
		/// <summary>
		/// 
		/// </summary>
		private KeysConverter m_convert = new KeysConverter();
		/// <summary>
		/// List of the keys, used in current key-sequence.
		/// </summary>
		private IList m_keysList = new ArrayList();
		/// <summary>
		/// Key-bindings container.
		/// </summary>
		private IKeyBinderContainer m_container;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets associated key processor.
		/// </summary>
		public KeyProcessor KeyBinder
		{
			get
			{
				return m_parent;
			}
			set
			{
				if( value == null )
					throw new ArgumentNullException( "value" );

				if( m_parent != value )
				{
					// if we replace parent collection then cancel changes made 
					// to previous instance
					if( m_parent != null )
					{
						m_parent.CancelEdit();
					}

					m_parent = value;

					// if we active form then reinit form
					if( this.Visible )
					{
						frmKeysBinding_Load( this, EventArgs.Empty );
					}
				}
			}
		}
		#endregion

		#region Form controls
		private System.Windows.Forms.Label lblCommands;
		private System.Windows.Forms.ListBox lstCommands;
		private ShortcutTextBox txtShortcut;
		private System.Windows.Forms.Label lblShortcut;
		private System.Windows.Forms.Button btnAssign;
		private System.Windows.Forms.Label lblCommandShortcuts;
		private System.Windows.Forms.ComboBox comboCommandShortcuts;
		private System.Windows.Forms.Button btnRemove;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnDefaults;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		#endregion

		#region Initialize/Finalize Methods
		/// <summary>
		/// Creates new instance of frmKeysBinding.
		/// </summary>
		public frmKeysBinding()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			EnableDoubleBuffering();
		}
		/// <summary>
		/// Creates new instance of frmKeysBinding.
		/// </summary>
		/// <param name="parent">Underlying KeyProcessor.</param>
		/// <param name="container">IKeyBinderContainer implementation.</param>
		public frmKeysBinding( KeyProcessor parent, IKeyBinderContainer container )
			: this()
		{
			if( parent == null ) throw new ArgumentNullException( "parent" );
			if( container == null ) throw new ArgumentNullException( "container" );

			m_parent = parent;
			m_container = container;
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
		/// <summary>
		/// Initializes list of commands.
		/// </summary>
		protected void InitializeListCommands()
		{
			lstCommands.BeginUpdate();
			lstCommands.Items.Clear();

			if( m_parent != null )
			{
				object[] objects = new object[ m_parent.Commands.Count ];
				m_parent.Commands.CopyTo( objects, 0 );
				lstCommands.Items.AddRange( objects );
			}

			lstCommands.EndUpdate();

			// select first element from list
			if( lstCommands.Items.Count > 0 )
				lstCommands.SelectedIndex = 0;
		}
		/// <summary>
		/// Initializes combobox with list of commands.
		/// </summary>
		/// <param name="bindings">Collection of bindings.</param>
		protected void InitializeComboList( IKeyCommandBinder[] bindings )
		{
			comboCommandShortcuts.BeginUpdate();
			comboCommandShortcuts.Items.Clear();

			if( bindings != null && bindings.Length > 0 )
			{
				for( int i = 0, len = bindings.Length; i < len; i++ )
				{
					comboCommandShortcuts.Items.Add( bindings[ i ] );
				}
			}

			comboCommandShortcuts.EndUpdate();

			if( comboCommandShortcuts.Items.Count > 0 )
				comboCommandShortcuts.SelectedIndex = 0;

			comboCommandShortcuts.Enabled = ( comboCommandShortcuts.Items.Count != 0 );
		}
		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager( typeof( frmKeysBinding ) );
			this.lblCommands = new System.Windows.Forms.Label();
			this.lstCommands = new System.Windows.Forms.ListBox();
			this.txtShortcut = new Syncfusion.Windows.Forms.Edit.Dialogs.ShortcutTextBox();
			this.lblShortcut = new System.Windows.Forms.Label();
			this.btnAssign = new System.Windows.Forms.Button();
			this.lblCommandShortcuts = new System.Windows.Forms.Label();
			this.comboCommandShortcuts = new System.Windows.Forms.ComboBox();
			this.btnRemove = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnDefaults = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lblCommands
			// 
			this.lblCommands.AccessibleDescription = resources.GetString( "lblCommands.AccessibleDescription" );
			this.lblCommands.AccessibleName = resources.GetString( "lblCommands.AccessibleName" );
			this.lblCommands.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "lblCommands.Anchor" ) ) );
			this.lblCommands.AutoSize = ( ( bool )( resources.GetObject( "lblCommands.AutoSize" ) ) );
			this.lblCommands.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "lblCommands.Dock" ) ) );
			this.lblCommands.Enabled = ( ( bool )( resources.GetObject( "lblCommands.Enabled" ) ) );
			this.lblCommands.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.lblCommands.Font = ( ( System.Drawing.Font )( resources.GetObject( "lblCommands.Font" ) ) );
			this.lblCommands.Image = ( ( System.Drawing.Image )( resources.GetObject( "lblCommands.Image" ) ) );
			this.lblCommands.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblCommands.ImageAlign" ) ) );
			this.lblCommands.ImageIndex = ( ( int )( resources.GetObject( "lblCommands.ImageIndex" ) ) );
			this.lblCommands.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "lblCommands.ImeMode" ) ) );
			this.lblCommands.Location = ( ( System.Drawing.Point )( resources.GetObject( "lblCommands.Location" ) ) );
			this.lblCommands.Name = "lblCommands";
			this.lblCommands.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "lblCommands.RightToLeft" ) ) );
			this.lblCommands.Size = ( ( System.Drawing.Size )( resources.GetObject( "lblCommands.Size" ) ) );
			this.lblCommands.TabIndex = ( ( int )( resources.GetObject( "lblCommands.TabIndex" ) ) );
			this.lblCommands.Text = resources.GetString( "lblCommands.Text" );
			this.lblCommands.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblCommands.TextAlign" ) ) );
			this.lblCommands.Visible = ( ( bool )( resources.GetObject( "lblCommands.Visible" ) ) );
			// 
			// lstCommands
			// 
			this.lstCommands.AccessibleDescription = resources.GetString( "lstCommands.AccessibleDescription" );
			this.lstCommands.AccessibleName = resources.GetString( "lstCommands.AccessibleName" );
			this.lstCommands.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "lstCommands.Anchor" ) ) );
			this.lstCommands.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "lstCommands.BackgroundImage" ) ) );
			this.lstCommands.ColumnWidth = ( ( int )( resources.GetObject( "lstCommands.ColumnWidth" ) ) );
			this.lstCommands.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "lstCommands.Dock" ) ) );
			this.lstCommands.Enabled = ( ( bool )( resources.GetObject( "lstCommands.Enabled" ) ) );
			this.lstCommands.Font = ( ( System.Drawing.Font )( resources.GetObject( "lstCommands.Font" ) ) );
			this.lstCommands.HorizontalExtent = ( ( int )( resources.GetObject( "lstCommands.HorizontalExtent" ) ) );
			this.lstCommands.HorizontalScrollbar = ( ( bool )( resources.GetObject( "lstCommands.HorizontalScrollbar" ) ) );
			this.lstCommands.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "lstCommands.ImeMode" ) ) );
			this.lstCommands.IntegralHeight = ( ( bool )( resources.GetObject( "lstCommands.IntegralHeight" ) ) );
			this.lstCommands.ItemHeight = ( ( int )( resources.GetObject( "lstCommands.ItemHeight" ) ) );
			this.lstCommands.Location = ( ( System.Drawing.Point )( resources.GetObject( "lstCommands.Location" ) ) );
			this.lstCommands.Name = "lstCommands";
			this.lstCommands.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "lstCommands.RightToLeft" ) ) );
			this.lstCommands.ScrollAlwaysVisible = ( ( bool )( resources.GetObject( "lstCommands.ScrollAlwaysVisible" ) ) );
			this.lstCommands.Size = ( ( System.Drawing.Size )( resources.GetObject( "lstCommands.Size" ) ) );
			this.lstCommands.Sorted = true;
			this.lstCommands.TabIndex = ( ( int )( resources.GetObject( "lstCommands.TabIndex" ) ) );
			this.lstCommands.Visible = ( ( bool )( resources.GetObject( "lstCommands.Visible" ) ) );
			this.lstCommands.SelectedIndexChanged += new System.EventHandler( this.lstCommands_SelectedIndexChanged );
			// 
			// txtShortcut
			// 
			this.txtShortcut.AccessibleDescription = resources.GetString( "txtShortcut.AccessibleDescription" );
			this.txtShortcut.AccessibleName = resources.GetString( "txtShortcut.AccessibleName" );
			this.txtShortcut.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "txtShortcut.Anchor" ) ) );
			this.txtShortcut.AutoSize = ( ( bool )( resources.GetObject( "txtShortcut.AutoSize" ) ) );
			this.txtShortcut.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "txtShortcut.BackgroundImage" ) ) );
			this.txtShortcut.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "txtShortcut.Dock" ) ) );
			this.txtShortcut.Enabled = ( ( bool )( resources.GetObject( "txtShortcut.Enabled" ) ) );
			this.txtShortcut.Font = ( ( System.Drawing.Font )( resources.GetObject( "txtShortcut.Font" ) ) );
			this.txtShortcut.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "txtShortcut.ImeMode" ) ) );
			this.txtShortcut.Location = ( ( System.Drawing.Point )( resources.GetObject( "txtShortcut.Location" ) ) );
			this.txtShortcut.MaxLength = ( ( int )( resources.GetObject( "txtShortcut.MaxLength" ) ) );
			this.txtShortcut.Multiline = ( ( bool )( resources.GetObject( "txtShortcut.Multiline" ) ) );
			this.txtShortcut.Name = "txtShortcut";
			this.txtShortcut.PasswordChar = ( ( char )( resources.GetObject( "txtShortcut.PasswordChar" ) ) );
			this.txtShortcut.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "txtShortcut.RightToLeft" ) ) );
			this.txtShortcut.ScrollBars = ( ( System.Windows.Forms.ScrollBars )( resources.GetObject( "txtShortcut.ScrollBars" ) ) );
			this.txtShortcut.Size = ( ( System.Drawing.Size )( resources.GetObject( "txtShortcut.Size" ) ) );
			this.txtShortcut.TabIndex = ( ( int )( resources.GetObject( "txtShortcut.TabIndex" ) ) );
			this.txtShortcut.Text = resources.GetString( "txtShortcut.Text" );
			this.txtShortcut.TextAlign = ( ( System.Windows.Forms.HorizontalAlignment )( resources.GetObject( "txtShortcut.TextAlign" ) ) );
			this.txtShortcut.Visible = ( ( bool )( resources.GetObject( "txtShortcut.Visible" ) ) );
			this.txtShortcut.WordWrap = ( ( bool )( resources.GetObject( "txtShortcut.WordWrap" ) ) );
			this.txtShortcut.TextChanged += new System.EventHandler( this.txtShortcut_TextChanged );
			// 
			// lblShortcut
			// 
			this.lblShortcut.AccessibleDescription = resources.GetString( "lblShortcut.AccessibleDescription" );
			this.lblShortcut.AccessibleName = resources.GetString( "lblShortcut.AccessibleName" );
			this.lblShortcut.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "lblShortcut.Anchor" ) ) );
			this.lblShortcut.AutoSize = ( ( bool )( resources.GetObject( "lblShortcut.AutoSize" ) ) );
			this.lblShortcut.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "lblShortcut.Dock" ) ) );
			this.lblShortcut.Enabled = ( ( bool )( resources.GetObject( "lblShortcut.Enabled" ) ) );
			this.lblShortcut.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.lblShortcut.Font = ( ( System.Drawing.Font )( resources.GetObject( "lblShortcut.Font" ) ) );
			this.lblShortcut.Image = ( ( System.Drawing.Image )( resources.GetObject( "lblShortcut.Image" ) ) );
			this.lblShortcut.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblShortcut.ImageAlign" ) ) );
			this.lblShortcut.ImageIndex = ( ( int )( resources.GetObject( "lblShortcut.ImageIndex" ) ) );
			this.lblShortcut.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "lblShortcut.ImeMode" ) ) );
			this.lblShortcut.Location = ( ( System.Drawing.Point )( resources.GetObject( "lblShortcut.Location" ) ) );
			this.lblShortcut.Name = "lblShortcut";
			this.lblShortcut.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "lblShortcut.RightToLeft" ) ) );
			this.lblShortcut.Size = ( ( System.Drawing.Size )( resources.GetObject( "lblShortcut.Size" ) ) );
			this.lblShortcut.TabIndex = ( ( int )( resources.GetObject( "lblShortcut.TabIndex" ) ) );
			this.lblShortcut.Text = resources.GetString( "lblShortcut.Text" );
			this.lblShortcut.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblShortcut.TextAlign" ) ) );
			this.lblShortcut.Visible = ( ( bool )( resources.GetObject( "lblShortcut.Visible" ) ) );
			// 
			// btnAssign
			// 
			this.btnAssign.AccessibleDescription = resources.GetString( "btnAssign.AccessibleDescription" );
			this.btnAssign.AccessibleName = resources.GetString( "btnAssign.AccessibleName" );
			this.btnAssign.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "btnAssign.Anchor" ) ) );
			this.btnAssign.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "btnAssign.BackgroundImage" ) ) );
			this.btnAssign.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "btnAssign.Dock" ) ) );
			this.btnAssign.Enabled = ( ( bool )( resources.GetObject( "btnAssign.Enabled" ) ) );
			this.btnAssign.FlatStyle = ( ( System.Windows.Forms.FlatStyle )( resources.GetObject( "btnAssign.FlatStyle" ) ) );
			this.btnAssign.Font = ( ( System.Drawing.Font )( resources.GetObject( "btnAssign.Font" ) ) );
			this.btnAssign.Image = ( ( System.Drawing.Image )( resources.GetObject( "btnAssign.Image" ) ) );
			this.btnAssign.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnAssign.ImageAlign" ) ) );
			this.btnAssign.ImageIndex = ( ( int )( resources.GetObject( "btnAssign.ImageIndex" ) ) );
			this.btnAssign.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "btnAssign.ImeMode" ) ) );
			this.btnAssign.Location = ( ( System.Drawing.Point )( resources.GetObject( "btnAssign.Location" ) ) );
			this.btnAssign.Name = "btnAssign";
			this.btnAssign.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "btnAssign.RightToLeft" ) ) );
			this.btnAssign.Size = ( ( System.Drawing.Size )( resources.GetObject( "btnAssign.Size" ) ) );
			this.btnAssign.TabIndex = ( ( int )( resources.GetObject( "btnAssign.TabIndex" ) ) );
			this.btnAssign.Text = resources.GetString( "btnAssign.Text" );
			this.btnAssign.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnAssign.TextAlign" ) ) );
			this.btnAssign.Visible = ( ( bool )( resources.GetObject( "btnAssign.Visible" ) ) );
			this.btnAssign.Click += new System.EventHandler( this.btnAssign_Click );
			// 
			// lblCommandShortcuts
			// 
			this.lblCommandShortcuts.AccessibleDescription = resources.GetString( "lblCommandShortcuts.AccessibleDescription" );
			this.lblCommandShortcuts.AccessibleName = resources.GetString( "lblCommandShortcuts.AccessibleName" );
			this.lblCommandShortcuts.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "lblCommandShortcuts.Anchor" ) ) );
			this.lblCommandShortcuts.AutoSize = ( ( bool )( resources.GetObject( "lblCommandShortcuts.AutoSize" ) ) );
			this.lblCommandShortcuts.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "lblCommandShortcuts.Dock" ) ) );
			this.lblCommandShortcuts.Enabled = ( ( bool )( resources.GetObject( "lblCommandShortcuts.Enabled" ) ) );
			this.lblCommandShortcuts.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.lblCommandShortcuts.Font = ( ( System.Drawing.Font )( resources.GetObject( "lblCommandShortcuts.Font" ) ) );
			this.lblCommandShortcuts.Image = ( ( System.Drawing.Image )( resources.GetObject( "lblCommandShortcuts.Image" ) ) );
			this.lblCommandShortcuts.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblCommandShortcuts.ImageAlign" ) ) );
			this.lblCommandShortcuts.ImageIndex = ( ( int )( resources.GetObject( "lblCommandShortcuts.ImageIndex" ) ) );
			this.lblCommandShortcuts.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "lblCommandShortcuts.ImeMode" ) ) );
			this.lblCommandShortcuts.Location = ( ( System.Drawing.Point )( resources.GetObject( "lblCommandShortcuts.Location" ) ) );
			this.lblCommandShortcuts.Name = "lblCommandShortcuts";
			this.lblCommandShortcuts.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "lblCommandShortcuts.RightToLeft" ) ) );
			this.lblCommandShortcuts.Size = ( ( System.Drawing.Size )( resources.GetObject( "lblCommandShortcuts.Size" ) ) );
			this.lblCommandShortcuts.TabIndex = ( ( int )( resources.GetObject( "lblCommandShortcuts.TabIndex" ) ) );
			this.lblCommandShortcuts.Text = resources.GetString( "lblCommandShortcuts.Text" );
			this.lblCommandShortcuts.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblCommandShortcuts.TextAlign" ) ) );
			this.lblCommandShortcuts.Visible = ( ( bool )( resources.GetObject( "lblCommandShortcuts.Visible" ) ) );
			// 
			// comboCommandShortcuts
			// 
			this.comboCommandShortcuts.AccessibleDescription = resources.GetString( "comboCommandShortcuts.AccessibleDescription" );
			this.comboCommandShortcuts.AccessibleName = resources.GetString( "comboCommandShortcuts.AccessibleName" );
			this.comboCommandShortcuts.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "comboCommandShortcuts.Anchor" ) ) );
			this.comboCommandShortcuts.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "comboCommandShortcuts.BackgroundImage" ) ) );
			this.comboCommandShortcuts.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "comboCommandShortcuts.Dock" ) ) );
			this.comboCommandShortcuts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboCommandShortcuts.Enabled = ( ( bool )( resources.GetObject( "comboCommandShortcuts.Enabled" ) ) );
			this.comboCommandShortcuts.Font = ( ( System.Drawing.Font )( resources.GetObject( "comboCommandShortcuts.Font" ) ) );
			this.comboCommandShortcuts.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "comboCommandShortcuts.ImeMode" ) ) );
			this.comboCommandShortcuts.IntegralHeight = ( ( bool )( resources.GetObject( "comboCommandShortcuts.IntegralHeight" ) ) );
			this.comboCommandShortcuts.ItemHeight = ( ( int )( resources.GetObject( "comboCommandShortcuts.ItemHeight" ) ) );
			this.comboCommandShortcuts.Location = ( ( System.Drawing.Point )( resources.GetObject( "comboCommandShortcuts.Location" ) ) );
			this.comboCommandShortcuts.MaxDropDownItems = ( ( int )( resources.GetObject( "comboCommandShortcuts.MaxDropDownItems" ) ) );
			this.comboCommandShortcuts.MaxLength = ( ( int )( resources.GetObject( "comboCommandShortcuts.MaxLength" ) ) );
			this.comboCommandShortcuts.Name = "comboCommandShortcuts";
			this.comboCommandShortcuts.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "comboCommandShortcuts.RightToLeft" ) ) );
			this.comboCommandShortcuts.Size = ( ( System.Drawing.Size )( resources.GetObject( "comboCommandShortcuts.Size" ) ) );
			this.comboCommandShortcuts.Sorted = true;
			this.comboCommandShortcuts.TabIndex = ( ( int )( resources.GetObject( "comboCommandShortcuts.TabIndex" ) ) );
			this.comboCommandShortcuts.Text = resources.GetString( "comboCommandShortcuts.Text" );
			this.comboCommandShortcuts.Visible = ( ( bool )( resources.GetObject( "comboCommandShortcuts.Visible" ) ) );
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
			this.btnOK.Click += new System.EventHandler( this.btnOK_Click );
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
			this.btnCancel.Click += new System.EventHandler( this.btnCancel_Click );
			// 
			// btnDefaults
			// 
			this.btnDefaults.AccessibleDescription = resources.GetString( "btnDefaults.AccessibleDescription" );
			this.btnDefaults.AccessibleName = resources.GetString( "btnDefaults.AccessibleName" );
			this.btnDefaults.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "btnDefaults.Anchor" ) ) );
			this.btnDefaults.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "btnDefaults.BackgroundImage" ) ) );
			this.btnDefaults.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "btnDefaults.Dock" ) ) );
			this.btnDefaults.Enabled = ( ( bool )( resources.GetObject( "btnDefaults.Enabled" ) ) );
			this.btnDefaults.FlatStyle = ( ( System.Windows.Forms.FlatStyle )( resources.GetObject( "btnDefaults.FlatStyle" ) ) );
			this.btnDefaults.Font = ( ( System.Drawing.Font )( resources.GetObject( "btnDefaults.Font" ) ) );
			this.btnDefaults.Image = ( ( System.Drawing.Image )( resources.GetObject( "btnDefaults.Image" ) ) );
			this.btnDefaults.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnDefaults.ImageAlign" ) ) );
			this.btnDefaults.ImageIndex = ( ( int )( resources.GetObject( "btnDefaults.ImageIndex" ) ) );
			this.btnDefaults.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "btnDefaults.ImeMode" ) ) );
			this.btnDefaults.Location = ( ( System.Drawing.Point )( resources.GetObject( "btnDefaults.Location" ) ) );
			this.btnDefaults.Name = "btnDefaults";
			this.btnDefaults.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "btnDefaults.RightToLeft" ) ) );
			this.btnDefaults.Size = ( ( System.Drawing.Size )( resources.GetObject( "btnDefaults.Size" ) ) );
			this.btnDefaults.TabIndex = ( ( int )( resources.GetObject( "btnDefaults.TabIndex" ) ) );
			this.btnDefaults.Text = resources.GetString( "btnDefaults.Text" );
			this.btnDefaults.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnDefaults.TextAlign" ) ) );
			this.btnDefaults.Visible = ( ( bool )( resources.GetObject( "btnDefaults.Visible" ) ) );
			this.btnDefaults.Click += new System.EventHandler( this.btnDefaults_Click );
			// 
			// frmKeysBinding
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
			this.Controls.Add( this.btnDefaults );
			this.Controls.Add( this.btnOK );
			this.Controls.Add( this.comboCommandShortcuts );
			this.Controls.Add( this.btnAssign );
			this.Controls.Add( this.txtShortcut );
			this.Controls.Add( this.lstCommands );
			this.Controls.Add( this.lblCommands );
			this.Controls.Add( this.lblShortcut );
			this.Controls.Add( this.lblCommandShortcuts );
			this.Controls.Add( this.btnRemove );
			this.Controls.Add( this.btnCancel );
			this.Enabled = ( ( bool )( resources.GetObject( "$this.Enabled" ) ) );
			this.Font = ( ( System.Drawing.Font )( resources.GetObject( "$this.Font" ) ) );
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.HelpButton = true;
			this.Icon = ( ( System.Drawing.Icon )( resources.GetObject( "$this.Icon" ) ) );
			this.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "$this.ImeMode" ) ) );
			this.Location = ( ( System.Drawing.Point )( resources.GetObject( "$this.Location" ) ) );
			this.MaximizeBox = false;
			this.MaximumSize = ( ( System.Drawing.Size )( resources.GetObject( "$this.MaximumSize" ) ) );
			this.MinimizeBox = false;
			this.MinimumSize = ( ( System.Drawing.Size )( resources.GetObject( "$this.MinimumSize" ) ) );
			this.Name = "frmKeysBinding";
			this.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "$this.RightToLeft" ) ) );
			this.RightToLeftLayout = true;
			this.StartPosition = ( ( System.Windows.Forms.FormStartPosition )( resources.GetObject( "$this.StartPosition" ) ) );
			this.Text = resources.GetString( "$this.Text" );
			this.Load += new System.EventHandler( this.frmKeysBinding_Load );
			this.ResumeLayout( false );
		}
		#endregion

		#region Utility Methods
		/// <summary>
		/// Updates controls.
		/// </summary>
		protected void UpdateControlsInfill()
		{
			if( m_parent != null )
			{
				if( lstCommands.SelectedItem != null )
				{
					string name = lstCommands.SelectedItem.ToString();
					IKeyCommandBinder[] bindings = m_parent.Binder.FindBindings( name );

					InitializeComboList( bindings );
				}
			}
		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lstCommands_SelectedIndexChanged( object sender, System.EventArgs e )
		{
			UpdateControlsInfill();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnRemove_Click( object sender, System.EventArgs e )
		{
			if( m_parent != null )
			{
				if( comboCommandShortcuts.SelectedItem != null )
				{
					IKeyCommandBinder binder = comboCommandShortcuts.SelectedItem as IKeyCommandBinder;
					binder.Parent.RemoveBinding( binder.Key );
				}
			}

			UpdateControlsInfill();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnAssign_Click( object sender, System.EventArgs e )
		{
			if( m_parent != null && lstCommands.SelectedItem != null )
			{
				if( txtShortcut.Text != null && txtShortcut.Text.Length > 0 )
				{
					string name = lstCommands.SelectedItem.ToString();

					try
					{
						ShortcutTextBox.KeyState[] keys = txtShortcut.EnteredKeys;
						IKeyCommandListBinder currentbinder = m_parent.Binder;

						for( int i = 0; i < keys.Length; i++ )
						{
							if( i + 1 < keys.Length )
								currentbinder = currentbinder.BindToBinder( keys[ i ].KeyCombination );
							else
								currentbinder.BindToCommand( keys[ i ].KeyCombination, name );
						}

					}
					catch( Exception ex )
					{
						Debug.WriteLine( ex.Message + Environment.NewLine + ex.StackTrace, "Exception" );
					}
				}
			}

			UpdateControlsInfill();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnOK_Click( object sender, System.EventArgs e )
		{
			// apply changes
			if( m_parent != null )
			{
				m_parent.EndEdit();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnCancel_Click( object sender, System.EventArgs e )
		{
			// cancel changes
			if( m_parent != null )
			{
				m_parent.CancelEdit();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void frmKeysBinding_Load( object sender, System.EventArgs e )
		{
			InitializeListCommands();

			if( m_parent != null )
			{
				m_parent.BeginEdit();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void txtShortcut_TextChanged( object sender, System.EventArgs e )
		{
			btnAssign.Enabled = txtShortcut.EnteredKeys.Length > 0;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnDefaults_Click( object sender, System.EventArgs e )
		{
			DialogResult result = MessageBox.Show( this as IWin32Window,
				Localizer.DEF_MSG_KEYBINDINGS_SET_DEFAULT,
				Localizer.DEF_MSG_KEYBINDINGS_SET_DEFAULT_CAPTION, MessageBoxButtons.YesNo,
				MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2 );

			if( result == DialogResult.Yes )
			{
				m_parent.InitializeClassDefaults( m_container );
				UpdateControlsInfill();
			}
		}
		#endregion
	}
}
