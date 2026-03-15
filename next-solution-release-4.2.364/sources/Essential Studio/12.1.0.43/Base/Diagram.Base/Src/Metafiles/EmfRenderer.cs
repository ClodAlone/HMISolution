#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms.VisualStyles;

namespace Syncfusion.Windows.Forms.Diagram
{
    internal class EmfRenderer : IDisposable
    {
        #region Static fields
        private static Image s_bmp = new Bitmap(1, 1);
        #endregion

        #region Fields
        private SizeF m_szSize;
        private Graphics m_graphics;

        /// <summary>
        /// Matrix indicating bounds for the metafile output.
        /// </summary>
        private Matrix m_bounds;

        /// <summary>
        /// Shows if the graphics state was changed.
        /// </summary>
        private bool m_stateChanged;
        private GraphicsState m_startState;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <value>The size.</value>
        public SizeF Size
        {
            get { return m_szSize; }
        }

        /// <summary>
        /// Gets the graphics object.
        /// </summary>
        public Graphics Graphics
        {
            get
            {
                return m_graphics;
            }
        }

        /// <summary>
        /// Gets the clip bounds.
        /// </summary>
        private RectangleF ClipBounds
        {
            get
            {
                return RectangleF.Empty;
            }
        }

        /// <summary>
        /// Gets or sets transformation of graphics.
        /// </summary>
        public Matrix Transform
        {
            get
            {
                return Graphics.Transform;
            }
            set
            {
                // Apply new transformation.
                Graphics.Transform = value;
            }
        }

        /// <summary>
        /// Gets or sets the scaling between world units and page units for this Graphics object.
        /// </summary>
        public float PageScale
        {
            get
            {
                return Graphics.PageScale;
            }
            set
            {
                Graphics.PageScale = value;
            }
        }

        /// <summary>
        /// Gets or sets the unit of measure used for page coordinates in this Graphics object.
        /// </summary>
        public GraphicsUnit PageUnit
        {
            get
            {
                return Graphics.PageUnit;
            }
            set
            {
                Graphics.PageUnit = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EmfRenderer"/> class.
        /// </summary>
        /// <param name="graphics">Graphics to draw on.</param>
        /// <param name="szSize">Size of the work area.</param>
        public EmfRenderer(Graphics graphics, SizeF szSize)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");

            if (szSize.IsEmpty)
                throw new ArgumentNullException("szSize is empty");

            m_szSize = new SizeF(Math.Abs(szSize.Width), Math.Abs(szSize.Height));
            m_graphics = graphics;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Begins a new virtual graphics container.
        /// </summary>
        /// <returns>A GraphicsContainer instance.</returns>
        public GraphicsContainer BeginContainer()
        {
            return Graphics.BeginContainer();
        }

        /// <summary>
        /// Begins a new virtual container.
        /// </summary>
        /// <param name="destRect">The destination rectangle.</param>
        /// <param name="srcRect">The source rectangle.</param>
        /// <param name="unit">The unit.</param>
        /// <returns>A GraphicsContainer instance.</returns>
        public GraphicsContainer BeginContainer(RectangleF destRect, RectangleF srcRect, GraphicsUnit unit)
        {
            return Graphics.BeginContainer(destRect, srcRect, unit);
        }

        /// <summary>
        /// Fills the entire graphics with the specified color.
        /// </summary>
        /// <param name="color">The color.</param>
        public void Clear(Color color)
        {
            Graphics.Clear(color);

            using (Brush brush = new SolidBrush(color))
            {
                RectangleF[] rects = new RectangleF[] { Graphics.ClipBounds };
                FillRectangles(brush, rects);
            }
        }

        /// <summary>
        /// Draws an arc.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="rect">The rectangle specifying the boundaries of the full circle,
        /// of which the arc is a part.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        public void DrawArc(Pen pen, RectangleF rect, float startAngle, float sweepAngle)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");

            OnDrawPrimitive();
            Graphics.DrawArc(pen, rect, startAngle, sweepAngle);
        }

        /// <summary>
        /// Draws one or more Bezier curves.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="points">The points.</param>
        public void DrawBeziers(Pen pen, PointF[] points)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");

            if (points == null)
                throw new ArgumentNullException("points");

            if (points.Length < 4)
                throw new ArgumentException("Incorrect size of array", "points");

            OnDrawPrimitive();

            // Number of points for bezier curve.
            int bound = 3;
            int index = 0;
            PointF start = points[index];
            index++;

            while (true)
            {
                if ((index + bound) > points.Length) break;

                PointF inner1 = points[index];
                index++;
                PointF inner2 = points[index];
                index++;
                PointF end = points[index];
                index++;

                Graphics.DrawBezier(pen, start, inner1, inner2, end);

                start = end;
            }
        }

