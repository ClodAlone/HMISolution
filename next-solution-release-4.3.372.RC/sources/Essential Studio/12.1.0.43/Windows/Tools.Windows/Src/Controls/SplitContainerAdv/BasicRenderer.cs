#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Tools.Enums;

namespace Syncfusion.Windows.Forms.Tools.Renderers
{
    /// <summary>
    /// Basic Renderer.
    /// </summary>
    public class BasicRenderer
      : Renderer
    {
        #region class Default renedrerer properties
        /// <summary>
        ///  Gets default value for ExpandFill
        /// </summary>
        public BrushInfo DefaultExpandFill
        {
            get
            {
                return m_defaultExpandFill;
            }
        }

        /// <summary>
        /// Gets default value for ExpandLine
        /// </summary>
        public Color DefaultExpandLine
        {
            get
            {
                return m_defaultExpandLine;
            }
        }

        /// <summary>
        /// Gets default value for DefaultGripDark
        /// </summary>
        public BrushInfo DefaultGripDark
        {
            get
            {
                return m_defaultGripDark;
            }
        }

        /// <summary>
        /// Gets default value for GripLight
        /// </summary>
        public BrushInfo DefaultGripLight
        {
            get
            {
                return m_defaultGripLight;
            }
        }

        /// <summary>
        /// Gets default value for Background color
        /// </summary>
        public BrushInfo DefaultBackgroundColor
        {
            get
            {
                return m_defaultBackgroundColor;
            }
        }

        /// <summary>
        /// Gets default value for ExpandFill
        /// </summary>
        public BrushInfo DefaultHotExpandFill
        {
            get
            {
                return m_defaultHotExpandFill;
            }
        }

        /// <summary>
        /// Gets default value for HotExpandLine
        /// </summary>
        public Color DefaultHotExpandLine
        {
            get
            {
                return m_defaultHotExpandLine;
            }
        }

        /// <summary>
        /// Gets default value for HotGripDark
        /// </summary>
        public BrushInfo DefaultHotGripDark
        {
            get
            {
                return m_defaultHotGripDark;
            }
        }

        /// <summary>
        /// Gets default value for HotGripLight
        /// </summary>
        public BrushInfo DefaultHotGripLight
        {
            get
            {
                return m_defaultHotGripLight;
            }
        }

        /// <summary>
        /// Gets default value for HotBackgroundColor
        /// </summary>
        public BrushInfo DefaultHotBackgroundColor
        {
            get
            {
                return m_defaultHotBackgroundColor;
            }
        }
        #endregion

        #region class members

        protected BasicRendererInfo m_rendererInfo = new BasicRendererInfo();

        /// <summary>
        /// storage for saving default value for ExpandFill property
        /// </summary>
        protected BrushInfo m_defaultExpandFill;

        /// <summary>
        /// storage for saving default value for ExpandLine property
        /// </summary>
        protected Color m_defaultExpandLine;

        /// <summary>
        /// storage for saving default value for GripDark property
        /// </summary>
        protected BrushInfo m_defaultGripDark;

        /// <summary>
        /// storage for saving default value for GripLight property
        /// </summary>
        protected BrushInfo m_defaultGripLight;

        /// <summary>
        /// storage for saving default value for BackgroundColor property
        /// </summary>
        protected BrushInfo m_defaultBackgroundColor;

        /// <summary>
        /// storage for saving default value for HotExpandFill property
        /// </summary>
        protected BrushInfo m_defaultHotExpandFill;

        /// <summary>
        /// storage for saving default value for HotExpandLine property
        /// </summary>
        protected Color m_defaultHotExpandLine;

        /// <summary>
        /// storage for saving default value for HotExpandGrip property
        /// </summary>
        protected BrushInfo m_defaultHotGripDark;

        /// <summary>
        /// storage for saving default value for HotGripLight property
        /// </summary>
        protected BrushInfo m_defaultHotGripLight;

        /// <summary>
        /// storage for saving default value for HotBackgroundColor property
        /// </summary>
        protected BrushInfo m_defaultHotBackgroundColor;

        /// <summary>
        /// Bit's flag, used for for suspending update. 
        /// </summary>
        private bool m_bUpdate = true;

        /// <summary>
        /// If true than background orientation is autochanged.
        /// </summary>
        protected bool m_bUseOrientation = false;
        #endregion

        #region class properties
        /// <summary>
        /// Gets renderer info
        /// </summary>
        public BasicRendererInfo RendererInfo
        {
            get
            {
                return m_rendererInfo;
            }
        }
        #endregion

        #region class overrides
        /// <summary>
        /// For usage only within SplitContainerAdv class. Just point "this" to this method, so
        /// control properties will became appropriate for this theme.
        /// </summary>
        /// <param name="container">Spliter containeradv container</param>
        /// <param name="bInit">bool value</param>
        /// <returns>Return render info</returns>
        public override IRendererInfo GetAppropriateThemeSettings(SplitContainerAdv container, bool bInit)
        {
            m_bUpdate = false;
            m_rendererInfo.SetSplitContainerByInfo(container, this, m_bUseOrientation, bInit);
            m_bUpdate = true;

            return base.GetAppropriateThemeSettings(container, bInit);
        }

        /// <summary>
        /// Used here just for initialize background / hot background gradient colors in respect to orientation.
        /// </summary>
        /// <param name="e">Paint context.</param>
        /// <param name="ri">An instance of RendererInfo, which should be used as datasource for painting.</param>
        /// <param name="bounds">Bounds within which we should paint.</param>
        public override void Draw(PaintEventArgs e, IRendererInfo ri, Rectangle bounds)
        {
            base.Draw(e, ri, bounds);
        }

        /// <summary>
        /// Draws background on graphics of some object.
        /// </summary>
        /// <param name="e"> EventArgs that contains the event data.</param>
        /// <param name="ri">Render Info</param>
        /// <param name="bounds">Rectangle bounds</param>
        public override void DrawBackground(PaintEventArgs e, IRendererInfo ri, Rectangle bounds)
        {
            if (!bounds.IsEmpty)
            {
                // Adding 1 pixels to left and top
                IncreaseBounds(ref bounds);

                BasicRendererInfo bri = ri as BasicRendererInfo;
                if (bri != null)
                {
                    if (!(bri.ThemesEnabled && XPThemes.IsThemedOS && XPThemes.IsThemeActive && XPThemes.IsAppThemed) ||
                      bri.IgnoreThemeBackground)
                    {
                        if (bri.BackgroundColor != BrushInfo.Empty)
                        {
                            BrushPaint.FillRectangle(e.Graphics, bounds, bri.BackgroundColor);
                        }
                    }
                    else
                    {
                        if (bri.Enabled)
                        {
                            if (bri.ThemedControl != null)
                            {
                                bri.ThemedControl.DrawThemeBackground(e.Graphics, 1, 1, new Rectangle(-2, -2, bounds.Width + 4, bounds.Height + 4)); 
                            }
                        }
                        else
                        {
                            using (Brush br = new SolidBrush(SystemColors.Control))
                            {
                                e.Graphics.FillRectangle(br, bounds);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draws hotbackground on graphics of some object
        /// </summary>
        /// <param name="e"> EventArgs that contains the event data.</param>
        /// <param name="ri">Render Info</param>
        /// <param name="bounds">Rectangle bounds</param>
        public override void DrawHotBackground(PaintEventArgs e, IRendererInfo ri, Rectangle bounds)
        {
            if (!bounds.IsEmpty)
            {
                BasicRendererInfo bri = ri as BasicRendererInfo;
                if (null != bri)
                {
                    if (!(bri.ThemesEnabled && XPThemes.IsThemedOS && XPThemes.IsThemeActive && XPThemes.IsAppThemed) || bri.IgnoreThemeBackground)
                    {
                        if (bri.HotBackgroundColor != BrushInfo.Empty)
                        {
                            BrushPaint.FillRectangle(e.Graphics, bounds, bri.HotBackgroundColor);
                        }
                    }
                    else
                    {
                        if (bri.Enabled)
                        {
                            if (bri.ThemedControl != null)
                            {
                                bri.ThemedControl.DrawThemeBackground(e.Graphics, 1, 1, new Rectangle(-2, -2, bounds.Width + 4, bounds.Height + 4));
                            }
                        }
                        else
                        {
                            Brush br = new SolidBrush(SystemColors.Control);
                            e.Graphics.FillRectangle(br, bounds);
                            br.Dispose();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// A stub: no thumbnails!!!
        /// </summary>
        /// <param name="e">Paint context.</param>
        /// <param name="ri">An instance of RendererInfo, which should be used as datasource for painting.</param>
        /// <param name="bounds">Bounds within which we should paint.</param>
        public override void DrawThumbnail(PaintEventArgs e, IRendererInfo ri, Rectangle bounds)
        {
            // No thumbnails by default
        }

        /// <summary>
        /// A stub: no hot thumbnails here!
        /// </summary>
        /// <param name="e">Paint context.</param>
        /// <param name="ri">An instance of RendererInfo, which should be used as datasource for painting.</param>
        /// <param name="bounds">Bounds within which we should paint.</param>
        public override void DrawHotThumbnail(PaintEventArgs e, IRendererInfo ri, Rectangle bounds)
        {
            // No thumbnails by default
        }

        /// <summary>
        /// A stub: no foreground here!
        /// </summary>
        /// <param name="e">Paint context.</param>
        /// <param name="ri">An instance of RendererInfo, which should be used as datasource for painting.</param>
        /// <param name="bounds">Bounds within which we should paint.</param>
        public override void DrawForeground(PaintEventArgs e, IRendererInfo ri, Rectangle bounds)
        {
            // No rendering for foreground by default
        }

        /// <summary>
        /// A stub: no hot foreground here!
        /// </summary>
        /// <param name="e">Paint context.</param>
        /// <param name="ri">An instance of RendererInfo, which should be used as datasource for painting.</param>
        /// <param name="bounds">Bounds within which we should paint.</param>
        public override void DrawHotForeground(PaintEventArgs e, IRendererInfo ri, Rectangle bounds)
        {
            // No rendering for hot foreground by default
        }

        /// <summary>
        /// Gets default value for property.
        /// </summary>
        /// <param name="property">Renderer property</param>
        /// <returns>Returns object </returns>
        public override object GetDefaultValue(RendererProperty property)
        {
            object res = null;

            switch (property)
            {
                case RendererProperty.BackgroundColor:
                    res = m_defaultBackgroundColor;
                    break;
                case RendererProperty.ExpandLine:
                    res = m_defaultExpandLine;
                    break;
                case RendererProperty.ExpandFill:
                    res = m_defaultExpandFill;
                    break;
                case RendererProperty.GripDark:
                    res = m_defaultGripDark;
                    break;
                case RendererProperty.GripLight:
                    res = m_defaultGripLight;
                    break;
                case RendererProperty.HotBackgroundColor:
                    res = m_defaultHotBackgroundColor;
                    break;
                case RendererProperty.HotExpandLine:
                    res = m_defaultHotExpandLine;
                    break;
                case RendererProperty.HotExpandFill:
                    res = m_defaultHotExpandFill;
                    break;
                case RendererProperty.HotGripDark:
                    res = m_defaultHotGripDark;
                    break;
                case RendererProperty.HotGripLight:
                    res = m_defaultHotGripLight;
                    break;
            }

            return res;
        }

        /// <summary>
        /// Updates renderer-specified information from control.
        /// </summary>
        /// <param name="instance">An instance of holding container.</param>
        /// <param name="iri">An instance of RendererInfo, which should be updated.</param>
        /// <returns>Modified RendererInfo instance.</returns>
        public override IRendererInfo UpdateRendererInfo(SplitContainerAdv instance, IRendererInfo iri)
        {
            BasicRendererInfo fri = iri as BasicRendererInfo;

            if (null == fri)
            {
                fri = new BasicRendererInfo();
            }

            if (null != m_rendererInfo && m_bUpdate)
            {
                fri.ConvertSpliContainerToRendererInfo(instance, m_bUseOrientation);
                m_rendererInfo.ConvertSpliContainerToRendererInfo(instance, m_bUseOrientation);
            }

            return fri;
        }
        #endregion

        #region Class constants
        /// <summary>
        /// Padding between grips.
        /// </summary>
        protected int m_nGripPadding = 4;
        #endregion Constants

        #region class initializes\finalizes methods
        /// <summary>
        /// Initializes a new instance of the BasicRenderer class.
        /// </summary>
        protected BasicRenderer()
        {
            m_defaultBackgroundColor = BrushInfo.Empty;
            m_defaultExpandFill = BrushInfo.Empty;
            m_defaultExpandLine = Color.Empty;
            m_defaultGripDark = BrushInfo.Empty;
            m_defaultHotBackgroundColor = BrushInfo.Empty;
            m_defaultHotExpandFill = BrushInfo.Empty;
            m_defaultGripLight = BrushInfo.Empty;
            m_defaultHotGripDark = BrushInfo.Empty;
            m_defaultHotGripLight = BrushInfo.Empty;
            m_defaultHotExpandLine = Color.Empty;
            m_rendererInfo.OrientationChanged += new EventHandler(M_RendererInfo_OrientationChanged);
            SetDefaultSettings();
        }
        #endregion

        #region class helper methods
        
        protected void IncreaseBounds(ref Rectangle bounds)
        {
            bounds.X -= 1;
            bounds.Y -= 1;
            bounds.Width += 1;
            bounds.Height += 1;
        }

        /// <summary>
        /// Draws arrows on a thumbnail.
        /// </summary>
        /// <param name="e">Arguments passed to OnPaint event handler.</param>
        /// <param name="brBackColorBrush">A brush for drawing background.</param>
        /// <param name="penExpandLine">A pen which paints triangles.</param>
        /// <param name="rectThumbnail">Thumbnail rectangle.</param>
        /// <param name="bri">Instance of RendererInfo which is used to retrieve settings from.</param>
        protected void DrawArrows(PaintEventArgs e, BrushInfo brBackColorBrush, Pen penExpandLine, Rectangle rectThumbnail, RendererInfo bri)
        {
            BasicRendererInfo fri = bri as BasicRendererInfo;

            if (null != fri)
            {
                // Making arrows.
                // This code is subject of change.
                CollapsedPanel panelToBeCollapsed = fri.PanelToCollapse;
                CollapsedPanel collapsedPanel = fri.CollapsedPanel;

                Point[] arrTriangleLeft = new Point[3];
                Point[] arrTriangleRight = new Point[3];

                if (fri.Orientation == Orientation.Horizontal)
                {
                    if (panelToBeCollapsed == CollapsedPanel.None)
                    {
                        arrTriangleLeft[0] = new Point(rectThumbnail.Left, rectThumbnail.Top);
                        arrTriangleLeft[1] = new Point(rectThumbnail.Right, rectThumbnail.Top);
                        arrTriangleLeft[2] = new Point((rectThumbnail.Left + rectThumbnail.Width / 2), rectThumbnail.Top + 6);
                        arrTriangleRight[0] = new Point((rectThumbnail.Left + rectThumbnail.Width / 2), rectThumbnail.Bottom - 6);
                        arrTriangleRight[1] = new Point(rectThumbnail.Right, rectThumbnail.Bottom);
                        arrTriangleRight[2] = new Point(rectThumbnail.Left, rectThumbnail.Bottom);
                    }
                    else
                    {
                        if (panelToBeCollapsed != collapsedPanel)
                        {
                            if (panelToBeCollapsed == CollapsedPanel.Panel1)
                            {
                                arrTriangleLeft = MakeLeftArrow(rectThumbnail, arrTriangleLeft, true);
                                arrTriangleRight = MakeLeftArrow(rectThumbnail, arrTriangleRight, false);
                            }
                            else if (panelToBeCollapsed == CollapsedPanel.Panel2)
                            {
                                arrTriangleLeft = MakeRightArrow(rectThumbnail, arrTriangleLeft, true);
                                arrTriangleRight = MakeRightArrow(rectThumbnail, arrTriangleRight, false);
                            }
                        }
                        else
                        {
                            if (panelToBeCollapsed == CollapsedPanel.Panel1)
                            {
                                arrTriangleLeft = MakeRightArrow(rectThumbnail, arrTriangleLeft, true);
                                arrTriangleRight = MakeRightArrow(rectThumbnail, arrTriangleRight, false);
                            }
                            else if (panelToBeCollapsed == CollapsedPanel.Panel2)
                            {
                                arrTriangleLeft = MakeLeftArrow(rectThumbnail, arrTriangleLeft, true);
                                arrTriangleRight = MakeLeftArrow(rectThumbnail, arrTriangleRight, false);
                            }
                        }
                    }
                }
                else
                {
                    if (panelToBeCollapsed == CollapsedPanel.None)
                    {
                        arrTriangleLeft[0] = new Point(rectThumbnail.Left, rectThumbnail.Top);
                        arrTriangleLeft[1] = new Point(rectThumbnail.Left, rectThumbnail.Bottom);
                        arrTriangleLeft[2] = new Point(rectThumbnail.Left + 6, (rectThumbnail.Top + rectThumbnail.Height / 2));
                        arrTriangleRight[0] = new Point(rectThumbnail.Right - 6, (rectThumbnail.Top + rectThumbnail.Height / 2));
                        arrTriangleRight[1] = new Point(rectThumbnail.Right, rectThumbnail.Bottom);
                        arrTriangleRight[2] = new Point(rectThumbnail.Right, rectThumbnail.Top);
                    }
                    else
                    {
                        if (panelToBeCollapsed != collapsedPanel)
                        {
                            if (panelToBeCollapsed == CollapsedPanel.Panel1)
                            {
                                arrTriangleLeft = MakeTopArrow(rectThumbnail, arrTriangleLeft, true);
                                arrTriangleRight = MakeTopArrow(rectThumbnail, arrTriangleRight, false);
                            }
                            else if (panelToBeCollapsed == CollapsedPanel.Panel2)
                            {
                                arrTriangleLeft = MakeBottomArrow(rectThumbnail, arrTriangleLeft, true);
                                arrTriangleRight = MakeBottomArrow(rectThumbnail, arrTriangleRight, false);
                            }
                        }
                        else
                        {
                            if (panelToBeCollapsed == CollapsedPanel.Panel1)
                            {
                                arrTriangleLeft = MakeBottomArrow(rectThumbnail, arrTriangleLeft, true);
                                arrTriangleRight = MakeBottomArrow(rectThumbnail, arrTriangleRight, false);
                            }
                            else if (panelToBeCollapsed == CollapsedPanel.Panel2)
                            {
                                arrTriangleLeft = MakeTopArrow(rectThumbnail, arrTriangleLeft, true);
                                arrTriangleRight = MakeTopArrow(rectThumbnail, arrTriangleRight, false);
                            }
                        }
                    }
                }

                // creating path, using for drawing triangles
                GraphicsPath pathLeftTriangle = new GraphicsPath();
                pathLeftTriangle.AddPolygon(arrTriangleLeft);

                // draws left triangle
                BrushPaint.FillPath(e.Graphics, pathLeftTriangle, brBackColorBrush);
                e.Graphics.DrawPolygon(penExpandLine, arrTriangleLeft);

                // clear path and add righy polygon
                pathLeftTriangle.ClearMarkers();
                pathLeftTriangle.AddPolygon(arrTriangleRight);

                // draws right triangle
                // e.Graphics.FillPolygon( brBackColorBrush, arrTriangleLeft );
                BrushPaint.FillPath(e.Graphics, pathLeftTriangle, brBackColorBrush);
                e.Graphics.DrawPolygon(penExpandLine, arrTriangleRight);

                // dispose system object
                pathLeftTriangle.Dispose();
            }
        }

        /// <summary>
        /// Fills an array of points with coordinates of the right arrow angles.
        /// </summary>
        /// <param name="rectThumbnail">Thumbnail rectangle (a rectangle to draw thumbnail in).</param>
        /// <param name="arrTriangle">Array of points which should be filled up with coordinates.</param>
        /// <param name="isTop">Is this a topmost arrow.</param>
        /// <returns>An array of points, which are coordinates of triangle which represents an arrow.</returns>
        protected Point[] MakeRightArrow(Rectangle rectThumbnail, Point[] arrTriangle, bool isTop)
        {
            if (isTop)
            {
                arrTriangle[0] = new Point(rectThumbnail.Left, rectThumbnail.Top);
                arrTriangle[1] = new Point(rectThumbnail.Right, rectThumbnail.Top + 3);
                arrTriangle[2] = new Point(rectThumbnail.Left, rectThumbnail.Top + 6);
            }
            else
            {
                arrTriangle[0] = new Point(rectThumbnail.Left, rectThumbnail.Bottom - 6);
                arrTriangle[1] = new Point(rectThumbnail.Right, rectThumbnail.Bottom - 3);
                arrTriangle[2] = new Point(rectThumbnail.Left, rectThumbnail.Bottom);
            }
            return arrTriangle;
        }

        /// <summary>
        /// Fills an array of points with coordinates of the left arrow angles.
        /// </summary>
        /// <param name="rectThumbnail">Thumbnail rectangle (a rectangle to draw thumbnail in).</param>
        /// <param name="arrTriangle">Array of points which should be filled up with coordinates.</param>
        /// <param name="isTop">Is this a topmost arrow.</param>
        /// <returns>An array of points, which are coordinates of triangle which represents an arrow.</returns>
        protected Point[] MakeLeftArrow(Rectangle rectThumbnail, Point[] arrTriangle, bool isTop)
        {
            if (isTop)
            {
                arrTriangle[0] = new Point(rectThumbnail.Right, rectThumbnail.Top);
                arrTriangle[1] = new Point(rectThumbnail.Left, rectThumbnail.Top + 3);
                arrTriangle[2] = new Point(rectThumbnail.Right, rectThumbnail.Top + 6);
            }
            else
            {
                arrTriangle[0] = new Point(rectThumbnail.Right, rectThumbnail.Bottom - 6);
                arrTriangle[1] = new Point(rectThumbnail.Left, rectThumbnail.Bottom - 3);
                arrTriangle[2] = new Point(rectThumbnail.Right, rectThumbnail.Bottom);
            }
            return arrTriangle;
        }

        /// <summary>
        /// Fills an array of points with coordinates of the top arrow angles.
        /// </summary>
        /// <param name="rectThumbnail">Thumbnail rectangle (a rectangle to draw thumbnail in).</param>
        /// <param name="arrTriangle">Array of points which should be filled up with coordinates.</param>
        /// <param name="isTop">Is this a topmost arrow.</param>
        /// <returns>An array of points, which are coordinates of triangle which represents an arrow.</returns>
        protected Point[] MakeTopArrow(Rectangle rectThumbnail, Point[] arrTriangle, bool isTop)
        {
            if (isTop)
            {
                arrTriangle[0] = new Point(rectThumbnail.X, rectThumbnail.Bottom);
                arrTriangle[1] = new Point(rectThumbnail.X + 3, rectThumbnail.Y);
                arrTriangle[2] = new Point(rectThumbnail.X + 7, rectThumbnail.Bottom);
            }
            else
            {
                arrTriangle[0] = new Point(rectThumbnail.Right - 7, rectThumbnail.Bottom);
                arrTriangle[1] = new Point(rectThumbnail.Right - 3, rectThumbnail.Y);
                arrTriangle[2] = new Point(rectThumbnail.Right, rectThumbnail.Bottom);
            }
            return arrTriangle;
        }

        /// <summary>
        /// Fills an array of points with coordinates of the bottom arrow angles.
        /// </summary>
        /// <param name="rectThumbnail">Thumbnail rectangle (a rectangle to draw thumbnail in).</param>
        /// <param name="arrTriangle">Array of points which should be filled up with coordinates.</param>
        /// <param name="isTop">Is this a topmost arrow.</param>
        /// <returns>An array of points, which are coordinates of triangle which represents an arrow.</returns>
        protected Point[] MakeBottomArrow(Rectangle rectThumbnail, Point[] arrTriangle, bool isTop)
        {
            if (isTop)
            {
                arrTriangle[0] = new Point(rectThumbnail.X, rectThumbnail.Top);
                arrTriangle[1] = new Point(rectThumbnail.X + 3, rectThumbnail.Bottom);
                arrTriangle[2] = new Point(rectThumbnail.X + 7, rectThumbnail.Top);
            }
            else
            {
                arrTriangle[0] = new Point(rectThumbnail.Right - 7, rectThumbnail.Top);
                arrTriangle[1] = new Point(rectThumbnail.Right - 3, rectThumbnail.Bottom);
                arrTriangle[2] = new Point(rectThumbnail.Right, rectThumbnail.Top);
            }
            return arrTriangle;
        }

        #endregion

        #region class public properties
        /// <summary>
        /// Sets renderer info to default value
        /// </summary>
        public void SetDefaultSettings()
        {
            m_rendererInfo.ExpandFill = m_defaultExpandFill;
            m_rendererInfo.HotExpandFill = m_defaultHotExpandFill;
            m_rendererInfo.ExpandLine = m_defaultExpandLine;
            m_rendererInfo.HotExpandLine = m_defaultHotExpandLine;
            m_rendererInfo.GripDark = m_defaultGripDark;
            m_rendererInfo.HotGripDark = m_defaultHotGripDark;
            m_rendererInfo.GripLight = m_defaultGripLight;
            m_rendererInfo.HotGripLight = m_defaultHotGripLight;
            m_rendererInfo.BackgroundColor = m_defaultBackgroundColor;
            m_rendererInfo.HotBackgroundColor = m_defaultHotBackgroundColor;
        }
        #endregion

        #region class public methods
        /// <summary>
        /// Retrieves an instance of DefaultRenderer
        /// </summary>
        /// <returns>"new DefaultRenderer()"</returns>
        public static BasicRenderer GetInstance()
        {
            return new BasicRenderer();
        }
        #endregion

        #region class event handlers

        private void M_RendererInfo_OrientationChanged(object sender, EventArgs e)
        {
            if (m_bUseOrientation)
            {
                m_defaultBackgroundColor = new BrushInfo((GradientStyle)GradientStyle.Parse(typeof(GradientStyle), m_rendererInfo.Orientation.ToString()), m_defaultBackgroundColor.ForeColor, m_defaultBackgroundColor.BackColor);
                m_defaultHotBackgroundColor = new BrushInfo((GradientStyle)GradientStyle.Parse(typeof(GradientStyle), m_rendererInfo.Orientation.ToString()), m_defaultHotBackgroundColor.ForeColor, m_defaultHotBackgroundColor.BackColor);
            }
        }
        #endregion
    }

    public class BasicRendererInfo
      : RendererInfo
    {
        #region Class members
        /// <summary>
        /// Size of thumbnail.
        /// </summary>
        private Size m_szThumbnailSize = new Size(70, 4);

        /// <summary>
        /// Default for this theme value for brush to fill thumbnail arrows.
        /// </summary>
        private BrushInfo m_brushExpandFill;

        /// <summary>
        /// Default for this theme value for brush to fill thumbnail arrows while under mouse cursor.
        /// </summary>
        private BrushInfo m_brushHotExpandFill;

        /// <summary>
        /// Default for this theme value for brush to draw thumbnail arrows.
        /// </summary>
        private Color m_colorExpandLine;

        /// <summary>
        /// Default for this theme value for brush to draw thumbnail arrows while under mouse cursor.
        /// </summary>
        private Color m_colorHotExpandLine;

        /// <summary>
        /// Default for this theme value for brush to fill thumbnail grips.
        /// </summary>
        private BrushInfo m_brushGripDark;

        /// <summary>
        /// Default for this theme value for brush to fill thumbnail grips while under mouse cursor.
        /// </summary>
        private BrushInfo m_brushHotGripDark;

        /// <summary>
        /// Default for this theme value for brush to fill thumbnail grip's shadow.
        /// </summary>
        private BrushInfo m_brushGripLight;

        /// <summary>
        /// Default for this theme value for brush to fill thumbnail grip's shadow while under mouse cursor.
        /// </summary>
        private BrushInfo m_brushHotGripLight;

        /// <summary>
        /// Default for this theme value for background brush while under mouse cursor.
        /// </summary>
        private BrushInfo m_bgHotBrush;

        /// <summary>
        /// Panel which should be collapsed by some event.
        /// </summary>
        private CollapsedPanel m_panelToBeCollapsed = CollapsedPanel.None;

        /// <summary>
        /// Panel which actually is collapsed.
        /// </summary>
        private CollapsedPanel m_collapsedPanel = CollapsedPanel.None;
        #endregion

        #region Class Initialize\Finalize methods
        /// <summary>
        /// Initializes a new instance of the BasicRendererInfo class.
        /// </summary>
        public BasicRendererInfo()
        {
            // default constructor 
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets thumbnail size.
        /// </summary>
        public Size ThumbnailSize
        {
            get
            {
                if (m_bOrientationChanged)
                {
                    Size szTmp = m_szThumbnailSize;
                    m_szThumbnailSize.Width = szTmp.Height;
                    m_szThumbnailSize.Height = szTmp.Width;

                    m_bOrientationChanged = false;
                }
                return m_szThumbnailSize;
            }
            set
            {
                if (m_szThumbnailSize != value)
                {
                    m_szThumbnailSize = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets brush for filling thumbnail arrows.
        /// </summary>
        public BrushInfo ExpandFill
        {
            get
            {
                return m_brushExpandFill;
            }
            set
            {
                if (m_brushExpandFill != value)
                {
                    m_brushExpandFill = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets brush for filling thumbnail arrows, while under mouse cursor.
        /// </summary>
        public BrushInfo HotExpandFill
        {
            get
            {
                return m_brushHotExpandFill;
            }
            set
            {
                if (m_brushHotExpandFill != value)
                {
                    m_brushHotExpandFill = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets pen color for drawing thumbnail arrows.
        /// </summary>
        public Color ExpandLine
        {
            get
            {
                return m_colorExpandLine;
            }
            set
            {
                if (m_colorExpandLine != value)
                {
                    m_colorExpandLine = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets pen color for drawing thumbnail arrows, while under mouse cursor.
        /// </summary>
        public Color HotExpandLine
        {
            get
            {
                return m_colorHotExpandLine;
            }
            set
            {
                if (m_colorHotExpandLine != value)
                {
                    m_colorHotExpandLine = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets brush for drawing a shadow around grip in thumbnail, if any.
        /// </summary>
        public BrushInfo GripDark
        {
            get
            {
                return m_brushGripDark;
            }
            set
            {
                if (m_brushGripDark != value)
                {
                    m_brushGripDark = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets brush for drawing a shadow around grip in thumbnail, if any, while under mouse cursor.
        /// </summary>
        public BrushInfo HotGripDark
        {
            get
            {
                return m_brushHotGripDark;
            }
            set
            {
                if (m_brushHotGripDark != value)
                {
                    m_brushHotGripDark = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets brush for drawing a grip in thumbnail, if any.
        /// </summary>
        public BrushInfo GripLight
        {
            get
            {
                return m_brushGripLight;
            }
            set
            {
                if (m_brushGripLight != value)
                {
                    m_brushGripLight = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets brush for drawing a grip in thumbnail, if any, while under mouse cursor.
        /// </summary>
        public BrushInfo HotGripLight
        {
            get
            {
                return m_brushHotGripLight;
            }
            set
            {
                if (m_brushHotGripLight != value)
                {
                    m_brushHotGripLight = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the  hot (under mouse cursor) background color, gradient and other styles can be set through 
        /// this property.
        /// </summary>
        /// <remarks>
        /// The SplitContainerAdv control provides this property to enable specialized
        /// custom gradient backgrounds.
        /// </remarks>
        public BrushInfo HotBackgroundColor
        {
            get
            {
                return m_bgHotBrush;
            }

            set
            {
                if (m_bgHotBrush != value)
                {
                    m_bgHotBrush = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets panel which is collapsed now.
        /// </summary>
        public CollapsedPanel CollapsedPanel
        {
            get
            {
                return m_collapsedPanel;
            }
            set
            {
                if (value != m_collapsedPanel)
                {
                    m_collapsedPanel = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets Panel which should be collapsed by some event. 
        /// </summary>
        public CollapsedPanel PanelToCollapse
        {
            get
            {
                return m_panelToBeCollapsed;
            }
            set
            {
                if (value != m_panelToBeCollapsed)
                {
                    m_panelToBeCollapsed = value;
                }
            }
        }
        #endregion

        #region class public methods
        /// <summary>
        /// Gets renderer info data from SplitContainerAdv object
        /// </summary>
        /// <param name="instance"> SplitContianerAdv instance</param>
        /// <param name="isOrientation">Orientation value</param>
        public void ConvertSpliContainerToRendererInfo(SplitContainerAdv instance, bool isOrientation)
        {
            if (null != instance)
            {
                DrawState = instance.DrawState;
                Orientation = instance.Orientation;
                ExpandFill = instance.ExpandFill;
                HotExpandFill = instance.HotExpandFill;
                ExpandLine = instance.ExpandLine;
                HotExpandLine = instance.HotExpandLine;
                GripDark = instance.GripDark;
                HotGripDark = instance.HotGripDark;
                GripLight = instance.GripLight;
                HotGripLight = instance.HotGripLight;
                CollapsedPanel = instance.CollapsedPanel;
                PanelToCollapse = instance.PanelToBeCollapsed;

                if (isOrientation)
                {
                    BackgroundColor = new BrushInfo((GradientStyle)GradientStyle.Parse(typeof(GradientStyle), instance.Orientation.ToString()), instance.BackgroundColor.ForeColor, instance.BackgroundColor.BackColor);
                    HotBackgroundColor = new BrushInfo((GradientStyle)GradientStyle.Parse(typeof(GradientStyle), instance.Orientation.ToString()), instance.HotBackgroundColor.ForeColor, instance.HotBackgroundColor.BackColor);
                }
                else
                {
                    BackgroundColor = instance.BackgroundColor;
                    HotBackgroundColor = instance.HotBackgroundColor;
                }
            }
        }

        /// <summary>
        /// Sets base info property for SpliContainerAdv
        /// </summary>
        /// <param name="container">Split Container</param>
        /// <param name="br">Basic renderer</param>
        /// <param name="isOrientation">Orientation value</param>
        /// <param name="initMode">bool value for initmode</param>
        public void SetSplitContainerByInfo(SplitContainerAdv container, BasicRenderer br, bool isOrientation, bool initMode)
        {
            if (null != container)
            {
                if (initMode)
                {
                    SetSplitContainerByInfoWhenInitialization(container, br, isOrientation);
                }
                else
                {
                    if (isOrientation)
                    {
                        container.BackgroundColor = new BrushInfo((GradientStyle)GradientStyle.Parse(typeof(GradientStyle), container.Orientation.ToString()), m_bgBrush.ForeColor, m_bgBrush.BackColor);
                        container.HotBackgroundColor = new BrushInfo((GradientStyle)GradientStyle.Parse(typeof(GradientStyle), container.Orientation.ToString()), m_bgHotBrush.ForeColor, m_bgHotBrush.BackColor);
                    }
                    else
                    {
                        container.BackgroundColor = m_bgBrush;
                        container.HotBackgroundColor = m_bgHotBrush;
                    }

                    container.ExpandFill = m_brushExpandFill;
                    container.ExpandLine = m_colorExpandLine;
                    container.GripDark = m_brushGripDark;
                    container.GripLight = m_brushGripLight;
                    container.HotExpandFill = m_brushHotExpandFill;
                    container.HotExpandLine = m_colorHotExpandLine;
                    container.HotGripDark = m_brushHotGripDark;
                    container.HotGripLight = m_brushHotGripLight;
                }
            }
        }

        /// <summary>
        /// Sets base info property for SpliContainerAdv
        /// </summary>
        /// <param name="container">SplitContainerAdv Container</param>
        /// <param name="br"> Basic Renderer</param>
        /// <param name="isOrientation">Orientation value</param>
        private void SetSplitContainerByInfoWhenInitialization(SplitContainerAdv container, BasicRenderer br, bool isOrientation)
        {
            if (null != container)
            {
                if (BrushInfo.Empty.Equals(container.BackgroundColor))
                {
                    container.BackgroundColor = br.DefaultBackgroundColor;
                }
                if (BrushInfo.Empty.Equals(container.HotBackgroundColor))
                {
                    container.HotBackgroundColor = br.DefaultHotBackgroundColor;
                }

                if (BrushInfo.Empty.Equals(container.ExpandFill))
                {
                    container.ExpandFill = br.DefaultExpandFill;
                }

                if (Color.Empty.Equals(container.ExpandLine))
                {
                    container.ExpandLine = br.DefaultExpandLine;
                }

                if (BrushInfo.Empty.Equals(container.GripDark))
                {
                    container.GripDark = br.DefaultGripDark;
                }

                if (BrushInfo.Empty.Equals(container.GripLight))
                {
                    container.GripLight = br.DefaultGripLight;
                }

                if (Color.Empty.Equals(container.HotExpandLine))
                {
                    container.HotExpandLine = br.DefaultHotExpandLine;
                }

                if (BrushInfo.Empty.Equals(container.HotExpandFill))
                {
                    container.HotExpandFill = br.DefaultHotExpandFill;
                }

                if (BrushInfo.Empty.Equals(container.HotGripDark))
                {
                    container.HotGripDark = br.DefaultHotGripDark;
                }

                if (BrushInfo.Empty.Equals(container.HotGripLight))
                {
                    container.HotGripLight = br.DefaultHotGripLight;
                }
            }
        }
        #endregion
    }
}
