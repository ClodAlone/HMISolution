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
using System.Drawing.Drawing2D;
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Represents renderer of the OfficeXP visual style for docked CommandBar.
	/// </summary>
	internal class CommandBarRendererOfficeXP : CommandBarRenderer
	{
		#region Class Initialize/Finalize Methods
		public CommandBarRendererOfficeXP( CommandBar commandBar ) : base( commandBar )
		{
		}
		#endregion

		#region Class Overrides
		/// <summary>
		/// Draws text of the CommandBar with OfficeXP visual style.
		/// </summary>
		protected override void DrawText( Graphics g, Rectangle rect, bool bRTL, bool bVertical )
		{
			string text = this.CmdBar.Text;
			Font textFont = this.CmdBar.Font;
			Color foreColor = this.CmdBar.ForeColor;
			CommandBarPainter.PaintDockedText( g, rect, text, textFont, foreColor, bRTL, bVertical );
		}
		/// <summary>
		/// Draws background of the CommandBar with OfficeXP visual style.
		/// </summary>
		protected override void DrawBackground( Graphics g, Rectangle rect, bool bRTL, bool bVertical )
		{
			Color backColor = this.CmdBar.cdbParent.BackColor;
			DockStyle dockState = ( this.CmdBar.cdbParent == null ) ? DockStyle.Top : 
				this.CmdBar.cdbParent.Dock;
			// For a Menu-type Bar on row 0 - don't paint the top line
			bool bDrawTopLine = (( this.CmdBar.nRCIndex == 0
					&& this.CmdBar.bFullRow 
					&& this.CmdBar.bDisableFloating && this.CmdBar.bHideChevron 
					&&	this.CmdBar.bHideDropDown && this.CmdBar.bHideGripper ) == false );
			
			CommandBarPainter.PaintDockedBackgroundOfficeXP( g, bRTL, dockState, rect, 
				backColor, bDrawTopLine );
		}

		/// <summary>
		/// Draws DropBownButton of the CommandBar with OfficeXP visual style.
		/// </summary>
		protected override void DrawDropDown( Graphics g, Rectangle rect, bool bRTL, bool bVertical,
			CBButtonState state, bool bShowChevron, bool bShowArrow )
		{
			Color chevronColor = this.CmdBar.ChevronColor;
			Color borderColor = Color.Empty;
			Color fillColor = Color.Empty;

			switch( state )
			{
				case CBButtonState.Hot :
				{
					borderColor = MenuColors.SelBorderColor;
					fillColor = MenuColors.SelColor;
					break;
				}
				case CBButtonState.Pressed :
				{
					borderColor = MenuColors.DropDownBorderColor;
					fillColor = this.CmdBar.BackColor;
					break;
				}
				default : 
				{
					borderColor = Color.Empty;
					fillColor = Color.Empty;
					break;
				}
			}

			CommandBarPainter.PaintDropDownButtonOfficeXP( g, rect, bRTL, bVertical, bShowChevron,
				borderColor, fillColor, chevronColor, bShowArrow );
		}
		/// <summary>
		/// Draws gripper of the CommandBar with OfficeXP visual style.
		/// </summary>
		protected override void DrawGripper( Graphics g, Rectangle rect, bool bRTL, bool bVertical )
		{
			Color gripperColor = MenuColors.FloatingCommandBarCaptionColor;
			DockStyle dockState = ( this.CmdBar.cdbParent == null ) ? DockStyle.Top : 
				this.CmdBar.cdbParent.Dock;

			CommandBarPainter.PaintGripperOfficeXP( g, rect, bRTL, dockState, gripperColor );
		}

		/// <summary>
		/// Gets rectangle of the DropDown button for OfficeXP visual style.
		/// </summary>
		protected override Rectangle GetDropDownRect()
		{
			return this.CmdBar.DropDownRect;
		}

		#endregion
	}

	/// <summary>
	/// Represents renderer of the OfficeXP visual style for floating CommandBar.
	/// </summary>
	internal class CommandBarFloatingRendererOfficeXP : CommandBarFloatingRenderer
	{
		#region Class Initialize/Finalize Methods
		public CommandBarFloatingRendererOfficeXP( CommandBar commandBar ) : base( commandBar )
		{
		}
		#endregion

		#region Class Overrides
		/// <summary>
		/// Draws text of the CommandBar with OfficeXP visual style.
		/// </summary>
		protected override void DrawText( Graphics g, Rectangle rect, bool bRTL, bool bVertical )
		{
			string text = this.CmdBar.Text;
			Color foreColor = SystemColors.ControlText;
			Font textFont = this.CmdBar.GetFloatinCaptionFont();
			CommandBarPainter.PaintFloatingText( g, rect, text, textFont, foreColor, bRTL );
		}
		/// <summary>
		/// Draws background of the CommandBar with OfficeXP visual style.
		/// </summary>
		protected override void DrawBackground( Graphics g, Rectangle rect, bool bRTL, bool bVertical )
		{
			Color backColor = MenuColors.FloatingCommandBarCaptionColor;
			Color borderColor = MenuColors.CommandBarBackColor;
			Color startColor = Color.Empty;
			Color endColor = Color.Empty;

			Rectangle captionRect = this.CmdBar.CaptionRect;
			Rectangle barRect = new Rectangle( rect.Left + 1, 
				rect.Top + captionRect.Height + 1, 
				rect.Width - 2, rect.Height - 2 );

			CommandBarPainter.PaintFloatingBackground( g, rect, barRect, 
				backColor, borderColor, startColor, endColor );
		}

		/// <summary>
		/// Draws DropBownButton of the CommandBar with OfficeXP visual style.
		/// </summary>
		protected override void DrawDropDown( Graphics g, Rectangle rect, bool bRTL, bool bVertical,
			CBButtonState state, bool bShowChevron, bool bShowArrow )
		{
			Color borderColor = Color.Empty;
			Color fillColor = Color.Empty;
			Color buttonColor = SystemColors.ControlText;

			switch( state )
			{
				case CBButtonState.Hot :
				{
					borderColor = MenuColors.SelBorderColor;
					fillColor = MenuColors.SelColor;
					break;
				}
				case CBButtonState.Pressed :
				{
					borderColor = MenuColors.DropDownBorderColor;
					fillColor = this.CmdBar.BackColor;
					break;
				}
				default :
				{
					borderColor = Color.Empty;
					fillColor = Color.Empty;
					break;
				}
			}

			CommandBarPainter.PaintFloatingDropDownButton( g, rect, borderColor, fillColor, buttonColor );
		}
		/// <summary>
		/// Draws gripper of the CommandBar with OfficeXP visual style.
		/// </summary>
		protected override void DrawGripper( Graphics g, Rectangle rect, bool bRTL, bool bVertical )
		{
			// doesn't need draw gripper
		}
		/// <summary>
		/// Draws close button of the floating CommandBar.
		/// </summary>
		protected override void DrawCloseButton( Graphics g, Rectangle rect, CBButtonState state )
		{
			Color borderColor = Color.Empty;
			Color fillColor = Color.Empty;
			Color buttonColor = SystemColors.ControlText;

			switch( state )
			{
				case CBButtonState.Hot :
				{
					borderColor = MenuColors.SelBorderColor;
					fillColor = MenuColors.SelColor;
					break;
				}
				case CBButtonState.Pressed :
				{
					borderColor = MenuColors.DropDownBorderColor;
					fillColor = MenuColors.PressedSelColor;
					break;
				}
				default :
				{
					borderColor = Color.Empty;
					fillColor = Color.Empty;
					break;
				}
			}

			CommandBarPainter.PaintFloatingCloseButton( g, rect, borderColor, fillColor, buttonColor );
		}
		#endregion
	}

	/// <summary>
	/// Represents renderer of the OfficeXP visual style for docked ControlBar.
	/// </summary>
	internal class ControlBarRendererOfficeXP : ControlBarRenderer
	{
		#region Class Initialize/Finalize Methods
		public ControlBarRendererOfficeXP( ControlBar controlBar ) : base( controlBar )
		{
		}
		#endregion

		#region Class Overrides
		/// <summary>
		/// Draws text of the ControlBar with OfficeXP visual style.
		/// </summary>
		protected override void DrawText(Graphics g, Rectangle rect, bool bRTL, bool bVertical)
		{
			string text = this.ControlBar.Text;
			Color foreColor = this.ControlBar.ForeColor;
			Font textFont = this.ControlBar.Font;
			CommandBarPainter.PaintControlBarText( g, rect, text, textFont, foreColor, bRTL );
		}

		/// <summary>
		/// Draws background of the ControlBar with OfficeXP visual style.
		/// </summary>
		protected override void DrawBackground(Graphics g, Rectangle rect, bool bRTL, bool bVerticalt)
		{
			if( this.ControlBar.ClientControl != null && 
				this.ControlBar.ClientControl.ContainsFocus == true )
			{
				Rectangle captionRect = this.CmdBar.CaptionRect;

				using( Brush captionBrush = new SolidBrush(SystemColors.ActiveCaption ) )
				{
					g.FillRectangle(captionBrush, captionRect);
				}
			}
		}

		/// <summary>
		/// Draws DropBownButton of the ControlBar with OfficeXP visual style.
		/// </summary>
		protected override void DrawDropDown( Graphics g, Rectangle rect, bool bRTL, bool bVertical, Syncfusion.Windows.Forms.Tools.CBButtonState state, bool bShowChevron, bool bShowArrow )
		{
			Color borderColor = Color.Empty;
			Color fillColor = Color.Empty;

			if( state == CBButtonState.Hot )
			{
				borderColor = MenuColors.SelBorderColor;
				fillColor = MenuColors.SelColor;
			}
			else if( state == CBButtonState.Pressed )
			{
				borderColor = MenuColors.DropDownBorderColor;
				fillColor = SystemColors.Control;
			}

			CommandBarPainter.PaintControlBarDropDown( g, rect, bRTL, borderColor, fillColor );
		}

		/// <summary>
		/// Draws close button of the ControlBar.
		/// </summary>
		protected override void DrawCloseButton(Graphics g, Rectangle rect, Syncfusion.Windows.Forms.Tools.CBButtonState state)
		{
			Color borderColor = Color.Empty;
			Color fillColor = Color.Empty;

			if( state == CBButtonState.Hot )
			{
				borderColor = MenuColors.SelBorderColor;
				fillColor = MenuColors.SelColor;
			}
			else if( state == CBButtonState.Pressed )
			{
				borderColor = MenuColors.DropDownBorderColor;
				fillColor = MenuColors.PressedSelColor;
			}

			CommandBarPainter.PaintControlBarCloseButton( g, rect, borderColor, fillColor );
		}
		/// <summary>
		/// Draws gripper of the ControlBar with OfficeXP visual style.
		/// </summary>
		protected override void DrawGripper(Graphics g, Rectangle rect, bool bRTL, bool bVertical)
		{
			Color gripperColor = MenuColors.FloatingCommandBarCaptionColor;
			int captionHeight = this.ControlBar.ControlBarCaptionHeight;
			bool bFloating = this.CmdBar.Floating;

			CommandBarPainter.PaintControlBarGripperOfficeXP( g, rect, captionHeight, bRTL, 
				bFloating, gripperColor );
		}

		#endregion
	}
}
