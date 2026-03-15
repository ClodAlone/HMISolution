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
using System.Drawing.Drawing2D;

using Syncfusion.Windows.Forms;


namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// Draws a themed scroll button.
	/// </summary>
	public class ThemedScrollButton: ThemedButtonBase
	{
		private Syncfusion.Windows.Forms.ThemedScrollButtonDrawing scrollDrawing = null;
		private ButtonID buttonID = ButtonID.Up;
		private ScrollButtonAppearance appearance = ScrollButtonAppearance.Horizontal;
		private ScrollButton sButton = ScrollButton.Up;
		private bool transparent = false;
		private Color m_arrowColor = Color.Black;

		public bool Transparent
		{
			get
			{
				return transparent;
			}
			set
			{
				if( transparent!=value )
				{
					transparent = value;
					Invalidate();
				}
			}
		}

		/// <summary>
		/// Gets / sets the type of scroll arrow.
		/// </summary>
		public System.Windows.Forms.ScrollButton ButtonType
		{
			get { return sButton; }
			set
			{
				if( sButton!= value )
				{
					sButton = value;
					switch( sButton )
					{
						case ScrollButton.Down: { appearance = ScrollButtonAppearance.Vertical; buttonID = ButtonID.Down; break; }
						case ScrollButton.Left: { appearance = ScrollButtonAppearance.Horizontal; buttonID = ButtonID.Up; break; }
						case ScrollButton.Right: { appearance = ScrollButtonAppearance.Horizontal; buttonID = ButtonID.Down; break; }
						case ScrollButton.Up: { appearance = ScrollButtonAppearance.Vertical; buttonID = ButtonID.Up; break; }
					}
					Invalidate();
				}
			}
		}

		/// <summary>
		/// Initializes a new object.
		/// </summary>
		public ThemedScrollButton()
		{
			if( XPThemes.IsThemedOS )
			{
				scrollDrawing = new ThemedScrollButtonDrawing( this );
			}
		}

		/// <summary>
		/// Disposes all resources being used.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( this.scrollDrawing!=null )
				{
					scrollDrawing.Dispose();
					scrollDrawing = null;
				}
			}
		}

		/// <override/>
		protected override void DrawThemedControl( Graphics g, ButtonState buttonState, CheckState checkState )
		{
			if( scrollDrawing == null ) return;
			scrollDrawing.DrawScrollButton( g, ClientRectangle, buttonID, appearance, buttonState );
		}

		/// <override/>
		protected override void DrawNotThemedControl( Graphics g, ButtonState buttonState, CheckState checkState )
		{
			ControlPaint.DrawScrollButton( g, ClientRectangle, sButton, buttonState );
		}

		/// <summary>
		/// Returns the color for paint control background.
		/// </summary>
		/// <param name="buttonState"></param>
		/// <returns></returns>
		protected virtual Color GetBackColor( ButtonState buttonState )
		{
			Color btnColor = Color.Empty;

			if( buttonState == ButtonState.Pushed )
			{
				if( this.Style == VisualStyle.OfficeXP )
					btnColor = MenuColors.PressedSelColor;
				else
					btnColor = Office2003Colors.PressedSelColor;
			}
			else if( this.mouseOver )
			{
				if( this.Style == VisualStyle.OfficeXP )
					btnColor = MenuColors.SelColor;
				else
					btnColor = Office2003Colors.SelColor;
			}
			else
			{
				try
				{
					btnColor = Parent.Parent.BackColor;
				}
				catch { btnColor = SystemColors.Control; }
			}
			return btnColor;
		}

		protected override void DrawStyledControl( Graphics g, ButtonState buttonState, CheckState checkState )
		{
			if( this.Bounds.Width <= 0 || this.Bounds.Height <= 0 )
				return;

			if( !transparent )
			{
                //if (this.Style == VisualStyle.Metro)
                //    g.DrawRectangle(new Pen(this.GetBackGroundBrush( buttonState )), this.ClientRectangle);
                if (this.Style == VisualStyle.Office2007 || this.Style == VisualStyle.Office2010)
                {
					Rectangle fillRect = this.ClientRectangle;

					if( this.Dock == DockStyle.Right )
						fillRect.Offset( 2, 0 );
					else if( this.Dock == DockStyle.Left )
						fillRect.Offset( 1, 0 );

					fillRect.Width -= 3;
					fillRect.Height--;

					g.FillRectangle( this.GetBackGroundBrush( buttonState ), fillRect );
				}
                else
                    g.FillRectangle(this.GetBackGroundBrush(buttonState), this.ClientRectangle);
			}
			// The arrow:
			Point[] dropDownArrowBounds = this.GetComboDropDownBorderBounds( this.ClientRectangle );

			GraphicsPath path = new GraphicsPath();
			path.AddLines( dropDownArrowBounds );

			Color arrowColor = Color.Empty;

			if( this.Style != VisualStyle.Office2007 )
			{
				arrowColor = SystemColors.ControlText;

                if (buttonState == ButtonState.Pushed)
                    arrowColor = SystemColors.ControlText;
                if (this.Style != VisualStyle.Metro)
				    g.DrawRectangle( this.GetBorderPen( buttonState ), new Rectangle( 0, 0, Width - 1, Height - 1 ) );
			}
			else
			{
				arrowColor = this.ArrowColor;

				if( this.Dock == DockStyle.Right )
					g.DrawLine( this.GetBorderPen( buttonState ), new Point( 0, 0 ), new Point( 0, this.Bottom ) );
				else if( this.Dock == DockStyle.Left )
					g.DrawLine( this.GetBorderPen( buttonState ), new Point( this.Right - 1, 0 ), new Point( this.Right - 1, this.Bottom ) );
			}

			if( !this.Enabled )
				arrowColor = SystemColors.GrayText;
            using (Region region = new Region(path))
            {
                if (buttonState == ButtonState.Pushed)
                {
                    using (Brush brush = new SolidBrush(ControlPaint.Dark(m_arrowColor)))
                        g.FillRegion(brush, region);
                }
                else
                {
                    using (Brush brush = new SolidBrush(m_arrowColor))
                        g.FillRegion(brush, region);
                }
            }
            path.Dispose();
		}

		protected virtual Pen GetBorderPen( ButtonState buttonState )
		{
			return new Pen( this.FlatColor );
		}

		protected virtual Brush GetBackGroundBrush( ButtonState buttonState )
		{
			return new SolidBrush( GetBackColor( buttonState ) );
		}

		public virtual Color ArrowColor
		{
			get { return m_arrowColor; }
			set
			{
				if( m_arrowColor != value )
					m_arrowColor = value;
			}
		}

		private float GetComboDropDownArrowWidth()
		{
			//float ddwidth = 7f * 
			//	(SystemInformation.MenuCheckSize.Width / 13f/*13 is the standard size of the checkboxes.*/);
			// Need to determine the exact transformation logic for higher font sizes.
			float ddwidth = 7f;

			// ddwidth cannot be an even no.
			int iddwidth = (int)ddwidth;
			if( ( iddwidth % 2 ) == 0 )
				ddwidth++;

			return ddwidth;
		}
		private float GetComboDropDownArrowHeight()
		{
			return ( (int)this.GetComboDropDownArrowWidth() )/2 + 1;
		}
		private Point[] GetComboDropDownBorderBounds( Rectangle btnBounds )
		{
			int arWidth = (int)this.GetComboDropDownArrowWidth();
			int arHeight = (int)this.GetComboDropDownArrowHeight();

			return this.GetComboDropDownBorderBounds( btnBounds, arWidth, arHeight );
		}
		private Point[] GetComboDropDownBorderBounds( Rectangle btnBounds,
			int arWidth, int arHeight )
		{
			int left = 0;
			int top = 0;

			if( sButton == ScrollButton.Up || sButton == ScrollButton.Down )
			{
				left = btnBounds.Left + ( btnBounds.Width-arWidth )/2;
				top = btnBounds.Top + ( btnBounds.Height - arHeight )/2;

				if( this.Style == VisualStyle.Office2007 )
					left += 1;
			}
			else
			{
				left = btnBounds.Left + ( btnBounds.Width - arHeight )/2;
				top = btnBounds.Top + ( btnBounds.Height - arWidth )/2;
			}

			Rectangle rcddbtn = Rectangle.Empty;
			if( sButton == ScrollButton.Up || sButton == ScrollButton.Down )
				rcddbtn = new Rectangle( left, top, arWidth, arHeight );
			else
				rcddbtn = new Rectangle( left, top, arHeight, arWidth );

			Point[] ptsscrll = new Point[0];

			switch( sButton )
			{
				case ScrollButton.Down:
				{
					ptsscrll = new Point[] { 
																   new Point(rcddbtn.Left, rcddbtn.Top),
																   new Point(rcddbtn.Right, rcddbtn.Top),
																   new Point(left + arWidth/2, rcddbtn.Bottom),
																   new Point(rcddbtn.Left, rcddbtn.Top) }; break;
				}
				case ScrollButton.Left:
				{
					ptsscrll = new Point[] { 
											   new Point(rcddbtn.Right, rcddbtn.Top),
											   new Point(rcddbtn.Right, rcddbtn.Bottom),
											   new Point(left , top + rcddbtn.Height/2),
											   new Point(rcddbtn.Right, rcddbtn.Top-1)};
					break;
				}
				case ScrollButton.Right:
				{
					ptsscrll = new Point[] { 
											   new Point(rcddbtn.Left, rcddbtn.Top),
											   new Point(rcddbtn.Left, rcddbtn.Bottom),
											   new Point(rcddbtn.Right, top + rcddbtn.Height/2),
											   new Point(rcddbtn.Left, rcddbtn.Top-1)};

					break;
				}
				case ScrollButton.Up:
				{
					ptsscrll = new Point[] { 
																  new Point(rcddbtn.Left, rcddbtn.Bottom),
																  new Point(rcddbtn.Right, rcddbtn.Bottom),
																  new Point(left + arWidth/2, rcddbtn.Top-1),
																  new Point(rcddbtn.Left-1, rcddbtn.Bottom)};
					break;
				}
			}


			return ptsscrll;
		}
	}
}
