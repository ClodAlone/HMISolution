#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT && !NETFX_CORE && !WP
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Microsoft.Win32;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Native;
using Syncfusion.Pdf.Primitives;
using System.IO;

namespace Syncfusion.Pdf.Graphics.Images.Metafiles
{
    internal class PdfEmfRenderer : IDisposable
    {
#region Static fields
        /// <summary>
        ///
        /// </summary>
        private static Image s_bmp = new Bitmap(1, 1);
        private float TextAngleLocal = 0;
        #endregion

#region Fields
        /// <summary>
        ///
        /// </summary>
        private PdfGraphics m_graphics;
        /// <summary>
        ///
        /// </summary>
        private System.Drawing.Graphics m_grCache;
        /// <summary>
        /// Matrix indicating bounds for the metafile output.
        /// </summary>
        private PdfTransformationMatrix m_bounds;
        /// <summary>
        /// Shows if the graphics state was changed.
        /// </summary>
        private bool m_stateChanged;
        /// <summary>
        /// Shows if it's the first call to the OnDrawPrimitive method.
        /// </summary>
        private bool m_bFirstCall = true;
        /// <summary>
        /// UnitConvertor instance for X coorditate.
        /// </summary>
        private PdfUnitConvertor m_convertX;
        /// <summary>
        /// UnitConvertor instance for Y coorditate.
        /// </summary>
        private PdfUnitConvertor m_convertY;

        /// <summary>
        /// Holds mapping between .NET graphics states and PDF graphics states.
        /// </summary>
        private Hashtable m_graphicsStates = new Hashtable();

        private PdfGraphicsState m_startState;
        /// <summary>
        /// Shows if it was the first transformation operation.
        /// </summary>
        private bool m_bFirstTransform = true;
        /// <summary>
        /// Gets or sets the quality.
        /// </summary>
        /// <remarks>When the image is stored into PDF not as a mask,
        /// you may reduce its quality, which saves the disk space.</remarks>
        private long m_quality = 100;
        /// <summary>
        /// get or sets the image resolution
        /// </summary>
        /// <remarks>When the image is stored into PDF not as a mask,
        /// you may reduce its resolution, which saves the disk space</remarks>
        private int m_imageResolution = 0;
        /// <summary>
        /// Represents flag indicating whether the page has been already transformed or not.
        /// TODO : Process Begin and End Container.
        /// </summary>
        private bool m_bPageTransformed;

        /// <summary>
        /// Shows if the graphics state was restored.
        /// </summary>
        private bool m_stateRestored;
        /// <summary>
        /// 
        /// </summary>
        private bool m_embedFonts;

        /// <summary>
        /// Internal varible to store the alpha pen.
        /// </summary>
        private float m_alphaPen = 1.0f;

        /// <summary>
        /// Internal varible to store the alpha brush.
        /// </summary>
        private float m_alphaBrush = 1.0f;

        /// <summary>
        /// Internal varible to store tranaparency is applied or not.
        /// </summary>
        private bool m_bIsTransparency = false;

        /// <summary>
        /// Internal varible to store the pdf blend mode.
        /// </summary>
        private PdfBlendMode m_blendMode = PdfBlendMode.Normal;

        /// <summary>
        /// Internal variable to store whether to connect last and first points.
        /// </summary>
        private bool m_CloseShape = false;

        /// <summary>
        /// Internal variable to store whether the EMF being drawn in a Tagged PDF.
        /// </summary>
        private bool m_taggedPDF = false;

        /// <summary>
        /// Asociated with parser context object.
        /// </summary>
        private object m_context;
       
        /// <summary>
       /// Used to store real clip value
       /// </summary>
        private RectangleF m_realClip;

         /// <summary>
       /// Internal variable to store the state change of the EMFplus and EMF
       /// </summary>
        internal bool m_EMFState = false;
        /// <summary>
        /// used to store current emf record type
        /// </summary>
        internal EmfPlusRecordType m_recordType;
        /// <summary>
        /// used to store prvious record type
        /// </summary>
        internal EmfPlusRecordType m_previousRecordtype;
        /// <summary>
        /// stores the text clip
        /// </summary>
        private RectangleF m_textClip;
        /// <summary>
        /// internal variable to store custom line cap arrow data
        /// </summary>
        internal CustomLineCapArrowData m_customLineCapArrowData;
        /// <summary>
        /// used to confirm wheather the text is clipped or not.
        /// </summary>
        private bool m_isIntersectClipRect = false;

       
        #endregion

#region Properties
        /// <summary>
        ///Get or Sets transparency is applied or not.
        /// </summary>
        internal bool IsTranparency
        {
            get
            {
                return m_bIsTransparency;
            }
            set
            {
                m_bIsTransparency = value;
            }
        }

        /// <summary>
        ///Get or Sets the alpha pen
        /// </summary>
        internal float AlphaPen
        {
            get
            {
                return m_alphaPen;
            }
            set
            {
                m_alphaPen = value;
            }
        }

        /// <summary>
        ///Get or Sets the alpha brush.
        /// </summary>
        internal float AlphaBrush
        {
            get
            {
                return m_alphaBrush;
            }
            set
            {
                m_alphaBrush = value;
            }
        }

        /// <summary>
        ///Get or Sets the Blend mode.
        /// </summary>
        internal PdfBlendMode BlendMode
        {
            get
            {
                return m_blendMode;
            }
            set
            {
                m_blendMode = value;
            }
        }


        /// <summary>
        /// Gets the PDF graphics object.
        /// </summary>
        public PdfGraphics Graphics
        {
            get
            {
                return m_graphics;
            }
        }

        /// <summary>
        /// Gets the native graphics object.
        /// </summary>
        public System.Drawing.Graphics NativeGraphics
        {
            get
            {
                return m_grCache;
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
                return NativeGraphics.Transform;
            }
            set
            {
                InternalResetClip();
                // Apply new transformation.
                NativeGraphics.Transform = value;
                PdfTransformationMatrix m = PrepareMatrix(value, PageScale);
                Graphics.PutComment("Transform property");
                SetTransform();
                Graphics.MultiplyTransform(m);
            }
        }

