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

#region file using directives
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Tools;
#endregion

namespace Syncfusion.Drawing
{
	/// <summary>
	/// This class allows users to simply draw chevron button with different Themes and Styles.
	/// </summary>
	public class ChevronPainter
	{
		#region Class constants
		/// <summary>
		/// Chevron button border width.
		/// </summary>
		public const int DEF_BORDER_WIDTH = 1;

		private const int DEF_ARROWS_COUNT = 2;
		private const int DEF_ARROWS_SPACE_X = 4;
		private const int DEF_CHEVRON_OFFSET_X_OFFICEXP = 2;
		private const int DEF_CHEVRON_OFFSET_X_OFFICE2003 = 2;
		private const int DEF_CHEVRON_OFFSET_Y_OFFICE2003 = 5;

		private const int DEF_CHEVRON_OFFSET_X_COMMON = 1;
		private const int DEF_CHEVRON_OFFSET_X_COMMON_RIGHT = 3;
		private const int DEF_CHEVRON_OFFSET_Y_COMMON = 3;

		private const int DEF_ARROWS_WIDTH = 4;
		private const int DEF_ARROWS_HEIGHT = 5;

		private const float DEF_SELECTIONCOLOR_PERCENT = 0.3f;
		private const float DEF_BACKCOLOR_PERCENT = 1.3f;

		#endregion

		#region Class members
		private static ThemedControlDrawing m_themedPainter = new ThemedControlDrawing( ThemedControls.TOOLBAR );
		#endregion

		#region Chevron drawing implementation
		/// <summary>
		/// Draws chevron button.
		/// </summary>
		/// <param name="g"> Graphics object to draw on. </param>
		/// <param name="chevronBounds"> Chevron button bounds. </param>
		/// <param name="style"> Visual style to draw chevron with. </param>
		/// <param name="state"> Button state to draw chevron with. </param>
		/// <param name="isRightToLeft"> Indicates whether to draw chevron in RightToLeft mode. </param>
		public static void DrawChevronButton( Graphics g, Rectangle chevronBounds,
			VisualStyle style, ButtonsState state, bool isRightToLeft )
		{     
			if( g == null )
				throw new ArgumentNullException( "g" );

			DrawDropDownButtonHighlight( g, chevronBounds, style, state );
      
			DrawChevron( g, chevronBounds, style, isRightToLeft );
		}
    
		/// <summary>
		/// Draws highlighted chevron background with specified style, button state and themes used.
		/// </summary>
		public static void DrawDropDownButtonHighlight( Graphics g, Rectangle chevronBounds, 
			VisualStyle style, ButtonsState state )
		{
			if( g == null )
				throw new ArgumentNullException( "g" );

			if( state != ButtonsState.None )
			{
				switch( style )
				{
					case VisualStyle.Office2003:
						DrawHighlightOffice2003( g, chevronBounds, state );
						break;

					case VisualStyle.VS2005:
						DrawHighlightVS2005( g, chevronBounds, state );
						break;

					case VisualStyle.OfficeXP:
						DrawHighlightThemed( g, chevronBounds, state );
						break;

					default:
						DrawHightlightCommon( g, chevronBounds, state );
						break;
				
				}
			}
		}

		/// <summary>
		/// Draws chevron arrows with specified style.
		/// </summary>
		public static void DrawChevron( Graphics g, Rectangle chevronButtonBounds, 
			VisualStyle style, bool isRightToLeft )
		{
			Rectangle chevronArrowsBounds = chevronButtonBounds;

			switch( style )
			{
				case VisualStyle.Office2003:
					DrawChevronOffice2003( g, chevronArrowsBounds, isRightToLeft );
					break;
				
				case VisualStyle.VS2005:
					DrawChevronOffice2003( g, chevronArrowsBounds, isRightToLeft );
					break;

				default:
					DrawChevronCommon( g, chevronArrowsBounds, style, isRightToLeft );
					break;
			}
		}

		#endregion

		#region Class utility methods
		/// <summary>
		/// Blends 30% of menu selection color.
		/// </summary>
		public static Color BlendColor( Color selectionColor, Color backColor )
		{
			Color blendedColor = Color.FromArgb(
				( int )( ( DEF_SELECTIONCOLOR_PERCENT * selectionColor.R + backColor.R ) / DEF_BACKCOLOR_PERCENT ),
				( int )( ( DEF_SELECTIONCOLOR_PERCENT * selectionColor.G + backColor.G ) / DEF_BACKCOLOR_PERCENT ),
				( int )( ( DEF_SELECTIONCOLOR_PERCENT * selectionColor.B + backColor.B ) / DEF_BACKCOLOR_PERCENT ) );

			return blendedColor;
		}

