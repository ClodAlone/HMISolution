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
using System.Drawing;
using System.Drawing.Drawing2D;

using Syncfusion.ComponentModel;
using Syncfusion.Windows.Forms.Tools;
using System;
#endregion

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// WindowsXP button Renderer.
	/// </summary>
	internal class WindowsXPButtonRenderer : ButtonRenderer
    {
        #region Class constants
        private const int c_INCREASE_HEIGHT_VALUE = 1;
        #endregion

        #region Class members
        /// <summary></summary>
		private Brush topColorBrush;
		/// <summary></summary>
		private Brush bottomColorBrush;
		/// <summary></summary>
		private Brush gradientBrush;
		/// <summary></summary>
		private Brush invertGradientBrush;
		/// <summary></summary>
		private Brush mouseOverBrush;
		/// <summary></summary>
		private Brush pressedBrush;
		/// <summary></summary>
		private Pen borderPen;
		/// <summary></summary>
		private GraphicsPath fullPath;
		/// <summary></summary>
		private GraphicsPath bottomPath;
		/// <summary></summary>
		private RectangleF topRect;
		/// <summary></summary>
		private RectangleF middleRect;
		/// <summary></summary>
		private RectangleF bottomRect;
		/// <summary></summary>
		private RectangleF fullRectangle;
		/// <summary></summary>
		private RectangleF fillRectangle;
		/// <summary></summary>
		private RectangleF leftBorderRectangle;
		/// <summary></summary>
		private RectangleF topBorderRectangle;
		/// <summary></summary>
		private RectangleF rightBorderRectangle;
		/// <summary></summary>
		private RectangleF bottomBorderRectangle;
		/// <summary></summary>
		private Brush arcColorBrush;
		/// <summary></summary>
		private Brush fillColorBrush;
		/// <summary></summary>
		private Brush fillColorMouseOverBrush;
		/// <summary></summary>
		private Brush fillColorPressedBrush;
		/// <summary></summary>
		private Brush borderColorBrush;
		/// <summary>
		/// The color scheme that the renderer will render. 
		/// </summary>
		private WindowsXPColorScheme colorScheme = WindowsXPColorScheme.DefaultBlue;
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary></summary>
		/// <param name="button"/>
		public WindowsXPButtonRenderer( ButtonAdv button ) : 
			base( button )
		{
			SetColorScheme();
			CreateDrawingObjects();
		}

		/// <summary></summary>
		/// <param name="disposing"></param>
		protected override void OnDispose( bool disposing )
		{
			if( disposing )
			{
				DisposeHelper.Dispose( ref arcColorBrush );
				DisposeHelper.Dispose( ref fillColorBrush );
				DisposeHelper.Dispose( ref fillColorMouseOverBrush );
				DisposeHelper.Dispose( ref fillColorPressedBrush );
				DisposeHelper.Dispose( ref borderColorBrush );
				DisposeHelper.Dispose( ref topColorBrush );
				DisposeHelper.Dispose( ref bottomColorBrush );
				DisposeHelper.Dispose( ref gradientBrush );
				DisposeHelper.Dispose( ref invertGradientBrush );
				DisposeHelper.Dispose( ref mouseOverBrush );
				DisposeHelper.Dispose( ref pressedBrush );
				DisposeHelper.Dispose( ref borderPen );
				DisposeHelper.Dispose( ref fullPath );
				DisposeHelper.Dispose( ref bottomPath );
			}

			base.OnDispose( disposing );
		}
		#endregion

		#region Class overrides
		/// <summary>
		/// Sets the rectangles and paths used to draw the XP button when the size of the button is set.
		/// </summary>
		/// <param name="bounds">Set drawing bounds.</param>
		protected internal override void SetBounds( Rectangle bounds )
		{
			base.SetBounds( bounds );

			// Create a RectangleF for the ClientRectangle.
			RectangleF buttonRect = this.bounds;

			switch( this.colorScheme )
			{
				case WindowsXPColorScheme.DefaultBlue:
					SetBoundsDefaultBlue( buttonRect );
					break;
				case WindowsXPColorScheme.DefaultBlueCombo:
					SetBoundsDefaultBlueCombo( buttonRect );
					break;
				case WindowsXPColorScheme.OliveGreen:
					SetBoundsOliveGreen( buttonRect );
					break;
				case WindowsXPColorScheme.OliveGreenCombo:
					SetBoundsOliveGreenCombo( buttonRect );
					break;
				case WindowsXPColorScheme.Silver:
					SetBoundsSilver( buttonRect );
					break;
				case WindowsXPColorScheme.SilverCombo:
					SetBoundsSilverCombo( buttonRect );
					break;
			}
		}

		/// <summary></summary>
		/// <param name="g"></param>
		public override void Render( Graphics g )
		{
			switch( this.colorScheme )
			{
				case WindowsXPColorScheme.DefaultBlue:
					RenderDefault( g );
					break;
				case WindowsXPColorScheme.DefaultBlueCombo:
					RenderDefaultBlueCombo( g );
					break;
				case WindowsXPColorScheme.OliveGreen:
					RenderDefault( g );
					break;
				case WindowsXPColorScheme.OliveGreenCombo:
					RenderOliveGreenCombo( g );
					break;
				case WindowsXPColorScheme.Silver:
					RenderDefault( g );
					break;
				case WindowsXPColorScheme.SilverCombo:
					RenderSilverCombo( g );
					break;
			}

			DrawTextAndImage( g );
		}

		/// <summary></summary>
		/// <param name="g"></param>
		/// <param name="bounds"></param>
		/// <param name="buttonImageType"></param>
		/// <param name="offset"></param>
		/// <param name="imageColor"></param>
		public override void DrawKnownImage( Graphics g, RectangleF bounds, ButtonTypes buttonImageType, 
			Point offset, Color imageColor )
		{
			if( this.colorScheme == WindowsXPColorScheme.OliveGreenCombo && 
				buttonImageType == ButtonTypes.ComboXPDown )
			{
				base.DrawKnownImage( g, bounds, buttonImageType, offset, Color.White );
			}
			else
			{
				base.DrawKnownImage( g, bounds, buttonImageType, offset, imageColor );
			}
		}
		#endregion

		#region Class helper methods
        /// <summary>
        /// Sets WindowsXP color scheme for the control.
        /// </summary>
        public void SetColorScheme(WindowsXPColorScheme colorScheme)
        {
            this.colorScheme = colorScheme;
            CreateDrawingObjects();
        }

		/// <summary>
		/// Set the color scheme for the button based on the current XP Scheme and
		/// the IsComboButton property.
		/// </summary>
		public void SetColorScheme()
		{
			if( this.Button.IsComboButton )
			{
				if( XPThemes.IsSilverThemeOn )
				{
					this.colorScheme = WindowsXPColorScheme.SilverCombo;
				}
				else if( XPThemes.IsOliveGreenThemeOn )
				{
					this.colorScheme = WindowsXPColorScheme.OliveGreenCombo;
				}
				else
				{
					this.colorScheme = WindowsXPColorScheme.DefaultBlueCombo;
				}
			}
			else
			{
				if( XPThemes.IsSilverThemeOn )
				{
					this.colorScheme = WindowsXPColorScheme.Silver;
				}
				else if( XPThemes.IsOliveGreenThemeOn )
				{
					this.colorScheme = WindowsXPColorScheme.OliveGreen;
				}
				else
				{
					this.colorScheme = WindowsXPColorScheme.DefaultBlue;
				}
			}
		}

		/// <summary></summary>
		private void CreateDrawingObjects()
		{
			switch( this.colorScheme )
			{
				case WindowsXPColorScheme.DefaultBlue:
					topColorBrush = new SolidBrush( WindowsXPColors.DefaultBlueTopColor );
					bottomColorBrush = new SolidBrush( WindowsXPColors.DefaultBlueBottomColor );
					gradientBrush = new LinearGradientBrush( this.bounds,
						WindowsXPColors.DefaultBlueGradientStartColor,
						WindowsXPColors.DefaultBlueGradientEndColor,
						LinearGradientMode.Vertical );

					invertGradientBrush = new LinearGradientBrush( this.bounds,
						WindowsXPColors.DefaultBlueGradientEndColor,
						WindowsXPColors.DefaultBlueGradientStartColor,
						LinearGradientMode.Vertical );

					mouseOverBrush = new SolidBrush( WindowsXPColors.DefaultBlueMouseOverColor );
					pressedBrush = new SolidBrush( Color.Blue );
					borderPen = new Pen( WindowsXPColors.DefaultBlueBorderColor, 1 );
					break;

				case WindowsXPColorScheme.DefaultBlueCombo:
					arcColorBrush = new SolidBrush( WindowsXPColors.DefaultBlueComboArcColor );
					fillColorBrush = new SolidBrush( WindowsXPColors.DefaultBlueComboButtonColor );
					fillColorMouseOverBrush = new SolidBrush( WindowsXPColors.DefaultBlueComboButtonMouseOverColor );
					fillColorPressedBrush = new SolidBrush( WindowsXPColors.DefaultBlueComboButtonPressedColor );
					borderColorBrush = new SolidBrush( WindowsXPColors.DefaultBlueComboButtonBorderColor );
					break;

				case WindowsXPColorScheme.OliveGreen:
					topColorBrush = new SolidBrush( WindowsXPColors.OliveGreenTopColor );
					bottomColorBrush = new SolidBrush( WindowsXPColors.OliveGreenBottomColor );
					gradientBrush = new LinearGradientBrush( this.bounds,
						WindowsXPColors.OliveGreenGradientStartColor,
						WindowsXPColors.OliveGreenGradientEndColor,
						LinearGradientMode.Vertical );

					invertGradientBrush = new LinearGradientBrush( this.bounds,
						WindowsXPColors.OliveGreenGradientEndColor,
						WindowsXPColors.OliveGreenGradientStartColor,
						LinearGradientMode.Vertical );

					mouseOverBrush = new SolidBrush( WindowsXPColors.OliveGreenMouseOverColor );
					pressedBrush = new SolidBrush( Color.Blue );
					borderPen = new Pen( WindowsXPColors.OliveGreenBorderColor, 1 );
					break;

				case WindowsXPColorScheme.OliveGreenCombo:
					arcColorBrush = new SolidBrush( WindowsXPColors.OliveGreenComboArcColor );
					fillColorBrush = new SolidBrush( WindowsXPColors.OliveGreenComboButtonColor );
					fillColorMouseOverBrush = new SolidBrush( WindowsXPColors.OliveGreenComboButtonMouseOverColor );
					fillColorPressedBrush = new SolidBrush( WindowsXPColors.OliveGreenComboButtonPressedColor );
					borderColorBrush = new SolidBrush( WindowsXPColors.OliveGreenComboButtonBorderColor );
					break;

				case WindowsXPColorScheme.Silver:
				case WindowsXPColorScheme.SilverCombo:
					topColorBrush = new SolidBrush( WindowsXPColors.SilverTopColor );
					bottomColorBrush = new SolidBrush( WindowsXPColors.SilverBottomColor );
					gradientBrush = new LinearGradientBrush( this.bounds,
						WindowsXPColors.SilverGradientStartColor,
						WindowsXPColors.SilverGradientEndColor,
						LinearGradientMode.Vertical );

					invertGradientBrush = new LinearGradientBrush( this.bounds,
						WindowsXPColors.SilverGradientEndColor,
						WindowsXPColors.SilverGradientStartColor,
						LinearGradientMode.Vertical );

					mouseOverBrush = new SolidBrush( WindowsXPColors.SilverMouseOverColor );
					pressedBrush = new SolidBrush( Color.Blue );
					borderPen = new Pen( WindowsXPColors.SilverBorderColor, 1 );
					break;
			}
		}

		/// <summary></summary>
		/// <param name="buttonRect"/>
		private void SetBoundsDefaultBlue( RectangleF buttonRect )
		{
			CalcBounds( buttonRect );
            if ( middleRect.Width > 0 && middleRect.Height > 0 )
            {
                gradientBrush = new LinearGradientBrush( middleRect,
                WindowsXPColors.DefaultBlueGradientStartColor,
                WindowsXPColors.DefaultBlueGradientEndColor,
                LinearGradientMode.Vertical );

                invertGradientBrush = new LinearGradientBrush( middleRect,
                    WindowsXPColors.DefaultBlueGradientEndColor,
                    WindowsXPColors.DefaultBlueGradientStartColor,
                    LinearGradientMode.Vertical );
            }
		}

		/// <summary>
		/// Calculates bounds for helper rectangles.
		/// </summary>
		/// <param name="buttonRect"></param>
		private void CalcBounds( RectangleF buttonRect )
		{
			float rectOnlyWidth = buttonRect.Width;
            float rectOnlyHeight = buttonRect.Height / 3;
            float middleTopValue = rectOnlyHeight;
            
            //If use rectOnlyHeight in calculation unnecessary line is drawn.
            if ( middleTopValue > 0 )
            {
                middleTopValue = ( float ) ( Math.Floor( middleTopValue ) + ( 1.0 / 3.0 ) );                
            }

			// The XP button is divided into 3 rectangles that are stacked inside the rectangle part of the full path.
			// The 3 rectangles are approximately 1/3rd of the Button's height with some clearance for the border and selection highlighting.
			topRect = new RectangleF( bounds.Left, bounds.Top, rectOnlyWidth, rectOnlyHeight + c_INCREASE_HEIGHT_VALUE );
            middleRect = new RectangleF( bounds.Left, bounds.Top + middleTopValue, rectOnlyWidth, rectOnlyHeight + c_INCREASE_HEIGHT_VALUE );
			bottomRect = new RectangleF( bounds.Left, bounds.Top + rectOnlyHeight * 2, rectOnlyWidth, rectOnlyHeight );
		}

		/// <summary></summary>
		/// <param name="buttonRect"/>
		private void SetBoundsDefaultBlueCombo( RectangleF buttonRect )
		{
			float paddingFactor = 1;
			float borderFactor = 1;

			fullRectangle = new RectangleF( new PointF( buttonRect.Location.X + paddingFactor, buttonRect.Location.Y + paddingFactor ), new SizeF( buttonRect.Size.Width - 2 * paddingFactor, buttonRect.Size.Height - 2 * paddingFactor ) );
			fillRectangle = new RectangleF( new PointF( buttonRect.Location.X + paddingFactor + borderFactor, buttonRect.Location.Y + paddingFactor + borderFactor ), new SizeF( buttonRect.Size.Width - 2 * paddingFactor - 2 * borderFactor, buttonRect.Size.Height - 2 * paddingFactor - 2 * borderFactor ) );

			topBorderRectangle = new RectangleF( new PointF( buttonRect.Location.X + paddingFactor + borderFactor, buttonRect.Location.Y + paddingFactor ), new SizeF( buttonRect.Size.Width - 2 * paddingFactor - 2 * borderFactor, borderFactor ) ); //borderColorBrush
			rightBorderRectangle = new RectangleF( new PointF( buttonRect.Location.X + buttonRect.Width - paddingFactor - borderFactor, buttonRect.Location.Y + paddingFactor + borderFactor ), new SizeF( borderFactor, buttonRect.Size.Height - 2 * paddingFactor - 2 * borderFactor ) );
			bottomBorderRectangle = new RectangleF( new PointF( buttonRect.Location.X + paddingFactor + borderFactor, buttonRect.Location.Y + buttonRect.Height - paddingFactor - borderFactor ), new SizeF( buttonRect.Size.Width - 2 * paddingFactor - 2 * borderFactor, paddingFactor ) );
			leftBorderRectangle = new RectangleF( new PointF( buttonRect.Location.X + paddingFactor, buttonRect.Location.Y + paddingFactor + borderFactor ), new SizeF( borderFactor, buttonRect.Size.Height - 2 * paddingFactor - 2 * borderFactor ) );
		}

		/// <summary></summary>
		/// <param name="buttonRect"/>
		private void SetBoundsOliveGreen( RectangleF buttonRect )
		{
			CalcBounds( buttonRect );

            if ( middleRect.Width > 0 && middleRect.Height > 0 )
            {
                gradientBrush = new LinearGradientBrush( middleRect,
                    WindowsXPColors.OliveGreenGradientStartColor,
                    WindowsXPColors.OliveGreenGradientEndColor,
                    LinearGradientMode.Vertical );

                invertGradientBrush = new LinearGradientBrush( middleRect,
                    WindowsXPColors.OliveGreenGradientEndColor,
                    WindowsXPColors.OliveGreenGradientStartColor,
                    LinearGradientMode.Vertical );
            }
		}

		/// <summary></summary>
		/// <param name="buttonRect"/>
		private void SetBoundsOliveGreenCombo( RectangleF buttonRect )
		{
			float paddingFactor = 1;
			float borderFactor = 1;

			fullRectangle = new RectangleF( new PointF( buttonRect.Location.X + paddingFactor, buttonRect.Location.Y + paddingFactor ), new SizeF( buttonRect.Size.Width - 2 * paddingFactor, buttonRect.Size.Height - 2 * paddingFactor ) );
			fillRectangle = new RectangleF( new PointF( buttonRect.Location.X + paddingFactor + borderFactor, buttonRect.Location.Y + paddingFactor + borderFactor ), new SizeF( buttonRect.Size.Width - 2 * paddingFactor - 2 * borderFactor, buttonRect.Size.Height - 2 * paddingFactor - 2 * borderFactor ) );

			topBorderRectangle = new RectangleF( new PointF( buttonRect.Location.X + paddingFactor + borderFactor, buttonRect.Location.Y + paddingFactor ), new SizeF( buttonRect.Size.Width - 2 * paddingFactor - 2 * borderFactor, borderFactor ) ); //borderColorBrush
			rightBorderRectangle = new RectangleF( new PointF( buttonRect.Location.X + buttonRect.Width - paddingFactor - borderFactor, buttonRect.Location.Y + paddingFactor + borderFactor ), new SizeF( borderFactor, buttonRect.Size.Height - 2 * paddingFactor - 2 * borderFactor ) );
			bottomBorderRectangle = new RectangleF( new PointF( buttonRect.Location.X + paddingFactor + borderFactor, buttonRect.Location.Y + buttonRect.Height - paddingFactor - borderFactor ), new SizeF( buttonRect.Size.Width - 2 * paddingFactor - 2 * borderFactor, paddingFactor ) );
			leftBorderRectangle = new RectangleF( new PointF( buttonRect.Location.X + paddingFactor, buttonRect.Location.Y + paddingFactor + borderFactor ), new SizeF( borderFactor, buttonRect.Size.Height - 2 * paddingFactor - 2 * borderFactor ) );
		}

		/// <summary></summary>
		/// <param name="buttonRect"/>
		private void SetBoundsSilver( RectangleF buttonRect )
		{
			CalcBounds( buttonRect );
            if ( middleRect.Width > 0 && middleRect.Height > 0 )
            {
                gradientBrush = new LinearGradientBrush( middleRect,
                    WindowsXPColors.SilverGradientStartColor,
                    WindowsXPColors.SilverGradientEndColor,
                    LinearGradientMode.Vertical );

                invertGradientBrush = new LinearGradientBrush( middleRect,
                    WindowsXPColors.SilverGradientEndColor,
                    WindowsXPColors.SilverGradientStartColor,
                    LinearGradientMode.Vertical );
            }
		}

		/// <summary></summary>
		/// <param name="buttonRect"/>
		private void SetBoundsSilverCombo( RectangleF buttonRect )
		{
			// The arc is the corner curve for the XP Button - though we do not use an arc.
			float arcWidth = 1;
			float arcHeight = 1;

			//The width of the border that is drawn around the button.
			float borderGap = 1;

			float widthAdjust = 0;
			float heightAdjust = 1; //TODO compute 

			float rectOnlyWidth = buttonRect.Width - 2 * arcWidth - 2 * borderGap + widthAdjust;
			float rectOnlyHeight = ( buttonRect.Height - 2 * arcHeight - 2 * borderGap ) / 3;

			// This is the full path defining the button shape.
			fullPath = new GraphicsPath();
			fullPath.AddLine( arcWidth + borderGap, 0 /*+ borderGap*/, buttonRect.Width - borderGap - arcWidth, 0 /*+borderGap*/ ); //x1,y1,x2,y2 Top left to Top Right
			fullPath.AddLine( buttonRect.Width - borderGap, arcHeight /*+ borderGap*/, buttonRect.Width - borderGap, buttonRect.Height - arcHeight - borderGap ); // Top Right to BottomRight
			fullPath.AddLine( buttonRect.Width - arcWidth - borderGap, buttonRect.Height - borderGap, arcWidth + borderGap, buttonRect.Height - borderGap ); // Bottom Right to Bottom Left
			fullPath.AddLine( 0 + borderGap, buttonRect.Height - arcHeight - borderGap, 0 + borderGap, arcHeight /*+ borderGap*/ ); //Bottom Left to Top Left

			// The XP button is divided into 3 rectangles that are stacked inside the rectangle part of the full path.
			// The 3 rectangles are approximately 1/3rd of the Button's height with some clearance for the border and selection highlighting.
			topRect = new RectangleF( borderGap + arcWidth + 1, borderGap + arcHeight, rectOnlyWidth - 1, rectOnlyHeight ); //
			middleRect = new RectangleF( borderGap + arcWidth + 1, borderGap + arcHeight + rectOnlyHeight - heightAdjust, rectOnlyWidth - 1, rectOnlyHeight + 2 * heightAdjust );
			bottomRect = new RectangleF( borderGap + arcWidth + 1, borderGap + arcHeight + rectOnlyHeight * 2, rectOnlyWidth - 1, rectOnlyHeight + heightAdjust - 1 );

			gradientBrush = new LinearGradientBrush( middleRect,
				WindowsXPColors.SilverGradientStartColor,
				WindowsXPColors.SilverGradientEndColor,
				LinearGradientMode.Vertical );

			invertGradientBrush = new LinearGradientBrush( middleRect,
				WindowsXPColors.SilverGradientEndColor,
				WindowsXPColors.SilverGradientStartColor,
				LinearGradientMode.Vertical );

			bottomPath = new GraphicsPath();
			bottomPath.AddLine( borderGap, buttonRect.Height - arcHeight - borderGap, buttonRect.Width - borderGap, buttonRect.Height - arcHeight - borderGap );
			bottomPath.AddLine( buttonRect.Width - borderGap - arcWidth, buttonRect.Height - borderGap, borderGap + arcWidth, buttonRect.Width - borderGap );
		}

		/// <summary>
		/// Renders ButtonAdv using visual styles.
		/// </summary>
		/// <param name="g"></param>
		private void RenderDefault( Graphics g )
		{
			if( ( this.Button.State & ButtonAdvState.Pressed ) == ButtonAdvState.Pressed )
			{
				g.FillRectangle( bottomColorBrush, topRect );
				g.FillRectangle( invertGradientBrush, middleRect );
				g.FillRectangle( topColorBrush, bottomRect );

				if( ( this.Button.State & ButtonAdvState.MouseOver ) == ButtonAdvState.MouseOver )
				{
                    using(Pen pen=new Pen( mouseOverBrush ))
                        g.DrawRectangle(pen, new Rectangle(this.bounds.Location, new Size(this.bounds.Width - 1, this.bounds.Height - 1)));
				}
				else
				{
                    using (Pen pen = new Pen(topColorBrush))
                        g.DrawRectangle(pen, new Rectangle(this.bounds.Location, new Size(this.bounds.Width - 1, this.bounds.Height - 1)));
				}
			}
			else if( ( this.Button.State & ButtonAdvState.Default ) == ButtonAdvState.Default )
			{
				g.FillRectangle( topColorBrush, topRect );
				g.FillRectangle( gradientBrush, middleRect );
				g.FillRectangle( bottomColorBrush, bottomRect );
			}
			else if( ( this.Button.State & ButtonAdvState.MouseOver ) == ButtonAdvState.MouseOver )
			{
				g.FillRectangle( topColorBrush, topRect );
				g.FillRectangle( gradientBrush, middleRect );
				g.FillRectangle( bottomColorBrush, bottomRect );
                using (Pen pen = new Pen(mouseOverBrush))
                    g.DrawRectangle(pen, new Rectangle(this.bounds.Location, new Size(this.bounds.Width - 1, this.bounds.Height - 1)));
			}
		}

		/// <summary></summary>
		/// <param name="g"/>
		private void RenderDefaultBlueCombo( Graphics g )
		{
            using (Brush brush = new SolidBrush(this.Button.ComboEditBackColor))
                g.FillRectangle(brush, this.Button.ClientRectangle);

			g.FillRectangle( this.arcColorBrush, this.fullRectangle );
			g.FillRectangle( this.fillColorBrush, this.fillRectangle );

			if( ( this.Button.State & ButtonAdvState.Pressed ) == ButtonAdvState.Pressed )
			{
				if( ( this.Button.State & ButtonAdvState.MouseOver ) == ButtonAdvState.MouseOver )
				{
					g.FillRectangle( this.fillColorMouseOverBrush, this.fillRectangle );
				}
				else
				{
					g.FillRectangle( this.fillColorPressedBrush, this.fillRectangle );
				}
			}
			else if( ( this.Button.State & ButtonAdvState.Default ) == ButtonAdvState.Default )
			{
				g.FillRectangle( this.fillColorBrush, this.fillRectangle );
			}
			else if( ( this.Button.State & ButtonAdvState.MouseOver ) == ButtonAdvState.MouseOver )
			{
				g.FillRectangle( this.fillColorMouseOverBrush, this.fillRectangle );
			}

			g.FillRectangle( this.borderColorBrush, this.leftBorderRectangle );
			g.FillRectangle( this.borderColorBrush, this.rightBorderRectangle );
			g.FillRectangle( this.borderColorBrush, this.topBorderRectangle );
			g.FillRectangle( this.borderColorBrush, this.bottomBorderRectangle );

		}

		/// <summary></summary>
		/// <param name="g"/>
		private void RenderOliveGreenCombo( Graphics g )
		{
            using(Brush brush =new SolidBrush( this.Button.ComboEditBackColor ))
                g.FillRectangle(brush, this.Button.ClientRectangle);

			g.FillRectangle( this.arcColorBrush, this.fullRectangle );
			g.FillRectangle( this.fillColorBrush, this.fillRectangle );

			if( ( this.Button.State & ButtonAdvState.Pressed ) == ButtonAdvState.Pressed )
			{
				if( ( this.Button.State & ButtonAdvState.MouseOver ) == ButtonAdvState.MouseOver )
				{
					g.FillRectangle( this.fillColorPressedBrush, this.fillRectangle );
				}
				else
				{
					g.FillRectangle( this.fillColorMouseOverBrush, this.fillRectangle );
				}
			}
			else if( ( this.Button.State & ButtonAdvState.Default ) == ButtonAdvState.Default )
			{
				g.FillRectangle( this.fillColorBrush, this.fillRectangle );
			}
			else if( ( this.Button.State & ButtonAdvState.MouseOver ) == ButtonAdvState.MouseOver )
			{
				g.FillRectangle( this.fillColorMouseOverBrush, this.fillRectangle );
			}

			g.FillRectangle( this.borderColorBrush, this.leftBorderRectangle );
			g.FillRectangle( this.borderColorBrush, this.rightBorderRectangle );
			g.FillRectangle( this.borderColorBrush, this.topBorderRectangle );
			g.FillRectangle( this.borderColorBrush, this.bottomBorderRectangle );
		}

		/// <summary></summary>
		/// <param name="g"/>
		private void RenderSilverCombo( Graphics g )
		{
            using (Brush brush = new SolidBrush(this.Button.BackColor))
            {
                g.FillRectangle(brush, this.Button.ClientRectangle);
            }
			
			if( this.Button.GetIsLastLeftButton() )
			{
				Rectangle leftRect = new Rectangle( this.Button.ClientRectangle.Location, new Size( this.Button.ClientRectangle.Width / 2, this.Button.ClientRectangle.Height ) );
                using (Brush brush = new SolidBrush(this.Button.ComboEditBackColor))
                {
                    g.FillRectangle(brush, leftRect);
                }
			}

			if( this.Button.GetIsFirstRightButton() )
			{
				Rectangle rightRect = new Rectangle( new Point( this.Button.ClientRectangle.Location.X + this.Button.ClientRectangle.Width / 2, this.Button.ClientRectangle.Location.Y ), new Size( this.Button.ClientRectangle.Width / 2, this.Button.ClientRectangle.Height ) );
                using (Brush brush = new SolidBrush(this.Button.ComboEditBackColor))
                {
                    g.FillRectangle(brush, rightRect);
                }
			}

			if( ( this.Button.State & ButtonAdvState.Pressed ) == ButtonAdvState.Pressed )
			{
				if( ( this.Button.State & ButtonAdvState.MouseOver ) == ButtonAdvState.MouseOver )
				{
					g.FillPath( mouseOverBrush, fullPath );
					g.FillRectangle( bottomColorBrush, topRect );
					g.FillRectangle( gradientBrush, middleRect );
					g.FillRectangle( topColorBrush, bottomRect );
					g.DrawPath( borderPen, fullPath );
				}
				else
				{
					g.FillPath( topColorBrush, fullPath );
					g.FillRectangle( bottomColorBrush, topRect );
					g.FillRectangle( invertGradientBrush, middleRect );
					g.FillRectangle( topColorBrush, bottomRect );
					g.DrawPath( borderPen, fullPath );
				}
			}
			else if( ( this.Button.State & ButtonAdvState.Default ) == ButtonAdvState.Default )
			{
				g.FillPath( topColorBrush, fullPath );
				g.FillRectangle( topColorBrush, topRect );
				g.FillRectangle( gradientBrush, middleRect );
				g.FillRectangle( bottomColorBrush, bottomRect );
				g.FillPath( bottomColorBrush, bottomPath );
				g.DrawPath( borderPen, fullPath );
			}
			else if( ( this.Button.State & ButtonAdvState.MouseOver ) == ButtonAdvState.MouseOver )
			{
				g.FillPath( mouseOverBrush, fullPath );
				g.FillRectangle( topColorBrush, topRect );
				g.FillRectangle( gradientBrush, middleRect );
				g.FillRectangle( bottomColorBrush, bottomRect );
				g.DrawPath( borderPen, fullPath );
			}
		}

		#endregion
	}
}