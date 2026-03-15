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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Syncfusion.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms.Chart
{
    #region Enums

    /// <summary>
    /// The MarkerStyle Enumerator.
    /// </summary>
    public enum MarkerStyle
    {
        /// <summary>
        /// Marker Style will be of Rectangle.
        /// </summary>
        Rectangle,

        /// <summary>
        /// Marker Style will be of Ellipse.
        /// </summary>
        Ellipse,

        /// <summary>
        /// Marker Style will be of SmoothRectangle.
        /// </summary>
        SmoothRectangle
    }
    #endregion

    /// <summary>
    /// Defines the fancy tooltip rendered on data points.
    /// </summary>
    [ToolboxItem(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class ChartFancyToolTip : IDisposable
    {
        #region Constants
        private const float COEF_RECT_TO_ELLIPSE = 1.4f;
        private const float REAL_PI = 180;
        private const float REAL_PI_2 = 90;
        private const float REAL_2_PI = 360;
        private const float REAL_3_PI_4 = 270;
        #endregion

        #region Members
        private Color m_symbolColor;

        private Control m_parent = null;
        /// <summary>
        /// Store top level form to show toolTop.
        /// </summary>
        private PopupHost m_hintWindow;

        private bool m_isShowed = false;

        /// <summary>
        /// The ChartFancyToolTipInfo instance.
        /// </summary>
        private ChartFancyToolTipInfo m_info = null;

        /// <summary>
        /// Store target point.
        /// </summary>
        private PointF m_target;

        /// <summary>
        /// Store shift.
        /// </summary>
        private int m_shift = 4;

        /// <summary>
        /// Store value indicates that draw symbol.
        /// </summary>
        private bool m_drawSymbol = true;

        /// <summary>
        /// Store value indicates that ToolTip is visible.
        /// </summary>
        private bool m_visible = true;

        /// <summary>
        /// Store client rectangle.
        /// </summary>
        private Rectangle m_clientRectangle = Rectangle.Empty;

        /// <summary>
        /// Store text property.
        /// </summary>
        private string m_text;

        private ChartFancyToolTipController m_controller;

        private Timer m_timer = new Timer();
        #endregion

        #region Properties
        /// <summary>
        /// Gets the info.
        /// </summary>
        /// <value>The info.</value>
        public ChartFancyToolTipInfo Info
        {
            get
            {
                return m_info;
            }
        }

        /// <summary>
        /// Gets or sets the shift.
        /// </summary>
        public int Shift
        {
            get
            {
                return m_shift;
            }

            set
            {
                if (m_shift != value)
                {
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ChartFancyToolTip"/> is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        public bool Visible
        {
            get
            {
                return m_visible;
            }
            set
            {
                if (m_visible != value)
                {
                    m_visible = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        public Point Location
        {
            get
            {
                return m_clientRectangle.Location;
            }

            set
            {
                if (m_clientRectangle.Location != value)
                {
                    m_clientRectangle.Location = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public int Width
        {
            get
            {
                return m_clientRectangle.Width;
            }

            set
            {
                if (m_clientRectangle.Width != value)
                {
                    m_clientRectangle.Width = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public int Height
        {
            get
            {
                return m_clientRectangle.Height;
            }

            set
            {
                if (m_clientRectangle.Height != value)
                {
                    m_clientRectangle.Height = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public Size Size
        {
            get
            {
                return m_clientRectangle.Size;
            }

            set
            {
                if (m_clientRectangle.Size != value)
                {
                    m_clientRectangle.Size = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text
        {
            get
            {
                return m_text;
            }

            set
            {
                if (m_text != value)
                {
                    m_text = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the symbol.
        /// </summary>
        /// <value>The color of the symbol.</value>
        public Color SymbolColor
        {
            get { return m_symbolColor; }

            set { m_symbolColor = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartFancyToolTip"/> class.
        /// </summary>
        public ChartFancyToolTip(ChartFancyToolTipInfo info)
        {
            m_info = info;
            m_info.Changed += new EventHandler(OnInfoChanged);

            m_timer.Tick += new EventHandler(OnTimerTick);
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            m_timer.Stop();

            if (m_hintWindow != null)
            {
                this.AutoSize(m_parent.ClientRectangle);
                Rectangle bounds = Rectangle.Inflate(m_clientRectangle, (int)m_info.Border.Width, (int)m_info.Border.Width);

                m_hintWindow.Location = m_parent.PointToScreen(bounds.Location);
                m_hintWindow.Size = bounds.Size;

                if (!m_isShowed)
                {
                    m_hintWindow.ShowWindowTopMost();
                    m_isShowed = true;
                }
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Inits by the specified parent.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="controller">The controller.</param>
        internal void Init(Control parent, ChartFancyToolTipController controller)
        {
            m_parent = parent;
            m_parent.FindForm().Deactivate += new EventHandler(this.OnParentFormDeactivate);

            m_hintWindow = new PopupHost();
            m_hintWindow.Paint += new PaintEventHandler(this.OnToolTipPaint);
            m_hintWindow.MouseMove += new MouseEventHandler(OnHintWindowMouseMove);

            m_controller = controller;
        }

        /// <summary>
        /// Shows this tooltip by specified parent.
        /// </summary>
        internal void Show(PointF target)
        {
            if (m_target != target)
            {
                this.Hide();

                m_target = target;
                m_timer.Start();
            }
        }

        /// <summary>
        /// Hides this tooltip.
        /// </summary>
        internal void Hide()
        {
            m_target = Point.Empty;

            if (m_hintWindow != null)
            {
                if (m_isShowed)
                {
                    m_hintWindow.Hide();
                    m_isShowed = false;
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            if (m_parent != null)
            {
                Form parentForm = m_parent.FindForm();

                if (parentForm != null)
                {
                    parentForm.Deactivate -= new EventHandler(this.OnParentFormDeactivate);
                }

                m_parent = null;
            }

            if (m_hintWindow != null)
            {
                m_hintWindow.MouseMove -= new MouseEventHandler(OnHintWindowMouseMove);
                m_hintWindow.Dispose();
                m_hintWindow = null;
            }

            if (m_timer != null)
            {
                m_timer.Dispose();
                m_timer = null;
            }
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Called when mouse is moved by hint window.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void OnHintWindowMouseMove(object sender, MouseEventArgs e)
        {
            Point offset = m_parent.PointToClient(m_hintWindow.Location);
            m_controller.OnMouseMove(this, new MouseEventArgs(e.Button, e.Clicks, offset.X + e.X, offset.Y + e.Y, e.Delta));
        }

        /// <summary>
        /// Invalidates this instance.
        /// </summary>
        private void Invalidate()
        {
            if (m_hintWindow != null)
            {
                m_hintWindow.Invalidate();
            }
        }

        /// <summary>
        /// Called when [tool tip paint].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        private void OnToolTipPaint(object sender, PaintEventArgs e)
        {
            if (this.m_target !=PointF.Empty)
            {
                Graphics g = e.Graphics;

                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Transform = new Matrix(1, 0, 0, 1, m_info.Border.Width / 2, m_info.Border.Width / 2);

                GraphicsPath gp = this.GetPath(Width, Height);
                GraphicsPath gpSymbol = this.GetSymbolPath();

                using (SolidBrush brush = new SolidBrush(m_info.BackColor))
                {
                    if (gpSymbol != null)
                    {
                        g.FillPath(brush, gpSymbol);
                    }

                    g.FillPath(brush, gp);
                }

                using (Pen pen = m_info.Border.Pen.Clone() as Pen)
                {
                    pen.LineJoin = LineJoin.Round;
                    g.DrawPath(pen, gp);

                    if (gpSymbol != null)
                    {
                        g.DrawPath(pen, gpSymbol);
                    }
                }

                this.DrawText(g, GetRectangleF());
            }
        }

        /// <summary>
        /// Called when parent form deactivated.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnParentFormDeactivate(object sender, EventArgs e)
        {
            this.Hide();
        }

        /// <summary>
        /// Gets the GraphicsPath of current ToolTip.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>GraphicsPath of the current ToolTip.</returns>
        public GraphicsPath GetPath(float width, float height)
        {
            GraphicsPath gp = null;

            switch (m_info.Style)
            {
                case MarkerStyle.Rectangle:
                    gp = GetPathRectangle(width, height);
                    break;
                case MarkerStyle.Ellipse:
                    gp = GetPathEllipse(width, height);
                    break;
                case MarkerStyle.SmoothRectangle:
                    gp = GetPathSmoothRectangle(width, height);
                    break;
            }

            return gp;
        }

        /// <summary>
        /// Gets rectangle to draw text.
        /// </summary>
        /// <returns>Rectangle to draw text.</returns>
        private RectangleF GetRectangleF()
        {
            RectangleF rect = RectangleF.Empty;

            switch (m_info.Alignment)
            {
                case TabAlignment.Right:
                    rect = new RectangleF(0, 0, Width - m_info.ToTarget, Height);
                    break;
                case TabAlignment.Left:
                    rect = new RectangleF(m_info.ToTarget, 0, Width - m_info.ToTarget, Height);
                    break;
                case TabAlignment.Top:
                    rect = new RectangleF(0, m_info.ToTarget, Width, Height - m_info.ToTarget);
                    break;
                case TabAlignment.Bottom:
                    rect = new RectangleF(0, 0, Width, Height - m_info.ToTarget);
                    break;
            }

            return rect;
        }

        /// <summary>
        /// Gets the path rectangle.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>Rectangle GraphicsPath of the current ToolTip.</returns>
        private GraphicsPath GetPathRectangle(float width, float height)
        {
            GraphicsPath gp = new GraphicsPath();
            float heightSub = height - m_info.ToTarget;
            float widthSub = width - m_info.ToTarget;
            float sin = (float)(m_info.ToTarget * Math.Sin(GetRadialByReal(m_info.Angle)));
            PointF target = new PointF(m_target.X - Location.X, m_target.Y - Location.Y);
            float toTarget = m_info.ToTarget;

            switch (m_info.Alignment)
            {
                case TabAlignment.Right:
                    sin = sin < height ? sin * 0.5f : height * 0.5f;
                    gp.AddPolygon(new PointF[]{
                                       new PointF( 0, 0 ),
                                       new PointF( widthSub, 0 ),
                                       new PointF( widthSub, height/2 - sin ),
                                       target,
                                       new PointF( widthSub, height/2 + sin ),
                                       new PointF( widthSub, height ),
                                       new PointF( 0, height )
                                     });
                    break;
                case TabAlignment.Left:
                    sin = sin < height ? sin * 0.5f : height * 0.5f;
                    gp.AddPolygon(new PointF[]{
                                       new PointF( m_info.ToTarget, 0 ),
                                       new PointF( width, 0 ),
                                       new PointF( width, height ),
                                       new PointF( m_info.ToTarget, height ),
                                       new PointF( m_info.ToTarget, height/2 + sin ),
                                       target,
                                       new PointF( m_info.ToTarget, height/2 - sin )
                                     });
                    break;
                case TabAlignment.Top:
                    sin = sin < width ? sin * 0.5f : width * 0.5f;
                    gp.AddPolygon(new PointF[]{
                                       new PointF( 0, m_info.ToTarget ),
                                       new PointF( width/2 - sin, m_info.ToTarget ),
                                       target,
                                       new PointF( width/2 + sin, m_info.ToTarget ),
                                       new PointF( width, m_info.ToTarget ),
                                       new PointF( width, height ),
                                       new PointF( 0, height )
                                     });
                    break;
                case TabAlignment.Bottom:
                    sin = sin < width ? sin * 0.5f : width * 0.5f;
                    gp.AddPolygon(new PointF[]{
                                       new PointF( 0, 0 ),
                                       new PointF( width, 0 ),
                                       new PointF( width, heightSub ),
                                       new PointF( width/2 + sin, heightSub ),
                                       target,
                                       new PointF( width/2 - sin, heightSub ),
                                       new PointF( 0, heightSub )
                                     });
                    break;
            }

            return gp;
        }

        /// <summary>
        /// Gets the path smooth rectangle.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>Smooth rectangle GraphicsPath of the current ToolTip.</returns>
        private GraphicsPath GetPathSmoothRectangle(float width, float height)
        {
            GraphicsPath gp = new GraphicsPath();
            float heightSub = height - m_info.ToTarget;
            float widthSub = width - m_info.ToTarget;
            float sin = (float)(m_info.ToTarget * Math.Sin(GetRadialByReal(m_info.Angle)));
            PointF target = new PointF(m_target.X - Location.X, m_target.Y - Location.Y);

            switch (m_info.Alignment)
            {
                case TabAlignment.Right:

                    gp.AddArc(widthSub - height / 2, 0, height / 2, height, m_info.Angle, REAL_PI_2 - m_info.Angle);
                    gp.AddLine(widthSub - height / 2, height, height / 2, height);
                    gp.AddArc(0, 0, height / 2, height, REAL_PI_2, REAL_PI);
                    gp.AddLine(height / 2, 0, widthSub - height / 2, 0);
                    gp.AddArc(widthSub - height / 2, 0, height / 2, height, REAL_3_PI_4, REAL_PI_2 - m_info.Angle);
                    gp.AddLine(gp.GetLastPoint(), target);
                    gp.CloseFigure();

                    break;
                case TabAlignment.Left:

                    gp.AddArc(m_info.ToTarget, 0, height / 2, height, REAL_PI + m_info.Angle, REAL_PI_2 - m_info.Angle);
                    gp.AddLine(m_info.ToTarget + height / 2, 0, width - height / 2, 0);
                    gp.AddArc(width - height / 2, 0, height / 2, height, REAL_3_PI_4, REAL_PI);
                    gp.AddLine(width - height / 2, height, m_info.ToTarget + height / 2, height);
                    gp.AddArc(m_info.ToTarget, 0, height / 2, height, REAL_PI_2, REAL_PI_2 - m_info.Angle);
                    gp.AddLine(gp.GetLastPoint(), target);
                    gp.CloseFigure();

                    break;
                case TabAlignment.Top:
                    sin = sin < width ? sin * 0.5f : width * 0.5f;

                    gp.AddArc(0, m_info.ToTarget, heightSub / 2, heightSub, REAL_3_PI_4, -REAL_PI);
                    gp.AddLine(heightSub / 2, height, width - heightSub / 2, height);
                    gp.AddArc(width - heightSub / 2, m_info.ToTarget, heightSub / 2, heightSub, REAL_PI_2, -REAL_PI);
                    gp.AddLine(width - heightSub / 2, m_info.ToTarget, width / 2 + sin, m_info.ToTarget);
                    gp.AddLine(gp.GetLastPoint(), target);
                    gp.AddLine(width / 2 - sin, m_info.ToTarget, heightSub / 2, m_info.ToTarget);
                    gp.CloseFigure();

                    break;
                case TabAlignment.Bottom:
                    sin = sin < width ? sin * 0.5f : width * 0.5f;

                    gp.AddArc(width - heightSub / 2, 0, heightSub / 2, heightSub, REAL_PI_2, -REAL_PI);
                    gp.AddLine(width - heightSub / 2, 0, heightSub / 2, 0);
                    gp.AddArc(0, 0, heightSub / 2, heightSub, REAL_3_PI_4, -REAL_PI);
                    gp.AddLine(heightSub / 2, heightSub, width / 2 - sin, heightSub);
                    gp.AddLine(gp.GetLastPoint(), target);
                    gp.AddLine(width / 2 + sin, heightSub, width - heightSub / 2, heightSub);
                    gp.CloseFigure();

                    break;
            }

            return gp;
        }

        /// <summary>
        /// Gets the path ellipse.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>Ellipse GraphicsPath of the current ToolTip.</returns>
        private GraphicsPath GetPathEllipse(float width, float height)
        {
            GraphicsPath gp = new GraphicsPath();
            float heightSub = height - m_info.ToTarget;
            float widthSub = width - m_info.ToTarget;
            PointF target = new PointF(m_target.X - Location.X, m_target.Y - Location.Y);

            switch (m_info.Alignment)
            {
                case TabAlignment.Right:
                    AddSector(gp, new RectangleF(0, 0, widthSub, height), (float)(m_info.Angle / 2), (float)(REAL_2_PI - m_info.Angle));
                    gp.AddLine(gp.GetLastPoint(), target);
                    gp.CloseFigure();
                    break;

                case TabAlignment.Left:
                    AddSector(gp, new RectangleF(m_info.ToTarget, 0, widthSub, height), (float)(REAL_PI + m_info.Angle / 2), (float)(REAL_2_PI - m_info.Angle));
                    gp.AddLine(gp.GetLastPoint(), target);
                    gp.CloseFigure();
                    break;

                case TabAlignment.Top:
                    AddSector(gp, new RectangleF(0, m_info.ToTarget, width, heightSub), (float)(REAL_3_PI_4 + m_info.Angle / 2), (float)(REAL_2_PI - m_info.Angle));
                    gp.AddLine(gp.GetLastPoint(), target);
                    gp.CloseFigure();
                    break;

                case TabAlignment.Bottom:
                    AddSector(gp, new RectangleF(0, 0, width, heightSub), (float)(REAL_PI_2 + m_info.Angle / 2), (float)(REAL_2_PI - m_info.Angle));
                    gp.AddLine(gp.GetLastPoint(), target);
                    gp.CloseFigure();
                    break;
            }

            return gp;
        }

        /// <summary>
        /// Adds the sector.
        /// </summary>
        /// <param name="gp">The gp.</param>
        /// <param name="rect">The rect.</param>
        /// <param name="start">The start.</param>
        /// <param name="angle">The angle.</param>
        private void AddSector(GraphicsPath gp, RectangleF rect, float start, float angle)
        {
            GraphicsPath arc = new GraphicsPath();
            PointF[] mtrPts = new PointF[]{ rect.Location,
          new PointF(rect.Right, rect.Top), new PointF(rect.Left, rect.Bottom)};

            if (rect.Width < rect.Height)
            {
                arc.AddArc(0, 0, rect.Width, rect.Width, start, angle);
                arc.Transform(new Matrix(new RectangleF(0, 0, rect.Width, rect.Width), mtrPts));
            }
            else
            {
                arc.AddArc(0, 0, rect.Height, rect.Height, start, angle);
                arc.Transform(new Matrix(new RectangleF(0, 0, rect.Height, rect.Height), mtrPts));
            }

            gp.AddPath(arc, true);
        }

        /// <summary>
        /// Recalculates size and location
        /// </summary>
        /// <param name="rect">The rectangle to check bounds.</param>
        public void AutoSize(Rectangle rect)
        {
            Point loc = Point.Empty;
            Size textSize = GetPrimarySize();
            m_info.Alignment = this.CalcAlignment(m_info.Alignment, rect);

            switch (m_info.Alignment)
            {
                case TabAlignment.Right:
                    Size = new SizeF(textSize.Width + 2 * m_info.Spacing + m_info.ToTarget + m_info.SymbolSize.Width / 2,
                      textSize.Height + 2 * m_info.Spacing).ToSize();
                    loc = new Point((int)(m_target.X - Width + m_info.SymbolSize.Width / 2),
                      (int)(m_target.Y - Height / 2) + m_shift);
                    break;

                case TabAlignment.Left:
                    Size = new SizeF(textSize.Width + 2 * m_info.Spacing + m_info.ToTarget + m_info.SymbolSize.Width / 2,
                      textSize.Height + 2 * m_info.Spacing).ToSize();
                    loc = new Point((int)(m_target.X - m_info.SymbolSize.Width / 2),
                      (int)(m_target.Y - Height / 2) + m_shift);
                    break;

                case TabAlignment.Top:
                    Size = new SizeF(textSize.Width + 2 * m_info.Spacing,
                      textSize.Height + 2 * m_info.Spacing + m_info.ToTarget + m_info.SymbolSize.Height / 2).ToSize();
                    loc = new Point((int)(m_target.X - Width / 2) + m_shift,
                      (int)(m_target.Y - m_info.SymbolSize.Height / 2));
                    break;

                case TabAlignment.Bottom:
                    Size = new SizeF(textSize.Width + 2 * m_info.Spacing,
                      textSize.Height + 2 * m_info.Spacing + m_info.ToTarget + m_info.SymbolSize.Height / 2).ToSize();
                    loc = new Point((int)(m_target.X - Width / 2) + m_shift,
                      (int)(m_target.Y - Height + m_info.SymbolSize.Height / 2));
                    break;
            }

            Location = loc;
        }

        /// <summary>
        /// Gets the  primary size.
        /// </summary>
        /// <returns>Primary size of hint.</returns>
        private Size GetPrimarySize()
        {
            Bitmap bmp = new Bitmap(1, 1);
            Graphics g = Graphics.FromImage(bmp);
            Size result = g.MeasureString(Text, m_info.Font).ToSize();

            if (result.Height < m_info.SymbolSize.Height)
            {
                result.Height = m_info.SymbolSize.Height;
            }

            if (m_info.Style == MarkerStyle.Ellipse)
            {
                result = new Size((int)(COEF_RECT_TO_ELLIPSE * result.Width),
                  (int)(COEF_RECT_TO_ELLIPSE * result.Height));
            }
            else if (m_info.Style == MarkerStyle.SmoothRectangle)
            {
                result.Width += result.Height;
            }

            if (m_drawSymbol)
            {
                result.Width += result.Height;
            }

            g.Dispose();
            bmp.Dispose();

            return result;
        }

        /// <summary>
        /// Converts degrees to radial.
        /// </summary>
        /// <param name="angle">The angle to convert.</param>
        /// <returns>Radial representation of the angle.</returns>
        private double GetRadialByReal(float angle)
        {
            return angle * (Math.PI / REAL_PI);
        }

        /// <summary>
        /// Gets the symbol path.
        /// </summary>
        /// <returns>GraphicsPath of the ToolTip</returns>
        private GraphicsPath GetSymbolPath()
        {
            GraphicsPath gps = null;
            Point target = new Point((int)(m_target.X - Location.X),
              (int)(m_target.Y - Location.Y));

            switch (m_info.Alignment)
            {
                case TabAlignment.Left:
                    target = new Point(0, target.Y - m_info.SymbolSize.Height / 2);
                    break;

                case TabAlignment.Top:
                    target = new Point(target.X - m_info.SymbolSize.Width / 2, 0);
                    break;

                case TabAlignment.Right:
                case TabAlignment.Bottom:

                    target = new Point(target.X - m_info.SymbolSize.Width / 2,
                      target.Y - m_info.SymbolSize.Height / 2);
                    break;
            }

            gps = ChartSymbolHelper.GetPathSymbol(m_info.Symbol,
              new Rectangle(target, m_info.SymbolSize));

            return gps;
        }

        /// <summary>
        /// Gets the center loaction.
        /// </summary>
        /// <param name="rc">The rc.</param>
        /// <param name="sz">The sz.</param>
        /// <returns>Returns the center location point.</returns>
        private PointF GetCenterLoaction(RectangleF rc, SizeF sz)
        {
            return new PointF(rc.X + (rc.Width - sz.Width) / 2,
              rc.Y + (rc.Height - sz.Height) / 2);
        }

        /// <summary>
        /// Draws the text.
        /// </summary>
        /// <param name="g">The graphics content.</param>
        /// <param name="rc">The rectangle to draw text.</param>
        private void DrawText(Graphics g, RectangleF rc)
        {
            using (Brush br = new SolidBrush(m_info.ForeColor))
            {
                SizeF sz = g.MeasureString(Text, m_info.Font);

                if (m_drawSymbol)
                {
                    sz.Width += sz.Height;
                }

                if (sz.Height < m_info.SymbolSize.Height)
                {
                    sz.Height = m_info.SymbolSize.Height;
                    sz.Width += m_info.SymbolSize.Height;
                }
                
                PointF pt = GetCenterLoaction(rc, sz);

                if (m_drawSymbol)
                {
                    using (SolidBrush brush = new SolidBrush(this.SymbolColor))
                    {
                        using (Pen pen = new Pen(m_info.ForeColor))
                        {
                            if (m_info.ResizeInsideSymbol)
                            {
                                  ChartSymbolHelper.FillAndDrawSymbol(g, m_info.Symbol,
                                  new Rectangle(new Point((int)pt.X, (int)pt.Y),
                                  m_info.SymbolSize), pen, brush);                            
                            }
                            else
                            {
                               ChartSymbolHelper.FillAndDrawSymbol(g, m_info.Symbol,
                               new Rectangle(new Point((int)pt.X, (int)pt.Y),
                               new SizeF(sz.Height, sz.Height).ToSize()), pen, brush);
                            }
                        }
                    }

                    pt = new PointF(pt.X + sz.Height, pt.Y);
                }

                g.DrawString(Text, m_info.Font, br, pt);
            }
        }

        /// <summary>
        /// Calculates the alignment.
        /// </summary>
        /// <param name="alignment">The current alignment.</param>
        /// <param name="rect">The rectangle to check alignment.</param>
        /// <returns>Returns TabAlignment.</returns>
        private TabAlignment CalcAlignment(TabAlignment alignment, Rectangle rect)
        {
            if (m_info.CheckLocation)
            {
                if ((rect.Width / 2 > Width) && (rect.Height / 2 > Height))
                {
                    if ((alignment == TabAlignment.Left) &&
                      (m_target.X + Width > rect.Width))
                    {
                        alignment = TabAlignment.Right;
                    }
                    else if ((alignment == TabAlignment.Right) &&
                      (m_target.X - Width < 0))
                    {
                        alignment = TabAlignment.Left;
                    }
                    else if ((alignment == TabAlignment.Top) &&
                      (m_target.Y + Height > rect.Height))
                    {
                        alignment = TabAlignment.Bottom;
                    }
                    else if ((alignment == TabAlignment.Bottom) &&
                      (m_target.Y - Height < 0))
                    {
                        alignment = TabAlignment.Top;
                    }
                }
            }

            return alignment;
        }

        /// <summary>
        /// Called when info was changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnInfoChanged(object sender, EventArgs e)
        {
            if (m_hintWindow != null)
            {
                m_hintWindow.Invalidate();
            }
        }
        #endregion
    }

    #region ToolTipInfo
    /// <summary>
    /// Contains the appearance properties of fancy tooltip.
    /// </summary>
    public sealed class ChartFancyToolTipInfo
    {
        #region Members
        private LineInfo m_border = new LineInfo();
        private bool m_visible = false;
        private bool m_resizeInsideSymbol = true;
        private MarkerStyle m_style = MarkerStyle.SmoothRectangle;
        private float m_toTarget = 20;
        private float m_spacing = 4;
        private TabAlignment m_alignment = TabAlignment.Left;
        private float m_angle = 15.0f;
        private Color m_backColor = SystemColors.Info;
        private Color m_foreColor = Color.Black;
        private Color m_sybmolColor = Color.Red;
        private Font m_font = new Font("Arial", 8);
        private bool m_checkLocation = true;
        private Size m_symbolSize = new Size(10, 10);
        private ChartSymbolShape m_symbol = ChartSymbolShape.Circle;
        #endregion

        #region Events
        internal event EventHandler Changed;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the information that is used for specifying border properties.
        /// </summary>
        public LineInfo Border
        {
            get
            {
                return m_border;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the visibility of the Fancy tooltip. Default is false.
        /// </summary>
        [ChartTemplate(ChartTemplateSet.Simple),DefaultValue(false)]
        public bool Visible
        {
            get
            {
                return m_visible;
            }

            set
            {
                if (m_visible != value)
                {
                    m_visible = value;
                    this.RaiseChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the Resize the Inside of Fancy Symbol. Default is false.
        /// </summary>
        [ChartTemplate(ChartTemplateSet.Simple), DefaultValue(false)]
        public bool ResizeInsideSymbol
        {
            get
            {
                return m_resizeInsideSymbol;
            }

            set
            {
                if (m_resizeInsideSymbol != value)
                {
                    m_resizeInsideSymbol = value;                    
                }
            }
        }

        /// <summary>
        /// Gets or Sets MarkerStyle of the tool tip. Default is SmoothRectangle.
        /// </summary>
        [ChartTemplate(ChartTemplateSet.Simple),DefaultValue(MarkerStyle.SmoothRectangle)]
        public MarkerStyle Style
        {
            get
            {
                return m_style;
            }

            set
            {
                if (m_style != value)
                {
                    m_style = value;
                    this.RaiseChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the space between the symbol and the marker. Default is 20f.
        /// </summary>
        [DefaultValue(20f)]
        public float ToTarget
        {
            get
            {
                return m_toTarget;
            }

            set
            {
                if (m_toTarget != value)
                {
                    m_toTarget = value;
                    this.RaiseChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or Sets the interval between the border and the tool tip text. Default is 4f.
        /// </summary>
        [ChartTemplate(ChartTemplateSet.Simple),DefaultValue(4f)]
        public float Spacing
        {
            get
            {
                return m_spacing;
            }

            set
            {
                if (m_spacing != value)
                {
                    m_spacing = value;
                    this.RaiseChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or Sets the alignment of the tab for the FancyToolTip. Default is Left.
        /// </summary>
        [ChartTemplate(ChartTemplateSet.Simple),DefaultValue(TabAlignment.Left)]
        public TabAlignment Alignment
        {
            get
            {
                return m_alignment;
            }

            set
            {
                if (m_alignment != value)
                {
                    m_alignment = value;
                    this.RaiseChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or Sets the angle of the arrow in the tooltip. Default is 15f.
        /// </summary>
        [ChartTemplate(ChartTemplateSet.Simple),DefaultValue(15f)]
        public float Angle
        {
            get
            {
                return m_angle;
            }

            set
            {
                if (m_angle != value)
                {
                    m_angle = value;
                    this.RaiseChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the backcolor for the FancyToolTip. Default is Color.Info.
        /// </summary>
        [ChartTemplate(ChartTemplateSet.Simple),DefaultValue(typeof(Color), "Info")]
        public Color BackColor
        {
            get
            {
                return m_backColor;
            }

            set
            {
                if (m_backColor != value)
                {
                    m_backColor = value;
                    this.RaiseChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the forecolor for the FancyToolTip. Default is Color.Black.
        /// </summary>
        [ChartTemplate(ChartTemplateSet.Simple),DefaultValue(typeof(Color), "Black")]
        public Color ForeColor
        {
            get
            {
                return m_foreColor;
            }

            set
            {
                if (m_foreColor != value)
                {
                    m_foreColor = value;
                    this.RaiseChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the backcolor for the symbol on FancyToolTip. Default is Color.Red.
        /// </summary>
        [DefaultValue(typeof(Color), "Red"), Browsable(false)
       , DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete]
        public Color SymbolColor
        {
            get
            {
                return m_sybmolColor;
            }

            set
            {
                if (m_sybmolColor != value)
                {
                    m_sybmolColor = value;
                    this.RaiseChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the font information of the FancyToolTip.
        /// </summary>
        [ChartTemplate(ChartTemplateSet.Simple),DefaultValue(typeof(Font), "Arial, 8pt")]
        public Font Font
        {
            get
            {
                return m_font;
            }

            set
            {
                if (m_font != value)
                {
                    m_font = value;
                    this.RaiseChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///Gets or Sets this property. When this property is set to true, the Tool Tip will be auto aligned depending on the size. Default is true.
        /// </summary>
        [DefaultValue(true)]
        public bool CheckLocation
        {
            get
            {
                return m_checkLocation;
            }

            set
            {
                if (m_checkLocation != value)
                {
                    m_checkLocation = value;
                    this.RaiseChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or Sets the size of the Symbol associated with the FancyToolTip. Default is (10, 10).
        /// </summary>
        [ChartTemplate(ChartTemplateSet.Simple),DefaultValue(typeof(Size), "10, 10")]
        public Size SymbolSize
        {
            get
            {
                return m_symbolSize;
            }

            set
            {
                if (m_symbolSize != value)
                {
                    m_symbolSize = value;
                    this.RaiseChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or Sets the symbol associated with the FancyToolTip. Default is Circle.
        /// </summary>
        /// <remarks>
        /// <b>Image</b> symbol isn't work for FancyToolTip.
        /// </remarks>
        [ChartTemplate(ChartTemplateSet.Simple),DefaultValue(ChartSymbolShape.Circle)]
        public ChartSymbolShape Symbol
        {
            get
            {
                return m_symbol;
            }

            set
            {
                if (m_symbol != value)
                {
                    m_symbol = value;
                    this.RaiseChanged(this, EventArgs.Empty);
                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartFancyToolTipInfo"/> class.
        /// </summary>
        public ChartFancyToolTipInfo()
        {
            m_border.SettingsChanged += new EventHandler(OnBorderSettingsChanged);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Sets the characteristics of this tooltip same as the input Fancy tooltip 
        /// </summary>
        /// <param name="fancyToolTip"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Read(ChartFancyToolTip fancyToolTip)
        {
            this.Read(fancyToolTip.Info);
        }

        /// <summary>
        /// Sets this characteristics for the input Fancy tooltip
        /// </summary>
        /// <param name="fancyToolTip"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Write(ChartFancyToolTip fancyToolTip)
        {
            this.Write(fancyToolTip.Info);
        }

        /// <summary>
        /// Sets the characteristics of this tooltip same as the input Fancy tooltip 
        /// </summary>
        /// <param name="fancyToolTip"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Read(ChartFancyToolTipInfo fancyToolTip)
        {
            m_alignment = fancyToolTip.Alignment;
            m_angle = fancyToolTip.Angle;
            m_backColor = fancyToolTip.BackColor;
            m_font = fancyToolTip.Font;
            m_foreColor = fancyToolTip.ForeColor;
            m_spacing = fancyToolTip.Spacing;
            m_style = fancyToolTip.Style;
            m_visible = fancyToolTip.Visible;
            m_resizeInsideSymbol = fancyToolTip.ResizeInsideSymbol;
            m_toTarget = fancyToolTip.ToTarget;
            m_checkLocation = fancyToolTip.CheckLocation;
            m_symbol = fancyToolTip.Symbol;
            m_symbolSize = fancyToolTip.SymbolSize;

            m_border.ForeColor = fancyToolTip.Border.ForeColor;
            m_border.PenType = fancyToolTip.Border.PenType;
            m_border.Width = fancyToolTip.Border.Width;
            m_border.BackColor = fancyToolTip.Border.BackColor;
            m_border.DashStyle = fancyToolTip.Border.DashStyle;
        }

        /// <summary>
        /// Sets this characteristics for the input Fancy tooltip
        /// </summary>
        /// <param name="fancyToolTip"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Write(ChartFancyToolTipInfo fancyToolTip)
        {
            fancyToolTip.Alignment = m_alignment;
            fancyToolTip.Angle = m_angle;
            fancyToolTip.BackColor = m_backColor;
            fancyToolTip.Font = m_font;
            fancyToolTip.ForeColor = m_foreColor;
            fancyToolTip.Spacing = m_spacing;
            fancyToolTip.Style = m_style;
            fancyToolTip.Visible = m_visible;
            fancyToolTip.ResizeInsideSymbol = m_resizeInsideSymbol;
            fancyToolTip.ToTarget = m_toTarget;
            fancyToolTip.CheckLocation = m_checkLocation;
            fancyToolTip.Symbol = m_symbol;
            fancyToolTip.SymbolSize = m_symbolSize;

            fancyToolTip.Border.ForeColor = m_border.ForeColor;
            fancyToolTip.Border.PenType = m_border.PenType;
            fancyToolTip.Border.Width = m_border.Width;
            fancyToolTip.Border.BackColor = m_border.BackColor;
            fancyToolTip.Border.DashStyle = m_border.DashStyle;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Called when border settings was changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnBorderSettingsChanged(object sender, EventArgs e)
        {
            this.RaiseChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the changed event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void RaiseChanged(object sender, EventArgs args)
        {
            if (Changed != null)
            {
                Changed(sender, args);
            }
        }
        #endregion
    }
    #endregion
}