		/// <summary>
		/// Draws highlighted chevron background in Office2003 style.
		/// </summary>
		private static void DrawHighlightOffice2003( Graphics g, Rectangle chevronBounds,
			ButtonsState state )
		{
			Color selectionColor = MenuColors.SelColor;
			Color backColorTop = Color.Empty;

			switch( state )
			{
				case ButtonsState.DropDownButtonHot :           					
					backColorTop = Office2003Colors.SelColor;
					break;

				case ButtonsState.DropDownButtonPushed:
					backColorTop = Office2003Colors.PressedSelColor;           
					break;
			}
        						
			backColorTop = BlendColor( selectionColor, backColorTop );
			Color backColorBottom = Office2003Colors.MenuItemHotColorDark;
			using( Brush brush = new LinearGradientBrush( chevronBounds, backColorTop, backColorBottom, LinearGradientMode.Vertical ) )
			{
				g.FillRectangle( brush, chevronBounds );
			}

			Color borderColor = Office2003Colors.FloatingCommandBarCaptionColor;
			using( Pen borderPen = new Pen( borderColor, DEF_BORDER_WIDTH ) )
			{
				g.DrawRectangle( borderPen, chevronBounds );
			}
		}

		/// <summary>
		/// Draws highlighted chevron background in VS2005 style.
		/// </summary>
		private static void DrawHighlightVS2005( Graphics g, Rectangle chevronBounds,
			ButtonsState state )
		{
			Color borderColor = VS2005Colors.MenuSelectedItemBorderColor;
			Color lightColor = Color.Empty;
			Color darkColor = Color.Empty;

			if( state == ButtonsState.DropDownButtonPushed )
			{
				lightColor = VS2005Colors.DropDownPressedLightColor;
				darkColor = VS2005Colors.DropDownPressedDarkColor;
			}
			else
			{
				lightColor = VS2005Colors.DropDownHighlightLightColor;
				darkColor = VS2005Colors.DropDownHighlightDarkColor;
			}

			using( Brush brush = new LinearGradientBrush( chevronBounds, lightColor, 
					   darkColor, LinearGradientMode.Vertical ) )
			{
				g.FillRectangle( brush, chevronBounds );
			}

			using( Pen borderPen = new Pen( borderColor, DEF_BORDER_WIDTH ) )
			{
				g.DrawRectangle( borderPen, chevronBounds );
			}
		}

		/// <summary>
		/// Draws highlighted chevron background using Themes.
		/// </summary>
		private static void DrawHighlightThemed( Graphics g, Rectangle chevronBounds,
			ButtonsState state )
		{
			int themeState = ThemeStates.TS_NORMAL;
			switch( state )
			{
				case ButtonsState.DropDownButtonHot :
					themeState = ThemeStates.TS_HOT;
					break;

				case ButtonsState.DropDownButtonPushed :
					themeState = ThemeStates.TS_PRESSED;
					break;
			}

			lock( m_themedPainter )
			{
				m_themedPainter.DrawThemeBackground( g, ThemeParts.TP_BUTTON, themeState, chevronBounds );
			}
		}

		/// <summary>
		/// Draws common highlighted chevron background, if Style and Themes are not used.
		/// </summary>
		private static void DrawHightlightCommon( Graphics g, Rectangle chevronBounds, 
			ButtonsState state )
		{
			Color borderColor = Color.Empty;
			Color fillColor = Color.Empty;

			switch( state )
			{
				case ButtonsState.DropDownButtonHot :
					borderColor = MenuColors.SelBorderColor;
					fillColor = MenuColors.SelColor;
					break;

				case ButtonsState.DropDownButtonPushed :
					borderColor = MenuColors.DropDownBorderColor;
					break;
			}
			
			if( fillColor != Color.Empty )
			{
				using( SolidBrush fillBrush = new SolidBrush( fillColor ) )
				{
					g.FillRectangle( fillBrush, chevronBounds );
				}
			}

			if( borderColor != Color.Empty )
			{
				using( Pen borderPen = new Pen( borderColor, DEF_BORDER_WIDTH ) )
				{
					g.DrawRectangle( borderPen, chevronBounds );
				}
			}
		}

