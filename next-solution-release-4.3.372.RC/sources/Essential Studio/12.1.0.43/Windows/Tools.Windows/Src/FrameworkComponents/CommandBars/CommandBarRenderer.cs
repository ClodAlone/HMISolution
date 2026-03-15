#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region File Using
using System;
using System.Drawing;
using System.Windows.Forms;

using Syncfusion.Windows.Forms.Tools.XPMenus;
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Basic class for all renderers. CommandBarRenderer provides the basic functionality 
	/// that is needed by all renderers.
	/// You must inherit from CommandBarRenderer to create your own renderers.
	/// </summary>
	internal abstract class CommandBarRenderer : IDisposable
	{
		#region Class Members
		/// <summary>
		/// Reference to CommandBar.
		/// </summary>
		private CommandBar m_commandBar = null;
		#endregion

		#region Class Properties
		/// <summary>
		/// Gets reference to CommandBar.
		/// </summary>
		protected virtual CommandBar CmdBar
		{
			get
			{
				return m_commandBar;
			}
		}
		#endregion

		#region Class Initialize/Finalize Methods
		public CommandBarRenderer( CommandBar commandBar )
		{
			if( null == commandBar )
				throw new ArgumentNullException();

			m_commandBar = commandBar;
		}

		~CommandBarRenderer()
		{
			Dispose();
		}
		#endregion

		#region Class Utility Methods
		/// <summary>
		/// Draws text for CommandBar.
		/// </summary>
		/// <param name="rect">The rectangle within which to draw text.</param>
		protected abstract void DrawText( Graphics g, Rectangle rect, bool bRTL, bool bVertical );
		/// <summary>
		/// Draws background for CommandBar.
		/// </summary>
		/// <param name="rect">The rectangle to draw background.</param>
		protected abstract void DrawBackground( Graphics g, Rectangle rect, bool bRTL, bool bVerticalt );
		/// <summary>
		/// Draws gripper of the CommanBar.
		/// </summary>
		/// <param name="rect">The rectangle within which to draw gripper.</param>
		protected abstract void DrawGripper( Graphics g, Rectangle rect, bool bRTL, bool bVertical );
		/// <summary>
		/// Draws DropDown button of the CommandBar.
		/// </summary>
		/// <param name="rect">The rectangle within which to draw DropDown button.</param>
		protected abstract void DrawDropDown( Graphics g, Rectangle rect, bool bRTL, bool bVertical,
			CBButtonState state, bool bShowChevron, bool bShowArrow );
		/// <summary>
		/// Gets rectangle of the DropDown button.
		/// </summary>
		protected virtual Rectangle GetDropDownRect()
		{
			Rectangle rect = this.CmdBar.DropDownRect;
			bool bRTL = this.CmdBar.IsRTL;
			bool bVertical = IsVertical();

			if( bVertical )
			{
				rect.Y -= ( bRTL ) ? 0 : 3;
				rect.Height += ( bRTL ) ? 2 : 3;
			}
			else
			{
				rect.X -= ( bRTL ) ? 0 : 2;
				rect.Width += 2;
			}

			return rect;
		}
		/// <summary>
		/// Gets rectangle within which to draw text for docked CommandBar.
		/// </summary>
		protected virtual Rectangle GetTextRectangle()
		{
			return this.CmdBar.GetTextRectangle();
		}
		/// <summary>
		/// Gets rectangle for gripper.
		/// </summary>
		protected virtual Rectangle GetGripperRect()
		{
			return this.CmdBar.ClientRectangle;
		}
		/// <summary>
		/// Indicates whether the CommandBar has MainMenu DockStyle.
		/// </summary>
		protected bool IsMainCommandBar()
		{
			bool bMain = false;
			CommandBarExt commandBarExt = this.CmdBar as CommandBarExt;

			if( commandBarExt != null && commandBarExt.Bar != null 
				&& ( commandBarExt.Bar.BarStyle & BarStyle.IsMainMenu ) > 0 )
			{
				bMain = true;
			}

			return bMain;
		}
		/// <summary>
		/// Indicates whether the CommandBar has vertical DockStyle.
		/// </summary>
		protected bool IsVertical()
		{
			bool bVertical = false;

			if( this.CmdBar.cdbParent != null )
			{
				bVertical = !( ( this.CmdBar.cdbParent.Dock == DockStyle.Top ) 
					|| ( this.CmdBar.cdbParent.Dock == DockStyle.Bottom ) );
			}

			return bVertical;
		}
		#endregion

		#region Class Public Methods
		/// <summary>
		/// Draws CommandBar.
		/// </summary>
		public virtual void Draw( Graphics g )
		{
			bool bRTL = this.CmdBar.IsRTL;
			bool bVertical = IsVertical();

			// draw background
			Rectangle clientRect = this.CmdBar.ClientRectangle;
			DrawBackground( g, clientRect, bRTL, bVertical );
			
			if( !m_commandBar.bHideDropDown )
			{
				// draw DropDown button
				CBButtonState state = this.CmdBar.GetDropDownState();
				bool bShowChevron = this.CmdBar.IsShowChevron();
				Rectangle dropDownRect = GetDropDownRect();
				DrawDropDown( g, dropDownRect, bRTL, bVertical, state, bShowChevron,
					this.CmdBar.AllowQuickCustomizing || this.CmdBar.HasExternalPopupMenu );
			}

			if( m_commandBar.IsShowText() || this.CmdBar.cbarDockState == CommandBarDockState.Float )
			{
				// draw text
				Rectangle textRect = GetTextRectangle();
				DrawText( g, textRect, bRTL, bVertical );
			}

			if( !m_commandBar.bHideGripper )
			{
				// draw gripper
				Rectangle gripperRect = GetGripperRect();
				DrawGripper( g, gripperRect, bRTL, bVertical );
			}
		}
		#endregion

		#region IDisposable
		private bool m_bDisposed = false;
		public void Dispose()
		{
			if( !m_bDisposed )
			{
				OnDispose( m_bDisposed );
				GC.SuppressFinalize( this );
				m_bDisposed = true;
			}
		}

		protected virtual void OnDispose( bool diposing )
		{
			// for inheritors only...
		}

		#endregion
	}

	/// <summary>
	/// Basic class for floating renderers. CommandBarFloatingRenderer provides the basic functionality 
	/// that is needed by floating renderers.
	/// You must inherit from CommandBarFloatingRenderer to create your own floating renderers.
	/// </summary>
	internal abstract class CommandBarFloatingRenderer : CommandBarRenderer
	{
		#region Class Initialize/Finalize Methods
		public CommandBarFloatingRenderer( CommandBar commandBar ) : base( commandBar )
		{}
		#endregion

		#region Class Utiliry Methods
		/// <summary>
		/// Draws close button of the CommandBar.
		/// </summary>
		/// <param name="rect">The rectangle within which to draw close button.</param>
		protected abstract void DrawCloseButton( Graphics g, Rectangle rect, CBButtonState state );
		/// <summary>
		/// Gets rectangle for close button.
		/// </summary>
		private Rectangle GetCloseButtonRect()
		{
			Rectangle rect = this.CmdBar.CloseButtonRect;
			rect.Width -= 1;
			rect.Height -= 1;
			return rect;
		}
		#endregion

		#region Class Overrides
		/// <summary>
		/// Draws floating CommandBar.
		/// </summary>
		public override void Draw( Graphics g )
		{
			base.Draw( g );

            if( !IsMainCommandBar() && !this.CmdBar.HideCloseButton )
            {
				CBButtonState state = this.CmdBar.GetCloseButtonState();
               	Rectangle closeButtonRect = GetCloseButtonRect();
				DrawCloseButton( g, closeButtonRect, state );
            }
		}
		/// <summary>
		/// Gets rectangle within which to draw text for floating CommandBar.
		/// </summary>
		/// <returns></returns>
		protected override Rectangle GetTextRectangle()
		{
			return this.CmdBar.GetFloatingTextRectangle();
		}

		/// <summary>
		/// Gets rectangle of the DropDown button for floationg CommandBar.
		/// </summary>
		protected override Rectangle GetDropDownRect()
		{
			return this.CmdBar.DropDownRect;
		}

		#endregion
	}

	/// <summary>
	/// Basic class for ControlBar renderers. ControlBarRenderer provides the basic functionality 
	/// that is needed by ControlBar renderers.
	/// You must inherit from ControlBarRenderer to create your own ControlBar renderers.
	/// </summary>
	internal abstract class ControlBarRenderer : CommandBarFloatingRenderer
	{
		#region Class Members
		/// <summary>
		/// Reference to ControlBar.
		/// </summary>
		private ControlBar m_controlBar = null;
		#endregion

		#region Class Properties
		/// <summary>
		/// Gets reference to ControlBar.
		/// </summary>
		protected ControlBar ControlBar
		{
			get
			{
				return m_controlBar;
			}
		}
		#endregion

		#region Class Initialize/Finalize Methods
		public ControlBarRenderer( ControlBar controlBar ) : base( controlBar )
		{
			if( null == controlBar )
				throw new ArgumentNullException();

			m_controlBar = controlBar;
		}
		#endregion

		#region Class Overrides
		/// <summary>
		/// Gets rectangle for gripper of the ControlBar.
		/// </summary>
		protected override Rectangle GetGripperRect()
		{
			Rectangle rect = this.ControlBar.GripperRect;
			rect.Height += 1;

			return rect;
		}

		/// <summary>
		/// Gets rectangle within which to draw text for ControlBar.
		/// </summary>
		protected override Rectangle GetTextRectangle()
		{
			return this.ControlBar.GetTextRectangle();
		}

		#endregion
	}
}
