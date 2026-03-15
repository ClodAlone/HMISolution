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
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Scrolling.Renderers;
#endregion

namespace Syncfusion.Windows.Forms.Renderers
{
    /// <summary>WindowsXP Silver Style renderer implementation.</summary>
    public class WindowsXPSilverRenderer
        : WindowsXPRenderer
    {
        #region Class members
        /// <summary>
        /// Blend for the default and selected arrow button.
        /// </summary>
        private Blend m_blendArrowButtonDefault = null;
        /// <summary>
        /// Blend for the pushed arrow button.
        /// </summary>
        private Blend m_blendArrowButtonPushed = null;
        /// <summary>
        /// Blend for the thumb.
        /// </summary>
        private Blend m_blendThumb = null;
        /// <summary>
        /// Blend for the pushed thumb.
        /// </summary>
        private Blend m_blendThumbPushed = null;
        /// <summary>
        /// Blend for the background.
        /// </summary>
        private Blend m_blendBackGround = null;
        /// <summary></summary>
        private Color m_defaultBackGroundBorderColor = Color.FromArgb(229, 230, 238);
        /// <summary></summary>
        private Color m_defaultBackGroundStartColor = Color.FromArgb(236, 238, 243);
        /// <summary></summary>
        private Color m_defaultBackGroundEndColor = Color.FromArgb(251, 251, 254);
        /// <summary></summary>
        private Color m_pushedBackGroundBorderColor = Color.FromArgb(194, 195, 215);
        /// <summary></summary>
        private Color m_pushedBackGroundStartColor = Color.FromArgb(211, 215, 227);
        /// <summary></summary>
        private Color m_pushedBackGroundEndColor = Color.FromArgb(246, 246, 253);
        /// <summary></summary>
        private Color m_defaultBorderColor = Color.FromArgb(148, 149, 162);
        /// <summary></summary>
        private Color m_pushedBorderColor = Color.FromArgb(91, 102, 101);
        /// <summary></summary>
        private Color m_pushedThumbBorderColor = Color.FromArgb(160, 181, 205);
        /// <summary></summary>
        private Color m_defaultArrowButtonStartColor = Color.White;
        /// <summary></summary>
        private Color m_defaultArrowButtonEndColor = Color.FromArgb(203, 204, 218);
        /// <summary></summary>
        private Color m_pushedArrowButtonStartColor = Color.FromArgb(191, 194, 219);
        /// <summary></summary>
        private Color m_pushedArrowButtonEndColor = Color.White;
        /// <summary></summary>
        private Color m_arrowColor = Color.FromArgb(63, 61, 61);
        /// <summary></summary>
        private Color m_defaultThumbStartColor = Color.White;
        /// <summary></summary>
        private Color m_defaultThumbEndColor = Color.FromArgb(199, 200, 214);
        #endregion

        #region class initialize\finalize methods

        /// <summary>
        /// Initialize new instance of WindowsXPRenderer
        /// </summary>
        protected internal WindowsXPSilverRenderer(bool isVerticalScrollBar)
            : base( isVerticalScrollBar )
        {
            IsVerticalScrollBar = isVerticalScrollBar;

            m_blendArrowButtonDefault = new Blend();
            m_blendArrowButtonDefault.Positions = new float[] { 0.0F, 0.45F, 1.0F };
            m_blendArrowButtonDefault.Factors = new float[] { 0.0F, 0.7F, 1.0F };

            m_blendArrowButtonPushed = new Blend();
            m_blendArrowButtonPushed.Positions = new float[] { 0.0F, 0.3F, 1.0F };
            m_blendArrowButtonPushed.Factors = new float[] { 0.0F, 0.7F, 1.0F };

            m_blendThumb = new Blend();
            m_blendThumb.Positions = new float[] { 0.0F, 0.2F, 0.2F, 0.4F, 0.4F, 0.6F, 0.6F, 0.8F, 0.8F, 1.0F };
            m_blendThumb.Factors = new float[] { 0.0F, 0.1F, 0.2F, 0.3F, 0.4F, 0.5F, 0.6F, 0.7F, 0.8F, 1.0F };

            m_blendThumbPushed = new Blend();
            m_blendThumbPushed.Positions = new float[] { 0.0F, 0.2F, 0.2F, 0.4F, 0.4F, 0.6F, 0.6F, 0.7F, 0.7F, 1.0F };
            m_blendThumbPushed.Factors = new float[] { 0.0F, 0.0F, 0.2F, 0.3F, 0.4F, 0.6F, 0.7F, 0.8F, 0.9F, 1.0F };

            m_blendBackGround = new Blend();
            m_blendBackGround.Positions = new float[] { 0.0F, 0.55F, 1.0F };
            m_blendBackGround.Factors = new float[] { 0.0F, 0.9F, 1.0F };
        }

        #endregion

        #region Class overrides
        /// <summary>
        /// Draws arrow button of scroll. If theme is disabled than draw classic scroll. 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="bounds"></param>
        /// <param name="type"></param>
        /// <param name="state"></param>
        public override void DrawArrowButton( Graphics g, Rectangle bounds, ScrollButton type, ButtonState state )
        {
            if ( bounds.Width > 0 && bounds.Height > 0 )
            {
                DrawBackgroundLines( g, bounds, m_defaultBackGroundBorderColor );

                Rectangle rc;
                if ( IsVerticalScrollBar )
                {
                    rc = new Rectangle( bounds.X + 1, bounds.Y, bounds.Width - 1, bounds.Height );
                }
                else
                {
                    rc = new Rectangle( bounds.X, bounds.Y + 1, bounds.Width, bounds.Height - 1 );
                }
                                                
                switch ( state )
                {
                    case ButtonState.Normal:
                        DrawBorders( g, rc, m_defaultBorderColor, state );
                        DrawDefaultArrowButton( g, rc );
                        break;

                    case ButtonState.Checked:
                        DrawBorders( g, rc, m_pushedBorderColor, state );
                        DrawDefaultArrowButton(g, rc);
                        break;

                    case ButtonState.Pushed:
                        DrawBorders( g, rc, m_pushedBorderColor, state );
                        DrawPushedArrowButton( g, rc );
                        break;
                }

                DrawArrows(g, bounds, type, m_arrowColor);
            }
        }

        /// <summary>
        /// Draws background of scroll. If theme is disabled than draw classic scroll. 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="bounds"></param>
        /// <param name="state"></param>
        public override void DrawBackground( Graphics g, Rectangle bounds, ButtonState state )
        {
            if ( bounds.Width > 0 && bounds.Height > 0 )
            {
                if ( state == ButtonState.Normal )
                {
                    DrawDefaultBackground( g, bounds );
                }
                else if ( state == ButtonState.Pushed )
                {
                    DrawPushedBackground( g, bounds );
                }
            }
        }

        /// <summary>
        /// Draws thumb for scroll. If theme is disabled than draw classic scroll. 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="bounds"></param>
        /// <param name="state"></param>
        public override void DrawThumb( Graphics g, Rectangle bounds, ButtonState state )
        {
            if ( bounds.Width > 0 && bounds.Height > 0 )
            {
                DrawBackground( g, bounds, ButtonState.Normal );

                Rectangle rc;
                if ( IsVerticalScrollBar )
                {
                    rc = new Rectangle( bounds.X + 1, bounds.Y, bounds.Width - 1, bounds.Height );
                }
                else
                {
                    rc = new Rectangle( bounds.X, bounds.Y + 1, bounds.Width, bounds.Height - 1 );
                }                               
                
                switch ( state )
                {
                    case ButtonState.Normal:
                        DrawBorders( g, rc, m_defaultBorderColor, state );
                        DrawDefaultThumb( g, rc );
                        break;

                    case ButtonState.Pushed:
                        DrawBorders( g, rc, m_pushedBorderColor, state );
                        DrawPushedThumb( g, rc );
                        break;

                    case ButtonState.Checked:
                        DrawBorders( g, rc, m_pushedBorderColor, state );
                        DrawDefaultThumb( g, rc );
                        break;
                }

                DrawMiddleLinesForThumb( g, bounds, Color.White, m_defaultBorderColor );
            }
        }
        #endregion

        #region Class Utility methods

        /// <summary>
        /// Fills background with specified colors and gradient.
        /// </summary>
        /// <param name="g">Graphics object to use.</param>
        /// <param name="bounds">Bounds of the background.</param>
        /// <param name="startColor">Start color of the gradient.</param>
        /// <param name="endColor">End color of the gradient.</param>
        protected void FillBackground( Graphics g, Rectangle bounds, Color startColor, Color endColor )
        {
            if ( IsVerticalScrollBar )
            {
                bounds.Inflate( -1, 0 );
                using ( LinearGradientBrush bgBrush = GetVerticalBrush(bounds, startColor, endColor ) )
                {
                    bgBrush.Blend = m_blendBackGround;
                    g.FillRectangle( bgBrush, bounds );
                }
            }
            else
            {
                bounds.Inflate( 0, -1 );
                using ( LinearGradientBrush bgBrush = GetHorizontalBrush( bounds, startColor, endColor ) )
                {
                    bgBrush.Blend = m_blendBackGround;
                    g.FillRectangle( bgBrush, bounds );
                }
            }
        }

        /// <summary>
        /// Draws default background.
        /// </summary>
        /// <param name="g">Graphics object to use.</param>
        /// <param name="bounds">Bounds of background.</param>
        private void DrawDefaultBackground( Graphics g, Rectangle bounds )
        {
            DrawBackgroundLines( g, bounds, m_defaultBackGroundBorderColor );
            FillBackground( g, bounds, m_defaultBackGroundStartColor, m_defaultBackGroundEndColor );
        }

        /// <summary>
        /// Draws pushed background.
        /// </summary>
        /// <param name="g">Graphics object to use.</param>
        /// <param name="bounds">Bounds of background.</param>
        private void DrawPushedBackground( Graphics g, Rectangle bounds )
        {
            DrawBackgroundLines( g, bounds, m_pushedBackGroundBorderColor );
            FillBackground( g, bounds, m_pushedBackGroundStartColor, m_pushedBackGroundEndColor );
        }

        /// <summary>
        /// Draws default arrow button.
        /// </summary>
        /// <param name="g">Graphics object to use.</param>
        /// <param name="bounds">Bounds of the arrow button.</param>
        private void DrawDefaultArrowButton( Graphics g, Rectangle bounds )
        {
            bounds.Inflate( -1, -1 );
            using ( LinearGradientBrush bgBrush = GetHorizontalBrush( bounds, m_defaultArrowButtonStartColor, m_defaultArrowButtonEndColor ) )
            {
                bounds.Inflate( -1, -1 );
                bgBrush.Blend = m_blendArrowButtonDefault;
                g.FillRectangle( bgBrush, bounds );
            }
        }

        /// <summary>
        /// Draws pushed arrow button.
        /// </summary>
        /// <param name="g">Graphics object to use.</param>
        /// <param name="bounds">Bounds of the arrow button.</param>
        private void DrawPushedArrowButton( Graphics g, Rectangle bounds )
        {
            bounds.Inflate( -1, -1 );
            using ( LinearGradientBrush bgBrush = GetHorizontalBrush( bounds, m_pushedArrowButtonStartColor, m_pushedArrowButtonEndColor ) )
            {
                bounds.Inflate( -1, -1 );
                bgBrush.Blend = m_blendArrowButtonPushed;
                g.FillRectangle( bgBrush, bounds );
            }
        }

        /// <summary>
        /// Draws default thumb.
        /// </summary>
        /// <param name="g">Graphics object to use.</param>
        /// <param name="bounds">Bounds of the thumb.</param>
        private void DrawDefaultThumb( Graphics g, Rectangle bounds )
        {
            Rectangle rc = bounds;            

            if ( IsVerticalScrollBar )
            {                
                using ( LinearGradientBrush bgBrush = GetVerticalBrush( rc, m_defaultThumbStartColor, m_defaultThumbEndColor ) )
                {
                    bgBrush.Blend = m_blendThumb;
                    rc = new Rectangle( rc.X + 2, rc.Y + 2, rc.Width - 4, rc.Height - 4 );
                    g.FillRectangle( bgBrush, rc );
                }
            }
            else
            {
                using ( LinearGradientBrush bgBrush = GetHorizontalBrush( rc, m_defaultThumbStartColor, m_defaultThumbEndColor ) )
                {
                    bgBrush.Blend = m_blendThumb;
                    rc = new Rectangle( rc.X + 2, rc.Y + 2, rc.Width - 4, rc.Height - 3 );
                    g.FillRectangle( bgBrush, rc );
                }
            }            
        }

        /// <summary>
        /// Draws pushed thumb.
        /// </summary>
        /// <param name="g">Graphics object to use.</param>
        /// <param name="bounds">Bounds of the thumb.</param>
        private void DrawPushedThumb(Graphics g, Rectangle bounds)
        {
            Rectangle rc = bounds;

            if ( IsVerticalScrollBar )
            {
                using ( LinearGradientBrush bgBrush = GetVerticalBrush( rc, m_defaultThumbEndColor, m_defaultThumbStartColor ) )
                {
                    bgBrush.Blend = m_blendThumbPushed;
                    rc = new Rectangle( rc.X + 2, rc.Y + 2, rc.Width - 4, rc.Height - 4 );
                    g.FillRectangle( bgBrush, rc );
                }
            }
            else
            {
                using ( LinearGradientBrush bgBrush = GetHorizontalBrush( rc, m_defaultThumbEndColor, m_defaultThumbStartColor ) )
                {
                    bgBrush.Blend = m_blendThumbPushed;
                    rc = new Rectangle( rc.X + 2, rc.Y + 2, rc.Width - 4, rc.Height - 4 );
                    g.FillRectangle( bgBrush, rc );
                }
            } 

            using ( Pen pen = new Pen( m_pushedThumbBorderColor ) )
            {
                g.DrawLine( pen, rc.Left - 1, rc.Bottom, rc.Right - 1, rc.Bottom );
                g.DrawLine( pen, rc.Left - 1, rc.Bottom - 1, rc.Left, rc.Bottom );
                g.DrawLine( pen, rc.Right - 1, rc.Bottom, rc.Right, rc.Bottom - 1 );
            }    
        }

        /// <summary>
        /// Draws borders for the arrow buttons and thumbs.
        /// </summary>
        /// <param name="g">The graphics object to use.</param>
        /// <param name="bounds">Bounds of the arrow button.</param>
        private void DrawBorders( Graphics g, RectangleF bounds, Color borderColor, ButtonState state )
        {
            InitializeRectangleEdges( bounds );
           
            int iRadius1 = DEF_BORDERS_RADIUS;

            Color bc = borderColor;
            if ( state == ButtonState.Normal )
            {
                bc = Color.FromArgb(200, borderColor);
            }
            else
            if ( state == ButtonState.Pushed )
            {
                using ( Pen myPen = new Pen( m_defaultBackGroundEndColor ) )
                {
                    g.DrawLine( myPen, m_iRight - 2, m_iTop + iRadius1, m_iLeft + 2, m_iTop + iRadius1 );
                }
            }


            using ( Pen myPen = new Pen( bc ) )
            {
                g.DrawLine( myPen, m_iRight - iRadius1, m_iBottom, m_iLeft + iRadius1, m_iBottom );
                g.DrawLine( myPen, m_iLeft + iRadius1, m_iTop, m_iRight - iRadius1, m_iTop );
            }

            using ( Pen myPen = new Pen( borderColor ) )
            {
                g.DrawLine( myPen, m_iRight, m_iTop + iRadius1, m_iRight, m_iBottom - iRadius1 );
                g.DrawLine( myPen, m_iRight, m_iBottom - iRadius1, m_iRight - iRadius1, m_iBottom );
                g.DrawLine( myPen, m_iLeft + iRadius1, m_iBottom, m_iLeft, m_iBottom - iRadius1 );
                g.DrawLine( myPen, m_iLeft, m_iBottom - iRadius1, m_iLeft, m_iTop + iRadius1 );
                g.DrawLine( myPen, m_iLeft, m_iTop + iRadius1, m_iLeft + iRadius1, m_iTop );
                g.DrawLine( myPen, m_iRight - iRadius1, m_iTop, m_iRight, m_iTop + iRadius1 );
            }

            DrawBordersInternal( g, bounds );
        }

        /// <summary>
        /// Draws internal borders for the arrow buttons and thumbs.
        /// </summary>
        /// <param name="g">The graphics object to use.</param>
        /// <param name="bounds">Bounds of the arrow button.</param>
        private void DrawBordersInternal( Graphics g, RectangleF bounds )
        {
            InitializeRectangleEdges( bounds );

            int iRadius1 = DEF_BORDERS_RADIUS;

            using ( Pen myPen = new Pen( Color.FromArgb( 100, m_defaultBorderColor ) ) )
            {
                g.DrawLine( myPen, m_iLeft + iRadius1, m_iTop + iRadius1, m_iLeft, m_iTop );
                g.DrawLine( myPen, m_iRight, m_iTop, m_iRight - iRadius1, m_iTop + iRadius1 );
            }

            using ( Pen myPen = new Pen( m_defaultBackGroundBorderColor ) )
            {
                g.DrawLine( myPen, m_iLeft, m_iBottom, m_iLeft + iRadius1, m_iBottom - iRadius1 );
                g.DrawLine( myPen, m_iRight - iRadius1, m_iBottom - iRadius1, m_iRight, m_iBottom );
                g.DrawLine( myPen, m_iRight - iRadius1, m_iBottom - iRadius1, m_iLeft + iRadius1, m_iBottom - iRadius1 );
            }
        }              

		#endregion
    }
}