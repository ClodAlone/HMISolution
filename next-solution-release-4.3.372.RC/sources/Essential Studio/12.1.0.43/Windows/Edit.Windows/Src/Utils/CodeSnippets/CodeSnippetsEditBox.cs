#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel;

using Syncfusion.Windows.Forms.Edit.Forms.Popup;
using Syncfusion.Windows.Forms.Edit.Interfaces;

namespace Syncfusion.Windows.Forms.Edit.Utils.CodeSnippets
{
	/// <summary>
	/// Edit box for work with VS 2005 like code snippets.
	/// </summary>
	[ToolboxItem( false )]
	public class CodeSnippetsEditBox
		: ContainerControl
	{
		#region Fields
		/// <summary>
		/// Caption label.
		/// </summary>
		private Label m_lblCaption;
		/// <summary>
		/// Collection of intermediate labels.
		/// </summary>
		private ArrayList m_labels;
		/// <summary>
		/// Textbox for editing the last chain.
		/// </summary>
		private TextBox m_txtEdit;
		/// <summary>
		/// Underlying ContextChoiceController.
		/// </summary>
		private CodeSnippetsPopupController m_contextChoiceController;
		/// <summary>
		/// Parent of the CodeSnippetEditBox
		/// </summary>
		internal Control m_parent;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets caption of the control.
		/// </summary>
		public string Caption
		{
			get
			{
				return m_lblCaption.Text;
			}
			set
			{
				if( value == null ) throw new ArgumentNullException( "Caption" );

				if( m_lblCaption.Text != value )
				{
					m_lblCaption.Text = value;
					PerformLayout();
				}
			}
		}
		Size formSize = new Size(100, 100);
		internal Size FormSize
		{
			get
			{
				return formSize;
			}
			set
			{
				if (!value.IsEmpty)
				{
					formSize = value;
					m_contextChoiceController.FormSize = value;
				}
				else throw new ArgumentOutOfRangeException("FormSize");
			}
		}
		/// <summary>
		/// Gets current text of edit textbox.
		/// </summary>
		public string CurrentText
		{
			get
			{
				return m_txtEdit.Text;
			}
		}
		/// <summary>
		/// Gets or sets underlying ContextChoiceController.
		/// </summary>
		internal CodeSnippetsPopupController ContextChoiceController
		{
			get
			{
				return m_contextChoiceController;
			}
			set
			{
                if (m_contextChoiceController != value)
                {
                    m_contextChoiceController = value;                    
                }
			}
		}
		#endregion

		#region Initialization
		/// <summary>
		/// Creates and initializes new instance of CodeSnippetsEditBox.
		/// </summary>
		/// <param name="caption">Caption of CodeSnippetsEditBox.</param>
		/// <param name="parent">Parent control.</param>
		public CodeSnippetsEditBox( string caption, Control parent )
		{
			if( caption == null ) throw new ArgumentNullException( "caption" );
			if( parent == null ) throw new ArgumentNullException( "parent" );

			SuspendLayout();

			m_parent = parent;

			m_lblCaption = CreateLabel( caption );
			m_lblCaption.Font = new Font( m_lblCaption.Font, FontStyle.Bold );
			m_lblCaption.ForeColor = Color.Black;
			this.Controls.Add( m_lblCaption );

			m_labels = new ArrayList();
			m_txtEdit = new TextBox();
			m_txtEdit.Width = 200;
			m_txtEdit.BorderStyle = BorderStyle.None;
			m_txtEdit.KeyPress += new KeyPressEventHandler( OnTxtEditKeyPress );
			m_txtEdit.KeyDown += new KeyEventHandler( OnTxtEditKeyDown );
			this.Controls.Add( m_txtEdit );

			this.BackColor = Color.FromArgb( 255, 238, 194 );
			this.ForeColor = Color.FromArgb( 122, 158, 221 );

			this.Visible = false;

			ResumeLayout();
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Adds new intermediate label.
		/// </summary>
		/// <param name="text">Text of new label.</param>
		public void AddLabel( string text )
		{
			if( text == null ) throw new ArgumentNullException( "text" );

			SuspendLayout();

			Label lbl = CreateLabel( text );
			lbl.ForeColor = this.ForeColor;
			lbl.MouseDown += new MouseEventHandler( OnLblMouseDown );
			lbl.Font = new Font( lbl.Font, FontStyle.Underline );
			lbl.Cursor = Cursors.IBeam;
			m_labels.Add( lbl );
			this.Controls.Add( lbl );
			lbl = CreateLabel( " > " );
			lbl.ForeColor = Color.Black;
			m_labels.Add( lbl );
			this.Controls.Add( lbl );
			m_txtEdit.Text = string.Empty;

			ResumeLayout();
		}
		#endregion

		#region Events
		/// <summary>
		/// Raised when text in edit textbox is changed.
		/// </summary>
		public event EventHandler CurrentTextChanged
		{
			add
			{
				m_txtEdit.TextChanged += value;
			}
			remove
			{
				m_txtEdit.TextChanged -= value;
			}
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Assigns backcolor to editbox.
		/// </summary>
		/// <param name="e">EventArgs.</param>
		protected override void OnBackColorChanged( EventArgs e )
		{
			base.OnBackColorChanged( e );

			m_txtEdit.BackColor = this.BackColor;
		}
		/// <summary>
		/// Lays out labels and edit.
		/// </summary>
		/// <param name="levent">LayoutEventArgs.</param>
		protected override void OnLayout( LayoutEventArgs levent )
		{
			base.OnLayout( levent );

			this.Height = m_lblCaption.Height;
			m_lblCaption.Location = new Point( 0, 0 );
			int left = m_lblCaption.Width;

			foreach( Label lbl in m_labels )
			{
				lbl.Location = new Point( left, 0 );
				left += lbl.Width;
			}

			m_txtEdit.Location = new Point( left, 0 );
			this.Width = m_txtEdit.Left + m_txtEdit.Width;
		}
		/// <summary>
		/// Performs visibility changing.
		/// </summary>
		/// <param name="e">EventArgs.</param>
		protected override void OnVisibleChanged( EventArgs e )
		{
			if( this.Visible )
			{
				// For Tooltips to let this form be 8)
				WinAPI.SetWindowPos( this.Handle, new IntPtr( -1/*HWND_TOPMOST*/ ),
					0, 0, 0, 0, ( int )( SetWindowPosFlags.SWP_SHOWWINDOW | SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOMOVE ) );

				foreach( Label lbl in m_labels )
				{
					lbl.Dispose();
				}
				m_labels.Clear();
				m_txtEdit.Text = string.Empty;
			}
			else
			{
				if( m_contextChoiceController != null )
				{
					m_contextChoiceController.Close();
				}
			}

			base.OnVisibleChanged( e );
		}
		/// <summary>
		/// Adds needed parameters to window.
		/// </summary>
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.Style |= unchecked( ( int )WindowStyles.WS_POPUP );
				cp.ExStyle |= ( int )WindowExStyles.WS_EX_TOPMOST;
				cp.ExStyle &= ( int )~WindowExStyles.WS_EX_APPWINDOW;
				cp.ExStyle |= ( int )WindowExStyles.WS_EX_TOOLWINDOW;
				return cp;
			}
		}
		/// <summary>
		/// Processes Windows messages. 
		/// </summary>
		/// <param name="m">The Windows Message to process.</param>
		protected override void WndProc( ref Message m )
		{
			switch( m.Msg )
			{
				case ( int )Msg.WM_ACTIVATE:
					{
						if( m.WParam == IntPtr.Zero )
						{
							if( m_contextChoiceController.Form != null && !m_contextChoiceController.Form.IsDisposed )
							{
								if( m_contextChoiceController.Form.Handle != m.LParam && m_txtEdit.Handle != m.LParam )
								{
									if( this.Visible )
									{
										m_parent.BeginInvoke( new MethodInvoker( HidePopup ) );
									}
								}
							}
						}
						break;
					}

				case ( int )Msg.WM_WINDOWPOSCHANGED:
					{
						WINDOWPOS winPos = ( WINDOWPOS )Marshal.PtrToStructure( m.LParam, typeof( WINDOWPOS ) );
						if( ( winPos.flags & SetWindowPosFlags.SWP_HIDEWINDOW ) != 0 )
						{
							m_contextChoiceController.Close();
						}
						else if( ( winPos.flags & SetWindowPosFlags.SWP_SHOWWINDOW ) != 0 )
						{
							m_contextChoiceController.Show();
						}
						break;
					}
			}

			base.WndProc( ref m );
		}
		#endregion

		#region Nonpublic Methods
		/// <summary>
		/// Creates label and initializes it with proper settings.
		/// </summary>
		/// <param name="text">Text of new label.</param>
		/// <returns>Created label.</returns>
		private Label CreateLabel( string text )
		{
			Label result = new Label();
			result.BackColor = Color.Transparent;
			result.AutoSize = true;
			result.Text = text;
			return result;
		}
		/// <summary>
		/// Transforms given label to editable textbox and destroys all the labels after it.
		/// </summary>
		/// <param name="lbl">Label to activate.</param>
		private void ActivateLabel( Label lbl )
		{
			SuspendLayout();

			string text = lbl.Text;

			int lblIndex = m_labels.IndexOf( lbl );
			int labelsDestroyed = ( m_labels.Count - lblIndex ) / 2;
			while( lblIndex < m_labels.Count )
			{
				( ( Label )m_labels[ lblIndex ] ).Dispose();
				m_labels.RemoveAt( lblIndex );
			}

			m_txtEdit.Text = text;
			m_txtEdit.Focus();
			m_txtEdit.SelectionStart = text.Length;

			if( m_contextChoiceController != null )
			{
				CodeSnippetsContainer curContainer = m_contextChoiceController.CurrentContainer;
				for( int i = 0; i < labelsDestroyed; i++ )
				{
					curContainer = curContainer.Parent;
				}
				m_contextChoiceController.Activate( curContainer );
			}

			ResumeLayout();
		}
		/// <summary>
		/// Hides current popup window.
		/// </summary>
		public void HidePopup()
		{
			if( m_contextChoiceController.Form != null && !m_contextChoiceController.Form.IsDisposed )
			{
				WinAPI.SetWindowPos( m_contextChoiceController.Form.Handle, IntPtr.Zero, 0, 0, 0, 0, ( int )( SetWindowPosFlags.SWP_HIDEWINDOW | SetWindowPosFlags.SWP_NOACTIVATE ) );
			}
			WinAPI.SetWindowPos( this.Handle, IntPtr.Zero, 0, 0, 0, 0, ( int )( SetWindowPosFlags.SWP_HIDEWINDOW | SetWindowPosFlags.SWP_NOACTIVATE ) );
			this.Visible = false;
		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// Handles keys.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTxtEditKeyPress( object sender, KeyPressEventArgs e )
		{
			switch( e.KeyChar )
			{
				case ( char )8:
					{
						if( m_txtEdit.SelectionStart == 0 )
						{
							int labelsCount = m_labels.Count;
							if( labelsCount > 0 )
							{
								ActivateLabel( ( Label )m_labels[ labelsCount - 2 ] );
							}
							e.Handled = true;
						}
						break;
					}
			}
		}
		/// <summary>
		/// Transforms clicked label into editable textbox.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnLblMouseDown( object sender, MouseEventArgs e )
		{
			if( e.Button == MouseButtons.Left )
			{
				Label lbl = ( Label )sender;
				ActivateLabel( lbl );
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTxtEditKeyDown( object sender, KeyEventArgs e )
		{
			switch( e.KeyData )
			{
				case Keys.Escape:
					{
						Form mainForm = m_parent.TopLevelControl as Form;
						if( mainForm != null )
						{
							mainForm.Activate();
							e.Handled = true;
						}
						break;
					}
			}

			this.OnKeyDown( e );
		}
		#endregion
	}
}