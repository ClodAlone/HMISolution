#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Syncfusion.SVG.IO
{
    /// <summary>
    /// Class containing Svg Graph.
    /// </summary>
    public class SvgGraph
    {
        #region Members
        private GElement m_g;
        private SvgDocument m_document;
        private Matrix m_tranform = new Matrix();
        private string m_curentClipID = string.Empty;
        #endregion

        #region Proprties
        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <value>The document.</value>
        public SvgDocument Document
        {
            get
            {
                return m_document;
            }
        }

        /// <summary>
        /// Gets or sets the transform.
        /// </summary>
        /// <value>The transform.</value>
        public Matrix Transform
        {
            get
            {
                return m_tranform;
            }
            set
            {
                m_tranform = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SvgGraph"/> class.
        /// </summary>
        public SvgGraph()
            : this(true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgGraph"/> class.
        /// </summary>
        /// <param name="generateDefault">if set to <c>true</c> [generate default].</param>
        public SvgGraph(bool generateDefault)
        {
            if (generateDefault)
            {
                GenerateNewDocument();
            }
            else
            {
                m_document = new SvgDocument();
            }
        }
        #endregion

        #region Public methods

        #region Document methods
        /// <summary>
        /// Draws the document.
        /// </summary>
        /// <param name="g">The graphics.</param>
        public void DrawDocument(Graphics g)
        {
            m_document.Draw(g);
        }

        /// <summary>
        /// Saves the document.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void SaveDocument(string filename)
        {
            m_document.Save(filename);
        }

        /// <summary>
        /// Loads the document.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void LoadDocument(string filename)
        {
            m_document.Load(filename);
        }
        #endregion

        #region Graphics Draw methods
        /// <summary>
        /// Draws the rectangle.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void DrawRectangle(Pen pen, float x, float y, float width, float height)
        {
            RectElement re = RectElement.FromRectangleF(new RectangleF(x, y, width, height));
            m_g.AddChild(re);

            SetClip(re);
            SetTransform(re);
            Utility.SetGDIPen(re, pen);
            Utility.SetGDIBrush(re, null);
        }

        /// <summary>
        /// Draws the ellipse.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void DrawEllipse(Pen pen, float x, float y, float width, float height)
        {
            EllipseElement ee = EllipseElement.FromRectangleF(new RectangleF(x, y, width, height));
            m_g.AddChild(ee);

            SetClip(ee);
            SetTransform(ee);
            Utility.SetGDIPen(ee, pen);
            Utility.SetGDIBrush(ee, null);
        }

        /// <summary>
        /// Draws the path.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="gp">The graphics path.</param>
        public void DrawPath(Pen pen, GraphicsPath gp)
        {
            PathElement pe = new PathElement();
            m_g.AddChild(pe);

            pe.D = new Data(gp.PathData);

            SetClip(pe);
            SetTransform(pe);
            Utility.SetGDIPen(pe, pen);
            Utility.SetGDIBrush(pe, null);
        }

        /// <summary>
        /// Draws the string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="br">The brush.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void DrawString(string text, Font font, Brush br, float x, float y)
        {
            TextElement te = new TextElement();
            m_g.AddChild(te);

            te.Text = text;
            te.X = new Length(x);
            te.Y = new Length(y + font.Height);

            SetClip(te);
            SetTransform(te);
            Utility.SetGDIFont(te, font);
            Utility.SetGDIBrush(te, br);
        }

        /// <summary>
        /// Draws the line.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
        {
            LineElement le = new LineElement();
            m_g.AddChild(le);

            le.X1 = new Length(x1);
            le.Y1 = new Length(y1);
            le.X2 = new Length(x2);
            le.Y2 = new Length(y2);

            SetClip(le);
            SetTransform(le);
            Utility.SetGDIPen(le, pen);
            Utility.SetGDIBrush(le, null);
        }

        /// <summary>
        /// Draws the polygon.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="points">The points.</param>
        public void DrawPolygon(Pen pen, PointF[] points)
        {
            PolygonElement pe = new PolygonElement();
            m_g.AddChild(pe);

            pe.Points = new PointsArray(points);

            SetClip(pe);
            SetTransform(pe);
            Utility.SetGDIPen(pe, pen);
            Utility.SetGDIBrush(pe, null);
        }

        /// <summary>
        /// Draws the lines.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="points">The points.</param>
        public void DrawLines(Pen pen, PointF[] points)
        {
            PolylineElement pe = new PolylineElement();
            m_g.AddChild(pe);

            pe.Points = new PointsArray(points);

            SetClip(pe);
            SetTransform(pe);
            Utility.SetGDIPen(pe, pen);
            Utility.SetGDIBrush(pe, null);
        }

        /// <summary>
        /// Draws the image.
        /// </summary>
        /// <param name="img">The img.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void DrawImage(Image img, float x, float y, float width, float height)
        {
            ImageElement ie = ImageElement.FromImage(img, x, y, width, height);
            m_g.AddChild(ie);

            SetClip(ie);
            SetTransform(ie);
        }
        #endregion

        #region Graphics Fill methods
        /// <summary>
        /// Fills the path.
        /// </summary>
        /// <param name="br">The brush.</param>
        /// <param name="gp">The graphics path.</param>
        public void FillPath(Brush br, GraphicsPath gp)
        {
            PathElement pe = new PathElement();
            m_g.AddChild(pe);

            pe.D = new Data(gp.PathData);

            SetClip(pe);
            SetTransform(pe);
            Utility.SetGDIPen(pe, null);
            Utility.SetGDIBrush(pe, br);
        }

        /// <summary>
        /// Fills the rectangle.
        /// </summary>
        /// <param name="br">The br.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void FillRectangle(Brush br, float x, float y, float width, float height)
        {
            RectElement re = RectElement.FromRectangleF(new RectangleF(x, y, width, height));
            m_g.AddChild(re);

            SetTransform(re);
            SetClip(re);
            Utility.SetGDIPen(re, null);
            Utility.SetGDIBrush(re, br);
        }

        /// <summary>
        /// Fills the ellipse.
        /// </summary>
        /// <param name="br">The brush.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void FillEllipse(Brush br, float x, float y, float width, float height)
        {
            EllipseElement rc = EllipseElement.FromRectangleF(new RectangleF(x, y, width, height));
            m_g.AddChild(rc);

            SetTransform(rc);
            SetClip(rc);
            Utility.SetGDIPen(rc, null);
            Utility.SetGDIBrush(rc, br);
        }

        /// <summary>
        /// Fills the polygon.
        /// </summary>
        /// <param name="br">The brush.</param>
        /// <param name="points">The points.</param>
        public void FillPolygon(Brush br, PointF[] points)
        {
            PolygonElement pe = new PolygonElement();
            m_g.AddChild(pe);

            pe.Points = new PointsArray(points);

            SetTransform(pe);
            SetClip(pe);
            Utility.SetGDIPen(pe, null);
            Utility.SetGDIBrush(pe, br);
        }
        #endregion

        #region Graphics Tranformation
        /// <summary>
        /// Translates the transform.
        /// </summary>
        /// <param name="dx">The dx.</param>
        /// <param name="dy">The dy.</param>
        public void TranslateTransform(float dx, float dy)
        {
            m_tranform.Translate(dx, dy);
        }
        #endregion

        #region Graphics Other
        /// <summary>
        /// Clears the specified color.
        /// </summary>
        /// <param name="color">The color.</param>
        public void Clear(Color color)
        {
            GenerateNewDocument();
            RectElement rc = new RectElement();
            m_g.AddChild(rc);
            rc.Width = new Length(100, LengthType.Percentage);
            rc.Height = new Length(100, LengthType.Percentage);
            (rc as IFillAttributes).Fill = new NoneColor(color);
        }

        /// <summary>
        /// Sets the clip.
        /// </summary>
        /// <param name="gp">The graphics path.</param>
        public void SetClip(GraphicsPath gp)
        {
            ClipPathElement cpe = new ClipPathElement();
            PathElement pe = new PathElement();
            cpe.AddChild(pe);
            m_g.AddChild(cpe);

            pe.D = new Data(gp.PathData);
            m_curentClipID = cpe.Id;
        }

        /// <summary>
        /// Resets the clip.
        /// </summary>
        public void ResetClip()
        {
            m_curentClipID = string.Empty;
        }
        #endregion

        #endregion

        #region Helper methods
        /// <summary>
        /// Sets the clip.
        /// </summary>
        /// <param name="elem">The element.</param>
        private void SetClip(IClipingAttribute elem)
        {
            if (m_curentClipID != string.Empty)
            {
                elem.ClipPath = Utility.GetUrlFromId(m_curentClipID);
            }
        }
        private void SetTransform(ITransformAttribute elem)
        {
            if (!m_tranform.IsIdentity)
            {
                elem.Transform = new TransformList(m_tranform);
            }
        }
        private void GenerateNewDocument()
        {
            m_document = new SvgDocument();
            m_g = new GElement();
            m_document.Svg.AddChild(m_g);
        }
        #endregion
    }
}
