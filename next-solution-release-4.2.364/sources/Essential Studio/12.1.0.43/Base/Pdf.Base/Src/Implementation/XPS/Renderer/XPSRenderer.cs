#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Globalization;

using Syncfusion.Pdf;
using Syncfusion.XPS;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.Primitives;
using Syncfusion.Pdf.Functions;
using Syncfusion.Pdf.ColorSpace;
using Syncfusion.Pdf.Graphics.Fonts;

namespace Syncfusion.XPS
{
    /// <summary>
    /// Represnets the XPS to PDF renderer.
    /// </summary>
    internal class XPSRenderer : IDisposable
    {
        #region Fields
        /// <summary>
        /// Represents the PdfGraphics where the XPS graphics is to be transformed.
        /// </summary>
        private PdfGraphics m_graphics;
        /// <summary>
        /// Represents the current PdfPage where the XPS graphics is transformed.
        /// </summary>
        private PdfPage m_page;
        /// <summary>
        /// Represents the PdfUnitConverter for the internal pixels to point conversion.
        /// </summary>
        private PdfUnitConvertor m_unitConvertor;
        /// <summary>
        /// Represents the XPS reader object.
        /// </summary>
        private XPSDocumentReader m_reader;
        /// <summary>
        /// Represents the comma separator
        /// </summary>
        private char[] m_commaSeparator = { ',' };
        /// <summary>
        /// 
        /// </summary>
        private bool m_bStateChanged;
        /// <summary>
        /// Represents the current transformation matrix.
        /// </summary>
        private PdfTransformationMatrix currentMatrix;
        private Canvas m_canvas;
        #endregion

        #region Constructors
        /// <summary>
        /// Intializes a new instance of the XPSRenderer class.
        /// </summary>
        /// <param name="page"> The current PDF Page</param>
        /// <param name="reader"> The XPS document reader</param>
        public XPSRenderer(PdfPage page, XPSDocumentReader reader)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            m_page = page;
            m_graphics = page.Graphics;
            m_unitConvertor = new PdfUnitConvertor();
            m_reader = reader;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the graphics of the current PdfPage
        /// </summary>
        internal PdfGraphics Graphics
        {
            get
            {
                return m_graphics;
            }
        }

