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
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Represents renderer of the VS2005 visual style for docked CommandBar.
	/// </summary>
	internal class CommandBarRendererVS2005 : CommandBarRenderer
	{
		#region Class Initialize/Finalize Methods
		public CommandBarRendererVS2005( CommandBar commandBar ) : base( commandBar )
		{
		}
		#endregion

		#region Class Overrides
		/// <summary>
		/// Draws text of the CommandBar with VS2005 visual style.
		/// </summary>
		protected override void DrawText( Graphics g, Rectangle rect, bool bRTL, bool bVertical )
		{
			string text = this.CmdBar.Text;
			Font textFont = this.CmdBar.Font;
			Color foreColor = this.CmdBar.ForeColor;
			CommandBarPainter.PaintDockedText( g, rect, text, textFont, foreColor, bRTL, bVertical );
		}
		/// <summary>
		/// Draws background of the CommandBar with VS2005 visual style.
		/// </summary>
		protected override void DrawBackground( Graphics g, Rectangle rect, bool bRTL, bool bVertical )
		{
			if( !IsMainCommandBar() )
			{
				CommandBarPainter.PaintDockedBackground( g, bRTL, bVertical, rect,
					VS2005Colors.CommandBarLightColor,
					VS2005Colors.CommandBarDarkColor,
					VS2005Colors.CommandBarBorderColor );
			}
		}

		/// <summary>
		/// Draws DropBownButton of the CommandBar with VS2005 visual style.
		/// </summary>
		protected override void DrawDropDown( Graphics g, Rectangle rect, bool bRTL, bool bVertical,
			CBButtonState state, bool bShowChevron, bool bShowArrow )
		{
			Color chevronColor = this.CmdBar.ChevronColor;
			Color startColor = Color.Empty;
			Color endColor = Color.Empty;

			switch( state )
			{
				case CBButtonState.Hot :
				{
					startColor = ( bVertical && bRTL ) ? VS2005Colors.DropDownHighlightDarkColor : 
						VS2005Colors.DropDownHighlightLightColor;
					endColor = ( bVertical && bRTL ) ? VS2005Colors.DropDownHighlightLightColor : 
						VS2005Colors.DropDownHighlightDarkColor;
					break;
				}
				case CBButtonState.Pressed :
				{
					startColor = ( bVertical && bRTL ) ? VS2005Colors.DropDownPressedDarkColor : 
						VS2005Colors.DropDownPressedLightColor;
					endColor = ( bVertical && bRTL ) ? VS2005Colors.DropDownPressedLightColor : 
						VS2005Colors.DropDownPressedDarkColor;
					break;
				}
				default : 
				{
					startColor = ( bVertical && bRTL ) ? VS2005Colors.CommandBarDropDownDarkColor : 
						VS2005Colors.CommandBarDropDownLightColor;
					endColor = ( bVertical && bRTL ) ? VS2005Colors.CommandBarDropDownLightColor : 
						VS2005Colors.CommandBarDropDownDarkColor;
					break;
				}
			}

			CommandBarPainter.PaintDropDownButton( g, rect, bRTL, bVertical, bShowChevron,
				startColor, endColor, chevronColor, bShowArrow );
		}
		/// <summary>
		/// Draws gripper of the CommandBar with VS2005 visual style.
		/// </summary>
		protected override void DrawGripper( Graphics g, Rectangle rect, bool bRTL, bool bVertical )
		{
			CommandBarPainter.PaintGripper( g, rect, bRTL, bVertical, 
				Color.White, Office2003Colors.ControlGripperColor );
		}

		#endregion
	}

	/// <summary>
	/// Represents renderer of the VS2005 visual style for floating CommandBar.
	/// </summary>
	internal class CommandBarFloatingRendererVS2005 : CommandBarFloatingRenderer
	{
		#region Class Initialize/Finalize Methods
		public CommandBarFloatingRendererVS2005( CommandBar commandBar ) : base( commandBar )
		{
		}
		#endregion

		#region Class Overrides
		/// <summary>
		/// Draws text of the CommandBar with VS2005 visual style.
		/// </summary>
		protected override void DrawText( Graphics g, Rectangle rect, bool bRTL, bool bVertical )
		{
			string text = this.CmdBar.Text;
			Color foreColor = VS2005Colors.FloatCaptionColor;
			Font textFont = this.CmdBar.GetFloatinCaptionFont();
			CommandBarPainter.PaintFloatingText( g, rect, text, textFont, foreColor, bRTL );
		}
		/// <summary>
		/// Draws background of the CommandBar with VS2005 visual style.
		/// </summary>
		protected override void DrawBackground( Graphics g, Rectangle rect, bool bRTL, bool bVertical )
		{
			Color backColor = VS2005Colors.FloatBackgroundColor;
			Color startColor = VS2005Colors.FloatCommandBarLightColor;
			Color endColor = VS2005Colors.FloatCommandBarDarkColor;
			Color borderColor = VS2005Colors.FloatLightBorderColor;

			Rectangle captionRect = this.CmdBar.CaptionRect;
			Rectangle barRect = new Rectangle( rect.Left + 1, 
				rect.Top + captionRect.Height + 1, 
				rect.Width - 2, rect.Height - 2 );

			CommandBarPainter.PaintFloatingBackground( g, rect, barRect, 
				backColor, borderColor, startColor, endColor );
		}

		/// <summary>
		/// Draws DropBownButton of the CommandBar with VS2005 visual style.
		/// </summary>
		protected override void DrawDropDown( Graphics g, Rectangle rect, bool bRTL, bool bVertical,
			CBButtonState state, bool bShowChevron, bool bShowArrow )
		{
			Color borderColor = Color.Empty;
			Color fillColor = Color.Empty;
			Color buttonColor = Color.Empty;

			switch( state )
			{
				case CBButtonState.Hot :
				{
					borderColor = VS2005Colors.MenuSelectedItemBorderColor;
					fillColor = VS2005Colors.MenuSelectedItemColor;
					buttonColor = SystemColors.ControlText;
					break;
				}
				case CBButtonState.Pressed :
				{
					borderColor = VS2005Colors.FloatPressButtonBorderColor;
					fillColor = VS2005Colors.FloatPressButtonColor;
					buttonColor = SystemColors.ControlText;
					break;
				}
				default :
				{
					borderColor = Color.Empty;
					fillColor = Color.Empty;
					buttonColor = Color.White;
					break;
				}
			}

			CommandBarPainter.PaintFloatingDropDownButton( g, rect, borderColor, fillColor, buttonColor );
		}
		/// <summary>
		/// Draws gripper of the CommandBar with VS2005 visual style.
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
			Color buttonColor = Color.Empty;

			switch( state )
			{
				case CBButtonState.Hot :
				{
					borderColor = VS2005Colors.MenuSelectedItemBorderColor;
					fillColor = VS2005Colors.MenuSelectedItemColor;
					buttonColor = SystemColors.ControlText;
					break;
				}
				case CBButtonState.Pressed :
				{
					borderColor = VS2005Colors.BarItemPressBorderColor;
					fillColor = VS2005Colors.BarItemPressLightColor;
					buttonColor = SystemColors.ControlText;
					break;
				}
				default :
				{
					borderColor = Color.Empty;
					fillColor = Color.Empty;
					buttonColor = Color.White;
					break;
				}
			}

			CommandBarPainter.PaintFloatingCloseButton( g, rect, borderColor, fillColor, buttonColor );
		}
		#endregion
	}

	/// <summary>
	/// Represents renderer of the VS2005 visual style for docked ControlBar.
	/// </summary>
	internal class ControlBarRendererVS2005 : ControlBarRenderer
	{
		#region Class Initialize/Finalize Methods
		public ControlBarRendererVS2005( ControlBar controlBar ) : base( controlBar )
		{
		}
		#endregion

		#region Class Overrides
		/// <summary>
		/// Draws text of the ControlBar with VS2005 visual style.
		/// </summary>
		protected override void DrawText(Graphics g, Rectangle rect, bool bRTL, bool bVertical)
		{
			string text = this.ControlBar.Text;
			Color foreColor = this.ControlBar.ForeColor;
			Font textFont = this.ControlBar.Font;
			CommandBarPainter.PaintControlBarText( g, rect, text, textFont, foreColor, bRTL );
		}

		/// <summary>
		/// Draws background of the ControlBar with VS2005 visual style.
		/// </summary>
		protected override void DrawBackground(Graphics g, Rectangle rect, bool bRTL, bool bVerticalt)
		{
			Color borderColor = VS2005Colors.CommandBarLightColor;
			Color lightColor = VS2005Colors.CommandBarLightColor;
			Color darkColor = VS2005Colors.CommandBarDarkColor;
			Color activeLightColor = VS2005Colors.BarItemHighlightLightColor;
			Color activeDarkColor = VS2005Colors.BarItemHighlightDarkColor;

			Rectangle captionRect = this.ControlBar.CaptionRect;
			bool bContainFocus = ( this.ControlBar.ClientControl != null ) 
				&& ( this.ControlBar.ClientControl.ContainsFocus == true );

			CommandBarPainter.PaintControlBarBackground( g, rect, captionRect, bRTL, bContainFocus,
				borderColor, lightColor, darkColor, activeLightColor, activeDarkColor );
		}

		/// <summary>
		/// Draws DropBownButton of the ControlBar with VS2005 visual style.
		/// </summary>
		protected override void DrawDropDown(
			Graphics g, Rectangle rect, bool bRTL, bool bVertical, Syncfusion.Windows.Forms.Tools.CBButtonState state, bool bShowChevron, bool bShowArrow )
		{
			Color borderColor = Color.Empty;
			Color fillColor = Color.Empty;

			if( state == CBButtonState.Hot )
			{
				borderColor = VS2005Colors.MenuSelectedItemBorderColor;
				fillColor = VS2005Colors.MenuSelectedItemColor;
			}
			else if( state == CBButtonState.Pressed )
			{
				borderColor = VS2005Colors.FloatPressButtonBorderColor;
				fillColor = VS2005Colors.FloatPressButtonColor;
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

			switch( state )
			{
				case CBButtonState.Hot :
				{
					borderColor = VS2005Colors.MenuSelectedItemBorderColor;
					fillColor = VS2005Colors.MenuSelectedItemColor;
					break;
				}
				case CBButtonState.Pressed :
				{
					borderColor = VS2005Colors.BarItemPressBorderColor;
					fillColor = VS2005Colors.BarItemPressLightColor;
					break;
				}
				default :
				{
					borderColor = Color.Empty;
					fillColor = Color.Empty;
					break;
				}
			}

			CommandBarPainter.PaintControlBarCloseButton( g, rect, borderColor, fillColor );
		}
		/// <summary>
		/// Draws gripper of the ControlBar with VS2005 visual style.
		/// </summary>
		protected override void DrawGripper(Graphics g, Rectangle rect, bool bRTL, bool bVertical)
		{
			CommandBarPainter.PaintGripper( g, rect, bRTL, false, 
				Color.White, Office2003Colors.ControlGripperColor );
		}

		#endregion
	}
}
