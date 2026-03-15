#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Syncfusion.Windows.Forms
{
    class Office2007TabsRenderer : TabsRendererBase
    {
        #region Class constants
        private const int c_defaultOverlapWidth = 16;
        private const int c_cornerCut = 2;
        #endregion

        #region Class members
        /// <summary>
        /// Blend used to draw the tab.
        /// </summary>
        private Blend m_tabNormalBlend = new Blend();
        /// <summary>
        /// Blend used to draw the tab.
        /// </summary>
        private Blend m_tabPushedBlend = new Blend();
        /// <summary>
        /// Blend used to draw the tab.
        /// </summary>
        private Blend m_tabHoveredBlend = new Blend();
        #endregion

        #region Class initialize

        public Office2007TabsRenderer( InternalTab parent )
            : base( parent )
        {
            m_tabNormalBlend.Positions = new float[] { 0f, 0.4f, 0.4f, 0.55f, 0.55f, 0.7f, 0.7f, 1f };
            m_tabNormalBlend.Factors = new float[] { 0.1f, 0.1f, 1f, 0.6f, 0.45f, 0.35f, 0.2f, 0f };

            m_tabPushedBlend.Positions = new float[] { 0f, 0.7f, 0.7f, 1f };
            m_tabPushedBlend.Factors = new float[] { 0f, 0.1f, 0.5f, 0.95f };

            m_tabHoveredBlend.Positions = new float[] { 0f, 0.65f, 0.65f, 1f };
            m_tabHoveredBlend.Factors = new float[] { 0f, 0.1f, 0.4f, 0.9f };
        }

        #endregion

        #region Class properties
        /// </override>
        protected override Font ActiveTabFont
        {
            get
            {
                return new Font( base.ActiveTabFont, FontStyle.Bold );
            }
        }

        /// </override>
        protected override Color ForeColor
        {
            get
            {
                if( this.Parent != null )
                {
                    return this.Parent.Office2007ColorTable.TabBarSplitterTextColor;
                }
                else
                {
                    return base.ForeColor;
                }
            }
        }

        /// </override>
        public override Region GetTabRegion
        {
            get
            {
                GraphicsPath path = this.GetBorderPathFromBounds( this.Bounds );
                return new Region( path );
            }
        }
        #endregion

        #region Class overrides

        /// </override>
        public override void DrawBackground( Graphics g )
        {
            Rectangle rectFill = this.Bounds;
            rectFill.X += 1;
            rectFill.Width -= 1; 

            Color startColor = this.Parent.Office2007ColorTable.TabBarSplitterTabStartColor;
            Color endColor = this.Parent.Office2007ColorTable.TabBarSplitterTabEndColor;
        
            if( this.Parent.Pushed || this.Parent.Hovered )
            {
                if( this.Parent.Hovered )
                {
                    rectFill.Y += 1;
                    rectFill.Height -= 1;
                    rectFill.Width -= 1; 
                }

                using( GraphicsPath gp = GetBorderPathFromBounds( rectFill ) )
                using( Brush br = new SolidBrush( Color.White ) )
                {
                    
                    g.FillPath( br, gp );
                }

                rectFill.X += 1;
                rectFill.Width -= 2;
                rectFill.Height -= 1;

                using( GraphicsPath gp = GetBorderPathFromBounds( rectFill ) )
                using( LinearGradientBrush linearBr = new LinearGradientBrush( rectFill, Color.White, endColor, LinearGradientMode.Vertical ) )
                {
                    if( this.Parent.Pushed )
                    {
                        linearBr.Blend = m_tabPushedBlend;
                    }
                    else
                    {
                        linearBr.Blend = m_tabHoveredBlend;
                    }
                    g.FillPath( linearBr, gp );
                }
            }
            else
            {
                rectFill.Y += 1;
                rectFill.Height -= 1;
                rectFill.Width -= 1; 
                using( GraphicsPath gp = GetBorderPathFromBounds( rectFill ) )
                using( Brush br = new SolidBrush( Color.White )  )
                {
                    g.FillPath( br, gp );
                }
                rectFill.Inflate( -1, -1 );
                rectFill.Width -= 2;
                using( GraphicsPath gp = GetBorderPathFromBounds( rectFill, this.GetOverlappedWidth() - 3 ) )
                using( LinearGradientBrush linearBr = new LinearGradientBrush( rectFill, startColor, endColor, LinearGradientMode.Vertical))
                {
                    linearBr.Blend = m_tabNormalBlend;
                    g.FillPath( linearBr, gp );
                }

                Point p1 = new Point( rectFill.Right, rectFill.Top );
                Point p2 = new Point( rectFill.Right - this.GetOverlappedWidth() + 4, rectFill.Bottom - 2 );
                using( LinearGradientBrush linearBr = new LinearGradientBrush( p1, p2, startColor, Color.FromArgb( 175, endColor ) ) )
                using( Pen pen = new Pen( linearBr ) )
                {
                    g.DrawLine( pen, p1, p2 );
                }
            }
        }

        /// </override>
        public override void DrawBorders( Graphics g )
        {
            using( Pen pen = new Pen( this.Parent.Office2007ColorTable.TabBarSplitterBorderColor ) )
            {
                using( GraphicsPath gp = GetBorderPathFromBounds( this.Bounds ) )
                {
                    g.DrawPath( pen, gp );
                }

                if( !this.Parent.Pushed )
                {
                    g.DrawLine( pen, this.Bounds.Left, this.Bounds.Top, this.Bounds.Right, this.Bounds.Top );
                }
            }
        }

        /// </override>
        public override Size GetItemPreferredSize()
        {
            Size size = base.GetItemPreferredSize();
            size.Width += c_defaultOverlapWidth / 2;
            return size;
        }

        /// </override>
        public override int GetOverlappedWidth()
        {
            return c_defaultOverlapWidth;
        }
        #endregion

        #region Class utility methods
        protected virtual GraphicsPath GetBorderPathFromBounds( RectangleF bounds )
		{
			return this.GetBorderPathFromBounds( bounds, c_defaultOverlapWidth );
		}	
	
        protected virtual GraphicsPath GetBorderPathFromBounds( RectangleF bounds, int overlappedWidth )
		{
			GraphicsPath path = new GraphicsPath();
			float height = this.Bounds.Height;

            PointF[] aptLines = new PointF[]
                {
                    new PointF( bounds.Left, bounds.Top ),
                    new PointF( bounds.Left, bounds.Bottom - c_cornerCut - 1 ),
                    new PointF( bounds.Left + c_cornerCut, bounds.Bottom - 1 ),
                    new PointF( bounds.Right - overlappedWidth, bounds.Bottom - 1 ),
                    new PointF( bounds.Right, bounds.Top )
                };

            path.AddLines( aptLines );

			return path;
		}
        #endregion
    }
}
