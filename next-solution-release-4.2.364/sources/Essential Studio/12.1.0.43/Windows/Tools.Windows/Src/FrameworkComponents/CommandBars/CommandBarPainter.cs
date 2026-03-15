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
using System.Drawing.Drawing2D;
using System.Windows.Forms;
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Represents helper methods for drawing primitives of the CommandBar and ControlBar 
	/// with like Office2003 and OfficeXP visual style.
	/// </summary>
	public sealed class CommandBarPainter
	{
		#region Class Constants
		/// <summary>
		/// Indent for gripper.
		/// </summary>
		private const int c_iGripperXOffset = 2;
		/// <summary>
		/// Space before for gripper.
		/// </summary>
		private const int c_iGripperYOffset = 4;
		/// <summary>
		/// Size for point of the gripper.
		/// </summary>
		private const int c_iGripperPointSize = 2;
		/// <summary>
		/// Offset for rectangle.
		/// </summary>
		private const int c_iRectOffset = 1;
		/// <summary>
		/// Radius for rounded corner of the DropeDown button.
		/// </summary>
		private const int c_iDropDownRadius = 2;
		/// <summary>
		/// Distance between chevron's arrows.
		/// </summary>
		private const int c_iChevronDistance = 4;
		/// <summary>
		/// Number of the chevron arrows.
		/// </summary>
		private const int c_iChevronArrowNumber = 2;
        #endregion

		#region Class Initialize/Finalize Methods
		static CommandBarPainter()
		{
		}
		#endregion

		#region Class Static Public Methods
		/// <summary>
		/// Draws text of the CommandBar for like Office2003 visual style.
		/// </summary>
		public static void PaintDockedText( Graphics g, Rectangle rect, string text, 
			Font textFont, Color foreColor, bool bRTL, bool bVertical )
		{
			StringFormat textFormat = new StringFormat();
			textFormat.Alignment = StringAlignment.Near;
			textFormat.LineAlignment = StringAlignment.Center;
			textFormat.Trimming = StringTrimming.EllipsisCharacter;
			textFormat.FormatFlags = StringFormatFlags.NoWrap;

			if( bRTL )
			{
				textFormat.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
			}

			if( bVertical )
			{
				textFormat.FormatFlags |= StringFormatFlags.DirectionVertical;
			}

			using( Brush textBrush = new SolidBrush( foreColor ) )
			{
				g.DrawString( text, textFont, textBrush, rect, textFormat );
			}
            textFormat.Dispose();
		}
		/// <summary>
		/// Draws text of the floating CommandBar for like Office2003 visual style.
		/// </summary>
		public static void PaintFloatingText( Graphics g, Rectangle rect, string text, 
			Font textFont, Color foreColor, bool bRTL )
		{
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Near;
			stringFormat.LineAlignment = StringAlignment.Center;
			stringFormat.Trimming = StringTrimming.EllipsisCharacter;
			stringFormat.FormatFlags = StringFormatFlags.NoWrap;

			if( bRTL )
			{
				stringFormat.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
			}

			using( Brush textBrush = new SolidBrush( foreColor ) )
			{
				g.DrawString( text, textFont, textBrush, rect, stringFormat );
			}
            stringFormat.Dispose();
		}
		/// <summary>
		/// Draws text of the ControlBar for like Office2003 visual style.
		/// </summary>
		public static void PaintControlBarText( Graphics g, Rectangle rect, string text, 
			Font textFont, Color foreColor, bool bRTL )
		{
            using (StringFormat stringFormat = (StringFormat)StringFormat.GenericDefault.Clone())
            {
                stringFormat.Alignment = StringAlignment.Near;
                stringFormat.LineAlignment = StringAlignment.Center;
                stringFormat.Trimming = StringTrimming.EllipsisCharacter;
                stringFormat.FormatFlags = StringFormatFlags.NoWrap
                    | StringFormatFlags.MeasureTrailingSpaces;

                if (bRTL)
                {
                    stringFormat.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
                }

                using (Brush textBrush = new SolidBrush(foreColor))
                {
                    g.DrawString(text, textFont, textBrush, rect, stringFormat);
                }
            }
		}
		/// <summary>
		/// Draws DropDown button for like Office2003 visual style.
		/// </summary>
		public static void PaintDropDownButton( Graphics g, Rectangle rect, bool bRTL, bool bVertical,
			bool bShowChevron, Color lightColor, Color darkColor, Color chevronColor, bool bShowArrow )
		{
			if( bVertical )
			{
				// CommandBar is vertical docked
				PaintDropDownVertical( g, rect, bRTL, lightColor, darkColor );
				if( bShowArrow )
				{
					PaintDropDownVerticalArrow( g, rect, bRTL );
				}
				
				if( bShowChevron )
				{
					PaintChevronVertical( g, rect, bRTL, chevronColor );
				}
			}
			else
			{
				// CommandBar is horizontal docked
				PaintDropDownHorizontal( g, rect, bRTL, lightColor, darkColor );
				if( bShowArrow )
				{
					PaintDropDownHorizontalArrow( g, rect, bRTL );
				}

				if( bShowChevron )
				{
					PaintChevronHorizontal( g, rect, bRTL, chevronColor );
				}
			}
		}
        /// <summary>
        /// Draws gripper for like Metro visual style.
        /// </summary>
        public static void PaintMetroGripper(Graphics g, Rectangle rect, bool bRTL, bool bVertical,
            Color lightColor, Color darkColor)
        {
            SolidBrush darkBrush = new SolidBrush(darkColor);
            SolidBrush lightBrush = new SolidBrush(lightColor);
            int numberPoints = (bVertical) ? GetsNumberGripperPoints(rect.Width) :
                GetsNumberGripperPoints(rect.Height);
            int gripperXGap = 2;
            int gripperYGap = 1;
            if (bVertical)
            {
                // draw gripper for vertical docked CommandBar
                Rectangle lightRect = bRTL ?
                    new Rectangle(c_iGripperYOffset, rect.Bottom - 2 * c_iGripperXOffset, c_iGripperPointSize, c_iGripperPointSize) :
                    new Rectangle(c_iGripperYOffset, c_iGripperXOffset, c_iGripperPointSize, c_iGripperPointSize);

                Rectangle darkRect = lightRect;
                darkRect.Offset(c_iRectOffset, c_iRectOffset);

                for (int i = 0; i < numberPoints; ++i)
                {
                    using (SolidBrush brush = new SolidBrush(ColorTranslator.FromHtml("#999999")))
                    {
                        g.DrawString(".", SystemFonts.DefaultFont, brush, new Point(darkRect.X , darkRect.Y - 5));
                        if (i < numberPoints - 1)
                            g.DrawString(".", SystemFonts.DefaultFont, brush, new Point(darkRect.X + 2, darkRect.Y - 7));
                        g.DrawString(".", SystemFonts.DefaultFont, brush, new Point(darkRect.X , darkRect.Y - 9));
                    }
                    lightRect.X += c_iGripperYOffset;
                    darkRect.X += c_iGripperYOffset;
                }
            }
            else
            {
                // draw gripper for horizontal docked CommandBar
                Rectangle lightRect = bRTL ?
                    new Rectangle(rect.Right - 2 * c_iGripperXOffset, c_iGripperYOffset, c_iGripperPointSize, c_iGripperPointSize) :
                    new Rectangle(c_iGripperXOffset, c_iGripperYOffset, c_iGripperPointSize, c_iGripperPointSize);

                Rectangle darkRect = lightRect;
                darkRect.Offset(c_iRectOffset, c_iRectOffset);
                lightRect.Y -= 8;
                for (int i = 0; i < numberPoints ; ++i)
                {
                    using (SolidBrush brush = new SolidBrush(ColorTranslator.FromHtml("#999999")))
                    {
                        g.DrawString(".", SystemFonts.DefaultFont, brush, new Point(lightRect.X - gripperYGap, lightRect.Y));
                        if (i < numberPoints - 1)
                            g.DrawString(".", SystemFonts.DefaultFont, brush, new Point(lightRect.X + gripperYGap, lightRect.Y + gripperXGap));
                        g.DrawString(".", SystemFonts.DefaultFont, brush, new Point(lightRect.X + 3, lightRect.Y));
                    }
                    lightRect.Y += c_iGripperYOffset;
                    darkRect.Y += c_iGripperYOffset;
                }
            }

            darkBrush.Dispose();
            lightBrush.Dispose();
        }
		/// <summary>
		/// Draws gripper for like Office2003 visual style.
		/// </summary>
		public static void PaintGripper( Graphics g, Rectangle rect, bool bRTL, bool bVertical,
			Color lightColor, Color darkColor )
		{
			SolidBrush darkBrush = new SolidBrush( darkColor );
			SolidBrush lightBrush = new SolidBrush( lightColor );
            int numberPoints = ( bVertical ) ? GetsNumberGripperPoints( rect.Width ) :
                GetsNumberGripperPoints( rect.Height );
			
			if( bVertical )
			{
				// draw gripper for vertical docked CommandBar
				Rectangle lightRect = bRTL ?
					new Rectangle( c_iGripperYOffset, rect.Bottom - 2 * c_iGripperXOffset, c_iGripperPointSize, c_iGripperPointSize ) :
					new Rectangle( c_iGripperYOffset, c_iGripperXOffset, c_iGripperPointSize, c_iGripperPointSize );
				
				Rectangle darkRect = lightRect;
				darkRect.Offset( c_iRectOffset, c_iRectOffset );

                for( int i = 0; i < numberPoints; ++i )
				{
					g.FillRectangle( lightBrush, darkRect );
					g.FillRectangle( darkBrush, lightRect );
					lightRect.X += c_iGripperYOffset;
					darkRect.X += c_iGripperYOffset;
				}
			}
			else
			{
				// draw gripper for horizontal docked CommandBar
				Rectangle lightRect = bRTL ?
					new Rectangle( rect.Right - 2 * c_iGripperXOffset, c_iGripperYOffset, c_iGripperPointSize, c_iGripperPointSize ) :
					new Rectangle( c_iGripperXOffset, c_iGripperYOffset, c_iGripperPointSize, c_iGripperPointSize );
				
				Rectangle darkRect = lightRect;
				darkRect.Offset( c_iRectOffset, c_iRectOffset );

                for( int i = 0; i < numberPoints; ++i )
				{
					g.FillRectangle( lightBrush, darkRect );
					g.FillRectangle( darkBrush, lightRect );
					lightRect.Y += c_iGripperYOffset;
					darkRect.Y += c_iGripperYOffset;
				}
			}

			darkBrush.Dispose();
			lightBrush.Dispose();
		}
		
		/// <summary>
		/// Draws gripper for OfficeXP visual style.
		/// </summary>
		public static void PaintGripperOfficeXP( Graphics g, Rectangle rect, bool bRTL,
			DockStyle dockState, Color gripperColor  )
		{
			Pen gripperPen = new Pen( gripperColor, 1 );
			Rectangle clientRect = rect;
			int iOffset = c_iGripperXOffset;
			int iGprXOff = c_iGripperXOffset;
			int iGprYOff = c_iGripperYOffset;
			int iGprCx = c_iGripperPointSize;

			int countPoint = 
				( ( dockState == DockStyle.Left ) || ( dockState == DockStyle.Right )) ?
				( rect.Width - 10 ) / 2 : ( rect.Height - 10 ) / 2;

			if( ( dockState == DockStyle.Top ) || ( dockState == DockStyle.Bottom ) )
			{
				Point ptFrom = new Point( bRTL ? clientRect.Right - iGprXOff : iGprXOff, iGprYOff );
				Point ptTo = new Point( bRTL ? clientRect.Right - iGprXOff - iGprCx 
					: iGprXOff + iGprCx, ptFrom.Y );

				for( int i = 0; i <= countPoint; ++i )
				{
					g.DrawLine( gripperPen, ptFrom, ptTo );
					ptFrom.Y += iOffset;
					ptTo.Y = ptFrom.Y;
				}
			}
			else
			{
				Point ptFrom = new Point( iGprYOff + iOffset, bRTL ? clientRect.Bottom - iGprXOff : 
					iGprXOff );
				Point ptTo = new Point( ptFrom.X, bRTL ? clientRect.Bottom - iGprXOff - iGprCx 
					: iGprXOff + iGprCx );

				for( int i = 0; i <= countPoint; ++i )
				{
					g.DrawLine( gripperPen, ptFrom, ptTo );
					ptFrom.X += iOffset;
					ptTo.X = ptFrom.X;
				}
			}

			gripperPen.Dispose();
		}
		
		/// <summary>
		/// Draws gripper for OfficeXP visual style.
		/// </summary>
		public static void PaintControlBarGripperOfficeXP( Graphics g, Rectangle rect, int captionHeight, 
			bool bRTL, bool bFloating, Color gripperColor  )
		{
			Pen gripperPen = new Pen( gripperColor, 1 );

			Rectangle rcClient = rect;
			int nClientRight = rcClient.Right;

			int iOffset = c_iGripperXOffset;
			int iGprXOff = c_iGripperXOffset;
			int iGprYOff = c_iGripperYOffset;
			int iGprCx = c_iGripperPointSize;
			int iCount = ( captionHeight - 6 ) / 2;
			int iXOffset = iGprXOff + 1;

			if( bRTL )
			{
				iXOffset = nClientRight - iGprCx - iXOffset;
			}

			int iYOffset = bFloating ? iGprYOff : iGprYOff + 2;
			int iYInc = 0;

			for( int i = 0; i <= iCount; i++ )
			{
				g.DrawLine( gripperPen, iXOffset, iYOffset + iYInc, iXOffset + iGprCx, iYOffset + iYInc );
				iYInc += iOffset;
			}

			gripperPen.Dispose();
		}
		
		/// <summary>
		/// Draws the background region of the CommandBar like Office2003 visual style.
		/// </summary>
		public static void PaintDockedBackground( Graphics g, bool bRTL, bool bVertical,
			Rectangle clientRect, Color barColorLight, Color barColorDark, Color barBorderColor )
		{
            if (clientRect.Width == 0 || clientRect.Height == 0)
                return;
			if( bVertical )
			{
				// draw vertical docked CommandBar
				PaintDockedBackgroundVertical( g, bRTL, clientRect,
					barColorLight, barColorDark, barBorderColor );
			}
			else
			{
				// draw horizontal docked CommandBar
				CommandBarPainter.PaintDockedBackgroundHorizontal( g, bRTL, clientRect,
					barColorLight, barColorDark, barBorderColor );
			}
		}
		/// <summary>
		/// Draws the background region of the CommandBar for OfficeXP visual style.
		/// </summary>
		public static void PaintDockedBackgroundOfficeXP( Graphics g, bool bRTL, DockStyle dockState,
			Rectangle rect, Color backColor, bool bDrawTopLine )
		{
			Rectangle clientRect = rect;

			if( bRTL )
			{
				++clientRect.X;
			}
			else
			{
				--clientRect.Width;
			}

			Pen pen = new Pen( backColor );

			if( dockState == DockStyle.Top )
			{
				if( bDrawTopLine )
				{
					g.DrawLine( pen, clientRect.Left, clientRect.Top, clientRect.Right, clientRect.Top );
				}
				
				g.DrawLine( pen, clientRect.Left, clientRect.Bottom, clientRect.Right, clientRect.Bottom );
			}
			else if( dockState == DockStyle.Bottom )
			{
				g.DrawLine( pen, clientRect.Left, clientRect.Top, clientRect.Right, clientRect.Top );
				g.DrawLine( pen, clientRect.Left, clientRect.Bottom, clientRect.Right, clientRect.Bottom );
			}
			else
			{
				g.DrawLine( pen, clientRect.Left, clientRect.Top, clientRect.Left, clientRect.Bottom );
				g.DrawLine( pen, clientRect.Right, clientRect.Top, clientRect.Right, clientRect.Bottom );
			}

			pen.Dispose();
		}

		/// <summary>
		/// Draws background of the floating CommandBar like Office2003 visual style.
		/// </summary>
		public static void PaintFloatingBackground( Graphics g, Rectangle clientRect, Rectangle barRect,
			Color backColor, Color borderColor, Color startColor, Color endColor )
		{
			// fill the client area
			using( Brush brush = new SolidBrush( backColor ) )
			{
				g.FillRectangle( brush, clientRect );
			}

			// fill the client area
			using( LinearGradientBrush fillBrush = new LinearGradientBrush( barRect, startColor,
					   endColor, LinearGradientMode.Vertical) )
			{
				g.FillRectangle( fillBrush, barRect );
			}

			// draw a border around the bar using the light background color
			Point[] points = new Point[ 8 ]{
											   new Point( clientRect.Left + 1, clientRect.Top ),
											   new Point( clientRect.Right - 2, clientRect.Top ),
											   new Point( clientRect.Right - 1, clientRect.Top + 1 ),
											   new Point( clientRect.Right - 1, clientRect.Bottom - 2 ),
											   new Point( clientRect.Right - 2, clientRect.Bottom - 1 ),
											   new Point( clientRect.Left + 1, clientRect.Bottom - 1 ),
											   new Point( clientRect.Left, clientRect.Bottom - 2 ),
											   new Point( clientRect.Left, clientRect.Top + 1 )
										   };

			GraphicsPath path = new GraphicsPath();
			path.AddLines( points );

			using( path )
			{
				using( Pen borderPen = new Pen( borderColor ) )
				{
					g.DrawPath( borderPen, path );
				}
			}
		}

		/// <summary>
		/// Draws close button of the floating CommandBar like Office2003 visual style.
		/// </summary>
		public static void PaintFloatingCloseButton( Graphics g, Rectangle rect, 
			Color borderColor, Color fillColor, Color buttonColor )
		{
			// draw background
			using( SolidBrush fillBrush = new SolidBrush( fillColor ) )
			{
				g.FillRectangle( fillBrush, rect );
			}

			// draw border
			using( Pen borderPen = new Pen( borderColor) )
			{
				g.DrawRectangle( borderPen, rect );
			}

			// draw button
			using( Pen buttonPen = new Pen( buttonColor ) )
			{
				g.DrawLine( buttonPen, new Point( rect.Left + 5, rect.Top + 5 ), 
					new Point( rect.Right - 5, rect.Bottom - 5 ) );
				g.DrawLine( buttonPen, new Point( rect.Left + 6, rect.Top + 5 ), 
					new Point( rect.Right - 4, rect.Bottom - 5 ) );
				g.DrawLine( buttonPen, new Point( rect.Left + 5, rect.Bottom - 5 ), 
					new Point( rect.Right - 5, rect.Top + 5 ) );
				g.DrawLine( buttonPen, new Point( rect.Left + 6,rect.Bottom - 5 ), 
					new Point( rect.Right - 4, rect.Top + 5 ) );
			}
		}
		/// <summary>
		/// Draws DropDown button of the floating CommandBar like Office2003 visual style.
		/// </summary>
		public static void PaintFloatingDropDownButton( Graphics g, Rectangle rect, 
			Color borderColor, Color fillColor, Color buttonColor )
		{
			// draw background
			using( SolidBrush fillBrush = new SolidBrush( fillColor ) )
			{
				g.FillRectangle( fillBrush, rect );
			}

			// draw border
			using( Pen borderPen = new Pen( borderColor) )
			{
				g.DrawRectangle( borderPen, rect );
			}

			// draw button
			Point centerPoint = new Point( rect.Left + rect.Width / 2, rect.Top + rect.Height / 2 );
			Point[] points = new Point[]{ 
											new Point( centerPoint.X - 4, centerPoint.Y - 2 ),
											new Point( centerPoint.X + 5, centerPoint.Y - 2 ),
											new Point( centerPoint.X, centerPoint.Y + 3 ),
											new Point( centerPoint.X - 4, centerPoint.Y - 2 ) };
			GraphicsPath path = new GraphicsPath();
			path.AddLines( points );

			using( path )
			{
				using( Brush buttonBrush = new SolidBrush( buttonColor ) )
				{
					g.FillPath( buttonBrush, path );
				}
			}
		}
		/// <summary>
		/// Draws DropDown button for OfficeXP visual style.
		/// </summary>
		public static void PaintDropDownButtonOfficeXP( Graphics g, Rectangle rect, bool bRTL,
			bool bVertical, bool bShowChevron, Color borderColor, Color fillColor, Color chevronColor, bool bShowArrow )
		{
			// draw background
			using( Brush fillBrush = new SolidBrush( fillColor ) )
			{
				g.FillRectangle( fillBrush, rect );
			}

			// draw border
			using( Pen borderPen = new Pen( borderColor ) )
			{
				g.DrawRectangle( borderPen, rect );
			}

			// Draw dropdowns
			if( bShowArrow )
			{
				PaintDropDownArrowOfficeXP( g, rect, bRTL, bVertical );
			}

			// Draw chevron
			if( bShowChevron )
			{
				PaintChevronOfficeXP( g, rect, bRTL, bVertical, chevronColor );
			}
		}

		/// <summary>
		/// Draws arrows for DropDown button of the CommandBar with themes.
		/// </summary>
		public static void PaintDropDownArrowsThemed( Graphics g, Rectangle rect, bool bRTL, 
			bool bVertical, bool bShowChevron, Color chevronColor, bool bShowArrow )
		{
			// Draw dropdowns
			if( bShowArrow )
			{
				PaintDropDownArrowOfficeXP( g, rect, bRTL, bVertical );
			}

			// Draw chevron
			if( bShowChevron )
			{
				PaintChevronOfficeXP( g, rect, bRTL, bVertical, chevronColor );
			}
		}

		/// <summary>
		/// Draws arrow for DropDown button of the floating CommandBar with themes.
		/// </summary>
		public static void PaintFloatingDropDownArrowsThemed( Graphics g, Rectangle rect, bool bRTL )
		{
			// draw arrow
			Point centerPoint = new Point( rect.Left + rect.Width / 2, rect.Top + rect.Height / 2 );
			Point[] points = new Point[]{ 
											new Point( centerPoint.X - 4, centerPoint.Y - 2 ),
											new Point( centerPoint.X + 5, centerPoint.Y - 2 ),
											new Point( centerPoint.X, centerPoint.Y + 3 ),
											new Point( centerPoint.X - 4, centerPoint.Y - 2 ) };
			GraphicsPath path = new GraphicsPath();
			path.AddLines( points );

			using( path )
			{
				using( Brush buttonBrush = new SolidBrush( Color.White ) )
				{
					g.FillPath( buttonBrush, path );
				}
			}
		}

		/// <summary>
		/// Draws background for ControlBar.
		/// </summary>
		public static void PaintControlBarBackground( Graphics g, Rectangle rect, Rectangle captionRect,
			bool bRTL, bool bContainFocus, Color borderColor, Color captionLightColor, Color captionDarkColor,
			Color activeLightColor, Color activeDarkColor )
		{
			Rectangle excludeRect = Rectangle.Inflate( rect, -2, -2 );
			Region cliprgn = g.Clip;
			g.ExcludeClip( excludeRect );

			// Fill the outer periphery of the ControlBar
			using( SolidBrush outerBrush = new SolidBrush( borderColor ) )
			{
				g.FillRectangle( outerBrush, rect );
			}

			// Restore the clip region
			g.Clip = cliprgn;
            if (captionRect.Width > 0 && captionRect.Height > 0)
            {
                // Fill the CaptionRect with the CommandBar gradient color
                if (bContainFocus)
                {
                    using (Brush captionBrush = new LinearGradientBrush(captionRect,
                               activeLightColor, activeDarkColor, LinearGradientMode.Vertical))
                    {
                        g.FillRectangle(captionBrush, captionRect);
                    }
                }
                else
                {
                    using (LinearGradientBrush captionBrush = new LinearGradientBrush(captionRect,
                               captionLightColor, captionDarkColor, LinearGradientMode.Vertical))
                    {
                        g.FillRectangle(captionBrush, captionRect);
                    }
                }
            }
		}

		/// <summary>
		/// Draws DropDown button for ControlBar.
		/// </summary>
		public static void PaintControlBarDropDown( Graphics g, Rectangle rect, bool bRTL,
			Color borderColor, Color fillColor )
		{
			// draw background
			using( Brush fillBrush = new SolidBrush( fillColor ) )
			{
				g.FillRectangle( fillBrush, rect );
			}

			// draw border
			using( Pen borderPen = new Pen( borderColor ) )
			{
				g.DrawRectangle( borderPen, rect );
			}

			// draw button
			Point centerPoint = ( bRTL ) ?
				new Point( rect.Left + 9, rect.Top + rect.Height / 2 ) :
				new Point( rect.Left + rect.Width - 9, rect.Top + rect.Height / 2 );
				
			Point[] points = new Point[]{ 
											new Point( centerPoint.X - 4, centerPoint.Y - 2 ),
											new Point( centerPoint.X + 5, centerPoint.Y - 2 ),
											new Point( centerPoint.X, centerPoint.Y + 3 ),
											new Point( centerPoint.X - 4, centerPoint.Y - 2 ) };
			GraphicsPath path = new GraphicsPath();
			path.AddLines( points );

			using( path )
			{
				using( Brush buttonBrush = new SolidBrush( SystemColors.ControlText ) )
				{
					g.FillPath( buttonBrush, path );
				}
			}

		}

		/// <summary>
		/// Draws close button for ControlBar.
		/// </summary>
		public static void PaintControlBarCloseButton( Graphics g, Rectangle rect,
			Color borderColor, Color fillColor )
		{
			// draw background
			using( Brush fillBrush = new SolidBrush( fillColor ) )
			{
				g.FillRectangle( fillBrush, rect );
			}

			// draw border
			using( Pen borderPen = new Pen( borderColor ) )
			{
				g.DrawRectangle( borderPen, rect );
			}

			// draw button
			using( Pen buttonPen = new Pen( SystemColors.ControlText ) )
			{
				g.DrawLine( buttonPen, new Point( rect.Left + 5, rect.Top + 5 ), 
					new Point( rect.Right - 5, rect.Bottom - 5 ) );
				g.DrawLine( buttonPen, new Point( rect.Left + 6, rect.Top + 5 ), 
					new Point( rect.Right - 4, rect.Bottom - 5 ) );
				g.DrawLine( buttonPen, new Point( rect.Left + 5, rect.Bottom - 5 ), 
					new Point( rect.Right - 5, rect.Top + 5 ) );
				g.DrawLine( buttonPen, new Point( rect.Left + 6,rect.Bottom - 5 ), 
					new Point( rect.Right - 4, rect.Top + 5 ) );
			}
		}

		/// <summary>
		/// Draws DropDown button for ControlBar with themes.
		/// </summary>
		public static void PaintControlBarDropDownThemed( Graphics g, Rectangle rect, 
			Color borderColor, Color fillColor )
		{
			// draw background
			using( Brush fillBrush = new SolidBrush( fillColor ) )
			{
				g.FillRectangle( fillBrush, rect );
			}

			// draw border
			using( Pen borderPen = new Pen( borderColor ) )
			{
				g.DrawRectangle( borderPen, rect );
			}
		}

		#endregion

		#region Class Static Utility Methods
		/// <summary>
		/// Draws chevron arrow for OfficeXP visual style of the docked CommandBar.
		/// </summary>
		/// <param name="rect">Rectangle of the DropDown button.</param>
		private static void PaintChevronOfficeXP( Graphics g, Rectangle rect, 
			bool bRTL, bool bVertical, Color chevronColor )
		{
			Pen chevronPen = new Pen( chevronColor );

			if( bVertical )
			{
				int iOffsetY1 = ( bRTL ) ? -1 : 1;
				int iOffsetY2 = ( bRTL ) ? 1 : -1;
				Point pt1 = ( bRTL ) ? new Point( rect.Right - 9, rect.Y + 4 ) : 
					new Point( rect.Right - 9, rect.Y + 2 );
				Point pt2 = new Point( pt1.X, pt1.Y + 1 );
					
				g.DrawLine( chevronPen, pt1, pt2 );
				g.DrawLine( chevronPen, pt1.X, pt1.Y + c_iChevronDistance, pt2.X, pt2.Y + c_iChevronDistance );
				pt1.Offset( 1, iOffsetY1 );
				pt2.Offset( 1, iOffsetY1 );
				g.DrawLine( chevronPen, pt1, pt2 );
				g.DrawLine( chevronPen, pt1.X, pt1.Y + c_iChevronDistance, pt2.X, pt2.Y + c_iChevronDistance );
				pt1.Offset( 1, iOffsetY1 );
				pt2.Offset( 1, iOffsetY1 );
				g.DrawLine( chevronPen, pt1, pt2 );
				g.DrawLine( chevronPen, pt1.X, pt1.Y + c_iChevronDistance, pt2.X, pt2.Y + c_iChevronDistance );
				pt1.Offset( 1, iOffsetY2 );
				pt2.Offset( 1, iOffsetY2 );
				g.DrawLine( chevronPen, pt1, pt2 );
				g.DrawLine( chevronPen, pt1.X, pt1.Y + c_iChevronDistance, pt2.X, pt2.Y + c_iChevronDistance );
				pt1.Offset( 1, iOffsetY2 );
				pt2.Offset( 1, iOffsetY2 );
				g.DrawLine( chevronPen, pt1, pt2 );
				g.DrawLine( chevronPen, pt1.X, pt1.Y + c_iChevronDistance, pt2.X, pt2.Y + c_iChevronDistance );
			}
			else
			{
				int iOffsetX1 = ( bRTL ) ? -1 : 1;
				int iOffsetX2 = ( bRTL ) ? 1 : -1;
				Point pt1 = ( bRTL ) ? new Point( rect.X + 3, rect.Y + 3 ) : 
					new Point( rect.X + 1, rect.Y + 3 );
				Point pt2 = new Point( pt1.X + 1, pt1.Y );
					
				g.DrawLine( chevronPen, pt1, pt2 );
				g.DrawLine( chevronPen, pt1.X + c_iChevronDistance, pt1.Y, pt2.X + c_iChevronDistance, pt2.Y );
				pt1.Offset( iOffsetX1, 1 );
				pt2.Offset( iOffsetX1, 1 );
				g.DrawLine( chevronPen, pt1, pt2 );
				g.DrawLine( chevronPen, pt1.X + c_iChevronDistance, pt1.Y, pt2.X + c_iChevronDistance, pt2.Y );
				pt1.Offset( iOffsetX1, 1 );
				pt2.Offset( iOffsetX1, 1 );
				g.DrawLine( chevronPen, pt1, pt2 );
				g.DrawLine( chevronPen, pt1.X + c_iChevronDistance, pt1.Y, pt2.X + c_iChevronDistance, pt2.Y );
				pt1.Offset( iOffsetX2, 1 );
				pt2.Offset( iOffsetX2, 1 );
				g.DrawLine( chevronPen, pt1, pt2 );
				g.DrawLine( chevronPen, pt1.X + c_iChevronDistance, pt1.Y, pt2.X + c_iChevronDistance, pt2.Y );
				pt1.Offset( iOffsetX2, 1 );
				pt2.Offset( iOffsetX2, 1 );
				g.DrawLine( chevronPen, pt1, pt2 );
				g.DrawLine( chevronPen, pt1.X + c_iChevronDistance, pt1.Y, pt2.X + c_iChevronDistance, pt2.Y );
			}

			chevronPen.Dispose();
		}
		/// <summary>
		/// Draws DropDown button for docked CommandBar with OfficeXP visual style.
		/// </summary>
		private static void PaintDropDownArrowOfficeXP( Graphics g, Rectangle rect, bool bRTL, bool bVertical )
		{
			if( bVertical )
			{
				Point pt1 = new Point( rect.Left + 7, rect.Top + 2 );
				Point pt2 = new Point( rect.Left + 7, rect.Top + 8 );
				Point pt3 = new Point( rect.Left + 4, rect.Top + 5 );
				GraphicsPath path = new GraphicsPath();
				path.AddLines( new Point[]{ pt1, pt2, pt3 } );
				g.FillPath( SystemBrushes.ControlText, path );
                path.Dispose();
			}
			else
			{
				Point pt1 = new Point( rect.Left + 3, rect.Bottom - 7 );
				Point pt2 = new Point( rect.Left + 8, rect.Bottom - 7 );
				Point pt3 = new Point( rect.Left + 5, rect.Bottom - 4 );
				GraphicsPath path = new GraphicsPath();
				path.AddLines( new Point[]{ pt1, pt2, pt3 } );
				g.FillPath( SystemBrushes.ControlText, path );
                path.Dispose();
			}
		}

		/// <summary>
		/// Draws DropDown button for vertical docked CommandBar with like Office2003 visual style.
		/// </summary>
		private static void PaintDropDownVertical( Graphics g, Rectangle rect, bool bRTL,
			Color lightColor, Color darkColor )
		{
			using( LinearGradientBrush brush = new LinearGradientBrush( rect, lightColor, darkColor,  LinearGradientMode.Horizontal ) )
			{
				using( GraphicsPath path = GetDropDownVerticalPath( rect, bRTL ) )
				{
					g.FillPath( brush, path );
				}
			}
		}

		/// <summary>
		/// Draws DropDown button for horizontal docked CommandBar with like Office2003 visual style.
		/// </summary>
		private static void PaintDropDownHorizontal( Graphics g, Rectangle rect, bool bRTL,
			Color lightColor, Color darkColor )
		{
			using( LinearGradientBrush brush = new LinearGradientBrush( rect, lightColor, darkColor,  LinearGradientMode.Vertical ) )
			{
				using( GraphicsPath path = GetDropDownHorizontalPath( rect, bRTL ) )
				{
					g.FillPath( brush, path );
				}
			}
		}

		/// <summary>
		/// Draws the background region of the CommandBar
		/// like Office2003 visual style when in a horizontal docked state.
		/// </summary>
		private static void PaintDockedBackgroundHorizontal( Graphics g, bool bRTL, Rectangle clientRect,
			Color barColorLight, Color barColorDark, Color barBorderColor )
		{
			Region initialcliprgn = g.Clip;
			SetCommadBarExcludeClip( g, clientRect );

			// Fill the remaining area
			using( LinearGradientBrush barBrush = new LinearGradientBrush( clientRect, barColorLight,
					   barColorDark, LinearGradientMode.Vertical ) )
			{
				g.FillRectangle( barBrush, clientRect );
			}

			Pen darkPen = new Pen( barBorderColor );

            // Draw a horizontal line along the bottom edge
			Point pt1 = ( bRTL ) ? new Point( clientRect.Left - 1, clientRect.Bottom - 1 ) :
				new Point( clientRect.Left, clientRect.Bottom - 1 );
			Point pt2 = ( bRTL ) ? new Point( clientRect.Right, clientRect.Bottom - 1 ) :
				new Point( clientRect.Right - 1, clientRect.Bottom - 1 );
            if (!CommandBarRendererMetro.IsMetroSet)
            {
                using (darkPen)
                {
                    g.DrawLine(darkPen, pt1, pt2);
                }
            }

			g.Clip = initialcliprgn;
		}

		/// <summary>
		/// Draws the background region of the CommandBar
		/// like Office2003 visual style when in a vertical docked state.
		/// </summary>
		private static void PaintDockedBackgroundVertical( Graphics g, bool bRTL, Rectangle clientRect,
			Color barColorLight, Color barColorDark, Color barBorderColor )
		{
			Region initialcliprgn = g.Clip;
			SetCommadBarExcludeClip( g, clientRect );

            if (clientRect.Width > 0 && clientRect.Height > 0 )
            {
                // Fill the remaining area
                using (LinearGradientBrush barBrush = bRTL ?
                           new LinearGradientBrush(clientRect, barColorDark, barColorLight, LinearGradientMode.Horizontal) :
                           new LinearGradientBrush(clientRect, barColorLight, barColorDark, LinearGradientMode.Horizontal))
                {
                    g.FillRectangle(barBrush, clientRect);
                }
            }
			// Draw a vertical line along the right edge
			int nXPos = clientRect.Right - 1;
            Pen borderPen = new Pen(barBorderColor);
            
            borderPen.Dispose();
			using( Pen darkPen = new Pen( barBorderColor ) )
			{
				g.DrawLine( darkPen, nXPos, clientRect.Top, nXPos, clientRect.Bottom );
			}

			g.Clip = initialcliprgn;
		}

		/// <summary>
		/// Draws chevron arrow for DropDown button of the horizontal docked CommandBar.
		/// </summary>
		/// <param name="rect">Rectangle of the DropDown button.</param>
		private static void PaintChevronHorizontal( Graphics g, Rectangle rect, bool bRTL,
			Color chevronColor )
		{
			Pen darkPen = new Pen( chevronColor );
			Pen lightPen = Pens.White;

			int iYOffset = -1;
			int iXOffset = -1;
			int iOffcet = c_iChevronDistance;

			Point pt1, pt2, pt3, pt4 = Point.Empty;
			pt1 = ( bRTL ) ? new Point( rect.X + rect.Width - 6, rect.Y + 6 ) : 
				new Point( rect.X + 5, rect.Y + 6 );
				
			for( int i = 0; i < c_iChevronArrowNumber; i++ )
			{
				pt2 = new Point( pt1.X, pt1.Y + 2 );
				pt3 = new Point( pt1.X, pt1.Y + 1 );
				pt4 = ( bRTL ) ? new Point( pt1.X - 1, pt1.Y + 1 ) : 
					new Point( pt1.X + 1, pt1.Y + 1 );

				// draw light arrow
				g.DrawLine( lightPen, pt1, pt2 );
				g.DrawLine( lightPen, pt3, pt4 );

				iXOffset = ( bRTL ) ? 1 : -1;
				pt1.Offset( iXOffset, iYOffset );
				pt2.Offset( iXOffset, iYOffset );
				pt3.Offset( iXOffset, iYOffset );
				pt4.Offset( iXOffset, iYOffset );

				// draw dark arrow
				g.DrawLine( darkPen, pt1, pt2 );
				g.DrawLine( darkPen, pt3, pt4 );
					
				iOffcet = ( bRTL ) ? ( - c_iChevronDistance - iXOffset ) : 
					( c_iChevronDistance - iXOffset );
				pt1.Offset( iOffcet, -iYOffset );
			}

			darkPen.Dispose();
		}
		
		/// <summary>
		/// Draws chevron arrow for DropDown button of the vertical docked CommandBar.
		/// </summary>
		/// <param name="rect">Rectangle of the DropDown button.</param>
		private static void PaintChevronVertical( Graphics g, Rectangle rect, bool bRTL,
			Color chevronColor )
		{
			Pen darkPen = new Pen( chevronColor );
			Pen lightPen = Pens.White;
			int iOffset = 1;

			Point pt1, pt2, pt3, pt4 = Point.Empty;
			pt1 = ( bRTL ) ? new Point( rect.X + rect.Width - 7, rect.Y + 3 ) : 
				new Point( rect.X + 6, rect.Y + 6 );
				
			for( int i = 0; i < c_iChevronArrowNumber; i++ )
			{
				pt2 = new Point( pt1.X + 2, pt1.Y );
				pt3 = new Point( pt1.X + 1, pt1.Y );
				pt4 = new Point( pt1.X + 1, pt1.Y + 1 );

				// draw light arrow
				g.DrawLine( lightPen, pt1, pt2 );
				g.DrawLine( lightPen, pt3, pt4 );

				pt1.Offset( -iOffset, -iOffset );
				pt2.Offset( -iOffset, -iOffset );
				pt3.Offset( -iOffset, -iOffset );
				pt4.Offset( -iOffset, -iOffset );

				// draw dark arrow
				g.DrawLine( darkPen, pt1, pt2 );
				g.DrawLine( darkPen, pt3, pt4 );

				pt1.Offset( iOffset, c_iChevronDistance + iOffset );
			}

			darkPen.Dispose();
		}
		/// <summary>
		/// Draws DropDown arrow for DropDown button of the horizontal docked CommandBar.
		/// </summary>
		/// <param name="rect">Rectangle of the DropDown button.</param>
		private static void PaintDropDownHorizontalArrow( Graphics g, Rectangle rect, bool bRTL )
		{
			rect.X += ( bRTL ) ? 0 : c_iGripperXOffset;

			// Draw the dark and light shaded lines above the dropdown
			Point startPoint = new Point( rect.Left + 2, rect.Bottom - 10 );
			Point endPoint = new Point( startPoint.X + 4, startPoint.Y );
			g.DrawLine( SystemPens.ControlText, startPoint, endPoint );
			startPoint.Offset( 1, 1 );
			endPoint.Offset( 1, 1 );
			g.DrawLine( Pens.White, startPoint, endPoint );

			// Draw the light dropdowns
			Point pt1 = new Point( rect.Left + 3, rect.Bottom - 6 );
			Point pt2 = new Point( rect.Left + 8, rect.Bottom - 6 );
			Point pt3 = new Point( rect.Left + 5, rect.Bottom - 3 );
			GraphicsPath path = new GraphicsPath();
			path.AddLines( new Point[]{ pt1, pt2, pt3 } );
			g.FillPath( Brushes.White, path );
			path.Reset();

			// Draw the dark dropdowns
			pt1.Offset( -1, -1 );
			pt2.Offset( -1, -1 );
			pt3.Offset( -1, -1 );
			path.AddLines( new Point[]{ pt1, pt2, pt3 } );
			g.FillPath( SystemBrushes.ControlText, path );
            path.Dispose();
		}

		/// <summary>
		/// Paint DropDown arrow for DropDown button of the vertical docked CommandBar.
		/// </summary>
		/// <param name="rect">Rectangle of the DropDown button.</param>
		private static void PaintDropDownVerticalArrow( Graphics g, Rectangle rect, bool bRTL )
		{
			if( bRTL )
			{
				// Draw the dark and light shaded lines above the dropdown
				Point startPoint = new Point( rect.Left + 10, rect.Top + 3 );
				Point endPoint = new Point( startPoint.X, startPoint.Y + 4 );
				g.DrawLine( SystemPens.ControlText, startPoint, endPoint );
				startPoint.Offset( -1, 1 );
				endPoint.Offset( -1, 1 );
				g.DrawLine( Pens.White, startPoint, endPoint );

				// Draw the light dropdowns
				Point pt1 = new Point( rect.Left + 6, rect.Top + 3 );
				Point pt2 = new Point( rect.Left + 6, rect.Top + 9 );
				Point pt3 = new Point( rect.Left + 3, rect.Top + 6 );
				GraphicsPath path = new GraphicsPath();
				path.AddLines( new Point[]{ pt1, pt2, pt3 } );
				g.FillPath( Brushes.White, path );
				path.Reset();

				// Draw the dark dropdowns
				pt1.Offset( 1, -1 );
				pt2.Offset( 1, -1 );
				pt3.Offset( 1, -1 );
				path.AddLines( new Point[]{ pt1, pt2, pt3 } );
				g.FillPath( SystemBrushes.ControlText, path );
                path.Dispose();
			}
			else
			{
				rect.Y -= c_iGripperXOffset;

				// Draw the dark and light shaded lines above the dropdown
				Point startPoint = new Point( rect.Right - 10, rect.Bottom - 2 );
				Point endPoint = new Point( startPoint.X, startPoint.Y - 4 );
				g.DrawLine( SystemPens.ControlText, startPoint, endPoint );
				startPoint.Offset( 1, 1 );
				endPoint.Offset( 1, 1 );
				g.DrawLine( Pens.White, startPoint, endPoint );
				
				// Draw the light dropdowns
				Point pt1 = new Point( rect.Right - 6, rect.Bottom );
				Point pt2 = new Point( rect.Right - 6, rect.Bottom - 6 );
				Point pt3 = new Point( rect.Right - 3, rect.Bottom - 3 );
				GraphicsPath path = new GraphicsPath();
				path.AddLines( new Point[]{ pt1, pt2, pt3 } );
				g.FillPath( Brushes.White, path );
				path.Reset();

				// Draw the dark dropdowns
				pt1.Offset( -1, -1 );
				pt2.Offset( -1, -1 );
				pt3.Offset( -1, -1 );
				path.AddLines( new Point[]{ pt1, pt2, pt3 } );
				g.FillPath( SystemBrushes.ControlText, path );
                path.Dispose();
			}
		}

		/// <summary>
		/// Gets path for DropDown button of the horizontal docked CommandBar.
		/// </summary>
		private static GraphicsPath GetDropDownHorizontalPath( Rectangle rect, bool bRTL )
		{
			int iLeft = rect.Left;
			int iRight = rect.Right;
			int iTop = rect.Top;
			int iBottm = rect.Bottom;
			Point[] points = null;

			if( bRTL )
			{
				points = new Point[] {
										 new Point( iLeft, iTop + c_iDropDownRadius ),
										 new Point( iLeft + c_iDropDownRadius, iTop ),
										 new Point( iRight, iTop ),
										 new Point( iRight - c_iDropDownRadius, iTop + c_iDropDownRadius ),
										 new Point( iRight - c_iDropDownRadius, iBottm - c_iDropDownRadius - 1 ),
										 new Point( iRight, iBottm ),
										 new Point( iLeft + c_iDropDownRadius + 1, iBottm ),
										 new Point( iLeft, iBottm - c_iDropDownRadius - 1 )
									 };
			}
			else
			{
				points = new Point[] {
										 new Point( iLeft, iTop ),
										 new Point( iRight - c_iDropDownRadius, iTop ),
										 new Point( iRight, iTop + c_iDropDownRadius ),
										 new Point( iRight, iBottm - c_iDropDownRadius - 1 ),
										 new Point( iRight - c_iDropDownRadius - 1, iBottm ),
										 new Point( iLeft - 1, iBottm ),
										 new Point( iLeft + c_iDropDownRadius, iBottm - c_iDropDownRadius - 1 ),
										 new Point( iLeft + c_iDropDownRadius, iTop + c_iDropDownRadius )
									 };
			}

			GraphicsPath path = new GraphicsPath();
			path.AddLines( points );

			return path;
		}

		/// <summary>
		/// Gets path for DropDown button of the vertical docked CommandBar.
		/// </summary>
		private static GraphicsPath GetDropDownVerticalPath( Rectangle rect, bool bRTL )
		{
			int iLeft = rect.Left;
			int iRight = rect.Right;
			int iTop = rect.Top;
			int iBottm = rect.Bottom;
			Point[] points = null;

			if( bRTL )
			{
				points = new Point[] {
										 new Point( iLeft, iTop + c_iDropDownRadius ),
										 new Point( iLeft + c_iDropDownRadius, iTop ),
										 new Point( iRight - c_iDropDownRadius, iTop ),
										 new Point( iRight, iTop + c_iDropDownRadius ),
										 new Point( iRight, iBottm ),
										 new Point( iRight - c_iDropDownRadius, iBottm - c_iDropDownRadius ),
										 new Point( iLeft + c_iDropDownRadius, iBottm - c_iDropDownRadius ),
										 new Point( iLeft, iBottm )
									 };
			}
			else
			{
				points = new Point[] {
										 new Point( iLeft, iTop ),
										 new Point( iLeft + c_iDropDownRadius + 1, iTop + c_iDropDownRadius + 1 ),
										 new Point( iRight - c_iDropDownRadius - 1, iTop + c_iDropDownRadius + 1 ),
										 new Point( iRight, iTop ),
										 new Point( iRight, iBottm - c_iDropDownRadius - 1 ),
										 new Point( iRight - c_iDropDownRadius - 1, iBottm ),
										 new Point( iLeft + c_iDropDownRadius + 1, iBottm ),
										 new Point( iLeft, iBottm - c_iDropDownRadius - 1 )
									 };
			}

			GraphicsPath path = new GraphicsPath();
			path.AddLines( points );

			return path;
		}

		/// <summary>
		/// Sets exclude clip for CommandBar background.
		/// </summary>
		private static void SetCommadBarExcludeClip( Graphics gph, Rectangle rect )
		{
			Rectangle rcclient = rect;
			Point[] pts = new Point[] {	  new Point( rcclient.Left, rcclient.Top ),
										  new Point( rcclient.Left + 2, rcclient.Top ),
										  new Point( rcclient.Left, rcclient.Top + 2 ),
										  new Point( rcclient.Left, rcclient.Top ) };
			
			GraphicsPath path = new GraphicsPath();
			path.AddLines( pts );
            using(Region region=new Region( path ))
                gph.ExcludeClip(region);
			path.Reset();

			pts[0] = new Point( rcclient.Right, rcclient.Top );
			pts[1] = new Point( rcclient.Right, rcclient.Top + 2 );
			pts[2] = new Point( rcclient.Right - 2, rcclient.Top );
			pts[3] = new Point( rcclient.Right, rcclient.Top );
			path.AddLines( pts );
            using (Region region = new Region(path))
                gph.ExcludeClip(region);
			path.Reset();

			pts[0] = new Point( rcclient.Right, rcclient.Bottom );
			pts[1] = new Point( rcclient.Right - 3, rcclient.Bottom );
			pts[2] = new Point( rcclient.Right, rcclient.Bottom - 3 );
			pts[3] = new Point( rcclient.Right, rcclient.Bottom );
			path.AddLines( pts );
            using (Region region = new Region(path))
                gph.ExcludeClip(region);
			path.Reset();

			pts[0] = new Point( rcclient.Left, rcclient.Bottom );
			pts[1] = new Point( rcclient.Left, rcclient.Bottom - 3 );
			pts[2] = new Point( rcclient.Left + 2, rcclient.Bottom );
			pts[3] = new Point( rcclient.Left, rcclient.Bottom );
			path.AddLines( pts );
            using (Region region = new Region(path))
                gph.ExcludeClip(region);
            path.Dispose();
		}

        /// <summary>
        /// Gets number gripper points.
        /// </summary>
        private static int GetsNumberGripperPoints( int gripperSize )
        {
            int numberPoints = 0;

            if( gripperSize > 0 )
            {
                decimal size = gripperSize;
                size = ( size - 10 ) / 4;

				#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                numberPoints = decimal.ToInt32( decimal.Round( size ) );
				#else
				numberPoints = decimal.ToInt32( size );
				#endif
            }

            return numberPoints;
        }
		#endregion
	}
}
