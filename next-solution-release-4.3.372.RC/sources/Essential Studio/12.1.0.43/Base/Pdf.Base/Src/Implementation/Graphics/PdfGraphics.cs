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
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Syncfusion.Pdf.ColorSpace;
using Syncfusion.Pdf.Graphics.Fonts;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Primitives;


#if !SILVERLIGHT && !NETFX_CORE &&!WP
using System.Drawing.Imaging;
#endif

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// The class representing a graphics context of the objects.
    /// It's used for performing simple graphics operations.
    /// </summary>
    //[System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
    public sealed class PdfGraphics
    {
        #region Constants
        /// <summary>
        /// Specifies the mask of path type values.
        /// </summary>
        private const int PathTypesValuesMask = 0xf;
        #endregion

        #region Fields
        /// <summary>
        /// Check whether it is an emf call
        /// </summary>
        internal bool m_isEMF = false;
        /// <summary>
        /// Check whether it is an emf call
        /// </summary>
        internal bool m_isBaselineFormat = true;
        /// <summary>
        internal float m_DpiY = 0;
        /// <summary>
        /// Represents the Stream writer object.
        /// </summary>
        private PdfStreamWriter m_streamWriter;
        /// <summary>
        /// Represents the Resource container for the graphics.
        /// </summary>
        private GetResources m_getResources;
        /// <summary>
        /// Represents the Size of the canvas.
        /// </summary>
        private SizeF m_canvasSize;
        /// <summary>
        /// Represents the size of the canvas reduced by margins and templates.
        /// </summary>
        internal RectangleF m_clipBounds;
        /// <summary>
        /// Represents the state, whether it is saved or not.
        /// </summary>
        internal bool m_bStateSaved;
        /// <summary>
        /// Represents the Current pen.
        /// </summary>
        private PdfPen m_currentPen;
        /// <summary>
        /// Represents the Current brush.
        /// </summary>
        private PdfBrush m_currentBrush;
        /// <summary>
        /// Represents the Current font.
        /// </summary>
        private PdfFont m_currentFont;
        /// <summary>
        /// Represents the Current color space.
        /// </summary>
        private PdfColorSpace m_currentColorSpace = PdfColorSpace.RGB;
        /// <summary>
        /// Indicates whether color space was initialized.
        /// </summary>
        private bool m_bCSInitialized;
        private bool m_CIEColors = false;
        /// <summary>
        /// Stack of the graphics states.
        /// </summary>
        private Stack<PdfGraphicsState> m_graphicsState;
        /// <summary>
        /// The transformation matrix monitoring all changes with CTM.
        /// </summary>
        private PdfTransformationMatrix m_matrix;
        /// <summary>
        /// Stores previous rendering mode.
        /// </summary>
        private TextRenderingMode m_previousTextRenderingMode = TextRenderingMode.Fill;
        /// <summary>
        /// Previous character spacing value or 0.
        /// </summary>
        private float m_previousCharacterSpacing = 0.0f;
        /// <summary>
        /// Previous word spacing value or 0.
        /// </summary>
        private float m_previousWordSpacing = 0.0f;
        /// <summary>
        /// The previously used text scaling value.
        /// </summary>
        private float m_previousTextScaling = 100.0f;
        /// <summary>
        /// Holds transparencies used in the graphics.
        /// </summary>
        private Dictionary<TransparencyData, PdfTransparency> m_trasparencies;
        /// <summary>
        /// Current string format.
        /// </summary>
        private PdfStringFormat m_currentStringFormat;
        /// <summary>
        /// Internal variable to store layer on which this graphics lays.
        /// </summary>
        private PdfPageLayer m_layer;
        /// <summary>
        /// Internal variable to store collection of automatic fields.
        /// </summary>
        private PdfAutomaticFieldInfoCollection m_automaticFields = null;
        /// <summary>
        /// Internal variable to store layout result after drawing string.
        /// </summary>
        private PdfStringLayoutResult m_stringLayoutResult;
        /// <summary>
        /// Internal variable to store position of split.
        /// </summary>
        private float m_split;
        /// <summary>
        /// Indicates whether the object had trasparency.
        /// </summary>
        private static bool m_transparencyObject = false;
        /// <summary>
        /// Helps to lock s_mask to avoid race conditions.
        /// </summary>
        private static object s_transparencyLock = new object();
        
        #endregion

        #region Properties
        /// <summary>
        /// Gets the size of the canvas.
        /// </summary>
        /// <remarks>Usually, this value is equal to the size of the object this graphics belongs to.</remarks>
        public SizeF Size
        {
            get
            {
                return m_canvasSize;
            }
        }

        /// <summary>
        /// Gets the size of the canvas reduced by margins and page templates.
        /// </summary>
        /// <remarks>It indicates a size of the canvas reduced by margins and template dimensions.
        /// This value doesn't change when any custom clip is set.</remarks>
        public SizeF ClientSize
        {
            get
            {
                return m_clipBounds.Size;
            }
        }

        /// <summary>
        /// Gets or sets the current color space.
        /// </summary>
        /// <remarks>The value change of this property has impact on the objects
        /// which will be drawn after the change.</remarks>
        public PdfColorSpace ColorSpace
        {
            get
            {
                return m_currentColorSpace;
            }
            set
            {
                m_currentColorSpace = value;
            }
        }

        /// <summary>
        /// Gets the stream writer.
        /// </summary>
        internal PdfStreamWriter StreamWriter
        {
            get
            {
                return m_streamWriter;
            }
        }

        /// <summary>
        /// Gets the transformation matrix reflecting current transformation.
        /// </summary>
        internal PdfTransformationMatrix Matrix
        {
            get
            {
                if (m_matrix == null)
                {
                    m_matrix = new PdfTransformationMatrix();
                }

                return m_matrix;
            }
        }

        /// <summary>
        /// Gets the layer for the graphics, if exists.
        /// </summary>
        /// <value>The layer.</value>
        internal PdfPageLayer Layer
        {
            get
            {
                return m_layer;
            }
        }

        /// <summary>
        /// Gets the page for this graphics, if exists.
        /// </summary>
        /// <value>The page.</value>
        internal PdfPageBase Page
        {
            get
            {
                return m_layer.Page;
            }
        }

        /// <summary>
        /// Gets the automatic fields.
        /// </summary>
        /// <value>The automatic fields.</value>
        internal PdfAutomaticFieldInfoCollection AutomaticFields
        {
            get
            {
                if (m_automaticFields == null)
                {
                    m_automaticFields = new PdfAutomaticFieldInfoCollection();
                }

                return m_automaticFields;
            }
        }

        /// <summary>
        /// Returns the result after drawing string.
        /// </summary>
        internal PdfStringLayoutResult StringLayoutResult
        {
            get
            {
                return m_stringLayoutResult;
            }
        }

        /// <summary>
        /// Gets or sets the split before being processed by Text and Image region managers.
        /// </summary>
        internal float Split
        {
            get
            {
                return m_split;
            }
            set
            {
                m_split = value;
            }
        }

        /// <summary>
        /// Gets the transparency object value
        /// </summary>
        internal static bool TransparencyObject
        {
            get
            {
                return m_transparencyObject;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfGraphics"/> class.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="resources">The resources.</param>
        /// <param name="writer">The stream writer of the current layer.</param>
        internal PdfGraphics(SizeF size, GetResources resources, PdfStreamWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (resources == null)
                throw new ArgumentNullException("resources");

            m_streamWriter = writer;
            m_getResources = resources;
            m_canvasSize = size;
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfGraphics"/> class.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="resources">The resources.</param>
        /// <param name="stream">The stream of the current layer.</param>
        internal PdfGraphics(SizeF size, GetResources resources, PdfStream stream)
            : this(size, resources, new PdfStreamWriter(stream))
        {
        }
        #endregion

        #region Public methods

        #region DrawLine overloads
        /// <summary>
        /// Draws a line.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="point1">The point1.</param>
        /// <param name="point2">The point2.</param>
        public void DrawLine(PdfPen pen, PointF point1, PointF point2)
        {
            DrawLine(pen, point1.X, point1.Y, point2.X, point2.Y);
        }

        /// <summary>
        /// Draws a line.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        public void DrawLine(PdfPen pen, float x1, float y1, float x2, float y2)
        {
            StateControl(pen, null, null);
            CapControl(pen, x1, y1, x2, y2);
            CapControl(pen, x2, y2, x1, y1);

            PdfStreamWriter sw = StreamWriter;

            sw.BeginPath(x1, y1);
            sw.AppendLineSegment(x2, y2);
            sw.StrokePath();

            m_getResources().RequireProcSet(ProcedureSets.PDF);
        }
        #endregion

        #region DrawRectangle overloads
        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="rectangle">The rectangle.</param>
        public void DrawRectangle(PdfPen pen, RectangleF rectangle)
        {
            DrawRectangle(pen, null, rectangle);
        }

        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void DrawRectangle(PdfPen pen, float x, float y, float width, float height)
        {
            DrawRectangle(pen, null, x, y, width, height);
        }

        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="rectangle">The rectangle.</param>
        public void DrawRectangle(PdfBrush brush, RectangleF rectangle)
        {
            DrawRectangle(null, brush, rectangle);
        }

        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void DrawRectangle(PdfBrush brush, float x, float y, float width, float height)
        {
            DrawRectangle(null, brush, x, y, width, height);
        }

        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="rectangle">The rectangle.</param>
        public void DrawRectangle(PdfPen pen, PdfBrush brush, RectangleF rectangle)
        {
            DrawRectangle(pen, brush, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void DrawRectangle(PdfPen pen, PdfBrush brush, float x, float y, float width, float height)
        {
            if ((brush is PdfSolidBrush) && (brush as PdfSolidBrush).Color.A == 0)
            {
                lock (s_transparencyLock)
                {
                    m_transparencyObject = true;
                }
            }
            if ((brush is PdfTilingBrush) == true)
            {
                m_bCSInitialized = false;

                float xOffset = m_matrix.OffsetX + x;
                float yOffset;
                if (this.Layer != null && this.Layer.Page != null)
                {
                    yOffset = (this.Layer.Page.Size.Height - m_matrix.OffsetY) + y;
                }
                else
                {
                    yOffset = (this.ClientSize.Height - m_matrix.OffsetY) + y;
                }
                (brush as PdfTilingBrush).Location = new PointF(xOffset, yOffset);

                (brush as PdfTilingBrush).Graphics.ColorSpace = ColorSpace;
            }
            else if ((brush is PdfGradientBrush) == true)
            {
                m_bCSInitialized = false;
                (brush as PdfGradientBrush).ColorSpace = ColorSpace;
            }
            if (brush is PdfSolidBrush && (brush as PdfSolidBrush).Color.IsEmpty)
            {
                brush = null;
            }

            StateControl(pen, brush, null);
            StreamWriter.AppendRectangle(x, y, width, height);
            DrawPath(pen, brush, false);
        }
        #endregion

        #region DrawEllipse overloads
        /// <summary>
        /// Draws an ellipse.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="rectangle">The rectangle.</param>
        public void DrawEllipse(PdfPen pen, RectangleF rectangle)
        {
            DrawEllipse(pen, null, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        /// <summary>
        /// Draws an ellipse.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void DrawEllipse(PdfPen pen, float x, float y, float width, float height)
        {
            DrawEllipse(pen, null, x, y, width, height);
        }

        /// <summary>
        /// Draws an ellipse.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="rectangle">The rectangle.</param>
        public void DrawEllipse(PdfBrush brush, RectangleF rectangle)
        {
            DrawEllipse(null, brush, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        /// <summary>
        /// Draws an ellipse.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void DrawEllipse(PdfBrush brush, float x, float y, float width, float height)
        {
            DrawEllipse(null, brush, x, y, width, height);
        }

        /// <summary>
        /// Draws an ellipse.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="rectangle">The rectangle.</param>
        public void DrawEllipse(PdfPen pen, PdfBrush brush, RectangleF rectangle)
        {
            DrawEllipse(pen, brush, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        /// <summary>
        /// Draws an ellipse.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void DrawEllipse(PdfPen pen, PdfBrush brush, float x, float y, float width, float height)
        {
            if ((brush is PdfTilingBrush) == true)
            {
                m_bCSInitialized = true;
                float xOffset = m_matrix.OffsetX + x;
                float yOffset;
                if (this.Layer != null && this.Layer.Page != null)
                {
                    yOffset = (this.Layer.Page.Size.Height - m_matrix.OffsetY) + y;
                }
                else
                {
                    yOffset = (this.ClientSize.Height - m_matrix.OffsetY) + y;
                }
                (brush as PdfTilingBrush).Location = new PointF(xOffset, yOffset);
                (brush as PdfTilingBrush).Graphics.ColorSpace = ColorSpace;
            }
            StateControl(pen, brush, null);
            ConstructArcPath(x, y, x + width, y + height, 0, 360);
            DrawPath(pen, brush, true);
        }
        #endregion

        #region DrawArc overloads
        /// <summary>
        /// Draws an arc.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        public void DrawArc(PdfPen pen, RectangleF rectangle, float startAngle, float sweepAngle)
        {
# if SILVERLIGHT || NETFX_CORE || WP
            DrawArc(pen, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, startAngle, sweepAngle);
# else
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rectangle, startAngle, sweepAngle);
            PointF[] points = path.PathPoints;
            byte[] pathTypes = path.PathTypes;            
            PdfPath drawPath = new PdfPath(pen, points, pathTypes);
            this.DrawPath(pen, drawPath);
            path.Dispose();
# endif
        }

        /// <summary>
        /// Draws an arc.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        public void DrawArc(PdfPen pen, float x, float y, float width, float height,
            float startAngle, float sweepAngle)
        {
            if (sweepAngle != 0) // The angle, what removes the arc.
            {
                StateControl(pen, null, null);
                ConstructArcPath(x, y, x + width, y + height, startAngle, sweepAngle);
                DrawPath(pen, null, false);
            }
        }
        #endregion

        #region DrawPie overloads
        /// <summary>
        /// Draws a pie.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        public void DrawPie(PdfPen pen, RectangleF rectangle, float startAngle, float sweepAngle)
        {
            DrawPie(pen, null, rectangle.X, rectangle.Y,
                rectangle.Width, rectangle.Height, startAngle, sweepAngle);
        }

        /// <summary>
        /// Draws a pie.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        public void DrawPie(PdfPen pen, float x, float y, float width, float height,
            float startAngle, float sweepAngle)
        {
            DrawPie(pen, null, x, y, width, height, startAngle, sweepAngle);
        }

        /// <summary>
        /// Draws a pie.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        public void DrawPie(PdfBrush brush, RectangleF rectangle, float startAngle, float sweepAngle)
        {
# if SILVERLIGHT || NETFX_CORE || WP
           DrawPie(null, brush, rectangle.X, rectangle.Y,
                rectangle.Width, rectangle.Height, startAngle, sweepAngle);
# else
            GraphicsPath path = new GraphicsPath();
            path.AddPie(new Rectangle((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height), startAngle, sweepAngle);
            PointF[] points = path.PathPoints;
            byte[] pathTypes = path.PathTypes;
            PdfPath drawPath = new PdfPath(points, pathTypes);
            this.DrawPath(brush, drawPath);
            path.Dispose();
# endif
        }

        /// <summary>
        /// Draws a pie.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        public void DrawPie(PdfBrush brush, float x, float y, float width, float height,
            float startAngle, float sweepAngle)
        {
            DrawPie(null, brush, x, y, width, height, startAngle, sweepAngle);
        }

        /// <summary>
        /// Draws a pie.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        public void DrawPie(PdfPen pen, PdfBrush brush, RectangleF rectangle,
            float startAngle, float sweepAngle)
        {
            DrawPie(pen, brush, rectangle.X, rectangle.Y,
                rectangle.Width, rectangle.Height, startAngle, sweepAngle);
        }

        /// <summary>
        /// Draws a pie.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        public void DrawPie(PdfPen pen, PdfBrush brush, float x, float y, float width, float height,
            float startAngle, float sweepAngle)
        {
            if (sweepAngle != 0) // The angle, what removes the pie.
            {
                if ((brush is PdfTilingBrush) == true)
                {
                    m_bCSInitialized = false;
                    float xOffset = m_matrix.OffsetX + x;
                    float yOffset;
                    if (this.Layer != null && this.Layer.Page != null)
                    {
                        yOffset = (this.Layer.Page.Size.Height - m_matrix.OffsetY) + y;
                    }
                    else
                    {
                        yOffset = (this.ClientSize.Height - m_matrix.OffsetY) + y;
                    }
                    (brush as PdfTilingBrush).Location = new PointF(xOffset, yOffset);
                    (brush as PdfTilingBrush).Graphics.ColorSpace = ColorSpace;
                }
                else if ((brush is PdfGradientBrush) == true)
                {
                    m_bCSInitialized = false;
                    (brush as PdfGradientBrush).ColorSpace = ColorSpace;
                }
                StateControl(pen, brush, null);
                ConstructArcPath(x, y, x + width, y + height, startAngle, sweepAngle);
                m_streamWriter.AppendLineSegment(x + width / 2, y + height / 2);
                DrawPath(pen, brush, true);
            }
        }
        #endregion

        #region DrawPolygon overloads
        /// <summary>
        /// Draws a polygon.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="points">The points.</param>
        public void DrawPolygon(PdfPen pen, PointF[] points)
        {
            DrawPolygon(pen, null, points);
        }

        /// <summary>
        /// Draws a polygon.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="points">The points.</param>
        public void DrawPolygon(PdfBrush brush, PointF[] points)
        {
            DrawPolygon(null, brush, points);
        }

        /// <summary>
        /// Draws a polygon.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="points">The points.</param>
        public void DrawPolygon(PdfPen pen, PdfBrush brush, PointF[] points)
        {
            if ((brush is PdfTilingBrush) == true)
            {
                m_bCSInitialized = false;
                (brush as PdfTilingBrush).Graphics.ColorSpace = ColorSpace;
            }
            else if ((brush is PdfGradientBrush) == true)
            {
                m_bCSInitialized = false;
                (brush as PdfGradientBrush).ColorSpace = ColorSpace;
            }
            int count = points.Length;
            if (count <= 0) return;

            StateControl(pen, brush, null);

            // Buld the polygon.
            m_streamWriter.BeginPath(points[0]);

            for (int i = 1; i < count; ++i)
            {
                m_streamWriter.AppendLineSegment(points[i]);
            }

            DrawPath(pen, brush, true);
        }
        #endregion

        #region DrawBezier overloads
        /// <summary>
        /// Draws a bezier curve.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="startPoint">The start point.</param>
        /// <param name="firstControlPoint">The first control point.</param>
        /// <param name="secondControlPoint">The second control point.</param>
        /// <param name="endPoint">The end point.</param>
        public void DrawBezier(PdfPen pen, PointF startPoint, PointF firstControlPoint,
            PointF secondControlPoint, PointF endPoint)
        {
            DrawBezier(pen, startPoint.X, startPoint.Y, firstControlPoint.X, firstControlPoint.Y,
                secondControlPoint.X, secondControlPoint.Y, endPoint.X, endPoint.Y);
        }

        /// <summary>
        /// Draws a bezier curve.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="startPointX">The start point X.</param>
        /// <param name="startPointY">The start point Y.</param>
        /// <param name="firstControlPointX">The first control point X.</param>
        /// <param name="firstControlPointY">The first control point Y.</param>
        /// <param name="secondControlPointX">The second control point X.</param>
        /// <param name="secondControlPointY">The second control point Y.</param>
        /// <param name="endPointX">The end point X.</param>
        /// <param name="endPointY">The end point Y.</param>
        public void DrawBezier(PdfPen pen, float startPointX, float startPointY,
            float firstControlPointX, float firstControlPointY, float secondControlPointX,
            float secondControlPointY, float endPointX, float endPointY)
        {
            StateControl(pen, null, null);
            CapControl(pen, secondControlPointX, secondControlPointY, endPointX, endPointY);
            CapControl(pen, firstControlPointX, firstControlPointY, secondControlPointX, startPointY);

            PdfStreamWriter sw = StreamWriter;

            sw.BeginPath(startPointX, startPointY);
            sw.AppendBezierSegment(firstControlPointX, firstControlPointY,
                secondControlPointX, secondControlPointY, endPointX, endPointY);
            sw.StrokePath();
        }
        #endregion

        #region DrawPath overloads
        /// <summary>
        /// Draws a path.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="path">The path.</param>
        public void DrawPath(PdfPen pen, PdfPath path)
        {
            DrawPath(pen, null, path);
        }

        /// <summary>
        /// Draws a path.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="path">The path.</param>
        public void DrawPath(PdfBrush brush, PdfPath path)
        {
            DrawPath(null, brush, path);
        }

        /// <summary>
        /// Draws a path.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="path">The path.</param>
        public void DrawPath(PdfPen pen, PdfBrush brush, PdfPath path)
        {
            if ((brush is PdfTilingBrush) == true)
            {
                m_bCSInitialized = false;
                (brush as PdfTilingBrush).Graphics.ColorSpace = ColorSpace;
            }
            else if ((brush is PdfGradientBrush) == true)
            {
                m_bCSInitialized = false;
                (brush as PdfGradientBrush).ColorSpace = ColorSpace;
            }
            StateControl(pen, brush, null);
            BuildUpPath(path);
            DrawPath(pen, brush, path.FillMode, false);
        }
        #endregion

        #region DrawImage overloads
        /// <summary>
        /// Draws an image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="point">The point.</param>
        public void DrawImage(PdfImage image, PointF point)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            DrawImage(image, point.X, point.Y);
        }

        /// <summary>
        /// Draws an image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void DrawImage(PdfImage image, float x, float y)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            SizeF size = image.PhysicalDimension;
            DrawImage(image, x, y, size.Width, size.Height);
        }

        /// <summary>
        /// Draws an image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="rectangle">The rectangle.</param>
        public void DrawImage(PdfImage image, RectangleF rectangle)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            DrawImage(image, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        /// <summary>
        /// Draws an image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="point">The point.</param>
        /// <param name="size">The size.</param>
        public void DrawImage(PdfImage image, PointF point, SizeF size)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            DrawImage(image, point.X, point.Y, size.Width, size.Height);
        }

        /// <summary>
        /// Draws an image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void DrawImage(PdfImage image, float x, float y, float width, float height)
        {
            bool tagged = false;
            if (image == null)
                throw new ArgumentNullException("image");
			
			if (this.ClientSize.Height < 0)
                y += this.ClientSize.Height;

            image.Save();

# if !SILVERLIGHT

            // Update StructTree for tagged PDF.
            if (Layer != null && Page != null && Page is PdfPage)
            {
                PdfSection parentSection = (Page as PdfPage).Section;
                if ((parentSection.ParentDocument is PdfDocument) && parentSection.ParentDocument.FileStructure.TaggedPdf)
                {
                    tagged = true;
                    PdfStructTreeRoot structTreeRoot = PdfCrossTable.Dereference(parentSection.ParentDocument.Catalog[DictionaryProperties.StructTreeRoot]) as PdfStructTreeRoot;
                    if (structTreeRoot != null)
                    {
                        int id = structTreeRoot.Add("Figure", "Image", Page, RectangleF.Empty);
                        m_streamWriter.WriteTag(string.Format("/{0} <</MCID {1} >>BDC", "Figure", id));
                    }
                }
            }
            else
            {
                PdfStructTreeRoot structTreeRoot = PdfCatalog.StructTreeRoot;
                if (structTreeRoot != null)
                {
                    tagged = true;
                    int id = structTreeRoot.Add("Figure", "Image", RectangleF.Empty);
                    StreamWriter.WriteTag(string.Format("/{0} <</MCID {1} >>BDC", "Figure", id));
                }
            }

# endif

            // Save state.
            PdfGraphicsState state = Save();

            PdfTransformationMatrix matrix = new PdfTransformationMatrix();
            GetTranslateTransform(x, y + height, matrix);

#if !SILVERLIGHT && !NETFX_CORE && !WP
            if (image.InternalImage is Metafile)
            {
                // COMMENT: Here we calculate the ratio of point to pixel because metafile was rendered in pixels
                // and the size of PdfTemplate is in pixels too.
                float dx = width / image.Width;
                float dy = height / image.Height;

                GetScaleTransform(dx, dy, matrix);
            }
            else
#endif
            {
                GetScaleTransform(width, height, matrix);
            }

            m_streamWriter.ModifyCTM(matrix);

            // Output template.
            PdfResources resources = m_getResources();
            PdfName name = resources.GetName(image);
            m_streamWriter.ExecuteObject(name);
#if !SILVERLIGHT &&!NETFX_CORE && !WP
            lock (s_transparencyLock)
            {
                if (image.SoftMask)
                    m_transparencyObject = true;
                if (m_transparencyObject && Layer != null && !Page.Dictionary.ContainsKey(DictionaryProperties.Group))
                {
                    SetTransparencyGroup(Page);
                }
            }
#endif
            // Restore state.
            Restore(state);

            if (tagged)
                m_streamWriter.WriteTag("EMC");

            m_getResources().RequireProcSet(ProcedureSets.ImageB);
            m_getResources().RequireProcSet(ProcedureSets.ImageC);
            m_getResources().RequireProcSet(ProcedureSets.ImageI);
            m_getResources().RequireProcSet(ProcedureSets.Text);

        }
        internal void SetTransparencyGroup(PdfPageBase page)
        {
            PdfDictionary group = new PdfDictionary();
            group.SetName(DictionaryProperties.CS, "DeviceRGB");
            group.SetBoolean(DictionaryProperties.K, false);
            group.SetName(DictionaryProperties.S, "Transparency");
            group.SetBoolean(DictionaryProperties.I, false);
            page.Dictionary[DictionaryProperties.Group] = group;
        }
        #endregion

        #region DrawString overloads
        /// <summary>
        /// Draws the specified text string at the specified location
        /// with the specified Brush and Font objects. 
        /// </summary>
        /// <param name="s">The text string.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="point">The location point.</param>
        public void DrawString(string s, PdfFont font, PdfBrush brush, PointF point)
        {
            DrawString(s, font, brush, point, null);
        }

        /// <summary>
        /// Draws the specified text string at the specified location
        /// with the specified Brush and Font objects. 
        /// </summary>
        /// <param name="s">The text string.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="point">The point.</param>
        /// <param name="format">The text string format.</param>
        public void DrawString(string s, PdfFont font, PdfBrush brush,
            PointF point, PdfStringFormat format)
        {
            DrawString(s, font, brush, point.X, point.Y, format);
        }

        /// <summary>
        /// Draws the specified text string at the specified location
        /// with the specified Brush and Font objects. 
        /// </summary>
        /// <param name="s">The text string.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void DrawString(string s, PdfFont font, PdfBrush brush, float x, float y)
        {
            DrawString(s, font, brush, x, y, null);
        }

        /// <summary>
        /// Draws the specified text string at the specified location
        /// with the specified Brush and Font objects. 
        /// </summary>
        /// <param name="s">The text string.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="format">The text string format.</param>
        public void DrawString(string s, PdfFont font, PdfBrush brush,
            float x, float y, PdfStringFormat format)
        {
            DrawString(s, font, null, brush, x, y, format);
        }

        /// <summary>
        /// Draws the specified text string at the specified location
        /// with the specified Brush and Font objects. 
        /// </summary>
        /// <param name="s">The text string.</param>
        /// <param name="font">The font.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="point">The location point.</param>
        public void DrawString(string s, PdfFont font, PdfPen pen, PointF point)
        {
            DrawString(s, font, pen, point, null);
        }

        /// <summary>
        /// Draws the specified text string at the specified location
        /// with the specified Brush and Font objects. 
        /// </summary>
        /// <param name="s">The text string.</param>
        /// <param name="font">The font.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="point">The point.</param>
        /// <param name="format">The text string format.</param>
        public void DrawString(string s, PdfFont font, PdfPen pen,
            PointF point, PdfStringFormat format)
        {
            DrawString(s, font, pen, point.X, point.Y, format);
        }

        /// <summary>
        /// Draws the specified text string at the specified location
        /// with the specified Brush and Font objects. 
        /// </summary>
        /// <param name="s">The text string.</param>
        /// <param name="font">The font.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void DrawString(string s, PdfFont font, PdfPen pen, float x, float y)
        {
            DrawString(s, font, pen, x, y, null);
        }

        /// <summary>
        /// Draws the specified text string at the specified location
        /// with the specified Brush and Font objects. 
        /// </summary>
        /// <param name="s">The text string.</param>
        /// <param name="font">The font.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="format">The text string format.</param>
        public void DrawString(string s, PdfFont font, PdfPen pen,
            float x, float y, PdfStringFormat format)
        {
            DrawString(s, font, pen, null, x, y, format);
        }

        /// <summary>
        /// Draws the specified text string at the specified location
        /// with the specified Brush and Font objects. 
        /// </summary>
        /// <param name="s">The text string.</param>
        /// <param name="font">The font.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="point">The location point.</param>
        public void DrawString(string s, PdfFont font, PdfPen pen, PdfBrush brush, PointF point)
        {
            DrawString(s, font, pen, brush, point, null);
        }

        /// <summary>
        /// Draws the specified text string at the specified location
        /// with the specified Brush and Font objects. 
        /// </summary>
        /// <param name="s">The text string.</param>
        /// <param name="font">The font.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="point">The point.</param>
        /// <param name="format">The text string format.</param>
        public void DrawString(string s, PdfFont font, PdfPen pen, PdfBrush brush,
            PointF point, PdfStringFormat format)
        {
            DrawString(s, font, pen, brush, point.X, point.Y, format);
        }

        /// <summary>
        /// Draws the specified text string at the specified location
        /// with the specified Brush and Font objects. 
        /// </summary>
        /// <param name="s">The text string.</param>
        /// <param name="font">The font.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="format">The text string format.</param>
        public void DrawString(string s, PdfFont font, PdfPen pen, PdfBrush brush,
            float x, float y, PdfStringFormat format)
        {
            RectangleF bounds = new RectangleF(x, y, 0, 0);
            DrawString(s, font, pen, brush, bounds, format);
        }

        /// <summary>
        /// Draws the specified text string at the specified location
        /// with the specified Brush and Font objects. 
        /// </summary>
        /// <param name="s">The text string.</param>
        /// <param name="font">The font.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void DrawString(string s, PdfFont font, PdfPen pen, PdfBrush brush, float x, float y)
        {
            DrawString(s, font, pen, brush, x, y, null);
        }

        /// <summary>
        /// Draws the specified text string at the specified location and size
        /// with the specified Brush and Font objects. 
        /// </summary>
        /// <param name="s">The text string.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="layoutRectangle">RectangleF structure that specifies the bounds of the drawn text.</param>
        public void DrawString(string s, PdfFont font, PdfBrush brush, RectangleF layoutRectangle)
        {
            DrawString(s, font, brush, layoutRectangle, null);
        }

        /// <summary>
        /// Draws the specified text string at the specified location and size
        /// with the specified Brush and Font objects. 
        /// </summary>
        /// <param name="s">The text string.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="layoutRectangle">RectangleF structure that specifies the bounds of the drawn text.</param>
        /// <param name="format">The text string format.</param>
        public void DrawString(string s, PdfFont font, PdfBrush brush,
            RectangleF layoutRectangle, PdfStringFormat format)
        {
            DrawString(s, font, null, brush, layoutRectangle, format);
        }

        /// <summary>
        /// Draws the specified text string at the specified location and size
        /// with the specified Pen and Font objects. 
        /// </summary>
        /// <param name="s">The text string.</param>
        /// <param name="font">The font.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="layoutRectangle">RectangleF structure that specifies the bounds of the drawn text.</param>
        public void DrawString(string s, PdfFont font, PdfPen pen, RectangleF layoutRectangle)
        {
            DrawString(s, font, pen, layoutRectangle, null);
        }

        /// <summary>
        /// Draws the specified text string at the specified location and size
        /// with the specified Pen and Font objects. 
        /// </summary>
        /// <param name="s">The text string.</param>
        /// <param name="font">The font.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="layoutRectangle">RectangleF structure that specifies the bounds of the drawn text.</param>
        /// <param name="format">The text string format.</param>
        public void DrawString(string s, PdfFont font, PdfPen pen,
            RectangleF layoutRectangle, PdfStringFormat format)
        {
            DrawString(s, font, pen, null, layoutRectangle, format);
        }

        /// <summary>
        /// Draws the specified text string at the specified location and size
        /// with the specified Pen, Brush and Font objects. 
        /// </summary>
        /// <param name="s">The text string.</param>
        /// <param name="font">The font.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="layoutRectangle">RectangleF structure that specifies the bounds of the drawn text.</param>
        /// <param name="format">The text string format.</param>
        public void DrawString(string s, PdfFont font, PdfPen pen,
            PdfBrush brush, RectangleF layoutRectangle, PdfStringFormat format)
        {
            if (s == null)
                throw new ArgumentNullException("s");

            if (font == null)
                throw new ArgumentNullException("font");

#if !SILVERLIGHT && !NETFX_CORE && !WP
            s = NormalizeText(font, s);
#endif
            PdfStringLayouter layouter = new PdfStringLayouter();
            PdfStringLayoutResult result = layouter.Layout(s, font, format, layoutRectangle.Size);

            if (!result.Empty)
            {
                RectangleF rect = CheckCorrectLayoutRectangle(result.ActualSize, layoutRectangle.X, layoutRectangle.Y, format);
                if (layoutRectangle.Width <= 0)
                {
                    layoutRectangle.X = rect.X;
                    layoutRectangle.Width = rect.Width;
                }
                if (layoutRectangle.Height <= 0)
                {
                    layoutRectangle.Y = rect.Y;
                    layoutRectangle.Height = rect.Height;
                }

                if (this.ClientSize.Height < 0)
                    layoutRectangle.Y += this.ClientSize.Height;
                DrawStringLayoutResult(result, font, pen, brush, layoutRectangle, format);
            }

            m_getResources().RequireProcSet(ProcedureSets.Text);

            m_stringLayoutResult = result;
        }
        #endregion

        #region Coordinate manipulation
        /// <summary>
        /// Translates the coordinates by specified coordinates.
        /// </summary>
        /// <param name="offsetX">The X value by which to translate
        /// coordinate system.</param>
        /// <param name="offsetY">The Y value by which to translate
        /// coordinate system.</param>
        /// <property name="flag" value="Finished"/>
        public void TranslateTransform(float offsetX, float offsetY)
        {
            PdfTransformationMatrix matrix = new PdfTransformationMatrix();

            GetTranslateTransform(offsetX, offsetY, matrix);
            m_streamWriter.ModifyCTM(matrix);
            Matrix.Multiply(matrix);
        }

        /// <summary>
        /// Scales the coordinates by specified coordinates.
        /// </summary>
        /// <param name="scaleX">The value by which to scale coordinate
        /// system in the X axis direction.</param>
        /// <param name="scaleY">The value by which to scale coordinate
        /// system in the Y axis direction.</param>
        /// <property name="flag" value="Finished"/>
        public void ScaleTransform(float scaleX, float scaleY)
        {
            PdfTransformationMatrix matrix = new PdfTransformationMatrix();

            GetScaleTransform(scaleX, scaleY, matrix);
            m_streamWriter.ModifyCTM(matrix);
            Matrix.Multiply(matrix);
        }

        /// <summary>
        /// Rotates the coordinate system in counterclockwise direction.
        /// </summary>
        /// <param name="angle">The angle of the rotation (in degrees).</param>
        /// <property name="flag" value="Finished"/>
        public void RotateTransform(float angle)
        {
            PdfTransformationMatrix matrix = new PdfTransformationMatrix();

            GetRotateTransform(angle, matrix);
            m_streamWriter.ModifyCTM(matrix);
            Matrix.Multiply(matrix);
        }

        /// <summary>
        /// Skews the coordinate system axes.
        /// </summary>
        /// <param name="angleX">Skews the X axis by this angle (in
        /// degrees).</param>
        /// <param name="angleY">Skews the Y axis by this angle (in
        /// degrees).</param>
        /// <property name="flag" value="Finished"/>
        public void SkewTransform(float angleX, float angleY)
        {
            PdfTransformationMatrix matrix = new PdfTransformationMatrix();

            GetSkewTransform(angleX, angleY, matrix);
            m_streamWriter.ModifyCTM(matrix);
            Matrix.Multiply(matrix);
        }

        /// <summary>
        /// Multiplies the world transformation of this Graphics and specified the Matrix. 
        /// </summary>
        /// <param name="matrix">The matrix that multiplies the world transformation.</param>
        internal void MultiplyTransform(PdfTransformationMatrix matrix)
        {
            m_streamWriter.ModifyCTM(matrix);
        }
        #endregion

        #region DrawPdfTemplate
        /// <summary>
        /// Draws a template using its original size, at the specified location.
        /// </summary>
        /// <param name="template"><see cref="PdfTemplate"/> object.</param>
        /// <param name="location">Location of the template.</param>
        public void DrawPdfTemplate(PdfTemplate template, PointF location)
        {
            if (template == null)
                throw new ArgumentNullException("template");

            DrawPdfTemplate(template, location, template.Size);
        }

        /// <summary>
        /// Draws a template at the specified location and size.
        /// </summary>
        /// <param name="template"><see cref="PdfTemplate"/> object.</param>
        /// <param name="location">Location of the template.</param>
        /// <param name="size">Size of the template.</param>
        public void DrawPdfTemplate(PdfTemplate template, PointF location, SizeF size)
        {
            PdfCrossTable crossTable = null;

            if (m_layer != null)
            {
                bool optimizeMemory = false;

                if (Page is PdfLoadedPage)
                {
                    crossTable = (Page as PdfLoadedPage).Document.CrossTable;
                    optimizeMemory = (Page as PdfLoadedPage).Document.EnableMemoryOptimization;
                }
                else if (Page is PdfPage)
                {
                    crossTable = (Page as PdfPage).Section.ParentDocument.CrossTable;
                    optimizeMemory = (Page as PdfPage).Section.ParentDocument.EnableMemoryOptimization;
                }

                if (template.ReadOnly && optimizeMemory)
                    template.CloneResources(crossTable);
            }

            if (template == null)
                throw new ArgumentNullException("template");

            float scaleX = (template.Width > 0) ? size.Width / template.Width : 1;
            float scaleY = (template.Height > 0) ? size.Height / template.Height : 1;
            bool bNeedScale = !(scaleX == 1f && scaleY == 1f);
            if (m_layer != null && this.Page.Dictionary.ContainsKey(DictionaryProperties.CropBox) && this.Page.Dictionary.ContainsKey(DictionaryProperties.MediaBox))
            {
                PdfArray cropBox = this.Page.Dictionary[DictionaryProperties.CropBox] as PdfArray;
                PdfArray mediaBox = this.Page.Dictionary[DictionaryProperties.MediaBox] as PdfArray;
                float mediaXCoordinates = (mediaBox[0] as PdfNumber).FloatValue;
                float mediaYCoordinates = (mediaBox[1] as PdfNumber).FloatValue;
                float xCoordinate = (cropBox[0] as PdfNumber).FloatValue;
                float yCoordinate = (cropBox[3] as PdfNumber).FloatValue;
                if (xCoordinate > 0 && yCoordinate > 0 && mediaXCoordinates < 0 && mediaYCoordinates < 0)
                {
                    TranslateTransform(xCoordinate, -yCoordinate);
                    location.X = -xCoordinate;
                    location.Y = yCoordinate;
                }
            }
            // Save state.
            PdfGraphicsState state = Save();

            // Take into consideration that rect location is bottom/left.
            PdfTransformationMatrix matrix = new PdfTransformationMatrix();

            if (m_layer != null)
            {
                bool needTransformation = false;
                if (this.Page.Dictionary.ContainsKey(DictionaryProperties.CropBox) && this.Page.Dictionary.ContainsKey(DictionaryProperties.MediaBox))
                {
                    PdfArray cropBox = this.Page.Dictionary[DictionaryProperties.CropBox] as PdfArray;
                    PdfArray mediaBox = this.Page.Dictionary[DictionaryProperties.MediaBox] as PdfArray;
                    if (cropBox.ToRectangle() == mediaBox.ToRectangle())
                        needTransformation = true;
                }
                if (this.Page.Dictionary.ContainsKey(DictionaryProperties.MediaBox))
                {
                    PdfArray mBox = this.Page.Dictionary[DictionaryProperties.MediaBox] as PdfArray;
                    if ((mBox[3] as PdfNumber).FloatValue == 0)
                        needTransformation = true;
                }
                if ((this.Page.Origin.X >= 0 && this.Page.Origin.Y >= 0) || needTransformation)
                    GetTranslateTransform(location.X, location.Y + size.Height, matrix);
                else
                    GetTranslateTransform(location.X, location.Y + 0, matrix);
            }

            else
                GetTranslateTransform(location.X, location.Y + size.Height, matrix);

            if (bNeedScale) GetScaleTransform(scaleX, scaleY, matrix);

            m_streamWriter.ModifyCTM(matrix);

            // Output template.
            PdfResources resources = m_getResources();
            PdfName name = resources.GetName(template);

            m_streamWriter.ExecuteObject(name);

            // Restore state.
            Restore(state);

            //Transfer automatic fields from template.
            PdfGraphics g = template.Graphics;

            if (g != null)
            {
                foreach (PdfAutomaticFieldInfo fieldInfo in g.AutomaticFields)
                {
                    PointF newLocation = new PointF(fieldInfo.Location.X + location.X,
                        fieldInfo.Location.Y + location.Y);

                    float scalingX = template.Size.Width == 0 ? 0 : size.Width / template.Size.Width;
                    float scalingY = template.Size.Height == 0 ? 0 : size.Height / template.Size.Height;

                    AutomaticFields.Add(new PdfAutomaticFieldInfo(fieldInfo.Field, newLocation,
                        scalingX, scalingY));

                    Page.Dictionary.Modify();

                }
            }

            m_getResources().RequireProcSet(ProcedureSets.ImageB);
            m_getResources().RequireProcSet(ProcedureSets.ImageC);
            m_getResources().RequireProcSet(ProcedureSets.ImageI);
            m_getResources().RequireProcSet(ProcedureSets.Text);
        }
        #endregion

        #region Other methods
        /// <summary>
        /// Flashes this instance.
        /// </summary>
        public void Flush()
        {
            if (m_bStateSaved)
            {
                m_streamWriter.RestoreGraphicsState();
                m_bStateSaved = false;
            }
        }

        /// <summary>
        /// Saves the current state of this Graphics and identifies the saved state with a GraphicsState.
        /// </summary>
        /// <returns>This method returns a GraphicsState that represents the saved state of this Graphics. </returns>
        /// <remarks>This method works similar to <see cref="System.Drawing.Graphics.Save"/> method.</remarks>
        public PdfGraphicsState Save()
        {
            PdfGraphicsState state = new PdfGraphicsState(this, Matrix.Clone());

            state.Brush = m_currentBrush;
            state.Pen = m_currentPen;
            state.Font = m_currentFont;
            state.ColorSpace = m_currentColorSpace;
            state.CharacterSpacing = m_previousCharacterSpacing;
            state.WordSpacing = m_previousWordSpacing;
            state.TextScaling = m_previousTextScaling;
            state.TextRenderingMode = m_previousTextRenderingMode;

            m_graphicsState.Push(state);

         //   m_streamWriter.RestoreGraphicsState();
            if (m_bStateSaved)
            {
                m_streamWriter.RestoreGraphicsState();
                m_bStateSaved = false;
            }

            m_streamWriter.SaveGraphicsState();

            return state;
        }

        /// <summary>
        /// Restores the last state of this Graphics.
        /// </summary>
        public void Restore()
        {
            if (m_graphicsState.Count > 0)
            {
                DoRestoreState();
            }
        }

        /// <summary>
        /// Restores the state of this Graphics to the state represented by a GraphicsState.
        /// </summary>
        /// <param name="state">GraphicsState that represents the state to which to restore this Graphics.</param>
        /// <remarks>This method works similar to <see cref="System.Drawing.Graphics.Restore"/> method.</remarks>
        public void Restore(PdfGraphicsState state)
        {
            if (state == null)
                throw new ArgumentNullException("state");

            if (state.Graphics != this)
                throw new ArgumentException("The GraphicsState belongs to another Graphics object.", "state");

            if (m_graphicsState.Contains(state))
            {
                while (true)
                {
                    if (m_graphicsState.Count == 0) break;

                    PdfGraphicsState popState = DoRestoreState();

                    if (popState == state)
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Modifying the current clipping path by intersecting it with the current path.
        /// </summary>
        /// <param name="rectangle">Clip rectangle.</param>
        public void SetClip(RectangleF rectangle)
        {
            SetClip(rectangle, PdfFillMode.Winding);
        }

        /// <summary>
        /// Modifying the current clipping path by intersecting it with the current path.
        /// </summary>
        /// <param name="rectangle">Clip rectangle.</param>
        /// <param name="mode">The fill mode to determine which regions lie inside the clipping	path.</param>
        public void SetClip(RectangleF rectangle, PdfFillMode mode)
        {
            m_streamWriter.AppendRectangle(rectangle);
            m_streamWriter.ClipPath((mode == PdfFillMode.Alternate));
        }

        /// <summary>
        /// Modifying the current clipping path by intersecting it with the current path.
        /// </summary>
        /// <param name="path">Clip path.</param>
        public void SetClip(PdfPath path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            SetClip(path, path.FillMode);
        }

        /// <summary>
        /// Modifying the current clipping path by intersecting it with the current path.
        /// </summary>
        /// <param name="path">Clip path.</param>
        /// <param name="mode">The fill mode to determine which regions lie inside the clipping	path.</param>
        public void SetClip(PdfPath path, PdfFillMode mode)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            BuildUpPath(path);
            m_streamWriter.ClipPath((mode == PdfFillMode.Alternate));
        }

        /// <summary>
        /// Sets the transparency.
        /// </summary>
        /// <param name="alpha">The alpha value for both pen
        /// and brush operations.</param>
        public void SetTransparency(float alpha)
        {
            SetTransparency(alpha, alpha, PdfBlendMode.Normal);
        }

        /// <summary>
        /// Sets the transparency.
        /// </summary>
        /// <param name="alphaPen">The alpha value for pen operations.</param>
        /// <param name="alphaBrush">The alpha value for brush operations.</param>
        public void SetTransparency(float alphaPen, float alphaBrush)
        {
            SetTransparency(alphaPen, alphaBrush, PdfBlendMode.Normal);
        }

        /// <summary>
        /// Sets the transparency.
        /// </summary>
        /// <param name="alphaPen">The alpha value for pen operations.</param>
        /// <param name="alphaBrush">The alpha value for brush operations.</param>
        /// <param name="blendMode">The blend mode.</param>
        public void SetTransparency(float alphaPen, float alphaBrush, PdfBlendMode blendMode)
        {
            if (m_trasparencies == null)
            {
                m_trasparencies = new Dictionary<TransparencyData, PdfTransparency>();
            }

            PdfTransparency transp = null;
            TransparencyData td = new TransparencyData(alphaPen, alphaBrush, blendMode);
            
            if(m_trasparencies.ContainsKey(td))
                transp = m_trasparencies[td] as PdfTransparency;

            if (transp == null)
            {
                transp = new PdfTransparency(alphaPen, alphaBrush, blendMode);
                m_trasparencies[td] = transp;
            }

            PdfResources resources = m_getResources();
            PdfName name = resources.GetName(transp);

            PdfStreamWriter sw = StreamWriter;
            sw.SetGraphicsState(name);
        }
        #endregion
        #endregion

        #region Implementation
        /// <summary>
        /// Normalizes the text.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="text">The text.</param>
        /// <returns>The normalized string.</returns>
        internal static string NormalizeText(PdfFont font, string text)
        {
            PdfTrueTypeFont ttf = font as PdfTrueTypeFont;

            if (font is PdfStandardFont || (ttf != null && !ttf.Unicode))
            {
                text = PdfStandardFont.Convert(text);
            }

            return text;
        }
       
        //Used to translate the transformation.
        internal void TranslateTransform(float offsetX, float offsetY, bool value)
        {
            PdfTransformationMatrix matrix = new PdfTransformationMatrix(value);
            GetTranslateTransform(offsetX, offsetY, matrix);
            m_streamWriter.ModifyCTM(matrix);
            Matrix.Multiply(matrix);
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            m_bStateSaved = false;
            m_currentPen = null;
            m_currentBrush = null;
            m_currentFont = null;
            m_currentColorSpace = PdfColorSpace.RGB;
            m_bCSInitialized = false;
            m_matrix = null;
            m_previousTextRenderingMode = (TextRenderingMode)(-1); //.Fill;
            m_previousCharacterSpacing = -1.0f;
            m_previousWordSpacing = -1.0f;
            m_previousTextScaling = -100.0f;
            m_trasparencies = null;
            m_currentStringFormat = null;

            m_clipBounds = new RectangleF(PointF.Empty, Size);
            m_graphicsState = new Stack<PdfGraphicsState>();
            m_getResources().RequireProcSet(ProcedureSets.PDF);
        }

        /// <summary>
        /// Sets the layer for the graphics.
        /// </summary>
        /// <param name="layer">The layer.</param>
        internal void SetLayer(PdfPageLayer layer)
        {
            m_layer = layer;

            PdfPage page = layer.Page as PdfPage;

            if (page != null)
            {
                page.BeginSave += new EventHandler(PageSave);
            }
            else
            {
                PdfLoadedPage lpage = layer.Page as PdfLoadedPage;
                lpage.BeginSave += new EventHandler(PageSave);
            }
        }

        /// <summary>
        /// Handles the Save event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void PageSave(object sender, EventArgs e)
        {
            if (m_automaticFields != null)
            {
                foreach (PdfAutomaticFieldInfo fieldInfo in m_automaticFields)
                {
                    fieldInfo.Field.PerformDraw(this, fieldInfo.Location,
                        fieldInfo.ScalingX, fieldInfo.ScalingY);
                }
            }
        }

        /// <summary>
        /// Updates y co-ordinate.
        /// </summary>
        /// <param name="y">Y co-ordinate.</param>
        /// <returns>Updated y co-ordinate.</returns>
        internal static float UpdateY(float y)
        {
            return -y;
            //return y;
        }

        /// <summary>
        /// Writes a comment line.
        /// </summary>
        /// <param name="comment">The comment.</param>
        internal void PutComment(string comment)
        {
            m_streamWriter.WriteComment(comment);
        }

        /// <summary>
        /// Clears an instance.
        /// </summary>
        internal void Reset(SizeF size)
        {
            m_canvasSize = size;
            m_streamWriter.Clear();
            Initialize();
            InitializeCoordinates();
        }

        /// <summary>
        /// Restores graphics state.
        /// </summary>
        /// <returns>The restored graphics state.</returns>
        private PdfGraphicsState DoRestoreState()
        {
            PdfGraphicsState state = m_graphicsState.Pop();

            m_matrix = state.Matrix;
            m_currentBrush = state.Brush;
            m_currentPen = state.Pen;
            m_currentFont = state.Font;
            m_currentColorSpace = state.ColorSpace;
            m_previousCharacterSpacing = state.CharacterSpacing;
            m_previousWordSpacing = state.WordSpacing;
            m_previousTextScaling = state.TextScaling;
            m_previousTextRenderingMode = state.TextRenderingMode;

            m_streamWriter.RestoreGraphicsState();

            return state;
        }

        /// <summary>
        /// Controls all state modifications and react repectively.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="font">The font.</param>
        private void StateControl(PdfPen pen, PdfBrush brush, PdfFont font)
        {
            StateControl(pen, brush, font, null);
        }

        /// <summary>
        /// Controls all state modifications and react respectively.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="font">The font.</param>
        /// <param name="format">Text settings.</param>
        private void StateControl(PdfPen pen, PdfBrush brush, PdfFont font, PdfStringFormat format)
        {
            //if( pen == null && brush == null )
            //throw new ArgumentNullException( "pen, brush", "The Pen and the brush are null, please specify the appropriate parameters for correct output." );
            if (((pen != null && pen.Color.A == 0) || (brush != null && brush is PdfSolidBrush && (brush as PdfSolidBrush).Color.A == 0)) && Layer != null && !Layer.Page.Dictionary.ContainsKey(DictionaryProperties.Group))
            {
                SetTransparencyGroup(Layer.Page);
            }
            if ((brush is PdfGradientBrush) == true)
            {
                m_bCSInitialized = false;
                (brush as PdfGradientBrush).ColorSpace = ColorSpace;
            }

            if ((brush is PdfTilingBrush) == true)
            {
                m_bCSInitialized = false;
                (brush as PdfTilingBrush).Graphics.ColorSpace = ColorSpace;
            }

            bool saveState = false;
            if (brush != null)
            {
                PdfSolidBrush s_brush = brush as PdfSolidBrush;
                if (s_brush != null)
                {
                    if (s_brush.Colorspaces != null)
                    {
                        ColorSpaceControl(s_brush.Colorspaces.ColorSpace);
                    }
                    else
                    {
                        if (m_layer != null)
                        {
                            if (m_layer.Page is PdfPage == true && ((m_layer.Page as PdfPage).Section.ParentDocument.GetType().Name != "PdfLoadedDocument"))
                            {
                                ColorSpace = (m_layer.Page as PdfPage).Document.ColorSpace;
                                m_currentColorSpace = (m_layer.Page as PdfPage).Document.ColorSpace;
                            }
                            else if (m_layer.Page is PdfLoadedPage == true)
                            {
                                ColorSpace = ((m_layer.Page as PdfLoadedPage).Document as PdfLoadedDocument).ColorSpace;
                                m_currentColorSpace = ((m_layer.Page as PdfLoadedPage).Document as PdfLoadedDocument).ColorSpace;
                            }
                        }
                        InitCurrentColorSpace(m_currentColorSpace);
                    }
                }
                else
                {
                    if (m_layer != null)
                    {
                        if (m_layer.Page is PdfPage == true && ((m_layer.Page as PdfPage).Section.ParentDocument.GetType().Name != "PdfLoadedDocument"))
                        {
                            ColorSpace = (m_layer.Page as PdfPage).Document.ColorSpace;
                            m_currentColorSpace = (m_layer.Page as PdfPage).Document.ColorSpace;
                        }
                        else if (m_layer.Page is PdfLoadedPage == true)
                        {
                            ColorSpace = ((m_layer.Page as PdfLoadedPage).Document as PdfLoadedDocument).ColorSpace;
                            m_currentColorSpace = ((m_layer.Page as PdfLoadedPage).Document as PdfLoadedDocument).ColorSpace;
                        }
                    }
                    InitCurrentColorSpace(m_currentColorSpace);
                }
            }
            else if (pen != null)
            {
                PdfPen s_brush = pen as PdfPen;
                if (s_brush != null)
                {
                    if (s_brush.Colorspaces != null)
                    {
                        ColorSpaceControl(s_brush.Colorspaces.ColorSpace);
                    }
                    else
                    {
                        if (m_layer != null)
                        {
                            if (m_layer.Page is PdfPage == true && ((m_layer.Page as PdfPage).Section.ParentDocument.GetType().Name != "PdfLoadedDocument"))
                            {
                                ColorSpace = (m_layer.Page as PdfPage).Document.ColorSpace;
                                m_currentColorSpace = (m_layer.Page as PdfPage).Document.ColorSpace;
                            }
                            else if (m_layer.Page is PdfLoadedPage == true)
                            {
                                ColorSpace = ((m_layer.Page as PdfLoadedPage).Document as PdfLoadedDocument).ColorSpace;
                                m_currentColorSpace = ((m_layer.Page as PdfLoadedPage).Document as PdfLoadedDocument).ColorSpace;
                            }
                        }
                        InitCurrentColorSpace(m_currentColorSpace);
                    }
                }
                else
                {
                    if (m_layer != null)
                    {
                        if (m_layer.Page is PdfPage == true && ((m_layer.Page as PdfPage).Section.ParentDocument.GetType().Name != "PdfLoadedDocument"))
                        {
                            ColorSpace = (m_layer.Page as PdfPage).Document.ColorSpace;
                            m_currentColorSpace = (m_layer.Page as PdfPage).Document.ColorSpace;
                        }
                        else if (m_layer.Page is PdfLoadedPage == true)
                        {
                            ColorSpace = ((m_layer.Page as PdfLoadedPage).Document as PdfLoadedDocument).ColorSpace;
                            m_currentColorSpace = ((m_layer.Page as PdfLoadedPage).Document as PdfLoadedDocument).ColorSpace;
                        }
                    }
                    InitCurrentColorSpace(m_currentColorSpace);
                }
            }

            if (saveState)
            {
                if (m_bStateSaved)
                {
                    m_streamWriter.RestoreGraphicsState();
                }

                m_streamWriter.SaveGraphicsState();
                m_bStateSaved = true;
            }

            PenControl(pen, saveState);

            BrushControl(brush, saveState);

            FontControl(font, format, saveState);
        }

        /// <summary>
        /// Saves the font and other font settings.
        /// </summary>
        /// <param name="font">Current font.</param>
        /// <param name="format">Current format.</param>
        /// <param name="saveState">If set to <c>true</c> the state's been changed.</param>
        private void FontControl(PdfFont font, PdfStringFormat format, bool saveState)
        {
            if (font != null)
            {
                PdfSubSuperScript curSubSuper = (format != null) ? format.SubSuperScript : PdfSubSuperScript.None;
                PdfSubSuperScript prevSubSuper = (m_currentStringFormat != null) ?
                    m_currentStringFormat.SubSuperScript : PdfSubSuperScript.None;

                if (saveState || font != m_currentFont || curSubSuper != prevSubSuper)
                {
                    PdfResources resources = m_getResources();
                    PdfName fontName = resources.GetName(font);

                    m_currentFont = font;
                    m_currentStringFormat = format;

                    float size = font.Metrics.GetSize(format);

                    m_streamWriter.SetFont(font, fontName, size);
                }
            }
        }

        /// <summary>
        /// Saves the ColorSpace and other ColorSpace settings.
        /// </summary>
        /// <param name="colorspace"></param>
        private void ColorSpaceControl(PdfColorSpaces colorspace)
        {
            if (colorspace != null)
            {
                PdfResources resources = m_getResources();
                PdfName colorName = resources.GetName(colorspace);
                m_streamWriter.SetColorSpace(colorspace, colorName);
            }
        }

        /// <summary>
        /// Controls the brush state.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="saveState">if set to <c>true</c> the state's been changed.</param>
        private void BrushControl(PdfBrush brush, bool saveState)
        {
            if (brush != null)
            {
                bool iccbased = false;
                bool indexed = false;
                PdfBrush b = brush.Clone();

                PdfGradientBrush lgb = b as PdfGradientBrush;

                if (lgb != null)
                {
                    PdfTransformationMatrix m = lgb.Matrix;

                    PdfTransformationMatrix matrix = Matrix.Clone();

                    if (m != null)
                    {
                        m.Multiply(matrix);
                        matrix = m;
                    }

                    lgb.Matrix = matrix;
                }
                PdfSolidBrush br = brush as PdfSolidBrush;
                if (br != null)
                {
                    if (br.Colorspaces != null)
                    {
                        if ((br.Colorspaces is PdfCalRGBColor) == true)
                        {
                            ColorSpace = PdfColorSpace.RGB;
                        }
                        else if ((br.Colorspaces is PdfCalGrayColor) == true)
                        {
                            ColorSpace = PdfColorSpace.GrayScale;
                        }
                        else if ((br.Colorspaces is PdfICCColor) == true)
                        {
                            iccbased = true;
                            PdfICCColor c1 = br.Colorspaces as PdfICCColor;
                            if (c1.ColorSpaces.AlternateColorSpace != null)
                            {
                                if (c1.ColorSpaces.AlternateColorSpace is PdfCalGrayColorSpace)
                                {
                                    ColorSpace = PdfColorSpace.GrayScale;
                                }
                                else if (c1.ColorSpaces.AlternateColorSpace is PdfCalRGBColorSpace)
                                {
                                    ColorSpace = PdfColorSpace.RGB;
                                }
                                else if (c1.ColorSpaces.AlternateColorSpace is PdfLabColorSpace)
                                {
                                    ColorSpace = PdfColorSpace.RGB;
                                }
                                else if (c1.ColorSpaces.AlternateColorSpace is PdfDeviceColorSpace == true)
                                {
                                    PdfDeviceColorSpace temp = c1.ColorSpaces.AlternateColorSpace as PdfDeviceColorSpace;
                                    string type = temp.DeviceColorSpaceType.ToString();
                                    if (type == "RGB")
                                    {
                                        ColorSpace = PdfColorSpace.RGB;
                                    }
                                    else if (type == "GrayScale")
                                    {
                                        ColorSpace = PdfColorSpace.GrayScale;
                                    }
                                    else if (type == "CMYK")
                                    {
                                        ColorSpace = PdfColorSpace.CMYK;
                                    }
                                }
                            }
                            else
                            {
                                ColorSpace = PdfColorSpace.RGB;
                            }
                        }
                        else if ((br.Colorspaces is PdfSeparationColor) == true)
                        {
                            iccbased = true;
                            ColorSpace = PdfColorSpace.GrayScale;
                        }
                        else if (br.Colorspaces is PdfIndexedColor)
                        {
                            indexed = true;
                            ColorSpace = PdfColorSpace.GrayScale;
                        }
                        bool diff;
                        if (iccbased == true)
                        {
                            diff = b.MonitorChanges(m_currentBrush, m_streamWriter, m_getResources, saveState, ColorSpace, true, true);
                        }
                        else if (indexed == true)
                        {
                            diff = b.MonitorChanges(m_currentBrush, m_streamWriter, m_getResources, saveState, ColorSpace, true, true, true);
                        }
                        else
                        {
                            diff = b.MonitorChanges(m_currentBrush, m_streamWriter, m_getResources, saveState, ColorSpace, true);
                        }

                        if (diff)
                        {
                            m_currentBrush = b;
                        }
                    }
                    else
                    {
                        bool diff1 = b.MonitorChanges(m_currentBrush, m_streamWriter, m_getResources, saveState, ColorSpace);
                        if (diff1)
                        {
                            m_currentBrush = b;
                        }
                    }
                }
                else
                {
                    bool diff = b.MonitorChanges(m_currentBrush, m_streamWriter, m_getResources, saveState, ColorSpace);

                    if (diff)
                    {
                        m_currentBrush = b;
                    }
                }
                brush = null;
            }
        }

        /// <summary>
        /// Initializes the current color space.
        /// </summary>
        private void InitCurrentColorSpace()
        {
            if (!m_bCSInitialized)
            {
                m_streamWriter.SetColorSpace("DeviceRGB", true);
                m_streamWriter.SetColorSpace("DeviceRGB", false);
                m_bCSInitialized = true;
            }
        }

        /// <summary>
        /// Initializes the current color space.
        /// </summary>
        /// <param name="colorspace"></param>
        private void InitCurrentColorSpace(PdfColorSpace colorspace)
        {
            PdfResources re = m_getResources() as PdfResources;
            if (!m_bCSInitialized)
            {
                if (m_currentColorSpace != PdfColorSpace.GrayScale)
                {
                    m_streamWriter.SetColorSpace("Device" + m_currentColorSpace.ToString(), true);
                    m_streamWriter.SetColorSpace("Device" + m_currentColorSpace.ToString(), false);
                    m_bCSInitialized = true;
                }
                else
                {
                    m_streamWriter.SetColorSpace("DeviceGray", true);
                    m_streamWriter.SetColorSpace("DeviceGray", false);
                    m_bCSInitialized = true;
                }
            }
        }

        /// <summary>
        /// Controls the pen state.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="saveState">if set to <c>true</c> the state's been changed.</param>
        private void PenControl(PdfPen pen, bool saveState)
        {
            if (pen != null)
            {
                bool iccbased = false;
                bool indexed = false;
                PdfPen br = pen as PdfPen;
                if (br != null)
                {
                    if (br.Colorspaces != null)
                    {
                        if ((br.Colorspaces is PdfCalRGBColor) == true)
                        {
                            ColorSpace = PdfColorSpace.RGB;
                        }
                        else if ((br.Colorspaces is PdfCalGrayColor) == true)
                        {
                            ColorSpace = PdfColorSpace.GrayScale;
                        }
                        else if ((br.Colorspaces is PdfICCColor) == true)
                        {
                            iccbased = true;
                            PdfICCColor c1 = br.Colorspaces as PdfICCColor;
                            if (c1.ColorSpaces.AlternateColorSpace != null)
                            {
                                if (c1.ColorSpaces.AlternateColorSpace is PdfCalGrayColorSpace)
                                {
                                    ColorSpace = PdfColorSpace.GrayScale;
                                }
                                else if (c1.ColorSpaces.AlternateColorSpace is PdfCalRGBColorSpace)
                                {
                                    ColorSpace = PdfColorSpace.RGB;
                                }
                                else if (c1.ColorSpaces.AlternateColorSpace is PdfLabColorSpace)
                                {
                                    ColorSpace = PdfColorSpace.RGB;
                                }
                                else if (c1.ColorSpaces.AlternateColorSpace is PdfDeviceColorSpace == true)
                                {
                                    PdfDeviceColorSpace temp = c1.ColorSpaces.AlternateColorSpace as PdfDeviceColorSpace;
                                    string type = temp.DeviceColorSpaceType.ToString();
                                    if (type == "RGB")
                                    {
                                        ColorSpace = PdfColorSpace.RGB;
                                    }
                                    else if (type == "GrayScale")
                                    {
                                        ColorSpace = PdfColorSpace.GrayScale;
                                    }
                                    else if (type == "CMYK")
                                    {
                                        ColorSpace = PdfColorSpace.CMYK;
                                    }
                                }
                            }
                            else
                            {
                                ColorSpace = PdfColorSpace.RGB;
                            }
                        }
                        else if ((br.Colorspaces is PdfSeparationColor) == true)
                        {
                            iccbased = true;
                            ColorSpace = PdfColorSpace.GrayScale;
                        }
                        else if (br.Colorspaces is PdfIndexedColor)
                        {
                            indexed = true;
                            ColorSpace = PdfColorSpace.GrayScale;
                        }
                    }
                }
                bool diff;
                if (iccbased == false && indexed == false)
                {
                    diff = pen.MonitorChanges(m_currentPen, m_streamWriter, m_getResources, saveState, ColorSpace, Matrix.Clone());
                }
                else if (indexed == true)
                {
                    diff = pen.MonitorChanges(m_currentPen, m_streamWriter, m_getResources, saveState, ColorSpace, Matrix.Clone(), true);
                }
                else
                {
                    diff = pen.MonitorChanges(m_currentPen, m_streamWriter, m_getResources, saveState, ColorSpace, Matrix.Clone(), true);
                }
                if (diff)
                {
                    m_currentPen = pen.Clone();
                }
            }

        }

        /// <summary>
        /// Draws custom or sets predefined line cap style.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        private void CapControl(PdfPen pen, float x2, float y2, float x1, float y1)
        {
            //throw new Exception( "The method or operation is not implemented." );
        }

        /// <summary>
        /// Draws the path.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="needClosing">if it is need to close, set to <c>true</c>.</param>
        private void DrawPath(PdfPen pen, PdfBrush brush, bool needClosing)
        {
            DrawPath(pen, brush, PdfFillMode.Winding, needClosing);
        }

        /// <summary>
        /// Draws the path.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="fillMode">The fill mode.</param>
        /// <param name="needClosing">if it is need to close, set to <c>true</c>.</param>
        private void DrawPath(PdfPen pen, PdfBrush brush, PdfFillMode fillMode, bool needClosing)
        {
            bool isPen = pen != null;
            bool isBrush = brush != null;
            bool isEvenOdd = (fillMode == PdfFillMode.Alternate);

            if (isPen && isBrush)
            {
                if (needClosing)
                {
                    StreamWriter.CloseFillStrokePath(isEvenOdd);
                }
                else
                {
                    StreamWriter.FillStrokePath(isEvenOdd);
                }
            }
            else if (!isPen && !isBrush)
            {
                StreamWriter.EndPath();
            }
            else if (isPen)
            {
                if (needClosing)
                {
                    StreamWriter.CloseStrokePath();
                }
                else
                {
                    StreamWriter.StrokePath();
                }
            }
            else if (isBrush)
            {
                if (needClosing)
                {
                    StreamWriter.CloseFillPath(isEvenOdd);
                }
                else
                {
                    StreamWriter.FillPath(isEvenOdd);
                }
            }
            else
            {
                throw new PdfException("Internal CLR error.");
            }
        }

        /// <summary>
        /// Gets the bezier points for arc constructing.
        /// </summary>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        /// <param name="startAng">The start angle.</param>
        /// <param name="extent">The extent.</param>
        /// <returns></returns>
        internal static List<float[]> GetBezierArcPoints(float x1, float y1, float x2, float y2,
            float startAng, float extent)
        {
            if (x1 > x2)
            {
                float tmp;

                tmp = x1;
                x1 = x2;
                x2 = tmp;
            }

            if (y2 > y1)
            {
                float tmp;

                tmp = y1;
                y1 = y2;
                y2 = tmp;
            }

            float fragAngle;
            int numFragments;

            if (Math.Abs(extent) <= 90f)
            {
                fragAngle = extent;
                numFragments = 1;
            }
            else
            {
                numFragments = (int)(Math.Ceiling(Math.Abs(extent) / 90f));
                fragAngle = extent / numFragments;
            }

            float x_cen = (x1 + x2) / 2f;
            float y_cen = (y1 + y2) / 2f;
            float rx = (x2 - x1) / 2f;
            float ry = (y2 - y1) / 2f;
            float halfAng = (float)(fragAngle * Math.PI / 360.0);
            float kappa = (float)(Math.Abs(4.0 / 3.0 * (1.0 - Math.Cos(halfAng)) / Math.Sin(halfAng)));
            List<float[]> pointList = new List<float[]>();

            for (int i = 0; i < numFragments; ++i)
            {
                float theta0 = (float)((startAng + i * fragAngle) * Math.PI / 180.0);
                float theta1 = (float)((startAng + (i + 1) * fragAngle) * Math.PI / 180.0);
                float cos0 = (float)Math.Cos(theta0);
                float cos1 = (float)Math.Cos(theta1);
                float sin0 = (float)Math.Sin(theta0);
                float sin1 = (float)Math.Sin(theta1);

                if (fragAngle > 0f)
                {
                    pointList.Add(new float[]{x_cen + rx * cos0,
																			y_cen - ry * sin0,
																			x_cen + rx * (cos0 - kappa * sin0),
																			y_cen - ry * (sin0 + kappa * cos0),
																			x_cen + rx * (cos1 + kappa * sin1),
																			y_cen - ry * (sin1 - kappa * cos1),
																			x_cen + rx * cos1,
																			y_cen - ry * sin1});
                }
                else
                {
                    pointList.Add(new float[]{x_cen + rx * cos0,
																			y_cen - ry * sin0,
																			x_cen + rx * (cos0 + kappa * sin0),
																			y_cen - ry * (sin0 - kappa * cos0),
																			x_cen + rx * (cos1 - kappa * sin1),
																			y_cen - ry * (sin1 + kappa * cos1),
																			x_cen + rx * cos1,
																			y_cen - ry * sin1});
                }
            }
            return pointList;
        }

        /// <summary>
        /// Constructs the arc path using Bezier curves.
        /// </summary>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        /// <param name="startAng">The start angle.</param>
        /// <param name="sweepAngle">The sweep angle.</param>
        private void ConstructArcPath(float x1, float y1, float x2, float y2,
            float startAng, float sweepAngle)
        {
            List<float[]> points = GetBezierArcPoints(x1, y1, x2, y2, startAng, sweepAngle);

            if (points.Count == 0) return;

            float[] pt = (float[])points[0];

            m_streamWriter.BeginPath(pt[0], pt[1]);

            for (int i = 0; i < points.Count; ++i)
            {
                pt = points[i];
                m_streamWriter.AppendBezierSegment(pt[2], pt[3], pt[4], pt[5], pt[6], pt[7]);
            }
        }

        /// <summary>
        /// Builds up the path.
        /// </summary>
        /// <param name="path">The path.</param>
        private void BuildUpPath(PdfPath path)
        {
            PointF[] points = path.PathPoints;
            byte[] types = path.PathTypes;

            BuildUpPath(points, types);
        }

        /// <summary>
        /// Gets the bezier points from respective arrays.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="types">The types.</param>
        /// <param name="i">The i.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="p3">The p3.</param>
        private void GetBezierPoints(PointF[] points, byte[] types, ref int i, out PointF p2, out PointF p3)
        {
            const string errorMsg = "Malforming path.";
            ++i;

            if ((PathPointType)(types[i] & PathTypesValuesMask) == PathPointType.Bezier3)
            {
                p2 = points[i];
                ++i;

                if ((PathPointType)(types[i] & PathTypesValuesMask) == PathPointType.Bezier3)
                {
                    p3 = points[i];
                }
                else
                {
                    throw new ArgumentException(errorMsg);
                }
            }
            else
            {
                throw new ArgumentException(errorMsg);
            }
        }

        /// <summary>
        /// Builds up the path.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="types">The types.</param>
        private void BuildUpPath(PointF[] points, byte[] types)
        {
            for (int i = 0, cnt = points.Length; i < cnt; ++i)
            {
                byte typeValue = types[i];
                PointF point = points[i];

                switch ((PathPointType)(typeValue & (byte)PathTypesValuesMask))
                {
                    case PathPointType.Start:
                        m_streamWriter.BeginPath(point);
                        break;

                    //case PathPointType.Bezier: // it's the same.
                    case PathPointType.Bezier3:
                        // collect 3 points.
                        PointF p2, p3;

                        GetBezierPoints(points, types, ref i, out p2, out p3);
                        m_streamWriter.AppendBezierSegment(point, p2, p3);
                        break;

                    case PathPointType.Line:
                        m_streamWriter.AppendLineSegment(point);
                        break;

                    default:
                        throw new ArithmeticException("Incorrect path formation.");
                }
                typeValue = types[i];
                CheckFlags(typeValue);
            }
        }

        /// <summary>
        /// Checks path point type flags.
        /// </summary>
        /// <param name="type">The path point type.</param>
        private void CheckFlags(byte type)
        {
            if ((PathPointType)(type & (byte)PathPointType.CloseSubpath) == PathPointType.CloseSubpath)
            {
                m_streamWriter.ClosePath();
            }

            // NOTE: We don't know how to support this flag correctly.
            //if( type & PathPointType.DashMode != 0 )
            //{
            //}
        }

        /// <summary>
        /// Gets the text rendering mode.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="format">The format.</param>
        /// <returns>Proper TextRenderingMode value.</returns>
        private TextRenderingMode GetTextRenderingMode(PdfPen pen, PdfBrush brush, PdfStringFormat format)
        {
            TextRenderingMode tm = TextRenderingMode.None;

            if (pen != null && brush != null)
            {
                tm = TextRenderingMode.FillStroke;
            }
            else if (pen != null)
            {
                tm = TextRenderingMode.Stroke;
            }
            else if (brush != null)
            {
                tm = TextRenderingMode.Fill;
            }

            if (format != null && format.ClipPath)
            {
                tm |= TextRenderingMode.ClipFlag;
            }

            return tm;
        }


        #region Transformation coordinates
        /// <summary>
        /// Sets the drawing area and translates origin.
        /// </summary>
        /// <param name="x">X co-ordinate of the co-ordinate system.</param>
        /// <param name="y">Y co-ordinate of the co-ordinate system.</param>
        /// <param name="left">Left margin value.</param>
        /// <param name="top">Top margin value.</param>
        /// <param name="right">Right margin value.</param>
        /// <param name="bottom">Bottom margin value.</param>
        internal void ClipTranslateMargins(float x, float y, float left, float top, float right, float bottom)
        {
            RectangleF clipArea = new RectangleF(left, top,
                Size.Width - left - right, Size.Height - top - bottom);

            m_clipBounds = clipArea;
            m_streamWriter.WriteComment("Clip margins.");
            m_streamWriter.AppendRectangle(clipArea);
            m_streamWriter.ClosePath();
            m_streamWriter.ClipPath(false);

            m_streamWriter.WriteComment("Translate co-ordinate system.");
            TranslateTransform(x, y);
        }

        /// <summary>
        /// Sets the drawing area and translates origin.
        /// </summary>
        /// <param name="clipBounds">Clip bounds of the graphics.</param>
        internal void ClipTranslateMargins(RectangleF clipBounds)
        {
            m_clipBounds = clipBounds;
            m_streamWriter.WriteComment("Clip margins.");
            m_streamWriter.AppendRectangle(clipBounds);
            m_streamWriter.ClosePath();
            m_streamWriter.ClipPath(false);

            m_streamWriter.WriteComment("Translate co-ordinate system.");
            TranslateTransform(clipBounds.X, clipBounds.Y);
        }

        /// <summary>
        /// Initializes coordinate system.
        /// </summary>
        internal void InitializeCoordinates()
        {
            // Matrix equation: TM(T-1)=M', where T=[1 0 0 -1 0 h]
            m_streamWriter.WriteComment("Change co-ordinate system to left/top.");

            // Translate co-ordinates only, don't flip.
            TranslateTransform(0, UpdateY(Size.Height));
        }

        /// <summary>
        /// Initializes coordinate system.
        /// </summary>
        internal void InitializeCoordinates(PdfPageBase page)
        {
            PointF location = PointF.Empty;
            PdfDictionary dictionary = page.Dictionary;
            bool needTransformation = false;

            if (page.Dictionary.ContainsKey(DictionaryProperties.CropBox) && page.Dictionary.ContainsKey(DictionaryProperties.MediaBox))
            {
                PdfArray cropBox = page.Dictionary[DictionaryProperties.CropBox] as PdfArray;
                PdfArray mediaBox = page.Dictionary[DictionaryProperties.MediaBox] as PdfArray;
                if (cropBox.ToRectangle() == mediaBox.ToRectangle())
                    needTransformation = true;

                if ((cropBox[0] as PdfNumber).FloatValue > 0 && (cropBox[3] as PdfNumber).FloatValue > 0 && (mediaBox[0] as PdfNumber).FloatValue < 0 && (mediaBox[1] as PdfNumber).FloatValue < 0)
                {
                    TranslateTransform((cropBox[0] as PdfNumber).FloatValue, -(cropBox[3] as PdfNumber).FloatValue);
                    location.X = -(cropBox[0] as PdfNumber).FloatValue;
                    location.Y = (cropBox[3] as PdfNumber).FloatValue;
                }
            }
            else
                if (!page.Dictionary.ContainsKey(DictionaryProperties.CropBox))
                {
                    needTransformation = true;
                }

            if (needTransformation)
            {
                // Matrix equation: TM(T-1)=M', where T=[1 0 0 -1 0 h]
                m_streamWriter.WriteComment("Change co-ordinate system to left/top.");

                // Translate co-ordinates only, don't flip.
                TranslateTransform(0, UpdateY(Size.Height));
            }
            else
            {
                PdfTransformationMatrix matrix = new PdfTransformationMatrix();
                GetTranslateTransform(location.X, location.Y + 0, matrix);
            }
           
        }

        /// <summary>
        /// Flips the hirizontally.
        /// </summary>
        private void FlipHorizontal()
        {
            PdfTransformationMatrix matrix = new PdfTransformationMatrix();
            matrix.Translate(0, Size.Height);

            // Flip around X axis.
            matrix.Scale(1, -1);
            m_streamWriter.ModifyCTM(matrix);
        }

        /// <summary>
        /// Flips the coordinates vertically.
        /// </summary>
        private void FlipVertical()
        {
            PdfTransformationMatrix matrix = new PdfTransformationMatrix();
            matrix.Translate(Size.Width, 0);
            matrix.Scale(-1, 1);
            m_streamWriter.ModifyCTM(matrix);
        }

        /// <summary>
        /// Translates coordinates of the input matrix.
        /// </summary>
        /// <param name="x">X translation.</param>
        /// <param name="y">Y translation.</param>
        /// <param name="input">Input matrix.</param>
        /// <returns>Output matrix.</returns>
        private PdfTransformationMatrix GetTranslateTransform(float x, float y, PdfTransformationMatrix input)
        {
            if (input == null)
            {
                input = new PdfTransformationMatrix();
            }

            input.Translate(x, UpdateY(y));

            return input;
        }

        /// <summary>
        /// Scales coordinates of the input matrix.
        /// </summary>
        /// <param name="x">X scaling.</param>
        /// <param name="y">Y scaling.</param>
        /// <param name="input">Input matrix.</param>
        /// <returns>Output matrix.</returns>
        private PdfTransformationMatrix GetScaleTransform(float x, float y, PdfTransformationMatrix input)
        {
            if (input == null)
            {
                input = new PdfTransformationMatrix();
            }

            input.Scale(x, y);

            return input;
        }

        /// <summary>
        /// Rotates coordinates of the input matrix.
        /// </summary>
        /// <param name="angle">Rotation angle.</param>
        /// <param name="input">Input matrix.</param>
        /// <returns>Output matrix.</returns>
        private PdfTransformationMatrix GetRotateTransform(float angle, PdfTransformationMatrix input)
        {
            if (input == null)
            {
                input = new PdfTransformationMatrix();
            }

            input.Rotate(UpdateY(angle));

            return input;
        }

        /// <summary>
        /// Skews coordinates of the input matrix.
        /// </summary>
        /// <param name="angleX">X skewing.</param>
        /// <param name="angleY">Y skewing.</param>
        /// <param name="input">Input matrix.</param>
        /// <returns>Output matrix.</returns>
        private PdfTransformationMatrix GetSkewTransform(float angleX, float angleY, PdfTransformationMatrix input)
        {
            if (input == null)
            {
                input = new PdfTransformationMatrix();
            }

            input.Skew(UpdateY(angleX), UpdateY(angleY));

            return input;
        }
        #endregion

        #region DrawString methods
        /// <summary>
        /// Draws a CJK string.
        /// </summary>
        /// <param name="lineInfo">The line info.</param>
        /// <param name="layoutRectangle">The layout rectangle.</param>
        /// <param name="font">The font.</param>
        /// <param name="format">The format.</param>
        private void DrawCjkString(LineInfo lineInfo, RectangleF layoutRectangle, PdfFont font, PdfStringFormat format)
        {
            if (font == null)
                throw new ArgumentNullException("font");

            JustifyLine(lineInfo, layoutRectangle.Width, format);

            string line = lineInfo.Text;
            byte[] str = GetCjkString(line);

            m_streamWriter.ShowNextLineText(str, false);
        }

        /// <summary>
        /// Gets a CJK string.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>A proper PdfString class instance.</returns>
        private byte[] GetCjkString(string line)
        {
            if (line == null)
                throw new ArgumentNullException("line");

            byte[] val = PdfString.ToUnicodeArray(line, false);

            val = PdfString.EscapeSymbols(val);

            return val;
        }

        /// <summary>
        /// Draws Ascii line.
        /// </summary>
        /// <param name="lineInfo">Text line.</param>
        /// <param name="layoutRectangle">Lay outing rectangle.</param>
        /// <param name="font">Font object.</param>
        /// <param name="format">Text format.</param>
        private void DrawAsciiLine(LineInfo lineInfo, RectangleF layoutRectangle,
            PdfFont font, PdfStringFormat format)
        {
            if (font == null)
                throw new ArgumentNullException("font");

            JustifyLine(lineInfo, layoutRectangle.Width, format);

            string line = lineInfo.Text;
            PdfString str = GetAsciiString(line);

            m_streamWriter.ShowNextLineText(str);
        }

        /// <summary>
        /// Creates PdfString from ASCII string. 
        /// </summary>
        /// <param name="token">String line.</param>
        /// <returns>PdfString object.</returns>
        private PdfString GetAsciiString(string token)
        {
            if (token == null)
                throw new ArgumentNullException("token");

            PdfString val = new PdfString(token);
            val.Encode = PdfString.ForceEncoding.ASCII;
            return val;
        }

        /// <summary>
        /// Draws unicode line.
        /// </summary>
        /// <param name="lineInfo">Text line.</param>
        /// <param name="layoutRectangle">Lay outing rectangle.</param>
        /// <param name="font">Font object.</param>
        /// <param name="format">Text format.</param>
        private void DrawUnicodeLine(LineInfo lineInfo, RectangleF layoutRectangle,
            PdfFont font, PdfStringFormat format)
        {
            if (font == null)
                throw new ArgumentNullException("font");

            string line = lineInfo.Text;
            float lineWidth = lineInfo.Width;
            bool rtl = (format != null && format.RightToLeft);
            bool useWordSpace = (format != null &&
                (format.WordSpacing != 0f || format.Alignment == PdfTextAlignment.Justify));
            PdfTrueTypeFont ttfFont = font as PdfTrueTypeFont;
            float wordSpacing = JustifyLine(lineInfo, layoutRectangle.Width, format);

#if !SILVERLIGHT && !NETFX_CORE && !WP
            if (rtl)
            {
                bool rightAlign = (format != null && format.Alignment == PdfTextAlignment.Right);
                string[] blocks = RtlRenderer.Layout(line, ttfFont, rightAlign, useWordSpace);

                string[] words = null;
                if (blocks.Length > 1)
                {
                    words = RtlRenderer.SplitLayout(line, ttfFont, rightAlign, useWordSpace);
                }
                else
                {
                    words = new string[] { line };
                }
                DrawUnicodeBlocks(blocks, words, font, format, wordSpacing);
            }
            else
#endif
            {
                if (useWordSpace)
                {
                    string[] words = null;
                    string[] blocks = BreakUnicodeLine(line, ttfFont, out words);
                    DrawUnicodeBlocks(blocks, words, font, format, wordSpacing);
                }
                else
                {
                    string token = ConvertToUnicode(line, ttfFont);
                    PdfString val = GetUnicodeString(token);
                    m_streamWriter.ShowNextLineText(val);
                }
            }
        }

        /// <summary>
        /// Creates PdfString from the unicode text.
        /// </summary>
        /// <param name="token">Unicode text.</param>
        /// <returns>PdfString object.</returns>
        private PdfString GetUnicodeString(string token)
        {
            if (token == null)
                throw new ArgumentNullException("token");

            PdfString val = new PdfString(token);
            val.Converted = true;
            val.Encode = PdfString.ForceEncoding.ASCII;
            return val;
        }

        /// <summary>
        /// Breakes the unicode line to the words and converts symbols to glyphs.
        /// </summary>
        /// <param name="line">String text.</param>
        /// <param name="ttfFont">TTF font.</param>
        /// <param name="words">Array of real words.</param>
        /// <returns>Array of text in glyphs.</returns>
        private string[] BreakUnicodeLine(string line, PdfTrueTypeFont ttfFont, out string[] words)
        {
            if (line == null)
                throw new ArgumentNullException("line");

            if (ttfFont == null)
                throw new ArgumentNullException("ttfFont");

            words = line.Split(null);
            string[] tokens = new string[words.Length];

            for (int i = 0, len = words.Length; i < len; i++)
            {
                // Reconvert string according to unicode standard.
                string word = words[i];
                string token = ConvertToUnicode(word, ttfFont);
                tokens[i] = token;
            }

            return tokens;
        }

        /// <summary>
        /// Converts to unicode format.
        /// </summary>
        /// <param name="text">Unicode text.</param>
        /// <param name="ttfFont">The TTF font.</param>
        /// <returns>Converted string</returns>
        private string ConvertToUnicode(string text, PdfTrueTypeFont ttfFont)
        {
            string token = null;
            if (text == null)
                throw new ArgumentNullException("text");

            if (ttfFont == null)
                throw new ArgumentNullException("ttfFont");
            if (ttfFont.InternalFont is UnicodeTrueTypeFont)
            {
                TtfReader ttfReader = (ttfFont.InternalFont as UnicodeTrueTypeFont).TtfReader;

                ttfFont.SetSymbols(text);
                token = ttfReader.ConvertString(text);
                byte[] bytes = PdfString.ToUnicodeArray(token, false);
                token = PdfString.ByteToString(bytes);

                
            }
#if !SILVERLIGHT && ! NETFX_CORE && !WP
            else
                if (ttfFont.InternalFont is TrueTypeFont)
                {
                    TtfReader ttfReader = (ttfFont.InternalFont as TrueTypeFont).TtfReader;

                    ttfFont.SetSymbols(text);
                    token = ttfReader.ConvertString(text);
                    byte[] bytes = PdfString.ToUnicodeArray(token, false);
                    token = PdfString.ByteToString(bytes);

                    
                }
#endif   
            return token;
        }

        /// <summary>
        /// Draws array of unicode tokens.
        /// </summary>
        /// <param name="blocks">Unicode tokens.</param>
        /// <param name="words">Array of the real words.</param>
        /// <param name="font">Font object.</param>
        /// <param name="format">Text formatting.</param>
        /// <param name="wordSpacing">Word spacing value if need to be justified.</param>
        private void DrawUnicodeBlocks(string[] blocks, string[] words, PdfFont font, PdfStringFormat format, float wordSpacing)
        {
            if (blocks == null)
                throw new ArgumentNullException("blocks");

            if (words == null)
                throw new ArgumentNullException("words");

            if (font == null)
                throw new ArgumentNullException("font");

            m_streamWriter.StartNextLine();

            float x = 0f;
            float xShift = 0f;
            float firstLineIndent = 0f;
            float paragraphIndent = 0f;

            try
            {
                if (format != null)
                {
                    firstLineIndent = format.FirstLineIndent;
                    paragraphIndent = format.ParagraphIndent;
                    format.FirstLineIndent = 0f;
                    format.ParagraphIndent = 0f;
                }

                float spaceWidth = font.GetCharWidth(StringTokenizer.WhiteSpace, format) + wordSpacing;
                float characterSpacing = (format != null) ? format.CharacterSpacing : 0f;
                float wordSpace = (format != null && wordSpacing == 0) ? format.WordSpacing : 0f;

                spaceWidth += characterSpacing + wordSpace;

                for (int i = 0, len = blocks.Length; i < len; i++)
                {
                    string token = blocks[i];
                    string word = words[i];
                    float tokenWidth = 0f;

                    if (x != 0f)
                    {
                        m_streamWriter.StartNextLine(x, 0);
                    }

                    if (word.Length > 0)
                    {
                        tokenWidth += /*Utils.Round(*/ font.MeasureString(word, format).Width /*)*/;
                        tokenWidth += characterSpacing;
                        PdfString val = GetUnicodeString(token);
                        m_streamWriter.ShowText(val);
                    }

                    if (i != len - 1)
                    {
                        x = tokenWidth + spaceWidth;
                        xShift += x;
                    }
                }

                // Rolback current line position.
                if (xShift > 0)
                {
                    m_streamWriter.StartNextLine(-xShift, 0);
                }
            }
            finally
            {
                if (format != null)
                {
                    format.FirstLineIndent = firstLineIndent;
                    format.ParagraphIndent = paragraphIndent;
                }
            }
        }

        /// <summary>
        /// Gets the text lines from the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        private string[] GetTextLines(string text)
        {
            Regex regex = new Regex("[^\r\n]*"); // Any character except new line.

            MatchCollection matches = regex.Matches(text);
            int count = matches.Count;
            List<string> list = new List<string>();
            bool wasEmpty = true;


            for (int i = 0; i < count; ++i)
            {
                Match match = matches[i];
                string value = match.Value;

                if (value == string.Empty && !wasEmpty)
                {
                    wasEmpty = true;
                    continue;
                }
                else if (value != string.Empty)
                {
                    wasEmpty = false;
                }

                list.Add(value);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Applies all the text settings.
        /// </summary>
        /// <param name="font">Font object.</param>
        /// <param name="pen">Pen object.</param>
        /// <param name="brush">Brush object.</param>
        /// <param name="format">Text format.</param>
        private void ApplyStringSettings(PdfFont font, PdfPen pen, PdfBrush brush, PdfStringFormat format, RectangleF bounds)
        {
            if (font == null)
                throw new ArgumentNullException("font");

            if ((brush is PdfTilingBrush) == true)
            {
                m_bCSInitialized = false;
                (brush as PdfTilingBrush).Graphics.ColorSpace = ColorSpace;
            }
            else if ((brush is PdfGradientBrush) == true)
            {
                m_bCSInitialized = false;
                (brush as PdfGradientBrush).ColorSpace = ColorSpace;
            }

            bool setLineWidth = false;
            // Set text rendering mode.
            TextRenderingMode tm = GetTextRenderingMode(pen, brush, format);

            if (font.Name.Equals("Arial Unicode MS"))
            {
                if (font.Bold)
                {
                    if ((font as PdfTrueTypeFont).Unicode)
                    {
                        if (pen == null && brush != null)
                            pen = new PdfPen(brush);
                        tm = TextRenderingMode.FillStroke;
                        setLineWidth = true;
                    }
                }
            }

            StateControl(pen, brush, font, format);
            m_streamWriter.BeginText();

#if !SILVERLIGHT && !NETFX_CORE && !WP
            // Update StructTree for tagged PDF.
            if (Layer != null && Page != null && Page is PdfPage)
            {
                PdfSection parentSection = (Page as PdfPage).Section;
                if ((parentSection.ParentDocument is PdfDocument) && parentSection.ParentDocument.FileStructure.TaggedPdf)
                {
                    PdfStructTreeRoot structTreeRoot = PdfCrossTable.Dereference(parentSection.ParentDocument.Catalog[DictionaryProperties.StructTreeRoot]) as PdfStructTreeRoot;
                    if (structTreeRoot != null)
                    {
                        int id = structTreeRoot.Add("P", "", Page, bounds);
                        StreamWriter.WriteTag(string.Format("/{0} <</MCID {1} >>BDC", "P", id));
                    }
                }
            }
            else
            {
                PdfStructTreeRoot structTreeRoot = PdfCatalog.StructTreeRoot;

                if (structTreeRoot != null)
                {
                    int id = structTreeRoot.Add("P", "", bounds);
                    StreamWriter.WriteTag(string.Format("/{0} <</MCID {1} >>BDC", "P", id));
                }
            }
# endif
            if (setLineWidth)
            {
                  m_streamWriter.SetLineWidth(font.Size / 30f);
               
            }
          
            if ((tm) != m_previousTextRenderingMode)
            {
                m_streamWriter.SetTextRenderingMode(tm);
                m_previousTextRenderingMode = tm;
            }

            // Set character spacing.
            float cs = (format != null) ? format.CharacterSpacing : 0;

            if (cs != m_previousCharacterSpacing)
            {
                m_streamWriter.SetCharacterSpacing(cs);
                m_previousCharacterSpacing = cs;
            }

            // Set word spacing.
            // NOTE: it works only if the space code is equal to 32 (0x20).
            float ws = (format != null) ? format.WordSpacing : 0;

            if (ws != m_previousWordSpacing)
            {
                m_streamWriter.SetWordSpacing(ws);
                m_previousWordSpacing = ws;
            }
        }

        /// <summary>
        /// Calculates shift value if the line is horizontaly aligned.
        /// </summary>
        /// <param name="lineWidth">Line width.</param>
        /// <param name="boundsWidth">Bounds width.</param>
        /// <param name="format">Text format.</param>
        /// <returns>Shift value.</returns>
        private float GetHorizontalAlignShift(float lineWidth, float boundsWidth, PdfStringFormat format)
        {
            float shift = 0f;

            if (boundsWidth >= 0 && format != null && format.Alignment != PdfTextAlignment.Left)
            {
                switch (format.Alignment)
                {
                    case PdfTextAlignment.Center:
                        shift = (boundsWidth - lineWidth) / 2f;
                        break;

                    case PdfTextAlignment.Right:
                        shift = boundsWidth - lineWidth;
                        break;
                }
            }

            return shift;
        }

        /// <summary>
        /// Calculates shift value if the text is vertically aligned.
        /// </summary>
        /// <param name="textHeight">Text height.</param>
        /// <param name="boundsHeight">Bounds height.</param>
        /// <param name="format">Text format.</param>
        /// <returns>Shift value.</returns>
        internal float GetTextVerticalAlignShift(float textHeight, float boundsHeight, PdfStringFormat format)
        {
            float shift = 0f;

            if (boundsHeight >= 0 && format != null && format.LineAlignment != PdfVerticalAlignment.Top)
            {
                switch (format.LineAlignment)
                {
                    case PdfVerticalAlignment.Middle:
                        shift = (boundsHeight - textHeight) / 2f;
                        break;

                    case PdfVerticalAlignment.Bottom:
                        shift = boundsHeight - textHeight;
                        break;
                }
            }

            return shift;
        }

        /// <summary>
        /// Justifies the line if needed.
        /// </summary>
        /// <param name="lineInfo">String text.</param>
        /// <param name="boundsWidth">Width of the bounds.</param>
        /// <param name="format">Text format.</param>
        /// <returns>Space width for justifying.</returns>
        private float JustifyLine(LineInfo lineInfo, float boundsWidth, PdfStringFormat format)
        {
            string line = lineInfo.Text;
            float lineWidth = lineInfo.Width;
            bool shouldJustify = ShouldJustify(lineInfo, boundsWidth, format);
            bool hasWordSpacing = (format != null && format.WordSpacing != 0);
            char[] symbols = StringTokenizer.Spaces;
            int whitespacesCount = StringTokenizer.GetCharsCount(line, symbols);
            float wordSpace = 0f;

            if (shouldJustify)
            {
                // Correct line width.
                if (hasWordSpacing)
                {
                    lineWidth -= (whitespacesCount * format.WordSpacing);
                }

                float difference = boundsWidth - lineWidth;

                wordSpace = difference / whitespacesCount;
                m_streamWriter.SetWordSpacing(wordSpace);
            }
            else if (format != null && format.Alignment == PdfTextAlignment.Justify)
            {
                // If there is justifying, but the line shouldn't be justified, restore default word spacing.
                m_streamWriter.SetWordSpacing(0f);
            }

            return wordSpace;
        }

        /// <summary>
        /// Checks whether the line should be justified.
        /// </summary>
        /// <param name="lineInfo">String text.</param>
        /// <param name="boundsWidth">Width of the bounds.</param>
        /// <param name="format">Text format.</param>
        /// <returns>True if the line should be justified.</returns>
        private bool ShouldJustify(LineInfo lineInfo, float boundsWidth, PdfStringFormat format)
        {
            string line = lineInfo.Text;
            float lineWidth = lineInfo.Width;
            bool justifyStyle = (format != null && format.Alignment == PdfTextAlignment.Justify);
            bool goodWidth = (boundsWidth >= 0 && lineWidth < boundsWidth);
            char[] symbols = StringTokenizer.Spaces;
            int whitespacesCount = StringTokenizer.GetCharsCount(line, symbols);
            bool hasSpaces = (whitespacesCount > 0 && line[0] != StringTokenizer.WhiteSpace);
            bool goodLineBreakStyle = ((lineInfo.LineType & LineType.LayoutBreak) > 0);

            bool shouldJustify = (justifyStyle && goodWidth && hasSpaces && goodLineBreakStyle);
            return shouldJustify;
        }

        /// <summary>
        /// Checks and corrects layoutRectangle for text lay outing.
        /// </summary>
        /// <param name="layoutRectangle">Text bounds.</param>
        /// <returns>True - if some part of the layoutRectangle fits the canvas ClipBounds, false otherwise.</returns>
        private bool CheckCorrectLayoutRectangle(ref RectangleF layoutRectangle)
        {
            bool fits = true;

            //// The layoutRectangle is out of the ClipBounds.
            //if( layoutRectangle.Y >= ClipBounds.Height || layoutRectangle.X >= ClipBounds.Right )
            //{
            //  fits = false;
            //}
            //else
            //{
            //  // The layoutRectangle is limited by ClipBounds by height only.
            //  if( layoutRectangle.Height <= 0 )
            //  {
            //    layoutRectangle.Height = ClipBounds.Height - layoutRectangle.Y;
            //  }
            //  else
            //  {
            //    // The layoutRectangle is out of the ClipBounds.
            //    if( layoutRectangle.Bottom < 0 )
            //    {
            //      fits = false;
            //    }
            //    else if( layoutRectangle.Bottom > ClipBounds.Height )
            //    {
            //      float difference = layoutRectangle.Bottom - ClipBounds.Height;
            //      // NOTE: here is a bug. If the text is visible or partly visible it disappears.
            //      // If the text is in the scaled environment it might disappear or appear unexpectedly.
            //      float newHeight = layoutRectangle.Height;// -difference;

            //      if( newHeight > difference )
            //      {
            //        layoutRectangle.Height = newHeight;
            //      }
            //      else
            //      {
            //        fits = false;
            //      }
            //    }
            //  }
            //}

            return fits;
        }

        /// <summary>
        /// Creates lay outed rectangle depending on the text settings.
        /// </summary>
        /// <param name="textSize">Size of the text.</param>
        /// <param name="x">X co-ordinate of the text.</param>
        /// <param name="y">Y co-ordinate of the text.</param>
        /// <param name="format">Text format settings.</param>
        /// <returns>layout rectangle.</returns>
        internal RectangleF CheckCorrectLayoutRectangle(SizeF textSize, float x, float y, PdfStringFormat format)
        {
            RectangleF layoutedRectangle = new RectangleF(x, y, textSize.Width, textSize.Width);

            if (format != null)
            {
                switch (format.Alignment)
                {
                    case PdfTextAlignment.Center:
                        layoutedRectangle.X -= layoutedRectangle.Width / 2f;
                        break;

                    case PdfTextAlignment.Right:
                        layoutedRectangle.X -= layoutedRectangle.Width;
                        break;
                }

                switch (format.LineAlignment)
                {
                    case PdfVerticalAlignment.Middle:
                        layoutedRectangle.Y -= layoutedRectangle.Height / 2f;
                        break;

                    case PdfVerticalAlignment.Bottom:
                        layoutedRectangle.Y -= layoutedRectangle.Height;
                        break;
                }
            }

            return layoutedRectangle;
        }

        /// <summary>
        /// Emulates Underline, Strikeout of the text if needed.
        /// </summary>
        /// <param name="pen">Current pen.</param>
        /// <param name="brush">Current brush.</param>
        /// <param name="result">Lay outing result.</param>
        /// <param name="font">Font object.</param>
        /// <param name="layoutRectangle">Lay outing rectangle.</param>
        /// <param name="format">Text format.</param>
        private void UnderlineStrikeoutText(PdfPen pen, PdfBrush brush,
            PdfStringLayoutResult result, PdfFont font, RectangleF layoutRectangle,
            PdfStringFormat format)
        {
            if (result == null)
                throw new ArgumentNullException("result");

            if (font == null)
                throw new ArgumentNullException("font");

            if (font.Underline || font.Strikeout)
            {
                // Calculate line width.
                PdfPen linePen = CreateUnderlineStikeoutPen(pen, brush, font, format);

                if (linePen != null)
                {
                    // Approximate line positions.
                    float vShift = GetTextVerticalAlignShift(result.ActualSize.Height, layoutRectangle.Height, format);
                    if (format != null && format.SubSuperScript == PdfSubSuperScript.SubScript)
                    {
                        vShift += (font.Height - font.Metrics.GetHeight(format));
                    }

                    float underlineYOffset = layoutRectangle.Y + vShift + font.Metrics.GetAscent(format) + 1.5f * linePen.Width;
                    float strikeoutYOffset = layoutRectangle.Y + vShift + font.Metrics.GetHeight(format) / 2f + 1.5f * linePen.Width;
                    LineInfo[] lines = result.Lines;

                    // Run through the text and draw lines.
                    for (int i = 0, len = result.LineCount; i < len; i++)
                    {
                        LineInfo lineInfo = lines[i];
                        string line = lineInfo.Text;
                        float lineWidth = lineInfo.Width;
                        float hShift = GetHorizontalAlignShift(lineWidth, layoutRectangle.Width, format);
                        float lineIndent = GetLineIndent(lineInfo, format, layoutRectangle, (i == 0));

                        hShift += (!RightToLeft(format)) ? lineIndent : 0f;

                        float x1 = layoutRectangle.X + hShift;
                        float x2 = (!ShouldJustify(lineInfo, layoutRectangle.Width, format)) ?
                            x1 + lineWidth - lineIndent : x1 + layoutRectangle.Width - lineIndent;

                        if (font.Underline)
                        {
                            float y = underlineYOffset;

                            DrawLine(linePen, x1, y, x2, y);
                            underlineYOffset += result.LineHeight;
                        }

                        if (font.Strikeout)
                        {
                            float y = strikeoutYOffset;

                            DrawLine(linePen, x1, y, x2, y);
                            strikeoutYOffset += result.LineHeight;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates a pen for drawing lines in the text.
        /// </summary>
        /// <param name="pen">Current pen.</param>
        /// <param name="brush">Current brush.</param>
        /// <param name="font">Current font.</param>
        /// <param name="format">Text format.</param>
        /// <returns>Pen for the lines.</returns>
        private PdfPen CreateUnderlineStikeoutPen(PdfPen pen, PdfBrush brush, PdfFont font,
            PdfStringFormat format)
        {
            if (font == null)
                throw new ArgumentNullException("font");

            // Calculate line width.
            float lineWidth = font.Metrics.GetSize(format) / 20f;
            PdfPen linePen = null;

            // Create a pen fo the lines.
            if (pen != null)
            {
                linePen = new PdfPen(pen.Color, lineWidth);
            }
            else if (brush != null)
            {
                linePen = new PdfPen(brush, lineWidth);
            }

            return linePen;
        }

        /// <summary>
        /// Draws layout result.
        /// </summary>
        /// <param name="result">Layout result.</param>
        /// <param name="font">Text font.</param>
        /// <param name="format">Text format.</param>
        /// <param name="layoutRectangle">Layout rectangle.</param>
        private void DrawLayoutResult(PdfStringLayoutResult result, PdfFont font,
            PdfStringFormat format, RectangleF layoutRectangle)
        {
            if (result == null)
                throw new ArgumentNullException("result");

            if (font == null)
                throw new ArgumentNullException("font");

            LineInfo[] lines = result.Lines;

            PdfTrueTypeFont ttfFont = font as PdfTrueTypeFont;
            bool unicode = (ttfFont != null && ttfFont.Unicode);
            bool embed = (ttfFont != null && ttfFont.Embed);
            for (int i = 0, len = lines.Length; i < len; i++)
            {
                LineInfo lineInfo = lines[i];
                string line = lineInfo.Text;
                float lineWidth = lineInfo.Width;

                if (line == null || line.Length == 0)
                {
                    m_streamWriter.StartNextLine();
                }
                else
                {
                    float hAlignShift = GetHorizontalAlignShift(lineWidth, layoutRectangle.Width, format);
                    float lineIndent = GetLineIndent(lineInfo, format, layoutRectangle, (i == 0));
                    hAlignShift += (!RightToLeft(format)) ? lineIndent : 0f;

                    if (hAlignShift != 0f)
                    {
                        m_streamWriter.StartNextLine(hAlignShift, 0);
                    }

                    if (font is PdfCjkStandardFont)
                    {
                        DrawCjkString(lineInfo, layoutRectangle, font, format);
                    }
                    else if (unicode)
                    {
                        DrawUnicodeLine(lineInfo, layoutRectangle, font, format);
                    }
                    else
                        if (embed)
                        {
                            DrawAsciiLine(lineInfo, layoutRectangle, font, format,embed);
                        }
                        else
                        {
                            DrawAsciiLine(lineInfo, layoutRectangle, font, format);
                        }

                    if (hAlignShift != 0f)
                    {
                        m_streamWriter.StartNextLine(-hAlignShift, 0);
                    }
                }
            }
            m_getResources().RequireProcSet(ProcedureSets.Text);
        }

        private void DrawAsciiLine(LineInfo lineInfo, RectangleF layoutRectangle, PdfFont font, PdfStringFormat format, bool embed)
        {
            if (font == null)
                throw new ArgumentNullException("font");

            JustifyLine(lineInfo, layoutRectangle.Width, format);

            string line = lineInfo.Text;
            PdfString str = GetAsciiString(line);

            m_streamWriter.ShowNextLineText(str);

            PdfTrueTypeFont ttfFont = font as PdfTrueTypeFont;
            string token = ConvertToUnicode(line, ttfFont);
        }

        /// <summary>
        /// Draws a layout result.
        /// </summary>
        /// <param name="result">Layout result.</param>
        /// <param name="font">Text font.</param>
        /// <param name="pen">The pen object.</param>
        /// <param name="brush">The brush object.</param>
        /// <param name="layoutRectangle">Layout rectangle.</param>
        /// <param name="format">Text format.</param>
        internal void DrawStringLayoutResult(PdfStringLayoutResult result, PdfFont font,
            PdfPen pen, PdfBrush brush, RectangleF layoutRectangle, PdfStringFormat format)
        {
            if (result == null)
                throw new ArgumentNullException("result");

            if (font == null)
                throw new ArgumentNullException("font");

            if (!result.Empty)
            {
                // Check whether we ned to clip text region.
                bool allowPartialLines = (format != null && !format.LineLimit);
                bool shouldClip = (format == null || !format.NoClip);
                bool clipRegion = allowPartialLines && shouldClip;
                PdfGraphicsState state = null;

                if (clipRegion)
                {
                    state = Save();

                    RectangleF clipBounds = new RectangleF(layoutRectangle.Location, result.ActualSize);

                    if (layoutRectangle.Width > 0)
                    {
                        clipBounds.Width = layoutRectangle.Width;
                    }

                    if (format.LineAlignment == PdfVerticalAlignment.Middle)
                    {
                        clipBounds.Y += (layoutRectangle.Height - clipBounds.Height) / 2.0f;
                    }
                    else if (format.LineAlignment == PdfVerticalAlignment.Bottom)
                    {
                        clipBounds.Y += (layoutRectangle.Height - clipBounds.Height);
                    }

                    SetClip(clipBounds);
                }

                ApplyStringSettings(font, pen, brush, format, layoutRectangle);

                // Set text scaling
                float textScaling = (format != null) ? format.HorizontalScalingFactor : 100.0f;

                if (textScaling != m_previousTextScaling)
                {
                    m_streamWriter.SetTextScaling(textScaling);
                    m_previousTextScaling = textScaling;
                }

                float height = (format == null || format.LineSpacing == 0f) ? font.Height : format.LineSpacing;
                bool subScript = (format != null && format.SubSuperScript == PdfSubSuperScript.SubScript);
                float shift = (subScript) ? height - (font.Height + font.Metrics.GetDescent(format)) :
                    (height - font.Metrics.GetAscent(format));
                if (m_isEMF && m_isBaselineFormat && format != null && (format.Alignment != PdfTextAlignment.Right))
                    shift = 0;
                m_streamWriter.StartNextLine(layoutRectangle.X, layoutRectangle.Y - shift);
                //m_streamWriter.SetLeading(+height);
                if (m_isEMF && m_isBaselineFormat && format != null && (format.Alignment == PdfTextAlignment.Right))
                {
                    if ((height > font.Size))
                    {
                        m_streamWriter.SetLeading(+height);
                    }
                    else
                    {
                        if (shift != 0.0)
                        {
                            m_streamWriter.SetLeading(+(font.Size + (font.Size / font.Height)));
                        }
                        else
                        {
                            m_streamWriter.SetLeading(+height);
                        }
                    }
                }
                else if (!m_isEMF || !m_isBaselineFormat)
                {
                    if ((height > font.Size))
                    {
                        m_streamWriter.SetLeading(+height);
                    }
                    else
                    {
                        if (shift != 0.0)
                        {
                            m_streamWriter.SetLeading(+(font.Size + (font.Size / font.Height)));
                        }
                        else
                        {
                            m_streamWriter.SetLeading(+height);
                        }
                    }
                }
                else
                {
                    m_streamWriter.SetLeading(0);
                }
                float vAlignShift = GetTextVerticalAlignShift(result.ActualSize.Height,
                    layoutRectangle.Height, format);

                if (m_isEMF)
                {   
                    if((result.ActualSize.Height-layoutRectangle.Height)>((font.Size/2)-1))
                    vAlignShift = GetTextVerticalAlignShift(result.ActualSize.Height, font.Height, format);
                }

                if (vAlignShift != 0f)
                {
                    m_streamWriter.StartNextLine(0, vAlignShift);
                }

                DrawLayoutResult(result, font, format, layoutRectangle);

                if (vAlignShift != 0f)
                {
                    m_streamWriter.StartNextLine(0, -(vAlignShift - result.LineHeight));
                }

#if !SILVERLIGHT && !NETFX_CORE && !WP
                if (((Layer != null && Page != null && Page is PdfPage) 
                    && ((Page as PdfPage).Section.ParentDocument is PdfDocument) && (Page as PdfPage).Section.ParentDocument.FileStructure.TaggedPdf)
                    || PdfCatalog.StructTreeRoot != null)
                    m_streamWriter.WriteTag("EMC");
# endif

                m_streamWriter.EndText();
                UnderlineStrikeoutText(pen, brush, result, font, layoutRectangle, format);

                if (clipRegion)
                {
                    Restore(state);
                }
            }
        }

        /// <summary>
        /// Returns line indent for the line.
        /// </summary>
        /// <param name="lineInfo">Line info.</param>
        /// <param name="format">Text settings.</param>
        /// <param name="layoutBounds">Layout Bounds.</param>
        /// <param name="firstLine">Indicates whether the line is the first in the text.</param>
        /// <returns>Returns line indent for the line.</returns>
        private float GetLineIndent(LineInfo lineInfo, PdfStringFormat format,
            RectangleF layoutBounds, bool firstLine)
        {
            float lineIndent = 0f;
            bool firstParagraphLine = ((lineInfo.LineType & LineType.FirstParagraphLine) > 0);

            if (format != null && firstParagraphLine)
            {
                lineIndent = (firstLine) ? format.FirstLineIndent : format.ParagraphIndent;
                lineIndent = (layoutBounds.Width > 0) ? Math.Min(layoutBounds.Width, lineIndent) : lineIndent;
                //lineIndent = Utils.Round( lineIndent );
            }

            return lineIndent;
        }

        /// <summary>
        /// Checks whether RTL is enabled.
        /// </summary>
        /// <param name="format">Text settings.</param>
        /// <returns>Tre if RTL is enabled.</returns>
        private bool RightToLeft(PdfStringFormat format)
        {
            bool rtl = (format != null && format.RightToLeft);
            return rtl;
        }

        /// <summary>
        /// Returns bounds of the line info.
        /// </summary>
        /// <param name="lineIndex">index of the line in the result.</param>
        /// <param name="result">Layout result.</param>
        /// <param name="font">Font used for this text.</param>
        /// <param name="layoutRectangle">Layout rectangle.</param>
        /// <param name="format">Text settings.</param>
        /// <returns>Returns bounds of the line info.</returns>
        internal RectangleF GetLineBounds(int lineIndex, PdfStringLayoutResult result, PdfFont font, RectangleF layoutRectangle, PdfStringFormat format)
        {
            if (result == null)
                throw new ArgumentNullException("result");

            if (font == null)
                throw new ArgumentNullException("font");

            RectangleF bounds = RectangleF.Empty;

            if (!result.Empty && lineIndex < result.LineCount && lineIndex >= 0)
            {
                LineInfo line = result.Lines[lineIndex];

                float vShift = GetTextVerticalAlignShift(result.ActualSize.Height, layoutRectangle.Height, format);
                float y = vShift + layoutRectangle.Y + (result.LineHeight * lineIndex);

                float lineWidth = line.Width;
                float hShift = GetHorizontalAlignShift(lineWidth, layoutRectangle.Width, format);
                float lineIndent = GetLineIndent(line, format, layoutRectangle, (lineIndex == 0));

                hShift += (!RightToLeft(format)) ? lineIndent : 0f;

                float x = layoutRectangle.X + hShift;
                float width = (!ShouldJustify(line, layoutRectangle.Width, format)) ?
                    lineWidth - lineIndent : layoutRectangle.Width - lineIndent;
                float height = result.LineHeight;

                bounds = new RectangleF(x, y, width, height);
            }

            return bounds;
        }
        #endregion

        /// <summary>
        /// Sets the BBox entry of the graphics dictionary.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        internal void SetBBox(RectangleF bounds)
        {
            PdfStream stream = m_streamWriter.GetStream();
            stream[DictionaryProperties.BBox] = PdfArray.FromRectangle(bounds);
        }
        #endregion

        #region Delegates
        /// <summary>
        /// Delegate declaring a method returning resources.
        /// </summary>
        /// <returns></returns>
        internal delegate PdfResources GetResources();
        #endregion

        #region Internals
        /// <summary>
        /// Holds info about transparency.
        /// </summary>
        private struct TransparencyData
        {
            #region Fields
            internal float AlphaPen;
            internal float AlphaBrush;
            internal PdfBlendMode BlendMode;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="T:TransparencyData"/> class.
            /// </summary>
            /// <param name="alphaPen">The alpha pen.</param>
            /// <param name="alphaBrush">The alpha brush.</param>
            /// <param name="blendMode">The blend mode.</param>
            internal TransparencyData(float alphaPen, float alphaBrush, PdfBlendMode blendMode)
            {
                AlphaPen = alphaPen;
                AlphaBrush = alphaBrush;
                BlendMode = blendMode;
            }
            #endregion

            #region Overrides
            /// <summary>
            /// Indicates whether this instance and a specified object are equal.
            /// </summary>
            /// <param name="obj">Another object to compare to.</param>
            /// <returns>
            /// true if obj and this instance are the same type and
            /// represent the same value; otherwise, false.
            /// </returns>
            public override bool Equals(object obj)
            {
                bool result = false;

                if (obj != null)
                {
                    if (obj is TransparencyData)
                    {
                        TransparencyData td = (TransparencyData)obj;

                        result = true;

                        result &= AlphaBrush == td.AlphaBrush;
                        result &= AlphaPen == td.AlphaPen;
                        result &= BlendMode == td.BlendMode;
                    }
                }

                return result;
            }

            /// <summary>
            /// Returns the hash code for this instance.
            /// </summary>
            /// <returns>
            /// A 32-bit signed integer that is the hash code for this instance.
            /// </returns>
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
            #endregion
        }
        #endregion
    }

    #region PdfGraphicsState
    /// <summary>
    /// Represents the state of a Graphics object.
    /// </summary>
    public class PdfGraphicsState
    {
        #region Fields
        /// <summary>
        /// Parent graphics object.
        /// </summary>
        private PdfGraphics m_graphics;
        /// <summary>
        /// The current transformation matrix.
        /// </summary>
        private PdfTransformationMatrix m_matrix;
        /// <summary>
        /// Stores previous rendering mode.
        /// </summary>
        private TextRenderingMode m_textRenderingMode = TextRenderingMode.Fill;
        /// <summary>
        /// Previous character spacing value or 0.
        /// </summary>
        private float m_characterSpacing = 0.0f;
        /// <summary>
        /// Previous word spacing value or 0.
        /// </summary>
        private float m_wordSpacing = 0.0f;
        /// <summary>
        /// The previously used text scaling value.
        /// </summary>
        private float m_textScaling = 100.0f;
        /// <summary>
        /// Current pen.
        /// </summary>
        private PdfPen m_pen;
        /// <summary>
        /// Current brush.
        /// </summary>
        private PdfBrush m_brush;
        /// <summary>
        /// Current font.
        /// </summary>
        private PdfFont m_font;
        /// <summary>
        /// Current color space.
        /// </summary>
        private PdfColorSpace m_colorSpace = PdfColorSpace.RGB;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the parent graphics object.
        /// </summary>
        internal PdfGraphics Graphics
        {
            get
            {
                return m_graphics;
            }
        }

        /// <summary>
        /// Gets the current matrix.
        /// </summary>
        internal PdfTransformationMatrix Matrix
        {
            get
            {
                return m_matrix;
            }
        }

        /// <summary>
        /// Gets or sets the current character spacing.
        /// </summary>
        internal float CharacterSpacing
        {
            get
            {
                return m_characterSpacing;
            }
            set
            {
                m_characterSpacing = value;
            }
        }

        /// <summary>
        /// Gets or sets the word spacing value.
        /// </summary>
        internal float WordSpacing
        {
            get
            {
                return m_wordSpacing;
            }
            set
            {
                m_wordSpacing = value;
            }
        }

        /// <summary>
        /// Gets or sets the text scaling value.
        /// </summary>
        internal float TextScaling
        {
            get
            {
                return m_textScaling;
            }
            set
            {
                m_textScaling = value;
            }
        }

        /// <summary>
        /// Gets or sets the current pen object.
        /// </summary>
        internal PdfPen Pen
        {
            get
            {
                return m_pen;
            }
            set
            {
                m_pen = value;
            }
        }

        /// <summary>
        /// Gets or sets the brush.
        /// </summary>
        internal PdfBrush Brush
        {
            get
            {
                return m_brush;
            }
            set
            {
                m_brush = value;
            }
        }

        /// <summary>
        /// Gets or sets the current font object.
        /// </summary>
        internal PdfFont Font
        {
            get
            {
                return m_font;
            }
            set
            {
                m_font = value;
            }
        }

        /// <summary>
        /// Gets or sets the current color space value.
        /// </summary>
        internal PdfColorSpace ColorSpace
        {
            get
            {
                return m_colorSpace;
            }
            set
            {
                m_colorSpace = value;
            }
        }

        /// <summary>
        /// Gets or sets the text rendering mode.
        /// </summary>
        internal TextRenderingMode TextRenderingMode
        {
            get
            {
                return m_textRenderingMode;
            }
            set
            {
                m_textRenderingMode = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// default constructor.
        /// </summary>
        private PdfGraphicsState()
        {
        }

        /// <summary>
        /// Creates new object.
        /// </summary>
        /// <param name="graphics">Parent graphics state.</param>
        /// <param name="matrix">The current transformation matrix.</param>
        internal PdfGraphicsState(PdfGraphics graphics, PdfTransformationMatrix matrix)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");

            if (matrix == null)
                throw new ArgumentNullException("matrix");

            m_graphics = graphics;
            m_matrix = matrix;
        }
        #endregion
    }
    #endregion
}