        /// <summary>
        /// Gets or sets the scaling between world units and page units for this Graphics object.
        /// </summary>
        public float PageScale
        {
            get
            {
                return NativeGraphics.PageScale;
            }
            set
            {
                NativeGraphics.PageScale = value;

                // Scale graphics.
                //if( NativeGraphics.PageScale != 1f )
                {
                    //Matrix oldMatrix = Transform;
                    try
                    {
                        //Matrix matrix = new Matrix();
                        //matrix.Scale( NativeGraphics.PageScale, NativeGraphics.PageScale );

                        Graphics.PutComment("PageScale property");
                        //NativeGraphics.Transform = matrix;
                        //Transform = matrix;
                        SetTransform();
                        Graphics.ScaleTransform(value, value);
                    }
                    finally
                    {
                        //  NativeGraphics.Transform = oldMatrix;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the unit of measure used for page coordinates in this Graphics object.
        /// </summary>
        public GraphicsUnit PageUnit
        {
            get
            {
                return NativeGraphics.PageUnit;
            }
            set
            {
                NativeGraphics.PageUnit = value;
                //m_UnitsChanged = true;

                float scaleX = 1.0f;
                float scaleY = 1.0f;

                if (value != GraphicsUnit.Display)
                {
                    scaleX = ConvertX.ConvertUnits(scaleX, GraphicsToPrintUnits(value), PdfGraphicsUnit.Pixel);
                    scaleY = ConvertY.ConvertUnits(scaleY, GraphicsToPrintUnits(value), PdfGraphicsUnit.Pixel);
                }

                Graphics.PutComment("PageUnit property");
                NativeGraphics.ScaleTransform(scaleX, scaleY, MatrixOrder.Prepend);
                Graphics.ScaleTransform(scaleX, scaleY);
            }
        }

        /// <summary>
        /// Gets the unit converter for X axis.
        /// </summary>
        private PdfUnitConvertor ConvertX
        {
            get
            {
                if (m_convertX == null)
                {
                    m_convertX = new PdfUnitConvertor(NativeGraphics);
                }

                return m_convertX;
            }
        }

        /// <summary>
        /// Gets the unit converter for Y axis.
        /// </summary>
        private PdfUnitConvertor ConvertY
        {
            get
            {
                if (m_convertY == null)
                {
                    m_convertY = new PdfUnitConvertor(NativeGraphics);
                }

                return m_convertY;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [page transformed].
        /// </summary>
        /// <value><c>true</c> if [page transformed]; otherwise, <c>false</c>.</value>
        public bool PageTransformed
        {
            get
            {
                return m_bPageTransformed;
            }
            set
            {
                m_bPageTransformed = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [embed fonts].
        /// </summary>
        /// <value><c>true</c> if [embed fonts]; otherwise, <c>false</c>.</value>
        internal bool EmbedFonts
        {
            get
            {
                return m_embedFonts;
            }
        }

        /// <summary>
        /// Gets text region manager
        /// </summary>
        private TextRegionManager TextRegions
        {
            get
            {
                return Context as TextRegionManager;
            }
        }

        /// <summary>
        /// Gets or sets context of the parser.
        /// </summary>
        internal object Context
        {
            get
            {
                return m_context;
            }

            set
            {
                if (m_context != value)
                {
                    m_context = value;
                }
            }
        }
       
        /// <summary>
       /// Gets or Sets real clip value
       /// </summary>
        private RectangleF RealClip
        {
            get
            {
                return m_realClip;
            }
            set
            {
                if (value != null)
                {
                    m_realClip = value;
                }
            }
        }
        #endregion

#region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfEmfRenderer"/> class.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        public PdfEmfRenderer(PdfGraphics graphics)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");

            m_graphics = graphics;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="T:PdfEmfRenderer"/> class.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="location"></param>
        /// <param name="tagged"></param>
        internal PdfEmfRenderer(PdfGraphics graphics, PointF location, bool tagged)
            :this(graphics)
        {
            m_taggedPDF = tagged;
            Context = new TextRegionManager();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfEmfRenderer"/> class.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="quality">The bitmap quality.</param>
        public PdfEmfRenderer(PdfGraphics graphics, long quality, bool embedFonts)
            : this(graphics)
        {
            m_quality = quality;
            m_embedFonts = embedFonts;
        }

        /// <summary>
        /// Initializes a new instance of the
        /// </summary>
        /// <param name="graphics">The Graphics</param>
        /// <param name="imageResolution">The Bitmap quality</param>
        /// <param name="embedFonts"></param>
         public PdfEmfRenderer(PdfGraphics graphics, int imageResolution, bool embedFonts)
            : this(graphics)
        {
            m_imageResolution = imageResolution;
            m_embedFonts = embedFonts;
        }
     
        #endregion

#region Public Methods
        /// <summary>
        /// Begins a new virtual graphics container.
        /// </summary>
        /// <returns>A GraphicsContainer instance.</returns>
        public GraphicsContainer BeginContainer()
        {
            InternalResetClip();
            InternalResetTransformation();

            Graphics.PutComment("BegingContainer");

            PdfGraphicsState gs = Graphics.Save();
            GraphicsContainer gc = NativeGraphics.BeginContainer();

            m_graphicsStates[gc] = gs;
            m_bFirstTransform = true;

            return gc;
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
            InternalResetClip();
            InternalResetTransformation();

            Graphics.PutComment("BegingContainer");

            PdfGraphicsState gs = Graphics.Save();

            GraphicsContainer gc = NativeGraphics.BeginContainer(destRect, srcRect, unit);
            
            m_graphicsStates[gc] = gs;

            return gc;
        }

        /// <summary>
        /// Fills the entire graphics with the specified color.
        /// </summary>
        /// <param name="color">The color.</param>
        public void Clear(Color color)
        {
            NativeGraphics.Clear(color);

            using (Brush brush = new SolidBrush(color))
            {
                RectangleF[] rects = new RectangleF[] { NativeGraphics.ClipBounds };
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

            // NOTE: Flip angles because PDF and GDI+ have different directions.
            //startAngle *= -1;
            //sweepAngle *= -1;

            OnDrawPrimitive();
            PdfPen pdfPen = ConvertPen(pen);
            Graphics.DrawArc(pdfPen, rect, startAngle, sweepAngle);
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
            PdfPen pdfPen = ConvertPen(pen);

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

                Graphics.DrawBezier(pdfPen, start, inner1, inner2, end);

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
        public void DrawClosedCurve(Pen pen, PointF[] points, float tension, PdfFillMode fillMode)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");

            PdfPen pdfPen = ConvertPen(pen);

            Graphics.DrawPolygon(pdfPen, points);
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
        /// <remarks>It isn't supported.</remarks>
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

            DrawCustomCap(pen, points, penPoints, false);
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
            PdfPen pdfPen = ConvertPen(pen);

            Graphics.DrawEllipse(pdfPen, rect);
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

            // If tagged PDF, checks for possible image split.
            if (m_taggedPDF)
            {
                if (!CheckPdfPage(destRect, true))
                    return;
            }

            OnDrawPrimitive();

            PdfImage img = PdfImage.FromImage(image);

            if (image is Bitmap)
            {
                if (m_imageResolution >0)
                {

                    Image resizeImage = ChangeResolution(m_imageResolution, image);
                    if (resizeImage != null)
                    {
                        Graphics.DrawImage(new PdfBitmap(resizeImage), destRect);
                    }
                    else
                    {
                        Graphics.DrawImage(new PdfBitmap(image), destRect);
                    }
                }
                else
                {
                    (img as PdfBitmap).Quality = m_quality;
                    Graphics.DrawImage(img, destRect);
                }
            }

            Graphics.DrawImage(img, destRect);
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

            float left = Math.Min(points[0].X, points[2].X);
            float top = points[0].Y;
            float width = points[1].X - points[0].X;
            float height = points[2].Y - points[0].Y;
            RectangleF rect = new RectangleF(left, top, width, height);

            // If tagged PDF, checks for possible image split.
            if (m_taggedPDF)
            {
                if (!CheckPdfPage(rect, true))
                    return;
            }

            OnDrawPrimitive();

           PdfImage img = PdfImage.FromImage(image);

           if (image is Bitmap)
           {

               if (m_imageResolution>0)
               {
                   Image resizeImage = ChangeResolution(m_imageResolution, image);
                   if (resizeImage != null)
                   {
                       Graphics.DrawImage(new PdfBitmap(resizeImage), rect);
                   }
                   else
                   {
                       Graphics.DrawImage(new PdfBitmap(image), rect);
                   }
               }
               else
               {
                   (img as PdfBitmap).Quality = m_quality;
                   Graphics.DrawImage(img, rect);
               }
           }
           else
           {
               Graphics.DrawImage(img, rect);
           }
        }

        /// <summary>
        /// Used to change the image resolution
        /// </summary>
        /// <param name="value">value of image resolution to set</param>
        /// <param name="image">original image</param>
        /// <returns></returns>
        private Image ChangeResolution(int value, Image image)
        {
       
                Image saved=null;
           
                Bitmap bitImage = image as Bitmap;
                bitImage.SetResolution(value,value);
                Stream s=new MemoryStream();
                
               
                ImageFormat imf = image.RawFormat;

                if (imf.Equals(ImageFormat.Jpeg) || imf.Equals(ImageFormat.Gif))
                {
                    bitImage.Save(s, ImageFormat.Jpeg);

                }
                else if (imf.Equals(ImageFormat.Png))
                {
                    bitImage.Save(s, ImageFormat.Png);

                }
                else if (imf.Equals(ImageFormat.Bmp))
                {
                    bitImage.Save(s, ImageFormat.Bmp);

                }
                else if (imf.Equals(ImageFormat.MemoryBmp))
                {
                    bitImage.Save(s, ImageFormat.MemoryBmp);

                }

                if (s.Length > 0)
                {
                    return Image.FromStream(s);
                }
                else
                {
                    return bitImage;
                }

        }

        /// <summary>
        /// Draws an image and/or brush.
        /// </summary>
        /// <param name="image">The image to draw.</param>
        /// <param name="brush">The brush to draw.</param>
        /// <param name="destRect">Where to draw.</param>
        /// <param name="srcRect">Where to draw from.</param>
        /// <param name="dwRop">Raster Operation Code.</param>
        public void DrawImage(Image image, Brush brush, RectangleF destRect, RectangleF srcRect,
            uint dwRop)
        {
            Graphics.PutComment("DrawImage");
            OnDrawPrimitive();

            // This is a workaround due to transparency problem.
            ConvertBrush(brush);
            PdfBitmap img = null;

            if (image != null)
            {
                img = (PdfBitmap)PdfBitmap.FromImage(image);
                img.Quality = m_quality;
            }

            switch ((RASTER_CODE)dwRop)
            {
                case RASTER_CODE.SRCCOPY:
                    if (image != null)
                    {
                        DrawImage(image, destRect, srcRect, GraphicsUnit.Pixel);
                    }
                    break;

                case RASTER_CODE.SRCAND:
                    if (image != null)
                    {
                        Graphics.Save();
                        Graphics.SetTransparency(1.1f, 1.1f, PdfBlendMode.Multiply);
                        Graphics.DrawImage(img, destRect);
                        Graphics.Restore();
                    }
                    break;

                case RASTER_CODE.PATCOPY:
                    FillRectangles(brush, new RectangleF[] { destRect });
                    break;
                case RASTER_CODE.SRCINVERT:
                    if (image != null)
                    {
                        Graphics.Save();
                        Graphics.SetTransparency(0.1f);
                        Graphics.DrawImage(img, destRect);
                        Graphics.Restore();
                    }
                    break;
                case RASTER_CODE.SRCPAINT:
                    if (image != null)
                    {
                        Graphics.Save();
                        Bitmap transImage = new Bitmap(image);
                        transImage.MakeTransparent(Color.Black);
                        MemoryStream imageStream = new MemoryStream();
                        transImage.Save(imageStream, ImageFormat.Png);
                        PdfImage  modifiedImage = PdfImage.FromStream(imageStream);
                        imageStream.Dispose();
                        Graphics.DrawImage(modifiedImage, destRect);
                        Graphics.Restore();
                    }
                    break;
                case RASTER_CODE.SRCANDDST:
                    PdfBrush solidBrush = ConvertBrush(brush);

                    Graphics.Save();
                    Graphics.SetTransparency(0.1f);
                    Graphics.DrawRectangle(solidBrush, destRect);
                    Graphics.Restore();
                    break;

                default:
                    if (image != null)
                    {
                        Graphics.Save();
                        Graphics.SetTransparency(1.0f);
                        Graphics.DrawImage(img, destRect);
                        Graphics.Restore();
                    }
                    else
                    {
                        PdfBrush br = ConvertBrush(brush);

                        Graphics.Save();
                        Graphics.SetTransparency(0.5f);
                        Graphics.DrawRectangle(br, destRect);
                        Graphics.Restore();
                    }
                    break;
            }
        }

        /// <summary>
        /// Draws extra line between the last and first points.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="points">The points.</param>
        /// <param name="closeShape">If true, connects last and first points.</param>
        internal void DrawLines(Pen pen, PointF[] points, bool closeShape)
        {
            m_CloseShape = closeShape;
            DrawLines(pen, points);

            // reset m_CloseShape.
            m_CloseShape = false;
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

            bool rotate = false;

            if (points.Length > 1 && pen.CompoundArray.Length != 0)
            {
                if (points[0].X == points[1].X)
                    rotate = true;
            }
             
            
            float alpha;
            PdfPen pdfPen = ConvertToPen(pen, out alpha);
            if (alpha == 0f)
            {
                pdfPen = new PdfPen(PdfBrushes.White);
            }
            else
            {
                if (IsTranparency)
                {
                    Graphics.SetTransparency(AlphaPen, AlphaBrush, BlendMode);
                }
                else
                {
                    Graphics.SetTransparency(alpha, alpha, PdfBlendMode.Normal);
                }
            }
          
           
            PointF start = points[0];

            if (pen.CompoundArray.Length != 0)
            {
                DrawCompoundLine(pen, points, rotate, pdfPen);
            }

           // Graphics.DrawLine(pdfPen, points[0], points[1]);
            else
            {
                for (int i = 1; i < length; i++)
                {
                    PointF last = points[i];
                    //  start.Y = last.Y += 1f;
                    Graphics.DrawLine(pdfPen, start, last);
                    start = last;
                }
            }

            // Draw extra line to connect last and first points.
            if (m_CloseShape)
                Graphics.DrawLine(pdfPen, points[length - 1], points[0]);

            PdfBrush brush = GetBrushFromPen(pdfPen);
            float width = pen.Width / 2;
            System.Drawing.Drawing2D.LineCap cap = pen.EndCap;

            if (length > 1 || cap == System.Drawing.Drawing2D.LineCap.Round)
            {
                DrawCap(cap, points, length - 2, length - 1, width, brush);
            }

            cap = pen.StartCap;

            if (length > 1 || cap == System.Drawing.Drawing2D.LineCap.Round)
            {
                DrawCap(cap, points, 0, 1, width, brush);
            }

        }

        /// <summary>
        /// Darw the multiple Line
        /// </summary>
        /// <param name="pen"></param>
        /// <param name="points"></param>
        /// <param name="rotate"></param>
        /// <param name="pdfPen"></param>
        private void DrawCompoundLine(Pen pen, PointF[] points, bool rotate, PdfPen pdfPen)
        {
            float diff = 0;
            float firstLine = 0;
            for (int i = 0; i < pen.CompoundArray.Length; i += 2)
            {

                float height = pen.Width;


                pdfPen.Width = (pen.CompoundArray[i + 1] - pen.CompoundArray[i]) * height;

                    if (!rotate)
                    {
                        Graphics.DrawLine(pdfPen, points[0].X, points[0].Y + diff, points[1].X, points[1].Y + diff);
                    }
                    else
                    {
                        Graphics.DrawLine(pdfPen, points[0].X + diff, points[0].Y, points[1].X + diff, points[1].Y);
                    }
                

                if (i + 1 < pen.CompoundArray.Length - 1)
                {

                    diff += (pen.CompoundArray[i + 2] - pen.CompoundArray[i + 1]) * height+pdfPen.Width;

                }

            }

        }

        /// <summary>
        /// Convert the pen to PdfPen
        /// </summary>
        /// <param name="pen"></param>
        /// <returns>PdfPen</returns>
        private PdfPen ConvertToPen(Pen pen,out float alpha)
        {
            PdfPen pdfPen = null;

            try
            {
                pdfPen = new PdfPen(pen.Color);
            }
            catch (ArgumentException)
            {
                pdfPen = new PdfPen(Color.Empty);
            }

            pdfPen.DashStyle = ConvertDashStyle(pen.DashStyle);

            if (pdfPen.DashStyle != PdfDashStyle.Solid)
            {
                pdfPen.DashOffset = pen.DashOffset;
                pdfPen.DashPattern = pen.DashPattern;
            }

            pdfPen.LineCap = ConvertCaps(pen.StartCap);
            pdfPen.LineCap = ConvertCaps(pen.EndCap);

            pdfPen.LineJoin = ConvertJoin(pen.LineJoin);
            pdfPen.MiterLimit = pen.MiterLimit;

            pdfPen.Width = pen.Width;

            try
            {
                alpha = (float)(pen.Color.A) / 255;
            }
            catch (ArgumentException)
            {
                alpha = 0;
            }

            if (pen.Brush != null)
            {
                float brushAlpha;
                PdfBrush brush = ConvertBrush(pen.Brush, out brushAlpha);
                PdfSolidBrush sBrush = brush as PdfSolidBrush;

                if ((sBrush != null && sBrush.Color.A != 0) || sBrush == null)
                {
                    pdfPen.Brush = brush;
                    alpha = brushAlpha;
                }

            }
            return pdfPen;
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

            PdfPen pdfPen = ConvertPen(pen);

            //if( pdfPen.Color.A == 0 )
            //{
            //  pdfPen.Color = Color.Black;
            //  Graphics.SetTransparency( 1.0f );
            //}

            PdfPath pdfPath = new PdfPath(path.PathPoints, path.PathTypes);

            Graphics.DrawPath(pdfPen, pdfPath);
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

            PdfPen pdfPen = ConvertPen(pen);

            Graphics.DrawPolygon(pdfPen, points);
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

            // NOTE: Flip angles because PDF and GDI+ have different directions.
            //startAngle *= -1;
            //sweepAngle *= -1;
            OnDrawPrimitive();

            PdfPen pdfPen = ConvertPen(pen);

            Graphics.DrawPie(pdfPen, rect, startAngle, sweepAngle);
        }

        /// <summary>
        /// Draws a series of rectangles.
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

            PdfPen pdfPen = ConvertPen(pen);

            for (int i = 0; i < rects.Length; i++)
            {
                RectangleF rect = rects[i];

                Graphics.DrawRectangle(pdfPen, rect);
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

            PdfBrush pdfBrush = ConvertBrush(brush);

            PdfFont pdfFont = null;

            try
            {
                pdfFont = GetPdfFont(text, font);
            }
            catch (Exception ex)
            {
                //Try resolving the font from its installed path.
                string fontFile = GetInstalledFontLocation(font);
                pdfFont = new PdfTrueTypeFont(fontFile, font.Size, (PdfFontStyle)font.Style);
            }

            SizeF textSize;
            float scalingFactor = ScaleText(text, pdfFont, rect, out textSize, null);

            //If tagged PDF, checks for possible text split.
            if (m_taggedPDF)
            {
                if (!CheckPdfPage(new RectangleF(rect.Location, new SizeF(rect.Width, Math.Max(rect.Height, pdfFont.Height))), false))
                    return;
            }

            if (scalingFactor != 1f)
            {
                PdfStringFormat format = new PdfStringFormat();
                format.HorizontalScalingFactor = scalingFactor * 100.0f;
                rect.Width /= scalingFactor;

                if (rect.Width == 0 && rect.Height == 0)
                {
                    PointF location = CorrectLocation(rect.Location, rect.Size, textSize, format);

                    Graphics.DrawString(text, pdfFont, pdfBrush, location, format);
                }
                else
                {
                    if (rect.Height == 0)
                    {
                        rect.Height = font.Height;
                    }

                    Graphics.DrawString(text, pdfFont, pdfBrush, rect, format);
                }

            }
            else
            {
                if (rect.Width == 0 && rect.Height == 0)
                {
                    PointF location = CorrectLocation(rect.Location, rect.Size, textSize, new PdfStringFormat());

                    Graphics.DrawString(text, pdfFont, pdfBrush, location);
                }
                else
                {
                    if (rect.Height == 0)
                    {
                        rect.Height = font.Height;
                    }

                    Graphics.DrawString(text, pdfFont, pdfBrush, rect);
                }
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
           
            if ( m_recordType==EmfPlusRecordType.EmfExtTextOutW&&m_EMFState)
            {
                if(this.Transform.Elements[3]==-1.0)
                Graphics.TranslateTransform(0,0,m_EMFState);

                m_EMFState = false;
            }
            OnDrawPrimitive();

            PdfBrush pdfBrush = ConvertBrush(brush);
            PdfStringFormat pdfFormat = ConvertFormat(format);

            PdfFont pdfFont = null;

            try
            {
               pdfFont = GetPdfFont(text, font);
            }
            catch(Exception ex)
            {
                //Try resolving the font from its installed path.
                string fontFile = GetInstalledFontLocation(font);
                pdfFont = new PdfTrueTypeFont(fontFile, font.Size, (PdfFontStyle)font.Style);
            }

            //Graphics.DrawRectangle( PdfPens.Black, rect );

            // Scale text according to its boundaries.
            SizeF rectSize = rect.Size;
            float scalingFactor = 1.0f;

            scalingFactor = ScaleText(text, pdfFont, rect, out rectSize, pdfFormat);

            if (scalingFactor != 1.0f /*&& pdfFormat.WordWrap == WordWrapType.None*/ )
            {
                if (pdfFormat == null)
                {
                    pdfFormat = new PdfStringFormat();
                }

                if (m_isIntersectClipRect)
                {
                    if (m_previousRecordtype != EmfPlusRecordType.EmfIntersectClipRect)
                        pdfFormat.HorizontalScalingFactor = scalingFactor * 100.0f;
                    m_isIntersectClipRect = false;
                }
                else
                {
                    pdfFormat.HorizontalScalingFactor = scalingFactor * 100.0f;
                }                
            }

            StringFormatFlags noBoundsMask =
                StringFormatFlags.NoWrap | StringFormatFlags.NoClip | StringFormatFlags.LineLimit;

            bool notUseBounds = (((format.FormatFlags & noBoundsMask) == noBoundsMask) &&
                (format.Trimming == StringTrimming.None) && (format.Alignment == StringAlignment.Near &&
                format.LineAlignment == StringAlignment.Near));

            if (rect.Width > pdfFont.MeasureString(text, pdfFormat).Width && text.Split(null).Length > 1)
            {
                char[] symbols = StringTokenizer.Spaces;
                int whitespacesCount = StringTokenizer.GetCharsCount(text, symbols);
                float charSpace = 0f;
                  if (pdfFormat.Alignment==PdfTextAlignment.Justify||pdfFormat.LineAlignment==PdfVerticalAlignment.Middle||notUseBounds)
                 { 
                       float lineWidth = pdfFont.MeasureString(text, pdfFormat).Width;
                       
                       float difference = rect.Width - lineWidth;
                       charSpace = difference / text.Length;
                 }     
                
              pdfFormat.CharacterSpacing = charSpace;
            }

            //If tagged PDF, checks for possible text split.
            if (m_taggedPDF)
            {
                if (!CheckPdfPage(new RectangleF(rect.Location, new SizeF(rect.Width, Math.Max(rect.Height, pdfFont.Height))), false))
                    return;
            }

            if ((rect.Width == 0 && rect.Height == 0) || notUseBounds)
            {
                //Graphics.Save();
                //Graphics.SetTransparency( 0.5f );
                //Graphics.DrawRectangle( PdfBrushes.LightGray, rect );
                //Graphics.Restore();

                PointF location = CorrectLocation(rect.Location, rect.Size, rectSize, pdfFormat);
              

                Graphics.DrawString(text, pdfFont, pdfBrush, location, pdfFormat);
            }
            else
            {
                rect.Width /= scalingFactor;

                if (rect.Width == 0)
                {
                    rect.Width = rectSize.Width;
                }
                else if (rect.Height == 0)
                {
                    rect.Height = rectSize.Height;
                }

                //Graphics.Save();
                //Graphics.SetTransparency( 0.5f );
                //Graphics.DrawRectangle( PdfBrushes.LightGray, rect );
                //Graphics.Restore();
                
                //Skip drawing hidden text present in the metafile.
                if (rect.Width > 0)
                {
                   //Skip to draw the clipped text
                    if (m_textClip.X != 0 && m_textClip.Y != 0 && m_textClip.Width != 0 && m_textClip.Height != 0)
                    {
                        if (m_textClip.Width >= rect.Width / 2)
                        {
                            Graphics.DrawString(text, pdfFont, pdfBrush, rect, pdfFormat);
                            m_textClip = RectangleF.Empty;
                        }
                    }
                    else
                    {
                        Graphics.DrawString(text, pdfFont, pdfBrush, rect, pdfFormat);
                    }
                }
                }
            }

        /// <summary>
        /// Checks if the given rectangle overflows the current page.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        private bool CheckPdfPage(RectangleF rect, bool image)
        {
            if (m_taggedPDF && Graphics.Page is PdfPage)
            {
                float pageHeight = (Graphics.Page as PdfPage).GetClientSize().Height;
                float pageWidth = (Graphics.Page as PdfPage).GetClientSize().Width;
                PdfUnitConvertor convertor = new PdfUnitConvertor();
                pageHeight = convertor.ConvertToPixels(pageHeight, PdfGraphicsUnit.Point);
                pageWidth = convertor.ConvertToPixels(pageWidth,PdfGraphicsUnit.Point);

                if (!image)
                {
                    TextRegion rgn = new TextRegion(rect.Location.Y, rect.Height);
                    TextRegions.Add(rgn);
                }

                if (pageHeight < rect.Bottom)
                {
                    // If any image is greater than the page height, force split.
                    if (image && (rect.Height > pageHeight))
                        return true;

                    //m_graphics.SetClip(new RectangleF(0, rect.Y, pageWidth, height));
                    if (m_graphics.Split > 0.0f)
                        m_graphics.Split = Math.Min(m_graphics.Split, convertor.ConvertFromPixels(rect.Y, PdfGraphicsUnit.Point));
                    else
                        m_graphics.Split = convertor.ConvertFromPixels(rect.Y, PdfGraphicsUnit.Point);

                    return false;
                }
            }
            return true;
        }
        

        /// <summary>
        /// Draws a text string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="rect">The text boundaries.</param>
        /// <param name="format">The string format.</param>
        /// <param name="textAngle">Rotation Angle</param>
        public void DrawString(string text, Font font, Brush brush, RectangleF rect, StringFormat format,float textAngle)
        {
            TextAngleLocal = textAngle;
            if (text == null)
                throw new ArgumentNullException("text");

            if (font == null)
                throw new ArgumentNullException("font");

            if (brush == null)
                throw new ArgumentNullException("brush");

            if (format == null)
                throw new ArgumentNullException("format");

                        
            OnDrawPrimitive();

            PdfBrush pdfBrush = ConvertBrush(brush);
            PdfStringFormat pdfFormat = ConvertFormat(format);

            PdfFont pdfFont = null;

            try
            {
                pdfFont = GetPdfFont(text, font);
            }
            catch (Exception ex)
            {
                //Try resolving the font from its installed path.
                string fontFile = GetInstalledFontLocation(font);
                pdfFont = new PdfTrueTypeFont(fontFile, font.Size, (PdfFontStyle)font.Style);
            }

            //Graphics.DrawRectangle( PdfPens.Black, rect );

            // Scale text according to its boundaries.
            SizeF rectSize = rect.Size;
            float scalingFactor = 1.0f;

            scalingFactor = ScaleText(text, pdfFont, rect, out rectSize, pdfFormat);

            if (scalingFactor != 1.0f /*&& pdfFormat.WordWrap == WordWrapType.None*/ )
            {
                if (pdfFormat == null)
                {
                    pdfFormat = new PdfStringFormat();
                }

                pdfFormat.HorizontalScalingFactor = scalingFactor * 100.0f;
            }

            StringFormatFlags noBoundsMask =
                StringFormatFlags.NoWrap | StringFormatFlags.NoClip | StringFormatFlags.LineLimit;

            bool notUseBounds = (((format.FormatFlags & noBoundsMask) == noBoundsMask) &&
                (format.Trimming == StringTrimming.None) && (format.Alignment == StringAlignment.Near &&
                format.LineAlignment == StringAlignment.Near));

            if (rect.Width > pdfFont.MeasureString(text, pdfFormat).Width && text.Split(null).Length > 1)
            {
                char[] symbols = StringTokenizer.Spaces;
                int whitespacesCount = StringTokenizer.GetCharsCount(text, symbols);
                float charSpace = 0f;
                float lineWidth = pdfFont.MeasureString(text, pdfFormat).Width;
                float difference = rect.Width - lineWidth;
                charSpace = difference / text.Length;
                pdfFormat.CharacterSpacing = charSpace;
            }

            //If tagged PDF, checks for possible text split.
            if (m_taggedPDF)
            {
                if (!CheckPdfPage(new RectangleF(rect.Location, new SizeF(rect.Width, Math.Max(rect.Height, pdfFont.Height))), false))
                    return;
            }

            if ((rect.Width == 0 && rect.Height == 0) || notUseBounds)
            {
                //Graphics.Save();
                //Graphics.SetTransparency( 0.5f );
                //Graphics.DrawRectangle( PdfBrushes.LightGray, rect );
                //Graphics.Restore();

                PointF location = CorrectLocation(rect.Location, rect.Size, rectSize, pdfFormat);

                if (EmbedFonts)
                    pdfFormat.RightToLeft = false;
                
                //Graphics.DrawString(text, pdfFont, pdfBrush, location, pdfFormat);
                Graphics.Save();
                Graphics.TranslateTransform(location.X, location.Y);
                Graphics.RotateTransform(textAngle);
                Graphics.DrawString(text, pdfFont, pdfBrush, PointF.Empty);
                Graphics.Restore();
            }
            else
            {
                rect.Width /= scalingFactor;

                if (rect.Width == 0)
                {
                    rect.Width = rectSize.Width;
                }
                else if (rect.Height == 0)
                {
                    rect.Height = rectSize.Height;
                }
                PointF location = CorrectLocation(rect.Location, rect.Size, rectSize, pdfFormat);
                //Graphics.Save();
                //Graphics.SetTransparency( 0.5f );
                //Graphics.DrawRectangle( PdfBrushes.LightGray, rect );
                //Graphics.Restore();

                //Skip drawing hidden text present in the metafile.

                //if (textAngle < 0 && (-textAngle > 180))
                //{
                //    textAngle = 180 + textAngle;
                //}
                if (rect.Width > 0)
                { //Graphics.DrawString(text, pdfFont, pdfBrush, rect, pdfFormat);
                    Graphics.Save();
                    Graphics.TranslateTransform(location.X, location.Y);
                    Graphics.RotateTransform(textAngle);
                    Graphics.DrawString(text, pdfFont, pdfBrush, PointF.Empty);
                    Graphics.Restore();
                }
            }
        }

        /// <summary>
        /// Corrects the location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="size">The size.</param>
        /// <param name="realSize">The real size of the text.</param>
        /// <param name="format">The format.</param>
        /// <returns>The corrected location.</returns>
        private PointF CorrectLocation(PointF location, SizeF size, SizeF realSize, PdfStringFormat format)
        {
            PointF result = location;
            if (TextAngleLocal == 0)
            {
                switch (format.Alignment)
                {
                    case PdfTextAlignment.Center:
                        result.X += size.Width / 2;
                        break;

                    case PdfTextAlignment.Right:
                        if (size.Width > realSize.Width)
                        {
                            result.X += size.Width - realSize.Width;
                        }
                        break;

                    case PdfTextAlignment.Left:
                    case PdfTextAlignment.Justify:
                    default:
                        break;
                }

                switch (format.LineAlignment)
                {
                    case PdfVerticalAlignment.Middle:
                        result.Y += size.Height / 2;
                        break;

                    case PdfVerticalAlignment.Bottom:
                        if (size.Height > realSize.Height)
                        {
                            result.Y += size.Height - realSize.Height;
                        }
                        break;

                    case PdfVerticalAlignment.Top:
                    default:
                        break;
                }
            }
            return result;
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

            Graphics.PutComment("EndContainer");
            PdfGraphicsState gs = m_graphicsStates[container] as PdfGraphicsState;

            if (gs != null)
            {
                Graphics.Restore(gs);
            }

            NativeGraphics.EndContainer(container);
            //Transform = NativeGraphics.Transform;
            m_stateChanged = true;
            m_bFirstCall = true;
            m_bFirstTransform = true;
            Transform = Transform;
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

            PdfBrush pdfBrush = ConvertBrush(brush);

            Graphics.DrawPolygon(pdfBrush, points);
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

            PdfBrush pdfBrush = ConvertBrush(brush);

            Graphics.DrawEllipse(pdfBrush, rect);
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

            PdfBrush pdfBrush = ConvertBrush(brush);

            PdfPath pdfPath = new PdfPath(path.PathPoints, path.PathTypes);

            Graphics.DrawPath(pdfBrush, pdfPath);
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
        public void FillPie(Brush brush, float x, float y, float width,
            float height, float startAngle, float sweepAngle)
        {
            if (brush == null)
                throw new ArgumentNullException("brush");

            // NOTE: Flip angles because PDF and GDI+ have different directions.
            //startAngle *= -1;
            //sweepAngle *= -1;
            OnDrawPrimitive();

            RectangleF rect = new RectangleF(x, y, width, height);
            PdfBrush pdfBrush = ConvertBrush(brush);

            Graphics.DrawPie(pdfBrush, rect, startAngle, sweepAngle);
        }

        /// <summary>
        /// Fills a polygon.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="points">The points of the polygon.</param>
        public void FillPolygon(Brush brush, PointF[] points)
        {
            if (brush == null)
                throw new ArgumentNullException("brush");

            if (points == null)
                throw new ArgumentNullException("points");

            OnDrawPrimitive();

            PdfBrush pdfBrush = ConvertBrush(brush);

            Graphics.DrawPolygon(pdfBrush, points);
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

            PdfBrush pdfBrush = ConvertBrush(brush);

            for (int i = 0; i < rects.Length; i++)
            {
                RectangleF rect = rects[i];
                Graphics.DrawRectangle(pdfBrush, rect);
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

            Matrix matrix = NativeGraphics.Transform;
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

            NativeGraphics.MultiplyTransform(matrix, order);

            Graphics.PutComment("MuliplyTransform");
            //InternalResetClip();
            //SetTransform();
            if (order == MatrixOrder.Append)
            {
                PdfTransformationMatrix m = PrepareMatrix(matrix);
                Graphics.MultiplyTransform(m);
            }
            else
            {
                //m_bFirstCall = true;
                Transform = NativeGraphics.Transform;          
            }
        }

        /// <summary>
        /// Translates the current clip region.
        /// </summary>
        /// <param name="dx">The dx.</param>
        /// <param name="dy">The dy.</param>
        public void TranslateClip(float dx, float dy)
        {
            NativeGraphics.TranslateClip(dx, dy);
            m_stateChanged = true;
        }

        /// <summary>
        /// Resets the current clip region to the infinite region.
        /// </summary>
        public void ResetClip()
        {
            NativeGraphics.ResetClip();
            m_stateChanged = true;
            m_textClip = RectangleF.Empty;
        }

        /// <summary>
        /// Resets the transformations.
        /// </summary>
        public void ResetTransform()
        {
            Graphics.PutComment("ResetTransform");
            InternalResetClip();
            SetTransform();
            NativeGraphics.ResetTransform();
        }

        /// <summary>
        /// Performs the rotate transformations.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <param name="order">The order.</param>
        public void RotateTransform(float angle, MatrixOrder order)
        {
            Graphics.PutComment("RotateTransform");
            //InternalResetClip();
            NativeGraphics.RotateTransform(angle, order);

            if (order == MatrixOrder.Append)
            {
                Graphics.RotateTransform(angle);
            }
            else
            {
                Transform = Transform;
            }
        }

        /// <summary>
        /// Saves the current graphics state.
        /// </summary>
        /// <returns>A GraphicsState instance that stores
        /// information about the current graphic state.</returns>
        public GraphicsState Save()
        {
            Graphics.PutComment("Save");
            InternalResetClip();
            InternalResetTransformation();
            PdfGraphicsState gs = Graphics.Save();
            GraphicsState ngs = NativeGraphics.Save();
            m_graphicsStates[ngs] = gs;
            //Graphics.StreamWriter.SaveGraphicsState();
            return ngs;
        }

        /// <summary>
        /// Restores the graphics state to the specified graphics state.
        /// </summary>
        /// <param name="gState">The saved graphics state.</param>
        public void Restore(GraphicsState gState)
        {
            Graphics.PutComment("Restore");
            PdfGraphicsState gs = m_graphicsStates[gState] as PdfGraphicsState;

            if (gs != null)
            {
                Graphics.Restore(gs);
            }

            NativeGraphics.Restore(gState);
            m_stateChanged = true;
            m_bFirstCall = true;
            m_bFirstTransform = false;
            m_stateRestored = true;
            //Transform = Transform;
        }

        /// <summary>
        /// Performs scaling transformations.
        /// </summary>
        /// <param name="sx">The scaling facto by x coordinate.</param>
        /// <param name="sy">The scaling facto by y coordinate.</param>
        /// <param name="order">The order.</param>
        public void ScaleTransform(float sx, float sy, MatrixOrder order)
        {
            Graphics.PutComment("ScaleTransform");
            //InternalResetClip();
            NativeGraphics.ScaleTransform(sx, sy, order);

            if (order == MatrixOrder.Append)
            {
                Graphics.ScaleTransform(sx, sy);
            }
            else
            {
                Transform = Transform;
            }
        }

        /// <summary>
        /// Sets the current clip region.
        /// </summary>
        /// <param name="path">The path specifying the clip region.</param>
        /// <param name="mode">The combining mode.</param>
        public void SetClip(GraphicsPath path, CombineMode mode)
        {
            NativeGraphics.SetClip(path, mode);

            SetClip();
        }

        /// <summary>
        /// Sets the current clip region.
        /// </summary>
        /// <param name="rect">The rectangle specifying the new clip region.</param>
        /// <param name="mode">The combining mode.</param>
        public void SetClip(RectangleF rect, CombineMode mode)
        {
            NativeGraphics.SetClip(rect, mode);
            RealClip = rect;
            m_textClip = rect;
            SetClip();
        }

        /// <summary>
        /// Sets the current clip region.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="mode">The combining mode.</param>
        public void SetClip(Region region, CombineMode mode)
        {
            NativeGraphics.SetClip(region, mode);

            SetClip();
        }

        /// <summary>
        /// Updates the clip region of this Graphics object to exclude the area specified by a Rectangle structure.
        /// </summary>
        /// <param name="rect">Rectangle structure that specifies the rectangle to exclude from the clip region.</param>
        public void ExcludeClip(System.Drawing.Rectangle rect)
        {
            NativeGraphics.ExcludeClip(rect);

            SetClip();
        }

        /// <summary>
        /// Updates the clip region of this Graphics object to exclude the area specified by a Region object.
        /// </summary>
        /// <param name="region">Region object that specifies the region to exclude from the clip region.</param>
        public void ExcludeClip(Region region)
        {
            NativeGraphics.ExcludeClip(region);

            SetClip();
        }

        /// <summary>
        /// Updates the clip region of this Graphics object to the intersection of the current clip region and the specified RectangleF structure.
        /// </summary>
        /// <param name="rect">RectangleF structure to intersect with the current clip region.</param>
        public void IntersectClip(RectangleF rect)
        {
            NativeGraphics.IntersectClip(rect);
            m_isIntersectClipRect = true;
            SetClip();
        }

        /// <summary>
        /// Updates the clip region of this Graphics object to the intersection of the current clip region and the specified Region object.
        /// </summary>
        /// <param name="region">Region object to intersect with the current region.</param>
        public void IntersectClip(Region region)
        {
            NativeGraphics.IntersectClip(region);
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
            //m_stateChanged = true;
            NativeGraphics.TransformPoints(destSpace, srcSpace, pts);
        }

        /// <summary>
        /// Sets the current rendering origin.
        /// </summary>
        /// <param name="origin">The origin.</param>
        public void SetRenderingOrigin(Point origin)
        {
            Graphics.PutComment("SetRenderingOrigin");
            //InternalResetClip();
            NativeGraphics.RenderingOrigin = origin;
            Graphics.TranslateTransform(-origin.X, -origin.Y);
            SetTransform();
        }

        /// <summary>
        /// Sets the specified transformation matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        public void SetTransform(Matrix matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            // NOTE: Because in PDF we can't just set transformation, we just modify existing matrix,
            // we have to reset current transformation and them apply new transformation.

            /*Matrix curTransform = NativeGraphics.Transform;

            // There is some transformation.
            if( !curTransform.IsIdentity && curTransform.IsInvertible )
            {
                Matrix invertable = curTransform.Clone() as Matrix;
                invertable.Invert();

                // Reset current transformation.
                NativeGraphics.Transform = invertable;
                SetTransform();
            }*/
            Graphics.PutComment("SetTransform( matrix )");
            InternalResetClip();

            //// Apply new transformation.
            //NativeGraphics.Transform = matrix;
            //PdfTransformationMatrix m = new PdfTransformationMatrix();
            //m.Matrix = matrix;
            //SetTransform();
            //Graphics.MultiplyTransform( m );
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
            Graphics.PutComment("TranslateTransform");
            //InternalResetClip();
            if (NativeGraphics != null)
                NativeGraphics.TranslateTransform(dx, dy, order);

            if (order == MatrixOrder.Append)
            {
                Graphics.TranslateTransform(dx, dy);
            }
            else
            {
                Transform = Transform;
            }
        }

        /// <summary>
        /// Is called when the metafile parsing have been started.
        /// </summary>
        public void BeforeStart()
        {
            lock (s_bmp)
            {
                m_grCache = System.Drawing.Graphics.FromImage(s_bmp);
            }

            Graphics.PutComment("BeforeStart");
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
            if (m_startState != null)
            {
                Graphics.PutComment("BeforeEnd");
                // Restore the state saved by primitives.
                // Bound was set so we need restore state.
                Graphics.Restore(m_startState);
            }
            //if( m_bounds != null )
            //{
            //  m_bounds = null;
            //  Graphics.RestoreState();
            //}

            if (m_grCache != null)
            {
                m_grCache.Dispose();
            }
        }

        /// <summary>
        /// Raises when error occured during metafile parsing.
        /// </summary>
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
            m_bFirstCall = true;
            m_grCache = null;
        }
        #endregion

#region Implementation
        /// <summary>
        /// Sets location / bounds for metafile object.
        /// </summary>
        /// <param name="location">Location of the metafile.</param>
        /// <param name="size">Size of the metafile.</param>
        internal void SetBounds(PointF location, SizeF size)
        {
            m_bounds = new PdfTransformationMatrix();

            if (!size.IsEmpty)
            {
                m_bounds.Scale(size);
            }

            if (!location.IsEmpty)
            {
                m_bounds.Translate(location.X, -location.Y);
            }
        }

        /// <summary>
        /// Sets the Boundaries box.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        internal void SetBBox(RectangleF bounds)
        {
            m_graphics.SetBBox(bounds);
        }

        /// <summary>
        /// Sets transformation matrix.
        /// </summary>
        private void SetTransform()
        {
            InternalResetClip();
            Graphics.PutComment("SetTransform");

            if (!m_bFirstTransform && !m_stateRestored)
            {
                Graphics.Restore();
            }
            else
            {
                m_bFirstTransform = false;
                m_stateRestored = false;
            }

            Graphics.Save();
        }

        /// <summary>
        /// Sets clip region.
        /// </summary>
        private void SetClip()
        {
            m_stateChanged = true;
        }

        /// <summary>
        /// Sets clip region.
        /// </summary>
        private void SetPdfClipPath()
        {
            GraphicsPath path = GetClipPath();

            if (path != null)
            {
                PointF[] points = path.PathPoints;
                byte[] types = path.PathTypes;
                PdfFillMode fillMode = GetPathFillMode(path);

                PdfPath pdfPath = new PdfPath(points, types);

                Graphics.SetClip(pdfPath, fillMode);
            }
        }

        /// <summary>
        /// Extracts fill mode of the path.
        /// </summary>
        /// <param name="path">Graphics path.</param>
        /// <returns>Fill mode of the path.</returns>
        private PdfFillMode GetPathFillMode(GraphicsPath path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            PdfFillMode rule = (path.FillMode == System.Drawing.Drawing2D.FillMode.Winding) ?
                PdfFillMode.Winding : PdfFillMode.Alternate;

            return rule;
        }

        /// <summary>
        /// Gets clip path for the graphics.
        /// </summary>
        private GraphicsPath GetClipPath()
        {
            GraphicsPath path = null;
            Region region = NativeGraphics.Clip;

            if (!region.IsEmpty(NativeGraphics) && !region.IsInfinite(NativeGraphics))
            {
                path = new GraphicsPath(System.Drawing.Drawing2D.FillMode.Winding);
                // NOTE: It requires more testing to enshure which matrix we should pass.
                //RectangleF[] rects = region.GetRegionScans( NativeGraphics.Transform );
                if (RealClip.X != 0 || RealClip.Y != 0 || RealClip.Width != 0 || RealClip.Height != 0)
                {
                    path.AddRectangle(RealClip);
                    RealClip = RectangleF.Empty;
                }
                else
                {
                    RectangleF[] rects = region.GetRegionScans(new Matrix());
                    path.AddRectangles(rects);
                }
            }

            return path;
        }

        /// <summary>
        /// Gets PDF font for the text.
        /// </summary>
        /// <param name="text">Text to be printed.</param>
        /// <param name="font">Font which will be used for printing.</param>
        /// <returns>PDF font object.</returns>
        private PdfFont GetPdfFont(string text, Font font)
        {
            bool isUnicode = false;

            if (text == null)
                throw new ArgumentNullException("text");

            if (font == null)
                throw new ArgumentNullException("font");

            if (PdfDocument.ConformanceLevel == PdfConformanceLevel.Pdf_A1B)
                isUnicode = true;
				
            else if (PdfString.IsUnicode(font.Name))            
                isUnicode = true;
			else
                isUnicode = PdfString.IsUnicode(text);

            float fontSize = font.Size;

            if (font.Name == "Wingdings"|| font.Name.ToLower() == "latha" || font.Name.ToLower() == "shruti"
                   || font.Name.ToLower() == "mangal" || font.Name.ToLower() == "tunga" || font.Name.ToLower() == "vrinda")
                isUnicode = true;
            //if (!isUnicode && EmbedFonts)
            //    isUnicode = true;
            
            PdfFont pdfFont = new PdfTrueTypeFont(font, fontSize, isUnicode);

            return pdfFont;
        }

        /// <summary>
        /// Called when we need to draw a primitive.
        /// </summary>
        private void OnDrawPrimitive()
        {
            if (m_stateChanged)
            {
                // If it's the first call to this function.
                InternalResetClip();
                m_bFirstCall = false;

                // Set new clip path and transformation.
                // Note: It's possible that some EMF+ files sets these parameters,
                // in the different order. If we had one we would fix this.
                Graphics.PutComment("OnDrawPrimitive");

                Graphics.Save();
                SetPdfClipPath();

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
        private void DrawCap(System.Drawing.Drawing2D.LineCap cap, PointF[] points, int startPointIndex,
            int endPointIndex, float width, PdfBrush brush)
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
                    Graphics.DrawPolygon(brush, triangle);
                    break;

                case System.Drawing.Drawing2D.LineCap.Round:
                    SizeF endCapSize = new SizeF(width, width);
                    RectangleF capRect = new RectangleF(points[endPointIndex], endCapSize);
                    Graphics.DrawEllipse(brush, capRect);
                    break;
                case System.Drawing.Drawing2D.LineCap.Custom:
                    if (m_customLineCapArrowData.width != 0)
                    {
                        DrawCustomLineCapArrow(cap, points, startPointIndex, endPointIndex, width, brush);
                        m_customLineCapArrowData.Reset();
                    }
                    break;
            }
        }

        /// <summary>
        /// Draws the customarrow end cap in the line
        /// </summary>
        /// <param name="cap"></param>
        /// <param name="points"></param>
        /// <param name="startPointIndex"></param>
        /// <param name="endPointIndex"></param>
        /// <param name="width"></param>
        /// <param name="brush"></param>
        private void DrawCustomLineCapArrow(System.Drawing.Drawing2D.LineCap cap, PointF[] points, int startPointIndex,
                    int endPointIndex, float width, PdfBrush brush)
        {
            PointF ePoint = points[endPointIndex];
            PointF bePoint = points[startPointIndex];
            double cos =Math.Cos((Math.PI / 180) * 30);
            double sin = Math.Sin((Math.PI / 180) *30);
            float x = ePoint.X;
            float y = ePoint.Y;
           
            float dx = (x - bePoint.X);
            float dy = (y - bePoint.Y);
        
        
            float l = (float)Math.Sqrt(dx * dx + dy * dy);
            float nx = dx / l;
            float ny = dy / l;

            float nyw = ny *m_customLineCapArrowData.width;
            float nxw = nx * m_customLineCapArrowData.width;

            PointF left = new PointF(
       (float)(x - (nxw * cos + nyw * -sin)),
       (float)(y - (nxw * sin + nyw * cos)));
            PointF right = new PointF(
                (float)(x - (nxw * cos + nyw * sin)),
                (float)(y - (nxw * -sin + nyw * cos)));
            if (m_customLineCapArrowData.fillState == 0)
            {
                PdfPen pen = new PdfPen(brush);
                Graphics.DrawLine(pen, new PointF(x, y), left);
                Graphics.DrawLine(pen, new PointF(x, y), right);
            }
            else if (m_customLineCapArrowData.fillState == 1)
            {
                PointF[] triangle = new PointF[] { new PointF(x, y), left, right };
                Graphics.DrawPolygon(brush, triangle);
            }

        }
        /// <summary>
        /// Calculates scaling factor for text that fits to the specifiedboundaries.
        /// </summary>
        /// <param name="text">String text to be scaled.</param>
        /// <param name="pdfFont">Font object.</param>
        /// <param name="rect">Text' boundaries.</param>
        /// <param name="textSize">Size of the text.</param>
        /// <param name="format">The format.</param>
        /// <returns>The scaling factor.</returns>
        private float ScaleText(string text, PdfFont pdfFont, RectangleF rect, out SizeF textSize,
            PdfStringFormat format)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            if (pdfFont == null)
                throw new ArgumentNullException("pdfFont");

            float factor = 1.0f;
            textSize = rect.Size;

            if (text.Length > 0)
            {
                if (format == null)
                {
                    format = new PdfStringFormat();
                }

                if (text.EndsWith(" "))
                {
                    format.MeasureTrailingSpaces = true;
                }

                textSize = pdfFont.MeasureString(text, format);

                if (rect.Width > 0 && rect.Width < Int32.MaxValue && textSize.Width > rect.Width)
                {
                    if (textSize.Width > 0)
                    {
                        factor = (rect.Width / textSize.Width);
                    }
                }
            }

            return factor;
        }

        /// <summary>
        /// Converts GraphicsUnits to PrintUnits.
        /// </summary>
        /// <param name="gUnits"></param>
        /// <returns>returns PrintUnits</returns>
        private PdfGraphicsUnit GraphicsToPrintUnits(GraphicsUnit gUnits)
        {
            switch (gUnits)
            {
                case GraphicsUnit.Display:
                    return PdfGraphicsUnit.Pixel;

                case GraphicsUnit.Document:
                    return PdfGraphicsUnit.Document;

                case GraphicsUnit.Inch:
                    return PdfGraphicsUnit.Inch;

                case GraphicsUnit.Millimeter:
                    return PdfGraphicsUnit.Millimeter;

                case GraphicsUnit.Pixel:
                    return PdfGraphicsUnit.Pixel;

                case GraphicsUnit.Point:
                    return PdfGraphicsUnit.Point;

                default:
                    return PdfGraphicsUnit.Point;
            }
        }

        /// <summary>
        /// Converts the .NET pen to a PDF pen.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <param name="alpha">The alpha channel value.</param>
        /// <returns>The proper PDF pen.</returns>
        private PdfPen ConvertPen(Pen pen, out float alpha, bool rotate)
        {
            PdfPen pdfPen = null;

            try
            {
                pdfPen = new PdfPen(pen.Color);
            }
            catch (ArgumentException)
            {
                pdfPen = new PdfPen(Color.Empty);
            }

            pdfPen.DashStyle = ConvertDashStyle(pen.DashStyle);

            if (pdfPen.DashStyle != PdfDashStyle.Solid)
            {
                pdfPen.DashOffset = pen.DashOffset;
                pdfPen.DashPattern = pen.DashPattern;
            }

            pdfPen.LineCap = ConvertCaps(pen.StartCap);
            pdfPen.LineCap = ConvertCaps(pen.EndCap);

            pdfPen.LineJoin = ConvertJoin(pen.LineJoin);
            pdfPen.MiterLimit = pen.MiterLimit;

            pdfPen.Width = pen.Width;

            try
            {
                alpha = (float)(pen.Color.A) / 255;
            }
            catch (ArgumentException)
            {
                alpha = 0;
            }

            if (pen.Brush != null)
            {
                float brushAlpha;
                PdfBrush brush = ConvertBrush(pen.Brush, out brushAlpha);
                PdfSolidBrush sBrush = brush as PdfSolidBrush;

                if ((sBrush != null && sBrush.Color.A != 0) || sBrush == null)
                {
                    pdfPen.Brush = brush;
                    alpha = brushAlpha;
                }

                if (pen.CompoundArray.Length != 0)
                {
                    float width = pen.Width;
                    float height = pen.Width;
                    float brushSize = (float)Math.Pow(2, (Math.Round(Math.Log(pen.Width, 2))) + 1);
                    PdfTilingBrush tBrush = new PdfTilingBrush(new SizeF(brushSize, brushSize));
                    PdfPen testPen = new PdfPen(brush);

                    testPen.Width = (pen.CompoundArray[1] - pen.CompoundArray[0]) * height;

                    if (!rotate)
                        tBrush.Graphics.DrawLine(testPen, 0, testPen.Width, brushSize, testPen.Width);
                    else
                        tBrush.Graphics.DrawLine(testPen, 0, testPen.Width, 0, brushSize);

                    testPen.Width = (pen.CompoundArray[1] - pen.CompoundArray[0]) * height;

                    if (!rotate)
                    {
                        tBrush.Graphics.TranslateTransform(0, pen.Width);
                        tBrush.Graphics.DrawLine(testPen, 0, testPen.Width / 2, brushSize, testPen.Width / 2);
                    }
                    else
                    {
                        tBrush.Graphics.TranslateTransform(pen.Width, 0);
                        tBrush.Graphics.DrawLine(testPen, testPen.Width / 2, testPen.Width, testPen.Width / 2, brushSize);
                    }

                    pdfPen.Brush = tBrush;
                }
            }

            return pdfPen;
        }

        /// <summary>
        /// Converts a .NET brush into a PDF brush.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="alpha">The alpha channel value.</param>
        /// <returns>The proper PdfBrush class instance.</returns>
        private PdfBrush ConvertBrush(Brush brush, out float alpha)
        {
            PdfBrush pdfBrush = null;
            SolidBrush sBrush = brush as SolidBrush;
            TextureBrush tBrush = brush as TextureBrush;
            LinearGradientBrush lBrush = brush as LinearGradientBrush;
            HatchBrush hBrush = brush as HatchBrush;
            PathGradientBrush pgBrush = brush as PathGradientBrush;

            alpha = 1.0f;

            if (sBrush != null)
            {
                pdfBrush = new PdfSolidBrush(sBrush.Color);
                alpha = (float)(sBrush.Color.A) / 255;
            }
            else if (tBrush != null)
            {
                Image img = tBrush.Image;
                PdfImage pdfImage = PdfImage.FromImage(img);
                PdfTilingBrush pdfTBrush = new PdfTilingBrush(img.Size);

                if (pdfImage is PdfBitmap)
                {
                    Bitmap nativeBitmap = (img as Bitmap);
                    PdfBitmap bitmap = (pdfImage as PdfBitmap);

                    PdfMask mask = CheckAlpha(nativeBitmap);

                    if (mask != null)
                    {
                        bitmap.Mask = mask;
                    }

                    pdfTBrush.Graphics.DrawImage(bitmap, PointF.Empty, img.Size);
                }
                else
                {
                    pdfTBrush.Graphics.DrawImage(pdfImage, PointF.Empty, img.Size);
                }

                pdfBrush = pdfTBrush;
            }
            else if (lBrush != null)
            {
                PdfLinearGradientBrush pdfLBrush = null;
                RectangleF bounds = lBrush.Rectangle;
                Color color1 = Color.Empty;
                Color color2 = Color.Empty;

                Color[] linearColors = lBrush.LinearColors;
                ColorBlend ncBlend = null;

                if (linearColors != null)
                {
                    color1 = linearColors[0];
                    color2 = linearColors[1];
                }

                if (color1.R == 0 && color1.G == 0 && color1.B == 0
                    && lBrush.WrapMode == WrapMode.TileFlipX)
                {
                    color1 = linearColors[1];
                    color2 = linearColors[0];
                }

            
                try
                {
                    ncBlend = lBrush.InterpolationColors;
                    if (ncBlend != null)
                    {
                        color1 = ncBlend.Colors[0];
                        color2 = color1;
                    }
                }
                catch
                {
                }
                
                if (lBrush.Transform.OffsetX > 0)
                {
                    pdfLBrush = new PdfLinearGradientBrush(bounds,color1, color2, PdfLinearGradientMode.Vertical);

                }
                else
                {
                    pdfLBrush = new PdfLinearGradientBrush(bounds, color1, color2, PdfLinearGradientMode.Horizontal);
                    PdfTransformationMatrix m = new PdfTransformationMatrix();
                    m.Matrix = lBrush.Transform;

                    pdfLBrush.Matrix = m;
                    
                }
                

                if (ncBlend != null)
                {
                    Color[] colours = ncBlend.Colors;
                    PdfColorBlend cBlend = new PdfColorBlend(colours.Length);
                    cBlend.Colors = ConvertColors(colours);
                    cBlend.Positions = ncBlend.Positions;
                    pdfLBrush.InterpolationColors = cBlend;
                }
                else
                {
                    Blend nBlend = lBrush.Blend;

                    if (nBlend != null)
                    {
                        PdfBlend blend = new PdfBlend();
                        blend.Factors = nBlend.Factors;
                        blend.Positions = nBlend.Positions;
                        pdfLBrush.Blend = blend;
                    }
                }

                if (lBrush.WrapMode == WrapMode.Tile
                    || lBrush.WrapMode == WrapMode.TileFlipX)
                {
                    pdfLBrush.Extend = PdfExtend.Both;
                }


                alpha = (float)(color1.A) / 255;
                pdfBrush = pdfLBrush;
            }
            else if (hBrush != null)
            {
                pdfBrush = ConvertHatchBrush(hBrush, out alpha);
            }
            else if (pgBrush != null)
            {
                // TODO: Draw correct brush using gradient brush.
                pdfBrush = new PdfSolidBrush(Color.Black);
                alpha = (float)pgBrush.CenterColor.A / 255;
            }
            else
            {
                throw new ArgumentException("Unsupported brush type: " + brush, "brush");
            }
            return pdfBrush;
        }

        /// <summary>
        /// Checks the alpha.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <returns>The proper transparency mask.</returns>
        private PdfMask CheckAlpha(Bitmap bitmap)
        {
            PdfMask mask = null;

            switch (bitmap.PixelFormat)
            {
                case PixelFormat.Format1bppIndexed:
                case PixelFormat.Format4bppIndexed:
                case PixelFormat.Format8bppIndexed:
                    {
                        Color[] array = bitmap.Palette.Entries;
                        int flags = bitmap.Palette.Flags;
                        mask = CheckAlpha(flags, bitmap, array);
                    }
                    break;

                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb:
                    mask = new PdfImageMask(new PdfBitmap(PdfBitmap.CreateMaskFromARGBImage(bitmap)));
                    break;

                default:
                    // Unsupported format.
                    break;
            }

            return mask;
        }

        /// <summary>
        /// Checks if the alpha channel is present.
        /// </summary>
        /// <param name="flags">The flags.</param>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="array">The array.</param>
        /// <returns>The proper mask.</returns>
        private PdfMask CheckAlpha(int flags, Image bitmap, Color[] array)
        {
            PdfMask mask = null;
            bool useMask = false;

            for (int i = 0, cnt = array.Length; i < cnt; ++i)
            {
                if (flags == 0x01)
                {
                    int alpha = array[i].A;

                    if (alpha < 255)
                    {
                        useMask = true;
                    }
                }
            }

            if (useMask)
            {
                mask = new PdfImageMask(new PdfBitmap(PdfBitmap.CreateMaskFromIndexedImage(bitmap)));
            }

            return mask;
        }

        /// <summary>
        /// Converts a .NET hatch brush to a PDF tiling brush.
        /// </summary>
        /// <param name="hatchBrush">The hatch brush.</param>
        /// <param name="alpha">The alpha channel value.</param>
        /// <returns>The well formed PdfBrush instance.</returns>
        private PdfBrush ConvertHatchBrush(HatchBrush hatchBrush, out float alpha)
        {
            Color colorFore = hatchBrush.ForegroundColor;
            Color colorBack = hatchBrush.BackgroundColor;
            SizeF brushSize = new SizeF(8, 8);
            PdfTilingBrush brush = new PdfTilingBrush(brushSize);
            PdfGraphics g = brush.Graphics;
            PdfPen pen = new PdfPen(colorFore, 1);

            alpha = (float)colorFore.A / 255.0f;

            if (!colorBack.IsEmpty && colorBack.A != 0)
            {
                g.DrawRectangle(new PdfSolidBrush(colorBack), new RectangleF(PointF.Empty, brushSize));
            }

            switch (hatchBrush.HatchStyle)
            {
                case HatchStyle.BackwardDiagonal:
                    DrawBackwardDiagonal(g, pen, brushSize);
                    break;

                case HatchStyle.Cross:
                    DrawCross(g, pen, brushSize);
                    break;

                case HatchStyle.DarkDownwardDiagonal:
                    pen.Width = 2.0f;
                    DrawDownwardDiagonal(g, pen, brushSize);
                    break;

                case HatchStyle.DarkHorizontal:
                    pen.Width = 2.0f;
                    DrawHorizontal(g, pen, brushSize);
                    break;

                case HatchStyle.DarkUpwardDiagonal:
                    pen.Width = 2.0f;
                    DrawUpwardDiagonal(g, pen, brushSize);
                    break;

                case HatchStyle.DarkVertical:
                    pen.Width = 2.0f;
                    DrawVertical(g, pen, brushSize);
                    break;

                case HatchStyle.ForwardDiagonal:
                    DrawForwardDiagonal(g, pen, brushSize);
                    break;

                case HatchStyle.DashedDownwardDiagonal:
                    pen.DashStyle = PdfDashStyle.Dash;
                    DrawDownwardDiagonal(g, pen, brushSize);
                    break;

                case HatchStyle.DashedHorizontal:
                    pen.DashStyle = PdfDashStyle.Dash;
                    DrawHorizontal(g, pen, brushSize);
                    break;

                case HatchStyle.DashedUpwardDiagonal:
                    pen.DashStyle = PdfDashStyle.Dash;
                    DrawUpwardDiagonal(g, pen, brushSize);
                    break;

                case HatchStyle.DashedVertical:
                    pen.DashStyle = PdfDashStyle.Dash;
                    DrawVertical(g, pen, brushSize);
                    break;

                case HatchStyle.DiagonalBrick:
                    DrawForwardDiagonal(g, pen, brushSize);
                    DrawBrickTails(g, pen, brushSize);
                    break;

                case HatchStyle.DiagonalCross:
                    DrawForwardDiagonal(g, pen, brushSize);
                    DrawBackwardDiagonal(g, pen, brushSize);
                    break;

                case HatchStyle.Divot:
                    // TODO: implement Divot pattern.
                    break;

                case HatchStyle.DottedDiamond:
                    pen.DashStyle = PdfDashStyle.Dot;
                    DrawForwardDiagonal(g, pen, brushSize);
                    DrawBackwardDiagonal(g, pen, brushSize);
                    break;

                case HatchStyle.DottedGrid:
                    pen.DashStyle = PdfDashStyle.Dot;
                    DrawCross(g, pen, brushSize);
                    break;

                case HatchStyle.Horizontal:
                    DrawHorizontal(g, pen, brushSize);
                    break;

                case HatchStyle.HorizontalBrick:
                    DrawHorizontalBrick(g, pen, brushSize);
                    break;

                case HatchStyle.LargeCheckerBoard:
                    DrawCheckerBoard(g, pen, brushSize, 4);
                    break;

                case HatchStyle.LargeConfetti:
                    // TODO: implement LargeConfetti pattern.
                    break;

                case HatchStyle.LightDownwardDiagonal:
                    DrawDownwardDiagonal(g, pen, brushSize);
                    break;

                case HatchStyle.LightHorizontal:
                    DrawHorizontal(g, pen, brushSize);
                    break;

                case HatchStyle.LightUpwardDiagonal:
                    DrawUpwardDiagonal(g, pen, brushSize);
                    break;

                case HatchStyle.LightVertical:
                    DrawVertical(g, pen, brushSize);
                    break;
                case HatchStyle.Weave:
                    DrawWeave(g, pen, brushSize);
                    break;

                //TODO: implement others.
                case HatchStyle.NarrowHorizontal:
                case HatchStyle.NarrowVertical:
                case HatchStyle.OutlinedDiamond:
                case HatchStyle.Percent05:
                case HatchStyle.Percent10:
                case HatchStyle.Percent20:
                case HatchStyle.Percent25:
                case HatchStyle.Percent30:
                case HatchStyle.Percent40:
                case HatchStyle.Percent50:
                case HatchStyle.Percent60:
                case HatchStyle.Percent70:
                case HatchStyle.Percent75:
                case HatchStyle.Percent80:
                case HatchStyle.Percent90:
                case HatchStyle.Plaid:
                case HatchStyle.Shingle:
                case HatchStyle.SmallCheckerBoard:
                case HatchStyle.SmallConfetti:
                case HatchStyle.SmallGrid:
                case HatchStyle.SolidDiamond:
                case HatchStyle.Sphere:
                case HatchStyle.Trellis:
                case HatchStyle.Vertical:
                case HatchStyle.Wave:
                case HatchStyle.WideDownwardDiagonal:
                case HatchStyle.WideUpwardDiagonal:
                case HatchStyle.ZigZag:
                default:
                    alpha = 0.5f;
                    brush = null;
                    break;
            }

            if (brush == null)
            {
                return new PdfSolidBrush(colorFore);
            }

            return brush;

        }

        /// <summary>
        /// Converts the .NET colors to PdfColor array.
        /// </summary>
        /// <param name="colors">The colors.</param>
        /// <returns>The well formed PdfColor array.</returns>
        internal static PdfColor[] ConvertColors(Color[] colors)
        {
            int count = colors.Length;
            PdfColor[] pdfColors = new PdfColor[count];

            for (int i = 0; i < count; ++i)
            {
                Color color = colors[i];
                pdfColors[i] = color;
            }

            return pdfColors;
        }

        /// <summary>
        /// Gets the brush from pen.
        /// </summary>
        /// <param name="pdfPen">The PDF pen.</param>
        /// <returns>The brush initialized from pen parameters.</returns>
        private PdfBrush GetBrushFromPen(PdfPen pdfPen)
        {
            PdfBrush brush = pdfPen.Brush;

            if (brush == null)
            {
                brush = new PdfSolidBrush(pdfPen.Color);
            }

            return brush;
        }

        /// <summary>
        /// Converts the System.Drawing.StringFormat format to
        /// Syncfusion.Pdf.Graphics.PdfStringFormat format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>Proper Syncfusion.Pdf.Graphics.PdfStringFormat format.</returns>
        private PdfStringFormat ConvertFormat(StringFormat format)
        {
            PdfStringFormat pdfFormat = null;

            if (format != null)
            {
                Graphics.PutComment("String Format Flags: " + format.FormatFlags + "(" +
                    (int)format.FormatFlags + ")");

                Graphics.PutComment("Alignment: " + format.Alignment);
                Graphics.PutComment("Line Alignment: " + format.LineAlignment);
                pdfFormat = new PdfStringFormat();
                pdfFormat.LineLimit = false;

                pdfFormat.Alignment = ConvertAlingnmet(format.Alignment);
                pdfFormat.LineAlignment = CovertLineAlignment(format.LineAlignment);

                float firstTabOffset;
                format.GetTabStops(out firstTabOffset);
                //pdfFormat.TabSize = firstTabOffset;

                pdfFormat.NoClip = true;// ( format.FormatFlags & StringFormatFlags.NoClip ) != 0;
                pdfFormat.RightToLeft = (format.FormatFlags & StringFormatFlags.DirectionRightToLeft) != 0;
                if (pdfFormat.NoClip)
                {
                    pdfFormat.LineLimit = false;
                }

                pdfFormat.WordWrap = GetWrapType(format.FormatFlags);
            }

            return pdfFormat;
        }

        /// <summary>
        /// Gets the type of the wrap.
        /// </summary>
        /// <param name="stringFormatFlags">The string format flags.</param>
        /// <returns>Proper StringWrapType value.</returns>
        private PdfWordWrapType GetWrapType(StringFormatFlags stringFormatFlags)
        {
            PdfWordWrapType wrapType = PdfWordWrapType.Word;

            if ((stringFormatFlags & StringFormatFlags.NoWrap) != 0)
            {
                wrapType = PdfWordWrapType.None;
            }

            return wrapType;
        }

        /// <summary>
        /// Converts the line alignment.
        /// </summary>
        /// <param name="stringAlignment">The string alignment.</param>
        /// <returns>The proper VerticalAlignment value.</returns>
        internal static PdfVerticalAlignment CovertLineAlignment(StringAlignment stringAlignment)
        {
            PdfVerticalAlignment va = PdfVerticalAlignment.Middle;

            switch (stringAlignment)
            {
                case StringAlignment.Far:
                    va = PdfVerticalAlignment.Bottom;
                    break;

                case StringAlignment.Near:
                    va = PdfVerticalAlignment.Top;
                    break;

                case StringAlignment.Center:
                default:
                    va = PdfVerticalAlignment.Middle;
                    break;
            }

            return va;
        }

        /// <summary>
        /// Converts the alingnmet.
        /// </summary>
        /// <param name="stringAlignment">The string alignment.</param>
        /// <returns>The proper TextAlignment value.</returns>
        internal static PdfTextAlignment ConvertAlingnmet(StringAlignment stringAlignment)
        {
            PdfTextAlignment ta = PdfTextAlignment.Left;

            switch (stringAlignment)
            {
                case StringAlignment.Far:
                    ta = PdfTextAlignment.Right;
                    break;

                case StringAlignment.Center:
                    ta = PdfTextAlignment.Center;
                    break;

                case StringAlignment.Near:
                default:
                    ta = PdfTextAlignment.Left;
                    break;
            }

            return ta;
        }

        private PdfPen ConvertPen(Pen pen)
        {
            return ConvertPen(pen, false);
        }

        /// <summary>
        /// Converts a .NET pen to a PDF pen and sets transparency.
        /// </summary>
        /// <param name="pen">The pen.</param>
        /// <returns></returns>
        private PdfPen ConvertPen(Pen pen, bool rotate)
        {
            float alpha;
            PdfPen pdfPen = ConvertPen(pen, out alpha, rotate);
            if (alpha == 0f)
            {
                pdfPen = new PdfPen(PdfBrushes.White);
            }
            else
            {
                if (IsTranparency)
                {
                    Graphics.SetTransparency(AlphaPen, AlphaBrush, BlendMode);
                }
                else
                {
                    Graphics.SetTransparency(alpha, alpha, PdfBlendMode.Normal);
                }
            }
            return pdfPen;
        }

        /// <summary>
        /// Converts a .NET brush to a PDF brush and sets transparency.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <returns></returns>
        private PdfBrush ConvertBrush(Brush brush)
        {
            float alpha;
            PdfBlendMode blendMode = PdfBlendMode.Normal;
            PdfBrush pdfBrush = ConvertBrush(brush, out alpha);

            //if (brush is SolidBrush)
            //{
            //    if ((brush as SolidBrush).Color.A.Equals((byte)255))
            //        blendMode = PdfBlendMode.Multiply;
            //}
            //if (alpha == 0)
            //{
            //    pdfBrush = PdfBrushes.White;
            //}
            //else
            //{
                if (IsTranparency)
                {
                    Graphics.SetTransparency(AlphaPen, AlphaBrush, BlendMode);
                }
                else
                {
                    Graphics.SetTransparency(alpha, alpha, blendMode);
                }
            //}
            return pdfBrush;
        }

        /// <summary>
        /// Internally resets clip region.
        /// </summary>
        private void InternalResetClip()
        {
            if (!m_bFirstCall)
            {
                Graphics.PutComment("InternalResetClip");
                m_bFirstCall = true;
                Graphics.Restore();
                m_stateChanged = true;
            }
        }

        /// <summary>
        /// Internally resets transformation.
        /// </summary>
        private void InternalResetTransformation()
        {
            if (!m_bFirstTransform)
            {
                Graphics.PutComment("InternalResetTransformation");
                Graphics.Restore();
            }

            m_bFirstTransform = true;
        }

        /// <summary>
        /// Draws custom cap.
        /// </summary>
        /// <param name="pen">Pen used to draw cap.</param>
        /// <param name="points">Path points.</param>
        /// <param name="penPoints">Custom points for cap.</param>
        /// <param name="isStartCap">Indicates whether cap is start.</param>
        private void DrawCustomCap(Pen pen, PointF[] points, PointF[] penPoints, bool isStartCap)
        {
            if (penPoints != null)
            {
                PdfGraphicsState state = Graphics.Save();

                PointF firstP = PointF.Empty;
                PointF secondP = PointF.Empty;

                if (isStartCap)
                {
                    firstP = points[1];
                    secondP = points[0];
                }
                else
                {
                    firstP = points[points.Length - 2];
                    secondP = points[points.Length - 1];
                }

                float dx = firstP.X - secondP.X;
                float dy = firstP.Y - secondP.Y;
                float d = (float)Math.Sqrt(dx * dx + dy * dy);

                float kx = dx / dy;
                float ky = dy / dx;

                float angle = (float)Math.Atan(ky);

                float cos = dx / d;

                if (cos < 0)
                {
                    angle += (float)(Math.PI);
                }

                Graphics.TranslateTransform(secondP.X, secondP.Y);
                Graphics.RotateTransform((float)((angle) * 180.0 / Math.PI) + 90);

                PointF[] modifiedPoints = new PointF[penPoints.Length];

                GraphicsPath path = new GraphicsPath();
                float newWidth = pen.Width / 2;

                modifiedPoints[0].X = penPoints[0].X * (pen.Width - newWidth);
                modifiedPoints[0].Y = penPoints[0].Y * (pen.Width);

                for (int i = 1; i < penPoints.Length; ++i)
                {
                    modifiedPoints[i].X = penPoints[i].X * (pen.Width - newWidth);
                    modifiedPoints[i].Y = penPoints[i].Y * (pen.Width);

                    path.AddLine(modifiedPoints[i - 1], modifiedPoints[i]);
                }

                pen.Width = newWidth;
                DrawPath(pen, path);

                Graphics.Restore(state);
            }
        }

        /// <summary>
        /// Checks is points line or not. 
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

#region Static methods
        /// <summary>
        /// Converts a System.Drawing.Drawing2D.LineCap value to the Syncfusion.Pdf.Graphics.LineCap.
        /// </summary>
        /// <param name="cap">The cap value.</param>
        /// <returns>Syncfusion.Pdf.Graphics.LineCap type value.</returns>
        internal static PdfLineCap ConvertCaps(System.Drawing.Drawing2D.LineCap cap)
        {
            PdfLineCap result = PdfLineCap.Flat;

            switch (cap)
            {
                case System.Drawing.Drawing2D.LineCap.Square:
                    result = PdfLineCap.Square;
                    break;

                case System.Drawing.Drawing2D.LineCap.Round:
                    result = PdfLineCap.Round;
                    break;

                case System.Drawing.Drawing2D.LineCap.Flat:
                default:
                    result = PdfLineCap.Flat;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Converts a System.Drawing.Drawing2D.LineJoin values to respective
        /// Syncfusion.Pdf.Graphics.LineJoin values.
        /// </summary>
        /// <param name="join">The join value.</param>
        /// <returns>Proper Syncfusion.Pdf.Graphics.LineJoin value.</returns>
        internal static PdfLineJoin ConvertJoin(System.Drawing.Drawing2D.LineJoin join)
        {
            PdfLineJoin result = PdfLineJoin.Miter;

            switch (join)
            {
                case System.Drawing.Drawing2D.LineJoin.Bevel:
                    result = PdfLineJoin.Bevel;
                    break;

                case System.Drawing.Drawing2D.LineJoin.Round:
                    result = PdfLineJoin.Round;
                    break;

                case System.Drawing.Drawing2D.LineJoin.Miter:
                case System.Drawing.Drawing2D.LineJoin.MiterClipped:
                default:
                    result = PdfLineJoin.Miter;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Converts a System.Drawing.Drawing2D.DashStyle dash style to the corresponding Syncfusion.Pdf.Graphics.DashStyle dash style.
        /// </summary>
        /// <param name="dashStyle">The dash style.</param>
        /// <returns>Proper Syncfusion.Pdf.Graphics.DashStyle value.</returns>
        internal static PdfDashStyle ConvertDashStyle(System.Drawing.Drawing2D.DashStyle dashStyle)
        {
            PdfDashStyle result = PdfDashStyle.Solid;

            switch (dashStyle)
            {
                case System.Drawing.Drawing2D.DashStyle.Custom:
                    result = PdfDashStyle.Custom;
                    break;

                case System.Drawing.Drawing2D.DashStyle.Dash:
                    result = PdfDashStyle.Dash;
                    break;

                case System.Drawing.Drawing2D.DashStyle.DashDot:
                    result = PdfDashStyle.DashDot;
                    break;

                case System.Drawing.Drawing2D.DashStyle.DashDotDot:
                    result = PdfDashStyle.DashDotDot;
                    break;

                case System.Drawing.Drawing2D.DashStyle.Dot:
                    result = PdfDashStyle.Dot;
                    break;

                case System.Drawing.Drawing2D.DashStyle.Solid:
                default:
                    result = PdfDashStyle.Solid;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Prepares a matrix to PDF.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>A properly prepared PdfTransformationMatrix class instance.</returns>
        private static PdfTransformationMatrix PrepareMatrix(Matrix matrix)
        {
            PdfTransformationMatrix pdfMatrix = new PdfTransformationMatrix();
            PdfTransformationMatrix m1 = new PdfTransformationMatrix();
            m1.Matrix = matrix;
            pdfMatrix.Scale(1, -1);
            pdfMatrix.Multiply(m1);
            pdfMatrix.Scale(1, -1);

            return pdfMatrix;
        }

        /// <summary>
        /// Prepares a matrix to PDF.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="pageScale">The page scale value.</param>
        /// <returns>
        /// A properly prepared PdfTransformationMatrix class instance.
        /// </returns>
        private static PdfTransformationMatrix PrepareMatrix(Matrix matrix, float pageScale)
        {
            PdfTransformationMatrix pdfMatrix = new PdfTransformationMatrix();
            PdfTransformationMatrix m1 = new PdfTransformationMatrix();
            m1.Matrix = matrix;
            pdfMatrix.Scale(pageScale, -pageScale);
            pdfMatrix.Multiply(m1);
            pdfMatrix.Scale(1, -1);

            return pdfMatrix;
        }

#region Draw Different hatch brush patterns
        /// <summary>
        /// Draws the cross brush pattern.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="brushSize">Size of the brush.</param>
        private static void DrawCross(PdfGraphics graphics, PdfPen pen, SizeF brushSize)
        {
            float x1 = brushSize.Width / 2;
            float y1 = brushSize.Height / 2;
            graphics.DrawLine(pen, x1, 0, x1, brushSize.Height);
            graphics.DrawLine(pen, 0, y1, brushSize.Width, y1);
        }

        /// <summary>
        /// Draws the backward diagonal brush pattern.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="brushSize">Size of the brush.</param>
        private static void DrawBackwardDiagonal(PdfGraphics graphics, PdfPen pen, SizeF brushSize)
        {
            graphics.DrawLine(pen, brushSize.Width, 0, 0, brushSize.Height);
            graphics.DrawLine(pen, -1, 1, 1, -1);
            graphics.DrawLine(pen, brushSize.Width - 1, brushSize.Height + 1, brushSize.Width + 1, brushSize.Height - 1);
        }

        /// <summary>
        /// Draws the forward diagonal brush pattern.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="brushSize">Size of the brush.</param>
        private static void DrawForwardDiagonal(PdfGraphics graphics, PdfPen pen, SizeF brushSize)
        {
            graphics.DrawLine(pen, 0, 0, brushSize.Width, brushSize.Height);
            graphics.DrawLine(pen, -1, -1, 1, 1);
            graphics.DrawLine(pen, brushSize.Width - 1, brushSize.Height - 1, brushSize.Width + 1, brushSize.Height + 1);
        }

        /// <summary>
        /// Draws the horizontal brush pattern.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="brushSize">Size of the brush.</param>
        private static void DrawHorizontal(PdfGraphics graphics, PdfPen pen, SizeF brushSize)
        {
            float y1 = 0;
            float y2 = brushSize.Height / 2;
            float y3 = brushSize.Height;

            graphics.DrawLine(pen, 0, y1, brushSize.Width, y1);
            graphics.DrawLine(pen, 0, y2, brushSize.Width, y2);
            graphics.DrawLine(pen, 0, y3, brushSize.Width, y3);
        }

        /// <summary>
        /// Draws the vertical pattern.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="brushSize">Size of the brush.</param>
        private static void DrawVertical(PdfGraphics graphics, PdfPen pen, SizeF brushSize)
        {
            float x1 = 0;
            float x2 = brushSize.Height / 2;
            float x3 = brushSize.Height;

            graphics.DrawLine(pen, x1, 0, x1, brushSize.Height);
            graphics.DrawLine(pen, x2, 0, x2, brushSize.Height);
            graphics.DrawLine(pen, x3, 0, x3, brushSize.Height);
        }

        /// <summary>
        /// Draws the downward diagonal brush pattern.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="brushSize">Size of the brush.</param>
        private static void DrawDownwardDiagonal(PdfGraphics graphics, PdfPen pen, SizeF brushSize)
        {
            float h2 = brushSize.Height / 2;
            float w2 = brushSize.Width / 2;

            graphics.DrawLine(pen, 0, 0, brushSize.Width, brushSize.Height);
            graphics.DrawLine(pen, 0, h2, w2, brushSize.Height);
            graphics.DrawLine(pen, w2, 0, brushSize.Width, h2);
            graphics.DrawLine(pen, -1, -1, 1, 1);
            graphics.DrawLine(pen, brushSize.Width - 1, brushSize.Height - 1, brushSize.Width + 1, brushSize.Height + 1);
        }

        /// <summary>
        /// Draws Weave style.
        /// </summary>
        /// <param name="g">Pdf Graphics on which style draws.</param>
        /// <param name="pen">Pdf pen which draws style.</param>
        /// <param name="brushSize">The size of the brush.</param>
        private void DrawWeave(PdfGraphics g, PdfPen pen, SizeF brushSize)
        {
            g.TranslateTransform(-0.5f, -0.5f);

            g.DrawLine(pen, new PointF(0, 0), new PointF(0.5f, 0.5f));
            g.DrawLine(pen, new PointF(0f, 1.0f), new PointF(1f, 0));
            g.DrawLine(pen, new PointF(0, 5), new PointF(5, 0));
            g.DrawLine(pen, new PointF(0, 4), new PointF(5f, 9f));
            g.DrawLine(pen, new PointF(2.5f, 2.5f), new PointF(9, 9));
            g.DrawLine(pen, new PointF(4, 0), new PointF(6.5f, 2.5f));
            g.DrawLine(pen, new PointF((float)(6.5f - Math.Sqrt(0.25 / 2)), (float)(2.5f + Math.Sqrt(0.25 / 2))), new PointF(9, 0.0f));
            g.DrawLine(pen, new PointF(6.5f, 6.5f), new PointF(9, 4));
            g.DrawLine(pen, new PointF(2.5f, 6.5f), new PointF(0.5f, 8.5f));
        }

        /// <summary>
        /// Draws the upward diagonal brush pattern.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="brushSize">Size of the brush.</param>
        private static void DrawUpwardDiagonal(PdfGraphics graphics, PdfPen pen, SizeF brushSize)
        {
            float h2 = brushSize.Height / 2;
            float w2 = brushSize.Width / 2;

            graphics.DrawLine(pen, brushSize.Width, 0, 0, brushSize.Height);
            graphics.DrawLine(pen, 0, h2, w2, 0);
            graphics.DrawLine(pen, w2, brushSize.Height, brushSize.Width, h2);
            graphics.DrawLine(pen, -1, 1, 1, -1);
            graphics.DrawLine(pen, brushSize.Width - 1, brushSize.Height + 1, brushSize.Width + 1, brushSize.Height - 1);
        }

        /// <summary>
        /// Draws the brick tails for the brick pattern.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="brushSize">Size of the brush.</param>
        private static void DrawBrickTails(PdfGraphics graphics, PdfPen pen, SizeF brushSize)
        {
            float x2 = brushSize.Width / 2;
            float y2 = brushSize.Height / 2;

            graphics.DrawLine(pen, x2, y2, brushSize.Width, brushSize.Height);
        }

        /// <summary>
        /// Draws the horizontal brick pattern.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="brushSize">Size of the brush.</param>
        private static void DrawHorizontalBrick(PdfGraphics graphics, PdfPen pen, SizeF brushSize)
        {
            float x2 = brushSize.Width / 2;
            float y2 = brushSize.Height / 2;

            graphics.DrawLine(pen, 0, 0, brushSize.Width, 0);
            graphics.DrawLine(pen, 0, brushSize.Height, brushSize.Width, brushSize.Height);
            graphics.DrawLine(pen, 0, y2, brushSize.Width, y2);
            graphics.DrawLine(pen, x2, 0, x2, y2);
            graphics.DrawLine(pen, 0, y2, 0, brushSize.Height);
            graphics.DrawLine(pen, brushSize.Width, y2, brushSize.Width, brushSize.Height);
        }

        /// <summary>
        /// Draws a checker board dash pattern.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="brushSize">Size of the brush.</param>
        /// <param name="cellSize">Size of the cell.</param>
        private static void DrawCheckerBoard(PdfGraphics graphics, PdfPen pen, SizeF brushSize, int cellSize)
        {
            int xCount = (int)(brushSize.Width / cellSize);
            int yCount = (int)(brushSize.Height / cellSize);
            PdfSolidBrush brush = new PdfSolidBrush(pen.Color);

            for (int j = 0; j < yCount; ++j)
            {
                float y = j * cellSize;

                for (int i = 0; i < xCount; ++i)
                {
                    float x = i * cellSize;

                    graphics.DrawRectangle(brush, x, y, cellSize, cellSize);
                }
            }
        }
        #endregion

        #endregion

#region Helper Method
        /// <summary>
        /// Locates the font file.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <remarks>Not the best way, but will work in most cases incase font substitution fails.</remarks>
        /// <returns></returns>
        private string GetInstalledFontLocation(Font font)
        {
            string fontKey = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts";
            RegistryKey rKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(fontKey, false);
            string[] subKeys = rKey.GetValueNames();

            string winDirPath = Environment.GetEnvironmentVariable("SystemRoot");
            string fileName = "";
            string fontName = string.Format("{0} {1}", font.Name, GetFontSuffix(font.Style));

            for (int i = 0, count = subKeys.Length; i < count; i++)
            {
                if (subKeys[i].Contains(fontName))
                {
                    fileName = rKey.GetValue(subKeys[i]).ToString();
                    return string.Format("{0}\\Fonts\\{1}", winDirPath, fileName);
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the font suffix.
        /// </summary>
        /// <param name="fs">The fs.</param>
        /// <returns></returns>
        private string GetFontSuffix(FontStyle fs)
        {
            string styleSuffix = "";
            if (fs == FontStyle.Bold)
                styleSuffix = "Bold";
            else if (fs == FontStyle.Italic)
                styleSuffix = "Italic";
            else if (fs == (FontStyle.Bold | FontStyle.Italic))
                styleSuffix = "Bold Italic";

            return styleSuffix;
        }
        #endregion

    }

    internal struct CustomLineCapArrowData
    {
        internal float width;
        internal float height;
        internal float middleInset;
        internal int fillState;
        internal int lineStartCap;
        internal int lineEndCap;
        internal int lineJoin;
        internal float lineMitterLimit;
        internal float widthScale;

        internal void Reset()
        {
            width = 0;
            height = 0;
            middleInset = 0;
            fillState = 0;
            fillState = 0;
            lineEndCap = 0;
            lineEndCap = 0;
            lineJoin = 0;
            lineMitterLimit = 0;
            widthScale = 0;
        }
    }
}
#endif