		/// <summary>
		/// Draws chevron arrows using Office2003 style.
		/// </summary>
		private static void DrawChevronOffice2003( Graphics g, Rectangle chevronBounds, bool isRightToLeft )
		{
			using( Pen whitePen = new Pen( Color.White ) )
			{
				Point arrowsLeft = new Point( chevronBounds.X + DEF_CHEVRON_OFFSET_X_OFFICE2003, 
					chevronBounds.Y + DEF_CHEVRON_OFFSET_Y_OFFICE2003 );
      
				using( isRightToLeft ? new GraphicsAutoMirrorX( g, chevronBounds.Width - 
					chevronBounds.X + DEF_CHEVRON_OFFSET_X_OFFICE2003 + 
					DEF_CHEVRON_OFFSET_X_COMMON_RIGHT ) : null )
				{
					for( int i = 0; i < DEF_ARROWS_COUNT; i++ )
					{
						g.DrawLine( SystemPens.ControlText, arrowsLeft, 
							new Point( arrowsLeft.X, arrowsLeft.Y + 2 ) );

						g.DrawLine( SystemPens.ControlText, 
							new Point( arrowsLeft.X, arrowsLeft.Y + 1 ), 
							new Point( arrowsLeft.X + 1, arrowsLeft.Y + 1 ) );

						g.DrawLine( whitePen, 
							new Point( arrowsLeft.X + 1, arrowsLeft.Y + 2 ), 
							new Point( arrowsLeft.X + 1, arrowsLeft.Y + 3 ) );

						g.DrawLine( whitePen, 
							new Point( arrowsLeft.X + 1, arrowsLeft.Y + 2 ), 
							new Point(arrowsLeft.X + 2, arrowsLeft.Y + 2 ) );

						arrowsLeft.Offset( DEF_ARROWS_SPACE_X, 0 );
					}
				}
			}
		}

		/// <summary>
		/// Draws chevron arrows using Themes or default style.
		/// </summary>
		private static void DrawChevronCommon( Graphics g, Rectangle chevronButtonBounds, 
			VisualStyle style, bool isRightToLeft )
		{
			int chevronOffsetX = ( style == VisualStyle.OfficeXP ) ? 
			DEF_CHEVRON_OFFSET_X_OFFICEXP : DEF_CHEVRON_OFFSET_X_COMMON;

			Rectangle chevronArrowBounds = chevronButtonBounds;
			chevronButtonBounds.Offset( chevronOffsetX, DEF_CHEVRON_OFFSET_Y_COMMON );
			chevronButtonBounds.Width = DEF_ARROWS_WIDTH;
			chevronButtonBounds.Height = DEF_ARROWS_HEIGHT;

			int arrowsLeft = chevronButtonBounds.Left;

			using( isRightToLeft ? new GraphicsAutoMirrorX( g, 
				chevronButtonBounds.Right + chevronButtonBounds.Width + DEF_CHEVRON_OFFSET_X_COMMON_RIGHT ) : null )
			{
				for( int i = 0; i < DEF_ARROWS_COUNT; i++ )
				{
					g.DrawLine( SystemPens.ControlText, new Point( arrowsLeft, chevronButtonBounds.Top ),
						new Point( arrowsLeft + 1, chevronButtonBounds.Top ) );

					g.DrawLine( SystemPens.ControlText, new Point( arrowsLeft + 1, chevronButtonBounds.Top + 1 ),
						new Point( arrowsLeft + 2, chevronButtonBounds.Top + 1 ) );

					g.DrawLine( SystemPens.ControlText, new Point( arrowsLeft + 2, chevronButtonBounds.Top + 2 ),
						new Point( arrowsLeft + 3, chevronButtonBounds.Top + 2 ) );

					g.DrawLine( SystemPens.ControlText, new Point( arrowsLeft + 1, chevronButtonBounds.Top + 3 ),
						new Point(arrowsLeft + 2, chevronButtonBounds.Top + 3 ) );

					g.DrawLine( SystemPens.ControlText, new Point( arrowsLeft, chevronButtonBounds.Top + 4 ),
						new Point( arrowsLeft + 1, chevronButtonBounds.Top + 4 ) );

					arrowsLeft = chevronButtonBounds.Left + DEF_ARROWS_SPACE_X;
				}
			}
		}
		#endregion
	}
}
