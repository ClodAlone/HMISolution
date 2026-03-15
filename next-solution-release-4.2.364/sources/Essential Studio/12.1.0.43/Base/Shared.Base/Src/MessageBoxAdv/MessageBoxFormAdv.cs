#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !(SyncfusionFramework1_0 || SyncfusionFramework1_1)

using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Localization;
using Syncfusion.Windows.Forms.Tools;
using System.Media;


namespace Syncfusion.Windows.Forms
{
	internal partial class MessageBoxFormAdv:
		Office2007Form,
		IMessageFilter
	{
		#region Constants

		private const int c_nHorzPad = 10;
		private const int c_nVertPad = c_nHorzPad;

		#endregion

		#region Enums

		enum SystemLocStrings
		{
			OK			= 0,
			Cancel		= 1,
			Abort		= 2,
			Retry		= 3,
			Ignore		= 4,
			Yes			= 5,
			No			= 6,
			Close		= 7,
			Help		= 8,
			TryAgain	= 9,
			Continue	= 10
		}

		#endregion

		#region Fields

		private FlowLayoutPanel m_contentPanel;
		private FlowLayoutPanel m_buttonsPanel;
		private ButtonAdv m_ok;
		private ButtonAdv m_cancel;
		private ButtonAdv m_yes;
		private ButtonAdv m_ignore;
		private ButtonAdv m_retry;
		private ButtonAdv m_abort;
		private ButtonAdv m_no;
		private PictureBox m_image;
		private Label m_text;

		private MessageBoxButtons m_buttons = MessageBoxButtons.OK;
		private MessageBoxIcon m_icon = MessageBoxIcon.None;
		private MessageBoxDefaultButton m_defaultButton = MessageBoxDefaultButton.Button1;
		private MessageBoxOptions m_options;
		private IWin32Window m_owner;

		private CancelEventHandler m_helpButtonClicked;

		#region Static

		private static DialogResult[][] s_buttons;
		private static string[] s_buttonNames;
		private static SystemLocStrings[] s_buttonLocIDs;

		#endregion

		#endregion
		
		#region Construction

		static MessageBoxFormAdv()
		{
			Initialize();
		}

		public MessageBoxFormAdv()
		{
			InitializeComponent();
		}

		public MessageBoxFormAdv( IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton,
			MessageBoxOptions options, bool bShowHelpButton, CancelEventHandler helpButtonClickHandler, Image image, Size sizeImage ):
			this()
		{
			m_buttons = buttons;
			m_icon = icon;
			m_defaultButton = defaultButton;
			m_options = options;
			m_helpButtonClicked = helpButtonClickHandler;
			m_owner = owner;
			m_text.Text = text;
			
			this.Text = caption;
			this.HelpButton = bShowHelpButton;

			this.ColorScheme = MessageBoxAdv.Office2007Theme;

			this.ApplyAeroTheme = MessageBoxAdv.ApplyAeroTheme ;

			if( this.ColorScheme == Office2007Theme.Black)
			{
				m_text.ForeColor = Color.White;
			}

			InitializeImage( image, sizeImage );
			InitializeButtons();
			InitializeOffice2007Style();
			InitializeOptions();
			InitializeSizes();
		}

		public MessageBoxFormAdv( IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton,
			MessageBoxOptions options, bool bShowHelpButton, CancelEventHandler helpButtonClickHandler ):
			this( owner, text, caption, buttons, icon, defaultButton, options, bShowHelpButton, helpButtonClickHandler, null, Size.Empty )
		{
		}

		#endregion

		#region Initialization

		private static void Initialize()
		{
			if( s_buttons == null )
			{
				s_buttons = new DialogResult[][]
				{
					new DialogResult[] { DialogResult.OK },
					new DialogResult[] { DialogResult.OK, DialogResult.Cancel },
					new DialogResult[] { DialogResult.Abort, DialogResult.Retry, DialogResult.Ignore },
					new DialogResult[] { DialogResult.Yes, DialogResult.No, DialogResult.Cancel },
					new DialogResult[] { DialogResult.Yes, DialogResult.No },
					new DialogResult[] { DialogResult.Retry, DialogResult.Cancel }
				};
			}

			if( s_buttonNames == null )
			{
				s_buttonNames = new string[]
				{
					null,		//None = 0
					"m_ok",		//OK = 1
					"m_cancel",	//Cancel = 2
					"m_abort",	//Abort = 3
					"m_retry",	//Retry = 4
					"m_ignore",	//Ignore = 5
					"m_yes",	//Yes = 6
					"m_no"		//No = 7
				};
			}

			if( s_buttonLocIDs == null )
			{
				s_buttonLocIDs = new SystemLocStrings[]
				{
					SystemLocStrings.Close,	//None = 0
					SystemLocStrings.OK,	//OK = 1
					SystemLocStrings.Cancel,//Cancel = 2
					SystemLocStrings.Abort,	//Abort = 3
					SystemLocStrings.Retry,	//Retry = 4
					SystemLocStrings.Ignore,//Ignore = 5
					SystemLocStrings.Yes,	//Yes = 6
					SystemLocStrings.No		//No = 7
				};
			}
		}

		private void InitializeButtons()
		{
			DialogResult[] buttonsAdv = GetButtons();
			int tabIndex = 0;

			foreach( DialogResult btn in buttonsAdv )
			{
				ButtonAdv btnAdv = GetButton( btn );
                    string btnText = SR.GetString(btn.ToString());
                    if (!string.IsNullOrEmpty(btnText))
                    {
                        btnAdv.Text = btnText;
                    }

				btnAdv.TabIndex = ++tabIndex;
				m_buttonsPanel.Controls.SetChildIndex( btnAdv, tabIndex );

				btnAdv.Visible = true;
			}

			int index = -1;
 
			switch( m_defaultButton )
			{
				case MessageBoxDefaultButton.Button1:
					index = 0;
					break;

				case MessageBoxDefaultButton.Button2:
					index = 1;
					break;

				case MessageBoxDefaultButton.Button3:
					index = 2;
					break;
			}

			if( index >= 0 && index < buttonsAdv.Length )
			{
				ButtonAdv defBtn = GetButton( buttonsAdv[index] );

				this.AcceptButton = defBtn;
				defBtn.Select();
			}

			this.ControlBox = this.IsCancelable;
		}

		private DialogResult[] GetButtons()
		{
			return s_buttons[(int)m_buttons];
		}

		private void InitializeOffice2007Style()
		{
			InitializeOffice2007Style( this );
			InitializeOffice2007Style( m_contentPanel );
			InitializeOffice2007Style( m_buttonsPanel );
		}

		private void InitializeOffice2007Style( Control control )
		{
			foreach( Control ctl in control.Controls )
			{
				ISupportOffice2007Theme support = ctl as ISupportOffice2007Theme;

				if( support != null )
				{
					support.Office2007ColorTheme = this.ColorScheme;
				}
			}
		}

		private void InitializeImage( Image image, Size sizeImage )
		{
			SystemSound sysSound = SystemSounds.Beep;

			if( image == null )
			{
				sysSound = InitializeSysIcon();
			}
			else
			{
				m_image.Size = (sizeImage.IsEmpty ? image.Size : sizeImage);

				m_image.Image = image;
				m_image.Visible = true;
			}

			sysSound.Play();
		}

		private SystemSound InitializeSysIcon()
		{
			SystemSound sysSound = null;
			Icon sysIcon = null;

			switch( m_icon )
			{
				case MessageBoxIcon.Asterisk:
					sysIcon = SystemIcons.Asterisk;
					sysSound = SystemSounds.Asterisk;
					break;

				case MessageBoxIcon.Hand:
					sysIcon = SystemIcons.Hand;
					sysSound = SystemSounds.Hand;
					break;

				case MessageBoxIcon.Exclamation:
					sysIcon = SystemIcons.Exclamation;
					sysSound = SystemSounds.Exclamation;
					break;

				case MessageBoxIcon.Question:
					sysIcon = SystemIcons.Question;
					sysSound = SystemSounds.Question;
					break;

				default:
					sysSound = SystemSounds.Beep;
					break;
			}

			if( sysIcon != null )
			{
				m_image.Size = sysIcon.Size;
				m_image.Image = sysIcon.ToBitmap();
				m_image.Visible = true;

				sysIcon.Dispose();
			}

			return sysSound;
		}

		private void InitializeOptions()
		{
			if( (m_options & MessageBoxOptions.RtlReading) != 0 )
			{
				this.RightToLeft = RightToLeft.Yes;
				this.RightToLeftLayout = true;
			}

			if( (m_options & MessageBoxOptions.RightAlign) != 0 )
			{
				m_text.TextAlign = ContentAlignment.MiddleRight;
			}
		}

		private void InitializeSizes()
		{
			Size ncSize = Size.Subtract( this.Bounds.Size, this.ClientSize );
			Screen screen = (m_owner != null && m_owner.Handle != IntPtr.Zero) ? Screen.FromHandle( m_owner.Handle ) : Screen.PrimaryScreen;
			Size waSize = screen.WorkingArea.Size;
			Size textSize = m_text.GetPreferredSize(Size.Empty);
			int nMaxWidth = waSize.Width - ncSize.Width - 2 * c_nHorzPad;

			if( this.IsImageShown )
			{
				nMaxWidth -= c_nHorzPad + m_image.Width;
			}

			float fRatio = (float)nMaxWidth / (float)textSize.Height;
			float height = Math.Max( (float)Math.Sqrt( (double)textSize.Height * (double)textSize.Width / fRatio ), textSize.Height );

			if( this.IsImageShown )
			{
				height = Math.Max( height, m_image.Height );
			}

			float width = fRatio * height;

			textSize = new Size( (int)Math.Round( width ), (int)Math.Round( height ) );
			textSize = m_text.GetPreferredSize(textSize);

			if( this.IsImageShown )
			{
				textSize.Height = Math.Max( textSize.Height, m_image.Height );
			}

			Size clientSize = GetClientSize( textSize );
			int btnsWidth = GetButtonsWidth();

			if( btnsWidth > clientSize.Width )
			{
				width = textSize.Width;
				height = textSize.Height;

				textSize.Width = btnsWidth - (clientSize.Width - textSize.Width);
				height = Math.Max( width * height / (float)textSize.Width, textSize.Height );

				if( this.IsImageShown )
				{
					height = Math.Max( height, m_image.Height );
				}
				
				textSize.Height = (int)Math.Round( height );
				clientSize = GetClientSize( textSize );
			}
			else
			{
				SetButonsPadding( clientSize, btnsWidth );
			}

			Size formSize = Size.Add( clientSize, ncSize );
			Size diff = Size.Subtract( waSize, formSize );

			if( diff.Width > 0 ) diff.Width = 0;
			if( diff.Height > 0 ) diff.Height = 0;

			if( !diff.IsEmpty )
			{
				clientSize = Size.Add( clientSize, diff );
				textSize = Size.Add( textSize, diff );

				if( btnsWidth < clientSize.Width )
				{
					SetButonsPadding( clientSize, btnsWidth );
				}
			}

			this.ClientSize = clientSize;
			m_text.ClientSize = textSize;
		}

		private bool IsImageShown
		{
			get
			{
				return m_image.Image != null;
			}
		}

		#endregion

		#region Implementation

		private ButtonAdv GetButton( DialogResult btn )
		{
			string name = s_buttonNames[(int)btn];

			return (ButtonAdv)(m_buttonsPanel.Controls[ name ]);
		}

		private void SetButonsPadding( Size clientSize, int btnsWidth )
		{
			int btnsHorzPad = (clientSize.Width - btnsWidth) / 2;
			Padding btnsPadding = m_buttonsPanel.Padding;

			if( this.RightToLeft == RightToLeft.Yes )
			{
				btnsPadding.Right = btnsHorzPad;
			}
			else
			{
				btnsPadding.Left = btnsHorzPad;
			}			

			m_buttonsPanel.Padding = btnsPadding;
		}

		private Size GetClientSize( Size textSize )
		{
			Size clientSize = new Size();

			clientSize.Width = c_nHorzPad + textSize.Width + c_nHorzPad;

			if( this.IsImageShown )
			{
				clientSize.Width += c_nHorzPad + m_image.Width + c_nHorzPad;
			}

			clientSize.Height = c_nVertPad + textSize.Height + c_nVertPad / 2 + m_buttonsPanel.Height + c_nVertPad;

			return clientSize;
		}

		private int GetButtonsWidth()
		{
			int width = c_nHorzPad;
			DialogResult[] buttonsAdv = GetButtons();

			foreach( DialogResult btn in buttonsAdv )
			{
				ButtonAdv btnAdv = GetButton( btn );

				width += btnAdv.Width + btnAdv.Margin.Horizontal;
			}

			return width;
		}

		private string GetLocString( DialogResult btn )
		{
			string s = null;
			SystemLocStrings sysLoscStr = s_buttonLocIDs[(int)btn];

			try
			{
				s = NativeMethods.MB_GetString( (int)sysLoscStr );
			}
			catch
			{
			}

			return s;
		}

		private bool IsCancelable
		{
			get
			{
				bool bResult = true;

				switch( m_buttons )
				{
					case MessageBoxButtons.AbortRetryIgnore:
					case MessageBoxButtons.YesNo:
						bResult = false;
						break;
				}

				return bResult;
			}
		}

		#endregion

		#region Overrides

		protected override void OnHandleCreated( EventArgs e )
		{
			base.OnHandleCreated( e );

			Application.AddMessageFilter( this );

			if( SystemInformation.IsSnapToDefaultEnabled  )
			{
				MethodInvoker snapToDefault = new MethodInvoker( SnapToDefault );

				this.BeginInvoke( snapToDefault );	
			}
		}

		private void SnapToDefault()
		{
			Control btn = this.AcceptButton as Control;

			if( btn != null )
			{
				Point curPos = new Point( btn.Width / 2, btn.Height / 2 );
				curPos = btn.PointToScreen( curPos );

				Cursor.Position = curPos;
			}
		}

		protected override void OnHandleDestroyed( EventArgs e )
		{
			Application.RemoveMessageFilter( this );

			base.OnHandleDestroyed( e );
		}

		protected override void OnClosing( CancelEventArgs e )
		{
			base.OnClosing( e );

			bool bCancel = true;
			DialogResult[] buttonsRes = GetButtons();

			foreach( DialogResult res in buttonsRes )
			{
				if( res == this.DialogResult )
				{
					bCancel = false;
					break;
				}
			}

			e.Cancel = bCancel && !(this.IsCancelable && this.DialogResult == DialogResult.Cancel);
		}

		protected override void OnHelpButtonClicked( CancelEventArgs e )
		{
			base.OnHelpButtonClicked( e );

			if( m_helpButtonClicked != null )
			{
				m_helpButtonClicked( this, e );
			}
		}

		#endregion

		#region IMessageFilter Members

		bool IMessageFilter.PreFilterMessage( ref Message m )
		{
			if( this.IsHandleCreated && m.Msg == NativeMethods.WM_KEYDOWN )
			{
				Keys keyCode = (Keys)m.WParam | Control.ModifierKeys;

				if( keyCode == Keys.Escape && this.ControlBox  )
				{
					if( this.Handle == m.HWnd || NativeMethods.IsChild( this.Handle, m.HWnd ) )
					{
						if( m_buttons == MessageBoxButtons.OK )
						{
							this.DialogResult = DialogResult.OK;
						}
						else
						{
							this.DialogResult = DialogResult.Cancel;
						}
					}
				}
			}

			return false;
		}

		#endregion
	}
}

#endif