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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Implementation.IO;
using Syncfusion.Windows.Forms.Edit.Enums;
using Syncfusion.Windows.Forms.Edit.Utils;
using System.Text.RegularExpressions;
using System.Text;
using Syncfusion.Windows.Forms.Localization;

namespace Syncfusion.Windows.Forms.Edit.Dialogs
{
	/// <summary>
	/// Dialog for replacing defined text.
	/// </summary>
	public class frmReplaceDialog
		: FrmFindDialog
		, IReplaceDialogForm
	{
		#region Fields
		/// <summary>
		/// History of searched text.
		/// </summary>
		private ArrayList m_replaceHistory;
		/// <summary>
		/// Text on which defined can be replaced.
		/// </summary>
		private string m_replaceText;
		#endregion

		#region Properties
		/// <summary>
		/// Gets history of replacing text.
		/// </summary>
		public ArrayList ReplaceHistory
		{
			get
			{
				return m_replaceHistory;
			}
		}
		/// <summary>
		/// Gets or sets text for replacing.
		/// </summary>
		public string ReplaceText
		{
			get
			{
				return m_replaceText;
			}
			set
			{
				if( value == null ) throw new ArgumentNullException( "ReplaceText" );

				if( m_replaceText != value )
				{
					m_replaceText = value;
				}
			}
		}
		#endregion

		#region Initialization & Finalization
		/// <summary>
		/// Creates new instance of frmReplaceDialog.
		/// </summary>
		public frmReplaceDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			EnableDoubleBuffering();
		}
		/// <summary>
		///Creates new instance of frmReplaceDialog.
		/// </summary>
		/// <param name="parent">StreamEditControl.</param>
		public frmReplaceDialog( StreamEditControl parent )
			: base( parent )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			m_replaceHistory = new ArrayList();
			m_replaceText = string.Empty;
		}
		/// <summary>
		/// Disposes all resources.
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

		#region Form Controls
		private System.Windows.Forms.Label lblReplace;
		private System.Windows.Forms.Button btnReplace;
		private System.Windows.Forms.Button btnReplaceAll;
		private System.Windows.Forms.Button btnReplaceTemplates;
		private System.Windows.Forms.ComboBox cmbReplace;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager( typeof( frmReplaceDialog ) );
			this.lblReplace = new System.Windows.Forms.Label();
			this.btnReplace = new System.Windows.Forms.Button();
			this.btnReplaceAll = new System.Windows.Forms.Button();
			this.cmbReplace = new System.Windows.Forms.ComboBox();
			this.btnReplaceTemplates = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// cmbFind
			// 
			this.cmbFind.Location = ( ( System.Drawing.Point )( resources.GetObject( "cmbFind.Location" ) ) );
			this.cmbFind.Name = "cmbFind";
			this.cmbFind.Size = ( ( System.Drawing.Size )( resources.GetObject( "cmbFind.Size" ) ) );
			// 
			// chkCase
			// 
            this.chkCase.AutoSize = true;
			this.chkCase.Location = ( ( System.Drawing.Point )( resources.GetObject( "chkCase.Location" ) ) );
			this.chkCase.Name = "chkCase";
			this.chkCase.TabIndex = ( ( int )( resources.GetObject( "chkCase.TabIndex" ) ) );
            this.chkCase.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FRchkCase) == null) ? resources.GetString("chkCase.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FRchkCase);
			// 
			// btnFind
			// 
            this.btnFind.AutoSize = true;
			this.btnFind.Name = "btnFind";
			this.btnFind.TabIndex = ( ( int )( resources.GetObject( "btnFind.TabIndex" ) ) );
			// 
			// btnClose
			// 
            this.btnClose.AutoSize = true;
			this.btnClose.Location = ( ( System.Drawing.Point )( resources.GetObject( "btnClose.Location" ) ) );
			this.btnClose.Name = "btnClose";
			this.btnClose.TabIndex = ( ( int )( resources.GetObject( "btnClose.TabIndex" ) ) );
            this.btnClose.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FRbtnClose) == null) ? resources.GetString("btnClose.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FRbtnClose);
			// 
			// lblFind
			// 
            this.lblFind.AutoSize = true;
			this.lblFind.Name = "lblFind";
			this.lblFind.Size = ( ( System.Drawing.Size )( resources.GetObject( "lblFind.Size" ) ) );
            this.lblFind.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FRlblFind) == null) ? resources.GetString("lblFind.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FRlblFind);
			// 
			// btnTempaltes
			// 
            this.btnTempaltes.AutoSize = true;
			this.btnTempaltes.Location = ( ( System.Drawing.Point )( resources.GetObject( "btnTempaltes.Location" ) ) );
			this.btnTempaltes.Name = "btnTempaltes";
			this.btnTempaltes.TabIndex = ( ( int )( resources.GetObject( "btnTempaltes.TabIndex" ) ) );
			// 
			// btnMarkAll
			// 
            this.btnMarkAll.AutoSize = true;
			this.btnMarkAll.Location = ( ( System.Drawing.Point )( resources.GetObject( "btnMarkAll.Location" ) ) );
			this.btnMarkAll.Name = "btnMarkAll";
			this.btnMarkAll.TabIndex = ( ( int )( resources.GetObject( "btnMarkAll.TabIndex" ) ) );
			// 
			// chkWholeWord
			// 
            this.chkWholeWord.AutoSize = true;
			this.chkWholeWord.Location = ( ( System.Drawing.Point )( resources.GetObject( "chkWholeWord.Location" ) ) );
			this.chkWholeWord.Name = "chkWholeWord";
			this.chkWholeWord.TabIndex = ( ( int )( resources.GetObject( "chkWholeWord.TabIndex" ) ) );
			// 
			// chkHidden
			// 
            this.chkHidden.AutoSize = true;
			this.chkHidden.Location = ( ( System.Drawing.Point )( resources.GetObject( "chkHidden.Location" ) ) );
			this.chkHidden.Name = "chkHidden";
			this.chkHidden.TabIndex = ( ( int )( resources.GetObject( "chkHidden.TabIndex" ) ) );
			// 
			// chkUp
			// 
            this.chkUp.AutoSize = true;
			this.chkUp.Location = ( ( System.Drawing.Point )( resources.GetObject( "chkUp.Location" ) ) );
			this.chkUp.Name = "chkUp";
			this.chkUp.TabIndex = ( ( int )( resources.GetObject( "chkUp.TabIndex" ) ) );
			// 
			// chkRegular
			// 
            this.chkRegular.AutoSize = true;
			this.chkRegular.Location = ( ( System.Drawing.Point )( resources.GetObject( "chkRegular.Location" ) ) );
			this.chkRegular.Name = "chkRegular";
			this.chkRegular.TabIndex = ( ( int )( resources.GetObject( "chkRegular.TabIndex" ) ) );
            // 
            // chkWrap
            // 
            this.chkWrap.AutoSize = true;
            this.chkWrap.Location = ((System.Drawing.Point)(resources.GetObject("chkWrap.Location")));
            this.chkWrap.Name = "chkRegular";
            this.chkWrap.TabIndex = ((int)(resources.GetObject("chkWrap.TabIndex")));
            
			// 
			// groupBox1
			// 
			this.groupBox1.Location = ( ( System.Drawing.Point )( resources.GetObject( "groupBox1.Location" ) ) );
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = ( ( System.Drawing.Size )( resources.GetObject( "groupBox1.Size" ) ) );
			this.groupBox1.TabIndex = ( ( int )( resources.GetObject( "groupBox1.TabIndex" ) ) );
            this.groupBox1.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FRGroupTitle) == null) ? resources.GetString("groupBox1.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FRGroupTitle);
			// 
			// rdbDocument
			// 
			this.rdbDocument.Name = "rdbDocument";
			this.rdbDocument.Size = ( ( System.Drawing.Size )( resources.GetObject( "rdbDocument.Size" ) ) );
			this.rdbDocument.CheckedChanged += new System.EventHandler( this.rdbSelection_CheckedChanged );
			// 
			// rdbSelection
			// 
			this.rdbSelection.Enabled = ( ( bool )( resources.GetObject( "rdbSelection.Enabled" ) ) );
			this.rdbSelection.Name = "rdbSelection";
			this.rdbSelection.Size = ( ( System.Drawing.Size )( resources.GetObject( "rdbSelection.Size" ) ) );
            this.rdbSelection.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FRrdbSelection) == null) ? resources.GetString("rdbSelection.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FRrdbSelection);
			this.rdbSelection.Visible = ( ( bool )( resources.GetObject( "rdbSelection.Visible" ) ) );
			this.rdbSelection.CheckedChanged += new System.EventHandler( this.rdbSelection_CheckedChanged );
			// 
			// lblReplace
			// 
			this.lblReplace.AccessibleDescription = resources.GetString( "lblReplace.AccessibleDescription" );
			this.lblReplace.AccessibleName = resources.GetString( "lblReplace.AccessibleName" );
			this.lblReplace.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "lblReplace.Anchor" ) ) );
			this.lblReplace.AutoSize = true;
			this.lblReplace.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "lblReplace.Dock" ) ) );
			this.lblReplace.Enabled = ( ( bool )( resources.GetObject( "lblReplace.Enabled" ) ) );
			this.lblReplace.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.lblReplace.Font = ( ( System.Drawing.Font )( resources.GetObject( "lblReplace.Font" ) ) );
			this.lblReplace.Image = ( ( System.Drawing.Image )( resources.GetObject( "lblReplace.Image" ) ) );
			this.lblReplace.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblReplace.ImageAlign" ) ) );
			this.lblReplace.ImageIndex = ( ( int )( resources.GetObject( "lblReplace.ImageIndex" ) ) );
			this.lblReplace.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "lblReplace.ImeMode" ) ) );
			this.lblReplace.Location = ( ( System.Drawing.Point )( resources.GetObject( "lblReplace.Location" ) ) );
			this.lblReplace.Name = "lblReplace";
			this.lblReplace.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "lblReplace.RightToLeft" ) ) );
			this.lblReplace.Size = ( ( System.Drawing.Size )( resources.GetObject( "lblReplace.Size" ) ) );
			this.lblReplace.TabIndex = ( ( int )( resources.GetObject( "lblReplace.TabIndex" ) ) );
            this.lblReplace.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FRlblReplace) == null) ? resources.GetString("lblReplace.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FRlblReplace);
			this.lblReplace.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblReplace.TextAlign" ) ) );
			this.lblReplace.Visible = ( ( bool )( resources.GetObject( "lblReplace.Visible" ) ) );
			// 
			// btnReplace
			// 
            this.btnReplace.AutoSize = true;
			this.btnReplace.AccessibleDescription = resources.GetString( "btnReplace.AccessibleDescription" );
			this.btnReplace.AccessibleName = resources.GetString( "btnReplace.AccessibleName" );
			this.btnReplace.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "btnReplace.BackgroundImage" ) ) );
			this.btnReplace.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "btnReplace.Dock" ) ) );
			this.btnReplace.Enabled = ( ( bool )( resources.GetObject( "btnReplace.Enabled" ) ) );
			this.btnReplace.FlatStyle = ( ( System.Windows.Forms.FlatStyle )( resources.GetObject( "btnReplace.FlatStyle" ) ) );
			this.btnReplace.Font = ( ( System.Drawing.Font )( resources.GetObject( "btnReplace.Font" ) ) );
			this.btnReplace.Image = ( ( System.Drawing.Image )( resources.GetObject( "btnReplace.Image" ) ) );
			this.btnReplace.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnReplace.ImageAlign" ) ) );
			this.btnReplace.ImageIndex = ( ( int )( resources.GetObject( "btnReplace.ImageIndex" ) ) );
			this.btnReplace.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "btnReplace.ImeMode" ) ) );
			this.btnReplace.Location = ( ( System.Drawing.Point )( resources.GetObject( "btnReplace.Location" ) ) );
			this.btnReplace.Name = "btnReplace";
			this.btnReplace.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "btnReplace.RightToLeft" ) ) );
			this.btnReplace.Size = ( ( System.Drawing.Size )( resources.GetObject( "btnReplace.Size" ) ) );
			this.btnReplace.TabIndex = ( ( int )( resources.GetObject( "btnReplace.TabIndex" ) ) );
            this.btnReplace.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FRbtnReplace) == null) ? resources.GetString("btnReplace.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FRbtnReplace);
			this.btnReplace.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnReplace.TextAlign" ) ) );
			this.btnReplace.Visible = ( ( bool )( resources.GetObject( "btnReplace.Visible" ) ) );
			this.btnReplace.Click += new System.EventHandler( this.Replace_Text );
			// 
			// btnReplaceAll
			// 
            this.btnReplaceAll.AutoSize = true;
			this.btnReplaceAll.AccessibleDescription = resources.GetString( "btnReplaceAll.AccessibleDescription" );
			this.btnReplaceAll.AccessibleName = resources.GetString( "btnReplaceAll.AccessibleName" );
			this.btnReplaceAll.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "btnReplaceAll.BackgroundImage" ) ) );
			this.btnReplaceAll.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "btnReplaceAll.Dock" ) ) );
			this.btnReplaceAll.Enabled = ( ( bool )( resources.GetObject( "btnReplaceAll.Enabled" ) ) );
			this.btnReplaceAll.FlatStyle = ( ( System.Windows.Forms.FlatStyle )( resources.GetObject( "btnReplaceAll.FlatStyle" ) ) );
			this.btnReplaceAll.Font = ( ( System.Drawing.Font )( resources.GetObject( "btnReplaceAll.Font" ) ) );
			this.btnReplaceAll.Image = ( ( System.Drawing.Image )( resources.GetObject( "btnReplaceAll.Image" ) ) );
			this.btnReplaceAll.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnReplaceAll.ImageAlign" ) ) );
			this.btnReplaceAll.ImageIndex = ( ( int )( resources.GetObject( "btnReplaceAll.ImageIndex" ) ) );
			this.btnReplaceAll.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "btnReplaceAll.ImeMode" ) ) );
			this.btnReplaceAll.Location = ( ( System.Drawing.Point )( resources.GetObject( "btnReplaceAll.Location" ) ) );
			this.btnReplaceAll.Name = "btnReplaceAll";
			this.btnReplaceAll.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "btnReplaceAll.RightToLeft" ) ) );
			this.btnReplaceAll.Size = ( ( System.Drawing.Size )( resources.GetObject( "btnReplaceAll.Size" ) ) );
			this.btnReplaceAll.TabIndex = ( ( int )( resources.GetObject( "btnReplaceAll.TabIndex" ) ) );
            this.btnReplaceAll.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FRbtnReplaceAll) == null) ? resources.GetString("btnReplaceAll.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FRbtnReplaceAll);
			this.btnReplaceAll.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnReplaceAll.TextAlign" ) ) );
			this.btnReplaceAll.Visible = ( ( bool )( resources.GetObject( "btnReplaceAll.Visible" ) ) );
			this.btnReplaceAll.Click += new System.EventHandler( this.Replace_Text );
			// 
			// cmbReplace
			// 
			this.cmbReplace.AccessibleDescription = resources.GetString( "cmbReplace.AccessibleDescription" );
			this.cmbReplace.AccessibleName = resources.GetString( "cmbReplace.AccessibleName" );
			this.cmbReplace.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "cmbReplace.BackgroundImage" ) ) );
			this.cmbReplace.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "cmbReplace.Dock" ) ) );
			this.cmbReplace.Enabled = ( ( bool )( resources.GetObject( "cmbReplace.Enabled" ) ) );
			this.cmbReplace.Font = ( ( System.Drawing.Font )( resources.GetObject( "cmbReplace.Font" ) ) );
			this.cmbReplace.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "cmbReplace.ImeMode" ) ) );
			this.cmbReplace.IntegralHeight = ( ( bool )( resources.GetObject( "cmbReplace.IntegralHeight" ) ) );
			this.cmbReplace.ItemHeight = ( ( int )( resources.GetObject( "cmbReplace.ItemHeight" ) ) );
			this.cmbReplace.Location = ( ( System.Drawing.Point )( resources.GetObject( "cmbReplace.Location" ) ) );
			this.cmbReplace.MaxDropDownItems = ( ( int )( resources.GetObject( "cmbReplace.MaxDropDownItems" ) ) );
			this.cmbReplace.MaxLength = ( ( int )( resources.GetObject( "cmbReplace.MaxLength" ) ) );
			this.cmbReplace.Name = "cmbReplace";
			this.cmbReplace.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "cmbReplace.RightToLeft" ) ) );
			this.cmbReplace.Size = ( ( System.Drawing.Size )( resources.GetObject( "cmbReplace.Size" ) ) );
			this.cmbReplace.TabIndex = ( ( int )( resources.GetObject( "cmbReplace.TabIndex" ) ) );
			this.cmbReplace.Text = resources.GetString( "cmbReplace.Text" );
			this.cmbReplace.Visible = ( ( bool )( resources.GetObject( "cmbReplace.Visible" ) ) );
			this.cmbReplace.TextChanged += new System.EventHandler( this.cmbReplace_TextChanged );
			this.cmbReplace.SelectedIndexChanged += new System.EventHandler( this.cmbReplace_SelectedIndexChanged );
			// 
			// btnReplaceTemplates
			// 
            this.btnReplaceAll.AutoSize = true;
			this.btnReplaceTemplates.AccessibleDescription = resources.GetString( "btnReplaceTemplates.AccessibleDescription" );
			this.btnReplaceTemplates.AccessibleName = resources.GetString( "btnReplaceTemplates.AccessibleName" );
			this.btnReplaceTemplates.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "btnReplaceTemplates.BackgroundImage" ) ) );
			this.btnReplaceTemplates.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "btnReplaceTemplates.Dock" ) ) );
			this.btnReplaceTemplates.Enabled = ( ( bool )( resources.GetObject( "btnReplaceTemplates.Enabled" ) ) );
			this.btnReplaceTemplates.FlatStyle = ( ( System.Windows.Forms.FlatStyle )( resources.GetObject( "btnReplaceTemplates.FlatStyle" ) ) );
			this.btnReplaceTemplates.Font = ( ( System.Drawing.Font )( resources.GetObject( "btnReplaceTemplates.Font" ) ) );
			this.btnReplaceTemplates.Image = ( ( System.Drawing.Image )( resources.GetObject( "btnReplaceTemplates.Image" ) ) );
			this.btnReplaceTemplates.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnReplaceTemplates.ImageAlign" ) ) );
			this.btnReplaceTemplates.ImageIndex = ( ( int )( resources.GetObject( "btnReplaceTemplates.ImageIndex" ) ) );
			this.btnReplaceTemplates.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "btnReplaceTemplates.ImeMode" ) ) );
			this.btnReplaceTemplates.Location = ( ( System.Drawing.Point )( resources.GetObject( "btnReplaceTemplates.Location" ) ) );
			this.btnReplaceTemplates.Name = "btnReplaceTemplates";
			this.btnReplaceTemplates.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "btnReplaceTemplates.RightToLeft" ) ) );
			this.btnReplaceTemplates.Size = ( ( System.Drawing.Size )( resources.GetObject( "btnReplaceTemplates.Size" ) ) );
			this.btnReplaceTemplates.TabIndex = ( ( int )( resources.GetObject( "btnReplaceTemplates.TabIndex" ) ) );
			this.btnReplaceTemplates.Text = resources.GetString( "btnReplaceTemplates.Text" );
			this.btnReplaceTemplates.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "btnReplaceTemplates.TextAlign" ) ) );
			this.btnReplaceTemplates.Visible = ( ( bool )( resources.GetObject( "btnReplaceTemplates.Visible" ) ) );
			this.btnReplaceTemplates.Click += new System.EventHandler( this.btnReplaceTemplates_Click );
			// 
			// frmReplaceDialog
			// 
			this.AccessibleDescription = resources.GetString( "$this.AccessibleDescription" );
			this.AccessibleName = resources.GetString( "$this.AccessibleName" );
			this.AutoScaleBaseSize = ( ( System.Drawing.Size )( resources.GetObject( "$this.AutoScaleBaseSize" ) ) );
			this.AutoScroll = ( ( bool )( resources.GetObject( "$this.AutoScroll" ) ) );
			this.AutoScrollMargin = ( ( System.Drawing.Size )( resources.GetObject( "$this.AutoScrollMargin" ) ) );
			this.AutoScrollMinSize = ( ( System.Drawing.Size )( resources.GetObject( "$this.AutoScrollMinSize" ) ) );
			this.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "$this.BackgroundImage" ) ) );
			this.ClientSize = ( ( System.Drawing.Size )( resources.GetObject( "$this.ClientSize" ) ) );
			this.Controls.Add( this.btnReplaceTemplates );
			this.Controls.Add( this.cmbReplace );
			this.Controls.Add( this.btnReplace );
			this.Controls.Add( this.lblReplace );
			this.Controls.Add( this.btnReplaceAll );
			this.Enabled = ( ( bool )( resources.GetObject( "$this.Enabled" ) ) );
			this.Font = ( ( System.Drawing.Font )( resources.GetObject( "$this.Font" ) ) );
			this.Icon = ( ( System.Drawing.Icon )( resources.GetObject( "$this.Icon" ) ) );
			this.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "$this.ImeMode" ) ) );
			this.Location = ( ( System.Drawing.Point )( resources.GetObject( "$this.Location" ) ) );
			this.MaximumSize = ( ( System.Drawing.Size )( resources.GetObject( "$this.MaximumSize" ) ) );
			this.MinimumSize = ( ( System.Drawing.Size )( resources.GetObject( "$this.MinimumSize" ) ) );
			this.Name = "frmReplaceDialog";
			this.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "$this.RightToLeft" ) ) );
			this.RightToLeftLayout = true;
			this.StartPosition = ( ( System.Windows.Forms.FormStartPosition )( resources.GetObject( "$this.StartPosition" ) ) );
            this.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FRTitle) == null) ? resources.GetString("$this.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FRTitle);
			this.Controls.SetChildIndex( this.chkRegular, 0 );
			this.Controls.SetChildIndex( this.chkUp, 0 );
			this.Controls.SetChildIndex( this.chkHidden, 0 );
			this.Controls.SetChildIndex( this.chkWholeWord, 0 );
			this.Controls.SetChildIndex( this.lblFind, 0 );
			this.Controls.SetChildIndex( this.cmbFind, 0 );
			this.Controls.SetChildIndex( this.chkCase, 0 );
			this.Controls.SetChildIndex( this.btnFind, 0 );
			this.Controls.SetChildIndex( this.btnTempaltes, 0 );
			this.Controls.SetChildIndex( this.groupBox1, 0 );
			this.Controls.SetChildIndex( this.btnMarkAll, 0 );
			this.Controls.SetChildIndex( this.btnClose, 0 );
			this.Controls.SetChildIndex( this.btnReplaceAll, 0 );
			this.Controls.SetChildIndex( this.lblReplace, 0 );
			this.Controls.SetChildIndex( this.btnReplace, 0 );
			this.Controls.SetChildIndex( this.cmbReplace, 0 );
			this.Controls.SetChildIndex( this.btnReplaceTemplates, 0 );
			this.groupBox1.ResumeLayout( false );
			this.ResumeLayout( false );

		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Invokes replacing process.
		/// </summary>
		public void ReplaceNext()
		{
			if( cmbFind.Text.Length != 0 )
			{
				this.SearchText = cmbFind.Text;
				m_replaceText = cmbReplace.Text;
				string replaceText = cmbReplace.Text;

				if( !cmbFind.Items.Contains( cmbFind.Text ) )
				{
					this.History.Insert( 0, cmbFind.Text );
					cmbFind.Items.Insert( 0, cmbFind.Text );
				}

				UpdateReplaceHistory();

				Regex searchRegex = CreateSearchRegex();
				if( searchRegex != null )
				{
					bool bReplaceAll = ( m_searchType == SearchType.MarkAll );
					bool bSearchInHidden = ( ( m_attributes & SearchAttributes.SearchHidden ) == SearchAttributes.SearchHidden );
					bool bSearchUp = ( ( ( m_attributes & SearchAttributes.SearchUp ) == SearchAttributes.SearchUp ) && !bReplaceAll );
					bool bUseRegExp = ( ( m_attributes & SearchAttributes.UseRegexp ) == SearchAttributes.UseRegexp );
					IParsePoint position = ( bReplaceAll ) ? m_control.Parser.BaseStream.GetParsePoint( 1, 1, true ) : m_control.GetRealCursorPosition();

					if( bUseRegExp )
					{
						replaceText = Regex.Replace( replaceText, @"\\n", Regex.Unescape( m_control.Parser.BaseStream.NewLineStr ) );
					}

					try
					{
						m_control.UndoGroupOpen();

						if( m_bInSelection )
						{
							string textSelected = m_control.SelectedText;
							m_control.SelectedText = searchRegex.Replace( textSelected, replaceText );
						}
						else
						{
							FindResult res;
							bool bMoreProcessing = false;
							if(bReplaceAll&&chkWrap.Checked)
								m_control.CurrentLine = m_control.CurrentLine = 1;
							do
							{
								bMoreProcessing = false;
								string text = m_control.SelectedText;
								Match match = searchRegex.Match( text );
								if( !match.Success || match.Value != text )
								{
									res = m_control.FindRegex( position, searchRegex, bSearchInHidden, bSearchUp );
									m_control.MarkSearchResult( res, bSearchUp );
									if( !res.Result.Success )
									{
										break;
									}
								}
								else
								{
									m_control.SelectedText = ( bUseRegExp ) ? ( searchRegex.Replace( text, replaceText ) ) : ( replaceText );
									IParsePoint pointCursor = m_control.GetRealCursorPosition();
									position = m_control.Parser.BaseStream.GetParsePoint( pointCursor.Line, ( int )pointCursor.Position, true );
									if( position == null )
									{
										if( pointCursor.Line < m_control.Parser.BaseStream.LinesCount )
										{
											position = m_control.Parser.BaseStream.GetParsePoint( pointCursor.Line + 1, 1, true );
										}

										if( position == null )
										{
											break;
										}
									}

									res = m_control.FindRegex( position, searchRegex, bSearchInHidden, bSearchUp );
									m_control.MarkSearchResult( res, bSearchUp );
									if( !res.Result.Success )
									{
										break;
									}
                                    else
                                        if(m_control != null)
                                            m_control.RaiseFindAndReplaceEvent();
								}
							}
							while( bReplaceAll || bMoreProcessing );
						}
					}
					finally
					{
						m_control.UndoGroupClose();
					}
				}
			}
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Overriden. Disables/enables regexp buttons.
		/// </summary>
		protected override void ToggleButtons()
		{
			base.ToggleButtons();
			btnReplaceTemplates.Enabled = chkRegular.Checked;
		}
		/// <summary>
		/// Overriden. Enables/disables replace buttons.
		/// </summary>
		protected override void ToggleElementsEnable()
		{
			base.ToggleElementsEnable();

			btnReplaceAll.Enabled = btnMarkAll.Enabled = ( cmbFind.Text.Length != 0 );
			btnReplace.Enabled = btnFind.Enabled = ( cmbFind.Text.Length != 0 && !rdbSelection.Checked );
		}
        private void LocalizeReplaceDialog()
        {
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmReplaceDialog));
            this.chkCase.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FRchkCase) == null) ? resources.GetString("chkCase.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FRchkCase);
            this.btnClose.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FRbtnClose) == null) ? resources.GetString("btnClose.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FRbtnClose);
            this.lblFind.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FRlblFind) == null) ? resources.GetString("lblFind.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FRlblFind);
            this.groupBox1.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FRGroupTitle) == null) ? resources.GetString("groupBox1.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FRGroupTitle);
            this.rdbSelection.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FRrdbSelection) == null) ? resources.GetString("rdbSelection.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FRrdbSelection);
            this.lblReplace.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FRlblReplace) == null) ? resources.GetString("lblReplace.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FRlblReplace);
            this.btnReplace.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FRbtnReplace) == null) ? resources.GetString("btnReplace.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FRbtnReplace);
            this.btnReplaceAll.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FRbtnReplaceAll) == null) ? resources.GetString("btnReplaceAll.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FRbtnReplaceAll);
            this.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FRTitle) == null) ? resources.GetString("$this.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FRTitle);
        }
		/// <summary>
		/// Returns active comboBox for editing data.
		/// </summary>
		/// <returns>Active combo box.</returns>
		protected override ComboBox GetActiveComboBox()
		{
			return ( m_activeCombo == cmbReplace ) ? m_activeCombo : cmbFind;
		}
		/// <summary>
		/// Overriden. Raises when dialog shows.
		/// </summary>
		/// <param name="e">EventArgs.</param>
		protected override void OnActivated( EventArgs e )
		{
			base.OnActivated( e );
            LocalizeReplaceDialog();
            int x=0,y=0,z = 0;
            this.btnReplaceTemplates.Size = this.btnTempaltes.Size;
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is CheckBox)
                {
                    if (ctrl.Width > x)
                        x = ctrl.Width;
                }
            }
            if (this.lblFind.Width > this.lblReplace.Width)
                z = this.lblFind.Location.X + this.lblFind.Width;
            else
                z = this.lblReplace.Location.X + this.lblReplace.Width;
            this.cmbFind.Location = new Point(z + 5, this.cmbFind.Bounds.Y);
            this.cmbReplace.Location = new Point(z+5, this.cmbReplace.Bounds.Y);
            if (this.cmbFind.Location.X > this.cmbReplace.Location.X)
                y = this.cmbFind.Location.X + this.cmbFind.Width;
            else
                y = this.cmbReplace.Location.X + this.cmbReplace.Width;
            this.btnTempaltes.Location = new Point(y+2, this.cmbFind.Bounds.Y);
            this.groupBox1.Location = new Point(x,this.groupBox1.Location.Y);
            if ((this.btnTempaltes.Location.X + this.btnTempaltes.Width) > (this.groupBox1.Location.X + this.groupBox1.Width))
                x = this.btnTempaltes.Location.X + this.btnTempaltes.Width + 2;
            else
                x = this.groupBox1.Location.X + this.groupBox1.Width + 2;
            this.btnReplaceTemplates.Location = new Point(y+2,this.btnReplaceTemplates.Bounds.Y);
            this.btnReplace.Location = new Point(x, this.btnReplace.Bounds.Y);
            this.btnReplaceAll.Location = new Point(x, this.btnReplaceAll.Bounds.Y);
            this.btnMarkAll.Location = new Point(x, this.btnMarkAll.Bounds.Y);
            this.btnClose.Location = new Point(x, this.btnClose.Bounds.Y);
            this.btnFind.Location = new Point(x, this.btnTempaltes.Bounds.Y);
            this.MinimumSize = new Size(this.Width, this.Height);
			m_bIndexChaged = false;
			cmbReplace.Items.Clear();
			cmbReplace.Items.AddRange( m_replaceHistory.ToArray() );
			if( m_replaceHistory.Count > 0 )
			{
				cmbReplace.Text = ( m_replaceText == null || m_replaceText == string.Empty ) ? ( ( string )m_replaceHistory[ 0 ] ) : ( m_replaceText );
			}

			m_bIndexChaged = true;
			ToggleElementsEnable();
		}
		/// <summary>
		/// Invokes searching process.
		/// </summary>
		/// <returns>Find result.</returns>
		public override FindNextResult FindNext()
		{
			UpdateReplaceHistory();
			return base.FindNext();
		}
		#endregion

		#region Nonpublic Methods
		/// <summary>
		/// Inserts new item to the replace histore if needed.
		/// </summary>
		private void UpdateReplaceHistory()
		{
			if( !cmbReplace.Items.Contains( cmbReplace.Text ) )
			{
				m_replaceHistory.Insert( 0, cmbReplace.Text );
				cmbReplace.Items.Insert( 0, cmbReplace.Text );
			}
		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// Shows context menu.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnReplaceTemplates_Click( object sender, System.EventArgs e )
		{
			if( sender == btnReplaceTemplates )
			{
				Button regButton = sender as Button;
				m_activeCombo = cmbReplace;
				Point menuPoint = regButton.PointToClient( Control.MousePosition );
				cmnTemplates.Show( regButton, menuPoint );
			}
		}
		/// <summary>
		/// Defines search type and invokes replacing process.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Replace_Text( object sender, System.EventArgs e )
		{
			m_searchType = ( sender == btnReplace ) ? ( SearchType.FindNext ) : ( SearchType.MarkAll );
			ReplaceNext();
		}
		/// <summary>
		/// Raises when item from replace history is selected.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cmbReplace_SelectedIndexChanged( object sender, System.EventArgs e )
		{
			if( m_bIndexChaged )
			{
				string text = ( string )cmbReplace.Items[ cmbReplace.SelectedIndex ];
				if( m_replaceHistory.Contains( text ) )
				{
					int index = cmbReplace.SelectedIndex;
					m_replaceHistory.RemoveAt( index );
					m_replaceHistory.Insert( 0, text );
				}
			}
		}
		/// <summary>
		/// Enables/disables replace buttons.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cmbReplace_TextChanged( object sender, System.EventArgs e )
		{
			ToggleElementsEnable();
		}
		/// <summary>
		/// Enables/disables replace buttons.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void rdbSelection_CheckedChanged( object sender, System.EventArgs e )
		{
			ToggleElementsEnable();
		}
		#endregion
	}
}