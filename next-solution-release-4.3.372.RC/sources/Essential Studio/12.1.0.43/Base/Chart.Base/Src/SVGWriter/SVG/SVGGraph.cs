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
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using Syncfusion.Documentation;

namespace Syncfusion.Windows.Forms.Chart.SvgBase
{
    /// <summary>
    ///  Provides methods for drawing primitives to the SVG document.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class SvgGraph
    {
        #region Members
        private GElement m_g;
        private SvgDocument m_document;
        private GElement m_currentG = null;
        private Matrix m_tranform = new Matrix();
        #endregion

        #region Proprties
        /// <summary>
        /// Gets the <see cref="SvgDocument"/>.
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
        /// Gets or sets the world transformation for this <see cref="SvgGraph"/>.
        /// </summary>
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
        /// <param name="generateDefault">if set to <c>true</c> create default document.</param>
        public SvgGraph(bool generateDefault)
        {
            if (generateDefault)
            {
                this.GenerateNewDocument();
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
        /// Draws the document to the specified <see cref="Graphics"/>.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/>.</param>
        public void DrawDocument(Graphics g)
        {
            m_document.Draw(g);
        }

        /// <summary>
        /// Saves the document to the file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void SaveDocument(string filename)
        {
            m_document.Save(filename);
        }

        /// <summary>
        /// Loads the document from the file.
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
        /// <param name="pen">The <see cref="Pen"/>.</param>
        /// <param name="x">The x coordinare of rectangle.</param>
        /// <param name="y">The y coordinare of rectangle.</param>
        /// <param name="width">The width of rectangle.</param>
        /// <param name="height">The height of rectangle.</param>
        public void DrawRectangle(Pen pen, float x, float y, float width, float height)
        {
            RectElement re = RectElement.FromRectangleF(new RectangleF(x, y, width, height));
            m_currentG.AddChild(re);

            SetTransform(re);
            Utility.SetGDIPen(re, pen);
            Utility.SetGDIBrush(re, null);
        }

        /// <summary>
        /// Draws the ellipse.
        /// </summary>
        /// <param name="pen">The <see cref="Pen"/>.</param>
        /// <param name="x">The x coordinare of ellipse.</param>
        /// <param name="y">The y coordinare of ellipse.</param>
        /// <param name="width">The width of ellipse.</param>
        /// <param name="height">The height of ellipse.</param>
        public void DrawEllipse(Pen pen, float x, float y, float width, float height)
        {
            EllipseElement ee = EllipseElement.FromRectangleF(new RectangleF(x, y, width, height));
            m_currentG.AddChild(ee);

            SetTransform(ee);
            Utility.SetGDIPen(ee, pen);
            Utility.SetGDIBrush(ee, null);
        }

        /// <summary>
        /// Draws the path.
        /// </summary>
        /// <param name="pen">The <see cref="Pen"/>.</param>
        /// <param name="gp">The <see cref="GraphicsPath"/>.</param>
        public void DrawPath(Pen pen, GraphicsPath gp)
        {
            PathElement pe = new PathElement();
            m_currentG.AddChild(pe);

            pe.D = new Data(gp.PathData);

            SetTransform(pe);
            Utility.SetGDIPen(pe, pen);
            Utility.SetGDIBrush(pe, null);
        }

        /// <summary>
        /// Draws the string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The <see cref="Font"/>.</param>
        /// <param name="br">The <see cref="Brush"/>.</param>
        /// <param name="x">The x coordinate of text.</param>
        /// <param name="y">The y coordinate of text.</param>
        public void DrawString(string text, Font font, Brush br, float x, float y)
        {
            TextElement te = new TextElement();
            m_currentG.AddChild(te);

            te.Text = text;
            te.X = new Length(x);
            te.Y = new Length(y + font.Height);

            SetTransform(te);
            Utility.SetGDIFont(te, font);
            Utility.SetGDIBrush(te, br);
        }

        /// <summary>
        /// Draws the line.
        /// </summary>
        /// <param name="pen">The <see cref="Pen"/>.</param>
        /// <param name="x1">The x coordinate of start point.</param>
        /// <param name="y1">The y coordinate of start point.</param>
        /// <param name="x2">The x coordinate of end point.</param>
        /// <param name="y2">The y coordinate of end point.</param>
        public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
        {
            LineElement le = new LineElement();
            m_currentG.AddChild(le);

            le.X1 = new Length(x1);
            le.Y1 = new Length(y1);
            le.X2 = new Length(x2);
            le.Y2 = new Length(y2);

            SetTransform(le);
            Utility.SetGDIPen(le, pen);
            Utility.SetGDIBrush(le, null);
        }

        /// <summary>
        /// Draws the polygon.
        /// </summary>
        /// <param name="pen">The <see cref="Pen"/>.</param>
        /// <param name="points">The array of <see cref="PointF"/>.</param>
        public void DrawPolygon(Pen pen, PointF[] points)
        {
            PolygonElement pe = new PolygonElement();
            m_currentG.AddChild(pe);

            pe.Points = new PointsArray(points);

            SetTransform(pe);
            Utility.SetGDIPen(pe, pen);
            Utility.SetGDIBrush(pe, null);
        }

        /// <summary>
        /// Draws the lines.
        /// </summary>
        /// <param name="pen">The <see cref="Pen"/>.</param>
        /// <param name="points">The array of <see cref="PointF"/>.</param>
        public void DrawLines(Pen pen, PointF[] points)
        {
            PolylineElement pe = new PolylineElement();
            m_currentG.AddChild(pe);

            pe.Points = new PointsArray(points);

            SetTransform(pe);
            Utility.SetGDIPen(pe, pen);
            Utility.SetGDIBrush(pe, null);
        }

        /// <summary>
        /// Draws the image.
        /// </summary>
        /// <param name="img">The <see cref="Image"/>.</param>
        /// <param name="x">The x coordinare of image.</param>
        /// <param name="y">The y coordinare of image.</param>
        /// <param name="width">The width of image.</param>
        /// <param name="height">The height of image.</param>
        public void DrawImage(Image img, float x, float y, float width, float height)
        {
            ImageElement ie = ImageElement.FromImage(img, x, y, width, height);
            m_currentG.AddChild(ie);
            SetTransform(ie);
        }
        #endregion

        #region Graphics Fill methods
        /// <summary>
        /// Fills the path.
        /// </summary>
        /// <param name="br">The <see cref="Brush"/>.</param>
        /// <param name="gp">The <see cref="GraphicsPath"/>.</param>
        public void FillPath(Brush br, GraphicsPath gp)
        {
            PathElement pe = new PathElement();
            m_currentG.AddChild(pe);

            pe.D = new Data(gp.PathData);

            SetTransform(pe);
            Utility.SetGDIPen(pe, null);
            Utility.SetGDIBrush(pe, br);
        }

        /// <summary>
        /// Fills the rectangle.
        /// </summary>
        /// <param name="br">The <see cref="Brush"/>.</param>
        /// <param name="x">The x coordinare of rectangle.</param>
        /// <param name="y">The y coordinare of rectangle.</param>
        /// <param name="width">The width of rectangle.</param>
        /// <param name="height">The height of rectangle.</param>
        public void FillRectangle(Brush br, float x, float y, float width, float height)
        {
            RectElement re = RectElement.FromRectangleF(new RectangleF(x, y, width, height));
            m_currentG.AddChild(re);

            SetTransform(re);
            Utility.SetGDIPen(re, null);
            Utility.SetGDIBrush(re, br);
        }

        /// <summary>
        /// Fills the ellipse.
        /// </summary>
        /// <param name="br">The <see cref="Brush"/>.</param>
        /// <param name="x">The x coordinare of ellipse.</param>
        /// <param name="y">The y coordinare of ellipse.</param>
        /// <param name="width">The width of ellipse.</param>
        /// <param name="height">The height of ellipse.</param>
        public void FillEllipse(Brush br, float x, float y, float width, float height)
        {
            EllipseElement rc = EllipseElement.FromRectangleF(new RectangleF(x, y, width, height));
            m_currentG.AddChild(rc);

            SetTransform(rc);
            Utility.SetGDIPen(rc, null);
            Utility.SetGDIBrush(rc, br);
        }

        /// <summary>
        /// Fills the polygon.
        /// </summary>
        /// <param name="br">The <see cref="Brush"/>.</param>
        /// <param name="points">The array of <see cref="PointF"/>.</param>
        public void FillPolygon(Brush br, PointF[] points)
        {
            PolygonElement pe = new PolygonElement();
            m_currentG.AddChild(pe);

            pe.Points = new PointsArray(points);

            SetTransform(pe);
            Utility.SetGDIPen(pe, null);
            Utility.SetGDIBrush(pe, br);
        }
        #endregion

        #region Graphics Tranformation
        /// <summary>
        /// Changes the origin of the coordinate system.
        /// </summary>
        /// <param name="dx">The horizontal offset.</param>
        /// <param name="dy">The vertival offset.</param>
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
            m_currentG.AddChild(rc);
            rc.Width = new Length(100, LengthType.Percentage);
            rc.Height = new Length(100, LengthType.Percentage);
            (rc as IFillAttributes).FillOpacity = new Opacity(color);
            (rc as IFillAttributes).Fill = new NoneColor(color);
        }

        /// <summary>
        /// Sets the clip.
        /// </summary>
        /// <param name="gp">The gp.</param>
        public void SetClip(GraphicsPath gp)
        {
            ClipPathElement cpe = new ClipPathElement();
            PathElement pe = new PathElement();
            cpe.AddChild(pe);
            m_g.AddChild(cpe);

            pe.D = new Data(gp.PathData);

            m_currentG = new GElement();
            this.SetClip(m_currentG, Utility.GetUrlFromId(cpe.Id));
            m_g.AddChild(m_currentG);
        }

        /// <summary>
        /// Resets the clip.
        /// </summary>
        public void ResetClip()
        {
            m_currentG = m_g;
        }
        #endregion

        #endregion

        #region Helper methods
        /// <summary>
        /// Sets the clip.
        /// </summary>
        /// <param name="elem">The elem.</param>
        /// <param name="uri">The URI.</param>
        private void SetClip(IClipingAttribute elem, string uri)
        {
            if (uri != null && uri != "")
            {
                elem.ClipPath = uri;
            }
        }

        /// <summary>
        /// Sets the transform.
        /// </summary>
        /// <param name="elem">The elem.</param>
        private void SetTransform(ITransformAttribute elem)
        {
            if (!m_tranform.IsIdentity)
            {
                elem.Transform = new TransformList(m_tranform);
            }
        }

        /// <summary>
        /// Generates the new document.
        /// </summary>
        private void GenerateNewDocument()
        {
            m_document = new SvgDocument();
            m_g = new GElement();
            m_currentG = m_g;
            m_document.Svg.AddChild(m_g);
        }
        #endregion
    }
}