        /// <summary>
        /// Draws a closed curve.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="points">The points.</param>
        /// <param name="tension">The tension.</param>
        /// <param name="fillMode">The fill mode.</param>
        public void DrawClosedCurve(Pen pen, PointF[] points, float tension, FillMode fillMode)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");

            Graphics.DrawPolygon(pen, points);
        }

        /// <summary>
        /// Draws a curve.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="points">The points.</param>
        /// <param name="penPoints">Points to custom cap.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="numSegments">The number of the segments.</param>
        /// <param name="tension">The tension.</param>
        /// <remarks>It isn't suppoted.</remarks>
        public void DrawCurve(Pen pen, PointF[] points, PointF[] penPoints, int offset, int numSegments, float tension)
        {
            GraphicsPath path = new GraphicsPath();

            bool isLine = IsLine(points);

            if (!isLine)
            {
                path.AddCurve(points, tension);
            }
            else
            {
                path.AddLines(points);
            }

            DrawPath(pen, path);
        }

        /// <summary>
        /// Draws an ellipse.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="rect">The rectangle specifying the boundaries of the ellipse.</param>
        public void DrawEllipse(Pen pen, RectangleF rect)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");

            OnDrawPrimitive();
            Graphics.DrawEllipse(pen, rect);
        }

        /// <summary>
        /// Draws an image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="destRect">The destination rectangle.</param>
        /// <param name="srcRect">The source rectangle.</param>
        /// <param name="units">The units.</param>
        public void DrawImage(Image image, RectangleF destRect, RectangleF srcRect, GraphicsUnit units)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            OnDrawPrimitive();
            Graphics.DrawImage(image, destRect);
        }

        /// <summary>
        /// Draws an image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="points">The points.</param>
        /// <param name="srcRect">The source rectangle.</param>
        /// <param name="units">The units.</param>
        public void DrawImage(Image image, PointF[] points, RectangleF srcRect, GraphicsUnit units)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            if (points.Length < 0 || points.Length > 3)
                throw new ArgumentOutOfRangeException("points", points, "Value can not be less 0 and greater 3");

            OnDrawPrimitive();
            float left = Math.Min(points[0].X, points[2].X);
            float top = points[0].Y;
            float width = points[1].X - points[0].X;
            float height = points[2].Y - points[0].Y;
            RectangleF rect = new RectangleF(left, top, width, height);

            Graphics.DrawImage(image, rect);
        }

        /// <summary>
        /// Draws an image and/or brush.
        /// </summary>
        /// <param name="image">The image to draw.</param>
        /// <param name="brush">The brush to draw.</param>
        /// <param name="destRect">Where to draw.</param>
        /// <param name="srcRect">Where to draw from.</param>
        /// <param name="dwRop">Raster Operation Code.</param>
        public void DrawImage(Image image, Brush brush, RectangleF destRect, RectangleF srcRect, uint dwRop)
        {
            OnDrawPrimitive();

            switch ((RASTER_CODE)dwRop)
            {
                case RASTER_CODE.SRCCOPY:
                    if (image != null)
                    {
                        DrawImage(image, destRect, srcRect, GraphicsUnit.Pixel);
                    }
                    break;

                case RASTER_CODE.PATCOPY:
                    FillRectangles(brush, new RectangleF[] { destRect });
                    break;

                default:
                    if (image != null)
                    {
                        Graphics.DrawImage(image, destRect);
                    }
                    else
                    {
                        Graphics.DrawRectangle(new Pen(brush), Geometry.ConvertRectangle(destRect));
                    }
                    break;
            }
        }

        /// <summary>
        /// Draws lines specified by vertices.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="points">The points.</param>
        public void DrawLines(Pen pen, PointF[] points)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");

            if (points == null)
                throw new ArgumentNullException("points");

            int length = points.Length;

            if (length < 2)
                throw new ArgumentException("Incorrect size of array", "points");

            OnDrawPrimitive();
            PointF start = points[0];

            for (int i = 1; i < length; i++)
            {
                PointF last = points[i];
                Graphics.DrawLine(pen, start, last);

                start = last;
            }

            float width = pen.Width / 2;
            System.Drawing.Drawing2D.LineCap cap = pen.EndCap;

            if (length > 1 || cap == System.Drawing.Drawing2D.LineCap.Round)
            {
                DrawCap(cap, points, length - 2, length - 1, width, pen.Brush);
            }

            cap = pen.StartCap;

            if (length > 1 || cap == System.Drawing.Drawing2D.LineCap.Round)
            {
                DrawCap(cap, points, 0, 1, width, pen.Brush);
            }
        }

        /// <summary>
        /// Draws a path.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="path">The path.</param>
        public void DrawPath(Pen pen, GraphicsPath path)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");

            if (path == null)
                throw new ArgumentNullException("path");

            OnDrawPrimitive();
            Graphics.DrawPath(pen, path);
        }

        /// <summary>
        /// Draws polygon.
        /// </summary>
        /// <param name="pen">Pen object.</param>
        /// <param name="points">Array of points.</param>
        public void DrawPolygon(Pen pen, PointF[] points)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");

            if (points == null)
                throw new ArgumentNullException("points");

            OnDrawPrimitive();
            Graphics.DrawPolygon(pen, points);
        }

        /// <summary>
        /// Draws a pie.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="rect">The rectangle specifying the boundaries of the complete circle,
        /// of which the pie is a part.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        public void DrawPie(Pen pen, RectangleF rect, float startAngle, float sweepAngle)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");

            OnDrawPrimitive();
            Graphics.DrawPie(pen, rect, startAngle, sweepAngle);
        }

        /// <summary>
        /// Draws a serie of rectangles.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="rects">An array of rectangles.</param>
        public void DrawRectangles(Pen pen, RectangleF[] rects)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");

            if (rects == null)
                throw new ArgumentNullException("rects");

            OnDrawPrimitive();

            for (int i = 0; i < rects.Length; i++)
            {
                Graphics.DrawRectangle(pen, Geometry.ConvertRectangle(rects[i]));
            }
        }

        /// <summary>
        /// Draws a text string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="rect">The boundaries of the text.</param>
        public void DrawString(string text, Font font, Brush brush, RectangleF rect)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            if (font == null)
                throw new ArgumentNullException("font");

            if (brush == null)
                throw new ArgumentNullException("brush");

            OnDrawPrimitive();

            if (rect.Size.IsEmpty)
            {
                SizeF textSize = Graphics.MeasureString(text, font);
                Graphics.DrawString(text, font, brush, rect.Location);
            }
            else
            {
                if (rect.Height == 0)
                    rect.Height = font.Height;

                Graphics.DrawString(text, font, brush, rect);
            }
        }

        /// <summary>
        /// Draws a text string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="rect">The text boundaries.</param>
        /// <param name="format">The string format.</param>
        public void DrawString(string text, Font font, Brush brush, RectangleF rect, StringFormat format)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            if (font == null)
                throw new ArgumentNullException("font");

            if (brush == null)
                throw new ArgumentNullException("brush");

            if (format == null)
                throw new ArgumentNullException("format");

            OnDrawPrimitive();
            Graphics.DrawString(text, font, brush, rect, format);
        }

        /// <summary>
        /// Ends the specified graphics container.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <remarks>Restores the graphics state saved 
        /// by the appropriate BeginContainer method.</remarks>
        public void EndContainer(GraphicsContainer container)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            Graphics.EndContainer(container);
            m_stateChanged = true;
        }

        /// <summary>
        /// Fills a closed curve.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="points">The points of the curve.</param>
        /// <param name="fillMode">The fill mode.</param>
        /// <param name="tension">The tension.</param>
        /// <remarks>It isn't supported.</remarks>
        public void FillClosedCurve(Brush brush, PointF[] points, System.Drawing.Drawing2D.FillMode fillMode, float tension)
        {
            if (brush == null)
                throw new ArgumentNullException("brush");

            Graphics.FillClosedCurve(brush, points);
        }

        /// <summary>
        /// Fills an ellipse.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="rect">The boundaries of the ellipse.</param>
        public void FillEllipse(Brush brush, RectangleF rect)
        {
            if (brush == null)
                throw new ArgumentNullException("brush");

            OnDrawPrimitive();
            Graphics.FillEllipse(brush, rect);
        }

        /// <summary>
        /// Fills a path.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="path">The path.</param>
        public void FillPath(Brush brush, GraphicsPath path)
        {
            if (brush == null)
                throw new ArgumentNullException("brush");

            if (path == null)
                throw new ArgumentNullException("path");

            OnDrawPrimitive();
            Graphics.FillPath(brush, path);
        }

        /// <summary>
        /// Fills a pie.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="x">The x coordinate of the ellipse boundaries, which the pie is a part of.</param>
        /// <param name="y">The y coordinate of the ellipse boundaries, which the pie is a part of.</param>
        /// <param name="width">The width of the ellipse boundaries, which the pie is a part of.</param>
        /// <param name="height">The height of the ellipse boundaries, which the pie is a part of.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            if (brush == null)
                throw new ArgumentNullException("brush");

            OnDrawPrimitive();

            Graphics.FillPie(brush, x, y, width, height, startAngle, sweepAngle);
        }

        /// <summary>
        /// Fills a polygon.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="points">The points of the poligon.</param>
        public void FillPolygon(Brush brush, PointF[] points)
        {
            if (brush == null)
                throw new ArgumentNullException("brush");

            if (points == null)
                throw new ArgumentNullException("points");

            OnDrawPrimitive();
            Graphics.FillPolygon(brush, points);
        }

        /// <summary>
        /// Fills rectangles.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="rects">The rectangles.</param>
        public void FillRectangles(Brush brush, RectangleF[] rects)
        {
            if (brush == null)
                throw new ArgumentNullException("brush");

            if (rects == null)
                throw new ArgumentNullException("rects");

            OnDrawPrimitive();

            for (int i = 0; i < rects.Length; i++)
            {
                Graphics.FillRectangle(brush, rects[i]);
            }
        }

        /// <summary>
        /// Fills a region.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="region">The region.</param>
        public void FillRegion(Brush brush, Region region)
        {
            if (brush == null)
                throw new ArgumentNullException("brush");

            if (region == null)
                throw new ArgumentNullException("region");

            OnDrawPrimitive();

            Matrix matrix = Graphics.Transform;
            RectangleF[] rects = region.GetRegionScans(matrix);

            FillRectangles(brush, rects);
        }

        /// <summary>
        /// Performs multiply transformations.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="order">The order.</param>
        public void MultiplyTransform(Matrix matrix, MatrixOrder order)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            Graphics.MultiplyTransform(matrix, order);
        }

        /// <summary>
        /// Translates the current clip region.
        /// </summary>
        /// <param name="dx">The dx.</param>
        /// <param name="dy">The dy.</param>
        public void TranslateClip(float dx, float dy)
        {
            Graphics.TranslateClip(dx, dy);
            m_stateChanged = true;
        }

        /// <summary>
        /// Resets the current clip region to the infinite region.
        /// </summary>
        public void ResetClip()
        {
            Graphics.ResetClip();
            m_stateChanged = true;
        }

        /// <summary>
        /// Resets the transformantions.
        /// </summary>
        public void ResetTransform()
        {
            Graphics.ResetTransform();
        }

        /// <summary>
        /// Performs the rotate transformations.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <param name="order">The order.</param>
        public void RotateTransform(float angle, MatrixOrder order)
        {
            Graphics.RotateTransform(angle, order);
        }

        /// <summary>
        /// Saves the current graphics state.
        /// </summary>
        /// <returns>A GraphicsState instance that stores
        /// iformation about the current graphic state.</returns>
        public GraphicsState Save()
        {
            return Graphics.Save();
        }

        /// <summary>
        /// Restores the graphics state to the specified graphics state.
        /// </summary>
        /// <param name="gState">The saved graphics state.</param>
        public void Restore(GraphicsState gState)
        {
            Graphics.Restore(gState);
            m_stateChanged = true;
        }

        /// <summary>
        /// Performs scaling transformations.
        /// </summary>
        /// <param name="sx">The scaling facto by x coordinate.</param>
        /// <param name="sy">The scaling facto by y coordinate.</param>
        /// <param name="order">The order.</param>
        public void ScaleTransform(float sx, float sy, MatrixOrder order)
        {
            Graphics.ScaleTransform(sx, sy, order);
        }

        /// <summary>
        /// Sets the current clip region.
        /// </summary>
        /// <param name="path">The path specifying the clip region.</param>
        /// <param name="mode">The combaining mode.</param>
        public void SetClip(GraphicsPath path, CombineMode mode)
        {
            Graphics.SetClip(path, mode);
            SetClip();
        }

        /// <summary>
        /// Sets the current clip region.
        /// </summary>
        /// <param name="rect">The rectangle cpecifying the new clip region.</param>
        /// <param name="mode">The combaining mode.</param>
        public void SetClip(RectangleF rect, CombineMode mode)
        {
            Graphics.SetClip(rect, mode);
            SetClip();
        }

        /// <summary>
        /// Sets the current clip region.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="mode">The combining mode.</param>
        public void SetClip(Region region, CombineMode mode)
        {
            Graphics.SetClip(region, mode);
            SetClip();
        }

        /// <summary>
        /// Updates the clip region of this Graphics object to exclude the area specified by a Rectangle structure.
        /// </summary>
        /// <param name="rect">Rectangle structure that specifies the rectangle to exclude from the clip region.</param>
        public void ExcludeClip(System.Drawing.Rectangle rect)
        {
            Graphics.ExcludeClip(rect);
            SetClip();
        }

        /// <summary>
        /// Updates the clip region of this Graphics object to exclude the area specified by a Region object.
        /// </summary>
        /// <param name="region">Region object that specifies the region to exclude from the clip region.</param>
        public void ExcludeClip(Region region)
        {
            Graphics.ExcludeClip(region);
            SetClip();
        }

        /// <summary>
        /// Updates the clip region of this Graphics object to the intersection of the current clip region and the specified RectangleF structure.
        /// </summary>
        /// <param name="rect">RectangleF structure to intersect with the current clip region.</param>
        public void IntersectClip(RectangleF rect)
        {
            Graphics.IntersectClip(rect);
            SetClip();
        }

        /// <summary>
        /// Updates the clip region of this Graphics object to the intersection of the current clip region and the specified Region object.
        /// </summary>
        /// <param name="region">Region object to intersect with the current region.</param>
        public void IntersectClip(Region region)
        {
            Graphics.IntersectClip(region);
            SetClip();
        }

        /// <summary>
        /// Transforms points.
        /// </summary>
        /// <param name="destSpace">Destination space.</param>
        /// <param name="srcSpace">Source space.</param>
        /// <param name="pts">Array of points.</param>
        public void TransformPoints(CoordinateSpace destSpace, CoordinateSpace srcSpace, PointF[] pts)
        {
            Graphics.TransformPoints(destSpace, srcSpace, pts);
        }

        /// <summary>
        /// Sets the current rendering origin.
        /// </summary>
        /// <param name="origin">The origin.</param>
        public void SetRenderingOrigin(Point origin)
        {
            Graphics.RenderingOrigin = origin;
            Graphics.TranslateTransform(-origin.X, -origin.Y);
        }

        /// <summary>
        /// Sets the specified transformation matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        public void SetTransform(Matrix matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            Transform = matrix;
        }

        /// <summary>
        /// Performs translate transformation of the graphics.
        /// </summary>
        /// <param name="dx">The offset by the x coordinate.</param>
        /// <param name="dy">The offset by the y coordinate.</param>
        /// <param name="order">The order of the matrix operations.</param>
        public void TranslateTransform(float dx, float dy, MatrixOrder order)
        {
            Graphics.TranslateTransform(dx, dy, order);
        }

        /// <summary>
        /// Is called when the metafile parsing have been started.
        /// </summary>
        public void BeforeStart()
        {
            m_startState = Graphics.Save();

            // Set bounds for the metafile.
            if (m_bounds != null)
            {
                Graphics.MultiplyTransform(m_bounds);
            }
        }

        /// <summary>
        /// Is called when the metafile is at the end.
        /// </summary>
        public void BeforeEnd()
        {
            // Restore the state saved by primitives.
            // Bound was set so we need restore state.
            Graphics.Restore(m_startState);
        }

        /// <summary>
        /// Raises when error occured during metafile parsing.
        /// </summary>
        /// <param name="ex">The exception thrown on error.</param>
        public void OnError(Exception ex)
        {
            BeforeEnd();
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Performs application-defined tasks associated with freeing,
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            m_graphics = null;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Sets location or bounds for metafile object.
        /// </summary>
        /// <param name="location">Location of the metafile.</param>
        /// <param name="size">Size of the metafile.</param>
        internal void SetBounds(PointF location, SizeF size)
        {
            m_bounds = new Matrix();

            if (!size.IsEmpty)
            {
                m_bounds.Scale(size.Width, size.Height);
            }

            if (!location.IsEmpty)
            {
                m_bounds.Translate(location.X, -location.Y);
            }
        }

        /// <summary>
        /// Sets clip region.
        /// </summary>
        private void SetClip()
        {
            m_stateChanged = true;
        }

        /// <summary>
        /// Called when we need to draw a primitive.
        /// </summary>
        private void OnDrawPrimitive()
        {
            if (m_stateChanged)
            {
                // Set new clip path and transformation.
                Graphics.Save();
                
                // Set state to unchanged.
                m_stateChanged = false;
            }
        }

        /// <summary>
        /// Called when the graphics state was changed.
        /// </summary>
        private void OnChangeState()
        {
            m_stateChanged = true;
        }

        /// <summary>
        /// Draws the cap.
        /// </summary>
        /// <param name="cap">The cap.</param>
        /// <param name="points">The points.</param>
        /// <param name="startPointIndex">The Start point index.</param>
        /// <param name="endPointIndex">The end point index.</param>
        /// <param name="width">The width.</param>
        /// <param name="brush">The brush.</param>
        private void DrawCap(System.Drawing.Drawing2D.LineCap cap, PointF[] points, int startPointIndex, int endPointIndex, float width, Brush brush)
        {
            switch (cap)
            {
                case System.Drawing.Drawing2D.LineCap.Triangle:
                    PointF ePoint = points[endPointIndex];
                    PointF bePoint = points[startPointIndex];

                    float x = ePoint.X;
                    float y = ePoint.Y;

                    float dx = x - bePoint.X;
                    float dy = y - bePoint.Y;

                    float l = (float)Math.Sqrt(dx * dx + dy * dy);
                    float nx = dx / l;
                    float ny = dy / l;

                    float nyw = ny * width;
                    float nxw = nx * width;

                    float lX = x - nyw;
                    float lY = y + nxw;

                    float rX = x + nyw;
                    float rY = y - nxw;

                    float cX = x + nxw;
                    float cY = y + nyw;

                    PointF p1 = new PointF(rX, rY);
                    PointF p2 = new PointF(lX, lY);
                    PointF p3 = new PointF(cX, cY);

                    PointF[] triangle = new PointF[] { p1, p3, p2 };
                    Graphics.FillPolygon(brush, triangle);
                    break;

                case System.Drawing.Drawing2D.LineCap.Round:
                    SizeF endCapSize = new SizeF(width, width);
                    RectangleF capRect = new RectangleF(points[endPointIndex], endCapSize);
                    Graphics.FillEllipse(brush, capRect);
                    break;
            }
        }

        /// <summary>
        /// Checks whether points is line or not. 
        /// </summary>
        /// <param name="points">Points to be check.</param>
        /// <returns>Is points line or not.</returns>
        private bool IsLine(PointF[] points)
        {
            int count = points.Length;
            float x = points[0].X;

            bool isLine = false;

            for (int i = 1; i < count; ++i)
            {
                isLine = false;
                if (x != points[i].X) break;
                isLine = true;
            }

            if (!isLine)
            {
                float y = points[0].Y;

                for (int i = 1; i < count; ++i)
                {
                    isLine = false;
                    if (y != points[i].Y) break;
                    isLine = true;
                }
            }

            if (!isLine)
            {
                float dx = points[1].X - points[0].X;
                float dy = points[1].Y - points[0].Y;
                float d = dy / dx;

                for (int i = 2; i < count; ++i)
                {
                    dx = points[i].X - points[i - 1].X;
                    dy = points[i].Y - points[i - 1].Y;
                    float currD = dy / dx;
                    isLine = false;
                    if (Math.Abs(d - currD) > float.Epsilon) break;
                    isLine = true;
                }
            }

            return isLine;
        }
        #endregion
    }
}