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
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms.Tools
{
	[Syncfusion.Documentation.DocumentationExclude()]
	public interface IIntegratedScrollClient
	{
		bool IsUpScrollButtonEnabled();
		bool IsDownScrollButtonEnabled();
		void UpScrollButtonPressed();
		void DownScrollButtonPressed();
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public interface IIntegratedScrollContainer
	{
		void InvalidateUpScrollButton();
		void InvalidateDownScrollButton();
		bool HandleScrollButtonDown(Point pt);
		bool HandleScrollButtonUp(Point pt);
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public enum MouseActions
	{
		LBtnDown = 0,
		LBtnUp,
		RBtnDown,
		RBtnUp,
		Move,
		Leave,
		Drag,
		None
	}

	[
	DesignTimeVisible(false),
	ToolboxItem(false),
	Syncfusion.Documentation.DocumentationExclude()
	]
	public sealed class RenameTextBox : TextBox
	{
		private class MouseMsgFilter : IMessageFilter, IMouseHookHLProcClient
		{
			protected RenameTextBox ctrlOwner = null;

			public MouseMsgFilter(RenameTextBox ctrl)
			{
				this.ctrlOwner = ctrl;
			}

			private void ProcessMouseMessage(Control control, Point ptScreen)
			{
				// Always forward exceptions caught here to the Application class so that Application.ThreadException listeners will 
				// get to handle it.
				try
				{
					if(ctrlOwner.ClientRectangle.Contains(ctrlOwner.PointToClient(ptScreen)) == false)
						ctrlOwner.CancelRename();	
				}
				catch(Exception e)
				{
					Application.OnThreadException(e);
				}
			}
			// Called in a .NET app.
			public bool PreFilterMessage(ref Message m)
			{
				// Cancel edit if a buttondown occurs outside the text box bounds.
				if( m.Msg >= 0x0201/*WM_LBUTTONDOWN*/ && m.Msg <= 0x0209/*WM_MBUTTONDBLCLK*/ ) 					
				{
					Control control = Control.FromHandle(m.HWnd);
					this.ProcessMouseMessage(control, control.PointToScreen(new Point((Int32)m.LParam)));
				}
				return false;
			}

			// Called when hosted in a native app.
			bool IMouseHookHLProcClient.MouseHookProc(int msg, Point point, IntPtr hwnd, int wHitTestCode, int dwExtraInfo)
			{
				if( msg >= 0x0201/*WM_LBUTTONDOWN*/ && msg <= 0x0209/*WM_MBUTTONDBLCLK*/ )
				{
					this.ProcessMouseMessage(Control.FromHandle(hwnd), point);
				}
				return false;
			}
		}	

		private MouseMsgFilter msgFilter = null;
		public event RenameCompleteEventHandler RenameComplete;

		public RenameTextBox()
		{
			this.BorderStyle = BorderStyle.FixedSingle;
			this.Visible = false;	
		}

		public void BeginRename(Rectangle rc, String text)
		{
			if(rc.IsEmpty || (text == null))
			{
				Debug.Assert(false, "Invalid Edit params.\n");
				return;
			}
			this.Text = text;
			this.Bounds = new Rectangle(rc.Left,rc.Top,rc.Width,rc.Height);
			this.Visible = true;
			this.Focus();
			// Create and set up the message filter.
			this.msgFilter = new MouseMsgFilter(this);
			MessageFilterEntryHelper.AddMessageFilter(this.msgFilter, false);
		}

		public String EndRename(bool bcancel)
		{
			MessageFilterEntryHelper.RemoveMessageFilter(this.msgFilter);
			this.msgFilter = null;
			if(this.Visible)
				this.Visible = false;
			String text = null;
			if(bcancel == false)
				text = this.Text;

			// Fire the RenameComplete event.
			if(this.RenameComplete != null)
			{
				this.RenameComplete(this, new RenameCompleteEventArgs(text));
			}

			return text;
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown( e );

			switch( e.KeyCode )
			{
				case Keys.Cancel:
					EndRename( true );
					break;
				case Keys.Escape:
					EndRename( true );
					break;
				case Keys.Enter:					
					EndRename( false );
					break;
				default:
					break;
			}
		}

		protected override void OnLostFocus(System.EventArgs e)
		{
			base.OnLostFocus(e);

			// Terminate the edit, if the sequence is not already underway.
            if( msgFilter != null )
            {
                EndRename( false );
            }
		}

		public void CancelRename()
		{
			EndRename( false );			
		}		
	}


	[Syncfusion.Documentation.DocumentationExclude()]
	public sealed class RenameCompleteEventArgs : EventArgs
	{
		private String strName = String.Empty;
		
		public String NewName
		{
			get { return this.strName; }
		}

		public RenameCompleteEventArgs(String newname)
		{
			this.strName = newname;
		}
	}


	[Syncfusion.Documentation.DocumentationExclude()]
	public delegate void RenameCompleteEventHandler(Object obj, RenameCompleteEventArgs arg);

	/// <summary>
	/// Provides data about the <see cref="GroupBar.GroupBarItemRenamed"/> and 
	/// <see cref="GroupView.GroupViewItemRenamed"/> events.
	/// </summary>
	/// <remarks>
	/// See <see cref="GroupBar"/>, <see cref="GroupView"/>, and <see cref="GroupItemRenamedEventHandler"/>.
	/// </remarks>
	public sealed class GroupItemRenamedEventArgs : EventArgs 
	{
		private int nIndex = -1;
		private String strNew = null;
		private String strOld = null;

		/// <summary>
		/// Returns the zero-based index of the renamed item.
		/// </summary>
		/// <value>An integer representing the item index.</value>
		public int Index
		{
			get { return this.nIndex; }
		}

		/// <summary>
		/// Returns the new text of the item.
		/// </summary>
		/// <remarks>A String value.</remarks>
		public String NewLabel
		{
			get { return this.strNew;	}
		}

		/// <summary>
		/// Returns the old text of the item.
		/// </summary>
		/// <remarks>A String value.</remarks>
		public String OldLabel
		{
			get { return this.strOld; }
		}
		
		/// <summary>
		/// Creates a new instance of the GroupItemRenamedEventArgs class.
		/// </summary>
		/// <param name="nindex">The zero-based index of the renamed item.</param>
		/// <param name="oldtext">The old item text.</param>
		/// <param name="newtext">The new item text.</param>
		public GroupItemRenamedEventArgs(int nindex, String oldtext, String newtext)
		{
			this.nIndex = nindex;
			this.strNew = newtext;
			this.strOld = oldtext;
		}
	}

	/// <summary>
	/// Delegate representing the method that will handle the <see cref="GroupBar.GroupBarItemRenamed"/> and 
	/// <see cref="GroupView.GroupViewItemRenamed"/> events.
	/// </summary>
	/// <param name="obj"> The source of the event.</param>
	/// <param name="arg"> A <see cref="GroupItemRenamedEventArgs"/> value that contains the event data.</param>	
	public delegate void GroupItemRenamedEventHandler(Object obj, GroupItemRenamedEventArgs arg);


	[Syncfusion.Documentation.DocumentationExclude()]
	public interface IGroupViewDesignerInvoke
	{
		void HandleMouseDown(MouseButtons button, Point pt);
		void HandleMouseUp(MouseButtons button, Point pt);
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public interface IGroupBarDesignerInvoke
	{
		int GetGroupAtLocation(Point pt);
	}

	/// <summary>
	/// Determines <see cref="GroupBarItem"> resize mode.
	/// </summary>
	public enum PopupResizeMode
	{
		/// <summary>
		/// Resize is not allowed.
		/// </summary>
		None = 0,
		/// <summary>
		/// Horizontal resize is allowed.
		/// </summary>
		Horizontal = 1,
		/// <summary>
		/// Vertical resize is allowed.
		/// </summary>
		Vertical = 2,
		/// <summary>
		/// Both horizontal and vertical resize are allowed.
		/// </summary>
		Both = Horizontal | Vertical
	}
}