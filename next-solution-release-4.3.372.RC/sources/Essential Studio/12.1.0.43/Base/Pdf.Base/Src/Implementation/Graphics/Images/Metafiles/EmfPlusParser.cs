#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT && !NETFX_CORE && !WP
using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.Graphics.Images.Metafiles
{
    /// <summary>
    /// Class for parsing EmfPlus metafiles.
    /// </summary>
    internal class EmfPlusParser : EmfParser
    {
#region Constants
        /// <summary>
        /// Flag for recognizing type of region.
        /// </summary>
        private const int RegionFlag = 0x10000000;

        /// <summary>
        /// Flag for objects recognizing.
        /// </summary>
        private const int ObjectFlag = 0xff00;

        /// <summary>
        /// Flag for object index recognizing.
        /// </summary>
        private const int IndexFlag = 0xff;

        /// <summary>
        /// Represents path filling type.
        /// </summary>
        private const int PathFillWinding = 0x2000;

        /// <summary>
        /// Flag indicating whether color is in the data or it's index of the pen/brush.
        /// </summary>
        private const int ColorFlag = 0x8000;

        /// <summary>
        /// Represents flag indicating whether short type should be used.
        /// </summary>
        private const int UseShorts = 0x4000;

        #endregion

#region Fields
        /// <summary>
        /// Storage of help objects.
        /// </summary>
        private ObjectData m_objects = new ObjectData();

        private PdfUnitConvertor m_convertX;
        private PdfUnitConvertor m_convertY;

        /// <summary>
        /// Type of metafile.
        /// </summary>
        private MetafileType m_type;

        /// <summary>
        /// Indicates whether we should start emf record processing.
        /// </summary>
        private bool m_bProcess;

        /// <summary>
        /// Store the path of current pen's end cap.
        /// </summary>
        private GraphicsPath m_currentPenCap;

        /// <summary>
        /// Flag for connecting last and first points.
        /// </summary>
        private int LineCloseFlag = 8192;
        #endregion

#region Properties
        /// <summary>
        /// Overloaded. Returns metafile type.
        /// </summary>
        public override MetafileType Type
        {
            get
            {
                return m_type;
            }
        }

        /// <summary>
        /// Gets a collection of objects in the stack.
        /// </summary>
        private ObjectData Objects
        {
            get
            {
                return m_objects;
            }
        }

        #endregion

#region Constructors

        /// <summary>
        /// Creates new object.
        /// </summary>
        /// <param name="dpi">The dpi.</param>
        public EmfPlusParser(MetafileType type, SizeF dpi)
            : base(dpi)
        {
            m_type = type;
            m_convertX = new PdfUnitConvertor(dpi.Width);
            m_convertY = new PdfUnitConvertor(dpi.Height);
        }

        /// <summary>
        /// Creates new object.
        /// </summary>
        /// <param name="renderer">The renderer.</param>
        /// <param name="dpi">The dpi.</param>
        public EmfPlusParser(PdfEmfRenderer renderer, SizeF dpi)
            : base(renderer)
        {
            m_convertX = new PdfUnitConvertor(dpi.Width);
            m_convertY = new PdfUnitConvertor(dpi.Height);
        }
        #endregion

#region Overrides
        /// <summary>
        /// Overloaded. Creates handler of function parsing metafile.
        /// </summary>
        /// <returns></returns>
        protected override System.Drawing.Graphics.EnumerateMetafileProc CreateParsingHandler()
        {
            return new System.Drawing.Graphics.EnumerateMetafileProc(EnumerateMetafile);
        }

        #endregion

#region Event handlers
        /// <summary>
        /// Enumerates metafile.
        /// </summary>
        /// <param name="recordType">Type of record.</param>
        /// <param name="flags">Help flags.</param>
        /// <param name="dataSize">Size of the data for the record.</param>
        /// <param name="data">Pointer on the memory where data are located.</param>
        /// <param name="callbackData">Callback function.</param>
        /// <returns>True - to proceed enumeration, False otherwise.</returns>
        private new bool EnumerateMetafile(EmfPlusRecordType recordType, int flags,
            int dataSize, IntPtr data, PlayRecordCallback callbackData)
        {
            byte[] recordData = new byte[dataSize];

            //float[] ft = new float[ dataSize / FloatSize ];
            //int[] nt   = new int[ dataSize / IntSize ];

            if (data != IntPtr.Zero)
            {
                Marshal.Copy(data, recordData, 0, dataSize);

                //Marshal.Copy( data, ft, 0, dataSize / FloatSize );
                //Marshal.Copy( data, nt, 0, dataSize / IntSize );
            }

            //Debug.WriteLine( recordType.ToString() );
            //DumpData( recordData, recordType );
            
            EmfProcess(recordType, flags, dataSize, data, callbackData);

            try
            {
                switch (recordType)
                {
                    case EmfPlusRecordType.Header:
                        Header(recordData, flags);
                        break;
                    case EmfPlusRecordType.EmfHeader:
                        EmfHeader(recordData);
                        break;
                    case EmfPlusRecordType.EndOfFile:
                    case EmfPlusRecordType.EmfEof:
                        EndOfFile();
                        break;
                    case EmfPlusRecordType.BeginContainer:
                        BeginContainer(recordData, flags);
                        break;
                    case EmfPlusRecordType.BeginContainerNoParams:
                        BeginContainerNoParams(recordData);
                        break;
                    case EmfPlusRecordType.Clear:
                        Clear(recordData);
                        break;
                    case EmfPlusRecordType.DrawArc:
                        DrawArc(recordData, flags);
                        break;
                    case EmfPlusRecordType.DrawBeziers:
                        DrawBeziers(recordData, flags);
                        break;
                    case EmfPlusRecordType.DrawClosedCurve:
                        DrawClosedCurve(recordData, flags);
                        break;
                    case EmfPlusRecordType.DrawCurve:
                        DrawCurve(recordData, flags);
                        break;
                    case EmfPlusRecordType.DrawEllipse:
                        DrawEllipse(recordData, flags);
                        break;
                    case EmfPlusRecordType.DrawImage:
                        DrawImage(recordData, flags);
                        break;
                    case EmfPlusRecordType.DrawImagePoints:
                        DrawImagePoints(recordData, flags);
                        break;
                    case EmfPlusRecordType.DrawLines:
                        DrawLines(recordData, flags);
                        break;
                    case EmfPlusRecordType.DrawPath:
                        DrawPath(recordData, flags);
                        break;
                    case EmfPlusRecordType.DrawPie:
                        DrawPie(recordData, flags);
                        break;
                    case EmfPlusRecordType.DrawRects:
                        DrawRects(recordData, flags);
                        break;
                    case EmfPlusRecordType.DrawString:
                        DrawString(recordData, flags);
                        break;
                    case EmfPlusRecordType.FillClosedCurve:
                        FillClosedCurve(recordData, flags);
                        break;
                    case EmfPlusRecordType.FillEllipse:
                        FillEllipse(recordData, flags);
                        break;
                    case EmfPlusRecordType.FillPath:
                        FillPath(recordData, flags);
                        break;
                    case EmfPlusRecordType.FillPie:
                        FillPie(recordData, flags);
                        break;
                    case EmfPlusRecordType.FillPolygon:
                        FillPolygon(recordData, flags);
                        break;
                    case EmfPlusRecordType.FillRects:
                        FillRects(recordData, flags);
                        break;
                    case EmfPlusRecordType.FillRegion:
                        FillRegion(recordData, flags);
                        break;

                    case EmfPlusRecordType.Object:
                        Object(recordData, flags);
                        break;

                    case EmfPlusRecordType.MultiplyWorldTransform:
                        MultiplyWorldTransform(recordData, flags);
                        break;
                    case EmfPlusRecordType.OffsetClip:
                        OffsetClip(recordData);
                        break;
                    case EmfPlusRecordType.ResetClip:
                        ResetClip(recordData);
                        break;
                    case EmfPlusRecordType.ResetWorldTransform:
                        ResetWorldTransform(recordData);
                        break;
                    case EmfPlusRecordType.Restore:
                        Restore(recordData);
                        break;
                    case EmfPlusRecordType.RotateWorldTransform:
                        RotateWorldTransform(recordData, flags);
                        break;
                    case EmfPlusRecordType.Save:
                        Save(recordData);
                        break;
                    case EmfPlusRecordType.ScaleWorldTransform:
                        ScaleWorldTransform(recordData, flags);
                        break;
                    case EmfPlusRecordType.SetAntiAliasMode:
                        SetAntiAliasMode(recordData, flags);
                        break;
                    case EmfPlusRecordType.SetClipPath:
                        SetClipPath(recordData, flags);
                        break;
                    case EmfPlusRecordType.SetClipRect:
                        SetClipRect(recordData, flags);
                        break;
                    case EmfPlusRecordType.SetClipRegion:
                        SetClipRegion(recordData, flags);
                        break;
                    case EmfPlusRecordType.SetCompositingMode:
                        SetComposingMode(recordData, flags);
                        break;
                    case EmfPlusRecordType.SetCompositingQuality:
                        SetCompositingQuality(recordData, flags);
                        break;
                    case EmfPlusRecordType.SetInterpolationMode:
                        SetInterpolationMode(recordData, flags);
                        break;
                    case EmfPlusRecordType.SetPageTransform:
                        if (!Renderer.PageTransformed)
                        {
                            Renderer.PageTransformed = true;
                            SetPageTransform(recordData, flags);
                        }
                        break;
                    case EmfPlusRecordType.SetPixelOffsetMode:
                        SetPixelOffsetMode(recordData, flags);
                        break;
                    case EmfPlusRecordType.SetRenderingOrigin:
                        SetRenderingOrigin(recordData);
                        break;
                    case EmfPlusRecordType.SetTextContrast:
                        SetTextContrast(flags);
                        break;
                    case EmfPlusRecordType.SetTextRenderingHint:
                        SetTextRenderingHint(flags);
                        break;
                    case EmfPlusRecordType.EndContainer:
                        EndContainer(recordData);
                        break;
                    case EmfPlusRecordType.SetWorldTransform:
                        SetWorldTransform(recordData);
                        break;
                    case EmfPlusRecordType.TranslateWorldTransform:
                        TranslateWorldTransform(recordData, flags);
                        break;
                    case EmfPlusRecordType.DrawDriverString:
                        DrawDriverString(recordData, flags);
                        break;
                    default:
                        Renderer.m_EMFState = true;
                        if (!base.EnumerateMetafile(recordType, flags, dataSize, data, callbackData))
                            throw new Exception("Record not properly implemented");
                        break;
                }
            }
            catch
            {
                Debug.WriteLine("Error occured!" + recordType.ToString());
            }

            return true;
        }

        #endregion

#region Parsing Methods
        /// <summary>
        /// Parses the meta record.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="flags">The flags.</param>
        private void Header(byte[] data, int flags)
        {
            // TODO: Implement the method later.
            if(PageUnit != GraphicsUnit.Display)
                Renderer.PageUnit = PageUnit;
            if(PageScale != 1)
                Renderer.PageScale = PageScale;
        }

        /// <summary>
        /// Parsing method.
        /// </summary>
        /// <param name="data">Method data.</param>
        private void EmfHeader(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            // Invoke start method.
            //Header();

            int step = IntSize;
            int index = 0;

            System.Drawing.Rectangle bounds = ReadRectL(data, ref index);
            System.Drawing.Rectangle frame = ReadRectL(data, ref index);

            int signature = BitConverter.ToInt32(data, index);
            index += step;

            int version = BitConverter.ToInt32(data, index);
            index += step;

            int bytes = BitConverter.ToInt32(data, index);
            index += step;

            int records = BitConverter.ToInt32(data, index);
            index += step;

            int handlers = BitConverter.ToInt32(data, index);
            index += step;

            int descNumber = BitConverter.ToInt32(data, index);
            index += step;

            int descOffset = BitConverter.ToInt32(data, index);
            index += step;

            int palEntriesCount = BitConverter.ToInt32(data, index);
            index += step;

            int devSize = BitConverter.ToInt32(data, index);
            index += step;

            int milimetersSize = BitConverter.ToInt32(data, index);
            index += step;

            PointF location = bounds.Location;

            if (location != PointF.Empty)
            {
                bounds.Width += /*-bounds.X*/ +1;
                bounds.Height += /*-bounds.Y*/ +1;

                bounds.Location = Point.Empty;
            }

            // Set clip rectangle of the graphics.
            GraphicsPath clipPath = new GraphicsPath();

            //clipPath.AddRectangle( bounds );
            //Renderer.SetClip( clipPath, CombineMode.Replace );
            SizeF size = (Renderer.EmbedFonts) ||location==PointF.Empty? 
                bounds.Size : Renderer.Graphics.Size;

            float scaleX = (size.Width == bounds.Width) ? 1.0f : size.Width / bounds.Width;
            float scaleY = (size.Height + location.Y == bounds.Height) ? 1.0f : size.Height / bounds.Height;
            PointF boundLocation = PointF.Empty;

            if (location != PointF.Empty)
            {
                if (location.X < 0 || location.X > 0)
                {
                    scaleX = (size.Width + location.X == bounds.Width) ? 1.0f : (size.Width + location.X) / bounds.Width;
                }

                if (location.Y < 0 || location.Y > 0 )
                {
                    scaleY = (size.Width + location.Y == bounds.Height) ? 1.0f : (size.Height + location.Y) / bounds.Height;
                }

                boundLocation = new PointF(-(location.X) + 2, -(location.Y) + 2);
            }

            if (location.X >= 0 && location.Y >= 0)
            {
                Renderer.SetBounds(boundLocation, new SizeF(scaleX, scaleY));
            }

            // Invoke start method.
            Header();
        }

        /// <summary>
        /// Headers this instance.
        /// </summary>
        private void Header()
        {
            Renderer.BeforeStart();
        }

        /// <summary>
        /// Ends the of file.
        /// </summary>
        private void EndOfFile()
        {
            Renderer.BeforeEnd();
        }

        /// <summary>
        /// Begins the container.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="flags">The flags.</param>
        private void BeginContainer(byte[] data, int flags)
        {
            if (Renderer.EmbedFonts)
                return;

            if (data == null)
                throw new ArgumentNullException("data");

            int index = 0;
            int step = FloatSize;

            RectangleF destRect = ReadRectangle(data, ref index, step);
            RectangleF srcRect = ReadRectangle(data, ref index, step);
            GraphicsUnit unit = (GraphicsUnit)flags;

            GraphicsContainer container = Renderer.BeginContainer(destRect,
                srcRect, unit);

            int objectIndex = ReadInteger(data, ref index);
            Objects.SetState(index, container);
        }

        /// <summary>
        /// Begins the container no params.
        /// </summary>
        /// <param name="data">The data.</param>
        private void BeginContainerNoParams(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            GraphicsContainer container = Renderer.BeginContainer();

            int index = 0;
            int objectIndex = ReadInteger(data, ref index);
            Objects.SetState(objectIndex, container);
        }

        /// <summary>
        /// Clears the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        private void Clear(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            int index = 0;
            Color color = ReadColor(data, ref index);

            Renderer.Clear(color);
        }

        /// <summary>
        /// Draws the arc.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="flags">The flags.</param>
        private void DrawArc(byte[] data, int flags)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Pen pen = Objects.GetObject(GetIndex(flags)) as Pen;

            if (pen != null)
            {
                int step = FloatSize;
                int index = 0;

                float startAngle = ReadNumber(data, index, step);
                index += step;

                float sweepAngle = ReadNumber(data, index, step);
                index += step;

                step = GetDataStep(flags);

                RectangleF rect = ReadRectangle(data, ref index, step);

                Renderer.DrawArc(pen, rect, startAngle, sweepAngle);
            }
        }

        /// <summary>
        /// Draws the beziers.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="flags">The flags.</param>
        private void DrawBeziers(byte[] data, int flags)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Pen pen = Objects.GetObject(GetIndex(flags)) as Pen;

            if (pen != null)
            {
                int index = 0;
                int step = IntSize;
                int numPoints = BitConverter.ToInt32(data, index);
                index += step;

                step = GetDataStep(flags);

                PointF[] points = ReadPoints(data, ref index, numPoints, step);

                Renderer.DrawBeziers(pen, points);
            }
        }

        /// <summary>
        /// Draws the closed curve.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="flags">The flags.</param>
        private void DrawClosedCurve(byte[] data, int flags)
        {
            Pen pen = Objects.GetObject(GetIndex(flags)) as Pen;
            int index = 0;
            float tension = ReadNumber(data, index, IntSize);
            index += IntSize;
            int pointsNum = BitConverter.ToInt32(data, index);
            index += IntSize;

            PointF[] points = ReadPoints(data, ref index, pointsNum, GetDataStep(flags));

            // TODO: Get proper Fill Mode.
            PdfFillMode fillMode = PdfFillMode.Winding;

            Renderer.DrawClosedCurve(pen, points, tension, fillMode);
        }

        /// <summary>
        /// Draws the curve.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="flags">The flags.</param> 
        private void DrawCurve(byte[] data, int flags)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Pen pen = Objects.GetObject(GetIndex(flags)) as Pen;

            int index = 0;
            float tension = ReadNumber(data, index, IntSize);
            index += IntSize;
            float offSet = ReadNumber(data, index, IntSize);
            index += IntSize;
            float segNum = ReadNumber(data, index, IntSize);
            index += IntSize;

            int pointsNum = BitConverter.ToInt32(data, index);
            index += IntSize;

            PointF[] points = ReadPoints(data, ref index, pointsNum, GetDataStep(flags));

            PointF[] pts = null;
            if ((m_currentPenCap != null))
            {
                pts = m_currentPenCap.PathPoints;
            }

            Renderer.DrawCurve(pen, points, pts, (int)offSet, 1, tension);
        }

        /// <summary>
        /// Draws the ellipse.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="flags">The flags.</param>
        private void DrawEllipse(byte[] data, int flags)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Pen pen = Objects.GetObject(GetIndex(flags)) as Pen;

            if (pen != null)
            {
                int index = 0;
                int step = GetDataStep(flags);

                RectangleF rect = ReadRectangle(data, ref index, step);

                Renderer.DrawEllipse(pen, rect);
            }
        }

        /// <summary>
        /// Draws the image.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="flags">The flags.</param>
        private void DrawImage(byte[] data, int flags)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Image image = Objects.GetObject(GetIndex(flags)) as Image;

            if (image != null)
            {
                int index = 0;
                int step = IntSize;

                int imgAttributeIndex = BitConverter.ToInt32(data, index);

                ImageAttributes ia =
                    (imgAttributeIndex >= 0) ? Objects.GetObject(imgAttributeIndex) as ImageAttributes : null;

                index += step;


                int unitsType = BitConverter.ToInt32(data, index);
                GraphicsUnit units = (GraphicsUnit)unitsType;
                index += step;

                step = FloatSize;
                RectangleF srcRect = ReadRectangle(data, ref index, step);

                step = GetDataStep(flags);
                RectangleF destRect = ReadRectangle(data, ref index, step);

                
                MemoryStream stream = null;
                if(image is Bitmap)
                    ConvertToPng(image, out stream);

                if (stream != null)
                {
                    image = Image.FromStream(stream);
                    Renderer.DrawImage(image, destRect, srcRect, units);
                    
                    stream.Dispose();
                }
                else
                {
                    Renderer.DrawImage(image, destRect, srcRect, units);
                }
            }
        }

        /// <summary>
        /// Draws the image points.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="flags">The flags.</param>
        private void DrawImagePoints(byte[] data, int flags)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Image image = Objects.GetObject(GetIndex(flags)) as Image;
            
            ImageFormat imf = image.RawFormat;

            if (imf.Equals(ImageFormat.Gif))
            {
                MemoryStream ms = new MemoryStream();
                image.Save(ms, ImageFormat.Png);
                image = Image.FromStream(ms);
            }
            //Memory size increases when pixel format changes in Windows7
            if ((int)image.PixelFormat == 8207)
            {
                Image tempImage = image;
                image = new Bitmap(image.Width, image.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(image);
                graphics.DrawImage(tempImage, 0, 0,image.Width,image.Height);
                graphics.Dispose();
                tempImage.Dispose();
            }
          
            if (image != null)
            {
                int index = 0;
                int step = IntSize;

                int imgAttributeIndex = BitConverter.ToInt32(data, index);
                index += step;

                ImageAttributes ia =
                    (imgAttributeIndex >= 0) ? Objects.GetObject(imgAttributeIndex) as ImageAttributes : null;

                int unitsType = BitConverter.ToInt32(data, index);
                index += step;
                GraphicsUnit units = (GraphicsUnit)unitsType;

                step = FloatSize;
                RectangleF srcRect = ReadRectangle(data, ref index, step);

                int pointsNum = BitConverter.ToInt32(data, index);
                index += step;

                step = GetDataStep(flags);

                PointF[] points = ReadPoints(data, ref index, pointsNum, step);

                bool skip = false;
                int count = 0;
                for (int i = 0; i < points.Length; i++)
                {
                    if (points[i] == PointF.Empty)
                    {
                        count++;
                    }
                }
                if (count == points.Length)
                {
                    skip = true;
                }
                    if (!skip)
                    {
                        Renderer.DrawImage(image, points, srcRect, units);
                    }
            }
        }

        /// <summary>
        /// Draws the lines.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="flags">The flags.</param>
        private void DrawLines(byte[] data, int flags)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Pen pen = Objects.GetObject(GetIndex(flags)) as Pen;

            if (pen != null)
            {
                int index = 0;
                int step = IntSize;
                int numPoints = BitConverter.ToInt32(data, index);
                index += step;

                step = GetDataStep(flags);

                PointF[] points = ReadPoints(data, ref index, numPoints, step);

                // Reads the flag whether to connect last and first points.
                int l = flags & LineCloseFlag;
                bool closeShape = (l == LineCloseFlag) ? true : false;

                Renderer.DrawLines(pen, points, closeShape);
            }
        }

        /// <summary>
        /// Draws the path.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="flags">The flags.</param>
        private void DrawPath(byte[] data, int flags)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            int penIndex = BitConverter.ToInt32(data, 0);

            Pen pen = Objects.GetObject(penIndex) as Pen;
            GraphicsPath path = Objects.GetObject(GetIndex(flags)) as GraphicsPath;

            if (pen != null && path != null)
            {
                Renderer.DrawPath(pen, path);
            }
        }

        /// <summary>
        /// Draws the pie.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="flags">The flags.</param>
        private void DrawPie(byte[] data, int flags)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Pen pen = Objects.GetObject(GetIndex(flags)) as Pen;

            if (pen != null)
            {
                int index = 0;
                int step = FloatSize;

                float startAngle = ReadNumber(data, index, step);
                index += step;

                float sweepAngle = ReadNumber(data, index, step);
                index += step;

                step = GetDataStep(flags);

                RectangleF rect = ReadRectangle(data, ref index, step);

                Renderer.DrawPie(pen, rect, startAngle, sweepAngle);
            }
        }

        /// <summary>
        /// Draws the rectangle.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="flags">The flags.</param>
        private void DrawRects(byte[] data, int flags)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Pen pen = Objects.GetObject(GetIndex(flags)) as Pen;

            if (pen != null)
            {
                int index = 0;
                int step = IntSize;

                int numRects = BitConverter.ToInt32(data, index);
                index += step;

                step = GetDataStep(flags);

                RectangleF[] rects = new RectangleF[numRects];

                for (int i = 0; i < numRects; i++)
                {
                    RectangleF rect = ReadRectangle(data, ref index, step);
                    rects[i] = rect;
                }

                Renderer.DrawRectangles(pen, rects);
            }
        }

        /// <summary>
        /// Draws string.
        /// </summary>
        /// <param name="data">Buffer containing record data.</param>
        /// <param name="flags">Record flags.</param>
        private void DrawString(byte[] data, int flags)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            int index = 0;
            int step = FloatSize;
            bool isComplexScripts = false;

            Font font = Objects.GetFont(GetIndex(flags));
            Brush brush = GetBrush(data, ref index, flags);

            int stringFormatIndex = BitConverter.ToInt32(data, index);
            index += IntSize;
            StringFormat format =
                (stringFormatIndex >= 0) ? Objects.GetObject(stringFormatIndex) as StringFormat : null;

            int textLength = BitConverter.ToInt32(data, index);
            index += IntSize;

            RectangleF rect = ReadRectangle(data, ref index, step);
            char[] chars = Encoding.Unicode.GetChars(data, index, textLength * 2);
            string text = new string(chars);

            //Assumed that this call won't fail.
            if (PdfString.IsUnicode(text))
            {
                PdfFont pdfFont = new PdfTrueTypeFont(font);
                try
                {
                    SizeF actualSize = pdfFont.MeasureString(text);
                }
                catch
                {
                    //Check for RTL
                    if ((format.FormatFlags & StringFormatFlags.DirectionRightToLeft)!=0)
                    {
                        font = new Font("Times New Roman", font.Size, font.Style);                       
                    }

                    if (font.Name.ToLower().Equals("microsoft sans serif"))
                        font = new Font("Arial Unicode MS", font.Size, font.Style);
                    else if (font.Name.ToLower().Equals("arial"))
                        font = new Font("Arial", font.Size, font.Style);
                }
                isComplexScripts = CheckForComplexScripts(text);
            }          
 

            if (!isComplexScripts)
            {
                if (format != null && font != null && IsValidRect(rect))
                {
                    Renderer.DrawString(text, font, brush, rect, format);
                }
                else if (font != null && IsValidRect(rect))
                {
                    if (rect.Width == 0 && rect.Height == 0)
                    {
                        if (font.Name.ToLower().Equals("microsoft sans serif"))
                        {
                            using (Bitmap bitmap = new Bitmap(1, 1))
                            {
                                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
                                SizeF size = g.MeasureString(text, font);
                                SizeF actualSize = new PdfTrueTypeFont(font).MeasureString(text);
                                float difference = size.Height - font.Height; ;
                                while (size.Height - difference > actualSize.Height)
                                {
                                    float fontSize = font.Size + .5f;
                                    font = new Font(font.Name, fontSize, font.Style);
                                    actualSize = new PdfTrueTypeFont(font).MeasureString(text);
                                }
                            }
                        }
                    }
                    Renderer.DrawString(text, font, brush, rect);
                }
            }
            else
            {
                GraphicsPath path = new GraphicsPath();                
                path.AddString(text,font.FontFamily,(int)font.Style, font.Size, rect, format); 
                Renderer.FillPath(brush, path);
                path.Dispose();
            }
        }
        /// <summary>
        /// Draws Driver string.
        /// </summary>
        /// <param name="data">Buffer containing record data.</param>
        /// <param name="flags">Record flags.</param>
        private void DrawDriverString(byte[] recordData, int flags)
        {
            Font font = Objects.GetFont(GetIndex(flags));
            int index = 0;
            int step = FloatSize;
            Brush brush = GetBrush(recordData, ref index, flags);
            int driverStringOptions = BitConverter.ToInt32(recordData, index);
            index += IntSize;
            int matrixPresent = BitConverter.ToInt32(recordData, index);
            index += IntSize;
            int textLength = BitConverter.ToInt32(recordData, index);
            index += IntSize;
            char[] chars = Encoding.Unicode.GetChars(recordData, index, textLength * 2);
            string text = new string(chars);
            index += textLength * 2;
            PointF point = new PointF(); ;
            foreach (char character in chars)
            {
                point = ReadPoint(recordData, ref index, step);
                RectangleF rect = new RectangleF(point.X, point.Y - font.SizeInPoints, 0, 0);
                Renderer.DrawString(character.ToString(), font, brush, rect);
            }
        }
        /// <summary>
        /// Parses EndContainer record.
        /// </summary>
        /// <param name="data">Record data.</param>
        private void EndContainer(byte[] data)
        {
            if (Renderer.EmbedFonts)
                return;

            if (data == null)
                throw new ArgumentNullException("data");

            int objIndex = BitConverter.ToInt32(data, 0);
            GraphicsContainer container = Objects.GetState(objIndex) as GraphicsContainer;
            Renderer.EndContainer(container);
        }

        /// <summary>
        /// Parses FillClosedCurve record.
        /// </summary>
        /// <param name="data">Record data.</param>
        /// <param name="flags">Record flags.</param>
        private void FillClosedCurve(byte[] data, int flags)
        {
            System.Drawing.Drawing2D.FillMode fillMode = GetFillMode(flags);

            int index = 0;
            Brush brush = GetBrush(data, ref index, flags);

            float tension = ReadNumber(data, index, FloatSize);

            int pointsNum = BitConverter.ToInt32(data, index);
            index += IntSize;

            int step = GetDataStep(flags);

            PointF[] points = ReadPoints(data, ref index, pointsNum, step);

            Renderer.FillClosedCurve(brush, points, fillMode, tension);
        }

        /// <summary>
        /// Parses FillEllipse method.
        /// </summary>
        /// <param name="data">Record data.</param>
        /// <param name="flags">Record flags.</param>
        private void FillEllipse(byte[] data, int flags)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            int index = 0;
            Brush brush = GetBrush(data, ref index, flags);

            int step = GetDataStep(flags);

            RectangleF rect = ReadRectangle(data, ref index, step);

            Renderer.FillEllipse(brush, rect);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="flags"></param>
        private void FillPath(byte[] data, int flags)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            GraphicsPath path = Objects.GetObject(GetIndex(flags)) as GraphicsPath;

            int index = 0;
            Brush brush = GetBrush(data, ref index, flags);

            if (path != null)
            {
                Renderer.FillPath(brush, path);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="flags"></param>
        private void FillPie(byte[] data, int flags)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            int index = 0;
            Brush brush = GetBrush(data, ref index, flags);

            int step = FloatSize;
            float startAngle = ReadNumber(data, index, step);
            index += step;

            float sweepAngle = ReadNumber(data, index, step);
            index += step;

            step = GetDataStep(flags);

            RectangleF rect = ReadRectangle(data, ref index, step);

            Renderer.FillPie(brush, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="flags"></param>
        private void FillPolygon(byte[] data, int flags)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            int index = 0;
            Brush brush = GetBrush(data, ref index, flags);

            int step = IntSize;
            int numPoints = BitConverter.ToInt32(data, index);
            index += step;

            step = GetDataStep(flags);
            System.Drawing.Drawing2D.FillMode fm = GetFillMode(flags);

            PointF[] points = ReadPoints(data, ref index, numPoints, step);

            Renderer.FillPolygon(brush, points);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="flags"></param>
        private void FillRects(byte[] data, int flags)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            int index = 0;
            Brush brush = GetBrush(data, ref index, flags);

            int step = IntSize;
            int numRects = BitConverter.ToInt32(data, index);
            index += step;

            step = GetDataStep(flags);

            RectangleF[] rects = new RectangleF[numRects];

            for (int i = 0; i < numRects; i++)
            {
                RectangleF rect = ReadRectangle(data, ref index, step);
                rects[i] = rect;
            }

            Renderer.FillRectangles(brush, rects);
        }

        /// <summary>
        /// Fills the region.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="flags">The flags.</param>
        private void FillRegion(byte[] data, int flags)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            int index = 0;
            Brush brush = GetBrush(data, ref index, flags);

            Region region = Objects.GetObject(GetIndex(flags)) as Region;

            if (region != null)
            {
                Renderer.FillRegion(brush, region);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void MultiplyWorldTransform(byte[] data, int flags)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            int index = 0;
            int step = FloatSize;
            Matrix matrix = ReadMatrix(data, ref index, step);
            MatrixOrder order = GetMatrixOrder(flags);

            Renderer.MultiplyTransform(matrix, order);
        }

        /// <summary>
        /// Objects the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="flags">The flags.</param>
        private void Object(byte[] data, int flags)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            int result = flags & ObjectFlag;

            ObjectType objType = (ObjectType)result;
            int objIndex = GetIndex(flags);
            int index = 0;
            object obj = null;

            switch (objType)
            {
                case ObjectType.Pen:
                    obj = ReadPen(data, ref index);
                    break;

                case ObjectType.Image:
                    obj = ReadImage(data, ref index);
                    break;

                case ObjectType.Path:
                    obj = ReadPath(data, ref index);
                    break;

                case ObjectType.Region:
                    obj = ReadRegion(data, ref index);
                    break;

                case ObjectType.Font:
                    obj = ReadFont(data, ref index);
                    break;

                case ObjectType.StringFormat:
                    obj = ReadStringFormat(data, ref index);
                    break;

                case ObjectType.Brush:
                    obj = ReadBrush(data, ref index);
                    break;

                default:
                    break;
            }

            // Store object.
            if (obj != null)
            {
                Objects.SetObject(objIndex, obj);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void OffsetClip(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            int index = 0;
            int step = FloatSize;

            float dx = ReadNumber(data, index, step);
            index += step;
            float dy = ReadNumber(data, index, step);
            index += step;

            Renderer.TranslateClip(dx, dy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void ResetClip(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Renderer.ResetClip();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void ResetWorldTransform(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Renderer.ResetTransform();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void Restore(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            int objIndex = BitConverter.ToInt32(data, 0);

            GraphicsState gs = Objects.GetState(objIndex) as GraphicsState;
            Renderer.Restore(gs);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="flags"></param>
        private void RotateWorldTransform(byte[] data, int flags)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            int index = 0;
            int step = FloatSize;

            float angle = ReadNumber(data, index, step);
            MatrixOrder order = GetMatrixOrder(flags);

            Renderer.RotateTransform(angle, order);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void Save(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            int objIndex = BitConverter.ToInt32(data, 0);
            Objects.SetState(objIndex, Renderer.Save());
        }

        /// <summary>
        /// Scales the world transform.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="flags">The flags.</param>
        private void ScaleWorldTransform(byte[] data, int flags)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            int index = 0;
            int step = FloatSize;

            float sx = ReadNumber(data, index, step);
            index += step;
            float sy = ReadNumber(data, index, step);
            index += step;

            MatrixOrder order = GetMatrixOrder(flags);

            Renderer.ScaleTransform(sx, sy, order);
        }

        /// <summary>
        /// Sets the antialias mode.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="flags">The flags.</param>
        private void SetAntiAliasMode(byte[] data, int flags)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            // TODO: implement it if necessary.
        }

        /// <summary>
        /// Sets the clip path.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="flags">The flags.</param>
        private void SetClipPath(byte[] data, int flags)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            CombineMode mode = GetCombineMode(flags);

            GraphicsPath path = Objects.GetObject(GetIndex(flags)) as GraphicsPath;

            if (path != null)
            {
                Renderer.SetClip(path, mode);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="flags"></param>
        private void SetClipRect(byte[] data, int flags)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            int index = 0;
            int step = FloatSize;

            RectangleF rect = ReadRectangle(data, ref index, step);
            CombineMode mode = GetCombineMode(flags);

            Renderer.SetClip(rect, mode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="flags"></param>
        private void SetClipRegion(byte[] data, int flags)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            CombineMode mode = GetCombineMode(flags);
            Region region = Objects.GetObject(GetIndex(flags)) as Region;

            if (region != null)
            {
                Renderer.SetClip(region, mode);
            }
        }

        /// <summary>
        /// Sets the composing mode.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="flags">The flags.</param>
        private void SetComposingMode(byte[] data, int flags)
        {
            CompositingMode composingMode = (CompositingMode)flags;
            // TODO: set composing mode.
        }

        /// <summary>
        /// Sets the compositing quality.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="flags">The flags.</param>
        private void SetCompositingQuality(byte[] data, int flags)
        {
            CompositingQuality composingQuality = (CompositingQuality)flags;
            // TODO: set composing quality.
        }

        /// <summary>
        /// Sets the interpolation mode.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="flags">The flags.</param>
        private void SetInterpolationMode(byte[] data, int flags)
        {
            InterpolationMode interpolationMode = (InterpolationMode)flags;
            // TODO: set Interpolation Mode.
        }

        /// <summary>
        /// Sets the page transform.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="flags">The flags.</param>
        private void SetPageTransform(byte[] data, int flags)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            GraphicsUnit unit = (GraphicsUnit)flags;
            float scaling = BitConverter.ToSingle(data, 0);

            if (unit != GraphicsUnit.World)
            {
                //if (unit == GraphicsUnit.Display)
                //{
                //    scaling *= m_convertX.ConvertToPixels(scaling, PdfGraphicsUnit.Pixel);
                //}
                //else
                //{
                //    scaling *= m_convertX.ConvertToPixels(scaling, (PdfGraphicsUnit)(int)unit);
                //}


                Renderer.PageUnit = (unit == GraphicsUnit.Point) ? unit : GraphicsUnit.Pixel;

                Renderer.PageScale = scaling;
            }
        }

        /// <summary>
        /// Sets the pixel offset mode.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="flags">The flags.</param>
        private void SetPixelOffsetMode(byte[] data, int flags)
        {
            PixelOffsetMode pom = (PixelOffsetMode)flags;
            // TODO: sent PixelOffsetMode.
        }

        /// <summary>
        /// Sets the rendering origin.
        /// </summary>
        /// <param name="data">The data.</param>
        private void SetRenderingOrigin(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            int index = 0;
            int step = IntSize;

            int x = BitConverter.ToInt32(data, index);
            index += step;
            int y = BitConverter.ToInt32(data, index);

            Point point = new Point(x, y);

            Renderer.SetRenderingOrigin(point);
        }

        /// <summary>
        /// Sets the text contrast.
        /// </summary>
        /// <param name="flags">The flags.</param>
        private void SetTextContrast(int flags)
        {
            // TODO: implement setting text contrast.
        }

        /// <summary>
        /// Sets the text rendering hint.
        /// </summary>
        /// <param name="flags">The flags.</param>
        private void SetTextRenderingHint(int flags)
        {
            // TODO: implement if possible.
            TextRenderingHint renderingHint = (TextRenderingHint)flags;
            Renderer.NativeGraphics.TextRenderingHint = renderingHint;
        }

        /// <summary>
        /// Sets the world transform.
        /// </summary>
        /// <param name="data">The data.</param>
        private void SetWorldTransform(byte[] data)
        {
            int index = 0;
            int step = FloatSize;

            Matrix matrix = ReadMatrix(data, ref index, step);

            Renderer.Transform = matrix;
        }

        /// <summary>
        /// Translates the world transform.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="flags">The flags.</param>
        private void TranslateWorldTransform(byte[] data, int flags)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            int index = 0;
            int step = FloatSize;

            float dx = ReadNumber(data, index, step);
            index += step;
            float dy = ReadNumber(data, index, step);
            index += step;

            MatrixOrder order = GetMatrixOrder(flags);

            Renderer.TranslateTransform(dx, dy, order);
        }
        #endregion

#region Implementation

        /// <summary>
        /// Reads the pen.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private Pen ReadPen(byte[] data, ref int index)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            m_currentPenCap = null;

            Pen pen = null;
            // Skip generic header.
            int header = BitConverter.ToInt32(data, index);
            index += IntSize;
            index += IntSize;

            int step = FloatSize;

            // Get information about data in the array.
            int iType = BitConverter.ToInt32(data, index);
            index += IntSize;
            PenFlags flags = (PenFlags)iType;

            // Skip 4 bytes.
            index += IntSize;

            float penWidth = ReadNumber(data, index, step);
            index += step;

            // Create a pen.
            pen = new Pen(Color.Empty, penWidth);

            if ((flags & PenFlags.Transform) != 0)
            {
                Matrix matrix = ReadMatrix(data, ref index, FloatSize);
                pen.Transform = matrix;
            }

            if ((flags & PenFlags.StartCap) != 0)
            {
                int value = BitConverter.ToInt32(data, index);
                index += IntSize;
                System.Drawing.Drawing2D.LineCap startCap = (System.Drawing.Drawing2D.LineCap)value;
                pen.StartCap = startCap;
            }

            if ((flags & PenFlags.EndCap) != 0)
            {
                int value = BitConverter.ToInt32(data, index);
                index += IntSize;
                System.Drawing.Drawing2D.LineCap endCap = (System.Drawing.Drawing2D.LineCap)value;
                pen.EndCap = endCap;
            }

            if ((flags & PenFlags.LineJoin) != 0)
            {
                int value = BitConverter.ToInt32(data, index);
                index += IntSize;
                System.Drawing.Drawing2D.LineJoin lineJoin = (System.Drawing.Drawing2D.LineJoin)value;
                pen.LineJoin = lineJoin;
            }

            if ((flags & PenFlags.MiterLimit) != 0)
            {
                float milterLimit = BitConverter.ToSingle(data, index);
                index += FloatSize;
                pen.MiterLimit = milterLimit;
            }

            if ((flags & PenFlags.DashStyle) != 0)
            {
                int value = BitConverter.ToInt32(data, index);
                index += IntSize;
                System.Drawing.Drawing2D.DashStyle dashStyle = (System.Drawing.Drawing2D.DashStyle)value;
                pen.DashStyle = dashStyle;
            }

            if ((flags & PenFlags.DashCap) != 0)
            {
                System.Drawing.Drawing2D.DashCap dashCap = (System.Drawing.Drawing2D.DashCap)BitConverter.ToInt32(data, index);
                index += IntSize;
                pen.DashCap = dashCap;
            }

            if ((flags & PenFlags.DashOffset) != 0)
            {
                float dashOffset = BitConverter.ToSingle(data, index);
                index += FloatSize;
                pen.DashOffset = dashOffset;
            }

            if ((flags & PenFlags.DashPattern) != 0)
            {
                float[] dashPattern = ReadSingleArray(data, ref index, FloatSize);
                pen.DashPattern = dashPattern;
            }

            if ((flags & PenFlags.Alignment) != 0)
            {
                PenAlignment value = (PenAlignment)BitConverter.ToInt32(data, index);
                index += IntSize;
                pen.Alignment = value;
            }

            if ((flags & PenFlags.CompoundArray) != 0)
            {
                float[] compound = ReadSingleArray(data, ref index, FloatSize);
                pen.CompoundArray = compound;
            }

            if ((flags & PenFlags.CustomStartCap) != 0)
            {
                int value = BitConverter.ToInt32(data, index);
                index += IntSize;
                // Skip the entire entry.
                index += value;
            }

            if ((flags & PenFlags.CustomEndCap) != 0)
            {
                int value = BitConverter.ToInt32(data, index);
                index += IntSize;

                //index += value;
                index += IntSize;
                int type = BitConverter.ToInt32(data, index);
                index += IntSize;
                if (type == 0)
                {
                    int customLineDataFlags = BitConverter.ToInt32(data, index);
                    index += IntSize;
                    index += 44;

                    if (customLineDataFlags == 1)
                    {  
                        int pathLength = BitConverter.ToInt32(data, index);
                        index += IntSize;
                        int pathHeader = BitConverter.ToInt32(data, index);

                        GraphicsPath path = CreatePath(data, ref index, (int)flags);
                        m_currentPenCap = new GraphicsPath(path.PathPoints, path.PathTypes);
                    }
                    else if (customLineDataFlags == 2)
                    {
                        int pathLength = BitConverter.ToInt32(data, index);
                        index += IntSize;
                        index += pathLength;
                      
                      
                    }
                }
                else if(type==1)
                {
                   Renderer.m_customLineCapArrowData=new CustomLineCapArrowData();

                   Renderer.m_customLineCapArrowData.width = BitConverter.ToSingle(data, index);
                    index += IntSize;
                    Renderer.m_customLineCapArrowData.height = BitConverter.ToSingle(data, index);
                    index += IntSize;
                    Renderer.m_customLineCapArrowData.middleInset = BitConverter.ToSingle(data, index);
                    index += IntSize;
                    Renderer.m_customLineCapArrowData.fillState = BitConverter.ToInt32(data, index);
                    index += IntSize;
                    Renderer.m_customLineCapArrowData.lineStartCap = BitConverter.ToInt32(data, index);
                    index += IntSize;
                    Renderer.m_customLineCapArrowData.lineEndCap = BitConverter.ToInt32(data, index);
                    index += IntSize;
                    Renderer.m_customLineCapArrowData.lineJoin = BitConverter.ToInt32(data, index);
                    index += IntSize;
                    Renderer.m_customLineCapArrowData.lineMitterLimit = BitConverter.ToSingle(data, index);
                    index += IntSize;
                    Renderer.m_customLineCapArrowData.widthScale = BitConverter.ToSingle(data, index);
                    index += IntSize;
                    index += 16;

                }
                pen.Width = penWidth;
               
            }

            // Get a brush.
            Brush brush = ReadBrush(data, ref index);
            pen.Brush = brush;

            return pen;
        }

        /// <summary>
        /// Creates the path of pen's custom end cap.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <param name="flags">The flags.</param>
        /// <returns></returns>
        private GraphicsPath CreatePath(byte[] data, ref int index, int flags)
        {
            GraphicsPath path = null;
            PointF[] points = null;
            int cnt = 0;

            index += IntSize;
            cnt = BitConverter.ToInt32(data, index);

            index += IntSize;
            int bytesPerNumber = BitConverter.ToInt32(data, index);
            points = new PointF[cnt];

            index += IntSize;
            points = ReadPoints(data, ref index, cnt, GetDataStep(flags));
            byte[] pathMode = new byte[cnt];

            Array.Copy(data, index, pathMode, 0, cnt);

            path = new GraphicsPath(points, pathMode);

            index += cnt;

            return path;
        }

        /// <summary>
        /// Reads the image.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private Image ReadImage(byte[] data, ref int index)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            Metafile result = null;
            // Skip generic header.
            index += IntSize;

            int step = IntSize;
            Image img = null;

            // Get information about data in the array.
            int iType = BitConverter.ToInt32(data, index);
            index += step;
            ObjectImageFormat type = (ObjectImageFormat)iType;
            if (type == ObjectImageFormat.Unknown)
                return img;

            bool memoryBitmap = false;
            int dataLength = 0;

            if (type == ObjectImageFormat.Bitmap)
            {
                int width = BitConverter.ToInt32(data, index);
                index += step;

                int height = BitConverter.ToInt32(data, index);
                index += step;

                int stride = BitConverter.ToInt32(data, index);
                index += step;

                int iFormat = BitConverter.ToInt32(data, index);
                PixelFormat format = (PixelFormat)iFormat;
                index += step;

                memoryBitmap = (width != 0 && height != 0 && stride != 0);

                index += IntSize; // Skip constant field.

                if (memoryBitmap)
                {
                    dataLength = stride * height;
                    byte[] imgData = new byte[dataLength];
                    Array.Copy(data, index, imgData, 0, dataLength);
                    IntPtr ptr = Marshal.AllocHGlobal(dataLength);
                    Marshal.Copy(imgData, 0, ptr, dataLength);
                    img = new Bitmap(width, height, stride, format, ptr);
                }
                else
                {
                    dataLength = data.Length - index;
                }

            }
            else if (type == ObjectImageFormat.Metafile)
            {
                int metaFileType = BitConverter.ToInt32(data, index);
                index += IntSize; // Skip constant field.

                dataLength = BitConverter.ToInt32(data, index);
                index += IntSize;
                //Check the Wmf type
                if (metaFileType == 2)
                {
                    //Placable Wmf
                    if (BitConverter.ToInt32(data, index) == unchecked((int)0x9AC6CDD7))
                    {
                        index += IntSize + 20;//Each placeable metafile begins with a 24-byte header followed by a standard metafile:
                    }
                }
            }

            if (!memoryBitmap)
            {
                MemoryStream ms = new MemoryStream(data, index,
                    dataLength);

                img = Image.FromStream(ms);
            }

            return img;
        }

        /// <summary>
        /// Reads the path.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private GraphicsPath ReadPath(byte[] data, ref int index)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            // Skip generic header.
            index += IntSize;

            GraphicsPath path = null;
            int step = FloatSize;

            int numPoints = BitConverter.ToInt32(data, index);
            index += IntSize;

            int additionalFlags = BitConverter.ToInt32(data, index);
            index += IntSize;
            System.Drawing.Drawing2D.FillMode fillMode = GetFillMode(additionalFlags);

            if (numPoints > 0)
            {
                step = GetDataStep(additionalFlags);

                PointF[] points = ReadPoints(data, ref index, numPoints, step);

                byte[] types = new byte[numPoints];
                Array.Copy(data, index, types, 0, numPoints);

                path = new GraphicsPath(points, types);

                index += numPoints;

                int tailLength = (numPoints % 4);

                if (tailLength > 0)
                {
                    index += 4 - tailLength;
                }

            }
            else
            {
                path = new GraphicsPath();
            }

            path.FillMode = fillMode;

            return path;
        }

        /// <summary>
        /// Reads the region.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private Region ReadRegion(byte[] data, ref int index)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Region region = null;

            index += IntSize; // Skip the generic header.
            int step = IntSize;
            int number = BitConverter.ToInt32(data, index);
            index += step;

            number /= 2;

            CombineMode[] modes = new CombineMode[number];
            for (int i = 0; i < number; i++)
            {
                int iMode = BitConverter.ToInt32(data, index);
                index += step;
                CombineMode mode = (CombineMode)iMode;
                modes[modes.Length - 1 - i] = mode;
            }

            region = ReadRegion(data, ref index, step);

            if (index < data.Length && modes.Length > 0)
            {
                for (int i = 0; i < modes.Length; i++)
                {
                    if (index >= data.Length)
                    {
                        break;
                    }

                    CombineMode mode = modes[i];
                    Region tmpRegion = ReadRegion(data, ref index, step);
                    region = CombineRegion(region, tmpRegion, mode);
                }
            }

            return region;
        }

        /// <summary>
        /// Reads the font.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private Font ReadFont(byte[] data, ref int index)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            // Skip generic header.
            index += IntSize;
            int step = FloatSize;

            float fontSize = ReadNumber(data, index, step);
            index += step;

            fontSize /= Renderer.PageScale;

            int graphicsUnit = BitConverter.ToInt32(data, index);
            index += IntSize;
            GraphicsUnit units = (GraphicsUnit)graphicsUnit;

            int style = BitConverter.ToInt32(data, index);
            index += IntSize;
            FontStyle fontStyle = (FontStyle)style;

            // Skip 4 bytes of unknown data.
            index += IntSize;

            int nameLength = BitConverter.ToInt32(data, index);
            index += IntSize;

            int byteLength = nameLength << 1;  // Name length * 2
            char[] chars = Encoding.Unicode.GetChars(data, index, byteLength);
            index += byteLength;

            string fontName = new string(chars);

            Font font = new Font(fontName, fontSize, fontStyle, units);

            return font;

        }

        /// <summary>
        /// Reads the string format.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private StringFormat ReadStringFormat(byte[] data, ref int index)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            index += IntSize; // Skip generic header.
            int step = IntSize;

            int value = BitConverter.ToInt32(data, index);
            index += step;
            StringFormatFlags flags = (StringFormatFlags)value;

            StringFormat format = new StringFormat(flags);

            index += step; // skip the next 4 bytes.

            value = BitConverter.ToInt32(data, index);
            index += step;
            StringAlignment alignment = (StringAlignment)value;
            format.Alignment = alignment;

            value = BitConverter.ToInt32(data, index);
            index += step;
            StringAlignment lineAlignment = (StringAlignment)value;
            format.LineAlignment = lineAlignment;

            value = BitConverter.ToInt32(data, index);
            index += step;
            StringDigitSubstitute digitSubstitutionMethod = (StringDigitSubstitute)value;

            // we're only interested in the lower word.
            int digitSubstitutionLanguage = BitConverter.ToInt32(data, index) & 0xffff;
            index += step;
            format.SetDigitSubstitution(digitSubstitutionLanguage, digitSubstitutionMethod);

            float firstTabStop = BitConverter.ToSingle(data, index);
            index += FloatSize;

            value = BitConverter.ToInt32(data, index);
            index += step;
            HotkeyPrefix prefix = (HotkeyPrefix)value;
            format.HotkeyPrefix = prefix;

            index += 12; // Skip next 12 bytes.

            value = BitConverter.ToInt32(data, index);
            index += step;
            StringTrimming trimming = (StringTrimming)value;
            format.Trimming = trimming;

            int tabStopsNum = BitConverter.ToInt32(data, index);
            index += step;

            int charNum = BitConverter.ToInt32(data, index);
            index += step;

            if (tabStopsNum > 0)
            {
                float[] tabStops = new float[tabStopsNum];

                for (int i = 0; i < tabStopsNum; ++i)
                {
                    tabStops[i] = BitConverter.ToSingle(data, index);
                    index += FloatSize;
                }

                format.SetTabStops(firstTabStop, tabStops);
            }

            if (charNum > 0)
            {
                CharacterRange[] charRanges = new CharacterRange[charNum];

                for (int i = 0; i < charNum; ++i)
                {
                    int start = BitConverter.ToInt32(data, index);
                    index += IntSize;
                    int end = BitConverter.ToInt32(data, index);
                    index += IntSize;

                    charRanges[i] = new CharacterRange(start, end - start);
                }

                format.SetMeasurableCharacterRanges(charRanges);
            }

            return format;
        }

        /// <summary>
        /// Reads the brush.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private Brush ReadBrush(byte[] data, ref int index)
        {
            // Skip generic header.
            index += IntSize;

            BrushType brushType = (BrushType)BitConverter.ToInt32(data, index);
            index += IntSize;
            Brush brush = null;

            switch (brushType)
            {
                case BrushType.SolidBrush:
                    Color color = ReadColor(data, ref index);
                    brush = new SolidBrush(color);
                    break;

                case BrushType.TextureBrush:
                    brush = ReadTextureBrush(data, ref index);
                    break;

                case BrushType.HatchBrush:
                    brush = ReadHatchBrush(data, ref index);
                    break;

                case BrushType.LienarGradientBrush:
                    brush = ReadGradientBrush(data, ref index);
                    break;

                case BrushType.PathGradientBrush:
                    brush = ReadPathGradientBrush(data, ref index);
                    break;
            }

            return brush;
        }

        /// <summary>
        /// Reads the hatch brush.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private Brush ReadHatchBrush(byte[] data, ref int index)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            int step = IntSize;

            int iStyle = BitConverter.ToInt32(data, index);
            index += step;
            HatchStyle style = (HatchStyle)iStyle;

            Color startColor = ReadColor(data, ref index);
            Color endColor = ReadColor(data, ref index);

            HatchBrush brush = new HatchBrush(style, startColor, endColor);

            return brush;
        }

        /// <summary>
        /// Reads the gradient brush.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private Brush ReadGradientBrush(byte[] data, ref int index)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            int numColors = 4;
            int step = IntSize;

            GradientBrushFlags flags = ReadGradientBrushFlags(data, ref index);

            WrapMode wrapMode = ReadWrapMode(data, ref index);

            step = FloatSize;
            RectangleF rect = ReadRectangle(data, ref index, step);
            Color[] colors = new Color[numColors];

            for (int i = 0; i < numColors; i++)
            {
                Color color = ReadColor(data, ref index);
                colors[i] = color;
            }

            LinearGradientBrush brush = new LinearGradientBrush(rect,
                colors[0], colors[1], 0.0f);

            //Set default blend properties.
            Blend initalBlend = new Blend();
            brush.Blend = initalBlend;

            brush.WrapMode = wrapMode;

            if ((flags & GradientBrushFlags.Matrix) > 0)
            {
                Matrix matrix = ReadMatrix(data, ref index, step);
                brush.Transform = matrix;
            }

            if ((flags & GradientBrushFlags.Blend) > 0)
            {
                float[] positions;
                float[] factors;

                index = ReadBlend(data, index, FloatSize, out positions, out factors);

                Blend bl = new Blend();
                bl.Factors = factors;
                bl.Positions = positions;

                brush.Blend = bl;
            }


            if ((flags & GradientBrushFlags.ColorBlend) > 0)
            {
                // InterpolationColors set.
                if (index < data.Length)
                {
                    ColorBlend blend = ReadColorBlend(data, ref index, FloatSize);
                    brush.InterpolationColors = blend;
                }

            }

            brush.GammaCorrection = ((flags & GradientBrushFlags.GammaCorrection) > 0);

            return brush;
        }

        /// <summary>
        /// Reads path gradient brush.
        /// </summary>
        /// <param name="data">Data for the brush.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private Brush ReadPathGradientBrush(byte[] data, ref int index)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            int step = IntSize;

            // Additional flags.
            GradientBrushFlags flags = ReadGradientBrushFlags(data, ref index);

            // Wrap Mode.
            WrapMode wrapMode = ReadWrapMode(data, ref index);

            // Center colour.
            Color centerColor = ReadColor(data, ref index);
            PointF centerPoint = ReadPoint(data, ref index, FloatSize);

            // The number of the surrounding colours.
            int surroundColorsNum = BitConverter.ToInt32(data, index);
            index += IntSize;

            // The surrounding colours.
            Color[] surroundingColors = new Color[surroundColorsNum];

            for (int i = 0; i < surroundColorsNum; ++i)
            {
                Color colour = ReadColor(data, ref index);
                surroundingColors[i] = colour;
            }

            // The length of the path.
            int pathLength = BitConverter.ToInt32(data, index);
            index += IntSize;

            GraphicsPath path = ReadPath(data, ref index);

            PathGradientBrush brush = new PathGradientBrush(path.PathPoints, wrapMode);
            brush.CenterColor = centerColor;
            brush.CenterPoint = centerPoint;
            brush.SurroundColors = surroundingColors;

            //Set default blend properties.
            Blend initalBlend = new Blend();
            brush.Blend = initalBlend;

            if ((flags & GradientBrushFlags.Matrix) != 0)
            {
                Matrix matrix = ReadMatrix(data, ref index, FloatSize);
                brush.Transform = matrix;
            }

            if ((flags & GradientBrushFlags.Blend) != 0)
            {
                // Reading blend.
                float[] positions;
                float[] factors;

                index = ReadBlend(data, index, FloatSize, out positions, out factors);

                Blend bl = new Blend();
                bl.Factors = factors;
                bl.Positions = positions;

                brush.Blend = bl;
            }

            if ((flags & GradientBrushFlags.ColorBlend) != 0)
            {
                ColorBlend colorBlend = ReadColorBlend(data, ref index, FloatSize);
                brush.InterpolationColors = colorBlend;
            }

            if ((flags & GradientBrushFlags.FocusScales) != 0)
            {
                index += IntSize;
                PointF focusScalesPoint = ReadPoint(data, ref index, FloatSize);

                brush.FocusScales = focusScalesPoint;
            }

            return brush;
        }

        /// <summary>
        /// Reads the texture brush.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <returns>The texture brush.</returns>
        private Brush ReadTextureBrush(byte[] data, ref int index)
        {
            int flag = BitConverter.ToInt32(data, index);
            index += IntSize;

            bool isMatrix = ((flag & 0x02) != 0);

            WrapMode wrapMode = ReadWrapMode(data, ref index);

            Matrix matrix = null;

            if (isMatrix)
            {
                matrix = ReadMatrix(data, ref index, FloatSize);
            }

            Image image = ReadImage(data, ref index);

            TextureBrush brush = new TextureBrush(image, wrapMode);

            if (isMatrix)
            {
                brush.Transform = matrix;
            }

            return brush;
        }

        /// <summary>
        /// Gets the rectangle step.
        /// </summary>
        /// <param name="flags">The flags which holds the step value.</param>
        /// <returns>The rectangle step.</returns>
        private int GetDataStep(int flags)
        {
            int step = ((flags & UseShorts) == 0) ? FloatSize : ShortSize;

            return step;
        }

        /// <summary>
        /// Reads the wrap mode.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <returns>The WrapMode enum.</returns>
        private WrapMode ReadWrapMode(byte[] data, ref int index)
        {
            int iWrapMode = BitConverter.ToInt32(data, index);
            index += IntSize;
            WrapMode wrapMode = (WrapMode)iWrapMode;

            return wrapMode;
        }

        /// <summary>
        /// Reads the gradient brush flags.
        /// </summary>
        /// <param name="data">The byte data array.</param>
        /// <param name="index">The index of the flags.</param>
        /// <returns>The initialized GradientBrushFlags.</returns>
        private GradientBrushFlags ReadGradientBrushFlags(byte[] data, ref int index)
        {
            int iFlags = BitConverter.ToInt32(data, index);
            index += IntSize;
            GradientBrushFlags flags = (GradientBrushFlags)iFlags;

            return flags;
        }

        /// <summary>
        /// Reads the blend.
        /// </summary>
        /// <param name="data">The byte data array.</param>
        /// <param name="start">The start position in the data.</param>
        /// <param name="step">The size of the single value.</param>
        /// <param name="positions">The positions array.</param>
        /// <param name="factors">The factors array.</param>
        /// <returns>The final index within the data.</returns>
        private int ReadBlend(byte[] data, int start, int step,
            out float[] positions, out float[] factors)
        {
            int index = start;
            int blendCount = BitConverter.ToInt32(data, index);
            index += IntSize;

            positions = new float[blendCount];
            factors = new float[blendCount];

            for (int i = 0; i < blendCount; ++i)
            {
                positions[i] = ReadNumber(data, index, FloatSize);
                index += FloatSize;
            }

            for (int i = 0; i < blendCount; ++i)
            {
                factors[i] = ReadNumber(data, index, FloatSize);
                index += FloatSize;
            }
            Array.Sort(positions, factors);

            return index;
        }

        /// <summary>
        /// Reads the color blent.
        /// </summary>
        /// <param name="data">The byte data array.</param>
        /// <param name="index">The start position in the data.</param>
        /// <param name="step">The size of the single value.</param>
        /// <returns>The blend object.</returns>
        private ColorBlend ReadColorBlend(byte[] data, ref int index, int step)
        {
            int blendNum = BitConverter.ToInt32(data, index);
            index += IntSize;

            float[] blendPositions = new float[blendNum];

            for (int i = 0; i < blendNum; i++)
            {
                float position = ReadNumber(data, index, step);
                index += step;
                blendPositions[i] = position;
            }

            Color[] blendColors = new Color[blendNum];

            for (int i = 0; i < blendNum; i++)
            {
                Color color = ReadColor(data, ref index);
                blendColors[i] = color;
            }

            ColorBlend blend = new ColorBlend(blendNum);

            Array.Sort(blendPositions, blendColors);

            blend.Positions = blendPositions;
            blend.Colors = blendColors;

            return blend;
        }

        /// <summary>
        /// Reads the points.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <param name="number">The number.</param>
        /// <param name="step">The step.</param>
        /// <returns></returns>
        private PointF[] ReadPoints(byte[] data, ref int index, int number, int step)
        {
            PointF[] points = new PointF[number];

            for (int i = 0; i < number; ++i)
            {
                points[i] = ReadPoint(data, ref index, step);
            }
            return points;
        }

        /// <summary>
        /// Reads the point.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <param name="step">The step.</param>
        /// <returns></returns>
        private PointF ReadPoint(byte[] data, ref int index, int step)
        {
            float x = ReadNumber(data, index, step);
            index += step;

            float y = ReadNumber(data, index, step);
            index += step;

            PointF pt = new PointF(x, y);

            return pt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        private RectangleF ReadRectangle(byte[] data, ref int index, int step)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            float x = ReadNumber(data, index, step);
            index += step;

            float y = ReadNumber(data, index, step);
            index += step;

            float width = ReadNumber(data, index, step);
            index += step;

            float height = ReadNumber(data, index, step);
            index += step;

            return new RectangleF(x, y, width, height);
        }

        /// <summary>
        /// Reads an integer from a data array at an index specified.
        /// </summary>
        /// <param name="data">The data array.</param>
        /// <param name="index">The index which the integer starts at.</param>
        /// <returns>The integer read.</returns>
        private int ReadInteger(byte[] data, ref int index)
        {
            int result = BitConverter.ToInt32(data, index);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private Color ReadColor(byte[] data, ref int index)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            byte b = data[index];
            index++;
            byte g = data[index];
            index++;
            byte r = data[index];
            index++;
            byte a = data[index];
            index++;

            // NOTE: in some cases alpha chanel is set to zero, but it shouldn't be zero.
            //a = ( a == 0 ) ? Byte.MaxValue : a;

            Color color = Color.FromArgb(a, r, g, b);

            return color;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        private Matrix ReadMatrix(byte[] data, ref int index, int step)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            float[] elements = new float[6];

            for (int i = 0; i < elements.Length; i++)
            {
                float element = ReadNumber(data, index, step);
                index += step;
                elements[i] = element;
            }

            Matrix matrix = new Matrix(elements[0], elements[1], elements[2],
                elements[3], elements[4], elements[5]);

            return matrix;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        private CombineMode GetCombineMode(int flags)
        {
            int mode = (flags >> 8) & 0xff; // Second-least significant byte;

            CombineMode result = (CombineMode)mode;

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        private MatrixOrder GetMatrixOrder(int flags)
        {
            MatrixOrder order = ((flags & 0x2000) == 0) ? MatrixOrder.Prepend :
                MatrixOrder.Append;

            return order;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        private float[] ReadSingleArray(byte[] data, ref int index, int step)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            int numArray = BitConverter.ToInt32(data, index);
            index += IntSize;
            float[] arrData = new float[numArray];

            for (int i = 0; i < numArray; i++)
            {
                float number = ReadNumber(data, index, step);
                index += step;
                arrData[i] = number;
            }

            return arrData;
        }

        /// <summary>
        /// Dumps the record data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="type">The type of the record.</param>
        private void DumpData(byte[] data, EmfPlusRecordType type)
        {
            string whitespace = " ";

            Debug.WriteLine(type.ToString());
            Debug.WriteLine("Size: " + data.Length);

            for (int i = 0; i < data.Length; i++)
            {
                Debug.Write(data[i].ToString() + whitespace);
            }

            Debug.WriteLine("");
        }

        /// <summary>
        /// Read Graphics path from the region.
        /// </summary>
        /// <param name="data">Data of the record.</param>
        /// <param name="index">Current index.</param>
        /// <param name="step">step value.</param>
        /// <returns>Graphics path object.</returns>
        private GraphicsPath ReadRegionPath(byte[] data, ref int index, int step)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            int pathLength = BitConverter.ToInt32(data, index);
            index += IntSize;
            GraphicsPath path = ReadPath(data, ref index);

            return path;
        }

        /// <summary>
        /// Reads base region from the data.
        /// </summary>
        /// <param name="data">Record data.</param>
        /// <param name="index">Current index.</param>
        /// <param name="step">Step value.</param>
        /// <returns>Region object.</returns>
        private Region ReadRegion(byte[] data, ref int index, int step)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Region region = null;
            int testNumber = BitConverter.ToInt32(data, index);
            index += IntSize;
            ObjectRegionInitState state = (ObjectRegionInitState)testNumber;

            switch (state)
            {
                case ObjectRegionInitState.Rectangle:
                    RectangleF rect = ReadRectangle(data, ref index, step);
                    region = new Region(rect);
                    break;

                case ObjectRegionInitState.GraphpicsPath:
                    GraphicsPath newPath = ReadRegionPath(data, ref index, step);
                    if (newPath != null)
                    {
                        region = new Region(newPath);
                    }
                    break;

                case ObjectRegionInitState.Empty:
                    region = new Region(RectangleF.Empty);
                    region.MakeEmpty();
                    break;

                case ObjectRegionInitState.Infinity:
                    region = new Region();
                    region.MakeInfinite();
                    break;
            }

            return region;
        }

        /// <summary>
        /// Combines regions.
        /// </summary>
        /// <param name="srcRegion">Source region.</param>
        /// <param name="dstRegion">Destination region.</param>
        /// <param name="mode">Combine mode.</param>
        /// <returns>Result region.</returns>
        private Region CombineRegion(Region srcRegion, Region dstRegion, CombineMode mode)
        {
            if (srcRegion == null)
                throw new ArgumentNullException("srcRegion");

            if (dstRegion == null)
                throw new ArgumentNullException("dstRegion");

            switch (mode)
            {
                case CombineMode.Complement:
                    srcRegion.Complement(dstRegion);
                    break;

                case CombineMode.Exclude:
                    srcRegion.Exclude(dstRegion);
                    break;

                case CombineMode.Intersect:
                    srcRegion.Intersect(dstRegion);
                    break;

                case CombineMode.Replace:
                    srcRegion = dstRegion.Clone() as Region;
                    break;

                case CombineMode.Union:
                    srcRegion.Union(dstRegion);
                    break;

                case CombineMode.Xor:
                    srcRegion.Xor(dstRegion);
                    break;
            }

            return srcRegion;
        }

        /// <summary>
        /// Returns index of the object in the table.
        /// </summary>
        /// <param name="flags">Flags data.</param>
        /// <returns>Index of the object in the table.</returns>
        private int GetIndex(int flags)
        {
            int index = flags & IndexFlag;

            return index;
        }

        /// <summary>
        /// Checks whether data contains color or index of the object.
        /// </summary>
        /// <param name="flags">Flags data.</param>
        /// <returns>Checks whether data contains color or index of the object.</returns>
        private bool ContainsColor(int flags)
        {
            bool value = ((flags & ColorFlag) > 0);

            return value;
        }

        /// <summary>
        /// Gets the fill mode.
        /// </summary>
        /// <param name="flags">The flags.</param>
        /// <returns></returns>
        private System.Drawing.Drawing2D.FillMode GetFillMode(int flags)
        {
            System.Drawing.Drawing2D.FillMode fm;

            if ((flags & PathFillWinding) != 0)
            {
                fm = System.Drawing.Drawing2D.FillMode.Winding;
            }
            else
            {
                fm = System.Drawing.Drawing2D.FillMode.Alternate;
            }

            return fm;
        }

        /// <summary>
        /// Gets the brush.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <param name="flags">The flags.</param>
        /// <returns></returns>
        private Brush GetBrush(byte[] data, ref int index, int flags)
        {
            Brush brush;

            if (ContainsColor(flags))
            {
                Color color = ReadColor(data, ref index);
                brush = new SolidBrush(color);
            }
            else
            {
                int brushIndex = BitConverter.ToInt32(data, index);
                index += IntSize;
                brush = Objects.GetBrush(brushIndex);
            }
            return brush;
        }

        /// <summary>
        /// Reads the RectL structure.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <returns>Rectangle structure initialized.</returns>
        private System.Drawing.Rectangle ReadRectL(byte[] data, ref int index)
        {
            int step = IntSize;

            int x = BitConverter.ToInt32(data, index);
            index += step;

            int y = BitConverter.ToInt32(data, index);
            index += step;

            int width = BitConverter.ToInt32(data, index);
            index += step;

            int height = BitConverter.ToInt32(data, index);
            index += step;

            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(x, y, width, height);

            return rect;
        }


        #endregion