        internal PrivateFontCollection PrivateFonts
        {
            get
            {
                return PdfDocument.PrivateFonts;
            }            
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Draws/converts the text objects from XPS to PDF.
        /// </summary>
        /// <param name="glyphs">Glyph object with the metrics</param>
        public void DrawGlyphs(Glyphs glyphs)
        {
            PdfGraphicsState state = m_graphics.Save();
            if (glyphs.RenderTransform != null)
            {
               
                Matrix matrix = ReadMatrix(glyphs.RenderTransform);
                float[] element = matrix.Elements;
                glyphs.OriginX = glyphs.OriginX * element[0];
                glyphs.OriginY = glyphs.OriginY * element[3];
                glyphs.FontRenderingEmSize = glyphs.FontRenderingEmSize * element[0];
              
            }

            if (glyphs.GlyphsRenderTransform != null)
            {
                state = m_graphics.Save();
                ApplyRenderTransform(glyphs.GlyphsRenderTransform.MatrixTransform.Matrix);
            }
          
            if (glyphs.UnicodeString != null && glyphs.FontRenderingEmSize > 0.0)
            {
                PointF location = new PointF(ConvertToPoints(glyphs.OriginX), ConvertToPoints(glyphs.OriginY));

                PdfFont font = GetFont(glyphs);

                PdfStringFormat glyphFormat = new PdfStringFormat();

                // GetFontAscent function causes difference in ascent values.
                // When drawing a string using PDF, it is read directly from metrics.
                float ascent = font.Metrics.GetAscent(glyphFormat);// GetFontAscent(font, glyphs);
                if (!glyphs.IsSideways)
                    location.Y = location.Y - ascent;

                PdfGraphicsState italic = null;

                if ((glyphs.StyleSimulations == StyleSimulations.ItalicSimulation || glyphs.StyleSimulations == StyleSimulations.BoldItalicSimulation) && !font.Metrics.PostScriptName.Contains("Italic"))
                {
                    italic = m_graphics.Save();
                    m_graphics.TranslateTransform(location.X, location.Y);
                    m_graphics.SkewTransform(0, -10);
                    location = PointF.Empty;
                }

                PdfBrush brush = GetSolidBrush(glyphs.Fill);
                PdfPen pen = null;
                bool boldStyle = false;

                if (glyphs.Fill == null && glyphs.GlyphsFill != null)
                {
                    if (glyphs.GlyphsFill.Item is SolidColorBrush)
                        brush = new PdfSolidBrush(FromHtml((glyphs.GlyphsFill.Item as SolidColorBrush).Color));
                    else if (glyphs.GlyphsFill.Item is LinearGradientBrush)
                        brush = ReadLinearGradientBrush((glyphs.GlyphsFill.Item as LinearGradientBrush));
                    else if (glyphs.GlyphsFill.Item is RadialGradientBrush)
                        brush = ReadRadialGradientBrush((glyphs.GlyphsFill.Item as RadialGradientBrush));
                }

                if ((glyphs.StyleSimulations == StyleSimulations.BoldSimulation || glyphs.StyleSimulations == StyleSimulations.BoldItalicSimulation) && !font.Metrics.PostScriptName.Contains("Bold"))
                {
                    boldStyle = true;
                    pen = new PdfPen(brush);
                    pen.Width = .3f;
                }

                //if (brush != null && (Regex.IsMatch(glyphs.Fill, "^#?(([a-fA-F0-9]){3}){1,2}$") || Regex.IsMatch(glyphs.Fill, "^#?(([a-fA-F0-9]){4}){1,2}$") || glyphs.Fill.Contains("StaticResource")))
                {
                    //if (glyphs.Clip != null)
                    //{
                    //    PdfPath path = GetPathFromGeometry(glyphs.Clip);
                    //    m_graphics.SetClip(path);
                    //}

                    PdfStringFormat stringFormat = new PdfStringFormat();
                    stringFormat.MeasureTrailingSpaces = true;

                    // Only advance width is supported. Combination of other attributes is currently not supported.
                    if (glyphs.Indices != null && glyphs.Indices.Contains(",") && !glyphs.Indices.Contains("(") && !glyphs.Indices.Contains(")") && !glyphs.Indices.Contains(".") && !glyphs.Indices.Contains("-") && !glyphs.Indices.Contains("E") && !glyphs.Indices.Contains("e"))
                    {
                        string[] charSplit = glyphs.Indices.Split(';');

                        PdfTextElement elem = new PdfTextElement();
                        PdfLayoutResult result = null;
                        PointF p = location;
                        float round = 0;

                        if (charSplit.Length > 0 && charSplit.Length <= glyphs.UnicodeString.Length)
                        {
                            for (int i = 0; i < charSplit.Length; i++)
                            {
                                elem.Text = glyphs.UnicodeString[i].ToString();
                                elem.Font = font;
                                elem.Brush = brush;
                                if (boldStyle)
                                    elem.Pen = pen;
                                elem.StringFormat = stringFormat;
                                if (result != null)
                                    if (round != 0)
                                        location.X = result.Bounds.Left + round;
                                    else
                                        location.X = result.Bounds.Right;

                                round = 0;

                                if (charSplit[i].Length > 0 && charSplit[i].Contains(","))
                                {
                                    charSplit[i] = charSplit[i].Split(new char[] { ',' })[1];

                                    // Get the advance width.
                                    int index = int.Parse(charSplit[i]);

                                    // It is specified in percentage of em.
                                    round = (float)(index * glyphs.FontRenderingEmSize / 100);

                                    // Convert to points. This should be the width of the current glyph.
                                    round = ConvertToPoints(round);
                                }
                                if (round > 0 && round > font.GetCharWidth(glyphs.UnicodeString[i], stringFormat))
                                    result = elem.Draw(m_page, location, round, new PdfLayoutFormat());
                                else
                                    result = elem.Draw(m_page, location);
                            }

                            if (round > 0)
                            {
                                location.X = result.Bounds.X + round;
                                round = 0;
                            }
                            else if (result != null)
                                location.X = result.Bounds.Right;
                            string rem = glyphs.UnicodeString.Substring(charSplit.Length);
                            if (!boldStyle)
                                m_graphics.DrawString(rem, font, brush, location, stringFormat);
                            else
                                m_graphics.DrawString(rem, font, pen, brush, location, stringFormat);

                            if (italic != null)
                            {
                                m_graphics.Restore(italic);
                                italic = null;
                            }

                            if (state != null)
                                m_graphics.Restore(state);

                            return;
                        }
                    }
                    if (glyphs.IsSideways)
                    {
                        m_graphics.RotateTransform(-90);
                        char[] unicodeString = glyphs.UnicodeString.ToCharArray();
                        foreach (char s in unicodeString)
                        {
                            PdfStringLayouter layouter = new PdfStringLayouter();
                            PdfStringLayoutResult result = layouter.Layout(s.ToString(), font, stringFormat, new SizeF());

                            PointF tLocation = location;
                            tLocation.X = -location.Y - result.ActualSize.Width / 2;
                            tLocation.Y = location.X;

                            if (!boldStyle)
                                m_graphics.DrawString(s.ToString(), font, brush, tLocation, stringFormat);
                            else
                                m_graphics.DrawString(s.ToString(), font, pen, brush, tLocation, stringFormat);

                            location.X += ConvertToPoints(result.ActualSize.Height);
                        }
                        m_graphics.RotateTransform(90);
                    }
                    else
                        if (!boldStyle)
                            m_graphics.DrawString(glyphs.UnicodeString, font, brush, location, stringFormat);
                        else
                            m_graphics.DrawString(glyphs.UnicodeString, font, pen, brush, location, stringFormat);

                    if (italic != null)
                    {
                        m_graphics.Restore(italic);
                        italic = null;
                    }
                }
            }
            if (glyphs.Clip != null)
            {
                PdfPath path = GetPathFromGeometry(glyphs.Clip);
                m_graphics.SetClip(path);
            }
            if (state != null)
                m_graphics.Restore(state);
        }

        /// <summary>
        /// Converts the path string to a PdfPath
        /// </summary>
        /// <param name="pathData">Represents the path string</param>
        /// <returns> Returns the PdfPath object</returns>
        public PdfPath GetPathFromGeometry(string pathData)
        {
            if (pathData == null)
                return null;

            if(pathData.Contains("StaticResource"))
            {
                PathGeometry pathGeometry = ReadStaticResource(pathData) as PathGeometry;
                if (pathGeometry.Figures != null)
                    pathData = pathGeometry.Figures;
                else
                    return GetPathFromPathGeometry(pathGeometry);
            }

            pathData = pathData.Trim();

            PathDataReader reader = new PathDataReader(pathData);
            reader.Position = 0;
            PointF location = PointF.Empty;
            PointF lastEndingPoint = PointF.Empty;

            GraphicsPath gp = new GraphicsPath();

            char previous = '\0';
            char ch;
            while ((ch = reader.ReadSymbol()) != '\0')
            {
                switch (ch)
                {
                    case 'F':
                        float fillMode;
                        if (reader.TryReadFloat(out fillMode))
                        {
                            gp.FillMode = (fillMode == 0) ? FillMode.Alternate : FillMode.Winding;
                        }
                        previous = ch;
                        break;

                    case 'M':
                        PointF point;
                        if (reader.TryReadPoint(out point))
                        {
                            gp.StartFigure();
                            location = ConvertToPoints(point);
                            // As per Table 11–2 in specification, if the move follows close, start a new figure.
                            //if (previous == 'Z' || previous == 'z')
                                lastEndingPoint = PointF.Empty;
                        }
                        previous = ch;
                        break;

                    case 'L':
                    case 'l':
                        PointF startPoint;
                        List<PointF> points = new List<PointF>();

                        if (lastEndingPoint != PointF.Empty)
                        {
                            startPoint = lastEndingPoint;
                        }
                        else
                        {
                            startPoint = location;
                        }

                        while (reader.TryReadPoint(out point))
                        {
                            gp.AddLine(startPoint, ConvertToPoints(point));
                            startPoint = lastEndingPoint = ConvertToPoints(point);
                        }
                        previous = ch;
                        break;

                    case 'C':

                        points = new List<PointF>();
                        if (lastEndingPoint != PointF.Empty)
                        {
                            points.Add(lastEndingPoint);
                        }
                        else
                        {
                            points.Add(location);
                        }

                        PointF[] pts = null;

                        while (reader.TryReadPointM3(out pts))
                        {
                            foreach (PointF pt in pts)
                                points.Add(ConvertToPoints(pt));
                        }

                        if ((points.Count - 1) % 3 == 0)
                            gp.AddBeziers(points.ToArray());
                        else
                            gp.AddLines(points.ToArray());
                        lastEndingPoint = points[points.Count - 1];
                        points.Clear();
                        previous = ch;
                        break;

                    case 'H':
                        float xCoordinate;
                        if (reader.TryReadFloat(out xCoordinate))
                        {
                            float xDistance = ConvertToPoints(xCoordinate);
                            if (location != null)
                            {
                                gp.AddLine(location, new PointF(xDistance, location.Y));
                                location = new PointF(xDistance, location.Y);
                            }
                        }
                        previous = ch;
                        break;

                    case 'V':
                        float yCoordinate;
                        if (reader.TryReadFloat(out yCoordinate))
                        {
                            float yDistance = ConvertToPoints(yCoordinate);
                            if (location != null)
                            {
                                gp.AddLine(location, new PointF(location.X, yDistance));
                                location = new PointF(location.X, yDistance);
                            }
                        }
                        previous = ch;
                        break;

                    case 'h':
                        if (reader.TryReadFloat(out xCoordinate))
                        {
                            float xDistance = ConvertToPoints(xCoordinate);
                            if (location != null)
                            {
                                gp.AddLine(location, new PointF((location.X + xDistance), location.Y));
                                location = new PointF((location.X + xDistance), location.Y);
                            }
                        }
                        previous = ch;
                        break;
                    case 'v':
                        if (reader.TryReadFloat(out yCoordinate))
                        {
                            float yDistance = ConvertToPoints(yCoordinate);
                            if (location != null)
                            {
                                gp.AddLine(location, new PointF(location.X, (location.Y + yDistance)));
                                location = new PointF(location.X, (location.Y + yDistance));
                            }
                        }
                        previous = ch;
                        break;

                    case 'A':
                        if (reader.TryReadPoint(out point))
                        {
                            float rotationAngle;
                            float isLargeArcFlag;
                            float sweepDirectionFlag;
                            PointF endpoint;
                            reader.TryReadFloat(out rotationAngle);
                            reader.TryReadFloat(out isLargeArcFlag);
                            reader.TryReadFloat(out sweepDirectionFlag);
                            reader.TryReadPoint(out endpoint);

                            point = ConvertToPoints(point);
                            endpoint = ConvertToPoints(endpoint);
                            if (lastEndingPoint != PointF.Empty)
                            {
                                location = lastEndingPoint;
                            }
                                List<PointF> arcArray = ComputeArc(location, endpoint, point.X, point.Y, rotationAngle, (isLargeArcFlag == 1.0 ? true : false), (sweepDirectionFlag == 1.0 ? false : true));
                            
                            if (arcArray.Count == 0)
                            {
                                gp.AddLine(location, endpoint);
                                location = endpoint;
                            }
                            else if (arcArray.Count < 0)
                            {
                                previous = ch;
                                break;
                            }

                            gp.AddBeziers(arcArray.ToArray());
                            lastEndingPoint = location = arcArray[arcArray.Count - 1];
                            arcArray.Clear();
                        }
                        previous = ch;
                        break;
                    case 'Z':
                    case 'z':
                        if (reader.EOF)
                        {
                            gp.CloseAllFigures();
                            break;
                        }
                        else
                        {
                            gp.CloseFigure();
                        }
                        previous = ch;
                        break;

                    default:
                        //    ThrowNotImplementedException();
                        break;
                }
            }

            if (gp.PathData.Points.LongLength > 0)
            {
                PdfPath pdfPath = new PdfPath(gp.PathPoints, gp.PathTypes);
                pdfPath.FillMode = (gp.FillMode == FillMode.Alternate) ? PdfFillMode.Alternate : PdfFillMode.Winding;
                pdfPath.CloseAllFigures();
                return pdfPath;
            }
            else
            {
                return new PdfPath();
            }
        }

        private List<PointF> ComputeArc(PointF startPoint, PointF endPoint, float radiusX, float radiusY, double rotationAngle, bool isLargeArc, bool isCounterClockwise)
        {
            List<PointF> points = new List<PointF>();

            // Start point
            points.Add(startPoint);

            double centerX, centerY;
            bool zeroCenter = false;

            Matrix matx = new Matrix();

            //PointF midPoint = new PointF((startPoint.X + endPoint.X) / 2, (startPoint.Y + endPoint.Y) / 2);
            double midPointX = (endPoint.X - startPoint.X) / 2;
            double midPointY = (endPoint.Y - startPoint.Y) / 2;

            PointF vect = new PointF(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
            double halfChord = Math.Sqrt(vect.X * vect.X + vect.Y * vect.Y) / 2;

            // Rotate
            rotationAngle = -rotationAngle * (Math.PI / 180);

            double cos = Math.Cos(rotationAngle);
            double sin = Math.Sin(rotationAngle);

            double temp = midPointX * cos - midPointY * sin;
            midPointY = midPointX * sin + midPointY * cos;
            midPointX = temp;

            // Scale
            midPointX /= radiusX;
            midPointY /= radiusY;

            halfChord = midPointX * midPointX + midPointY * midPointY;

            if (halfChord > 1)
            {
                temp = Math.Sqrt(halfChord);
                radiusX *= (float)temp;
                radiusY *= (float)temp;
                centerX = centerY = 0;
                zeroCenter = true;

                midPointX /= temp;
                midPointY /= temp;
            }
            else
            {
                temp = Math.Sqrt((1 - halfChord) / halfChord);
                
                if (isLargeArc == isCounterClockwise)
                {
                    // in the direction of (-y, x) 
                    centerX = -temp * midPointY;
                    centerY = temp * midPointX;
                }
                else
                {
                    // in the direction of (y, -x)
                    centerX = temp * midPointY;
                    centerY = -temp * midPointX;
                }
            }

            PointF strPoint = new PointF((float)(-midPointX - centerX), (float)(-midPointY - centerY));
            PointF ePoint = new PointF((float)(midPointX - centerX), (float)(midPointY - centerY));

            double adjustOffsetX = (!zeroCenter) ? (cos * radiusX * centerX + sin * radiusX * centerY) : 0;
            double adjustOffsetY = (!zeroCenter) ? (-sin * radiusY * centerX + cos * radiusY * centerY) : 0;

            matx = new Matrix((float)cos * radiusX, (float)-sin * radiusX, (float)sin * radiusY, (float)cos * radiusY,
                (float)adjustOffsetX + (endPoint.X + startPoint.X) / 2, (float)adjustOffsetY + (endPoint.Y + startPoint.Y) / 2);

            double cosAngle, sinAngle;
            int lines;

            // Get the sine & cosine of the angle
            GetArcAngle(strPoint, ePoint, isLargeArc, isCounterClockwise, out cosAngle, out sinAngle, out lines);

            // Get Bezier control point 
            double bezDist = GetBezierDistance(cosAngle, 1);

            if (isCounterClockwise)
                bezDist = -bezDist;

            PointF vecToBez1 = new PointF((float)-bezDist * strPoint.Y, (float)bezDist * strPoint.X);
            PointF vecToBez2;

            PointF pt1 = PointF.Empty;
            PointF pt2 = PointF.Empty;
            PointF[] array;

            for (int idx = 1; idx < lines; idx++)
            {
                PointF ptPieceEnd = new PointF((float)(strPoint.X * cosAngle - strPoint.Y * sinAngle), (float)(strPoint.X * sinAngle + strPoint.Y * cosAngle));
                vecToBez2 = new PointF((float)-bezDist * ptPieceEnd.Y, (float)bezDist * ptPieceEnd.X);

                pt1 = new PointF((float)(strPoint.X + vecToBez1.X), (float)(strPoint.Y + vecToBez1.Y));
                pt2 = new PointF((float)(ptPieceEnd.X - vecToBez2.X), (float)(ptPieceEnd.Y - vecToBez2.Y));
                array = new PointF[] { pt1, pt2, ptPieceEnd };

                matx.TransformPoints(array);
                points.AddRange(array);

                strPoint = ptPieceEnd;
                vecToBez1 = vecToBez2;
            }

            vecToBez2 = new PointF((float)-bezDist * ePoint.Y, (float)bezDist * ePoint.X);

            pt1 = new PointF((float)(strPoint.X + vecToBez1.X), (float)(strPoint.Y + vecToBez1.Y));
            pt2 = new PointF((float)(ePoint.X - vecToBez2.X), (float)(ePoint.Y - vecToBez2.Y));

            array = new PointF[] { pt1, pt2 };
            matx.TransformPoints(array);

            points.AddRange(array);
            
            // End point
            points.Add(new PointF(endPoint.X, endPoint.Y));
            
            return points;
        }

        private void GetArcAngle(PointF startPoint, PointF endPoint, bool isLargeArc, bool isCounterClockwise, out double cosArcAngle, out double sinArcAngle, out int lines)
        {
            cosArcAngle = startPoint.X * endPoint.X + startPoint.Y * endPoint.Y;
            sinArcAngle = startPoint.X * endPoint.Y - startPoint.Y * endPoint.X;

            if (cosArcAngle >= 0)
            {
                if (isLargeArc)
                    lines = 4;
                else
                {
                    lines = 1;
                    return;
                }
            }
            else
            {
                if (isLargeArc)
                    lines = 3;
                else
                    lines = 2;
            }

            double angle = Math.Atan2(sinArcAngle, cosArcAngle);

            if (!isCounterClockwise)
            {
                if (angle < 0)
                    angle += Math.PI * 2;
            }
            else
            {
                if (angle > 0)
                    angle -= Math.PI * 2;
            }

            angle /= lines;
            cosArcAngle = Math.Cos(angle);
            sinArcAngle = Math.Sin(angle);
        }

        private double GetBezierDistance(double dot, double radius)
        {
            double radSquared = radius * radius;
            double dist = 0;
            double cos = (radSquared + dot) / 2;

            if (cos < 0)
                return dist;

            double sin = radSquared - cos;
            if (sin <= 0)
                return dist;

            sin = Math.Sqrt(sin);
            cos = Math.Sqrt(cos);

            dist = 4 * (radius - cos) / 3;
            if (dist <= sin * 0.000001)
                dist = 0;
            else
                dist = 4 * (radius - cos) / sin / 3;

            return dist;
        }

        private void ThrowNotImplementedException()
        {
#if DEBUG
            throw new NotImplementedException();
#endif
        }

        /// <summary>
        /// Draws/converts the XPS Path to PDF path.
        /// </summary>
        /// <param name="path"> The XPS path object</param>
        public void DrawPath(Path path)
        {
            float[] dashPattern = null;
            PdfPen pen = null;
            PdfBrush brush = null;
            Image brushImage = null;
            SizeF rect = SizeF.Empty;

            PdfGraphicsState gs = Graphics.Save();

            // Transformation
            if (path.RenderTransform != null)
            {
                string renderTrans = path.RenderTransform;
                if (renderTrans.Contains("StaticResource"))
                {
                    MatrixTransform matrixTransform = ReadStaticResource(renderTrans) as MatrixTransform;
                    renderTrans = matrixTransform.Matrix;
                }
                ApplyRenderTransform(renderTrans);
            }

            PdfPath pdfPath = GetPathFromGeometry(path.Data);

            if (pdfPath == null && path.PathData != null)
            {
                pdfPath = GetPathFromPathGeometry(path.PathData.PathGeometry);
            }

            //Detect Strokes
            if (path.StrokeDashArray != null)
            {
                string[] pattern;
                PathDataReader reader = new PathDataReader(path.StrokeDashArray);
                reader.TryReadPositionArray(out pattern);
                dashPattern = new float[pattern.Length];
                for (int i = 0; i < pattern.Length; i++)
                    dashPattern[i] = ConvertToPoints(ParseFloat(pattern[i]));
            }

            //Get Pen
            if (path.Stroke != null )
            {
                if (path.Stroke.Contains("StaticResource"))
                    path.PathStroke = ReadStaticResource(path.Stroke) as Brush;
                else
                {
                Color color = FromHtml(path.Stroke);

                pen = new PdfPen(new PdfColor(color), ConvertToPoints(path.StrokeThickness));
                pen.LineJoin = (path.StrokeLineJoin == LineJoin.Bevel) ?
                    PdfLineJoin.Bevel : (path.StrokeLineJoin == LineJoin.Miter) ? PdfLineJoin.Miter
                    : PdfLineJoin.Round;

                if (dashPattern != null)
                {
                    pen.DashStyle = PdfDashStyle.Dash;
                    pen.DashPattern = dashPattern;
                }
                }
            }
            if (path.PathStroke is Brush)
            {
                if (path.PathStroke.Item is LinearGradientBrush)
                {
                    LinearGradientBrush gradientBrush = (path.PathStroke.Item as LinearGradientBrush);
                    PdfBrush tempBrush = ReadLinearGradientBrush(gradientBrush);

                    pen = new PdfPen(tempBrush, (float)path.StrokeThickness);
                }
                else if (path.PathStroke.Item is ImageBrush)
                {
                    ImageBrush newBrush = (path.PathStroke.Item as ImageBrush);
                    if (!String.IsNullOrEmpty(newBrush.Transform))
                        ApplyRenderTransform(newBrush.Transform);

                    try
                    {
                        brushImage = Image.FromStream(m_reader.ReadImage(newBrush.ImageSource));

                        SizeF imageSize = new SizeF(ConvertToPoints(brushImage.Size.Width), ConvertToPoints(brushImage.Size.Height));
                        if (pdfPath != null)
                            rect = new SizeF((pdfPath.Points[1].X - pdfPath.Points[0].X), (pdfPath.Points[2].Y - pdfPath.Points[0].Y));
                        PdfTilingBrush imageBrush = new PdfTilingBrush(imageSize);

                        if (newBrush.TileMode != TileMode.None)
                        {
                            RectangleF viewPort = RectangleF.Empty;
                            RectangleF viewBox = RectangleF.Empty;

                            PdfImage pdfImage = PdfImage.FromImage(brushImage);

                            if (!string.IsNullOrEmpty(newBrush.Viewport))
                                viewPort = StringToRectangleF(newBrush.Viewport);

                            if (!string.IsNullOrEmpty(newBrush.Viewbox))
                                viewBox = StringToRectangleF(newBrush.Viewbox);

                            if (viewPort.Width > .75f && viewPort.Height > .75f)
                            {
                                imageBrush = new PdfTilingBrush(viewBox, m_page);

                                imageBrush.Graphics.ScaleTransform((viewPort.Width / viewBox.Width), (viewPort.Height / viewBox.Height));
                                viewPort.X -= viewBox.X;
                                viewPort.Y -= viewBox.Y;
                                viewPort.Width = pdfImage.Width;
                                viewPort.Height = pdfImage.Height;
                                
                                imageBrush.Graphics.DrawImage(pdfImage, viewBox);

                                PdfPen tempPen = new PdfPen(imageBrush, (float)path.StrokeThickness);
                                Graphics.DrawRectangle(tempPen, new RectangleF(pdfPath.Points[0], rect));

                            }
                            else
                                Graphics.DrawImage(pdfImage, new RectangleF(pdfPath.Points[0], rect));
                            brushImage = null;
                        }
                    }
                    catch (Exception)
                    {

                    }
               }
                else if (path.PathStroke.Item is SolidColorBrush)
                {
                    PdfBrush tempBrush = new PdfSolidBrush(ColorTranslator.FromHtml(
                     (path.PathStroke.Item as SolidColorBrush).Color));

                    pen = new PdfPen(tempBrush, (float)path.StrokeThickness);
                }
                else if (path.PathStroke.Item is RadialGradientBrush)
                {
                    RadialGradientBrush xpsRadialBrush = path.PathStroke.Item as RadialGradientBrush;
                    PdfBrush tempBrush = ReadRadialGradientBrush(xpsRadialBrush);

                    pen = new PdfPen(tempBrush, (float)path.StrokeThickness);
                }
                else if (path.PathStroke.Item is VisualBrush)
                {
                    VisualBrush xpsVisualBrush = path.PathStroke.Item as VisualBrush;

                    if (xpsVisualBrush.VisualBrushVisual.Item is Glyphs)
                    {
                        Glyphs glyphs = xpsVisualBrush.VisualBrushVisual.Item as Glyphs;
                        rect = new SizeF((pdfPath.Points[1].X - pdfPath.Points[0].X), (pdfPath.Points[2].Y - pdfPath.Points[0].Y));

                        RectangleF viewPort = StringToRectangleF(xpsVisualBrush.Viewport);
                        RectangleF viewBox = StringToRectangleF(xpsVisualBrush.Viewbox);
                        PdfFont pdfFont = GetFont(glyphs);
                        PdfTilingBrush tilingBrush = new PdfTilingBrush(new SizeF(viewPort.Width, viewPort.Height));
                        PdfBrush tempBrush = new PdfSolidBrush(ColorTranslator.FromHtml(glyphs.Fill));

                        tilingBrush.Graphics.ScaleTransform((float)(viewPort.Width / viewBox.Width), (float)(viewPort.Height / viewBox.Height));
                        tilingBrush.Graphics.DrawString(glyphs.UnicodeString, pdfFont, tempBrush, new PointF());

                        PdfPen tempPen = new PdfPen(tilingBrush, (float)path.StrokeThickness);
                        Graphics.DrawRectangle(tempPen, new RectangleF(pdfPath.Points[0], rect));
                    }
                    else
                        ReadVisualBrush(xpsVisualBrush);
                }
            }

            //Get Brush
            if (path.Fill != null)
            {
                if (path.Fill.Contains("StaticResource"))
                    path.PathFill = ReadStaticResource(path.Fill) as Brush;
                else if (path.Fill.Contains("icc"))
                    brush = null;
                else
                {
                    Color color = FromHtml(path.Fill);
                    if (path.Fill.Contains("sc#"))
                    {
                        double alpha = color.A < 255 ? (color.A / 256.0) : 1;
                        if (alpha < 1)
                            Graphics.SetTransparency((float)alpha);
                    }
                    if (color.A != 0)
                        brush = new PdfSolidBrush(color);
                }
            }

            if (path.PathFill is Brush)
            {
                if (path.PathFill.Item is ImageBrush)
                {
                    ImageBrush newBrush = (path.PathFill.Item as ImageBrush);
                    if (!String.IsNullOrEmpty(newBrush.Transform))
                        ApplyRenderTransform(newBrush.Transform);

                    try
                    {
                        brushImage = Image.FromStream(m_reader.ReadImage(newBrush.ImageSource));

                        SizeF imageSize = new SizeF(ConvertToPoints(brushImage.Size.Width), ConvertToPoints(brushImage.Size.Height));
                        if (pdfPath != null)
                            rect = new SizeF((pdfPath.Points[1].X - pdfPath.Points[0].X), (pdfPath.Points[2].Y - pdfPath.Points[0].Y));
                        PdfTilingBrush imageBrush = new PdfTilingBrush(imageSize);

                        if (newBrush.TileMode != TileMode.None)
                        {
                            RectangleF viewPort = RectangleF.Empty;
                            RectangleF viewBox = RectangleF.Empty;

                            PdfImage pdfImage = PdfImage.FromImage(brushImage);

                            if (!string.IsNullOrEmpty(newBrush.Viewport))
                                viewPort = StringToRectangleF(newBrush.Viewport);

                            if (!string.IsNullOrEmpty(newBrush.Viewbox))
                                viewBox = StringToRectangleF(newBrush.Viewbox);

                            if (Graphics.Matrix.Matrix.Elements[0] < 1)
                                viewPort.Size = new SizeF(viewPort.Width * Graphics.Matrix.Matrix.Elements[0], viewPort.Height * Graphics.Matrix.Matrix.Elements[3]);

                            if (viewPort.Width > .75f && viewPort.Height > .75f)
                            {
                                imageBrush = new PdfTilingBrush(viewPort, m_page);

                                imageBrush.Graphics.ScaleTransform((viewPort.Width / viewBox.Width), (viewPort.Height / viewBox.Height));
                                viewPort.X -= viewBox.X;
                                viewPort.Y -= viewBox.Y;
                                viewPort.Width = pdfImage.Width;
                                viewPort.Height = pdfImage.Height;
                                
                                imageBrush.Graphics.DrawImage(pdfImage, viewPort);

                                Graphics.DrawRectangle(imageBrush, new RectangleF(pdfPath.Points[0], rect));
                            }
                            else
                                Graphics.DrawImage(pdfImage, new RectangleF(pdfPath.Points[0], rect));
                            brushImage = null;
                        }
                    }
                    catch (Exception)
                    {
                        
                    }
                }
                else if (path.PathFill.Item is SolidColorBrush)
                {
                    brush = new PdfSolidBrush(ColorTranslator.FromHtml(
                        (path.PathFill.Item as SolidColorBrush).Color));
                }
                else if (path.PathFill.Item is LinearGradientBrush)
                {
                    LinearGradientBrush gradientBrush = (path.PathFill.Item as LinearGradientBrush);
                    brush = ReadLinearGradientBrush(gradientBrush);
                }
                else if (path.PathFill.Item is RadialGradientBrush)
                {
                    RadialGradientBrush xpsRadialBrush = path.PathFill.Item as RadialGradientBrush;
                    brush = ReadRadialGradientBrush(xpsRadialBrush);
                }
                else if (path.PathFill.Item is VisualBrush)
                {
                    VisualBrush visualBrush = path.PathFill.Item as VisualBrush;

                    PdfGraphicsState graphicsState = Graphics.Save();
                    RectangleF viewPort = RectangleF.Empty;
                    RectangleF viewBox = RectangleF.Empty;

                    if (!string.IsNullOrEmpty(visualBrush.Viewport))
                        viewPort = StringToRectangleF(visualBrush.Viewport);

                    if (!string.IsNullOrEmpty(visualBrush.Viewbox))
                        viewBox = StringToRectangleF(visualBrush.Viewbox);

                    Graphics.ScaleTransform((viewPort.Width / viewBox.Width), (viewPort.Height / viewBox.Height));
                    viewPort.X -= viewBox.X;
                    viewPort.Y -= viewBox.Y;
                    Graphics.TranslateTransform(viewPort.X, viewPort.Y);
                    ReadVisualBrush(path.PathFill.Item as VisualBrush);
                    Graphics.Restore(graphicsState);
                }
            }

            //Get Opacity

            if (path.Opacity != null && path.Opacity < 1)
            {
                Graphics.SetTransparency((float)path.Opacity);
            }

            if (path.Clip != null)
            {
                PdfPath clipPath = GetPathFromGeometry(path.Clip);
                Graphics.SetClip(clipPath);
            }

            // Line Join type
            if (path.StrokeLineJoin != null)
            {
                if (pen != null)
                    pen.LineJoin = (PdfLineJoin)Enum.Parse(typeof(PdfLineJoin), Enum.GetName(typeof(LineJoin), path.StrokeLineJoin));
            }

            // Line End Caps

            if (path.StrokeStartLineCap != null)
            {
                if (pen != null && (path.StrokeStartLineCap != LineCap.Triangle))
                    pen.LineCap = (PdfLineCap)Enum.Parse(typeof(PdfLineCap), Enum.GetName(typeof(LineCap), path.StrokeStartLineCap));
            }
            else if (path.StrokeEndLineCap != null && (path.StrokeEndLineCap != LineCap.Triangle))
            {
                if (pen != null)
                    pen.LineCap = (PdfLineCap)Enum.Parse(typeof(PdfLineCap), Enum.GetName(typeof(LineCap), path.StrokeEndLineCap));
            }
            //Draw Path

            if (path.FixedPageNavigateUri != null && currentMatrix != null)
            {
                PdfUriAnnotation annotation;
                if (pdfPath != null)
                {
                    rect = new SizeF((pdfPath.Points[2].X - pdfPath.Points[0].X) * currentMatrix.Matrix.Elements[0], (pdfPath.Points[2].Y - pdfPath.Points[0].Y) * currentMatrix.Matrix.Elements[3]);
                    annotation = new PdfUriAnnotation(new RectangleF(new PointF(pdfPath.Points[0].X * currentMatrix.Matrix.Elements[0], pdfPath.Points[0].Y * currentMatrix.Matrix.Elements[3]), rect));
                }
                else
                    annotation = new PdfUriAnnotation(new RectangleF());
                annotation.Uri = path.FixedPageNavigateUri;
                annotation.Color = new PdfColor(Color.Transparent);
                m_page.Annotations.Add(annotation);
            }
            else
            {
                if (path.PathFill != null && path.PathFill.Item is ImageBrush && brushImage != null && pdfPath.PointCount > 0)
                {
                    if (rect.Height < 0)
                    {
                        rect.Height = -rect.Height;
                        Graphics.DrawImage(PdfImage.FromImage(brushImage), new RectangleF(new PointF(pdfPath.Points[0].X, pdfPath.Points[0].Y - rect.Height), rect));
                    } 
                    else
                        Graphics.DrawImage(PdfImage.FromImage(brushImage), new RectangleF(new PointF(pdfPath.Points[0].X, pdfPath.Points[0].Y), rect));
                    if (pen != null)
                        Graphics.DrawPath(pen, pdfPath);
                }
                else
                    Graphics.DrawPath(pen, brush, pdfPath);
            }

            Graphics.Restore(gs);
            if (brushImage != null)
                brushImage.Dispose();
        }

        /// <summary>
        /// Read and draw contents of VisualBrush.
        /// </summary>
        private void ReadVisualBrush(VisualBrush xpsVisualBrush)
        {
            if (xpsVisualBrush.Key != null && xpsVisualBrush.Key == "EmptyBrush")
                return;

            // Apply transform
            if (xpsVisualBrush.Transform != null)
                ApplyRenderTransform(xpsVisualBrush.Transform);

            object brush = null;

            if (xpsVisualBrush.VisualBrushVisual != null)
                brush = xpsVisualBrush.VisualBrushVisual.Item;
            else if (!String.IsNullOrEmpty(xpsVisualBrush.Visual) && xpsVisualBrush.Visual.Contains("StaticResource"))
                brush = ReadStaticResource(xpsVisualBrush.Visual);

            if(brush != null)
            {
                if (brush is Canvas)
                    DrawCanvas(brush as Canvas);
                else if (brush is Glyphs)
                    DrawGlyphs(brush as Glyphs);
                else if (brush is Path)
                    DrawPath(brush as Path);
            }
        }

        /// <summary>
        /// Convert from XPS linear gradient brush to PDF.
        /// </summary>
        /// <param name="gradientBrush">LinearGradientBrush</param>
        /// <returns>PdfLinearGradientBrush</returns>
        private PdfLinearGradientBrush ReadLinearGradientBrush(LinearGradientBrush gradientBrush)
        {
            PointF point;

            //Get Start point
            PathDataReader reader = new PathDataReader(gradientBrush.StartPoint);
            reader.TryReadPoint(out point);
            PointF startPoint = new PointF(ConvertToPoints(point.X), ConvertToPoints(point.Y));

            //Get end point
            reader = new PathDataReader(gradientBrush.EndPoint);
            reader.TryReadPoint(out point);
            PointF endPoint = new PointF(ConvertToPoints(point.X), ConvertToPoints(point.Y));

            List<object> offSet;
            List<PdfColor> colors;

            //Get Colors
            PreProcessGradientStops(gradientBrush.LinearGradientBrushGradientStops, out offSet, out colors);

            float[] pos = new float[offSet.Count];
            int i = 0;
            foreach (object obj in offSet)
            {
                pos[i] = ParseFloat(obj.ToString());
                i++;
            }

            PdfLinearGradientBrush linearBrush = new PdfLinearGradientBrush(startPoint, endPoint, colors[0], colors[colors.Count - 1]);

            if (gradientBrush.SpreadMethod == SpreadMethod.Pad)
                linearBrush.Extend = PdfExtend.Both;
            else if (gradientBrush.SpreadMethod == SpreadMethod.Repeat)
                linearBrush.Extend = PdfExtend.None;
            else if (gradientBrush.SpreadMethod == SpreadMethod.Reflect)
                linearBrush.Extend = PdfExtend.None;

            PdfColorBlend blend = new PdfColorBlend(linearBrush);
            blend.Positions = pos;
            blend.Colors = colors.ToArray();
            linearBrush.InterpolationColors = blend;

            return linearBrush;
        }

        /// <summary>
        /// Convert from XPS radial gradient brush to PDF.
        /// </summary>
        /// <param name="xpsRadialBrush">RadialGradientBrush</param>
        /// <returns>PdfRadialGradientBrush</returns>
        private PdfRadialGradientBrush ReadRadialGradientBrush(RadialGradientBrush xpsRadialBrush)
        {
            float radiusStart = (float)xpsRadialBrush.RadiusX;
            float radiusEnd = (float)xpsRadialBrush.RadiusY;

            PointF point;

            //Get Start point
            PathDataReader reader = new PathDataReader(xpsRadialBrush.GradientOrigin);
            reader.TryReadPoint(out point);
            PointF gradientOrigin = new PointF(ConvertToPoints(point.X), ConvertToPoints(point.Y));

            //Get end point
            reader = new PathDataReader(xpsRadialBrush.Center);
            reader.TryReadPoint(out point);
            PointF center = new PointF(ConvertToPoints(point.X), ConvertToPoints(point.Y));

            List<object> offSet;
            List<PdfColor> colors;

            //Get Colors
            PreProcessGradientStops(xpsRadialBrush.RadialGradientBrushGradientStops, out offSet, out colors);

            float[] pos = new float[offSet.Count];
            int i = 0;
            foreach (object obj in offSet)
            {
                pos[i] = ParseFloat(obj.ToString());
                i++;
            }

            // Create a new radial gradient brush.
            PdfRadialGradientBrush radialBrush = new PdfRadialGradientBrush(center, radiusStart, gradientOrigin, radiusEnd, colors[0], colors[colors.Count - 1]);

            if (xpsRadialBrush.SpreadMethod == SpreadMethod.Pad)
                radialBrush.Extend = PdfExtend.Both;
            else if (xpsRadialBrush.SpreadMethod == SpreadMethod.Repeat)
                radialBrush.Extend = PdfExtend.None;
            else if (xpsRadialBrush.SpreadMethod == SpreadMethod.Reflect)
                radialBrush.Extend = PdfExtend.None;

            PdfColorBlend colorBlend = new PdfColorBlend(radialBrush);
            colorBlend.Positions = pos;
            colorBlend.Colors = colors.ToArray();
            radialBrush.InterpolationColors = colorBlend;
         
            return radialBrush;
        }

        /// <summary>
        /// Converts XPS pathgeometry to PdfPath.
        /// </summary>
        /// <param name="xpsPathGeometry">PathGeometry</param>
        /// <returns>PdfPath</returns>
        private PdfPath GetPathFromPathGeometry(PathGeometry xpsPathGeometry)
        {
            PdfPath pdfPath = null;

            if (xpsPathGeometry == null)
                return pdfPath;

            if (xpsPathGeometry.Figures != null)
                pdfPath = GetPathFromGeometry(xpsPathGeometry.Figures);
            else if (xpsPathGeometry.PathFigure != null)
            {
                PathFigure[] xpsPathFigures = xpsPathGeometry.PathFigure;
                GraphicsPath gp = new GraphicsPath();
                for (int i = 0; i < xpsPathFigures.Length; i++)
                {
                    PathFigure xpsPathFigure = xpsPathFigures[i];
                    PointF location = StringToPointF(xpsPathFigure.StartPoint);
                    PointF startPoint = location;

                    foreach (object obj in xpsPathFigure.Items)
                    {
                        if (obj is PolyLineSegment)
                        {
                            PolyLineSegment polyLineSegment = obj as PolyLineSegment;
                            string[] points = polyLineSegment.Points.Split(' ');
                            
                            foreach (string pts in points)
                            {
                                PointF s = StringToPointF(pts);
                                gp.AddLine(startPoint, s);
                                startPoint = s;
                            }
                        }
                        else if (obj is ArcSegment)
                        {
                            ArcSegment arcSegment = obj as ArcSegment;
                            PointF endPoint = StringToPointF(arcSegment.Point);
                            PointF size = StringToPointF(arcSegment.Size);
                            bool arc = false;
                            if (arcSegment.SweepDirection == SweepDirection.Clockwise)
                                arc = false;
                            else if(arcSegment.SweepDirection == SweepDirection.Counterclockwise)
                                arc = true;

                            List<PointF> points = ComputeArc(startPoint, endPoint, size.X, size.Y, (float)arcSegment.RotationAngle, arcSegment.IsLargeArc, arc);

                            if (points.Count == 0)
                            {
                                if (arcSegment.IsStroked)
                                    gp.AddLine(location, endPoint);
                                location = endPoint;
                            }
                            else if (points.Count > 0)
                            {
                                if (arcSegment.IsStroked)
                                    gp.AddBeziers(points.ToArray());
                                startPoint = points[points.Count - 1];
                                points.Clear();
                            }
                        }
                        else if (obj is PolyQuadraticBezierSegment)
                        {
                            PolyQuadraticBezierSegment quadraticBS = obj as PolyQuadraticBezierSegment;
                        }
                        else if (obj is PolyBezierSegment)
                        {
                            PolyBezierSegment polyBS = obj as PolyBezierSegment;

                            List<PointF> bezierPoints = new List<PointF>();

                            string[] points = polyBS.Points.Split(' ');
                            if (startPoint != PointF.Empty)
                                bezierPoints.Add(startPoint);
                            else
                                bezierPoints.Add(location);
                            
                            foreach (string pts in points)
                                bezierPoints.Add(StringToPointF(pts));

                            if (polyBS.IsStroked)
                                gp.AddBeziers(bezierPoints.ToArray());
                            
                            startPoint = bezierPoints[bezierPoints.Count - 1];
                            bezierPoints.Clear();
                        }
                    }

                    if (xpsPathFigure.IsClosed)
                    {
                        gp.CloseFigure();
                        gp.StartFigure();
                        startPoint = PointF.Empty;
                    }
                }

                //gp.CloseAllFigures();

                if (gp.PointCount > 0)
                {
                    pdfPath = new PdfPath(gp.PathPoints, gp.PathTypes);
                    pdfPath.FillMode = (gp.FillMode == FillMode.Alternate) ? PdfFillMode.Alternate : PdfFillMode.Winding;
                    pdfPath.CloseAllFigures();
                }
                else if(pdfPath == null)
                    pdfPath = new PdfPath();
            }

            return pdfPath;
        }

        /// <summary>
        /// Convert string values to PointF
        /// </summary>
        /// <param name="point">String data.</param>
        /// <returns>Data in PointF</returns>
        private PointF StringToPointF(string point)
        {
            string[] pts = point.Split(',');
            PointF pt = new PointF(ParseFloat(pts[0]), ParseFloat(pts[1]));

            return ConvertToPoints(pt);
        }

        /// <summary>
        /// Convert string values to RectangleF
        /// </summary>
        /// <param name="rect">String data.</param>
        /// <returns>Data in RectangleF.</returns>
        private RectangleF StringToRectangleF(string rect)
        {
            string[] val = rect.Split(new char[] { ',' });
            PointF loc = new PointF(ParseFloat(val[0]), ParseFloat(val[1]));
            PointF sz = new PointF(ParseFloat(val[2]), ParseFloat(val[3]));

            loc = ConvertToPoints(loc);
            sz = ConvertToPoints(sz);

            SizeF size = new SizeF(sz.X, sz.Y);

            return new RectangleF(loc, size);
        }

        /// <summary>
        /// Processes Gradient stops.
        /// </summary>
        /// <param name="stops">Gradient stops read from file.</param>
        /// <param name="offSet">Offsets</param>
        /// <param name="colors">Colors</param>
        private void PreProcessGradientStops(GradientStop[] stops, out List<object> offSet, out List<PdfColor> colors)
        {
            offSet = new List<object>();
            colors = new List<PdfColor>();

            float val = -1; PdfColor pdfCol = PdfColor.Empty;

            foreach (GradientStop col in stops)
            {
                val = (float)col.Offset;
                pdfCol = new PdfColor(FromHtml(col.Color));

                int firstIndex = offSet.IndexOf(val);
                int lastIndex = offSet.LastIndexOf(val);

                if (firstIndex == -1 && firstIndex == lastIndex)
                {
                    offSet.Add(val);
                    offSet.Sort();

                    firstIndex = offSet.IndexOf(val);
                    colors.Insert(firstIndex, pdfCol);
                }
                else if(firstIndex > -1 && (firstIndex == lastIndex))
                {
                    offSet.Add(val);
                    offSet.Sort();

                    lastIndex = offSet.LastIndexOf(val);
                    colors.Insert(lastIndex, pdfCol);
                }
                else if (firstIndex > -1 && (firstIndex != lastIndex))
                {
                    lastIndex = offSet.LastIndexOf(val);
                    offSet.RemoveAt(lastIndex);
                    colors.RemoveAt(lastIndex);

                    offSet.Insert(lastIndex, val);
                    colors.Insert(lastIndex, pdfCol);
                }
            }

            if (!offSet.Contains(0.0f))
            {
                Object least = offSet.FindLast(LowerLeastGradient);
                Object most = offSet.Find(LowerMostGradient);

                if ((float)offSet[0] > 0.0f)
                {
                    offSet.Insert(0, 0.0f);
                    pdfCol = colors[0];
                    colors.Insert(0, pdfCol);
                }

                least = offSet.FindLast(LowerLeastGradient);
                most = offSet.Find(LowerMostGradient);

                if (least != null && most != null)
                {
                    PdfColor leastCol = colors[offSet.IndexOf(least)];
                    PdfColor mostCol = colors[offSet.IndexOf(most)];
                    
                    PdfColor col = colors[offSet.IndexOf(least)]; // Need to interpolate least and most colors here.
                    col = PdfBlendBase.Interpolate(0.5, leastCol, mostCol, PdfColorSpace.RGB);

                    while (least != null)
                    {
                        int index = offSet.IndexOf(least);
                        offSet.RemoveAt(index);
                        colors.RemoveAt(index);

                        least = offSet.Find(LowerLeastGradient);
                    }

                    offSet.Insert(0, 0.0f);
                    colors.Insert(0, col);
                }

                least = offSet.FindLast(LowerLeastGradient);
                most = offSet.Find(LowerMostGradient);

                if (least != null && most == null)
                {
                    PdfColor col = colors[colors.Count - 1];

                    while (least != null)
                    {
                        int index = offSet.IndexOf(least);
                        offSet.RemoveAt(index);
                        colors.RemoveAt(index);

                        least = offSet.Find(LowerLeastGradient);
                    }

                    offSet.Insert(0, 0.0f);
                    colors.Insert(0, col);
                }
            }

            if (!offSet.Contains(1.0f))
            {
                Object least = offSet.FindLast(HigherLeastGradient);
                Object most = offSet.Find(HigherMostGradient);

                if ((float)offSet[offSet.Count - 1] < 1.0f)
                {
                    int count = offSet.Count;
                    offSet.Add(1.0f);
                    pdfCol = colors[count - 1];
                    colors.Add(pdfCol);
                }

                least = offSet.FindLast(HigherLeastGradient);
                most = offSet.Find(HigherMostGradient);

                if (most != null && least != null)
                {
                    PdfColor leastCol = colors[offSet.IndexOf(least)];
                    PdfColor mostCol = colors[offSet.IndexOf(most)];

                    PdfColor col = colors[offSet.IndexOf(most)]; // Need to interpolate least and most colors here.
                    col = PdfBlendBase.Interpolate(.5, leastCol, mostCol, PdfColorSpace.RGB);

                    while (most != null)
                    {
                        int index = offSet.IndexOf(most);
                        offSet.RemoveAt(index);
                        colors.RemoveAt(index);

                        most = offSet.Find(HigherMostGradient);
                    }

                    offSet.Add(1.0f);
                    colors.Add(col);
                }

                least = offSet.FindLast(HigherLeastGradient);
                most = offSet.Find(HigherMostGradient);
                
                if (most != null && least == null)
                {
                    PdfColor col = colors[0];

                    while (most != null)
                    {
                        int index = offSet.IndexOf(most);
                        offSet.RemoveAt(index);
                        colors.RemoveAt(index);

                        most = offSet.Find(HigherMostGradient);
                    }

                    offSet.Add(1.0f);
                    colors.Add(col);
                }
            }
        }

        /// <summary>
        /// Returns the lowest value based on 0.0f
        /// </summary>
        private static bool LowerLeastGradient(object p)
        {
            float val = (float)p;
            if (val < 0.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the first value greater than 0.0f
        /// </summary>
        private static bool LowerMostGradient(object p)
        {
            float val = (float)p;
            if (val > 0.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the highest value based on 1.0f
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private static bool HigherLeastGradient(object p)
        {
            float val = (float)p;
            if (val < 1.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the top most value based on 1.0f
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private static bool HigherMostGradient(object p)
        {
            float val = (float)p;
            if (val > 1.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Reads static resource based on hierarchy of elements.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>Object containing the resource.</returns>
        private object ReadStaticResource(string resourceName)
        {
            resourceName = resourceName.Trim(new char[]{'{','}'}).Replace("StaticResource ",String.Empty);
            object resourceObj = null;
            int index = m_page.Section.Parent.IndexOf(m_page.Section);
            object[] collection = null;

            if (m_canvas != null)
            {
                collection = GetResourceCollection(m_canvas.CanvasResources);
                resourceObj = GetResource(collection, resourceName);
            }

            if (m_canvas != null && resourceObj == null)
            {
                Canvas canvasParent = m_canvas;
                while (canvasParent.m_parent != null)
                {
                    canvasParent = canvasParent.m_parent as Canvas;
                    collection = GetResourceCollection(canvasParent.CanvasResources);
                    resourceObj = GetResource(collection, resourceName);
                    if (resourceObj != null)
                        break;
                }
            }

            if (collection == null && m_reader.Pages[index].FixedPageResources != null)
            {
                collection = GetResourceCollection(m_reader.Pages[index].FixedPageResources);
                resourceObj = GetResource(collection, resourceName);
            }

            //if (resourceObj == null)
            //    throw new Exception("Resource cannot be found!");

            return resourceObj;
        }

        /// <summary>
        /// Reads resource dictionary from the document.
        /// </summary>
        private object[] GetResourceCollection(Resources resources)
        {
            if (resources != null)
            {
                ResourceDictionary dict = resources.ResourceDictionary;
                if (dict != null && dict.Items == null && dict.Source != null && dict.Source != String.Empty)
                {
                    Stream resourceStream = m_reader.ReadResource(dict.Source);
                    TextReader reader = new StreamReader(resourceStream);
                    XmlSerializer serializer = new XmlSerializer(typeof(ResourceDictionary));

                    dict = (ResourceDictionary)serializer.Deserialize(reader);
                    resources.ResourceDictionary = dict;
                }
            }

            return (resources != null && resources.ResourceDictionary != null) ? resources.ResourceDictionary.Items : null;
        }

        /// <summary>
        /// Returns resource from the collection.
        /// </summary>
        private object GetResource(object[] collection, string resourceName)
        {
            object obj = null;
            Brush brush = new Brush();
            PathGeometry pathGeometry = new PathGeometry();

            if (collection != null && collection.Length > 0)
            {
                for (int i = 0; i < collection.Length; i++)
                {
                    obj = collection[i];
                    if (obj is ImageBrush && (obj as ImageBrush).Key == resourceName)
                    {
                        brush.Item = obj;
                        return brush;
                    }
                    if (obj is VisualBrush && (obj as VisualBrush).Key == resourceName)
                    {
                        brush.Item = obj;
                        return brush;
                    }
                    if (obj is LinearGradientBrush && (obj as LinearGradientBrush).Key == resourceName)
                    {
                        brush.Item = obj;
                        return brush;
                    }
                    if (obj is RadialGradientBrush && (obj as RadialGradientBrush).Key == resourceName)
                    {
                        brush.Item = obj;
                        return brush;
                    }
                    if (obj is PathGeometry && (obj as PathGeometry).Key == resourceName)
                    {
                        return (obj as PathGeometry);
                    }
                    if (obj is MatrixTransform && (obj as MatrixTransform).Key == resourceName)
                    {
                        return (obj as MatrixTransform);
                    }
                    if (obj is VisualBrush && (obj as VisualBrush).Key == resourceName)
                    {
                        return (obj as VisualBrush);
                    }
                }
            }

            return obj;
        }

        /// <summary>
        /// Converts the canvas graphics to PDF graphics.
        /// </summary>
        /// <param name="canvas">XPS Canvas</param>
        public void DrawCanvas(Canvas canvas)
        {
            PdfGraphicsState gs = Graphics.Save();
            InitializeCanvas(canvas);
            if (m_canvas != null)
                canvas.m_parent = m_canvas;
            m_canvas = canvas;

            if (canvas.Items != null)
            {
                foreach (object item in canvas.Items)
                {
                    if (item is Canvas)
                        DrawCanvas((Canvas)item);
                    else if (item is Path)
                        DrawPath((Path)item);
                    else if (item is Glyphs)
                        DrawGlyphs((Glyphs)item);
                    else
                        throw new NotImplementedException(item.GetType().ToString());
                }
            }
            m_canvas = (Canvas)m_canvas.m_parent;
            Graphics.Restore(gs);
        }

        /// <summary>
        /// Intializes the XPS canvas
        /// </summary>
        /// <param name="canvas"> XPS canvas</param>
        public void InitializeCanvas(Canvas canvas)
        {
            if (canvas == null)
                throw new ArgumentNullException("Canvas");

            if (canvas.CanvasRenderTransform != null)
                ApplyRenderTransform(canvas.CanvasRenderTransform.MatrixTransform.Matrix);

            if (canvas.RenderTransform != null)
                ApplyRenderTransform(canvas.RenderTransform);


            if (canvas.Clip != null)
            {
                PdfPath path = GetPathFromGeometry(canvas.Clip);
                Graphics.SetClip(path);
            }
            return;
        }

        /// <summary>
        /// Applies the XPS transformation.
        /// </summary>
        /// <param name="data">Transformation matrix</param>
        /// <param name="graphics">Grahics where the transformation is to be done.</param>
        public void ApplyRenderTransform(string data, PdfGraphics graphics)
        {
            Matrix matrix = ReadMatrix(data);
            PdfTransformationMatrix transformationMatrix = PrepareMatrix(matrix);
            Graphics.MultiplyTransform(transformationMatrix);
        }
        /// <summary>
        /// Applies the Graphics transformation
        /// </summary>
        /// <param name="data">Transformation matrix</param>
        public void ApplyRenderTransform(string data)
        {
            Matrix matrix = null;

            if (data.Contains("StaticResource"))
            {
                matrix = ReadMatrix((ReadStaticResource(data) as MatrixTransform).Matrix);
            }
            else
                matrix = ReadMatrix(data);

            PdfTransformationMatrix transformationMatrix = PrepareMatrix(matrix);
            Graphics.Matrix.Multiply(transformationMatrix);
            Graphics.MultiplyTransform(transformationMatrix);
            currentMatrix = transformationMatrix;
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Prepares a matrix to PDF.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>A properly prepared PdfTransformationMatrix class instance.</returns>
        private PdfTransformationMatrix PrepareMatrix(Matrix matrix)
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
        /// Converts the matrix string to a System.Drawing.Drawing2D.Matrix object
        /// </summary>
        /// <param name="data"> matrix string</param>
        /// <returns>System.Drawing.Drawing2D.Matrix object</returns>
        private Matrix ReadMatrix(string data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            float[] elements = new float[6];

            string[] items = data.Split(m_commaSeparator);

            for (int i = 0; i < elements.Length; i++)
            {
                elements[i] = ParseFloat(items[i]);
            }

            Matrix matrix = new Matrix(elements[0], elements[1], elements[2],
               elements[3], ConvertToPoints(elements[4]), ConvertToPoints(elements[5]));

            return matrix;
        }

        /// <summary>
        /// Converts the Glyph font to PdfFont
        /// </summary>
        /// <param name="glyphs">XPS Glyph object</param>
        /// <returns>PdfFont</returns>
        private PdfFont GetFont(Glyphs glyphs)
        {
            Font originalFont = null;
            PdfTrueTypeFont font = null;
            float fontSize = ConvertToPoints(glyphs.FontRenderingEmSize);
            FontStyle style = GetDeviceFontStyle(glyphs.StyleSimulations);

            bool isUnicode = (glyphs.UnicodeString == null) ? false : PdfString.IsUnicode(glyphs.UnicodeString);
            List<FontFamily> fontList = new List<FontFamily>(FontFamily.Families);

            if (glyphs.FontUri != null)
            {
                if (m_reader.Fonts.ContainsKey(glyphs.FontUri))
                {
                    font = m_reader.Fonts[glyphs.FontUri];
                    font = new PdfTrueTypeFont(font, fontSize);
                }
                else
                {
                    Stream fontStream = (MemoryStream)m_reader.ReadFont(glyphs.FontUri);
                    fontStream.Position = 0;

                    BinaryReader reader = new BinaryReader(fontStream);
                    TtfReader fontReader = new TtfReader(reader);

                    //bool isSystemFont = false;

                    //foreach (FontFamily family in fontList)
                    //{
                    //    if (family.Name == fontReader.Metrics.FontFamily)
                    //    {
                    //        isSystemFont = true;
                    //        break;
                    //    }
                    //}

                    //if (!isSystemFont)
                    {
                        fontStream.Position = 0;
                        font = new PdfTrueTypeFont(fontStream, fontSize);
                        m_reader.Fonts.Add(glyphs.FontUri, font);
                        return font;
                    }

                    //bool isBold = fontReader.Metrics.IsBold;
                    //bool isItalic = fontReader.Metrics.IsItalic;

                    //fontStream.Position = 0;
                    //int fontStreamLength = (int)fontStream.Length;
                    //IntPtr data = Marshal.AllocCoTaskMem(fontStreamLength);

                    //byte[] fontData = new byte[fontStreamLength];
                    //fontStream.Read(fontData, 0, fontStreamLength);
                    //Marshal.Copy(fontData, 0, data, fontStreamLength);

                    //PrivateFonts.AddMemoryFont(data, fontStreamLength);
                    //Marshal.FreeCoTaskMem(data);

                    //FontStyle originalStyle = FontStyle.Regular;
                    //if (isBold)
                    //    originalStyle |= FontStyle.Bold;
                    //if (isItalic)
                    //    originalStyle |= FontStyle.Italic;

                    //int index = Find(fontReader.Metrics.FontFamily);
                    //originalFont = new Font(PrivateFonts.Families[index], 1f, originalStyle);
                }

                //if (originalFont != null)
                //{
                //    style |= originalFont.Style;

                //    if (glyphs.StyleSimulations == StyleSimulations.None && originalFont.Style != FontStyle.Regular)
                //    {
                //        // font = new PdfTrueTypeFont(originalFont,fontSize);
                //        font = new PdfTrueTypeFont(
                //            new Font(originalFont, originalFont.Style), fontSize, true);
                //    }
                //    else
                //    {
                //        font = new PdfTrueTypeFont(
                //            new Font(originalFont, style), fontSize, true);
                //    }

                //    m_reader.Fonts.Add(glyphs.FontUri, font);
                //}
            }
            else if (glyphs.DeviceFontName != null)
            {
                Font deviceFont = new Font(glyphs.DeviceFontName, fontSize,
                    GetDeviceFontStyle(glyphs.StyleSimulations));

                font = new PdfTrueTypeFont(deviceFont, true);
            }

            return font;
        }

        ///// <summary>
        ///// Gets the Ascent from the Font.
        ///// </summary>
        ///// <param name="pdfFont">Currently used PdfFont</param>
        ///// <param name="glyphs">XpS Glyphs</param>
        ///// <returns>Font Ascent</returns>
        //private float GetFontAscent(PdfFont pdfFont, Glyphs glyphs)
        //{
        //    Font font;
        //    FontFamily fontFamily;
        //    FontStyle fontStyle = GetFontStyle(pdfFont.Style);// GetDeviceFontStyle(glyphs.StyleSimulations);

        //    //if (SystemFonts.GetFontByName(pdfFont.Name) == null)
        //    if (glyphs.FontUri != null)
        //    {
        //        font = m_reader.Fonts[glyphs.FontUri];
        //        fontFamily = font.FontFamily;

        //        //if (glyphs.StyleSimulations == StyleSimulations.None && font.Style != FontStyle.Regular)
        //        //    fontStyle = font.Style;

        //        font = new Font(fontFamily, (float)glyphs.FontRenderingEmSize, fontStyle, GraphicsUnit.Pixel);
        //        fontFamily = font.FontFamily;
        //    }
        //    else
        //    {
        //        fontFamily = new FontFamily(pdfFont.Name);
        //        font = new Font(fontFamily, (float)glyphs.FontRenderingEmSize, fontStyle, GraphicsUnit.Pixel);
        //    }
                 
        //    float ascent = fontFamily.GetCellAscent(fontStyle);

        //    float ascentPixel =
        //       font.Size * ascent / fontFamily.GetEmHeight(fontStyle);

        //    font.Dispose();
        //    //fontFamily.Dispose();
        //    return ConvertToPoints(ascentPixel);
        //}

        /// <summary>
        /// Gets the SolidColorBrush form the color string
        /// </summary>
        /// <param name="color">Color string</param>
        /// <returns>PdfBrush</returns>
        private PdfBrush GetSolidBrush(string color)
        {
            if (color == null)
                return null;

            if (color.Contains("StaticResource"))
            {
                PdfBrush pdfBrush = PdfBrushes.Transparent;
                Brush brush = ReadStaticResource(color) as Brush;

                if (brush.Item is SolidColorBrush)
                    pdfBrush = new PdfSolidBrush(FromHtml((brush.Item as SolidColorBrush).Color));
                else if (brush.Item is LinearGradientBrush)
                    pdfBrush = ReadLinearGradientBrush((brush.Item as LinearGradientBrush));
                else if (brush.Item is RadialGradientBrush)
                    pdfBrush = ReadRadialGradientBrush((brush.Item as RadialGradientBrush));

                return pdfBrush;
            }

            return new PdfSolidBrush(
                new PdfColor(
                    ColorTranslator.FromHtml(color)));
        }

        /// <summary>
        /// Gets the font style of the glyph
        /// </summary>
        /// <param name="style">StyleSimulations</param>
        /// <returns>FontStyle</returns>
        private FontStyle GetDeviceFontStyle(StyleSimulations style)
        {
            FontStyle fontStyle = FontStyle.Regular;

            switch (style)
            {
                case StyleSimulations.BoldItalicSimulation:
                    fontStyle = FontStyle.Bold | FontStyle.Italic;
                    break;

                case StyleSimulations.BoldSimulation:
                    fontStyle = FontStyle.Bold;
                    break;

                case StyleSimulations.ItalicSimulation:
                    fontStyle = FontStyle.Italic;
                    break;
            }

            return fontStyle;
        }

        /// <summary>
        /// Gets the font style of the glyph
        /// </summary>
        /// <param name="style">StyleSimulations</param>
        /// <returns>FontStyle</returns>
        private FontStyle GetFontStyle(PdfFontStyle style)
        {
            FontStyle fontStyle = FontStyle.Regular;

            if ((style & PdfFontStyle.Bold) == PdfFontStyle.Bold)
                fontStyle |= FontStyle.Bold;
            if ((style & PdfFontStyle.Italic) == PdfFontStyle.Italic)
                fontStyle |= FontStyle.Italic;
            if ((style & PdfFontStyle.Strikeout) == PdfFontStyle.Strikeout)
                fontStyle |= FontStyle.Strikeout;
            if ((style & PdfFontStyle.Underline) == PdfFontStyle.Underline)
                fontStyle |= FontStyle.Underline;

            return fontStyle;
        }

        /// <summary>
        /// Converts the pixel values to point
        /// </summary>
        /// <param name="value">pixel value</param>
        /// <returns>Points</returns>
        private float ConvertToPoints(double value)
        {
            return m_unitConvertor.ConvertFromPixels((float)value, PdfGraphicsUnit.Point);
        }

        /// <summary>
        /// Convert the pixel coordinates to points
        /// </summary>
        /// <param name="point">pixel coordinates</param>
        /// <returns>Coordinates in Points</returns>
        private PointF ConvertToPoints(PointF point)
        {
            return new PointF(
                ConvertToPoints(point.X),
                ConvertToPoints(point.Y));
        }

        private int Find(string fontName)
        {
            for (int i = 0; i < PrivateFonts.Families.Length; i++)
            {
                if (PrivateFonts.Families[i].Name == fontName)
                    return i;
            }
            return 0;
        }

        private Color FromHtml(string colorString)
        {
            string temp = colorString.Replace("sc#", string.Empty);
            string[] tstroke = temp.Split(',');
            Color color = Color.Empty;

            if (tstroke != null && tstroke.Length > 1)
            {
                int i = 0;
                double[] colorArray = new double[4];
                colorArray[0] = tstroke.Length == 4 ? ParseDouble(tstroke[i++]) : 1.0;
                colorArray[1] = ParseDouble(tstroke[i++]);
                colorArray[2] = ParseDouble(tstroke[i++]);
                colorArray[3] = ParseDouble(tstroke[i++]);

                int a = (int)(colorArray[0] == 1.0 ? 255 : colorArray[0] * 256.0);
                int r = (int)(colorArray[1] == 1.0 ? 255 : colorArray[1] * 256.0);
                int g = (int)(colorArray[2] == 1.0 ? 255 : colorArray[2] * 256.0);
                int b = (int)(colorArray[3] == 1.0 ? 255 : colorArray[3] * 256.0);
                color = Color.FromArgb(a, r, g, b);
            }
            else
                color = ColorTranslator.FromHtml(colorString);

            return color;
        }

        /// <summary>
        /// Converts string to float.
        /// </summary>
        /// <param name="f">Number as string.</param>
        /// <returns>Converted number in float.</returns>
        internal static float ParseFloat(string f)
        {
            float p = 0.0f;
            if (CultureInfo.CurrentCulture.ToString() == "pt-BR")
                f = f.Replace(',', '.');
            if (float.TryParse(f, NumberStyles.Any, CultureInfo.InvariantCulture, out p))
                p = float.Parse(f, CultureInfo.InvariantCulture);
            return p;
        }

        /// <summary>
        /// Converts string to double.
        /// </summary>
        /// <param name="f">Number as string.</param>
        /// <returns>Converted number in double.</returns>
        internal static double ParseDouble(string f)
        {
            double p = 0.0f;
            if (double.TryParse(f, NumberStyles.Any, CultureInfo.InvariantCulture, out p))
                p = double.Parse(f, CultureInfo.InvariantCulture);
            return p;
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        #endregion
    }
}
