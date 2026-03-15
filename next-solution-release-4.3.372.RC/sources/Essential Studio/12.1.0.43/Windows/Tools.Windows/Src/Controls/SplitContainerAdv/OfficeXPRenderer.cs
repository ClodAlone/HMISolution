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
using Syncfusion.Windows.Forms.Tools;

#endregion Syncfusion dependencies

namespace Syncfusion.Windows.Forms.Tools.Renderers
{
    /// <summary>
    /// Renderer of OfficeXP style.
    /// </summary>
    public class OfficeXPRenderer
      : BasicRenderer
    {
        #region Class construction
        /// <summary>
        /// Initializes a new instance of the OfficeXPRenderer class.
        /// No possibility to construct this class "normally"!
        /// Use FancyRenderer.GetInstance() property to retrieve instance pointer instead.
        /// </summary>
        protected OfficeXPRenderer()
            : base()
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

        #region Overridables
        /// <summary>
        /// Draws a thumbnail. Draws it regarding to orientation.
        /// </summary>
        /// <param name="e">Graphics context.</param>
        /// <param name="ri">Instance of RendererInfo which is used to retrieve settings from.</param>
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
                        Pen penExpandLine = new Pen(fri.ExpandLine);
                        Point ptCenter = new Point(bounds.Left + (bounds.Right - bounds.Left) / 2, bounds.Top + (bounds.Bottom - bounds.Top) / 2);
                        if (fri.Orientation == Orientation.Horizontal)
                        {
                            Rectangle rectThumbnail = new Rectangle(ptCenter.X - szThumbnail.Height / 2, ptCenter.Y - szThumbnail.Width / 2, szThumbnail.Height, szThumbnail.Width);

                            // Making arrows.
                            DrawArrows(e, fri.ExpandFill, penExpandLine, rectThumbnail, fri);

                            // Computing grip position.
                            Point ptGrip = new Point();
                            ptGrip.X = ptCenter.X - 1;  // Vertical alignment.
                            for (int i = 0; i < 5; i++)
                            {
                                ptGrip.Y = ptCenter.Y + m_nGripPadding * i; // Drawing grips from the center to the end
                                DrawGrip(e, fri, ptGrip, false);            // of the area.
                                if (i > 0)
                                {
                                    ptGrip.Y = ptCenter.Y - m_nGripPadding * i; // Drawing also to the other side.
                                    DrawGrip(e, fri, ptGrip, false);
                                }
                            }
                        }
                        else
                        {
                            Rectangle rectThumbnail = new Rectangle(ptCenter.X - szThumbnail.Width / 2, ptCenter.Y - szThumbnail.Height / 2, szThumbnail.Width, szThumbnail.Height);

                            // Making arrows.
                            DrawArrows(e, fri.ExpandFill, penExpandLine, rectThumbnail, fri);

                            // Computing grip position.
                            Point ptGrip = new Point();
                            ptGrip.Y = ptCenter.Y - 1;  // Horizontal alignment.
                            for (int i = 0; i < 5; i++)
                            {
                                ptGrip.X = ptCenter.X + m_nGripPadding * i; // Drawing grips from the center to the end
                                DrawGrip(e, fri, ptGrip, false);            // of the area.
                                if (i > 0)
                                {
                                    ptGrip.X = ptCenter.X - m_nGripPadding * i; // Drawing also to the other side.
                                    DrawGrip(e, fri, ptGrip, false);
                                }
                            }
                        }
                        penExpandLine.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// Draws 1 grip, resulting from 7 points: 4 for foregroung and 2 for a shadow.
        /// </summary>
        /// <param name="e">PaintEventArgs. We usually use e.Graphics to draw.</param>
        /// <param name="fri">Instance of RendererInfo which is used to retrieve settings from.</param>
        /// <param name="ptArea">The point, which should be a center of a grip.</param>
        /// <param name="bHot">True - use hot colors, otherwise false.</param>
        private void DrawGrip(PaintEventArgs e, BasicRendererInfo fri, Point ptArea, bool bHot)
        {
            Rectangle rectDark = new Rectangle(ptArea.X, ptArea.Y, 2, 2);
            Rectangle rectLight = new Rectangle(ptArea.X + 1, ptArea.Y + 1, 2, 2);
            Color lightColor = bHot ? fri.HotGripLight.BackColor : fri.GripLight.BackColor;
            Color darkColor = bHot ? fri.HotGripDark.BackColor : fri.GripDark.BackColor;

            if (!fri.GripLight.IsEmpty && !fri.GripDark.IsEmpty)
            {
                Brush brGripLight = new SolidBrush(lightColor);
                Brush brGripDark = new SolidBrush(darkColor);

                // Rectangle rectArea = new Rectangle( ptArea.X - 1, ptArea.Y - 1, 3, 3 );
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                e.Graphics.FillRectangle(brGripLight, rectLight);

                // Although the dark (foreground) part of grip is drawn AFTER a shadow, it is obviousely drawn OVER it.
                e.Graphics.FillRectangle(brGripDark, rectDark);
                brGripDark.Dispose();
                brGripLight.Dispose();
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
            BasicRendererInfo fri = ri as BasicRendererInfo;
            if (null != fri)
            {
                if (!bounds.IsEmpty && !fri.HotExpandFill.IsEmpty)
                {
                    Size szThumbnail = fri.ThumbnailSize;
                    if (szThumbnail != Size.Empty)
                    {
                        Pen penHotExpandLine = new Pen(fri.HotExpandLine);
                        Point ptCenter = new Point(bounds.Left + (bounds.Right - bounds.Left) / 2, bounds.Top + (bounds.Bottom - bounds.Top) / 2);
                        if (fri.Orientation == Orientation.Horizontal)
                        {
                            Rectangle rectThumbnail = new Rectangle(ptCenter.X - szThumbnail.Height / 2, ptCenter.Y - szThumbnail.Width / 2, szThumbnail.Height, szThumbnail.Width);

                            // Making arrows.
                            DrawArrows(e, fri.HotExpandFill, penHotExpandLine, rectThumbnail, fri);

                            // Computing grip position.
                            Point ptGrip = new Point();
                            ptGrip.X = ptCenter.X - 1;  // Vertical alignment.
                            for (int i = 0; i < 5; i++)
                            {
                                ptGrip.Y = ptCenter.Y + m_nGripPadding * i; // Drawing grips from the center to the end
                                DrawGrip(e, fri, ptGrip, true);            // of the area.
                                if (i > 0)
                                {
                                    ptGrip.Y = ptCenter.Y - m_nGripPadding * i; // Drawing also to the other side.
                                    DrawGrip(e, fri, ptGrip, true);
                                }
                            }
                        }
                        else
                        {
                            Rectangle rectThumbnail = new Rectangle(ptCenter.X - szThumbnail.Width / 2, ptCenter.Y - szThumbnail.Height / 2, szThumbnail.Width, szThumbnail.Height);

                            // Making arrows.
                            DrawArrows(e, fri.HotExpandFill, penHotExpandLine, rectThumbnail, fri);

                            // Computing grip position.
                            Point ptGrip = new Point();
                            ptGrip.Y = ptCenter.Y - 1;  // Horizontal alignment.
                            for (int i = 0; i < 5; i++)
                            {
                                ptGrip.X = ptCenter.X + m_nGripPadding * i; // Drawing grips from the center to the end
                                DrawGrip(e, fri, ptGrip, true);            // of the area.
                                if (i > 0)
                                {
                                    ptGrip.X = ptCenter.X - m_nGripPadding * i; // Drawing also to the other side.
                                    DrawGrip(e, fri, ptGrip, true);
                                }
                            }
                        }
                        penHotExpandLine.Dispose();
                    }
                }
            }
        }
        #endregion Overridables

        #region Methods
        /// <summary>
        /// Retrieves an instance of OfficeXPRenderer
        /// </summary>
        /// <returns>"new OfficeXPRenderer()"</returns>
        public static new OfficeXPRenderer GetInstance()
        {
            return new OfficeXPRenderer();
        }
        #endregion Methods
    }
}
