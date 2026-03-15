#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region System dependencies
using System.Drawing;
using System.Windows.Forms;

using Syncfusion.Drawing;
#endregion Syncfusion dependencies

namespace Syncfusion.Windows.Forms.Tools.Renderers
{
    /// <summary>
    /// Renderer of Office2003 style.
    /// </summary>
    public class Office2003Renderer
    : BasicRenderer
    {
        #region Class construction
        /// <summary>
        /// Initializes a new instance of the Office2003Renderer class.
        /// No possibility to construct this class "normally"!
        /// Use FancyRenderer.GetInstance() property to retrieve instance pointer instead.
        /// </summary>
        protected Office2003Renderer()
        {
            if (null != m_rendererInfo)
            {
                // Initializing default values for this theme.
                m_defaultBackgroundColor = new BrushInfo(Color.FromArgb(218, 234, 253));
                m_defaultExpandFill = new BrushInfo(Color.FromArgb(39, 65, 108));
                m_defaultExpandLine = Color.Transparent;
                m_defaultHotExpandFill = new BrushInfo(Color.FromArgb(39, 65, 158));
                m_defaultHotExpandLine = Color.FromArgb(100, 100, 220);
                m_defaultGripDark = new BrushInfo(Color.FromArgb(39, 65, 118));
                m_defaultGripLight = new BrushInfo(Color.FromArgb(255, 255, 255));
                m_defaultHotGripDark = new BrushInfo(Color.FromArgb(39, 65, 118));
                m_defaultHotGripLight = new BrushInfo(Color.FromArgb(146, 146, 255));
                m_defaultHotBackgroundColor = new BrushInfo(Color.FromArgb(200, 214, 255));
                SetDefaultSettings();
            }
        }
        #endregion Class construction

        #region  class Overridables

        /// <summary>
        /// Draws a thumbnail. Draws it regarding to orientation.
        /// </summary>
        /// <param name="e">Graphics context.</param>
        /// <param name="ri">Container instance.</param>
        /// <param name="bounds">Container bounds, within which a thumbnail should be drown.</param>
        public override void DrawThumbnail(PaintEventArgs e, IRendererInfo ri, Rectangle bounds)
        {
            BasicDrawThumbnail(e, ri, bounds, false);
        }

        private void DrawGripInternal(BasicRendererInfo fri, PaintEventArgs e, Point ptCenter, Point ptGrip)
        {
            for (int i = 0; i < 5; i++)
            {
                if (fri.Orientation == Orientation.Vertical)
                {
                    ptGrip.X = ptCenter.X + m_nGripPadding * i; // Drawing grips from the center to the end
                }
                else
                {
                    ptGrip.Y = ptCenter.Y + m_nGripPadding * i; // Drawing grips from the center to the end
                }

                DrawGrip(e, fri, ptGrip, false); // of the area.

                if (i > 0)
                {
                    if (fri.Orientation == Orientation.Vertical)
                    {
                        ptGrip.X = ptCenter.X - m_nGripPadding * i; // Drawing also to the other side.
                    }
                    else
                    {
                        ptGrip.Y = ptCenter.Y - m_nGripPadding * i; // Drawing also to the other side.
                    }
                    DrawGrip(e, fri, ptGrip, false);
                }
            }
        }

        /// <summary>
        /// Draws 1 grip, resulting from 7 points: 4 for foregroung and 2 for a shadow.
        /// </summary>   
        /// <param name="e">PaintEventArgs. We usually use e.Graphics to draw.</param>
        /// <param name="fri">A container instance.</param>
        /// <param name="ptArea">The point, which should be a center of a grip.</param>
        /// <param name="bHot">True - use hot colors, otherwise false.</param>
        private void DrawGrip(PaintEventArgs e, BasicRendererInfo fri, Point ptArea, bool bHot)
        {
            Rectangle rectDark = new Rectangle(ptArea.X, ptArea.Y, 2, 2);
            Rectangle rectLight = new Rectangle(ptArea.X + 1, ptArea.Y + 1, 2, 2);

            bool bGrip = !fri.GripLight.IsEmpty && !fri.GripDark.IsEmpty;

            // bool bHotGrip = ( !fri.HotGripLight.IsEmpty && !fri.HotGripDark.IsEmpty );
            Brush brushLigh;
            brushLigh = (!bHot && bGrip) ? new SolidBrush(fri.GripLight.BackColor) : null;

            Brush brushDark;
            brushDark = (!bHot && bGrip) ? new SolidBrush(fri.GripDark.BackColor) : null;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

            // draw light part
            if (brushLigh != null && brushDark != null)
            {
                e.Graphics.FillRectangle(brushLigh, rectLight);

                // BrushPaint.FillRectangle( e.Graphics, rectLight, fri.GripLight );
                brushLigh.Dispose();
            }

            // draw dark part
            if (brushDark != null)
            {
                // Although the dark (foreground) part of grip is drawn AFTER a shadow, it is obviousely drawn OVER it.
                e.Graphics.FillRectangle(brushDark, rectDark);

                // BrushPaint.FillRectangle( e.Graphics, rectDark, fri.GripDark );
                brushDark.Dispose();
            }
        }

        /// <summary>
        /// Draws hot thumbnail.
        /// </summary>
        /// <param name="e">Paint context.</param>
        /// <param name="ri">An instance of RendererInfo, which should be used as datasource for painting.</param>
        /// <param name="bounds">Bounds within which we should paint.</param>
        public override void DrawHotThumbnail(PaintEventArgs e, IRendererInfo ri, Rectangle bounds)
        {
            BasicDrawThumbnail(e, ri, bounds, true);
        }
        #endregion class Overridables

        #region class Methods
        /// <summary>
        /// Retrieves an instance of Office2003Renderer
        /// </summary>
        /// <returns>"new Office2003Renderer()"</returns>
        public static new Office2003Renderer GetInstance()
        {
            return new Office2003Renderer();
        }
        #endregion Methods

        #region class helper methods
        /// <summary>
        /// Draws a thumbnail. Draws it regarding to orientation.
        /// </summary>
        /// <param name="e">Graphics context.</param>
        /// <param name="ri">Container instance.</param>
        /// <param name="bounds">Container bounds, within which a thumbnail should be drown.</param>
        /// <param name="bHot">bool value hot</param>
        private void BasicDrawThumbnail(PaintEventArgs e, IRendererInfo ri, Rectangle bounds, bool bHot)
        {
            BasicRendererInfo fri = ri as BasicRendererInfo;
            if (null != fri)
            {
                if (!bounds.IsEmpty && !fri.ExpandFill.IsEmpty)
                {
                    Size szThumbnail = fri.ThumbnailSize;

                    if (szThumbnail != Size.Empty)
                    {
                        // initializing helper variables
                        BrushInfo backBI = bHot ? fri.HotExpandFill : fri.ExpandFill;
                        Color expandLineColor = bHot ? fri.HotExpandLine : fri.ExpandLine;
                        Pen penExpandLine = new Pen(expandLineColor);
                        Rectangle rectThumbnail;
                        Point ptGrip = new Point();
                        Point ptCenter = new Point(bounds.Left + (bounds.Right - bounds.Left) / 2, bounds.Top + (bounds.Bottom - bounds.Top) / 2);
                        
                        if (fri.Orientation == Orientation.Vertical)
                        {
                            rectThumbnail = new Rectangle(ptCenter.X - szThumbnail.Width / 2, ptCenter.Y - szThumbnail.Height / 2, szThumbnail.Width, szThumbnail.Height);
        
                            ptGrip.Y = ptCenter.Y - 1; // Vertical alignment. 
                        }
                        else
                        {
                            rectThumbnail = new Rectangle(ptCenter.X - szThumbnail.Height / 2, ptCenter.Y - szThumbnail.Width / 2, szThumbnail.Height, szThumbnail.Width);
                            ptGrip.X = ptCenter.X - 1; // Horisontal alignment.
                        }

                        // Making arrows.
                        DrawArrows(e, backBI, penExpandLine, rectThumbnail, fri);
                        DrawGripInternal(fri, e, ptCenter, ptGrip);

                        penExpandLine.Dispose();
                    }
                }
            }
        }
        #endregion
    }
}