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
using Syncfusion.Runtime.InteropServices;
using System;
using System.Windows.Forms;
using System.Reflection;
#endregion

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// Represents renderer of the Office 2007 style for ButtonAdv.
	/// </summary>
	internal class Office2007ButtonRenderer : ButtonRenderer
	{
		#region Class Constants
		/// <summary>
		/// Default radius truncation coreners of the control.
		/// </summary>
		private const int DEF_BORDERS_RADIUS = 1;
		/// <summary>
		/// Angle for vertical gradient brush.
		/// </summary>
		private const int DEF_VERTICAL_BRUSH_ANGLE = 90;
		/// <summary>
		/// Width for brush.
		/// </summary>
		private const int DEF_WIDTH_BRUSH = 1;
		/// <summary>
		/// 
		/// </summary>
		private const ButtonAdvState HIGHTLIGHTED = ButtonAdvState.MouseOver | ButtonAdvState.Pressed;
		#endregion

		#region Class Members
		/// <summary>
		/// Blend for selected control.
		/// </summary>
		private Blend m_blButtonSelected = null;
		/// <summary>
		/// Blend for control.
		/// </summary>
		private Blend m_blButtonDefault = null;
		/// <summary>
		/// Blend for pressed control.
		/// </summary>
		private Blend m_blButtonPressed = null;
		/// <summary>
		/// Blend for disabled control.
		/// </summary>
		private Blend m_blButtonDisabled = null;
		/// <summary>
		/// The color scheme that the renderer will render. 
		/// </summary>
		private Office2007Theme m_colorScheme = Office2007Theme.Blue;
		/// <summary>
		/// Current color table.
		/// </summary>
		private Office2007Colors m_colorTable;
		#endregion

        #region Class Initialize/Finalize Methods

        public Office2007ButtonRenderer( ButtonAdv button ) : 
			base( button )
		{
			m_blButtonSelected = new Blend();
			m_blButtonSelected.Positions = new float[ ]{0.0F, 0.45F, 0.5F, 1.0F};
			m_blButtonSelected.Factors = new float[ ]{0.0F, 0.4F, 0.8F, 0.2F};

			m_blButtonDefault = new Blend();
			m_blButtonDefault.Positions = new float[ ]{0.0F, 0.45F, 0.45F, 1.0F};
			m_blButtonDefault.Factors = new float[ ]{0.0F, 0.5F, 1.1F, 0.5F};

			m_blButtonPressed = new Blend();
			m_blButtonPressed.Positions = new float[ ]{0.0F, 0.50F, 0.55F, 1.0F};
			m_blButtonPressed.Factors = new float[ ]{0.0F, 0.6F, 1.0F, 0.4F};

			m_blButtonDisabled = new Blend();
			m_blButtonDisabled.Positions = new float[ ]{0.0F, 0.45F, 0.5F, 1.0F};
			m_blButtonDisabled.Factors = new float[ ]{0.0F, 0.4F, 1.0F, 0.6F};

			CreateDrawingObjects();
		}

		/// <summary>
		/// Initialize all drawing objects
		/// </summary>
		private void CreateDrawingObjects()
		{
			m_colorScheme = this.Button.Office2007ColorScheme;
			m_colorTable = Office2007Colors.GetColorTable( this.m_colorScheme );			
		}

		/// <summary>Make class cleanup</summary>
		/// <param name="disposing"></param>
		protected override void OnDispose( bool disposing )
		{
			if( disposing )
			{
				m_blButtonSelected = null;
				m_blButtonDefault = null;
				m_blButtonPressed = null;
				m_blButtonDisabled = null;
			}

			base.OnDispose( disposing );
		}

		#endregion

		#region Class Utility Methods
		/// <summary>
		/// Sets vista color scheme for the control.
		/// </summary>
		public override void SetColorScheme( Office2007Theme colorScheme )
		{
			this.m_colorScheme = colorScheme;
			CreateDrawingObjects();
		}

		/// <summary>
		/// Gets rounded path for control.
		/// </summary>
		private GraphicsPath GetRoundedPath( Rectangle bounds, int iRadius, ButtonAdvState state )
		{
			int iLeft = bounds.X;
			int iTop = bounds.Y;
			int iRight = bounds.Right - 1;
			int iBottom = bounds.Bottom - 1;

			GraphicsPath path = new GraphicsPath();

			path.AddLine( iLeft, iBottom - iRadius, iLeft, iTop + iRadius );
			path.AddLine( iLeft, iTop + iRadius, iLeft + iRadius, iTop );
			path.AddLine( iLeft + iRadius, iTop, iRight - iRadius, iTop );
			path.AddLine( iRight - iRadius, iTop, iRight, iTop + iRadius );
			path.AddLine( iRight, iTop + iRadius, iRight, iBottom - iRadius );

			if( state != ButtonAdvState.Pressed )
			{
				path.AddLine( iRight, iBottom - iRadius, iRight - iRadius, iBottom );
				path.AddLine( iRight - iRadius, iBottom, iLeft + iRadius, iBottom );
				path.AddLine( iLeft + iRadius, iBottom, iLeft, iBottom - iRadius );
			}

			return path;
		}

		/// <summary>
		/// Gets rectangle for background.
		/// </summary>
		private Rectangle GetButtonBackgroundRect( Rectangle rc, ButtonAdvState state )
		{
			Rectangle rcResult = rc;
			rcResult.Inflate( -1, -1 );

			if( state == ButtonAdvState.Pressed )
			{
				rcResult.Y = rcResult.Y + 1;
			}

			return rcResult;
		}

		/// <summary>
		/// Gets rectangle for internal border.
		/// </summary>
		private Rectangle GetInternalBorderRect( Rectangle rc, ButtonAdvState state )
		{
			Rectangle rcResult = rc;

			switch( state )
			{
				case ButtonAdvState.Default:
				{
					rcResult = new Rectangle( rc.X + 1, rc.Y + 1, rc.Width - 3, rc.Height - 3 );
					break;
				}
				case ButtonAdvState.MouseOver:
				{
					rcResult = new Rectangle( rc.X + 1, rc.Y + 1, rc.Width - 3, rc.Height - 3 );
					break;
				}
				case ButtonAdvState.Pressed:
				{
					rcResult = new Rectangle( rc.X + 1, rc.Y + 2, rc.Width - 3, rc.Height - 3 );
					break;
				}
			}

			return rcResult;
		}

		/// <summary>
		/// Gets vertical gradient brush.
		/// </summary>
		private LinearGradientBrush GetVerticalBrush( ref Rectangle rc, Color cl1, Color cl2 )
		{
			Rectangle rcBrush = new Rectangle( rc.Left, rc.Top, DEF_WIDTH_BRUSH, rc.Height );

			return new LinearGradientBrush( rcBrush, cl1, cl2, DEF_VERTICAL_BRUSH_ANGLE );
		}

		/// <summary>
		/// Draws background.
		/// </summary>
		private void DrawBackground( Graphics g, Rectangle rc, ButtonAdvState state )
		{
			if( rc.Width > 0 && rc.Height > 0 )
			{
				GraphicsState gState = g.Save();
				g.SmoothingMode = SmoothingMode.Default;                  

                if (this.Button.Enabled)
                {
                    
                        if (IsPressed(this.Button))
                        {
                            DrawBackgroundPressed(g, rc);
                        }
                        else if (IsDefault(this.Button))
                        {
                            DrawBackgroundDefault(g, rc);
                        }
                        else if (IsMouseOver(this.Button))
                        {
                            DrawBackgroundSelected(g, rc);
                        }
                      }
                else
                {
                    DrawBackgroundDisabled(g, rc);
                }

				g.Restore( gState );
			}
		}

		/// <summary>
		/// Draws border.
		/// </summary>
		private void DrawBorder( Graphics g, Rectangle rc, ButtonAdvState state, bool enabled )
		{
			if( rc.Width > 0 && rc.Height > 0 )
			{
				GraphicsState gState = g.Save();
				g.SmoothingMode = SmoothingMode.AntiAlias;
                Office2007Colors silverColors = Office2007Colors.GetColorTable(Office2007Theme.Silver);
				if( enabled )
				{
                    
                        switch (state)
                        {
                            case ButtonAdvState.Default:
                                {
                                    if (!this.Button.OverrideFormManagedColor)
                                    {
                                    Pen p=new Pen(m_colorTable.ButtonDefaultBorderColor);
                                    g.DrawPath(p,GetRoundedPath(rc, DEF_BORDERS_RADIUS, state));
                                    Pen p1 = new Pen(m_colorTable.ButtonDefaultInternalBorderColor);
                                    g.DrawRectangle(p1,GetInternalBorderRect(rc, state));
                                    p.Dispose();
                                    p1.Dispose();
                                    }
                                    else
                                    {
                                    Pen p=new Pen(MergeColors(silverColors.ButtonDefaultBorderColor,this.Button.CustomManagedColor));
                                    g.DrawPath(p,GetRoundedPath(rc, DEF_BORDERS_RADIUS, state));
                                    Pen p1=new Pen(MergeColors(silverColors.ButtonDefaultInternalBorderColor,this.Button.CustomManagedColor));
                                    g.DrawRectangle(p1,GetInternalBorderRect(rc, state));
                                    p.Dispose();
                                    p1.Dispose();
                                    }

                                    break;
                                }
                            case ButtonAdvState.MouseOver:
                                {   Pen p=new Pen(Office2007Colors.Default.ButtonSelectedBorderColor);
                                    g.DrawPath(p,GetRoundedPath(rc, DEF_BORDERS_RADIUS, state));
                                    Pen p1=new Pen(m_colorTable.ButtonSelectedInternalBorderColor);
                                    g.DrawRectangle(p1,GetInternalBorderRect(rc, state));
                                    p.Dispose();
                                    p1.Dispose();
                                    break;
                                }
                            case ButtonAdvState.Pressed:
                                {   Pen p=new Pen(Office2007Colors.Default.ButtonPressedBorderColor);
                                    g.DrawPath(p,GetRoundedPath(rc, DEF_BORDERS_RADIUS, state));
                                    Pen p1=new Pen(m_colorTable.ButtonPressedInternalBorderColor);
                                    g.DrawRectangle(p1,GetInternalBorderRect(rc, state));
                                    Color shadowColor = Color.FromArgb(255, m_colorTable.ButtonPressedInternalBorderColor);
                                    Pen shadowPen = new Pen(shadowColor);
                                    g.DrawLine(shadowPen, rc.X + 1, rc.Y + 1, rc.Right - 2, rc.Y + 1);
                                    p.Dispose();
                                    p1.Dispose();
                                    shadowPen.Dispose();
                                    break;
                                }
                        }
                    }                   
				else
				{   Pen p=new Pen( Office2007Colors.Default.ButtonDisabledBorderColor );
					g.DrawPath( p,GetRoundedPath( rc, DEF_BORDERS_RADIUS, state ) );
                    Pen p1=new Pen( m_colorTable.ButtonDefaultInternalBorderColor );
					g.DrawRectangle( p1, GetInternalBorderRect( rc, state ) );
                    p.Dispose();
                    p1.Dispose();
				}

				g.Restore( gState );
			}
		}
      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseColor"></param>
        /// <param name="blendColor"></param>
        /// <returns></returns>
        internal static Color MergeColors(Color baseColor, Color blendColor)
        {
            int r = MergeChannels(baseColor.R, blendColor.R);
            int g = MergeChannels(baseColor.G, blendColor.G);
            int b = MergeChannels(baseColor.B, blendColor.B);

            return Color.FromArgb(r, g, b);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseChannel"></param>
        /// <param name="blendChannel"></param>
        /// <returns></returns>
        private static int MergeChannels(int baseChannel, int blendChannel)
        {
            const int MAX = 255;

            int min = (baseChannel * blendChannel) / MAX;
            int max = MAX - ((MAX - baseChannel) * (MAX - blendChannel)) / MAX;

            return (byte)(min + (baseChannel * (max - min)) / MAX);
        }
		/// <summary>
		/// Draws background for control when it don't press and mouse don't over it.
		/// </summary>
		private void DrawBackgroundDefault( Graphics g, Rectangle rc )
        {
            Office2007Colors silverColors = Office2007Colors.GetColorTable(Office2007Theme.Silver);
			if( rc.Width > 0 && rc.Height > 0 )
			{
                Color cl1 = Color.Empty;
                Color cl2 = Color.Empty;
                if (!this.Button.OverrideFormManagedColor)
                {
                     cl1 = m_colorTable.ButtonDefaultTopColor;
                     cl2 = m_colorTable.ButtonDefaultBottomColor;                
                }
                else
                {
                    //UpdateColors(this.Button.CustomManagedColor);
                    cl1 = MergeColors(silverColors.ButtonDefaultTopColor, this.Button.CustomManagedColor); //m_colorTable.ButtonDefaultTopColor;
                    cl2 = MergeColors(silverColors.ButtonDefaultBottomColor, this.Button.CustomManagedColor);    
                }
                    PaintGradientDefault(g, GetButtonBackgroundRect(rc, ButtonAdvState.Default), cl1, cl2);
                    DrawBorder(g, rc, ButtonAdvState.Default, true);
                }
			
		}

		/// <summary>
		/// Draws background for control when it pressed.
		/// </summary>
		private void DrawBackgroundPressed( Graphics g, Rectangle rc )
		{
			if( rc.Width > 0 && rc.Height > 0 )
			{
				Color cl1 = Office2007Colors.Default.ButtonPressedTopColor;
				Color cl2 = Office2007Colors.Default.ButtonPressedBottomColor; 

				PaintGradientPressed( g, GetButtonBackgroundRect( rc, ButtonAdvState.Pressed ), cl1, cl2 );
				DrawBorder( g, rc, ButtonAdvState.Pressed, true );
			}
		}

		/// <summary>
		/// Draws background for control when mouse over it.
		/// </summary>
		private void DrawBackgroundSelected( Graphics g, Rectangle rc )
		{
			if( rc.Width > 0 && rc.Height > 0 )
			{
				Color cl1 = Office2007Colors.Default.ButtonSelectedTopColor;
				Color cl2 = Office2007Colors.Default.ButtonSelectedBottomColor; 

				PaintGradientSelected( g, GetButtonBackgroundRect( rc, ButtonAdvState.MouseOver ), cl1, cl2 );
				DrawBorder( g, rc, ButtonAdvState.MouseOver, true );
			}
		}

		/// <summary>
		/// Draws background for disable control.
		/// </summary>
		private void DrawBackgroundDisabled( Graphics g, Rectangle rc )
		{
			if( rc.Width > 0 && rc.Height > 0 )
			{
				Color cl1 = Office2007Colors.Default.ButtonDisabledTopColor;
				Color cl2 = Office2007Colors.Default.ButtonDisabledBottomColor; 

				PaintGradientDisabled( g, GetButtonBackgroundRect( rc, ButtonAdvState.Default ), cl1, cl2 );
				DrawBorder( g, rc, ButtonAdvState.MouseOver, false );
			}
		}


		/// <summary>
		/// Fill rectangle with gradient.
		/// </summary>
		private void PaintGradientDefault( Graphics g, Rectangle rc, Color clBegin, Color clEnd )
		{
			if( rc.Width > 0 && rc.Height > 0 )
			{
				using( LinearGradientBrush brush = GetVerticalBrush( ref rc, clBegin, clEnd ) )
				{
					brush.Blend = m_blButtonDefault;

					brush.WrapMode = WrapMode.TileFlipXY;
					g.FillRectangle( brush, rc );
				}
			}
		}

		/// <summary>
		/// Fill rectangle with gradient for pressed control.
		/// </summary>
		private void PaintGradientPressed( Graphics g, Rectangle rc, Color clBegin, Color clEnd )
		{
			if( rc.Width > 0 && rc.Height > 0 )
			{
				using( LinearGradientBrush brush = GetVerticalBrush( ref rc, clBegin, clEnd ) )
				{
					brush.Blend = m_blButtonPressed;
					brush.WrapMode = WrapMode.TileFlipXY;
					g.FillRectangle( brush, rc );
				}
			}
		}

		/// <summary>
		/// Fill rectangle with gradient for selected control.
		/// </summary>
		private void PaintGradientSelected( Graphics g, Rectangle rc, Color clBegin, Color clEnd )
		{
			if( rc.Width > 0 && rc.Height > 0 )
			{
				using( LinearGradientBrush brush = GetVerticalBrush( ref rc, clBegin, clEnd ) )
				{
					brush.Blend = m_blButtonSelected;
					brush.WrapMode = WrapMode.TileFlipXY;
					g.FillRectangle( brush, rc );
				}
			}
		}

		/// <summary>
		/// Fill rectangle with gradient for disabled control.
		/// </summary>
		private void PaintGradientDisabled( Graphics g, Rectangle rc, Color clBegin, Color clEnd )
		{
			if( rc.Width > 0 && rc.Height > 0 )
			{
				using( LinearGradientBrush brush = GetVerticalBrush( ref rc, clBegin, clEnd ) )
				{
					brush.Blend = m_blButtonDisabled;
					brush.WrapMode = WrapMode.TileFlipXY;
					g.FillRectangle( brush, rc );
				}
			}
		}
		#endregion

		#region Class Overrides
		/// <summary></summary>
		/// <param name="g"></param>        
		public override void Render( Graphics g )
		{
            DrawBackground( g, this.bounds, this.Button.State );            
			DrawTextAndImage( g );
		}

        /// <summary>
        /// Specifies region for drawing
        /// </summary>
        public override Region GetRegion(Rectangle bounds)
        {
            Region r = new Region( bounds );
            r.Exclude( new Rectangle( bounds.X, bounds.Y, 1, 1 ) );
            r.Exclude( new Rectangle( bounds.Width - 1, bounds.Y, 1, 1 ) );
            r.Exclude( new Rectangle( bounds.X, bounds.Height - 1, 1, 1 ) );
            r.Exclude( new Rectangle( bounds.Width - 1, bounds.Height - 1, 1, 1 ) );
            return r;
        }

		/// <summary>
		/// Draws text on ButtonAdv with specified color
		/// </summary>
		///<param name="g" type="System.Drawing.Graphics"><para>
		/// The graphics object to use.  
		/// </para></param>
		/// <param name="textColor">Color of the text</param>
		public override void DrawText( Graphics g, Color textColor )
		{
			ButtonAdv button = this.Button;

			if (!button.ShouldSerializeForeColor() && m_colorScheme == Office2007Theme.Black)
			{
				if ( (button.State & HIGHTLIGHTED)==0 )
				{
					textColor = Color.White;
				}
			}

			base.DrawText( g, textColor );
		}
		#endregion        
	}
    /// <summary>
    /// Represents renderer of the Office 2007 style for ButtonAdv.
    /// </summary>
    internal class Office2010ButtonRenderer : ButtonRenderer
    {
        #region Class Constants
        /// <summary>
        /// Default radius truncation coreners of the control.
        /// </summary>
        private const int DEF_BORDERS_RADIUS = 1;
        /// <summary>
        /// Angle for vertical gradient brush.
        /// </summary>
        private const int DEF_VERTICAL_BRUSH_ANGLE = 90;
        /// <summary>
        /// Width for brush.
        /// </summary>
        private const int DEF_WIDTH_BRUSH = 1;
        /// <summary>
        /// 
        /// </summary>
        private const ButtonAdvState HIGHTLIGHTED = ButtonAdvState.MouseOver | ButtonAdvState.Pressed;
        #endregion

        #region Class Members
        /// <summary>
        /// Blend for selected control.
        /// </summary>
        private Blend m_blButtonSelected = null;
        /// <summary>
        /// Blend for control.
        /// </summary>
        private Blend m_blButtonDefault = null;
        /// <summary>
        /// Blend for pressed control.
        /// </summary>
        private Blend m_blButtonPressed = null;
        /// <summary>
        /// Blend for disabled control.
        /// </summary>
        private Blend m_blButtonDisabled = null;
        /// <summary>
        /// The color scheme that the renderer will render. 
        /// </summary>
        private Office2010Theme m_colorScheme = Office2010Theme.Blue;
        /// <summary>
        /// Current color table.
        /// </summary>
        private Office2010Colors m_colorTable;
        #endregion

        #region Class Initialize/Finalize Methods

        public Office2010ButtonRenderer(ButtonAdv button) :
            base(button)
        {
            m_blButtonSelected = new Blend();
            m_blButtonSelected.Positions = new float[] { 0.0F, 0.30F, 0.45F, 1.0F };
            m_blButtonSelected.Factors = new float[] { 0.0F, 0.5F, 1.1F, 0.5F };

            m_blButtonDefault = new Blend();
            m_blButtonDefault.Positions = new float[] { 0.0F, 0.30F, 0.45F, 1.0F };
            m_blButtonDefault.Factors = new float[] { 0.0F, 0.5F, 1.1F, 0.5F };

            m_blButtonPressed = new Blend();
            m_blButtonPressed.Positions = new float[] { 0.0F, 0.50F, 0.55F, 1.0F };
            m_blButtonPressed.Factors = new float[] { 0.0F, 0.6F, 1.0F, 0.4F };

            m_blButtonDisabled = new Blend();
            m_blButtonDisabled.Positions = new float[] { 0.0F, 0.45F, 0.5F, 1.0F };
            m_blButtonDisabled.Factors = new float[] { 0.0F, 0.4F, 1.0F, 0.6F };

            CreateDrawingObjects();
        }

        /// <summary>
        /// Initialize all drawing objects
        /// </summary>
        private void CreateDrawingObjects()
        {
            m_colorScheme = this.Button.Office2010ColorScheme;
            m_colorTable = Office2010Colors.GetColorTable(this.m_colorScheme);
        }

        /// <summary>Make class cleanup</summary>
        /// <param name="disposing"></param>
        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {
                m_blButtonSelected = null;
                m_blButtonDefault = null;
                m_blButtonPressed = null;
                m_blButtonDisabled = null;
            }

            base.OnDispose(disposing);
        }

        #endregion

        #region Class Utility Methods
        /// <summary>
        /// Sets vista color scheme for the control.
        /// </summary>
        public override void Set2010ColorScheme(Office2010Theme colorScheme)
        {
            this.m_colorScheme = colorScheme;
            CreateDrawingObjects();
        }

        /// <summary>
        /// Gets rounded path for control.
        /// </summary>
        private GraphicsPath GetRoundedPath(Rectangle bounds, int iRadius, ButtonAdvState state)
        {
            int iLeft = bounds.X;
            int iTop = bounds.Y;
            int iRight = bounds.Right - 1;
            int iBottom = bounds.Bottom - 1;

            GraphicsPath path = new GraphicsPath();

            path.AddLine(iLeft, iBottom - iRadius, iLeft, iTop + iRadius);
            path.AddLine(iLeft, iTop + iRadius, iLeft + iRadius, iTop);
            path.AddLine(iLeft + iRadius, iTop, iRight - iRadius, iTop);
            path.AddLine(iRight - iRadius, iTop, iRight, iTop + iRadius);
            path.AddLine(iRight, iTop + iRadius, iRight, iBottom - iRadius);

            if (state != ButtonAdvState.Pressed)
            {
                path.AddLine(iRight, iBottom - iRadius, iRight - iRadius, iBottom);
                path.AddLine(iRight - iRadius, iBottom, iLeft + iRadius, iBottom);
                path.AddLine(iLeft + iRadius, iBottom, iLeft, iBottom - iRadius);
            }

            return path;
        }

        /// <summary>
        /// Gets rectangle for background.
        /// </summary>
        private Rectangle GetButtonBackgroundRect(Rectangle rc, ButtonAdvState state)
        {
            Rectangle rcResult = rc;
            //rcResult.Inflate(-1, -1);

            if (state == ButtonAdvState.Pressed)
            {
                rcResult.Y = rcResult.Y + 1;
            }

            return rcResult;
        }

        /// <summary>
        /// Gets rectangle for internal border.
        /// </summary>
        private Rectangle GetInternalBorderRect(Rectangle rc, ButtonAdvState state)
        {
            Rectangle rcResult = rc;

            switch (state)
            {
                case ButtonAdvState.Default:
                    {
                        rcResult = new Rectangle(rc.X + 1, rc.Y + 1, rc.Width - 3, rc.Height - 3);
                        break;
                    }
                case ButtonAdvState.MouseOver:
                    {
                        rcResult = new Rectangle(rc.X + 1, rc.Y + 1, rc.Width - 3, rc.Height - 3);
                        break;
                    }
                case ButtonAdvState.Pressed:
                    {
                        rcResult = new Rectangle(rc.X + 1, rc.Y + 2, rc.Width - 3, rc.Height - 3);
                        break;
                    }
            }

            return rcResult;
        }

        /// <summary>
        /// Gets vertical gradient brush.
        /// </summary>
        private LinearGradientBrush GetVerticalBrush(ref Rectangle rc, Color cl1, Color cl2)
        {
            Rectangle rcBrush = new Rectangle(rc.Left, rc.Top, DEF_WIDTH_BRUSH, rc.Height);

            return new LinearGradientBrush(rcBrush, cl1, cl2, DEF_VERTICAL_BRUSH_ANGLE);
        }

        /// <summary>
        /// Draws background.
        /// </summary>
        private void DrawBackground(Graphics g, Rectangle rc, ButtonAdvState state)
        {
            if (rc.Width > 0 && rc.Height > 0)
            {
                GraphicsState gState = g.Save();
                g.SmoothingMode = SmoothingMode.Default;

                if (this.Button.Enabled)
                {

                    if (IsPressed(this.Button))
                    {
                        DrawBackgroundPressed(g, rc);
                    }
                    else if (IsDefault(this.Button))
                    {
                        DrawBackgroundDefault(g, rc);
                    }
                    else if (IsMouseOver(this.Button))
                    {
                        DrawBackgroundSelected(g, rc);
                    }
                }
                else
                {
                    DrawBackgroundDisabled(g, rc);
                }

                g.Restore(gState);
            }
        }

        /// <summary>
        /// Draws border.
        /// </summary>
        private void DrawBorder(Graphics g, Rectangle rc, ButtonAdvState state, bool enabled)
        {
            if (rc.Width > 0 && rc.Height > 0)
            {
                GraphicsState gState = g.Save();
                g.SmoothingMode = SmoothingMode.AntiAlias;
                Office2010Colors silverColors = Office2010Colors.GetColorTable(Office2010Theme.Silver);
                if (enabled)
                {

                    switch (state)
                    {
                        case ButtonAdvState.Default:
                            {
                                if (!this.Button.OverrideFormManagedColor)
                                {
                                    Pen p = new Pen(m_colorTable.ButtonDefaultBorderColor);
                                    g.DrawPath(p, GetRoundedPath(rc, DEF_BORDERS_RADIUS, state));
                                    Pen p1 = new Pen(m_colorTable.ButtonDefaultInternalBorderColor);
                                    g.DrawRectangle(p1, GetInternalBorderRect(rc, state));
                                    p.Dispose();
                                    p1.Dispose();
                                }
                                else
                                {
                                    Pen p = new Pen(MergeColors(silverColors.ButtonDefaultBorderColor, this.Button.CustomManagedColor));
                                    g.DrawPath(p, GetRoundedPath(rc, DEF_BORDERS_RADIUS, state));
                                    Pen p1 = new Pen(MergeColors(silverColors.ButtonDefaultInternalBorderColor, this.Button.CustomManagedColor));
                                    g.DrawRectangle(p1, GetInternalBorderRect(rc, state));
                                    p.Dispose();
                                    p1.Dispose();
                                }

                                break;
                            }
                        case ButtonAdvState.MouseOver:
                            {
                                if (!Button.IsBackStageButton)
                                {
                                    Pen p = new Pen(Office2010Colors.Default.ButtonSelectedBorderColor);
                                    g.DrawPath(p, GetRoundedPath(rc, DEF_BORDERS_RADIUS, state));
                                    Pen p1 = new Pen(m_colorTable.ButtonSelectedInternalBorderColor);
                                    g.DrawRectangle(p1, GetInternalBorderRect(rc, state));
                                    p.Dispose();
                                    p1.Dispose();
                                }
                                else
                                {
                                    Pen p = new Pen(Button.BackColor);
                                    g.DrawPath(p, GetRoundedPath(rc, DEF_BORDERS_RADIUS, state));
                                }
                                break;
                            }
                        case ButtonAdvState.Pressed:
                            {
                                Pen p = new Pen(Office2010Colors.Default.ButtonPressedBorderColor);
                                g.DrawPath(p, GetRoundedPath(rc, DEF_BORDERS_RADIUS, state));
                                Pen p1 = new Pen(m_colorTable.ButtonPressedInternalBorderColor);
                                g.DrawRectangle(p1, GetInternalBorderRect(rc, state));
                                Color shadowColor = Color.FromArgb(255, m_colorTable.ButtonPressedInternalBorderColor);
                                Pen shadowPen = new Pen(shadowColor);
                                g.DrawLine(shadowPen, rc.X + 1, rc.Y + 1, rc.Right - 2, rc.Y + 1);
                                p.Dispose();
                                p1.Dispose();
                                shadowPen.Dispose();
                                break;
                            }
                    }
                }
                else
                {
                    Pen p = new Pen(Office2010Colors.Default.ButtonDisabledBorderColor);
                    g.DrawPath(p, GetRoundedPath(rc, DEF_BORDERS_RADIUS, state));
                    Pen p1 = new Pen(m_colorTable.ButtonDefaultInternalBorderColor);
                    g.DrawRectangle(p1, GetInternalBorderRect(rc, state));
                    p.Dispose();
                    p1.Dispose();
                }

                g.Restore(gState);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseColor"></param>
        /// <param name="blendColor"></param>
        /// <returns></returns>
        internal static Color MergeColors(Color baseColor, Color blendColor)
        {
            int r = MergeChannels(baseColor.R, blendColor.R);
            int g = MergeChannels(baseColor.G, blendColor.G);
            int b = MergeChannels(baseColor.B, blendColor.B);

            return Color.FromArgb(r, g, b);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseChannel"></param>
        /// <param name="blendChannel"></param>
        /// <returns></returns>
        private static int MergeChannels(int baseChannel, int blendChannel)
        {
            const int MAX = 255;

            int min = (baseChannel * blendChannel) / MAX;
            int max = MAX - ((MAX - baseChannel) * (MAX - blendChannel)) / MAX;

            return (byte)(min + (baseChannel * (max - min)) / MAX);
        }
        /// <summary>
        /// Draws background for control when it don't press and mouse don't over it.
        /// </summary>
        private void DrawBackgroundDefault(Graphics g, Rectangle rc)
        {
            Office2010Colors silverColors = Office2010Colors.GetColorTable(Office2010Theme.Silver);
            if (rc.Width > 0 && rc.Height > 0)
            {
                Color cl1 = Color.Empty;
                Color cl2 = Color.Empty;
                if (!this.Button.OverrideFormManagedColor)
                {
                    cl1 = m_colorTable.ButtonDefaultTopColor;
                    cl2 = m_colorTable.ButtonDefaultBottomColor;
                }
                else
                {
                    //UpdateColors(this.Button.CustomManagedColor);
                    cl1 = MergeColors(silverColors.ButtonDefaultTopColor, this.Button.CustomManagedColor); //m_colorTable.ButtonDefaultTopColor;
                    cl2 = MergeColors(silverColors.ButtonDefaultBottomColor, this.Button.CustomManagedColor);
                }
                if (!Button.IsBackStageButton)
                {
                    PaintGradientDefault(g, GetButtonBackgroundRect(rc, ButtonAdvState.Default), cl1, cl2);
                    DrawBorder(g, rc, ButtonAdvState.Default, true);
                }
                else
                    g.FillRectangle(new SolidBrush(ControlPaint.LightLight(ControlPaint.LightLight(ControlPaint.LightLight(Button.BackColor)))), rc);

            }

        }

        /// <summary>
        /// Draws background for control when it pressed.
        /// </summary>
        private void DrawBackgroundPressed(Graphics g, Rectangle rc)
        {
            if (rc.Width > 0 && rc.Height > 0)
            {
                Color cl1 = Office2010Colors.Default.ButtonPressedTopColor;
                Color cl2 = Office2010Colors.Default.ButtonPressedBottomColor;

                if (!Button.IsBackStageButton)
                {
                    PaintGradientPressed(g, GetButtonBackgroundRect(rc, ButtonAdvState.Pressed), cl2, cl1);
                    DrawBorder(g, rc, ButtonAdvState.Pressed, true);
                }
                else
                    g.FillRectangle(new SolidBrush(ControlPaint.LightLight(ControlPaint.LightLight(ControlPaint.LightLight(Button.BackColor)))), rc);
            }
        }

        /// <summary>
        /// Draws background for control when mouse over it.
        /// </summary>
        private void DrawBackgroundSelected(Graphics g, Rectangle rc)
        {
            if (rc.Width > 0 && rc.Height > 0)
            {
                Color cl1 = Office2010Colors.Default.ButtonSelectedTopColor;
                Color cl2 = Office2010Colors.Default.ButtonSelectedBottomColor;
                if (!Button.IsBackStageButton)
                {
                    PaintGradientSelected(g, GetButtonBackgroundRect(rc, ButtonAdvState.MouseOver), cl1, cl2);
                    DrawBorder(g, rc, ButtonAdvState.MouseOver, true);
                }
                else
                {
                    g.FillRectangle(new SolidBrush(ControlPaint.LightLight(ControlPaint.LightLight(ControlPaint.LightLight(Button.BackColor)))), rc);
                    DrawBorder(g, rc, ButtonAdvState.MouseOver, true);
                }
            }
        }

        /// <summary>
        /// Draws background for disable control.
        /// </summary>
        private void DrawBackgroundDisabled(Graphics g, Rectangle rc)
        {
            if (rc.Width > 0 && rc.Height > 0)
            {
                Color cl1 = Office2010Colors.Default.ButtonDisabledTopColor;
                Color cl2 = Office2010Colors.Default.ButtonDisabledBottomColor;

                PaintGradientDisabled(g, GetButtonBackgroundRect(rc, ButtonAdvState.Default), cl1, cl2);
                DrawBorder(g, rc, ButtonAdvState.MouseOver, false);
            }
        }


        /// <summary>
        /// Fill rectangle with gradient.
        /// </summary>
        private void PaintGradientDefault(Graphics g, Rectangle rc, Color clBegin, Color clEnd)
        {
            if (rc.Width > 0 && rc.Height > 0)
            {
                using (LinearGradientBrush brush = GetVerticalBrush(ref rc, clBegin, clEnd))
                {
                    brush.Blend = m_blButtonDefault;

                    brush.WrapMode = WrapMode.TileFlipY;
                    g.FillRectangle(brush, rc);
                }
            }
        }

        /// <summary>
        /// Fill rectangle with gradient for pressed control.
        /// </summary>
        private void PaintGradientPressed(Graphics g, Rectangle rc, Color clBegin, Color clEnd)
        {
            if (rc.Width > 0 && rc.Height > 0)
            {
                using (LinearGradientBrush brush = GetVerticalBrush(ref rc, clBegin, clEnd))
                {
                    brush.Blend = m_blButtonPressed;
                    brush.WrapMode = WrapMode.TileFlipXY;
                    g.FillRectangle(brush, rc);
                }
            }
        }

        /// <summary>
        /// Fill rectangle with gradient for selected control.
        /// </summary>
        private void PaintGradientSelected(Graphics g, Rectangle rc, Color clBegin, Color clEnd)
        {
            if (rc.Width > 0 && rc.Height > 0)
            {
                using (LinearGradientBrush brush = GetVerticalBrush(ref rc, clBegin, clEnd))
                {
                    brush.Blend = m_blButtonSelected;
                    brush.WrapMode = WrapMode.TileFlipX;
                    g.FillRectangle(brush, rc);
                }
            }
        }

        /// <summary>
        /// Fill rectangle with gradient for disabled control.
        /// </summary>
        private void PaintGradientDisabled(Graphics g, Rectangle rc, Color clBegin, Color clEnd)
        {
            if (rc.Width > 0 && rc.Height > 0)
            {
                using (LinearGradientBrush brush = GetVerticalBrush(ref rc, clBegin, clEnd))
                {
                    brush.Blend = m_blButtonDisabled;
                    brush.WrapMode = WrapMode.TileFlipXY;
                    g.FillRectangle(brush, rc);
                }
            }
        }
        #endregion

        #region Class Overrides
        /// <summary></summary>
        /// <param name="g"></param>        
        public override void Render(Graphics g)
        {
            DrawBackground(g, this.bounds, this.Button.State);
            DrawTextAndImage(g);
        }

        /// <summary>
        /// Specifies region for drawing
        /// </summary>
        public override Region GetRegion(Rectangle bounds)
        {
            Region r = new Region(bounds);
            r.Exclude(new Rectangle(bounds.X, bounds.Y, 1, 1));
            r.Exclude(new Rectangle(bounds.Width - 1, bounds.Y, 1, 1));
            r.Exclude(new Rectangle(bounds.X, bounds.Height - 1, 1, 1));
            r.Exclude(new Rectangle(bounds.Width - 1, bounds.Height - 1, 1, 1));
            return r;
        }

        /// <summary>
        /// Draws text on ButtonAdv with specified color
        /// </summary>
        ///<param name="g" type="System.Drawing.Graphics"><para>
        /// The graphics object to use.  
        /// </para></param>
        /// <param name="textColor">Color of the text</param>
        public override void DrawText(Graphics g, Color textColor)
        {
            ButtonAdv button = this.Button;

            if (!button.ShouldSerializeForeColor() && m_colorScheme == Office2010Theme.Black)
            {
                if ((button.State & HIGHTLIGHTED) == 0)
                {
                    textColor = Color.White;
                }
            }

            base.DrawText(g, textColor);
        }
        #endregion
    }
}