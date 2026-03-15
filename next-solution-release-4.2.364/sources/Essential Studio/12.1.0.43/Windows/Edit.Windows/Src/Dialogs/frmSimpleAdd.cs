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

using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Syncfusion.Windows.Forms.Edit.Dialogs
{
	/// <summary>
	/// Class for adding simple text item somewhere.
	/// </summary>
	public class frmSimpleAdd
		: System.Windows.Forms.Form
	{
		#region Constants
		/// <summary>
		/// Step between control
		/// </summary>
		private const int DEF_STEP = 8;
		/// <summary>
		/// Error message.
		/// </summary>
		private const string DEF_ERROR = "Entered value is invalid.";
		#endregion

		#region Fields
		/// <summary>
		/// Regex validator for checking input.
		/// </summary>
		private Regex m_validator;
		/// <summary>
		/// Color for valid text.
		/// </summary>
		private Color m_clrValid = Control.DefaultForeColor;
		/// <summary>
		/// Color for invalid text.
		/// </summary>
		private Color m_clrInvalid = Color.Red;
		/// <summary>
		/// Reverse validation.
		/// </summary>
		private bool m_bReverse;
		/// <summary>
		/// Text is valid or not.
		/// </summary>
		private bool m_bSuccess;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets form input control value
		/// </summary>
		[Category( "Data" )
	 , Browsable( true )
	 , DefaultValue( null )
	 , Description( "DescrInputControl" )]
		public string Value
		{
			get
			{
				return txtValue.Text;
			}
			set
			{
				txtValue.Text = value;
			}
		}
		/// <summary>
		/// Gets or sets example of input value for better understanding how to enter value. Needed for better user GUI.
		/// </summary>
		[Category( "Appearance" )
	 , Browsable( true )
	 , DefaultValue( null )
	 , Description( "DescrInputExample" )]
		public string Example
		{
			get
			{
				return lblValueExample.Text;
			}
			set
			{
				lblValueExample.Text = value;
				UpdateFormSize();
			}
		}
		/// <summary>
		/// Gets or sets Regular expression which validate input control data
		/// </summary>
		[Category( "Behavior" )
	 , TypeConverter( typeof( RegexConverter ) )
	 , RefreshProperties( RefreshProperties.All )
	 , Description( "DescrValidationRegex" )]
		public Regex Validator
		{
			get
			{
				return m_validator;
			}
			set
			{
				m_validator = value;
			}
		}
		/// <summary>
		/// Gets or sets color of font in input control for valid value
		/// </summary>
		[Category( "Appearance" )
	 , Browsable( true )
	 , Description( "Get or Set color of valid inputed value" )]
		public Color ValidColor
		{
			get
			{
				return m_clrValid;
			}
			set
			{
				m_clrValid = value;
				UpdateControlColor();
			}
		}
		/// <summary>
		/// Gets or sets color of font in input control for invalid value
		/// </summary>
		[Category( "Appearance" )
	 , Browsable( true )
	 , DefaultValue( typeof( Color ), "Red" )
	 , Description( "Get or Set invalid value color" )]
		public Color InvalidColor
		{
			get
			{
				return m_clrInvalid;
			}
			set
			{
				m_clrInvalid = value;
				UpdateControlColor();
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

		#region Form controls
		private System.Windows.Forms.Label lblValue;
		private System.Windows.Forms.TextBox txtValue;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label lblExample;
		private System.Windows.Forms.Label lblValueExample;
		private System.Windows.Forms.ErrorProvider errorPrv;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		#endregion

		#region Initialize/Finalize methods
		/// <summary>
		/// Creates new instance of frmSimpleAdd.
		/// </summary>
		public frmSimpleAdd()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			EnableDoubleBuffering();

			m_bSuccess = true;
		}
		/// <summary>
		/// Creates new dialog object.
		/// </summary>
		/// <param name="value">Value string.</param>
		/// <param name="example">Example string.</param>
		/// <param name="validator">Validator.</param>
		public frmSimpleAdd( string value, string example, Regex validator )
			: this()
		{
			this.Value = value;
			this.Example = example;
			this.Validator = validator;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager( typeof( frmSimpleAdd ) );
			this.lblValue = new System.Windows.Forms.Label();
			this.txtValue = new System.Windows.Forms.TextBox();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.lblExample = new System.Windows.Forms.Label();
			this.lblValueExample = new System.Windows.Forms.Label();
			this.errorPrv = new System.Windows.Forms.ErrorProvider();
			this.SuspendLayout();
			// 
			// lblValue
			// 
			this.lblValue.AccessibleDescription = resources.GetString( "lblValue.AccessibleDescription" );
			this.lblValue.AccessibleName = resources.GetString( "lblValue.AccessibleName" );
			this.lblValue.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "lblValue.Anchor" ) ) );
			this.lblValue.AutoSize = ( ( bool )( resources.GetObject( "lblValue.AutoSize" ) ) );
			this.lblValue.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "lblValue.Dock" ) ) );
			this.lblValue.Enabled = ( ( bool )( resources.GetObject( "lblValue.Enabled" ) ) );
			this.errorPrv.SetError( this.lblValue, resources.GetString( "lblValue.Error" ) );
			this.lblValue.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.lblValue.Font = ( ( System.Drawing.Font )( resources.GetObject( "lblValue.Font" ) ) );
			this.errorPrv.SetIconAlignment( this.lblValue, ( ( System.Windows.Forms.ErrorIconAlignment )( resources.GetObject( "lblValue.IconAlignment" ) ) ) );
			this.errorPrv.SetIconPadding( this.lblValue, ( ( int )( resources.GetObject( "lblValue.IconPadding" ) ) ) );
			this.lblValue.Image = ( ( System.Drawing.Image )( resources.GetObject( "lblValue.Image" ) ) );
			this.lblValue.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblValue.ImageAlign" ) ) );
			this.lblValue.ImageIndex = ( ( int )( resources.GetObject( "lblValue.ImageIndex" ) ) );
			this.lblValue.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "lblValue.ImeMode" ) ) );
			this.lblValue.Location = ( ( System.Drawing.Point )( resources.GetObject( "lblValue.Location" ) ) );
			this.lblValue.Name = "lblValue";
			this.lblValue.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "lblValue.RightToLeft" ) ) );
			this.lblValue.Size = ( ( System.Drawing.Size )( resources.GetObject( "lblValue.Size" ) ) );
			this.lblValue.TabIndex = ( ( int )( resources.GetObject( "lblValue.TabIndex" ) ) );
			this.lblValue.Text = resources.GetString( "lblValue.Text" );
			this.lblValue.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblValue.TextAlign" ) ) );
			this.lblValue.Visible = ( ( bool )( resources.GetObject( "lblValue.Visible" ) ) );
			// 
			// txtValue
			// 
			this.txtValue.AccessibleDescription = resources.GetString( "txtValue.AccessibleDescription" );
			this.txtValue.AccessibleName = resources.GetString( "txtValue.AccessibleName" );
			this.txtValue.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "txtValue.Anchor" ) ) );
			this.txtValue.AutoSize = ( ( bool )( resources.GetObject( "txtValue.AutoSize" ) ) );
			this.txtValue.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "txtValue.BackgroundImage" ) ) );
			this.txtValue.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "txtValue.Dock" ) ) );
			this.txtValue.Enabled = ( ( bool )( resources.GetObject( "txtValue.Enabled" ) ) );
			this.errorPrv.SetError( this.txtValue, resources.GetString( "txtValue.Error" ) );
			this.txtValue.Font = ( ( System.Drawing.Font )( resources.GetObject( "txtValue.Font" ) ) );
			this.errorPrv.SetIconAlignment( this.txtValue, ( ( System.Windows.Forms.ErrorIconAlignment )( resources.GetObject( "txtValue.IconAlignment" ) ) ) );
			this.errorPrv.SetIconPadding( this.txtValue, ( ( int )( resources.GetObject( "txtValue.IconPadding" ) ) ) );
			this.txtValue.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "txtValue.ImeMode" ) ) );
			this.txtValue.Location = ( ( System.Drawing.Point )( resources.GetObject( "txtValue.Location" ) ) );
			this.txtValue.MaxLength = ( ( int )( resources.GetObject( "txtValue.MaxLength" ) ) );
			this.txtValue.Multiline = ( ( bool )( resources.GetObject( "txtValue.Multiline" ) ) );
			this.txtValue.Name = "txtValue";
			this.txtValue.PasswordChar = ( ( char )( resources.GetObject( "txtValue.PasswordChar" ) ) );
			this.txtValue.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "txtValue.RightToLeft" ) ) );
			this.txtValue.ScrollBars = ( ( System.Windows.Forms.ScrollBars )( resources.GetObject( "txtValue.ScrollBars" ) ) );
			this.txtValue.Size = ( ( System.Drawing.Size )( resources.GetObject( "txtValue.Size" ) ) );
			this.txtValue.TabIndex = ( ( int )( resources.GetObject( "txtValue.TabIndex" ) ) );
			this.txtValue.Text = resources.GetString( "txtValue.Text" );
			this.txtValue.TextAlign = ( ( System.Windows.Forms.HorizontalAlignment )( resources.GetObject( "txtValue.TextAlign" ) ) );
			this.txtValue.Visible = ( ( bool )( resources.GetObject( "txtValue.Visible" ) ) );
			this.txtValue.WordWrap = ( ( bool )( resources.GetObject( "txtValue.WordWrap" ) ) );
			this.txtValue.Validating += new System.ComponentModel.CancelEventHandler( this.txtValue_Validating );
			this.txtValue.TextChanged += new System.EventHandler( this.txtValue_TextChanged );
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
			this.errorPrv.SetError( this.btnOK, resources.GetString( "btnOK.Error" ) );
			this.btnOK.FlatStyle = ( ( System.Windows.Forms.FlatStyle )( resources.GetObject( "btnOK.FlatStyle" ) ) );
			this.btnOK.Font = ( ( System.Drawing.Font )( resources.GetObject( "btnOK.Font" ) ) );
			this.errorPrv.SetIconAlignment( this.btnOK, ( ( System.Windows.Forms.ErrorIconAlignment )( resources.GetObject( "btnOK.IconAlignment" ) ) ) );
			this.errorPrv.SetIconPadding( this.btnOK, ( ( int )( resources.GetObject( "btnOK.IconPadding" ) ) ) );
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
			this.errorPrv.SetError( this.btnCancel, resources.GetString( "btnCancel.Error" ) );
			this.btnCancel.FlatStyle = ( ( System.Windows.Forms.FlatStyle )( resources.GetObject( "btnCancel.FlatStyle" ) ) );
			this.btnCancel.Font = ( ( System.Drawing.Font )( resources.GetObject( "btnCancel.Font" ) ) );
			this.errorPrv.SetIconAlignment( this.btnCancel, ( ( System.Windows.Forms.ErrorIconAlignment )( resources.GetObject( "btnCancel.IconAlignment" ) ) ) );
			this.errorPrv.SetIconPadding( this.btnCancel, ( ( int )( resources.GetObject( "btnCancel.IconPadding" ) ) ) );
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
			// lblExample
			// 
			this.lblExample.AccessibleDescription = resources.GetString( "lblExample.AccessibleDescription" );
			this.lblExample.AccessibleName = resources.GetString( "lblExample.AccessibleName" );
			this.lblExample.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "lblExample.Anchor" ) ) );
			this.lblExample.AutoSize = ( ( bool )( resources.GetObject( "lblExample.AutoSize" ) ) );
			this.lblExample.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "lblExample.Dock" ) ) );
			this.lblExample.Enabled = ( ( bool )( resources.GetObject( "lblExample.Enabled" ) ) );
			this.errorPrv.SetError( this.lblExample, resources.GetString( "lblExample.Error" ) );
			this.lblExample.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.lblExample.Font = ( ( System.Drawing.Font )( resources.GetObject( "lblExample.Font" ) ) );
			this.errorPrv.SetIconAlignment( this.lblExample, ( ( System.Windows.Forms.ErrorIconAlignment )( resources.GetObject( "lblExample.IconAlignment" ) ) ) );
			this.errorPrv.SetIconPadding( this.lblExample, ( ( int )( resources.GetObject( "lblExample.IconPadding" ) ) ) );
			this.lblExample.Image = ( ( System.Drawing.Image )( resources.GetObject( "lblExample.Image" ) ) );
			this.lblExample.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblExample.ImageAlign" ) ) );
			this.lblExample.ImageIndex = ( ( int )( resources.GetObject( "lblExample.ImageIndex" ) ) );
			this.lblExample.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "lblExample.ImeMode" ) ) );
			this.lblExample.Location = ( ( System.Drawing.Point )( resources.GetObject( "lblExample.Location" ) ) );
			this.lblExample.Name = "lblExample";
			this.lblExample.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "lblExample.RightToLeft" ) ) );
			this.lblExample.Size = ( ( System.Drawing.Size )( resources.GetObject( "lblExample.Size" ) ) );
			this.lblExample.TabIndex = ( ( int )( resources.GetObject( "lblExample.TabIndex" ) ) );
			this.lblExample.Text = resources.GetString( "lblExample.Text" );
			this.lblExample.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblExample.TextAlign" ) ) );
			this.lblExample.Visible = ( ( bool )( resources.GetObject( "lblExample.Visible" ) ) );
			// 
			// lblValueExample
			// 
			this.lblValueExample.AccessibleDescription = resources.GetString( "lblValueExample.AccessibleDescription" );
			this.lblValueExample.AccessibleName = resources.GetString( "lblValueExample.AccessibleName" );
			this.lblValueExample.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "lblValueExample.Anchor" ) ) );
			this.lblValueExample.AutoSize = ( ( bool )( resources.GetObject( "lblValueExample.AutoSize" ) ) );
			this.lblValueExample.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "lblValueExample.Dock" ) ) );
			this.lblValueExample.Enabled = ( ( bool )( resources.GetObject( "lblValueExample.Enabled" ) ) );
			this.errorPrv.SetError( this.lblValueExample, resources.GetString( "lblValueExample.Error" ) );
			this.lblValueExample.Font = ( ( System.Drawing.Font )( resources.GetObject( "lblValueExample.Font" ) ) );
			this.errorPrv.SetIconAlignment( this.lblValueExample, ( ( System.Windows.Forms.ErrorIconAlignment )( resources.GetObject( "lblValueExample.IconAlignment" ) ) ) );
			this.errorPrv.SetIconPadding( this.lblValueExample, ( ( int )( resources.GetObject( "lblValueExample.IconPadding" ) ) ) );
			this.lblValueExample.Image = ( ( System.Drawing.Image )( resources.GetObject( "lblValueExample.Image" ) ) );
			this.lblValueExample.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblValueExample.ImageAlign" ) ) );
			this.lblValueExample.ImageIndex = ( ( int )( resources.GetObject( "lblValueExample.ImageIndex" ) ) );
			this.lblValueExample.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "lblValueExample.ImeMode" ) ) );
			this.lblValueExample.Location = ( ( System.Drawing.Point )( resources.GetObject( "lblValueExample.Location" ) ) );
			this.lblValueExample.Name = "lblValueExample";
			this.lblValueExample.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "lblValueExample.RightToLeft" ) ) );
			this.lblValueExample.Size = ( ( System.Drawing.Size )( resources.GetObject( "lblValueExample.Size" ) ) );
			this.lblValueExample.TabIndex = ( ( int )( resources.GetObject( "lblValueExample.TabIndex" ) ) );
			this.lblValueExample.Text = resources.GetString( "lblValueExample.Text" );
			this.lblValueExample.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblValueExample.TextAlign" ) ) );
			this.lblValueExample.Visible = ( ( bool )( resources.GetObject( "lblValueExample.Visible" ) ) );
			// 
			// errorPrv
			// 
			this.errorPrv.ContainerControl = this;
			this.errorPrv.Icon = ( ( System.Drawing.Icon )( resources.GetObject( "errorPrv.Icon" ) ) );
			// 
			// frmSimpleAdd
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
			this.Controls.Add( this.lblValueExample );
			this.Controls.Add( this.lblExample );
			this.Controls.Add( this.btnOK );
			this.Controls.Add( this.txtValue );
			this.Controls.Add( this.lblValue );
			this.Controls.Add( this.btnCancel );
			this.Enabled = ( ( bool )( resources.GetObject( "$this.Enabled" ) ) );
			this.Font = ( ( System.Drawing.Font )( resources.GetObject( "$this.Font" ) ) );
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ( ( System.Drawing.Icon )( resources.GetObject( "$this.Icon" ) ) );
			this.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "$this.ImeMode" ) ) );
			this.Location = ( ( System.Drawing.Point )( resources.GetObject( "$this.Location" ) ) );
			this.MaximizeBox = false;
			this.MaximumSize = ( ( System.Drawing.Size )( resources.GetObject( "$this.MaximumSize" ) ) );
			this.MinimizeBox = false;
			this.MinimumSize = ( ( System.Drawing.Size )( resources.GetObject( "$this.MinimumSize" ) ) );
			this.Name = "frmSimpleAdd";
			this.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "$this.RightToLeft" ) ) );
			this.StartPosition = ( ( System.Windows.Forms.FormStartPosition )( resources.GetObject( "$this.StartPosition" ) ) );
			this.Text = resources.GetString( "$this.Text" );
			this.ResumeLayout( false );

		}
		#endregion

		#region Utility Methods
		/// <summary>
		/// Method check is for property ValidColor code serialization required. Method used by IDE for code Dom serialization.
		/// </summary>
		/// <returns>True - serialization required, otherwise False.</returns>
		protected bool ShouldSerializeValidColor()
		{
			return ( this.ValidColor != Control.DefaultForeColor );
		}
		/// <summary>
		/// According to user setting update size of form
		/// </summary>
		protected void UpdateFormSize()
		{
			string value = lblValueExample.Text;
			bool bExample = !( value == null || value.Length == 0 );

			lblExample.Visible = bExample;
			lblValueExample.Visible = bExample;

			this.SuspendLayout();

			// if nothing set then do not display Example labels
			if( bExample )
			{
				lblValueExample.Height += DEF_STEP;//lblValueExample.PreferredHeight;

				btnOK.Top = btnCancel.Top = lblValueExample.Bottom + DEF_STEP;
			}
			else
			{
				btnOK.Top = btnCancel.Top = txtValue.Bottom + DEF_STEP;
			}

			this.Height = btnOK.Bottom + DEF_STEP + SystemInformation.CaptionHeight;

			this.ResumeLayout( false );
		}
		/// <summary>
		/// According to validation options update control font color
		/// </summary>
		protected void UpdateControlColor()
		{
			txtValue.ForeColor = this.ValidColor;

			if( m_validator != null )
			{
				if( this.Value != null )
				{
					m_bSuccess = m_validator.Match( this.Value ).Success;
					m_bSuccess = ( m_bReverse ) ? !m_bSuccess : m_bSuccess;

					txtValue.ForeColor = ( m_bSuccess ) ? this.ValidColor : this.InvalidColor;
				}
			}
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

		#region Event Handlers
		/// <summary>
		/// On text enter check is value correct or not
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void txtValue_TextChanged( object sender, System.EventArgs e )
		{
			UpdateControlColor();
		}
		/// <summary>
		/// Validating value by regexp.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void txtValue_Validating( object sender, CancelEventArgs e )
		{
			if( !m_bSuccess && !btnCancel.Focused )
			{
				e.Cancel = true;
				errorPrv.SetError( txtValue, DEF_ERROR );
				errorPrv.SetIconAlignment( txtValue, ErrorIconAlignment.MiddleLeft );
			}
			else
			{
				errorPrv.SetError( txtValue, string.Empty );
			}
		}
		#endregion
	}
}
