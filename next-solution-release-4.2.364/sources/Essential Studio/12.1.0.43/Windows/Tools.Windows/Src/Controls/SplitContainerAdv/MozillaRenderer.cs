#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region System dependencies
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using Syncfusion.Drawing;
#endregion

namespace Syncfusion.Windows.Forms.Tools.Renderers
{
    /// <summary>
    /// Renderer of Mozilla style.
    /// </summary>
    public class MozillaRenderer
      : BasicRenderer
    {
        #region Class construction
        /// <summary>
        /// Initializes a new instance of the MozillaRenderer class
        /// No possibility to construct this class "normally"!
        /// Use MozillaRenderer.GetInstance() method to retrieve instance pointer instead.
        /// </summary>
        protected MozillaRenderer()
        {
            if (null != m_rendererInfo)
            {
                // Initializing default values for this theme.
                m_nGripPadding = 3;
                m_defaultBackgroundColor = new BrushInfo(SystemColors.ControlLight);
                m_defaultHotBackgroundColor = new BrushInfo(Color.FromArgb(204, 204, 255));
                m_defaultExpandFill = new BrushInfo(Color.FromArgb(144, 156, 187));
                m_defaultHotExpandFill = new BrushInfo(Color.FromArgb(49, 49, 99));
                m_defaultExpandLine = Color.FromArgb(144, 156, 187);
                m_defaultHotExpandLine = Color.FromArgb(49, 49, 99);
                m_defaultGripDark = new BrushInfo(SystemColors.Highlight);
                m_defaultHotGripDark = new BrushInfo(SystemColors.Highlight);
                m_defaultGripLight = new BrushInfo(Color.FromArgb(246, 245, 244));
                m_defaultHotGripLight = new BrushInfo(Color.FromArgb(246, 245, 244));
                SetDefaultSettings();
            }
        }
        #endregion Class construction

        #region Overridables
        /// <summary>
        /// Draws a thumbnail. Draws it regarding to orientation.
        /// </summary>
        /// <param name="e">Graphics context.</param>
        /// <param name="ri">Render Info</param>
        /// <param name="bounds">Container bounds, within which a thumbnail should be drown.</param>
        public override void DrawThumbnail(PaintEventArgs e, IRendererInfo ri, Rectangle bounds)
        {
            BasicRendererInfo fri = ri as BasicRendererInfo;
            if (null != fri)
            {
                if (!bounds.IsEmpty && !fri.ExpandFill.IsEmpty)
                {
                    Size szThumbnail = fri.ThumbnailSize;
                    if (szThumbnail != Size.Empty)
                    {
                        SmoothingMode sm = e.Graphics.SmoothingMode;
                        Pen penExpandLine = new Pen(fri.ExpandLine);
                        DrawThumbnailRegardlessToHotness(e, ref bounds, fri, ref szThumbnail, sm, fri.ExpandFill, penExpandLine, false);
                        penExpandLine.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// Incapsulates thumbnail routines similar in DrawThumbnail and DrawHotThumbnail.
        /// </summary>
        /// <param name="e">Drawing context.</param>
        /// <param name="bounds">Bounds to draw within.</param>
        /// <param name="fri">RendererInfo instance to take properties from.</param>
        /// <param name="szThumbnail">Size of thumbnail.</param>
        /// <param name="sm">Smoothing mode.</param>
        /// <param name="brBackColorBrush">Brush to draw thumbnail background color with.</param>
        /// <param name="penExpandLine">Pen to draw bounds of thumbnail arrows.</param>
        /// <param name="bHot">Indicates whether or not we will draw hot thumbnail.</param>
        private void DrawThumbnailRegardlessToHotness(PaintEventArgs e, ref Rectangle bounds, BasicRendererInfo fri, ref Size szThumbnail, SmoothingMode sm, BrushInfo brBackColorBrush, Pen penExpandLine, bool bHot)
        {
            Point ptCenter = new Point(bounds.Left + (bounds.Right - bounds.Left) / 2, bounds.Top + (bounds.Bottom - bounds.Top) / 2);
            Rectangle rectThumbnail;

            if (fri.Orientation == Orientation.Horizontal)
            {
                rectThumbnail = new Rectangle(ptCenter.X - szThumbnail.Height / 2, ptCenter.Y - szThumbnail.Width / 2, szThumbnail.Height, szThumbnail.Width);
            }
            else
            {
                rectThumbnail = new Rectangle(ptCenter.X - szThumbnail.Width / 2, ptCenter.Y - szThumbnail.Height / 2, szThumbnail.Width, szThumbnail.Height);
            }

            // Making arrows.
            DrawArrows(e, brBackColorBrush, penExpandLine, rectThumbnail, fri);

            // Computing grip position.
            Point ptGrip = new Point();
            Point ptNewCenter = ptCenter;
            e.Graphics.SmoothingMode = SmoothingMode.Default;

            if (fri.Orientation == Orientation.Horizontal)
            {
                ptNewCenter.Y = ptNewCenter.Y - 3;
                ptNewCenter.X--;
            }
            else
            {
                ptNewCenter.X = ptNewCenter.X - 3;
                ptNewCenter.Y--;
            }

            for (int i = 0; i < 8; i++)
            {
                int nPadding = m_nGripPadding * i;
                if (fri.Orientation == Orientation.Horizontal)
                {
                    ptGrip.X = ptNewCenter.X;  // Vertical alignment.
                    // ptNewCenter.Y--;
                    DrawGripHorizontalRelativeToPaddingInternal(e, fri, ref ptGrip, ref ptNewCenter, i, nPadding, bHot);

                    // ptGrip.X = ptNewCenter.X - 3;  // Vertical alignment.
                    // ptNewCenter.Y++;
                    // DrawGripHorizontalRelativeToPaddingInternal( e, fri, ref ptGrip, ref ptNewCenter, i, nPadding, bHot );
                }
                else
                {
                    ptGrip.Y = ptNewCenter.Y;  // Horizontal alignment.

                    // ptNewCenter.X--;
                    DrawGripVerticalRelativeToPaddingInternal(e, fri, ref ptGrip, ref ptNewCenter, i, nPadding, bHot);

                    // ptGrip.Y = ptNewCenter.Y - 3;  // Horizontal alignment.
                    // ptNewCenter.X++;
                    // DrawGripVerticalRelativeToPaddingInternal( e, fri, ref ptGrip, ref ptNewCenter, i, nPadding, bHot );
                }
            }
            e.Graphics.SmoothingMode = sm;
        }

        /// <summary>
        /// Draws grip with vertical orientation.
        /// </summary>
        /// <param name="e">Drawing context.</param>
        /// <param name="fri">RendererInfo instance to take properties from.</param>
        /// <param name="ptGrip">Center of a grip.</param>
        /// <param name="ptNewCenter">A new center of a grip.</param>
        /// <param name="i">Incremental internal value.</param>
        /// <param name="nPadding">Padding between grips.</param>
        /// <param name="bHot">Indicates whether we will write hot thumbnail.</param>
        private void DrawGripVerticalRelativeToPaddingInternal(PaintEventArgs e, BasicRendererInfo fri, ref Point ptGrip, ref Point ptNewCenter, int i, int nPadding, bool bHot)
        {
            ptGrip.X = ptNewCenter.X + nPadding; // Drawing grips from the center to the end
            DrawGrip(e, fri, ptGrip, bHot);            // of the area.
            if (i < 7)
            {
                ptGrip.X = ptNewCenter.X - nPadding; // Drawing also to the other side.
                DrawGrip(e, fri, ptGrip, bHot);
            }
        }

        /// <summary>
        /// Draws grip with horizontal orientation.
        /// </summary>
        /// <param name="e">Drawing context.</param>
        /// <param name="fri">RendererInfo instance to take properties from.</param>
        /// <param name="ptGrip">Center of a grip.</param>
        /// <param name="ptNewCenter">A new center of a grip.</param>
        /// <param name="i">Incremental internal value.</param>
        /// <param name="nPadding">Padding between grips.</param>
        /// <param name="bHot">Indicates whether we will write hot thumbnail.</param>
        private void DrawGripHorizontalRelativeToPaddingInternal(PaintEventArgs e, BasicRendererInfo fri, ref Point ptGrip, ref Point ptNewCenter, int i, int nPadding, bool bHot)
        {
            ptGrip.Y = ptNewCenter.Y + nPadding; // Drawing grips from the center to the end
            DrawGrip(e, fri, ptGrip, bHot);            // of the area.
            if (i < 7)
            {
                ptGrip.Y = ptNewCenter.Y - nPadding; // Drawing also to the other side.
                DrawGrip(e, fri, ptGrip, bHot);
            }
        }

        /// <summary>
        /// Draws 1 grip, resulting from 7 points: 4 for foregroung and 2 for a shadow.
        /// </summary>
        /// <param name="e">PaintEventArgs. We usually use e.Graphics to draw.</param>
        /// <param name="fri">Basic Render info</param>
        /// <param name="ptArea">The point, which should be a center of a grip.</param>
        /// <param name="bHot">Bool  for hot</param>   
        private void DrawGrip(PaintEventArgs e, BasicRendererInfo fri, Point ptArea, bool bHot)
        {
            Rectangle rectLight = new Rectangle(ptArea.X, ptArea.Y, 1, 1);
            Rectangle rectDark = new Rectangle(ptArea.X + 1, ptArea.Y + 1, 1, 1);
            Brush brGripLight = new SolidBrush(SystemColors.Control);
            Brush brGripDark = new SolidBrush(SystemColors.Control);

            if (!bHot)
            {
                if (!fri.GripLight.IsEmpty && !fri.GripDark.IsEmpty)
                {
                    brGripLight = new SolidBrush(fri.GripLight.BackColor);
                    brGripDark = new SolidBrush(fri.GripDark.BackColor);
                }
            }
            else
            {
                if (!fri.HotGripLight.IsEmpty && !fri.HotGripDark.IsEmpty)
                {
                    brGripLight = new SolidBrush(fri.HotGripLight.BackColor);
                    brGripDark = new SolidBrush(fri.HotGripDark.BackColor);
                }
            }

            e.Graphics.FillRectangle(brGripLight, rectLight);

            // Although the dark (foreground) part of grip is drawn AFTER a shadow, it is obviousely drawn OVER it.
            e.Graphics.FillRectangle(brGripDark, rectDark);
            brGripDark.Dispose();
            brGripLight.Dispose();
        }

        /// <summary>
        /// Draws hot thumbnail.
        /// </summary>
        /// <param name="e">Paint context.</param>
        /// <param name="ri">An instance of RendererInfo, which should be used as datasource for painting.</param>
        /// <param name="bounds">Bounds within which we should paint.</param>
        public override void DrawHotThumbnail(PaintEventArgs e, IRendererInfo ri, Rectangle bounds)
        {
            BasicRendererInfo fri = ri as BasicRendererInfo;
            if (null != fri)
            {
                if (!bounds.IsEmpty && !fri.ExpandFill.IsEmpty)
                {
                    Size szThumbnail = fri.ThumbnailSize;
                    if (szThumbnail != Size.Empty)
                    {
                        SmoothingMode sm = e.Graphics.SmoothingMode;
                        Pen penExpandLine = new Pen(fri.HotExpandLine);
                        DrawThumbnailRegardlessToHotness(e, ref bounds, fri, ref szThumbnail, sm, fri.HotExpandFill, penExpandLine, true);
                        penExpandLine.Dispose();
                    }
                }
            }
        }
        #endregion Overridables

        #region Methods
        /// <summary>
        /// Retrieves an instance of MozillaRenderer
        /// </summary>
        /// <returns>"new MozillaRenderer()"</returns>
        public static new MozillaRenderer GetInstance()
        {
            /*
            if( m_rendererInstance == null || ( m_rendererInstance as MozillaRenderer ) == null  )
            {
              m_rendererInstance = new MozillaRenderer();
            }
            */

            return new MozillaRenderer();
        }
        #endregion Methods
    }
}
