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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Syncfusion.Pdf.Graphics.Fonts;
using Syncfusion.Pdf.Native;

namespace Syncfusion.Pdf.Graphics.Images.Metafiles
{
    /// <summary>
    /// Class implementing capabilities to parse EMF metafiles.
    /// </summary>
    internal class EmfParser : MetafileParser
    {
#region Constants
        /// <summary>
        /// Pixels per inch amount
        /// </summary>
        private const int PointsPerInch = 72;

        /// <summary>
        /// Number of degrees in one radian.
        /// </summary>
        internal const float DegreeCount = 180.0f;
        #endregion

#region Fields
        private static object syncLock = new object();
        /// <summary>
        /// Help objects.
        /// </summary>
        private EmfObjectData m_objects;

        /// <summary>
        /// Type of metafile.
        /// </summary>
        private MetafileType m_type;
        /// <summary>
        /// Text Rotation Angle
        /// </summary>
        internal float TextAngle;
        /// <summary>
        /// Holds the selected font
        /// </summary>
        private Font m_selectedFont;
        #endregion

#region Properties
        /// <summary>
        /// Overloaded. Gets MetafileType.Emf
        /// </summary>
        public override MetafileType Type
        {
            get
            {
                return m_type;
            }
        }
        /// <summary>
        /// Gets help objects.
        /// </summary>
        private EmfObjectData Objects
        {
            get
            {
                return m_objects;
            }
        }
        /// <summary>
        /// Gets text region mananger.
        /// </summary>
        private TextRegionManager TextRegions
        {
            get
            {
                return Context as TextRegionManager;
            }
        }
        /// <summary>
        /// Gets image region mananger.
        /// </summary>
        private ImageRegionManager ImageRegions
        {
            get
            {
                return ImageContext as ImageRegionManager;
            }
        }
        #endregion

#region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EmfParser"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="dpi">The dpi.</param>
        public EmfParser(MetafileType type, SizeF dpi)
        {
            m_type = type;
            Context = new TextRegionManager();
            ImageContext = new ImageRegionManager();

            m_objects = new EmfObjectData(dpi);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmfParser"/> class.
        /// </summary>
        /// <param name="dpi">The dpi.</param>
        internal EmfParser(SizeF dpi)
            : base()
        {
            Context = new TextRegionManager();
            ImageContext = new ImageRegionManager();

            m_objects = new EmfObjectData(dpi);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmfParser"/> class.
        /// </summary>
        /// <param name="renderer">The renderer.</param>
        internal EmfParser(PdfEmfRenderer renderer)
            : base(renderer)
        {
        }

        /// <summary>
        /// Overloaded. Disposes resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            if (m_objects != null)
            {
                m_objects.Dispose();
                m_objects = null;
            }
        }
        #endregion

#region Overrides
        /// <summary>
        /// Overloaded. Creates handler of parsing function.
        /// </summary>
        /// <returns>Handler of parsing function.</returns>
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
        /// <param name="ptrData">Pointer on the memory where data are located.</param>
        /// <param name="callbackData">Callback function.</param>
        /// <returns>True - to proceed enumeration, False otherwise.</returns>
        internal bool EnumerateMetafile(EmfPlusRecordType recordType, int flags,
            int dataSize, IntPtr ptrData, PlayRecordCallback callbackData)
        {
            //Debug.WriteLine( recordType.ToString() );
            //DumpData( data, recordType );
            Renderer.m_recordType = recordType;

            if (recordType != EmfPlusRecordType.EmfExtTextOutW)
                Renderer.m_EMFState = false;

            lock (syncLock)
            {

                try
                {
                    switch (recordType)
                    {
                        case EmfPlusRecordType.EmfHeader:
                            Header(GetData(ptrData, dataSize));
                            break;
                        case EmfPlusRecordType.EmfSaveDC:
                            SaveDC(GetData(ptrData, dataSize));
                            break;
                        case EmfPlusRecordType.EmfRestoreDC:
                            RestoreDC(GetData(ptrData, dataSize));
                            break;
                        case EmfPlusRecordType.EmfEof:
                            EndOfFile();
                            break;

                        case EmfPlusRecordType.EmfAbortPath:
                            AbortPath(ptrData);
                            break;
                        case EmfPlusRecordType.EmfAlphaBlend:
                            AlphaBlend(ptrData);
                            break;
                        case EmfPlusRecordType.EmfSetIcmMode:
                            SetIcmMode(ptrData);
                            break;
                        case EmfPlusRecordType.EmfSetMiterLimit:
                            SetMiterLimit(ptrData);
                            break;
                        case EmfPlusRecordType.EmfModifyWorldTransform:
                            ModifyWorldTransform(ptrData);
                            break;
                        case EmfPlusRecordType.EmfExtCreatePen:
                            ExtCreatePen(ptrData);
                            break;
                        case EmfPlusRecordType.EmfSelectObject:
                            SelectObject(GetData(ptrData, dataSize));
                            break;
                        case EmfPlusRecordType.EmfDeleteObject:
                            DeleteObject(GetData(ptrData, dataSize));
                            break;
                        case EmfPlusRecordType.EmfCreateBrushIndirect:
                            CreateBrushIndirect(ptrData);
                            break;
                        case EmfPlusRecordType.EmfSetPolyFillMode:
                            SetPolyFillMode(ptrData);
                            break;
                        case EmfPlusRecordType.EmfPolyPolygon16:
                            PolyPolygon(GetData(ptrData, dataSize), false);
                            break;
                        case EmfPlusRecordType.EmfSetMapMode:
                            SetMapMode(ptrData);
                            break;
                        case EmfPlusRecordType.EmfSetWindowOrgEx:
                            SetWindowOrgEx(ptrData);
                            break;
                        case EmfPlusRecordType.EmfSetWindowExtEx:
                            SetWindowExtEx(ptrData);
                            break;
                        case EmfPlusRecordType.EmfSetViewportOrgEx:
                            SetViewportOrgEx(ptrData);
                            break;
                        case EmfPlusRecordType.EmfSetViewportExtEx:
                            SetViewportExtEx(ptrData);
                            break;
                        case EmfPlusRecordType.EmfBeginPath:
                            BeginPath(ptrData);
                            break;
                        case EmfPlusRecordType.EmfMoveToEx:
                            MoveToEx(ptrData);
                            break;
                        case EmfPlusRecordType.EmfLineTo:
                            LineTo(ptrData);
                            break;
                        case EmfPlusRecordType.EmfEndPath:
                            EndPath(ptrData);
                            break;
                        case EmfPlusRecordType.EmfSelectClipPath:
                            SelectClipPath(ptrData);
                            break;
                        case EmfPlusRecordType.EmfPolygon16:
                            Polygon16(ptrData);
                            break;
                        case EmfPlusRecordType.EmfPolyBezier:
                            PolyBezier(ptrData);
                            break;
                        case EmfPlusRecordType.EmfPolyBezier16:
                            PolyBezier16(ptrData);
                            break;
                        case EmfPlusRecordType.EmfPolygon:
                            Polygon(ptrData);
                            break;
                        case EmfPlusRecordType.EmfPolyline:
                            Polyline(ptrData);
                            break;
                        case EmfPlusRecordType.EmfPolyline16:
                            Polyline16(ptrData);
                            break;
                        case EmfPlusRecordType.EmfPolyBezierTo:
                            PolyBezierTo(ptrData);
                            break;
                        case EmfPlusRecordType.EmfPolyBezierTo16:
                            PolyBezierTo16(ptrData);
                            break;
                        case EmfPlusRecordType.EmfPolyLineTo:
                            PolyLineTo(ptrData);
                            break;
                        case EmfPlusRecordType.EmfPolylineTo16:
                            PolyLineTo16(ptrData);
                            break;
                        case EmfPlusRecordType.EmfPolyPolyline:
                            PolyPolyline(GetData(ptrData, dataSize), true);
                            break;
                        case EmfPlusRecordType.EmfPolyPolygon:
                            PolyPolygon(GetData(ptrData, dataSize), true);
                            break;
                        case EmfPlusRecordType.EmfOffsetClipRgn:
                            OffsetClipRgn(ptrData);
                            break;
                        case EmfPlusRecordType.EmfExcludeClipRect:
                            ExcludeClipRect(ptrData);
                            break;
                        case EmfPlusRecordType.EmfIntersectClipRect:
                            IntersectClipRect(ptrData);
                            break;
                        case EmfPlusRecordType.EmfPolyDraw:
                            PolyDraw(GetData(ptrData, dataSize), true);
                            break;
                        case EmfPlusRecordType.EmfSetArcDirection:
                            SetArcDirection(ptrData);
                            break;
                        case EmfPlusRecordType.EmfFlattenPath:
                            FlattenPath(ptrData);
                            break;
                        case EmfPlusRecordType.EmfWidenPath:
                            WidenPath(ptrData);
                            break;
                        case EmfPlusRecordType.EmfFillRgn:
                            FillRgn(GetData(ptrData, dataSize), ptrData);
                            break;
                        case EmfPlusRecordType.EmfPaintRgn:
                            PaintRgn(GetData(ptrData, dataSize), ptrData);
                            break;
                        case EmfPlusRecordType.EmfExtSelectClipRgn:
                            ExtSelectClipRgn(GetData(ptrData, dataSize), ptrData);
                            break;
                        case EmfPlusRecordType.EmfPolyDraw16:
                            PolyDraw(GetData(ptrData, dataSize), false);
                            break;
                        case EmfPlusRecordType.EmfSetBkMode:
                            SetBkMode(ptrData);
                            break;
                        case EmfPlusRecordType.EmfSetTextAlign:
                            SetTextAlign(ptrData);
                            break;
                        case EmfPlusRecordType.EmfSetTextColor:
                            SetTextColor(ptrData);
                            break;
                        case EmfPlusRecordType.EmfSetBkColor:
                            SetBkColor(ptrData);
                            break;
                        case EmfPlusRecordType.EmfSetWorldTransform:
                            SetWorldTransform(ptrData);
                            break;
                        case EmfPlusRecordType.EmfCreatePen:
                            CreatePen(ptrData);
                            break;
                        case EmfPlusRecordType.EmfAngleArc:
                            AngleArc(ptrData);
                            break;
                        case EmfPlusRecordType.EmfEllipse:
                            Ellipse(ptrData);
                            break;
                        case EmfPlusRecordType.EmfRectangle:
                            RectangleEx(ptrData);
                            break;
                        case EmfPlusRecordType.EmfRoundRect:
                            RoundRect(ptrData);
                            break;
                        case EmfPlusRecordType.EmfChord:
                            Chord(ptrData);
                            break;
                        case EmfPlusRecordType.EmfPie:
                            Pie(ptrData);
                            break;
                        case EmfPlusRecordType.EmfArcTo:
                            ArcTo(ptrData, true);
                            break;
                        case EmfPlusRecordType.EmfRoundArc:
                            ArcTo(ptrData, false);
                            break;
                        case EmfPlusRecordType.EmfCloseFigure:
                            CloseFigure(ptrData);
                            break;
                        case EmfPlusRecordType.EmfFillPath:
                            FillPath(ptrData);
                            break;
                        case EmfPlusRecordType.EmfStrokeAndFillPath:
                            StrokeAndFillPath(ptrData);
                            break;
                        case EmfPlusRecordType.EmfStrokePath:
                            StrokePath(ptrData);
                            break;
                        case EmfPlusRecordType.EmfStretchDIBits:
                            StretchDIBits(ptrData);
                            break;
                        case EmfPlusRecordType.EmfBitBlt:
                            BitBlt(ptrData);
                            break;
                        case EmfPlusRecordType.EmfStretchBlt:
                            StretchBlt(ptrData);
                            break;
                        case EmfPlusRecordType.EmfExtCreateFontIndirect:
                            ExtCreateFontIndirect(ptrData);
                            break;
                        case EmfPlusRecordType.EmfExtTextOutA:
                            ExtTextOut(ptrData, false);
                            break;
                        case EmfPlusRecordType.EmfExtTextOutW:
                            ExtTextOut(ptrData, true);
                            break;
                        case EmfPlusRecordType.EmfCreateDibPatternBrushPt:
                            CreateDibPatternBrushPt(ptrData);
                            break;
                        case EmfPlusRecordType.EmfSetStretchBltMode:
                            SetStretchBltMode(ptrData);
                            break;
                        case EmfPlusRecordType.EmfMaskBlt:
                            MaskBlt(ptrData);
                            break;
                        case EmfPlusRecordType.EmfSetLayout:
                            SetLayout(ptrData);
                            break;
                        case EmfPlusRecordType.EmfScaleViewportExtEx:
                            ScaleViewportExtEx(ptrData);
                            break;
                        case EmfPlusRecordType.EmfScaleWindowExtEx:
                            ScaleWindowExtEx(ptrData);
                            break;
                        case EmfPlusRecordType.EmfSetPixelV:
                            SetPixelV(ptrData);
                            break;
                        case EmfPlusRecordType.EmfSetMetaRgn:
                            SetMetaRgn(ptrData);
                            break;
                        case EmfPlusRecordType.EmfPolyPolyline16:
                            PolyPolyline(GetData(ptrData, dataSize), false);
                            break;
                        case EmfPlusRecordType.EmfTransparentBlt:
                            TransparentBlt(ptrData);
                            break;
                        default:
                            //Debug.WriteLine( "The record is not implemented: " + recordType.ToString() );
                            Renderer.m_EMFState = false;
                            break;
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Error occured: " + e.Message + Environment.NewLine + e.StackTrace);
                }
                Renderer.m_previousRecordtype = recordType;
                return true;
            }
        }
        #endregion

#region Parsing methods
        /// <summary>
        /// Starts enumeration.
        /// </summary>
        private void Header(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            int index = 0;
            RectangleF pixSize = ReadRECT(data, ref index);
            RectangleF unitSize = ReadRECT(data, ref index);

            int prMode = GdiApi.SetMapMode(Objects.Handle, (int)MAPPING_MODE.MM_ANISOTROPIC);

            //SIZE oldSize = new SIZE();
            POINT oldPt = new POINT();

            float lWidth = pixSize.Width;
            float lHeight = pixSize.Height;

            //GdiApi.SetWindowExtEx( Objects.Handle, ( int )lWidth, ( int )lHeight, ref oldSize );
            //GdiApi.SetViewportExtEx( Objects.Handle, ( int )pixSize.Width, ( int )pixSize.Height, ref oldSize );
            GdiApi.SetWindowOrgEx(Objects.Handle, (int)pixSize.X, (int)pixSize.Y, ref oldPt);
            //GdiApi.SetViewportOrgEx( Objects.Handle, ( int )pixSize.X, ( int )pixSize.Y, ref oldPt);




            //RectangleF bounds = new RectangleF( PointF.Empty, new SizeF( pixSize.Width - pixSize.X, pixSize.Height - pixSize.Y ) );
            RectangleF bounds = new RectangleF(PointF.Empty, new SizeF(pixSize.Width + 1, pixSize.Height + 1));
            SizeF size = Renderer.Graphics.Size;

            float scaleX = size.Width / bounds.Width;
            float scaleY = size.Height / bounds.Height;
            //Renderer.ScaleTransform( scaleX, scaleY, MatrixOrder.Append );
            //Renderer.Graphics.TranslateTransform( scaleX, scaleY );
            //Renderer.SetBBox( bounds );

            PointF location = pixSize.Location;

            if (location != PointF.Empty && location.X <= size.Width && location.Y <= size.Height)
            {
                Renderer.TranslateTransform(location.X, location.Y, MatrixOrder.Append);
            }
            // Notify about start of parsing.
            Renderer.BeforeStart();
            //RectangleF[] rects = new RectangleF[] { pixSize };
            //Renderer.FillRectangles( new SolidBrush( Color.LightGray ), rects );
        }

        /// <summary>
        /// Finishes enumeration.
        /// </summary>
        private void EndOfFile()
        {
            Renderer.BeforeEnd();
        }

        /// <summary>
        /// Saves graphic state of the graphics context.
        /// </summary>
        /// <param name="data">Data for the record.</param>
        private void SaveDC(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            GdiApi.SaveDC(Objects.Handle);
            Objects.GraphicsState = Renderer.Save();
            Objects.Save();

        }

        /// <summary>
        /// Restores device context to the previous state.
        /// </summary>
        /// <param name="data">Data for the record.</param>
        private void RestoreDC(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            int id = BitConverter.ToInt32(data, 0);

            GdiApi.RestoreDC(Objects.Handle, id);
            Renderer.Restore(Objects.GraphicsState);

            Objects.Restore(id);
        }

        /// <summary>
        /// Stores miter limit.
        /// </summary>
        /// <param name="ptr">Pointer to data.</param>
        private void SetMiterLimit(IntPtr ptr)
        {
            Type type = typeof(EMR_SETMITERLIMIT);
            EMR_SETMITERLIMIT recordData = (EMR_SETMITERLIMIT)GetStructure(ptr, type);
            float previous;
            bool result = GdiApi.SetMiterLimit(Objects.Handle, recordData.eMiterLimit, out previous);
            MetafileParser.CheckResult(result);

            if (Objects.Pen != null)
            {
                Objects.Pen.MiterLimit = LPtoDPX(recordData.eMiterLimit);
            }
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">Pointer on record data.</param>
        private void ModifyWorldTransform(IntPtr ptr)
        {
            Type type = typeof(EMR_MODIFYWORLDTRANSFORM);
            EMR_MODIFYWORLDTRANSFORM recordData = (EMR_MODIFYWORLDTRANSFORM)GetStructure(ptr, type);
            if (Math.Abs(recordData.xform.eM12) < 0.001 && Math.Abs(recordData.xform.eM21) < 0.001)
            {
                recordData.xform.eM12 = 0;
                recordData.xform.eM21 = 0;
            }
            TextAngle = CalculateRotationAngle(recordData);

            // Set proper graphics mode.
            SetValidGraphicsMode();

            bool result = GdiApi.ModifyWorldTransform(Objects.Handle, ref recordData.xform,
                (int)recordData.iMode);
            CheckResult(result);

            // NOTE: do not apply transformartion to destination graphics.
            // Convert points to device units by using LPtoDP instead.
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">Pointer on record data.</param>
        private void ExtCreatePen(IntPtr ptr)
        {
            Type type = typeof(EMR_EXTCREATEPEN);
            EMR_EXTCREATEPEN recordData = new EMR_EXTCREATEPEN();
            recordData = (EMR_EXTCREATEPEN)GetStructureEx(ptr, recordData);

            PS_PEN_TYPE flag = (PS_PEN_TYPE)(recordData.elpPenStyle &
                WinGdiConst.PS_TYPE_MASK);

            // TODO: this doesn't work properly.
            float width = recordData.elpWidth;

            if (flag == PS_PEN_TYPE.PS_GEOMETRIC)
            {
                width = LPtoDPWidth(width);
            }

            Color color = ColorTranslator.FromWin32(recordData.elpColor);
            Pen pen = new Pen(color, width);

            float miterLimit;
            bool result = GdiApi.GetMiterLimit(Objects.Handle, out miterLimit);

            if (result)
            {
                pen.MiterLimit = miterLimit;
            }

            PS_PEN_STYLE penStyle = (PS_PEN_STYLE)(recordData.elpPenStyle &
                WinGdiConst.PS_STYLE_MASK);

            if (penStyle < PS_PEN_STYLE.PS_NULL)
            {
                pen.DashStyle = (System.Drawing.Drawing2D.DashStyle)penStyle;
            }

            PS_PEN_CAP_STYLE capStyle = (PS_PEN_CAP_STYLE)(recordData.elpPenStyle &
                WinGdiConst.PS_ENDCAP_MASK);

            switch (capStyle)
            {
                case PS_PEN_CAP_STYLE.PS_ENDCAP_FLAT:
                    pen.EndCap = System.Drawing.Drawing2D.LineCap.Flat;
                    break;
                case PS_PEN_CAP_STYLE.PS_ENDCAP_ROUND:
                    pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                    break;
                case PS_PEN_CAP_STYLE.PS_ENDCAP_SQUARE:
                    pen.EndCap = System.Drawing.Drawing2D.LineCap.Square;
                    break;
            }

            PS_PEN_JOIN_STYLE joinStyle = (PS_PEN_JOIN_STYLE)(recordData.elpPenStyle &
                WinGdiConst.PS_JOIN_MASK);

            switch (joinStyle)
            {
                case PS_PEN_JOIN_STYLE.PS_JOIN_BEVEL:
                    pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Bevel;
                    break;
                case PS_PEN_JOIN_STYLE.PS_JOIN_MITER:
                    pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Miter;
                    break;
                case PS_PEN_JOIN_STYLE.PS_JOIN_ROUND:
                    pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
                    break;
            }

            int handleId = recordData.ihPen;
            Objects.SelectedObjects.AddObject(pen, handleId);
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="data">The data.</param>
        private void SelectObject(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            int objId = BitConverter.ToInt32(data, 0);
            object obj = Objects.SelectedObjects.SelectObject(objId);

            Objects.SelectObject(obj);

            if (obj is FontEx)
                m_selectedFont = (obj as FontEx).Font;
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="data">The data.</param>
        private void DeleteObject(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            int objId = BitConverter.ToInt32(data, 0);
            object obj = Objects.SelectedObjects.DeleteObject(objId);

            Objects.DeleteObject(obj);
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">Pointer on record data.</param>
        private void CreateBrushIndirect(IntPtr ptr)
        {
            Type type = typeof(EMR_CREATEBRUSHINDIRECT);
            EMR_CREATEBRUSHINDIRECT recordData = (EMR_CREATEBRUSHINDIRECT)GetStructure(ptr, type);

            Brush brush = null;
            Color color = ColorTranslator.FromWin32(recordData.lb.lbColor);

            switch (recordData.lb.lbStyle)
            {
                case BS_BRUSH_STYLE.BS_SOLID:
                    brush = new SolidBrush(color);
                    break;

                case BS_BRUSH_STYLE.BS_NULL:
                    brush = new SolidBrush(Color.Empty);
                    break;

                case BS_BRUSH_STYLE.BS_HATCHED:
                    HatchStyle hatchStyle = (HatchStyle)recordData.lb.lbHatch;
                    int ibkColor = GdiApi.GetBkColor(Objects.Handle);
                    Color bkColor = ColorTranslator.FromWin32(ibkColor);
                    brush = new HatchBrush(hatchStyle, color, bkColor);
                    break;
                // TODO: implement the rest of brushes.
            }

            if (brush != null)
            {
                int objId = recordData.ihBrush;
                Objects.SelectedObjects.AddObject(brush, objId);
            }
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">Pointer on record data.</param>
        private void SetPolyFillMode(IntPtr ptr)
        {
            Type type = typeof(EMR_SELECTCLIPPATH);
            EMR_SELECTCLIPPATH recordData = (EMR_SELECTCLIPPATH)GetStructure(ptr, type);

            System.Drawing.Drawing2D.FillMode mode = (System.Drawing.Drawing2D.FillMode)(recordData.iMode - 1);
            Objects.FillMode = mode;
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="data">Record data.</param>
        /// <param name="bIs32Bit">Indicates if it's 32 or 16 bit version.</param>
        private void PolyPolygon(byte[] data, bool bIs32Bit)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            // NOTE: Marshaling data to EMR_POLYPOLYLINE16 structure was unabled.
            // So we get retrieve data manually.

            int index = 0;
            int step = IntSize;

            RECT bounds = ReadRECT(data, ref index);
            int numPolys = BitConverter.ToInt32(data, index);
            index += step;
            int numPoints = BitConverter.ToInt32(data, index);
            index += step;

            int[] numPointsInPoly = ReadInt32Array(data, numPolys, ref index);
            Point[] polyPoints = ReadPointArray(data, numPoints, ref index, bIs32Bit);
            int pointIndex = 0;
            GraphicsPath path = new GraphicsPath(Objects.FillMode);

            for (int i = 0; i < numPolys; i++)
            {
                int polyCount = numPointsInPoly[i];
                PointF[] polygon = new PointF[polyCount];
                int polyPolyIndex = 0;
                bool different = false;

                for (int j = pointIndex, len = pointIndex + polyCount; j < len; j++)
                {
                    PointF point = polyPoints[j];

                    polygon[polyPolyIndex] = point;

                    if (polyPolyIndex > 0)
                    {
                        PointF prevPt = polygon[polyPolyIndex - 1];

                        if (prevPt != point)
                        {
                            different = true;
                        }
                    }

                    polyPolyIndex++;
                }

                pointIndex += polyPolyIndex;

                if (different)
                {
                    path.AddPolygon(polygon);
                    path.CloseFigure();
                }
            }


            if (Objects.IsOpenPath)
            {
                // NOTE: it needs to be checked if last parameter has to be true or false.
                Objects.Path.AddPath(path, false);
            }
            else
            {
                if (Objects.Brush != null && Objects.FillMode == FillMode.Winding)
                {
                    Renderer.FillPath(Objects.Brush, path);
                }
                else if (Objects.Brush != null && Objects.FillMode == FillMode.Alternate)
                {
                    Renderer.FillRegion(Objects.Brush, new Region(path));
                }
                if(Objects.Pen != null && Objects.Pen.Color.A != 0)
                {
                    Renderer.DrawPath(Objects.Pen, path);
                }
            }

            path.Dispose();
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">The PTR.</param>
        private void SetMapMode(IntPtr ptr)
        {
            Type type = typeof(EMR_SELECTCLIPPATH);
            EMR_SELECTCLIPPATH recordData = (EMR_SELECTCLIPPATH)GetStructure(ptr, type);

            GdiApi.SetMapMode(Objects.Handle, recordData.iMode);
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">The PTR.</param>
        private void SetWindowOrgEx(IntPtr ptr)
        {
            Type type = typeof(EMR_SETVIEWPORTEXTEX);
            EMR_SETVIEWPORTEXTEX recordData = (EMR_SETVIEWPORTEXTEX)GetStructure(ptr, type);

            SIZE newSize = recordData.szlExtent;
            POINT oldPt = new POINT();
            GdiApi.SetWindowOrgEx(Objects.Handle, newSize.cx, newSize.cy, ref oldPt);
            //Renderer.TranslateTransform( -/*oldPt.x + */newSize.cx, -/*oldPt.y +*/ newSize.cy, MatrixOrder.Append );
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">The PTR.</param>
        private void SetWindowExtEx(IntPtr ptr)
        {
            Type type = typeof(EMR_SETVIEWPORTEXTEX);
            EMR_SETVIEWPORTEXTEX recordData = (EMR_SETVIEWPORTEXTEX)GetStructure(ptr, type);

            SIZE newSize = recordData.szlExtent;
            SIZE oldSize = new SIZE();

            GdiApi.SetWindowExtEx(Objects.Handle, newSize.cx, newSize.cy, ref oldSize);
            
            oldSize = new SIZE();
            GdiApi.GetViewportExtEx(Objects.Handle, ref oldSize);
            if (oldSize.cx <= 1 && oldSize.cy <= 1)
            {                    
                oldSize = new SIZE();
                GdiApi.SetViewportExtEx(Objects.Handle, newSize.cx, newSize.cy, ref oldSize);
            }            
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">The PTR.</param>
        private void SetViewportOrgEx(IntPtr ptr)
        {
            Type type = typeof(EMR_SETVIEWPORTORGEX);
            EMR_SETVIEWPORTORGEX recordData = (EMR_SETVIEWPORTORGEX)GetStructure(ptr, type);

            POINT newPoint = recordData.ptlOrigin;
            POINT oldPoint = new POINT();
            GdiApi.SetViewportOrgEx(Objects.Handle, newPoint.x, newPoint.y, ref oldPoint);
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">The PTR.</param>
        private void SetViewportExtEx(IntPtr ptr)
        {
            Type type = typeof(EMR_SETVIEWPORTEXTEX);
            EMR_SETVIEWPORTEXTEX recordData = (EMR_SETVIEWPORTEXTEX)GetStructure(ptr, type);

            SIZE newSize = recordData.szlExtent;
            SIZE oldSize = new SIZE();

            GdiApi.SetViewportExtEx(Objects.Handle, newSize.cx, newSize.cy, ref oldSize);

            //if( !IsExtentEmpty( newSize ) )
            //{
            //  SizeF graphSize = oldSize; //Renderer.Graphics.ActualSize;

            //  if( graphSize == SizeF.Empty || IsExtentEmpty( oldSize ) )
            //  {
            //    graphSize = Renderer.Graphics.ActualSize;
            //  }

            //  float kx = graphSize.Width / newSize.cx;
            //  float ky = graphSize.Height / newSize.cy;
            //  Renderer.ScaleTransform( kx, ky, MatrixOrder.Append );
            //}
        }

        private bool IsExtentEmpty(SIZE extent)
        {
            return (extent.cx <= 1 && extent.cy <= 1);
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">The PTR.</param>
        private void ScaleViewportExtEx(IntPtr ptr)
        {
            Type type = typeof(EMR_SCALEVIEWPORTEXTEX);
            EMR_SCALEVIEWPORTEXTEX recordData = (EMR_SCALEVIEWPORTEXTEX)GetStructure(ptr, type);

            SIZE oldSize = new SIZE();
            bool result = GdiApi.ScaleViewportExtEx(Objects.Handle, recordData.xNum, recordData.xDenom,
                recordData.yNum, recordData.yDenom, ref oldSize);

            MetafileParser.CheckResult(result);
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">The PTR.</param>
        private void ScaleWindowExtEx(IntPtr ptr)
        {
            Type type = typeof(EMR_SCALEVIEWPORTEXTEX);
            EMR_SCALEVIEWPORTEXTEX recordData = (EMR_SCALEVIEWPORTEXTEX)GetStructure(ptr, type);

            SIZE oldSize = new SIZE();
            bool result = GdiApi.ScaleWindowExtEx(Objects.Handle, recordData.xNum, recordData.xDenom,
                recordData.yNum, recordData.yDenom, ref oldSize);

            MetafileParser.CheckResult(result);
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">The PTR.</param>
        private void BeginPath(IntPtr ptr)
        {
            bool result = GdiApi.BeginPath(Objects.Handle);
            CheckResult(result);

            System.Drawing.Drawing2D.FillMode mode = Objects.FillMode;
            Objects.Path = new GraphicsPath(mode);

            // There is open path object at device context.
            Objects.IsOpenPath = true;
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">The PTR.</param>
        private void MoveToEx(IntPtr ptr)
        {
            Type type = typeof(EMR_LINETO);
            EMR_LINETO recordData = (EMR_LINETO)GetStructure(ptr, type);
            POINT newCurPoint = recordData.ptl;

            Objects.CurrentPoint = newCurPoint;
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">The PTR.</param>
        private void LineTo(IntPtr ptr)
        {
            Type type = typeof(EMR_LINETO);
            EMR_LINETO recordData = (EMR_LINETO)GetStructure(ptr, type);
            POINT newCurPoint = recordData.ptl;

            PointF curPoint = Objects.CurrentPoint;
            PointF lastPoint = new PointF(newCurPoint.x, newCurPoint.y);

            // Transform coordinates.
            curPoint = LPtoDP(curPoint);
            curPoint.X += (Objects.Pen.Width / 2);
            curPoint.Y += (Objects.Pen.Width / 2);
            lastPoint = LPtoDP(lastPoint);
            lastPoint.X += (Objects.Pen.Width / 2);
            lastPoint.Y += (Objects.Pen.Width / 2);

            bool result = GdiApi.LineTo(Objects.Handle, newCurPoint.x, newCurPoint.y);
            CheckResult(result);

            if (Objects.IsOpenPath)
            {
                Objects.Path.AddLine(curPoint, lastPoint);
            }
            else if (Objects.Pen != null)
            {
                PointF[] points = new PointF[] { curPoint, lastPoint };
                Renderer.DrawLines(Objects.Pen, points);
            }
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">The PTR.</param>
        private void EndPath(IntPtr ptr)
        {
            bool result = GdiApi.EndPath(Objects.Handle);
            CheckResult(result);

            //GraphicsPath path = Objects.Path;

            //if( path != null )
            //{
            //  path.CloseAllFigures();
            //}

            // We close open path object.
            Objects.IsOpenPath = false;
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">The PTR.</param>
        private void AbortPath(IntPtr ptr)
        {
            bool result = GdiApi.AbortPath(Objects.Handle);
            CheckResult(result);

            GraphicsPath path = Objects.Path;

            if (path != null)
            {
                Objects.Path = null;
            }

            Objects.IsOpenPath = false;
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">The PTR.</param>
        private void SelectClipPath(IntPtr ptr)
        {
            Type type = typeof(EMR_SELECTCLIPPATH);
            EMR_SELECTCLIPPATH recordData = (EMR_SELECTCLIPPATH)GetStructure(ptr, type);

            bool result = GdiApi.SelectClipPath(Objects.Handle, recordData.iMode);
            CheckResult(result);

            GraphicsPath path = Objects.Path;

            if (path != null)
            {
                COMBINE_RGN iMode = (COMBINE_RGN)recordData.iMode;
                int icMode = (iMode == COMBINE_RGN.RGN_MAX) ?
                    (int)COMBINE_RGN.RGN_MIN - 1 : (int)iMode;

                CombineMode mode = (CombineMode)icMode;
                Renderer.SetClip(path, mode);
            }
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">The PTR.</param>
        private void Polygon16(IntPtr ptr)
        {
            EMR_POLYLINE16 objData = new EMR_POLYLINE16();
            objData = (EMR_POLYLINE16)GetStructureEx(ptr, objData);

            PointF[] points = ConvertType(objData.apts);

            try
            {
                //if (Objects.IsOpenPath || ((Objects.Brush as SolidBrush).Color.Name == "ffffffff") || ((Objects.Pen as Pen).Color.Name == "ffffff"))
                if (Objects.IsOpenPath )
                {
                    Objects.Path.AddPolygon(points);
                }
                else
                {
                    GraphicsPath path = new GraphicsPath(Objects.FillMode);
                    path.AddPolygon(points);

                    if (Objects.Brush != null)
                    {
                        Renderer.FillPath(Objects.Brush, path);
                    }

                    if (Objects.Pen != null && Objects.Pen.Color.A != 0)
                    {
                        Renderer.DrawPath(Objects.Pen, path);
                    }

                    path.Dispose();
                }
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Can't draw Polygon16");
            }
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">The PTR.</param>
        private void SetIcmMode(IntPtr ptr)
        {
            Type type = typeof(EMR_SELECTCLIPPATH);
            EMR_SELECTCLIPPATH recordData = (EMR_SELECTCLIPPATH)GetStructure(ptr, type);

            GdiApi.SetICMMode(Objects.Handle, recordData.iMode);
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">The PTR.</param>
        private void AlphaBlend(IntPtr ptr)
        {
            Type type = typeof(EMR_ALPHABLEND);
            EMR_ALPHABLEND recordData = (EMR_ALPHABLEND)GetStructure(ptr, type);

            if (Objects.Image != null)
            {
                RectangleF srcRect = RectangleF.Empty;
                srcRect.X = LPtoDPX(recordData.xSrc);
                srcRect.Y = LPtoDPY(recordData.ySrc);
                srcRect.Width = LPtoDPWidth(recordData.cxSrc);
                srcRect.Height = LPtoDPHeight(recordData.cySrc);

                RectangleF destRect = RectangleF.Empty;
                destRect.X = LPtoDPX(recordData.xDest);
                destRect.Y = LPtoDPY(recordData.yDest);
                destRect.Width = LPtoDPWidth(recordData.cxDest);
                destRect.Height = LPtoDPHeight(recordData.cyDest);

                Renderer.DrawImage(Objects.Image, destRect, srcRect,
                    Objects.Graphics.PageUnit);
            }
            else
            {
                //Note : Temp Fix

                int bitmapInfoOffset = recordData.offBmiSrc;

                // NOTE: shift 8 bytes because EMR structure is not included here.
                bitmapInfoOffset -= (2 * IntSize);

                IntPtr bitmapInfoPtr = new IntPtr(ptr.ToInt64() + bitmapInfoOffset);

                type = typeof(BITMAPINFOHEADER);
                BITMAPINFOHEADER bitmapHeader = (BITMAPINFOHEADER)GetStructure(bitmapInfoPtr, type);


                int imageOffset = recordData.offBitsSrc;
                uint imgSize = recordData.cbBitsSrc;

                System.Drawing.Rectangle srcRect = new System.Drawing.Rectangle(recordData.xSrc, recordData.ySrc,
                    recordData.cxSrc, recordData.cySrc);
                System.Drawing.Rectangle destRect = new System.Drawing.Rectangle(recordData.xDest, recordData.yDest,
                    recordData.cxDest, recordData.cyDest);

                // Convert coordinates:
                destRect = System.Drawing.Rectangle.Truncate(LPtoDP(destRect));
                srcRect = System.Drawing.Rectangle.Truncate(LPtoDP(srcRect));

                Image img = GetAlphaBlendedBitmap(imageOffset, imgSize, ptr, bitmapInfoPtr, bitmapHeader, recordData.iUsageSrc);

                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(img);

                // Store the information about image region at the context.
                if (destRect.Height > 0)
                {
                    //Skip the invalid regions.
                    if (destRect.Location.Y != 0)
                    {
                        ImageRegion rgn = new ImageRegion(destRect.Location.Y, destRect.Height);
                        ImageRegions.Add(rgn);
                    }
                }
                Renderer.DrawImage(img, destRect, srcRect, g.PageUnit);

                g.Dispose();
            }
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">The PTR.</param>
        private void PolyBezier(IntPtr ptr)
        {
            EMR_POLYLINE objData = new EMR_POLYLINE();
            objData = (EMR_POLYLINE)GetStructureEx(ptr, objData);

            PointF[] points = ConvertType(objData.apts);

            if (Objects.IsOpenPath)
            {
                Objects.Path.AddBeziers(points);
            }
            else if (Objects.Pen != null)
            {
                Renderer.DrawBeziers(Objects.Pen, points);
            }
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">The PTR.</param>
        private void PolyBezier16(IntPtr ptr)
        {
            EMR_POLYLINE16 objData = new EMR_POLYLINE16();
            objData = (EMR_POLYLINE16)GetStructureEx(ptr, objData);

            PointF[] points = ConvertType(objData.apts);

            if (Objects.IsOpenPath)
            {
                Objects.Path.AddBeziers(points);
            }
            else if (Objects.Pen != null)
            {
                Renderer.DrawBeziers(Objects.Pen, points);
            }
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">The PTR.</param>
        private void Polygon(IntPtr ptr)
        {
            EMR_POLYLINE objData = new EMR_POLYLINE();
            objData = (EMR_POLYLINE)GetStructureEx(ptr, objData);

            PointF[] points = ConvertType(objData.apts);

            try
            {
                if (Objects.IsOpenPath)
                {
                    Objects.Path.AddPolygon(points);
                }
                else
                {
                    GraphicsPath path = new GraphicsPath(Objects.FillMode);
                    path.AddPolygon(points);

                    if (Objects.Brush != null)
                    {
                        Renderer.FillPath(Objects.Brush, path);
                    }

                    if (Objects.Pen != null)
                    {
                        Renderer.DrawPath(Objects.Pen, path);
                    }

                    path.Dispose();
                }
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Can't draw Polygon16");
            }
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">The PTR.</param>
        private void Polyline(IntPtr ptr)
        {
            EMR_POLYLINE objData = new EMR_POLYLINE();
            objData = (EMR_POLYLINE)GetStructureEx(ptr, objData);

            PointF[] points = ConvertType(objData.apts);

            if (Objects.IsOpenPath)
            {
                Objects.Path.AddLines(points);
            }
            else if (Objects.Pen != null)
            {
                Renderer.DrawLines(Objects.Pen, points);
            }
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">The PTR.</param>
        private void Polyline16(IntPtr ptr)
        {
            EMR_POLYLINE16 objData = new EMR_POLYLINE16();
            objData = (EMR_POLYLINE16)GetStructureEx(ptr, objData);

            PointF[] points = ConvertType(objData.apts);

            if (Objects.IsOpenPath)
            {
                Objects.Path.AddLines(points);
            }
            else if (Objects.Pen != null)
            {
                Renderer.DrawLines(Objects.Pen, points);
            }
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">The PTR.</param>
        private void PolyBezierTo(IntPtr ptr)
        {
            EMR_POLYLINE objData = new EMR_POLYLINE();
            objData = (EMR_POLYLINE)GetStructureEx(ptr, objData);

            PointF[] points = ConvertType(objData.apts);
            PointF[] newPoints = AddCurrentPointTo(points);

            GdiApi.PolyBezierTo(Objects.Handle, objData.apts, objData.cpts);

            if (Objects.IsOpenPath)
            {
                Objects.Path.AddBeziers(newPoints);
            }
            else if (Objects.Pen != null)
            {
                Renderer.DrawBeziers(Objects.Pen, newPoints);
            }
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">The PTR.</param>
        private void PolyBezierTo16(IntPtr ptr)
        {
            EMR_POLYLINE16 objData = new EMR_POLYLINE16();
            objData = (EMR_POLYLINE16)GetStructureEx(ptr, objData);

            PointF[] points = ConvertType(objData.apts);
            PointF[] newPoints = AddCurrentPointTo(points);

            POINT[] pts = ConvertTypeEx(objData.apts);
            GdiApi.PolyBezierTo(Objects.Handle, pts, objData.cpts);

            if (Objects.IsOpenPath)
            {
                Objects.Path.AddBeziers(newPoints);
            }
            else if (Objects.Pen != null)
            {
                Renderer.DrawBeziers(Objects.Pen, newPoints);
            }
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">The PTR.</param>
        private void PolyLineTo(IntPtr ptr)
        {
            EMR_POLYLINE objData = new EMR_POLYLINE();
            objData = (EMR_POLYLINE)GetStructureEx(ptr, objData);

            PointF[] points = ConvertType(objData.apts);
            PointF[] newPoints = AddCurrentPointTo(points);

            GdiApi.PolylineTo(Objects.Handle, objData.apts, objData.cpts);

            if (Objects.IsOpenPath)
            {
                Objects.Path.AddLines(newPoints);
            }
            else if (Objects.Pen != null)
            {
                Renderer.DrawLines(Objects.Pen, newPoints);
            }
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">The PTR.</param>
        private void PolyLineTo16(IntPtr ptr)
        {
            EMR_POLYLINE16 objData = new EMR_POLYLINE16();
            objData = (EMR_POLYLINE16)GetStructureEx(ptr, objData);

            PointF[] points = ConvertType(objData.apts);
            PointF[] newPoints = AddCurrentPointTo(points);

            POINT[] pts = ConvertTypeEx(objData.apts);
            GdiApi.PolylineTo(Objects.Handle, pts, objData.cpts);

            if (Objects.IsOpenPath)
            {
                Objects.Path.AddLines(newPoints);
            }
            else if (Objects.Pen != null)
            {
                Renderer.DrawLines(Objects.Pen, newPoints);
            }
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="data">Record data.</param>
        /// <param name="bIs32Bit">Indicates if it's 32 or 16 bit version.</param>
        private void PolyPolyline(byte[] data, bool bIs32Bit)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            // NOTE: Marshaling data to EMR_POLYPOLYLINE structure was unabled.
            // So we get retrieve data manually.

            int index = 0;
            int step = IntSize;

            RECT bounds = ReadRECT(data, ref index);
            int numPolys = BitConverter.ToInt32(data, index);
            index += step;
            int numPoints = BitConverter.ToInt32(data, index);
            index += step;

            int[] numPointsInPoly = ReadInt32Array(data, numPolys, ref index);
            Point[] polyPoints = ReadPointArray(data, numPoints, ref index, bIs32Bit);
            int pointIndex = 0;
            GraphicsPath path = new GraphicsPath(Objects.FillMode);

            for (int i = 0; i < numPolys; i++)
            {
                int polyCount = numPointsInPoly[i];
                PointF[] polygon = new PointF[polyCount];
                int polyPolyIndex = 0;

                for (int j = pointIndex, len = pointIndex + polyCount; j < len; j++)
                {
                    PointF point = polyPoints[j];

                    polygon[polyPolyIndex] = point;
                    polyPolyIndex++;
                }

                pointIndex += polyPolyIndex;

                if (Objects.Pen != null)
                {
                    path.AddLines(polygon);
                    if (polygon[0] == polygon[polygon.Length - 1])
                    {
                        path.CloseFigure();
                    }
                }
            }

            if (Objects.IsOpenPath)
            {
                // NOTE: it needs to be checked if last parameter has to be true or false.
                Objects.Path.AddPath(path, false);
            }
            else if (Objects.Pen != null)
            {
                Renderer.DrawPath(Objects.Pen, path);
                path.Dispose();
            }
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">The PTR.</param>
        private void OffsetClipRgn(IntPtr ptr)
        {
            Type type = typeof(EMR_OFFSETCLIPRGN);
            EMR_OFFSETCLIPRGN recordData = (EMR_OFFSETCLIPRGN)GetStructure(ptr, type);

            float x = LPtoDPX(recordData.ptlOffset.x);
            float y = LPtoDPY(recordData.ptlOffset.y);

            Renderer.TranslateClip(x, y);
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">Pointer to record data.</param>
        private void ExcludeClipRect(IntPtr ptr)
        {
            Type type = typeof(EMR_EXCLUDECLIPRECT);
            EMR_EXCLUDECLIPRECT recordData = (EMR_EXCLUDECLIPRECT)GetStructure(ptr, type);

            RectangleF rect = LPtoDP(recordData.rclClip);

            Renderer.ExcludeClip(System.Drawing.Rectangle.Truncate(rect));
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">Pointer to record data.</param>
        private void IntersectClipRect(IntPtr ptr)
        {
            Type type = typeof(EMR_EXCLUDECLIPRECT);
            EMR_EXCLUDECLIPRECT recordData = (EMR_EXCLUDECLIPRECT)GetStructure(ptr, type);

            RectangleF rect = LPtoDP(recordData.rclClip);

            Renderer.IntersectClip(rect);
            if(rect.X <= 0 && rect.Y <= 0)
            Renderer.ExcludeClip(new Rectangle((int)rect.Left, (int)rect.Top, (int)rect.Width, (int)rect.Height));
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="data">Record data.</param>
        /// <param name="bIs32Bit">Indicates if it's 32 or 16 bit version.</param>
        private void PolyDraw(byte[] data, bool bIs32Bit)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            // NOTE: Marshaling data to EMR_POLYPOLYLINE structure was unabled.
            // So we get retrieve data manually.

            int index = 0;
            int step = IntSize;

            RECT bounds = ReadRECT(data, ref index);
            int numPoints = BitConverter.ToInt32(data, index);
            index += step;

            Point[] polyPoints = ReadPointArray(data, numPoints, ref index, bIs32Bit);

            // Read types of the points.
            int count = Math.Min(polyPoints.Length, data.Length - index);
            PointF curPoint = Objects.CurrentPoint;
            byte[] ptTypes = new byte[count];

            for (int i = index; i < count; i += step)
            {
                PT_POINT_TYPE ptType = (PT_POINT_TYPE)data[i];
                PathPointType pathPtType = PathPointType.Start;

                if ((ptType & PT_POINT_TYPE.PT_CLOSEFIGURE) > 0)
                {
                    pathPtType |= PathPointType.CloseSubpath;
                }

                if ((ptType & PT_POINT_TYPE.PT_LINETO) > 0)
                {
                    pathPtType |= PathPointType.Line;
                }

                if ((ptType & PT_POINT_TYPE.PT_BEZIERTO) > 0)
                {
                    pathPtType |= PathPointType.Bezier3;
                }

                curPoint = polyPoints[i - index];
                ptTypes[i] = (byte)pathPtType;
            }

            GraphicsPath path = new GraphicsPath(polyPoints, ptTypes);
            path.FillMode = Objects.FillMode;

            if (Objects.IsOpenPath)
            {
                // NOTE: it needs to be checked if last parameter has to be true or false.
                Objects.Path.AddPath(path, false);
            }
            else if (Objects.Pen != null)
            {
                Renderer.DrawPath(Objects.Pen, path);
                path.Dispose();
            }
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">Pointer to record data.</param>
        private void SetArcDirection(IntPtr ptr)
        {
            Type type = typeof(EMR_SETARCDIRECTION);
            EMR_SETARCDIRECTION recordData = (EMR_SETARCDIRECTION)GetStructure(ptr, type);

            AD_ANGLEDIRECTION direction = (AD_ANGLEDIRECTION)recordData.iArcDirection;
            Objects.ArcDirection = direction;
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">Pointer to record data.</param>
        private void FlattenPath(IntPtr ptr)
        {
            if (Objects.Path != null)
            {
                Objects.Path.Flatten();
            }
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">Pointer to record data.</param>
        private void WidenPath(IntPtr ptr)
        {
            if (Objects.Path != null && Objects.Pen != null)
            {
                Objects.Path.Widen(Objects.Pen);
            }
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="data">Datra of the record.</param>
        /// <param name="ptr">Pointer to record data.</param>
        private void FillRgn(byte[] data, IntPtr ptr)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Type type = typeof(EMR_FILLRGN);
            EMR_FILLRGN recordData = (EMR_FILLRGN)GetStructure(ptr, type);
            int strSize = Marshal.SizeOf(type);

            int numRects = recordData.RgnData.rdh.nCount;

            // Read region rectanges, they are at the and of the data.
            int index = strSize;
            int length = Math.Min(numRects * IntSize * RectNumber,
                data.Length - index);
            int step = Marshal.SizeOf(typeof(RECT));

            Region region = new Region(System.Drawing.Rectangle.Empty);

            for (int i = 0; i < length; i += step)
            {
                RectangleF rect = ReadRECT(data, ref index);

                // NOTE: it needs to be tested if data are in logical units or in device units.
                rect = LPtoDP(rect);
                region.Union(rect);
            }

            // Retrieve brush for region fill.
            int brushIndex = recordData.ihBrush;
            object brObj = Objects.SelectedObjects.CreatedGraphicObjects[brushIndex];
            Brush brush = brObj as Brush;

            if (brush != null)
            {
                Renderer.FillRegion(brush, region);
            }

            region.Dispose();
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="data">Data of the record.</param>
        /// <param name="ptr">Pointer to record data.</param>
        private void PaintRgn(byte[] data, IntPtr ptr)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Type type = typeof(EMR_INVERTRGN);
            EMR_INVERTRGN recordData = (EMR_INVERTRGN)GetStructure(ptr, type);
            int strSize = Marshal.SizeOf(type);

            int numRects = recordData.RgnData.rdh.nCount;

            // Read region rectanges, they are at the and of the data.
            int index = strSize;
            int length = Math.Min(numRects * IntSize * RectNumber,
                data.Length - index);
            int step = Marshal.SizeOf(typeof(RECT));

            Region region = new Region(System.Drawing.Rectangle.Empty);

            for (int i = 0; i < length; i += step)
            {
                RectangleF rect = ReadRECT(data, ref index);

                // NOTE: it needs to be tested if data are in logical units or in device units.
                rect = LPtoDP(rect);
                region.Union(rect);
            }

            if (Objects.Brush != null)
            {
                Renderer.FillRegion(Objects.Brush, region);
            }

            region.Dispose();
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="data">Datra of the record.</param>
        /// <param name="ptr">Pointer to record data.</param>
        private void ExtSelectClipRgn(byte[] data, IntPtr ptr)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Type type = typeof(EMR_EXTSELECTCLIPRGN);
            EMR_EXTSELECTCLIPRGN recordData = (EMR_EXTSELECTCLIPRGN)GetStructure(ptr, type);
            int strSize = Marshal.SizeOf(type);

            if (recordData.cbRgnData > 0)
            {
                int numRects = recordData.RgnData.rdh.nCount;

                // Read region rectanges, they are at the and of the data.
                int index = strSize;
                int length = Math.Min(numRects * IntSize * RectNumber,
                    data.Length - index);
                int step = Marshal.SizeOf(typeof(RECT));

                Region region = new Region(System.Drawing.Rectangle.Empty);

                for (int i = 0; i < length; i += step)
                {
                    RectangleF rect = ReadRECT(data, ref index);

                    // NOTE: it needs to be tested if data are in logical units or in device units.
                    // Probably already in device units.
                    //rect = LPtoDP( rect );
                    switch ((COMBINE_RGN)recordData.iMode)
                    {
                        case COMBINE_RGN.RGN_AND:
                            region.Intersect(rect);
                            break;
                        case COMBINE_RGN.RGN_OR:
                            region.Union(rect);
                            break;
                        case COMBINE_RGN.RGN_XOR:
                            region.Xor(rect);
                            break;
                        case COMBINE_RGN.RGN_DIFF:
                            region.Exclude(rect);
                            break;
                        case COMBINE_RGN.RGN_COPY:
                            region = new Region(rect);
                            break;
                    }
                }

                COMBINE_RGN iMode = (COMBINE_RGN)recordData.iMode;
                int icMode = (iMode == COMBINE_RGN.RGN_MAX) ?
                    (int)COMBINE_RGN.RGN_MIN - 1 : (int)iMode;

                CombineMode mode = (CombineMode)icMode;

                Renderer.SetClip(region, mode);
                region.Dispose();
            }
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">Pointer to record data.</param>
        private void SetBkMode(IntPtr ptr)
        {
            Type type = typeof(EMR_SELECTCLIPPATH);
            EMR_SELECTCLIPPATH recordData = (EMR_SELECTCLIPPATH)GetStructure(ptr, type);

            GdiApi.SetBkMode(Objects.Handle, recordData.iMode);
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">Pointer to record data.</param>
        private void SetTextAlign(IntPtr ptr)
        {
            Type type = typeof(EMR_SELECTCLIPPATH);
            EMR_SELECTCLIPPATH recordData = (EMR_SELECTCLIPPATH)GetStructure(ptr, type);

            TA_TEXT_ALIGN textAlign = (TA_TEXT_ALIGN)recordData.iMode;

            Objects.TextAlign = textAlign;
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">Pointer to record data.</param>
        private void SetTextColor(IntPtr ptr)
        {
            Type type = typeof(EMR_SETTEXTCOLOR);
            EMR_SETTEXTCOLOR recordData = (EMR_SETTEXTCOLOR)GetStructure(ptr, type);

            Color textColor = ColorTranslator.FromWin32(recordData.crColor);
            Objects.ForeColor = textColor;
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">Pointer to record data.</param>
        private void SetBkColor(IntPtr ptr)
        {
            Type type = typeof(EMR_SETTEXTCOLOR);
            EMR_SETTEXTCOLOR recordData = (EMR_SETTEXTCOLOR)GetStructure(ptr, type);

            Color bgColor = ColorTranslator.FromWin32(recordData.crColor);
            Objects.BackColor = bgColor;
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">Pointer to record data.</param>
        private void SetWorldTransform(IntPtr ptr)
        {
            Type type = typeof(EMR_SETWORLDTRANSFORM);
            EMR_SETWORLDTRANSFORM recordData = (EMR_SETWORLDTRANSFORM)GetStructure(ptr, type);

            // Set proper graphics mode.
            SetValidGraphicsMode();

            bool result = GdiApi.SetWorldTransform(Objects.Handle, ref recordData.xform);

            MetafileParser.CheckResult(result);
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">Pointer to record data.</param>
        private void CreatePen(IntPtr ptr)
        {
            Type type = typeof(EMR_CREATEPEN);
            EMR_CREATEPEN recordData = (EMR_CREATEPEN)GetStructure(ptr, type);

            int iColor = recordData.lopn.lopnColor;
            Color penColor = ColorTranslator.FromWin32(iColor);
            Pen pen = new Pen(penColor);

            Point penSize = recordData.lopn.lopnWidth;

            // pen is 1 pixel width.
            if (penSize.IsEmpty)
            {
                pen.Width = 1;
            }
            else
            {
                // Pen width is in logical units.
                pen.Width = LPtoDPWidth(penSize.X);
            }

            PS_PEN_STYLE penStyle = (PS_PEN_STYLE)recordData.lopn.lopnStyle;

            if (penStyle != PS_PEN_STYLE.PS_INSIDEFRAME)
            {
                System.Drawing.Drawing2D.DashStyle style =
                    (System.Drawing.Drawing2D.DashStyle)recordData.lopn.lopnStyle;
                pen.DashStyle = style;
            }

            int handleId = recordData.ihPen;
            Objects.SelectedObjects.AddObject(pen, handleId);
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">Pointer to record data.</param>
        private void AngleArc(IntPtr ptr)
        {
            Type type = typeof(EMR_ANGLEARC);
            EMR_ANGLEARC recordData = (EMR_ANGLEARC)GetStructure(ptr, type);

            int radius = recordData.nRadius;

            RectangleF bounds = RectangleF.Empty;
            bounds.X = recordData.ptlCenter.x - radius;
            bounds.Y = recordData.ptlCenter.y - radius;

            bounds.Width = 2 * radius;
            bounds.Height = 2 * radius;


            PointF curPoint = Objects.CurrentPoint;

            // Define start point of the arc.
            // This will move current point to start point of the arc.
            bool result = GdiApi.AngleArc(Objects.Handle, recordData.ptlCenter.x, recordData.ptlCenter.y,
                recordData.nRadius, recordData.eStartAngle, recordData.eStartAngle);

            MetafileParser.CheckResult(result);

            PointF startPoint = Objects.CurrentPoint;

            float newStartAngle = -recordData.eStartAngle;
            float newSweepAngle = -recordData.eSweepAngle;

            // Convert to device coordinates.
            bounds = LPtoDP(bounds);
            curPoint = LPtoDP(curPoint);
            startPoint = LPtoDP(startPoint);

            if (Objects.IsOpenPath)
            {
                Objects.Path.AddLine(curPoint, startPoint);
                Objects.Path.AddArc(bounds, newStartAngle, newSweepAngle);
            }
            else
            {
                if (Objects.Pen != null)
                {
                    // Draw line.
                    PointF[] points = new PointF[] { curPoint, startPoint };
                    Renderer.DrawLines(Objects.Pen, points);

                    // Draw arc.
                    Renderer.DrawArc(Objects.Pen, bounds,
                        newStartAngle, newSweepAngle);
                }
            }

            // Move current point.
            GdiApi.AngleArc(Objects.Handle, recordData.ptlCenter.x,
                recordData.ptlCenter.y, radius, recordData.eStartAngle, recordData.eSweepAngle);
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">Pointer to record data.</param>
        private void Ellipse(IntPtr ptr)
        {
            Type type = typeof(EMR_RECTANGLE);
            EMR_RECTANGLE recordData = (EMR_RECTANGLE)GetStructure(ptr, type);

            RectangleF bounds = LPtoDP(recordData.rclBox);

            if (Objects.IsOpenPath)
            {
                Objects.Path.AddEllipse(bounds);
            }
            else
            {
                if (Objects.Brush != null)
                {
                    Renderer.FillEllipse(Objects.Brush, bounds);
                }

                if (Objects.Pen != null)
                {
                    Renderer.DrawEllipse(Objects.Pen, bounds);
                }
            }
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">Pointer to record data.</param>
        private void RectangleEx(IntPtr ptr)
        {
            Type type = typeof(EMR_RECTANGLE);
            EMR_RECTANGLE recordData = (EMR_RECTANGLE)GetStructure(ptr, type);

            RectangleF bounds = LPtoDP(recordData.rclBox);
            RectangleF[] rects = new RectangleF[] { bounds };
          
            if (Objects.IsOpenPath)
            {
                Objects.Path.AddRectangle(bounds);
            }
            else
            {
                if (Objects.Brush != null)
                {
                    Renderer.FillRectangles(Objects.Brush, rects);
                  
                }

                if (Objects.Pen != null && Objects.Pen.Color.A != 0)
                {
                  
                       Renderer.DrawRectangles(Objects.Pen, rects);
                   
                }
            }
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">Pointer to record data.</param>
        private void RoundRect(IntPtr ptr)
        {
            Type type = typeof(EMR_ROUNDRECT);
            EMR_ROUNDRECT recordData = (EMR_ROUNDRECT)GetStructure(ptr, type);

            float halfWidth = (float)recordData.szlCorner.cx / 2.0f;
            float halfHeight = (float)recordData.szlCorner.cy / 2.0f;

            RectangleF rect = recordData.rclBox;
            GraphicsPath path = new GraphicsPath();

            // Convert coordinates.
            halfWidth = LPtoDPWidth(halfWidth);
            halfHeight = LPtoDPHeight(halfHeight);
            rect = LPtoDP(rect);

            // Left line.
            float x1 = rect.X;
            float y1 = rect.Bottom - halfHeight;
            float x2 = x1;
            float y2 = rect.Y + halfHeight;
            path.AddLine(x1, y1, x2, y2);

            // Add left top angle.
            float startAngle = 180;
            float sweepAngle = 90;
            RectangleF arcBounds = RectangleF.Empty;
            arcBounds.Location = rect.Location;
            arcBounds.Width = LPtoDPWidth(recordData.szlCorner.cx);
            arcBounds.Height = LPtoDPHeight(recordData.szlCorner.cy);
            path.AddArc(arcBounds, startAngle, sweepAngle);


            // Top line.
            x1 = rect.X + halfWidth;
            y1 = rect.Y;
            x2 = rect.Right - halfWidth;
            y2 = y1;
            path.AddLine(x1, y1, x2, y2);


            // Add right top angle.
            startAngle = -90;
            sweepAngle = 90;
            arcBounds.X = rect.Right - arcBounds.Width;
            arcBounds.Y = rect.Y;
            path.AddArc(arcBounds, startAngle, sweepAngle);


            // Right line.
            x1 = rect.Right;
            y1 = rect.Y + halfHeight;
            x2 = x1;
            y2 = rect.Bottom - halfHeight;
            path.AddLine(x1, y1, x2, y2);

            // Add bottom right angle.
            startAngle = 0.0f;
            sweepAngle = 90;
            arcBounds.X = rect.Right - arcBounds.Width;
            arcBounds.Y = rect.Bottom - arcBounds.Height;
            path.AddArc(arcBounds, startAngle, sweepAngle);


            // Bottom line.
            x1 = rect.Right - halfWidth;
            y1 = rect.Bottom;
            x2 = rect.X + halfWidth;
            y2 = y1;
            path.AddLine(x1, y1, x2, y2);


            // Add bottom left point.
            startAngle = 90.0f;
            sweepAngle = 90;
            arcBounds.X = rect.X;
            arcBounds.Y = rect.Bottom - arcBounds.Height;
            path.AddArc(arcBounds, startAngle, sweepAngle);

            if (Objects.IsOpenPath)
            {
                Objects.Path.AddPath(path, false);
            }
            else
            {
                if (Objects.Brush != null)
                {
                    Renderer.FillPath(Objects.Brush, path);
                }

                if (Objects.Pen != null)
                {
                    Renderer.DrawPath(Objects.Pen, path);
                }
            }

            path.Dispose();
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">Pointer to record data.</param>
        private void Chord(IntPtr ptr)
        {
            Type type = typeof(EMR_ARC);
            EMR_ARC recordData = (EMR_ARC)GetStructure(ptr, type);

            RectangleF bounds = LPtoDP(recordData.rclBox);
            float xCenter = (float)(bounds.Left + bounds.Right) / 2.0f;
            float yCenter = (float)(bounds.Top + bounds.Bottom) / 2.0f;

            PointF start = LPtoDP(recordData.ptlStart);
            PointF end = LPtoDP(recordData.ptlEnd);

            // These angles are already in GDI+ directions.
            float startAngle = GetAngle(xCenter, yCenter, start.X, start.Y);
            float sweepAngle = GetAngle(xCenter, yCenter, end.X, end.Y);

            sweepAngle -= startAngle;
            sweepAngle = (sweepAngle < 0) ? sweepAngle : sweepAngle - 2 * DegreeCount;

            GraphicsPath path = new GraphicsPath();
            path.AddArc(bounds, startAngle, sweepAngle);
            path.CloseFigure();

            if (Objects.IsOpenPath)
            {
                Objects.Path.AddPath(path, false);
            }
            else
            {
                if (Objects.Brush != null)
                {
                    Renderer.FillPath(Objects.Brush, path);
                }

                if (Objects.Pen != null)
                {
                    Renderer.DrawPath(Objects.Pen, path);
                }
            }

            path.Dispose();
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">Pointer to record data.</param>
        private void Pie(IntPtr ptr)
        {
            Type type = typeof(EMR_ARC);
            EMR_ARC recordData = (EMR_ARC)GetStructure(ptr, type);

            System.Drawing.Rectangle bounds = System.Drawing.Rectangle.Truncate(LPtoDP(recordData.rclBox));
            float xCenter = (float)(bounds.Left + bounds.Right) / 2.0f;
            float yCenter = (float)(bounds.Top + bounds.Bottom) / 2.0f;

            PointF start = LPtoDP(recordData.ptlStart);
            PointF end = LPtoDP(recordData.ptlEnd);

            // These angles are already in GDI+ directions.
            float startAngle = GetAngle(xCenter, yCenter, start.X, start.Y);
            float sweepAngle = GetAngle(xCenter, yCenter, end.X, end.Y);

            sweepAngle -= startAngle;
            sweepAngle = (sweepAngle < 0) ? sweepAngle : sweepAngle - 2 * DegreeCount;

            if (Objects.IsOpenPath)
            {
                Objects.Path.AddPie(bounds, startAngle, sweepAngle);
            }
            else
            {
                if (Objects.Brush != null)
                {
                    Renderer.FillPie(Objects.Brush, bounds.X, bounds.Y,
                        bounds.Width, bounds.Height, startAngle, sweepAngle);
                }

                if (Objects.Pen != null)
                {
                    Renderer.DrawPie(Objects.Pen, bounds, startAngle, sweepAngle);
                }
            }
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">Pointer to record data.</param>
        /// <param name="bIsArcTo">if b is arc, set to <c>true</c>.</param>
        private void ArcTo(IntPtr ptr, bool bIsArcTo)
        {
            Type type = typeof(EMR_ARC);
            EMR_ARC recordData = (EMR_ARC)GetStructure(ptr, type);

            RectangleF bounds = LPtoDP(recordData.rclBox);
            float xCenter = (float)(bounds.Left + bounds.Right) / 2.0f;
            float yCenter = (float)(bounds.Top + bounds.Bottom) / 2.0f;

            PointF start = LPtoDP(recordData.ptlStart);
            PointF end = LPtoDP(recordData.ptlEnd);

            // These angles are already in GDI+ directions.
            float startAngle = GetAngle(xCenter, yCenter, start.X, start.Y);
            float sweepAngle = GetAngle(xCenter, yCenter, end.X, end.Y);

            sweepAngle -= startAngle;
            sweepAngle = (sweepAngle < 0) ? sweepAngle : sweepAngle - 2 * DegreeCount;

            PointF startPoint = PointF.Empty;
            PointF endPoint = PointF.Empty;

            if (bIsArcTo)
            {
                startPoint = LPtoDP(Objects.CurrentPoint);
                endPoint = GetStartPoint(recordData.rclBox, recordData.ptlStart);
                endPoint = LPtoDP(endPoint);
            }

            if (Objects.IsOpenPath)
            {
                if (bIsArcTo)
                {
                    Objects.Path.AddLine(startPoint, endPoint);
                }

                Objects.Path.AddArc(bounds, startAngle, sweepAngle);
            }
            else
            {
                if (Objects.Pen != null)
                {
                    if (bIsArcTo)
                    {
                        PointF[] points = new PointF[] { startPoint, endPoint };
                        Renderer.DrawLines(Objects.Pen, points);
                    }
                    Renderer.DrawArc(Objects.Pen, bounds, startAngle, sweepAngle);
                }
            }

            // Move current point.
            Point fRad = recordData.ptlStart;
            Point sRad = recordData.ptlEnd;

            if (bIsArcTo)
            {
                GdiApi.ArcTo(Objects.Handle, recordData.rclBox.left, recordData.rclBox.top,
                    recordData.rclBox.right, recordData.rclBox.bottom,
                    fRad.X, fRad.Y, sRad.X, sRad.Y);
            }
            else
            {
                GdiApi.Arc(Objects.Handle, recordData.rclBox.left, recordData.rclBox.top,
                    recordData.rclBox.right, recordData.rclBox.bottom,
                    fRad.X, fRad.Y, sRad.X, sRad.Y);
            }
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">Pointer to record data.</param>
        private void CloseFigure(IntPtr ptr)
        {
            if (Objects.IsOpenPath)
            {
                Objects.Path.CloseFigure();

                GdiApi.CloseFigure(Objects.Handle);
            }
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">Pointer to record data.</param>
        private void FillPath(IntPtr ptr)
        {
            Type type = typeof(EMR_FILLPATH);
            EMR_FILLPATH recordData = (EMR_FILLPATH)GetStructure(ptr, type);

            if (Objects.Path != null && !Objects.IsOpenPath)
            {
                Objects.Path.CloseAllFigures();

                if (Objects.Brush != null)
                {
                    Objects.Path.FillMode = Objects.FillMode;
                    Renderer.FillPath(Objects.Brush, Objects.Path);
                }
            }

            GdiApi.FillPath(Objects.Handle);
            Objects.Path = null;
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">Pointer to record data.</param>
        private void StrokeAndFillPath(IntPtr ptr)
        {
            Type type = typeof(EMR_FILLPATH);
            EMR_FILLPATH recordData = (EMR_FILLPATH)GetStructure(ptr, type);

            if (Objects.Path != null && !Objects.IsOpenPath)
            {
                Objects.Path.CloseAllFigures();
                Objects.Path.FillMode = Objects.FillMode;

                if (Objects.Brush != null)
                {
                    Renderer.FillPath(Objects.Brush, Objects.Path);
                }

                if (Objects.Pen != null)
                {
                    Renderer.DrawPath(Objects.Pen, Objects.Path);
                }
            }

            GdiApi.StrokeAndFillPath(Objects.Handle);
            Objects.Path = null;
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">Pointer to record data.</param>
        private void StrokePath(IntPtr ptr)
        {
            Type type = typeof(EMR_FILLPATH);
            EMR_FILLPATH recordData = (EMR_FILLPATH)GetStructure(ptr, type);

            if (Objects.Path != null && !Objects.IsOpenPath)
            {
                //Objects.Path.CloseAllFigures();

                if (Objects.Pen != null)
                {
                    if (recordData.rclBounds.bottom >= 0)
                    {
                        Objects.Path.FillMode = Objects.FillMode;
                        Renderer.DrawPath(Objects.Pen, Objects.Path);
                    }
                }
            }

            GdiApi.StrokePath(Objects.Handle);
            Objects.Path = null;
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">Pointer to record data.</param>
        private void StretchDIBits(IntPtr ptr)
        {
            Type type = typeof(EMR_STRETCHDIBITS);
            EMR_STRETCHDIBITS recordData = (EMR_STRETCHDIBITS)GetStructure(ptr, type);

            int bitmapInfoOffset = recordData.offBmiSrc;

            // NOTE: shift 8 bytes because EMR structure is not included here.
            bitmapInfoOffset -= (2 * IntSize);

            IntPtr bitmapInfoPtr = new IntPtr(ptr.ToInt64() + bitmapInfoOffset);

            type = typeof(BITMAPINFOHEADER);
            BITMAPINFOHEADER bitmapHeader = (BITMAPINFOHEADER)GetStructure(bitmapInfoPtr, type);

            // TODO: finish retrieving image from here.
            int imageOffset = recordData.offBitsSrc;
            uint imgSize = recordData.cbBitsSrc;

            System.Drawing.Rectangle srcRect = new System.Drawing.Rectangle(recordData.xSrc, recordData.ySrc,
                recordData.cxSrc, recordData.cySrc);
            System.Drawing.Rectangle destRect = new System.Drawing.Rectangle(recordData.xDest, recordData.yDest,
                recordData.cxDest, recordData.cyDest);
            // Convert coordinates:
            destRect = System.Drawing.Rectangle.Truncate(LPtoDP(destRect));
            srcRect = System.Drawing.Rectangle.Truncate(LPtoDP(srcRect));

            DrawImage(imageOffset, imgSize, ptr, bitmapInfoPtr, destRect,
                srcRect, (RASTER_CODE)recordData.dwRop, recordData.iUsageSrc);
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">Pointer to record data.</param>
        private void BitBlt(IntPtr ptr)
        {
            Type type = typeof(EMR_BITBLT);
            EMR_BITBLT recordData = (EMR_BITBLT)GetStructure(ptr, type);

            int bitmapInfoOffset = recordData.offBmiSrc;

            // NOTE: shift 8 bytes because EMR structure is not included here.
            bitmapInfoOffset -= (2 * IntSize);

            IntPtr bitmapInfoPtr = new IntPtr(ptr.ToInt64() + bitmapInfoOffset);

            type = typeof(BITMAPINFOHEADER);
            BITMAPINFOHEADER bitmapHeader = (BITMAPINFOHEADER)GetStructure(bitmapInfoPtr, type);

            int imageOffset = recordData.offBitsSrc;
            uint imgSize = recordData.cbBitsSrc;

            System.Drawing.Rectangle srcRect = new System.Drawing.Rectangle(recordData.xSrc, recordData.ySrc,
                recordData.cxDest, recordData.cyDest);
            System.Drawing.Rectangle destRect = new System.Drawing.Rectangle(recordData.xDest, recordData.yDest,
                recordData.cxDest, recordData.cyDest);
            // Convert coordinates:
            destRect = System.Drawing.Rectangle.Truncate(LPtoDP(destRect));
            srcRect = System.Drawing.Rectangle.Truncate(LPtoDP(srcRect));

            DrawImage(imageOffset, imgSize, ptr, bitmapInfoPtr, destRect,
                srcRect, recordData.dwRop, recordData.iUsageSrc);
        }
        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">Pointer to record data.</param>
        private void TransparentBlt(IntPtr ptr)
        {
            Type type = typeof(EMR_TRANSPARENTBLT);
            EMR_TRANSPARENTBLT recordData = (EMR_TRANSPARENTBLT)GetStructure(ptr, type);

            int bitmapInfoOffset = recordData.offBmiSrc;

            // NOTE: shift 8 bytes because EMR structure is not included here.
            bitmapInfoOffset -= (2 * IntSize);

            IntPtr bitmapInfoPtr = new IntPtr(ptr.ToInt64() + bitmapInfoOffset);

            type = typeof(BITMAPINFOHEADER);
            BITMAPINFOHEADER bitmapHeader = (BITMAPINFOHEADER)GetStructure(bitmapInfoPtr, type);

            int imageOffset = recordData.offBitsSrc;
            uint imgSize = recordData.cbBitsSrc;

            System.Drawing.Rectangle srcRect = new System.Drawing.Rectangle(recordData.xSrc, recordData.ySrc,
                recordData.cxDest, recordData.cyDest);
            System.Drawing.Rectangle destRect = new System.Drawing.Rectangle(recordData.xDest, recordData.yDest,
                recordData.cxDest, recordData.cyDest);
            // Convert coordinates:
            destRect = System.Drawing.Rectangle.Truncate(LPtoDP(destRect));
            srcRect = System.Drawing.Rectangle.Truncate(LPtoDP(srcRect));

            DrawTransparentImage(imageOffset, imgSize, ptr, bitmapInfoPtr, destRect,
                srcRect, recordData.iUsageSrc);
        }

        /// <summary>
        /// Implements the final stage of the 'Blt'-family functions.
        /// </summary>
        /// <param name="imageOffset"></param>
        /// <param name="imgSize"></param>
        /// <param name="ptr"></param>
        /// <param name="bitmapInfoPtr"></param>
        /// <param name="destRect"></param>
        /// <param name="srcRect"></param>
        /// <param name="dwRop"></param>
        /// <param name="iUsageSrc"></param>
        private void DrawTransparentImage(int imageOffset, uint imgSize, IntPtr ptr, IntPtr bitmapInfoPtr,
            RectangleF destRect, RectangleF srcRect, int iUsageSrc)
        {
            Bitmap bmp = null;

            if (imageOffset > 0 && imgSize > 0 && destRect.Width > 0 && destRect.Height > 0)
            {
                bmp = GetBitmap(imageOffset, imgSize, ptr, bitmapInfoPtr, iUsageSrc);
            }

            // Store the information about image region at the context.
            if (destRect.Height > 0)
            {
                //Skip the invalid regions.
                if (destRect.Location.Y != 0 && bmp != null)
                {
                    ImageRegion rgn = new ImageRegion(destRect.Location.Y, destRect.Height);
                    ImageRegions.Add(rgn);
                }
            }
            
            //Make the Transparent background
            bmp.MakeTransparent();
            Image img = bmp;
            using (MemoryStream ms = new MemoryStream())
            {
                bmp.Save(ms, ImageFormat.Png);
                ms.Position = 0;
                img = Image.FromStream(ms);
            }

            if (img != null)
                Renderer.DrawImage(img, destRect, srcRect, GraphicsUnit.Pixel);

            if (bmp != null)
                bmp.Dispose();
            if (img != null)
                img.Dispose();
        }
        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">Pointer to record data.</param>
        private void StretchBlt(IntPtr ptr)
        {
            Type type = typeof(EMR_STRETCHBLT);
            EMR_STRETCHBLT recordData = (EMR_STRETCHBLT)GetStructure(ptr, type);

            int bitmapInfoOffset = recordData.offBmiSrc;

            // NOTE: shift 8 bytes because EMR structure is not included here.
            bitmapInfoOffset -= (2 * IntSize);

            IntPtr bitmapInfoPtr = new IntPtr(ptr.ToInt64() + bitmapInfoOffset);

            type = typeof(BITMAPINFOHEADER);
            BITMAPINFOHEADER bitmapHeader = (BITMAPINFOHEADER)GetStructure(bitmapInfoPtr, type);

            // TODO: finish retrieving image from here.
            int imageOffset = recordData.offBitsSrc;
            uint imgSize = recordData.cbBitsSrc;

            System.Drawing.Rectangle srcRect = new System.Drawing.Rectangle(recordData.xSrc, recordData.ySrc,
                recordData.cxSrc, recordData.cySrc);
            System.Drawing.Rectangle destRect = new System.Drawing.Rectangle(recordData.xDest, recordData.yDest,
                recordData.cxDest, recordData.cyDest);
            // Convert coordinates:
            destRect = System.Drawing.Rectangle.Truncate(LPtoDP(destRect));
            srcRect = System.Drawing.Rectangle.Truncate(LPtoDP(srcRect));

            DrawImage(imageOffset, imgSize, ptr, bitmapInfoPtr, destRect,
                srcRect, recordData.dwRop, recordData.iUsageSrc);
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">Pointer to record data.</param>
        private void MaskBlt(IntPtr ptr)
        {
            Type type = typeof(EMR_MASKBLT);
            EMR_MASKBLT recordData = (EMR_MASKBLT)GetStructure(ptr, type);

            int bitmapInfoOffset = recordData.offBmiSrc;
            // NOTE: shift 8 bytes because EMR structure is not included here.
            bitmapInfoOffset -= (2 * IntSize);
            IntPtr bitmapInfoPtr = new IntPtr(ptr.ToInt64() + bitmapInfoOffset);
            int imageOffset = recordData.offBitsSrc;
            uint imgSize = recordData.cbBitsSrc;
            Bitmap srcBitmap = null;

            if (imageOffset > 0 && imgSize > 0)
            {
                srcBitmap = GetBitmap(imageOffset, imgSize, ptr, bitmapInfoPtr, recordData.iUsageSrc);
            }

            bitmapInfoOffset = recordData.offBmiMask;
            // NOTE: shift 8 bytes because EMR structure is not included here.
            bitmapInfoOffset -= (2 * IntSize);
            bitmapInfoPtr = new IntPtr(ptr.ToInt64() + bitmapInfoOffset);

            type = typeof(BITMAPINFOHEADER);
            BITMAPINFOHEADER maskBitmapHeader = (BITMAPINFOHEADER)GetStructure(bitmapInfoPtr, type);

            imageOffset = recordData.offBitsMask;
            imgSize = recordData.cbBitsMask;
            Bitmap mskBitmap = null;
            IntPtr hmskBitmap = IntPtr.Zero;

            if (imageOffset > 0 && imgSize > 0)
            {
                mskBitmap = GetBitmap(imageOffset, imgSize, ptr, bitmapInfoPtr, 1);
            }

            System.Drawing.Rectangle dstRect = new System.Drawing.Rectangle(recordData.xDest, recordData.yDest,
                recordData.cxDest, recordData.cyDest);
            System.Drawing.Rectangle srcRect = new System.Drawing.Rectangle(recordData.xSrc, recordData.ySrc,
                recordData.cxDest, recordData.cyDest);
            System.Drawing.Rectangle mskRect = new System.Drawing.Rectangle(recordData.xMask, recordData.yMask,
                recordData.cxDest, recordData.cyDest);
            dstRect = System.Drawing.Rectangle.Truncate(LPtoDP(dstRect));
            srcRect = System.Drawing.Rectangle.Truncate(LPtoDP(srcRect));
            mskRect = System.Drawing.Rectangle.Truncate(LPtoDP(mskRect));

            // Store information about text region at the context.
            if (dstRect.Height > 0)
            {
                ImageRegion rgn = new ImageRegion(dstRect.Location.Y, dstRect.Height);
                ImageRegions.Add(rgn);
            }

            if (srcBitmap != null)
            {
                //Renderer.DrawImage( srcBitmap, dstRect, srcRect, GraphicsUnit.Pixel );
                Renderer.DrawImage(srcBitmap, Objects.Brush, dstRect, srcRect, (uint)recordData.dwRop);
                srcBitmap.Dispose();

                if (mskBitmap != null)
                {
                    mskBitmap.Dispose();
                }
            }
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">Pointer to record data.</param>
        private void ExtCreateFontIndirect(IntPtr ptr)
        {
            Type type = typeof(EMR_EXTCREATEFONTINDIRECTW);
            EMR_EXTCREATEFONTINDIRECTW recordData = (EMR_EXTCREATEFONTINDIRECTW)GetStructure(ptr, type);

            string faceName = TrimFontName(recordData.elfw.lfFaceName);
            float emHeight = recordData.elfw.lfHeight;

            emHeight = GetFontSize(emHeight);

            if (emHeight <= 0)
                return;

            FontStyle fontStyle = FontStyle.Regular;

            if (recordData.elfw.lfItalic)
            {
                fontStyle |= FontStyle.Italic;
            }

            if (recordData.elfw.lfUnderline)
            {
                fontStyle |= FontStyle.Underline;
            }

            if (recordData.elfw.lfStrikeOut)
            {
                fontStyle |= FontStyle.Strikeout;
            }

            if (recordData.elfw.lfWeight > FW_FONT_WEIGHT.FW_SEMIBOLD)
            {
                fontStyle |= FontStyle.Bold;
            }
            Font font = null;
            try
            {
                if (recordData.elfw.lfCharSet > 1)
                    font = Font.FromLogFont(recordData.elfw);
                else
                {
                    font = new Font(faceName, emHeight, fontStyle, GraphicsUnit.Point, recordData.elfw.lfCharSet);
                }
            }

            catch(ArgumentException ex)
            {
                if (ex.Message == "Font 'Monotype Corsiva' does not support style 'Regular'.")
                    font = new Font(faceName, emHeight, FontStyle.Italic);
            }
            //COMMENT: Added for the issue caused due to usage of default font(MS SansSerif)
            m_selectedFont = font;

            FontEx fontEx = new FontEx(font, recordData.elfw);
            int hIndex = recordData.ihFonts;

            Objects.SelectedObjects.AddObject(fontEx, hIndex);
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">Pointer to record data.</param>
        /// <param name="bIsUnicode">True - if string is unicode, False - otherwise.</param>
        private void ExtTextOut(IntPtr ptr, bool bIsUnicode)
        {
            Type type = typeof(EMR_EXTTEXTOUTA);
            EMR_EXTTEXTOUTA recordData = (EMR_EXTTEXTOUTA)GetStructure(ptr, type);
            Renderer.Graphics.m_isEMF = true;
            RectangleF bounds = recordData.rclBounds;
            PointF logPt = recordData.emrtext.ptlReference;
            if (logPt.Y < 0 && logPt.X >= 0)
                Objects.TextAngle = -Objects.TextAngle;
            PointF location = LPtoDP(logPt);

            int numChars = recordData.emrtext.nChars;
            int offset = recordData.emrtext.offString - 2 * IntSize;

            IntPtr textPtr = new IntPtr(ptr.ToInt64() + offset);

            // Extract string data.
            int numBytes = (bIsUnicode) ? numChars * 2 : numChars;
            byte[] textData = new byte[numBytes];
            Marshal.Copy(textPtr, textData, 0, numBytes);

            int[] width = null;
            float[] widths = null;

            if (recordData.emrtext.offDx > 0)
            {
                offset = recordData.emrtext.offDx - 2 * IntSize;
                IntPtr widthPtr = new IntPtr(ptr.ToInt64() + offset);
                width = new int[numChars];
                Marshal.Copy(widthPtr, width, 0, numChars /* IntSize*/ );

                int length = width.Length;
                widths = new float[length];

                for (int i = 0; i < length; ++i)
                {
                    widths[i] = LPtoDPWidth(width[i]);
                }
            }
            
            ETO options = (ETO)recordData.emrtext.fOptions;
            if ((options & ETO.OPAQUE) == ETO.OPAQUE && (Objects.Brush as SolidBrush).Color.ToArgb() != Objects.BackColor.ToArgb())
            {
                using (Brush brush = new SolidBrush(Objects.BackColor))
                {
                    Renderer.FillRectangles(brush, new RectangleF[] { bounds });
                }
            }
            string text = string.Empty;

            // Convert data to string.
            if (bIsUnicode)
            {
                text = Encoding.Unicode.GetString(textData);
            }
            else
            {
                text = Encoding.UTF8.GetString(textData);
            }

            // Don't proceed working if the text is empty.
            if (text == null || text.Length == 0)
                return;

            //Debug.WriteLine( "text: " + text );
            Font font;
            if (m_selectedFont != null)
                font = m_selectedFont;
            else
                font = Objects.Font;

            OUTLINETEXTMETRIC metric = GetFontMetrix(font);

            PointF loc = location;
            StringFormat format = GetStringFormat(text, metric, ref loc);
            if ((format.FormatFlags & StringFormatFlags.DirectionRightToLeft) != StringFormatFlags.DirectionRightToLeft)
            {
                bounds.Location = loc;
            }

            

            if ((options & (ETO.CLIPPED | ETO.OPAQUE)) != 0)
            {
                RectangleF clipRect = recordData.emrtext.rcl;
                //bounds.Width = LPtoDPWidth( clipRect.Width );
                //bounds.Height = LPtoDPHeight( clipRect.Height );
                format.FormatFlags &= ~StringFormatFlags.NoClip;

                // Fill rectangle by using back color.
                if ((options & ETO.OPAQUE) != 0)
                {
                    using (Brush brush = new SolidBrush(Objects.BackColor))
                    {
                        Renderer.FillRectangles(brush, new RectangleF[] { bounds });
                    }
                }
            }
            else
            {
                format.FormatFlags |= StringFormatFlags.NoClip;
                format.FormatFlags |= StringFormatFlags.NoWrap;
                format.Trimming = StringTrimming.None;
            }

            if ((options & ETO.GLYPH_INDEX) != 0)
            {
                if (font != null)
                {
                    string glyphText = ConvertGlyphIndices(text, font);
                    if (!IsComplexScript(glyphText))
                        text = glyphText;
                }
            }

            if ((options & ETO.RTLREADING) != 0)
            {
                format.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
            }
            // if GLYPH_INDEX is set, it means all language processing is completed.
            else if (((options & ETO.GLYPH_INDEX) != 0) && (format.FormatFlags & StringFormatFlags.DirectionRightToLeft) != 0)
                format.FormatFlags &= ~StringFormatFlags.DirectionRightToLeft;

            //bounds.Y = location.Y;
            string txt = text.Trim();
            if (!String.IsNullOrEmpty(txt))
            {
                DrawText(text, format, bounds, metric, widths);
                // Store information about text region at the context.
                if (bounds.Height > 0)
                {
                    TextRegion rgn = null;
                   if(Renderer.Graphics.m_isBaselineFormat)
                     rgn= new TextRegion(bounds.Location.Y-font.Size, bounds.Height);
                    else
                    rgn=new TextRegion(bounds.Location.Y, bounds.Height);
                    TextRegions.Add(rgn);
                }
                // Check whether we have to update current point.
                if ((Objects.TextAlign & TA_TEXT_ALIGN.TA_UPDATECP) > 0)
                {
                    offset = recordData.emrtext.offDx - 2 * IntSize;
                    IntPtr charPtr = new IntPtr(ptr.ToInt64() + offset);
                    POINT pt = recordData.emrtext.ptlReference;
                    bool result = GdiApi.ExtTextOut(Objects.Handle, pt.x, pt.y, recordData.emrtext.fOptions,
                        ref recordData.emrtext.rcl, text, numChars, charPtr);
                    MetafileParser.CheckResult(result);
                }
            }
            format.Dispose();
        }
        private bool IsComplexScript(string text)
        {
            foreach (char c in text)
            {
                //Tamil, Telugu
                if ((int)c > 2946 && (int)c < 3183)
                {
                    return true;
                }
                //Bengali, Hindi
                if ((int)c > 2304 && (int)c < 2555)
                {
                    return true;
                }
                //Gujarati, Kannada
                if ((int)c > 2689 && (int)c < 2801 || (int)c > 3202 && (int)c < 3314)
                {
                    return true;
                }
            }
            foreach (char c in text)
            {
                if ((int)c != 32)
                    return false;
            }
            return true;
        }
        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">Pointer to record data.</param>
        private void CreateDibPatternBrushPt(IntPtr ptr)
        {
            Type type = typeof(EMR_CREATEDIBPATTERNBRUSHPT);
            EMR_CREATEDIBPATTERNBRUSHPT recordData = (EMR_CREATEDIBPATTERNBRUSHPT)GetStructure(ptr, type);

            int bitmapInfoOffset = recordData.offBmi;

            // NOTE: shift 8 bytes because EMR structure is not included here.
            bitmapInfoOffset -= (2 * IntSize);

            IntPtr bitmapInfoPtr = new IntPtr(ptr.ToInt64() + bitmapInfoOffset);

            type = typeof(BITMAPINFOHEADER);
            BITMAPINFOHEADER bitmapHeader = (BITMAPINFOHEADER)GetStructure(bitmapInfoPtr, type);

            // TODO: finish retrieving image from here.
            int imageOffset = recordData.offBits;
            uint imgSize = recordData.cbBits;

            System.Drawing.Rectangle srcRect = new System.Drawing.Rectangle(0, 0, bitmapHeader.biWidth, Math.Abs(bitmapHeader.biHeight));
            System.Drawing.Rectangle destRect = srcRect;

            Bitmap bmp = GetBitmap(imageOffset, imgSize, ptr, bitmapInfoPtr, recordData.iUsage);

            if (bmp != null)
            {
                TextureBrush brush = new TextureBrush(bmp);

                int objId = recordData.ihBrush;
                Objects.SelectedObjects.AddObject(brush, objId);
            }
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">Pointer to record data.</param>
        private void SetStretchBltMode(IntPtr ptr)
        {
            Type type = typeof(EMR_SELECTCLIPPATH);
            EMR_SELECTCLIPPATH recordData = (EMR_SELECTCLIPPATH)GetStructure(ptr, type);

            int prevMode = GdiApi.SetStretchBltMode(Objects.Handle, recordData.iMode);
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">Pointer to record data.</param>
        private void SetLayout(IntPtr ptr)
        {
            Type type = typeof(EMR_SELECTCLIPPATH);
            EMR_SELECTCLIPPATH recordData = (EMR_SELECTCLIPPATH)GetStructure(ptr, type);

            int prevMode = GdiApi.SetLayout(Objects.Handle, recordData.iMode);
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">Pointer to record data.</param>
        private void SetPixelV(IntPtr ptr)
        {
            Type type = typeof(EMR_SETPIXELV);
            EMR_SETPIXELV recordData = (EMR_SETPIXELV)GetStructure(ptr, type);

            Color color = ColorTranslator.FromWin32(recordData.crColor);
            PointF pt = LPtoDP(recordData.ptlPixel);

            PointF[] points = new PointF[] { pt, pt };

            using (Pen pen = new Pen(color))
            {
                Renderer.DrawLines(pen, points);
            }
        }

        /// <summary>
        /// Process record of metafile.
        /// </summary>
        /// <param name="ptr">Pointer to record data.</param>
        private void SetMetaRgn(IntPtr ptr)
        {
            int result = GdiApi.SetMetaRgn(Objects.Handle);
        }
        #endregion

#region Implementation
        /// <summary>
        /// Converts glyph indices to unicode charachter codes.
        /// </summary>
        /// <param name="text">The glyph indices.</param>
        /// <param name="font">The font.</param>
        /// <returns>The unicode string.</returns>
        private string ConvertGlyphIndices(string text, Font font)
        {
            UnicodeTrueTypeFont pdfFont = new UnicodeTrueTypeFont(font, font.Size, CompositeFontType.Type0);

            (pdfFont as ITrueTypeFont).CreateInternals();

            TtfReader reader = pdfFont.TtfReader;
            string result = string.Empty;

            foreach (char c in text)
            {
                TtfGlyphInfo gi = reader.GetGlyph((int)c);

                result += (char)(gi.CharCode);
            }

            return result;
        }

        /// <summary>
        /// Trims the name of the font.
        /// </summary>
        /// <param name="fontName">Name of the font.</param>
        /// <returns></returns>
        private string TrimFontName(string fontName)
        {
            int length = fontName.IndexOf('(');

            if (length > 0)
            {
                fontName = fontName.Substring(0, length);
                fontName = fontName.Trim();
            }
            return fontName;
        }

        /// <summary>
        /// Makes dump of the data.
        /// </summary>
        /// <param name="data">Data array.</param>
        /// <param name="type">The type.</param>
        private void DumpData(byte[] data, EmfPlusRecordType type)
        {
            string whitespace = " ";

            Debug.WriteLine(type.ToString() + " | Size: " + data.Length);

            for (int i = 0; i < data.Length; i++)
            {
                Debug.Write(data[i].ToString() + whitespace);
            }

            Debug.WriteLine("");
        }

        /// <summary>
        /// Gets structure with data from Record data.
        /// </summary>
        /// <param name="ptr">The PTR.</param>
        /// <param name="type">The type.</param>
        /// <returns>Structure from the data.</returns>
        private ValueType GetStructure(IntPtr ptr, Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return (ValueType)Marshal.PtrToStructure(ptr, type);
        }

        /// <summary>
        /// Gets structure with data from Record data.
        /// </summary>
        /// <param name="ptr">Record data.</param>
        /// <param name="structure">Sample structure object.</param>
        /// <returns>Structure with data from IntPtr.</returns>
        /// <remarks>This method marshals structures with arrays of unknown length.</remarks>
        private ValueType GetStructureEx(IntPtr ptr, ValueType structure)
        {
            object objReference = (object)structure;
            Type type = structure.GetType();
            IntPtr pointer = new IntPtr(ptr.ToInt64());
            FieldInfo[] fields = type.GetFields();
            uint numElm = 0;

            for (int i = 0, len = fields.Length; i < len; i++)
            {
                FieldInfo field = fields[i];
                Type fieldType = field.FieldType;

                if (!fieldType.IsArray)
                {
                    object val = Marshal.PtrToStructure(pointer, fieldType);

                    field.SetValue(objReference, val,
                        BindingFlags.Public, null, CultureInfo.InvariantCulture);

                    if (fieldType == typeof(UInt32))
                    {
                        numElm = (uint)val;
                    }

                    int increment = Marshal.SizeOf(fieldType);
                    pointer = new IntPtr(pointer.ToInt64() + increment);
                }
                else
                {
                    Type elementType = fieldType.GetElementType();
                    int elementSize = Marshal.SizeOf(elementType);
                    ArrayList elements = new ArrayList();

                    for (int j = 0; j < numElm; j++)
                    {
                        object elmVal = null;
                        elmVal = Marshal.PtrToStructure(pointer, elementType);
                        elements.Add(elmVal);
                        pointer = new IntPtr(pointer.ToInt64() + elementSize);
                    }

                    Array data = elements.ToArray(elementType);
                    field.SetValue(objReference, data,
                        BindingFlags.Instance, null, CultureInfo.InvariantCulture);
                }
            }

            structure = (ValueType)objReference;

            return structure;
        }

        /// <summary>
        /// Converts logical point to device point.
        /// </summary>
        /// <param name="point">Logical point to be converted.</param>
        /// <returns>Converted point to device point.</returns>
        private PointF LPtoDP(PointF point)
        {
            Point result = Point.Truncate(point);
            POINT[] data = new POINT[] { result };
            GdiApi.LPtoDP(Objects.Handle, data, 1);
            result = data[0];

            return result;
        }

        /// <summary>
        /// Converts rectangle from logic units to device units.
        /// </summary>
        /// <param name="rc">System.Drawing.Rectangle object.</param>
        /// <returns>Converted rectangle.</returns>
        public RectangleF LPtoDP(RectangleF rc)
        {
            RectangleF rect = RectangleF.Empty;
            rect.Location = LPtoDP(rc.Location);
            rect.Width = LPtoDPWidth(rc.Width);
            rect.Height = LPtoDPHeight(rc.Height);

            return rect;
        }

        /// <summary>
        /// Converts logical point to device point.
        /// </summary>
        /// <param name="x">Logical point to be converted.</param>
        /// <returns>Converted point to device point.</returns>
        private float LPtoDPX(float x)
        {
            Point result = new Point((int)x, (int)x);
            POINT[] data = new POINT[] { result };
            GdiApi.LPtoDP(Objects.Handle, data, 1);
            result = data[0];

            return result.X;
        }

        /// <summary>
        /// Converts logical point to device point.
        /// </summary>
        /// <param name="y">Logical point to be converted.</param>
        /// <returns>Converted point to device point.</returns>
        private float LPtoDPY(float y)
        {
            Point result = new Point((int)y, (int)y);
            POINT[] data = new POINT[] { result };
            GdiApi.LPtoDP(Objects.Handle, data, 1);
            result = data[0];

            return result.Y;
        }

        /// <summary>
        /// Converts logic value to device value;
        /// </summary>
        /// <param name="logicValue">Value in logic coordinates.</param>
        /// <returns></returns>
        private float LPtoDPWidth(float logicValue)
        {
            PointF p = new PointF(logicValue, logicValue);

            PointF result = LPtoDP(p);
            PointF emptyRes = LPtoDP(PointF.Empty);
            float res = result.X - emptyRes.X;

            return res;
        }

        /// <summary>
        /// Converts logic value to device value;
        /// </summary>
        /// <param name="logicValue">Value in logic coordinates.</param>
        /// <returns></returns>
        private float LPtoDPHeight(float logicValue)
        {
            PointF p = new PointF(logicValue, logicValue);

            PointF result = LPtoDP(p);
            PointF emptyRes = LPtoDP(PointF.Empty);
            float res = result.Y - emptyRes.Y;

            return res;
        }

        /// <summary>
        /// Converts device point to logical point.
        /// </summary>
        /// <param name="point">Device point to be converted.</param>
        /// <returns>Converted point to logical point.</returns>
        private PointF DPtoLP(PointF point)
        {
            Point result = Point.Truncate(point);
            POINT[] data = new POINT[] { result };
            GdiApi.DPtoLP(Objects.Handle, data, 1);
            result = data[0];

            return result;
        }

        /// <summary>
        /// Converts device value to logical value;
        /// </summary>
        /// <param name="deviceValue">Value in device coordinates.</param>
        /// <returns></returns>
        private float DPtoLPWidth(float deviceValue)
        {
            PointF p = new PointF(deviceValue, deviceValue);

            PointF result = DPtoLP(p);
            PointF emptyRes = DPtoLP(PointF.Empty);
            float res = result.X - emptyRes.X;

            return res;
        }

        /// <summary>
        /// Converts device value to logical value;
        /// </summary>
        /// <param name="deviceValue">Value in device coordinates.</param>
        /// <returns></returns>
        private float DPtoLPHeight(float deviceValue)
        {
            PointF p = new PointF(deviceValue, deviceValue);

            PointF result = DPtoLP(p);
            PointF emptyRes = DPtoLP(PointF.Empty);
            float res = result.Y - emptyRes.Y;

            return res;
        }

        /// <summary>
        /// Converts device System.Drawing.Rectangle to logical value;
        /// </summary>
        /// <param name="rc">The rectangle.</param>
        /// <returns>Converted rectangle</returns>
        private RectangleF DPtoLP(RectangleF rc)
        {
            RectangleF rect = RectangleF.Empty;
            rect.Location = DPtoLP(rc.Location);
            rect.Width = DPtoLPWidth(rc.Width);
            rect.Height = DPtoLPHeight(rc.Height);

            return rect;
        }

        /// <summary>
        /// Reads RECT structure.
        /// </summary>
        /// <param name="data">Data array.</param>
        /// <param name="index">Current index.</param>
        /// <returns>RECT structure.</returns>
        private RECT ReadRECT(byte[] data, ref int index)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            int step = IntSize;
            int left = BitConverter.ToInt32(data, index);
            index += step;

            int top = BitConverter.ToInt32(data, index);
            index += step;

            int right = BitConverter.ToInt32(data, index);
            index += step;

            int bottom = BitConverter.ToInt32(data, index);
            index += step;

            RECT rect = new RECT(left, top, right, bottom);

            return rect;
        }

        /// <summary>
        /// Reads array from the record.
        /// </summary>
        /// <param name="data">Record data.</param>
        /// <param name="dataSize">Size of the array.</param>
        /// <param name="index">Current index.</param>
        /// <returns>Array of data.</returns>
        private int[] ReadInt32Array(byte[] data, int dataSize, ref int index)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            int[] result = new int[dataSize];

            for (int i = 0; i < dataSize; i++)
            {
                int number = BitConverter.ToInt32(data, index);
                index += IntSize;
                result[i] = number;
            }

            return result;
        }

        /// <summary>
        /// Creates array of points.
        /// </summary>
        /// <param name="data">Record data.</param>
        /// <param name="dataSize">Size of the array.</param>
        /// <param name="index">Current index.</param>
        /// <param name="bIs32bit">If true - reads Int32 numbers, otherwise reads Short numbers.</param>
        /// <returns>Array of data.</returns>
        private Point[] ReadPointArray(byte[] data, int dataSize, ref int index, bool bIs32bit)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Point[] result = new Point[dataSize];
            Point point;

            for (int i = 0; i < dataSize; i++)
            {
                int x = ReadInteger(data, ref index, bIs32bit);
                int y = ReadInteger(data, ref index, bIs32bit);
                point = new Point(x, y);
                if (bIs32bit)
                {
                    
                        result[i] = Point.Truncate(LPtoDP(point));
                 
                }
                else
                    result[i] = Point.Truncate(LPtoDP(point));
                
            }

            return result;
        }

        /// <summary>
        /// Reads number.
        /// </summary>
        /// <param name="data">Data array.</param>
        /// <param name="index">Current index.</param>
        /// <param name="bIs32">If true - reads Int32 numbers, otherwise reads Short numbers.</param>
        /// <returns>Number from the data.</returns>
        private int ReadInteger(byte[] data, ref int index, bool bIs32)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            int result = (bIs32) ? BitConverter.ToInt32(data, index) :
                BitConverter.ToInt16(data, index);
            index += (bIs32) ? IntSize : ShortSize;

            return result;
        }

        /// <summary>
        /// Sets proper graphics mode.
        /// </summary>
        private void SetValidGraphicsMode()
        {
            int curMode = GdiApi.GetGraphicsMode(Objects.Handle);

            if (curMode > 0)
            {
                GM_GraphicsMode mode = (GM_GraphicsMode)curMode;

                if (mode != GM_GraphicsMode.GM_ADVANCED)
                {
                    int result = GdiApi.SetGraphicsMode(Objects.Handle, (int)GM_GraphicsMode.GM_ADVANCED);
                }
            }
        }

        /// <summary>
        /// Converts points from GDI poiont type to PointF.
        /// </summary>
        /// <param name="points">Array of points.</param>
        /// <returns>Converted array.</returns>
        private PointF[] ConvertType(POINTS[] points)
        {
            if (points == null)
                throw new ArgumentNullException("points");

            PointF[] result = new PointF[points.Length];

            for (int i = 0, len = points.Length; i < len; i++)
            {
                result[i] = LPtoDP(points[i]);
            }

            return result;
        }

        /// <summary>
        /// Converts points from GDI poiont type to PointF.
        /// </summary>
        /// <param name="points">Array of points.</param>
        /// <returns>Converted array.</returns>
        private PointF[] ConvertType(POINT[] points)
        {
            if (points == null)
                throw new ArgumentNullException("points");

            PointF[] result = new PointF[points.Length];

            for (int i = 0, len = points.Length; i < len; i++)
            {
               
                    result[i] = LPtoDP(points[i]);
            
            }

            return result;
        }

        /// <summary>
        /// Converts points from GDI poiont type to PointF.
        /// </summary>
        /// <param name="points">Array of points.</param>
        /// <returns>Converted array.</returns>
        private POINT[] ConvertTypeEx(POINTS[] points)
        {
            if (points == null)
                throw new ArgumentNullException("points");

            POINT[] result = new POINT[points.Length];

            for (int i = 0, len = points.Length; i < len; i++)
            {
                POINTS pt = points[i];
                result[i] = new POINT(pt.x, pt.y);
                // Point.Truncate( LPtoDP( points[ i ] ) );
            }

            return result;
        }

        /// <summary>
        /// Adds current point to array of points.
        /// </summary>
        /// <param name="points">Array of points.</param>
        /// <returns>Array with current point.</returns>
        private PointF[] AddCurrentPointTo(PointF[] points)
        {
            if (points == null)
                throw new ArgumentNullException("points");

            PointF firstPoint = Objects.CurrentPoint;
            PointF[] newPoints = new PointF[points.Length + 1];
            newPoints[0] = LPtoDP(Objects.CurrentPoint);
            Array.Copy(points, 0, newPoints, 1, points.Length);

            return newPoints;
        }

        /// <summary>
        /// Converts XFORM structure to Matrix object.
        /// </summary>
        /// <param name="xMatrix">XFORM structure.</param>
        /// <returns>Matrix object.</returns>
        private Matrix ConvertMatrix(XFORM xMatrix)
        {
            Matrix matrix = new Matrix(xMatrix.eM11, xMatrix.eM12, xMatrix.eM21,
                xMatrix.eM22, xMatrix.eDx, xMatrix.eDy);

            return matrix;
        }

        /// <summary>
        /// Calculates angle between two vectors.
        /// </summary>
        /// <param name="x0">x coordinate of start origin.</param>
        /// <param name="y0">y coordinate of start origin.</param>
        /// <param name="x1">x coordinate of vector.</param>
        /// <param name="y1">y coordinate of vector.</param>
        /// <returns>Angle between vector and x origin.</returns>
        private float GetAngle(float x0, float y0, float x1, float y1)
        {
            double vWidth = Math.Sqrt((double)(Math.Pow(x1 - x0, 2) + Math.Pow(y1 - y0, 2)));

            float angleDegr = 0.0f;

            if (vWidth != 0.0f)
            {
                double cosAngle = ((double)(x1 - x0)) / vWidth;

                float angleRad = (float)Math.Acos(cosAngle);
                angleDegr = (float)(((double)angleRad) / Math.PI * DegreeCount);

                if (y1 < y0)
                {
                    angleDegr = -angleDegr;
                }

                angleDegr = (angleDegr >= 0) ? angleDegr : 2 * DegreeCount + angleDegr;
            }

            return angleDegr;
        }

        /// <summary>
        /// Returns point on the ellipse bounded by rectangle intersecyet with radial point.
        /// </summary>
        /// <param name="bounds">Bounds structure.</param>
        /// <param name="radialPoint">Radial point.</param>
        /// <returns>Point of intersection.</returns>
        private PointF GetStartPoint(System.Drawing.Rectangle bounds, Point radialPoint)
        {
            // Idea is that we use function moving current point.
            PointF curPoint = Objects.CurrentPoint;

            GdiApi.ArcTo(Objects.Handle, bounds.Left, bounds.Top, bounds.Right,
                bounds.Bottom, radialPoint.X, radialPoint.Y, radialPoint.X, radialPoint.Y);

            PointF result = Objects.CurrentPoint;
            Objects.CurrentPoint = curPoint;

            return result;
        }

        /// <summary>
        /// Creates bitmap image.
        /// </summary>
        /// <param name="imageOffset">Offset to image data.</param>
        /// <param name="imgSize">Size of the image data.</param>
        /// <param name="ptr">Pointer to the data.</param>
        /// <param name="bitmapInfoPtr">Pointer to the BitmapInfo structure.</param>
        /// <param name="bmiHeader">Bitmap Info header.</param>
        /// <param name="iUsageSrc">Usage of the pixels.</param>
        /// <returns>Bitmap image.</returns>
        private Image GetAlphaBlendedBitmap(int imageOffset, uint imgSize, IntPtr ptr, IntPtr bitmapInfoPtr,
            BITMAPINFOHEADER bmiHeader, int iUsageSrc)
        {
            imageOffset -= (2 * IntSize);
            IntPtr imgPtr = new IntPtr(ptr.ToInt64() + imageOffset);
            byte[] imgData = new byte[imgSize];
            Marshal.Copy(imgPtr, imgData, 0, imgData.Length);

            Bitmap copy = new Bitmap(bmiHeader.biWidth, bmiHeader.biHeight, PixelFormat.Format32bppPArgb);

            BitmapData bmd = new BitmapData();
            Rectangle rect = new Rectangle(0, 0, bmiHeader.biWidth, bmiHeader.biHeight);

            copy.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb, bmd);

            System.Runtime.InteropServices.Marshal.Copy(imgData, 0, bmd.Scan0, imgData.Length);

            //Unlock the locked region.
            copy.UnlockBits(bmd);

            //Flip and Rotate the image.
            copy.RotateFlip(RotateFlipType.Rotate180FlipX);

            MemoryStream ms = new MemoryStream();

            copy.Save(ms, ImageFormat.Png);
            copy.Dispose();

            Image img = Image.FromStream(ms);
            return img;
        }


        /// <summary>
        /// Creates bitmap image.
        /// </summary>
        /// <param name="imageOffset">Offset to image data.</param>
        /// <param name="imgSize">Size of the image data.</param>
        /// <param name="ptr">Pointer to the data.</param>
        /// <param name="bitmapInfoPtr">Pointer to the BitmapInfo structure.</param>
        /// <param name="iUsageSrc">Usage of the pixels.</param>
        /// <returns>Bitmap image.</returns>
        private Bitmap GetBitmap(int imageOffset, uint imgSize, IntPtr ptr, IntPtr bitmapInfoPtr,
            int iUsageSrc)
        {
            imageOffset -= (2 * IntSize);
            IntPtr imgPtr = new IntPtr(ptr.ToInt64() + imageOffset);
            byte[] imgData = new byte[imgSize];
            Marshal.Copy(imgPtr, imgData, 0, imgData.Length);

            IntPtr hBitmap = GdiApi.CreateDIBitmap(Objects.Handle, bitmapInfoPtr,
                WinGdiConst.CBM_INIT, imgData, bitmapInfoPtr, (uint)iUsageSrc);

            Bitmap bmp = null;

            if (hBitmap != IntPtr.Zero)
            {
                // Clone image for preventing disposing it before document save.
                bmp = Image.FromHbitmap(hBitmap);
                bmp = bmp.Clone() as Bitmap;
            }
            else
            {
                Debug.WriteLine("Can't create bitmap");
            }

            GdiApi.DeleteObject(hBitmap);

            return bmp;
        }

        /// <summary>
        /// Creates bitmap image.
        /// </summary>
        /// <param name="imageOffset">Offset to image data.</param>
        /// <param name="imgSize">Size of the image data.</param>
        /// <param name="ptr">Pointer to the data.</param>
        /// <param name="bitmapInfoPtr">Pointer to the BitmapInfo structure.</param>
        /// <param name="iUsageSrc">Usage of the pixels.</param>
        /// <returns>Bitmap image.</returns>
        private IntPtr GetHBitmap(int imageOffset, uint imgSize, IntPtr ptr, IntPtr bitmapInfoPtr,
            int iUsageSrc)
        {
            imageOffset -= (2 * IntSize);
            IntPtr imgPtr = new IntPtr(ptr.ToInt64() + imageOffset);
            byte[] imgData = new byte[imgSize];
            Marshal.Copy(imgPtr, imgData, 0, imgData.Length);

            IntPtr hBitmap = GdiApi.CreateDIBitmap(Objects.Handle, bitmapInfoPtr,
                WinGdiConst.CBM_INIT, imgData, bitmapInfoPtr, (uint)iUsageSrc);

            return hBitmap;
        }

        /// <summary>
        /// Converts logical height of the font to it's point's value.
        /// </summary>
        /// <param name="logHeight">Logical height of the font.</param>
        /// <returns>Size of the font.</returns>
        private float GetFontSize(float logHeight)
        {
            logHeight = Math.Abs(logHeight);

            float pixSize = LPtoDPHeight(logHeight);

            float result = Math.Abs(pixSize /* * PointsPerInch / Objects.Resolution.X*/ );

            return result;
        }

        /// <summary>
        /// Returns metric of the font.
        /// </summary>
        /// <param name="font">Font object.</param>
        /// <returns>Metric of the font.</returns>
        private OUTLINETEXTMETRIC GetFontMetrix(Font font)
        {
            if (font == null)
                throw new ArgumentNullException("font");

            OUTLINETEXTMETRIC metric = new OUTLINETEXTMETRIC();

            IntPtr fontDC = font.ToHfont();
            using (Bitmap bmp = new Bitmap(1, 1))
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp))
            {
                IntPtr hDC = g.GetHdc();
                IntPtr prevObj = GdiApi.SelectObject(hDC, fontDC);

                int size = GdiApi.GetOutlineTextMetricsEx(hDC, 0, IntPtr.Zero);

                if (size != 0)
                {
                    IntPtr strPtr = Marshal.AllocHGlobal(size);

                    size = GdiApi.GetOutlineTextMetricsEx(hDC, size, strPtr);

                    if (size != 0)
                    {
                        metric = (OUTLINETEXTMETRIC)GetStructure(strPtr, typeof(OUTLINETEXTMETRIC));
                    }
                    Marshal.FreeHGlobal(strPtr);
                }

                GdiApi.SelectObject(Objects.Handle, prevObj);
                GdiApi.DeleteObject(fontDC);
                g.ReleaseHdc(hDC);
            }

            return metric;
        }

        /// <summary>
        /// Returns string format for the text.
        /// </summary>
        /// <returns>String format for the text.</returns>
        private StringFormat GetStringFormat(string text, OUTLINETEXTMETRIC metric, ref PointF location)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            SIZE textSize = new SIZE();
            GdiApi.GetTextExtentPoint32(Objects.Handle, text, text.Length, out textSize);

            float width = LPtoDPWidth(textSize.cx);

            TA_TEXT_ALIGN textAlign = Objects.TextAlign;

            StringFormat format = StringFormat.GenericTypographic.Clone() as StringFormat;

            // Get current point.
            if ((textAlign & TA_TEXT_ALIGN.TA_UPDATECP) > 0)
            {
                location = LPtoDP(Objects.CurrentPoint);
            }

            // Horizontal alignment
            TA_TEXT_ALIGN horAlignment = textAlign & TA_TEXT_ALIGN.TA_CENTER;

            if (horAlignment == TA_TEXT_ALIGN.TA_CENTER)
            {
                format.Alignment = StringAlignment.Center;
                //format.LineAlignment = StringAlignment.Center;
                location.X -= (width / 2.0f);
            }
            else if (horAlignment == TA_TEXT_ALIGN.TA_RIGHT)
            {
                format.Alignment = StringAlignment.Far;
                location.X -= width;
            }
            else if (horAlignment == TA_TEXT_ALIGN.TA_LEFT)
            {
                format.Alignment = StringAlignment.Near;
                //location.X -= width;
            }

            // Vertical alignment
            TA_TEXT_ALIGN vertAlignment = textAlign & TA_TEXT_ALIGN.TA_BASELINE;

            if (vertAlignment == TA_TEXT_ALIGN.TA_BASELINE)
            {
                //float height = metric.otmrcFontBox.top - metric.otmrcFontBox.bottom;

                //if (Objects.Font.Bold)
                //    location.Y -= (int)(height / 2);
                //else
                //Font f = m_selectedFont;

                if (Objects.TextAngle != 0)
                {
                    Font f = m_selectedFont;
                    float baselineOffset = f.SizeInPoints / f.FontFamily.GetEmHeight(f.Style) * f.FontFamily.GetCellAscent(f.Style);
                    location.Y -= baselineOffset;
                }
                //float baselineOffset = f.SizeInPoints / f.FontFamily.GetEmHeight(f.Style) * f.FontFamily.GetCellAscent(f.Style);
                //float baselineOffsetPixels = baselineOffset;
                //baselineOffsetPixels = (baselineOffsetPixels + 0.5f);
                ////if (f.Name.ToLower() == "arial")
                //{
                //    baselineOffsetPixels = Renderer.Graphics.m_DpiY / 72f * baselineOffset;
                //    baselineOffsetPixels = (int)(baselineOffsetPixels + 0.5f);
                //}
                //location.Y -= baselineOffsetPixels;
                Renderer.Graphics.m_isBaselineFormat = true;
                format.LineAlignment = StringAlignment.Center;
            }
            else if (vertAlignment == TA_TEXT_ALIGN.TA_BOTTOM)
            {
                Renderer.Graphics.m_isBaselineFormat = false;
                float height = metric.otmrcFontBox.top - metric.otmrcFontBox.bottom;
                location.Y -= height;
                format.LineAlignment = StringAlignment.Far;
            }
            else if (vertAlignment == TA_TEXT_ALIGN.TA_TOP)
            {
                Renderer.Graphics.m_isBaselineFormat = false;
                //Renderer.Graphics.m_isEMF = false;
                //float height = metric.otmMacAscent - metric.otmMacDescent + metric.otmMacLineGap;
                //location.Y -= height;
                format.LineAlignment = StringAlignment.Near;
            }

            if ((textAlign & TA_TEXT_ALIGN.TA_RTLREADING) > 0)
            {
                format.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
            }

            return format;
        }

        /// <summary>
        /// Draws text.
        /// </summary>
        /// <param name="text">Text to be printed.</param>
        /// <param name="format">String format object.</param>
        /// <param name="bounds">System.Drawing.Rectangle structure.</param>
        /// <param name="metric">Structure describing text settings.</param>
        /// <param name="widths">The widths.</param>
        private void DrawText(string text, StringFormat format, RectangleF bounds,
            OUTLINETEXTMETRIC metric, float[] widths)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            if (format == null)
                throw new ArgumentNullException("format");

            Font font;
            if (m_selectedFont != null)
                font = m_selectedFont;
            else
                font = Objects.Font;

            if (Objects.IsOpenPath && font != null)
            {
                Objects.Path.AddString(text, font.FontFamily, (int)font.Style,
                    font.SizeInPoints, bounds.Location, format);
            }
            else if (font != null)
            {
                 //Rotate text.
                //if (Objects.TextAngle != 0f)
                //{
                //    Objects.GraphicsState = Renderer.Save();

                //    Matrix transform = new Matrix();
                //    PointF pt = new PointF(bounds.X, bounds.Y + metric.otmMacAscent);

                //    // Rotate text.
                //    transform.Translate(bounds.X, bounds.Y);
                //    transform.RotateAt(Objects.TextAngle, pt, MatrixOrder.Append);

                //    Renderer.MultiplyTransform(transform, MatrixOrder.Prepend);
                //    bounds.Location = PointF.Empty;
                //}

                using (Brush brush = new SolidBrush(Objects.ForeColor))
                {
                    if (widths == null)
                    {
                        if (TextAngle != 0f)
                           Renderer.DrawString(text, font, brush,new RectangleF(bounds.X  - font.Size,bounds.Y - font.Size,bounds.Width,bounds.Height), format, TextAngle);
                        else if (Objects.TextAngle != 0)
                            Renderer.DrawString(text, font, brush,new RectangleF(bounds.X - font.Size, bounds.Y - font.Size, bounds.Width, bounds.Height), format, Objects.TextAngle);                      
                        else
                            Renderer.DrawString(text, font, brush, bounds, format);
                    }
                    else
                    {
                        RectangleF pos = bounds;

                        if (format.Alignment != StringAlignment.Near &&
                            (format.FormatFlags & StringFormatFlags.DirectionRightToLeft) != 0)
                        {
                            float totalWidth = 0;

                            foreach (float width in widths)
                            {
                                totalWidth += width;
                            }

                            if (format.Alignment == StringAlignment.Center)
                            {
                                pos.X += (pos.Width - totalWidth) / 2;
                                format.Alignment = StringAlignment.Near;
                            }
                            else
                            {
                                bool far = ((format.FormatFlags & StringFormatFlags.DirectionRightToLeft) != 0) ?
                                    format.Alignment == StringAlignment.Near : format.Alignment == StringAlignment.Far;

                                if (far)
                                {
                                    pos.X += (pos.Width - totalWidth);
                                }
                            }

                            format.Alignment = ((format.FormatFlags & StringFormatFlags.DirectionRightToLeft) != 0) ?
                                StringAlignment.Far : StringAlignment.Near;
                        }

                        if (TextAngle != 0f)
                        {
                            Renderer.Graphics.m_isBaselineFormat = false;
                            Renderer.DrawString(text, font, brush, new RectangleF((bounds.X - font.Size), (bounds.Y + font.Size), bounds.Width, bounds.Height), format, TextAngle);
                        }
                        else if (Objects.TextAngle >= -180 && Objects.TextAngle < -90)
                        {
                            Renderer.Graphics.m_isBaselineFormat = false;
                            Renderer.DrawString(text, font, brush, new RectangleF(bounds.X, bounds.Y + font.Height, bounds.Width, bounds.Height), format, Objects.TextAngle);
                        }
                        else if (Objects.TextAngle < -180 && Objects.TextAngle > -360)
                        {
                            Renderer.Graphics.m_isBaselineFormat = false;
                            Renderer.DrawString(text, font, brush, new RectangleF(bounds.X + font.Size, bounds.Y + font.Size, bounds.Width, bounds.Height), format, Objects.TextAngle);
                        }
                        else if (Objects.TextAngle >= -90 && Objects.TextAngle != 0)
                        {
                            Renderer.Graphics.m_isBaselineFormat = false;
                            Renderer.DrawString(text, font, brush, new RectangleF(bounds.X - font.Size, bounds.Y + font.Size, bounds.Width, bounds.Height), format, Objects.TextAngle);
                        }
                        else
                        {
                            Renderer.DrawString(text, font, brush, bounds, format);
                        }
                    }
                }

                if (Objects.TextAngle != 0f)
                {
                    Renderer.Restore(Objects.GraphicsState);
                }
            }
        }

        /// <summary>
        /// Gets current map mode.
        /// </summary>
        /// <returns>Current map mode.</returns>
        private MAPPING_MODE GetMapMode()
        {
            int iMode = GdiApi.GetMapMode(Objects.Handle);

            return (MAPPING_MODE)iMode;
        }

        /// <summary>
        /// Retireves array of bytes from the unmanaged memory.
        /// </summary>
        /// <param name="ptrData">Pointer to the memory.</param>
        /// <param name="dataSize">Size of the data.</param>
        /// <returns>Byte array.</returns>
        private byte[] GetData(IntPtr ptrData, int dataSize)
        {
            byte[] data = new byte[dataSize];

            if (ptrData != IntPtr.Zero)
            {
                Marshal.Copy(ptrData, data, 0, dataSize);
            }

            return data;
        }

        /// <summary>
        /// Implements the final stage of the 'Blt'-family functions.
        /// </summary>
        /// <param name="imageOffset"></param>
        /// <param name="imgSize"></param>
        /// <param name="ptr"></param>
        /// <param name="bitmapInfoPtr"></param>
        /// <param name="destRect"></param>
        /// <param name="srcRect"></param>
        /// <param name="dwRop"></param>
        /// <param name="iUsageSrc"></param>
        private void DrawImage(int imageOffset, uint imgSize, IntPtr ptr, IntPtr bitmapInfoPtr,
            RectangleF destRect, RectangleF srcRect, RASTER_CODE dwRop, int iUsageSrc)
        {
            Bitmap bmp = null;

            if (imageOffset > 0 && imgSize > 0 && destRect.Width > 0 && destRect.Height > 0)
            {
                bmp = GetBitmap(imageOffset, imgSize, ptr, bitmapInfoPtr, iUsageSrc);
            }

            // Store the information about image region at the context.
            if (destRect.Height > 0)
            {
                //Skip the invalid regions.
                if (destRect.Location.Y != 0 && bmp != null)
                {
                    ImageRegion rgn = new ImageRegion(destRect.Location.Y, destRect.Height);
                    ImageRegions.Add(rgn);
                }
            }

                if ((Objects.Brush is SolidBrush) && (Objects.Brush as SolidBrush).Color.A == 0)
                    Renderer.DrawImage(bmp, destRect, srcRect, GraphicsUnit.Pixel);
                else if (Enum.IsDefined(typeof(RASTER_CODE), dwRop))
                {
                    Renderer.DrawImage(bmp, Objects.Brush, destRect, srcRect, (uint)dwRop);
                }
            
            if (bmp != null)
                bmp.Dispose();
        }

       private float CalculateRotationAngle(EMR_MODIFYWORLDTRANSFORM recordData)
        {
            float cosineValue = recordData.xform.eM12;
            double cos = (double)cosineValue;

            double angle = Math.Asin(cos);
            float angleR = (float)(angle*180/Math.PI);
                                  
            return angleR;
        }
        #endregion
    }
}
#endif