#region Helper Methods

        /// <summary>
        ///  Starts processing of emf records.
        /// </summary>
        /// <param name="recordType">Type of record.</param>
        /// <param name="flags">Help flags.</param>
        /// <param name="dataSize">Size of the data for the record.</param>
        /// <param name="data">Pointer on the memory where data are located.</param>
        /// <param name="callbackData">Callback function.</param>		
        private void EmfProcess(EmfPlusRecordType recordType, int flags,
            int dataSize, IntPtr data, PlayRecordCallback callbackData)
        {
            if ((recordType == EmfPlusRecordType.GetDC) && (Type != MetafileType.EmfPlusDual))
            {
                m_bProcess = true;
                return;
            }

            if (m_bProcess)
            {
                bool processContinue = ((recordType >= EmfPlusRecordType.EmfMin) && (recordType <= EmfPlusRecordType.EmfMax));

                if (!processContinue)
                {
                    m_bProcess = false;
                }
                else
                {
                    base.EnumerateMetafile(recordType, flags, dataSize, data, callbackData);
                }
            }

        }

        /// <summary>
        /// Determines whether [is valid rect] [the specified rect].
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <returns>
        /// 	<c>true</c> if [is valid rect] [the specified rect]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsValidRect(RectangleF rect)
        {
            return !(float.IsNaN(rect.X) || float.IsNaN(rect.Y) || float.IsNaN(rect.Width) || float.IsNaN(rect.Height));
        }

        /// <summary>
        /// Converts bitmap to PNG.
        /// </summary>
        /// <param name="img">The img.</param>
        /// <returns></returns>
        private void ConvertToPng(Image img, out MemoryStream stream)
        {
            try
            {
                System.Drawing.Imaging.Encoder qualityEncoder = System.Drawing.Imaging.Encoder.Quality;
                int quality = 100;
                EncoderParameter ratio = new EncoderParameter(qualityEncoder, quality);
                EncoderParameters codecParams = new EncoderParameters(1);
                codecParams.Param[0] = ratio;

                ImageCodecInfo[] codecInfos = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo pngCodecInfo = null;
                stream = new MemoryStream();

                foreach (ImageCodecInfo ci in codecInfos)
                {
                    if (ci.MimeType == "image/png")
                    {
                        pngCodecInfo = ci;
                        break;
                    }
                }

                if (pngCodecInfo != null)
                {
                    img.Save(stream, pngCodecInfo, codecParams);
                }
            }
            catch(Exception ex)
            {
                stream = null;
            }
        }

        private bool CheckForComplexScripts(string text)
        {
            foreach (char character in text)
            {
                if ((int)character >= 2304 && (int)character <= 2431)
                    return true;
            }
            return false;
        }
        #endregion

    }
}
#endif