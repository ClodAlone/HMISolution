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
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Edit.Dialogs
{
	/// <summary>
	/// Result Dialog window.
	/// </summary>
	public class frmNotificationDialog
		: System.Windows.Forms.Form
	{
		#region Control Members
		private System.Windows.Forms.CheckBox chkOption;
		private System.Windows.Forms.Label lblMessage;
		private System.Windows.Forms.PictureBox picIcon;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		#endregion

		#region Fields
		/// <summary>
		/// Indicates if hide Cancel button.
		/// </summary>
		private bool m_bHideCancel;
		/// <summary>
		/// Ok button location.
		/// </summary>
		private Point m_okLocation;
		/// <summary>
		/// Parent window for this dialog,
		/// </summary>
		private Form m_parent;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets icon on the dialog.
		/// </summary>
		public Image DialogIcon
		{
			get
			{
				return picIcon.Image;
			}
			set
			{
				if( value == null )
					throw new ArgumentNullException( "DialogIcon" );

				if( picIcon.Image != value )
				{
					picIcon.Image = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets notice message for dialog.
		/// </summary>
		public string NotifyText
		{
			get
			{
				return lblMessage.Text;
			}
			set
			{
				if( value == null )
					throw new ArgumentNullException( "NotifyText" );

				if( value.Length == 0 )
					throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_40 );

				if( lblMessage.Text != value )
				{
					lblMessage.Text = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets string text near Check box on the dialog.
		/// </summary>
		public string CheckText
		{
			get
			{
				return chkOption.Text;
			}
			set
			{
				if( value == null )
					throw new ArgumentNullException( "CheckText" );

				if( value.Length == 0 )
					throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_41 );

				if( chkOption.Text != value )
				{
					chkOption.Text = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets caption text of the dialog.
		/// </summary>
		public string DialogCaption
		{
			get
			{
				return this.Text;
			}
			set
			{
				if( value == null )
					throw new ArgumentNullException( "DialogCaption" );

				if( value.Length == 0 )
					throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_42 );

				if( this.Text != value )
				{
					this.Text = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets if hide cancel button or not.
		/// </summary>
		public bool HideCancel
		{
			get
			{
				return m_bHideCancel;
			}
			set
			{
				if( m_bHideCancel != value )
				{
					m_bHideCancel = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets parent form for this dialog.
		/// </summary>
		public Form DialogParent
		{
			get
			{
				return m_parent;
			}
			set
			{
				if( m_parent != value )
				{
					m_parent = value;
				}
			}
		}
		#endregion

		#region Initialize/Finalize methods
		/// <summary>
		/// Creates new instance of frmNotificationDialog.
		/// </summary>
		public frmNotificationDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			EnableDoubleBuffering();
			m_okLocation = btnOK.Location;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager( typeof( frmNotificationDialog ) );
			this.btnCancel = new System.Windows.Forms.Button();
			this.lblMessage = new System.Windows.Forms.Label();
			this.chkOption = new System.Windows.Forms.CheckBox();
			this.picIcon = new System.Windows.Forms.PictureBox();
			this.btnOK = new System.Windows.Forms.Button();
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
			// lblMessage
			// 
			this.lblMessage.AccessibleDescription = resources.GetString( "lblMessage.AccessibleDescription" );
			this.lblMessage.AccessibleName = resources.GetString( "lblMessage.AccessibleName" );
			this.lblMessage.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "lblMessage.Anchor" ) ) );
			this.lblMessage.AutoSize = ( ( bool )( resources.GetObject( "lblMessage.AutoSize" ) ) );
			this.lblMessage.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "lblMessage.Dock" ) ) );
			this.lblMessage.Enabled = ( ( bool )( resources.GetObject( "lblMessage.Enabled" ) ) );
			this.lblMessage.Font = ( ( System.Drawing.Font )( resources.GetObject( "lblMessage.Font" ) ) );
			this.lblMessage.Image = ( ( System.Drawing.Image )( resources.GetObject( "lblMessage.Image" ) ) );
			this.lblMessage.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblMessage.ImageAlign" ) ) );
			this.lblMessage.ImageIndex = ( ( int )( resources.GetObject( "lblMessage.ImageIndex" ) ) );
			this.lblMessage.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "lblMessage.ImeMode" ) ) );
			this.lblMessage.Location = ( ( System.Drawing.Point )( resources.GetObject( "lblMessage.Location" ) ) );
			this.lblMessage.Name = "lblMessage";
			this.lblMessage.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "lblMessage.RightToLeft" ) ) );
			this.lblMessage.Size = ( ( System.Drawing.Size )( resources.GetObject( "lblMessage.Size" ) ) );
			this.lblMessage.TabIndex = ( ( int )( resources.GetObject( "lblMessage.TabIndex" ) ) );
			this.lblMessage.Text = resources.GetString( "lblMessage.Text" );
			this.lblMessage.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "lblMessage.TextAlign" ) ) );
			this.lblMessage.Visible = ( ( bool )( resources.GetObject( "lblMessage.Visible" ) ) );
			// 
			// chkOption
			// 
			this.chkOption.AccessibleDescription = resources.GetString( "chkOption.AccessibleDescription" );
			this.chkOption.AccessibleName = resources.GetString( "chkOption.AccessibleName" );
			this.chkOption.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "chkOption.Anchor" ) ) );
			this.chkOption.Appearance = ( ( System.Windows.Forms.Appearance )( resources.GetObject( "chkOption.Appearance" ) ) );
			this.chkOption.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "chkOption.BackgroundImage" ) ) );
			this.chkOption.CheckAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "chkOption.CheckAlign" ) ) );
			this.chkOption.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "chkOption.Dock" ) ) );
			this.chkOption.Enabled = ( ( bool )( resources.GetObject( "chkOption.Enabled" ) ) );
			this.chkOption.FlatStyle = ( ( System.Windows.Forms.FlatStyle )( resources.GetObject( "chkOption.FlatStyle" ) ) );
			this.chkOption.Font = ( ( System.Drawing.Font )( resources.GetObject( "chkOption.Font" ) ) );
			this.chkOption.Image = ( ( System.Drawing.Image )( resources.GetObject( "chkOption.Image" ) ) );
			this.chkOption.ImageAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "chkOption.ImageAlign" ) ) );
			this.chkOption.ImageIndex = ( ( int )( resources.GetObject( "chkOption.ImageIndex" ) ) );
			this.chkOption.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "chkOption.ImeMode" ) ) );
			this.chkOption.Location = ( ( System.Drawing.Point )( resources.GetObject( "chkOption.Location" ) ) );
			this.chkOption.Name = "chkOption";
			this.chkOption.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "chkOption.RightToLeft" ) ) );
			this.chkOption.Size = ( ( System.Drawing.Size )( resources.GetObject( "chkOption.Size" ) ) );
			this.chkOption.TabIndex = ( ( int )( resources.GetObject( "chkOption.TabIndex" ) ) );
			this.chkOption.Text = resources.GetString( "chkOption.Text" );
			this.chkOption.TextAlign = ( ( System.Drawing.ContentAlignment )( resources.GetObject( "chkOption.TextAlign" ) ) );
			this.chkOption.Visible = ( ( bool )( resources.GetObject( "chkOption.Visible" ) ) );
			// 
			// picIcon
			// 
			this.picIcon.AccessibleDescription = resources.GetString( "picIcon.AccessibleDescription" );
			this.picIcon.AccessibleName = resources.GetString( "picIcon.AccessibleName" );
			this.picIcon.Anchor = ( ( System.Windows.Forms.AnchorStyles )( resources.GetObject( "picIcon.Anchor" ) ) );
			this.picIcon.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "picIcon.BackgroundImage" ) ) );
			this.picIcon.Dock = ( ( System.Windows.Forms.DockStyle )( resources.GetObject( "picIcon.Dock" ) ) );
			this.picIcon.Enabled = ( ( bool )( resources.GetObject( "picIcon.Enabled" ) ) );
			this.picIcon.Font = ( ( System.Drawing.Font )( resources.GetObject( "picIcon.Font" ) ) );
			this.picIcon.Image = ( ( System.Drawing.Image )( resources.GetObject( "picIcon.Image" ) ) );
			this.picIcon.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "picIcon.ImeMode" ) ) );
			this.picIcon.Location = ( ( System.Drawing.Point )( resources.GetObject( "picIcon.Location" ) ) );
			this.picIcon.Name = "picIcon";
			this.picIcon.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "picIcon.RightToLeft" ) ) );
			this.picIcon.Size = ( ( System.Drawing.Size )( resources.GetObject( "picIcon.Size" ) ) );
			this.picIcon.SizeMode = ( ( System.Windows.Forms.PictureBoxSizeMode )( resources.GetObject( "picIcon.SizeMode" ) ) );
			this.picIcon.TabIndex = ( ( int )( resources.GetObject( "picIcon.TabIndex" ) ) );
			this.picIcon.TabStop = false;
			this.picIcon.Text = resources.GetString( "picIcon.Text" );
			this.picIcon.Visible = ( ( bool )( resources.GetObject( "picIcon.Visible" ) ) );
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
			// 
			// frmNotificationDialog
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
			this.Controls.Add( this.picIcon );
			this.Controls.Add( this.chkOption );
			this.Controls.Add( this.lblMessage );
			this.Controls.Add( this.btnCancel );
			this.Controls.Add( this.btnOK );
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
			this.Name = "frmNotificationDialog";
			this.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "$this.RightToLeft" ) ) );
			this.RightToLeftLayout = true;
			this.StartPosition = ( ( System.Windows.Forms.FormStartPosition )( resources.GetObject( "$this.StartPosition" ) ) );
			this.Text = resources.GetString( "$this.Text" );
			this.ResumeLayout( false );

		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Shows modal dialog.
		/// </summary>
		/// <param name="parent">Parent for this dialog. ( Can be NULL )</param>
		/// <param name="captionText">Caption text of dialog. ( Can be NULL )</param>
		/// <param name="noticeText">Notification Text. ( Can be NULL )</param>
		/// <param name="checkText">Text near CheckBox. ( Can be NULL )</param>
		/// <param name="hideCancel">if TRUE - hides Cancel button.</param>
		/// <param name="icon">Icon on dialog. ( Can be NULL )</param>
		/// <returns>TRUE - if CheckBox was checked, FALSE - Otherwise.</returns>
		public bool Show( Form parent, string captionText, string noticeText, string checkText, bool hideCancel, Image icon )
		{
			if( noticeText != null && noticeText.Length > 0 )
			{
				this.NotifyText = noticeText;
			}

			if( checkText != null && checkText.Length > 0 )
			{
				this.CheckText = checkText;
			}

			if( icon != null )
			{
				this.DialogIcon = icon;
			}

			if( captionText != null && captionText.Length > 0 )
			{
				this.DialogCaption = captionText;
			}

			this.HideCancel = hideCancel;
			ToggleCancelButton( hideCancel );
			this.DialogParent = parent;

			if( parent != null )
			{
				this.ShowDialog( parent );
			}
			else
			{
				this.ShowDialog();
			}

			return chkOption.Checked;
		}
		/// <summary>
		/// Shows modal dialog.
		/// </summary>
		new public void ShowDialog()
		{
			ToggleCancelButton( this.HideCancel );

			base.ShowDialog();
		}
		#endregion

		#region Utility Methods
		/// <summary>
		/// Hides or shows Cancel button.
		/// </summary>
		/// <param name="hideButton"></param>
		private void ToggleCancelButton( bool hideButton )
		{
			if( hideButton && btnCancel.Visible )
			{
				btnCancel.Visible = false;
				btnOK.Location = btnCancel.Location;
			}
			else if( !hideButton && !btnCancel.Visible )
			{
				btnCancel.Visible = true;
				btnOK.Location = m_okLocation;
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
	}
}