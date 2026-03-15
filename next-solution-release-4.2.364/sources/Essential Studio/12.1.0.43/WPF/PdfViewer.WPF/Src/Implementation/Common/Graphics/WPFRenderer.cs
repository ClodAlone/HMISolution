#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.PdfViewer.Base;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Diagnostics;
using Syncfusion.Pdf.Graphics;
using System.Globalization;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Primitives;
using System.IO;
using System.IO.Packaging;
using System.Text.RegularExpressions;

namespace Syncfusion.Windows.PdfViewer
{
    class GraphicsState
    {

    }

    class WPFRenderer
    {
        #region Members
        private List<CffGlyphs> m_glyphDataCollection = new List<CffGlyphs>();
        bool IsRegionCliped = false;
        private Rect textSearchClipRect = new Rect();
        internal static List<TextSearch> txtDictonary = new List<TextSearch>();
        private Dictionary<object, int> m_pageKidsCollection = new Dictionary<object, int>();
        internal System.Drawing.PointF currentTransformationLoc = new System.Drawing.PointF();
        internal System.Drawing.PointF scalingFactor = new System.Drawing.PointF();
        internal List<PageURL> URLDictonary = new List<PageURL>();
        internal System.Drawing.PointF currentTransformLocation = new System.Drawing.PointF();
        private string graphicDictonaykey = string.Empty;
        private float m_opacity;
        private bool graphiStateOpacity = false;
        char[] m_symbolChars = new char[] { '(', ')', '[', ']', '<', '>' };
        char[] m_startText = new char[] { '(', '[', '<', };
        char[] m_endText = new char[] { ')', ']', '>' };
        PdfPageResources m_resources;
        PdfRecordCollection m_contentElements;
        private Point m_currentLocation = new Point(0, 0);
        private bool m_beginText;
        private float m_wordSpacing;
        private float m_characterSpacing;
        private WPFGraphics m_graphics;
        private Rect m_clipRectangle;
        private PathFigure m_path;
        private float m_mitterLength;
        private float m_textLeading;
        private Stack<GraphicsState> m_graphicsState = new Stack<GraphicsState>();
        private Stack<GraphicObjectData> m_objects = new Stack<GraphicObjectData>();
        private PdfViewerExceptions exception = new PdfViewerExceptions();
        private float m_textScaling = 100;
        private float m_textAngle;
        private bool needRotation = false;
        int textTransformation;
        Color m_emptyColor = new Color();
        private List<PathFigure> m_subPaths = new List<PathFigure>();
        private List<PathFigure> m_tempSubPaths = new List<PathFigure>();
        private double m_previousTextWidth;
        private bool m_isCurrentPositionChanged;
        private double m_textElementWidth;
        bool textMatrix;
        double textEndPosition;
        int lineCount;
        bool isSameFont = false;
        DeviceCMYK decodecmykColor = new DeviceCMYK();
        private string[] m_dashedLine;
        private List<Rect> m_clipRectangleList = new List<Rect>();
        private bool IsNegativeFont;
        private bool PushedTextScale = false;
        #endregion

        #region Constructors
        public WPFRenderer(PdfRecordCollection contentElements, PdfPageResources resources, WPFGraphics graphics, Rect bounds, bool newPage)
        {
            GraphicObjectData newObject = new GraphicObjectData();
            m_objects.Push(newObject);
            m_contentElements = contentElements;
            m_resources = resources;
            m_graphics = graphics;
            PdfUnitConvertor converter = new PdfUnitConvertor();
            float bottom = converter.ConvertFromPixels((float)bounds.Bottom, PdfGraphicsUnit.Point);
            isSameFont = resources.isSameFont();
            if (newPage)
                m_graphics.PushTranslateTransform(0, bottom, true);
        }
        #endregion

        #region Properties
        public WPFGraphics PageGraphics
        {
            get
            {
                return m_graphics;
            }
        }

        private Point CurrentLocation
        {
            get
            {
                return m_currentLocation;
            }
            set
            {
                m_isCurrentPositionChanged = true;
                m_currentLocation = value;
            }
        }

        private string CurrentFont
        {
            get
            {
                if (Objects.CurrentFont != null)
                {
                    return Objects.CurrentFont;
                }
                else
                {
                    string tempFontName = "";
                    foreach (GraphicObjectData objectData in m_objects)
                    {
                        if (objectData.CurrentFont != null)
                        {
                            tempFontName = objectData.CurrentFont;
                            break;
                        }
                    }
                    return tempFontName;
                }
            }
            set
            {
                Objects.CurrentFont = value;
            }

        }

        private float FontSize
        {
            get
            {
                if (Objects.CurrentFont != null)
                {
                    return Objects.FontSize;
                }
                else
                {
                    float tempFontSize = 0;
                    foreach (GraphicObjectData objectData in m_objects)
                    {
                        if (objectData.CurrentFont != null)
                        {
                            tempFontSize = objectData.FontSize;
                        }
                    }
                    return tempFontSize;
                }
            }
            set
            {
                Objects.FontSize = value;
            }
        }

        private GraphicObjectData Objects
        {
            get
            {
                return m_objects.Peek();
            }
        }

        private Color NonStrokingColorSpace
        {
            get
            {
                if (Objects.NonStrokingColorspace != m_emptyColor)
                {
                    return Objects.NonStrokingColorspace;
                }
                else
                {
                    foreach (GraphicObjectData objectData in m_objects)
                    {
                        if (objectData.NonStrokingColorspace != m_emptyColor)
                        {
                            return objectData.NonStrokingColorspace;
                        }
                    }
                    return m_emptyColor;
                }
            }
            set
            {
                Objects.NonStrokingColorspace = value;
            }
        }

        private Color StrokingColorSpace
        {
            get
            {
                if (Objects.StrokingColorspace != m_emptyColor)
                {
                    return Objects.StrokingColorspace;
                }
                else
                {
                    foreach (GraphicObjectData objectData in m_objects)
                    {
                        if (objectData.StrokingColorspace != m_emptyColor)
                        {
                            return objectData.StrokingColorspace;
                        }
                    }
                    return m_emptyColor;
                }
            }
            set
            {
                Objects.StrokingColorspace = value;
            }
        }

        private Rect ClipRectangle
        {
            get
            {
                return m_clipRectangle;
            }
            set
            {
                m_clipRectangle = value;
            }
        }

        private float WordSpacing
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

        private float CharacterSpacing
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

        private float MitterLength
        {
            get
            {
                return m_mitterLength;
            }
            set
            {
                m_mitterLength = value;
            }

        }

        private float TextLeading
        {
            get
            {
                return m_textLeading;
            }
            set
            {
                m_textLeading = value;
            }
        }

        private float TextScaling
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

        private PathFigure Path
        {
            get
            {
                return m_path;
            }
            set
            {
                m_path = value;
            }
        }
        #endregion

        #region Implementation
        public void Render()
        {
            try
            {
                if (m_contentElements != null)
                {
                    foreach (PdfRecord record in m_contentElements)
                    {
                        string token = record.OperatorName;
                        string[] element = record.Operands;

                        switch (token.Trim())
                        {
                            case "BDC":
                                {
                                    if (element[0].Contains("Span"))
                                    {
                                        textEndPosition = m_previousTextWidth;
                                    }
                                    break;
                                }
                            case "q":
                                {
                                    GraphicObjectData data = new GraphicObjectData();
                                    PageGraphics.Push();
                                    m_objects.Push(data);
                                    break;
                                }
                            case "Q":
                                {
                                    PageGraphics.Pop();
                                    m_objects.Pop();
                                    scalingFactor = new System.Drawing.PointF();
                                    if (textSearchClipRect != new Rect())
                                    {
                                        IsRegionCliped = true;
                                    }
                                    m_opacity = 0; graphiStateOpacity = false;
                                    break;
                                }
                            case "Tm":
                                {
                                    try
                                    {
                                        textEndPosition = 0;
                                        m_textAngle = 0;
                                        needRotation = false;

                                        if (textMatrix)
                                        {
                                            for (int i = 0; i < textTransformation; i++)
                                            {
                                                PageGraphics.PopTransform();
                                            }
                                            textTransformation = 0;
                                        }
                                        //[a b c d e f]
                                        float a = float.Parse(element[0]);
                                        float b = float.Parse(element[1]);
                                        float c = float.Parse(element[2]);
                                        float d = float.Parse(element[3]);
                                        float e = float.Parse(element[4]);
                                        float f = float.Parse(element[5]);
                                        PdfUnitConvertor convertor = new PdfUnitConvertor();
                                        float sizeY = convertor.ConvertFromPixels((float)PageGraphics.Height, PdfGraphicsUnit.Point);

                                        if (e != null && f != null)
                                        {
                                            PageGraphics.PushTranslateTransform(e, -f, false);
                                            currentTransformationLoc = new System.Drawing.PointF(e, (sizeY - f));
                                            textTransformation++;
                                        }

                                        if (a != 1 || d != 1)
                                        {
                                            if (a != 0 && d != 0)
                                            {
                                                PageGraphics.PushScaleTransform(a, d);
                                                scalingFactor.X = a;
                                                scalingFactor.Y = d;
                                                textTransformation++;
                                            }
                                        }

                                        if (b != 0 || c != 0)
                                        {
                                            double rad = Math.Acos(a);
                                            double degree = Math.Round((180 / Math.PI) * rad);

                                            double checkRad = Math.Asin(b);
                                            double checkDegree = Math.Round((180 / Math.PI) * checkRad);
                                            Debug.WriteLine(checkDegree.ToString());
                                            if (degree == checkDegree)
                                            {
                                                m_textAngle = -(float)degree;
                                            }
                                            else
                                            {
                                                if (double.IsNaN(checkDegree))
                                                {
                                                    m_textAngle = -(float)degree;
                                                    PageGraphics.PushScaleTransform(b, -c);
                                                    scalingFactor = new System.Drawing.PointF(b, -c);
                                                    textTransformation++;
                                                }
                                                else
                                                {
                                                    m_textAngle = -(float)checkDegree;
                                                }
                                            }
                                            needRotation = true;

                                            PageGraphics.PushRotateTransform(m_textAngle);
                                            textTransformation++;
                                        }

                                        CurrentLocation = new Point(0, 0);
                                        textMatrix = true;
                                    }
                                    catch
                                    {
                                        Debug.WriteLine(token + "not properly implemented");
                                    }
                                    break;
                                }
                            case "cm":
                                {
                                    bool translateTransform = false;
                                    //[a b c d e f]
                                    float a = float.Parse(element[0]);
                                    float b = float.Parse(element[1]);
                                    float c = float.Parse(element[2]);
                                    float d = float.Parse(element[3]);
                                    float e = float.Parse(element[4]);
                                    float f = float.Parse(element[5]);

                                    //TranslateTransform [ 1 0 0 1 tx ty ]
                                    //if (e != 0 || f != 0)
                                    {
                                        if ((a != 1 || d != 1) /*&& a != 0 && d != 0*/)
                                        {
                                            translateTransform = true;
                                            if (a == 0 && d == 0)
                                            {
                                                PageGraphics.PushTranslateTransform(c + e, -(f + d), false);
                                                currentTransformationLoc = new System.Drawing.PointF(e, -(f + d));
                                                PageGraphics.PushScaleTransform(-c, b);
                                                scalingFactor = new System.Drawing.PointF(-c, b);
                                            }
                                            else
                                            {
                                                if (e == 0 && f == 0)
                                                {
                                                    if (b == 0 && c == 0)
                                                    {
                                                        PageGraphics.PushTranslateTransform(e, -(f + d), false);
                                                        currentTransformationLoc.X += e;
                                                        currentTransformationLoc.Y += -(f + d);
                                                        PageGraphics.PushScaleTransform(a, d);
                                                        scalingFactor = new System.Drawing.PointF(a, d);
                                                    }
                                                    else
                                                    {
                                                        PageGraphics.PushTranslateTransform(e, -(f + d), false);
                                                        currentTransformationLoc = new System.Drawing.PointF(e, -(f + d));
                                                    }
                                                }
                                                else if ((e > 0 && e < 1) || (f > 0 && f < 1))
                                                {
                                                    PageGraphics.PushTranslateTransform(e, -(f + d), false);
                                                    PageGraphics.PushScaleTransform(a, d);
                                                }
                                                else
                                                {
                                                    PageGraphics.PushTranslateTransform(e, -(f + d), false);
                                                    currentTransformationLoc = new System.Drawing.PointF(e, -(f + d));
                                                    if (b <= 0 && c <= 0)
                                                    {
                                                        PageGraphics.PushScaleTransform(a, d);
                                                        scalingFactor = new System.Drawing.PointF(a, d);
                                                    }
                                                }
                                            }
                                            CurrentLocation = new Point(0, 0);
                                        }
                                        else
                                        {
                                            PageGraphics.PushTranslateTransform(e, -f, false);
                                            currentTransformationLoc = new System.Drawing.PointF(e, -f);
                                            CurrentLocation = new Point(0, 0);
                                        }
                                    }

                                    //ScaleTransform [ sx 0 0 sy 0 0 ]
                                    if ((a != 0 && a != 1) && (d != 0 && d != 1) && (a != d) && (b + c) == 0 && (b == c) && (e + f) == 0 && (e == f) && translateTransform == false)
                                    {
                                        PageGraphics.PushScaleTransform(a, d);
                                        scalingFactor = new System.Drawing.PointF(a, d);
                                        CurrentLocation = new Point(0, 0);
                                    }

                                    //SkewTransform [ 1 tan α tan β 1 0 0 ]
                                    else if ((a == d) && (d == 1) && (e == f) && (e == 0))
                                    {
                                        //perform skew
                                    }

                                    //RotateTransform [ cos θ sin θ −sin θ cos θ 0 0 ]
                                    else
                                    {
                                        //check for rotate transform
                                        double rad = Math.Acos(a);
                                        double degree = Math.Round((180 / Math.PI) * rad);

                                        double checkRad = Math.Asin(b);
                                        double checkDegree = Math.Round((180 / Math.PI) * checkRad);

                                        double bSin = Math.Asin(b);
                                        double cSin = Math.Asin(c);

                                        if (degree == checkDegree || (bSin > 0 && cSin < 0 && !double.IsNaN(degree)))
                                        {
                                            PageGraphics.PushRotateTransform(-(float)degree);
                                        }
                                        else
                                        {
                                            if ((a == 0 && d == 0) || ((e > 0 && e < 1) || (f > 0 && f < 1)))
                                            {
                                                if (!double.IsNaN(degree))
                                                    PageGraphics.PushRotateTransform(-(float)degree);
                                            }
                                            else
                                            {
                                                if (!double.IsNaN(checkDegree))
                                                    PageGraphics.PushRotateTransform(-(float)checkDegree);
                                                else if (!double.IsNaN(degree))
                                                    PageGraphics.PushRotateTransform(-(float)degree);
                                            }
                                        }
                                    }

                                    break;
                                }
                            case "BT":
                                {
                                    m_beginText = true;
                                    CurrentLocation = new Point(0, 0);
                                    break;
                                }
                            case "ET":
                                {
                                    if (TextScaling != 100 && PushedTextScale)
                                    {
                                        PageGraphics.PopTransform();
                                        PushedTextScale = false;
                                    }
                                    TextLeading = 0;
                                    CurrentLocation = new Point(0, 0);
                                    if (textMatrix)
                                    {
                                        for (int i = 0; i < textTransformation; i++)
                                        {
                                            PageGraphics.PopTransform();
                                        }
                                        textTransformation = 0;
                                    }

                                    if (needRotation)
                                    {
                                        needRotation = false;
                                        m_textAngle = 0;
                                    }
                                    break;
                                }
                            case "T*":
                                {
                                    DrawNewLine();
                                    break;
                                }
                            case "Tj":
                                {
                                    RenderTextElement(element, token);
                                    break;
                                }
                            case "'":
                                {
                                    RenderTextElementWithLeading(element, token);
                                    break;
                                }
                            case "TJ":
                                {
                                    if (TextScaling != 100)
                                    {
                                        PageGraphics.PushScaleTransform(TextScaling / 100, 1);
                                        PushedTextScale = true;
                                    }
                                    RenderTextElementWithSpacing(element, token);
                                    break;
                                }
                            case "Tf":
                                {
                                    RenderFont(element);
                                    break;
                                }
                            case "TD":
                                {
                                    m_textLeading = float.Parse(element[1]);
                                    CurrentLocation = new Point(CurrentLocation.X + float.Parse(element[0]), CurrentLocation.Y - (float.Parse(element[1])));
                                    break;
                                }
                            case "Td":
                                {
                                    textEndPosition = 0;
                                    lineCount = 0;
                                    CurrentLocation = new Point(CurrentLocation.X + float.Parse(element[0]), CurrentLocation.Y - (float.Parse(element[1])));
                                    break;
                                }
                            case "Tr":
                                {
                                    if (element[0] == "3")
                                    {
                                        StrokingColorSpace = Colors.Transparent;
                                        NonStrokingColorSpace = Colors.Transparent;
                                    }
                                    break;
                                }
                            case "TL":
                                {
                                    m_textLeading = float.Parse(element[0]);
                                    break;
                                }
                            case "Tw":
                                {
                                    GetWordSpacing(element);
                                    break;
                                }
                            case "Tc":
                                {
                                    GetCharacterSpacing(element);
                                    break;
                                }
                            case "Tz":
                                {
                                    GetScalingFactor(element);
                                    break;
                                }
                            case "k":
                                {
                                    GetColorspace(element, "stroking", "DeviceCMYK");
                                    break;
                                }
                            case "K":
                                {
                                    GetColorspace(element, "nonstroking", "DeviceCMYK");
                                    break;
                                }
                            case "RG":
                            case "SC":
                            case "cs":
                            case "SCN":
                                {
                                    GetColorspace(element, "nonstroking", "RGB");
                                    break;
                                }
                            case "rg":
                            case "sc":
                            case "CS":
                            case "scn":
                                {
                                    GetColorspace(element, "stroking", "RGB");
                                    break;
                                }
                            case "g":
                                {
                                    GetColorspace(element, "stroking", "Gray");
                                    break;
                                }
                            case "G":
                                {
                                    GetColorspace(element, "nonstroking", "Gray");
                                    break;
                                }
                            case "gs":
                                {
                                    GetExtendedGraphicsStateObject(element[0]);
                                    break;
                                }
                            case "Do":
                                {
                                    currentTransformationLoc = new System.Drawing.PointF();
                                    GetXObject(element);
                                    break;
                                }
                            case "re":
                                {
                                    GetClipRectangle(element);
                                    break;
                                }
                            case "d":
                                {
                                    if (element[0] != "[]" && !element[0].Contains("\n"))
                                    {
                                        m_dashedLine = element;
                                    }
                                    break;
                                }
                            case "b":
                            case "b*":
                                {
                                    if (Path != null)
                                    {
                                        Pen borderPen = GetPen(NonStrokingColorSpace, MitterLength);
                                        Path.IsClosed = true;
                                        PageGraphics.DrawPath(borderPen, Path);

                                        PathGeometry geom = new PathGeometry();
                                        PathFigureCollection col = new PathFigureCollection();
                                        col.Add(Path);
                                        geom.Figures = col;

                                        if (token.Trim() == "b")
                                            geom.FillRule = FillRule.Nonzero;
                                        else
                                            geom.FillRule = FillRule.EvenOdd;

                                        PageGraphics.FillPath(GetBrush(StrokingColorSpace), geom);
                                    }
                                    else
                                    {
                                    }
                                    break;
                                }
                            case "B":
                            case "B*":
                                {
                                    if (ClipRectangle == Rect.Empty)
                                    {
                                        Pen borderPen = GetPen(NonStrokingColorSpace, MitterLength);
                                        Path.IsClosed = true;
                                        PageGraphics.DrawPath(borderPen, Path);

                                        PathGeometry geom = new PathGeometry();
                                        PathFigureCollection col = new PathFigureCollection();
                                        col.Add(Path);
                                        geom.Figures = col;

                                        if (token.Trim() == "B")
                                            geom.FillRule = FillRule.Nonzero;
                                        else
                                            geom.FillRule = FillRule.EvenOdd;

                                        PageGraphics.FillPath(GetBrush(StrokingColorSpace), geom);
                                    }
                                    else
                                    {
                                        Pen borderPen = GetPen(NonStrokingColorSpace, MitterLength);
                                        PageGraphics.DrawRectangle(borderPen, new Rect(ClipRectangle.X, ClipRectangle.Y, ClipRectangle.Width, ClipRectangle.Height));
                                        PageGraphics.FillRectangle(GetBrush(StrokingColorSpace), new Rect(ClipRectangle.X, ClipRectangle.Y, ClipRectangle.Width, ClipRectangle.Height));
                                    }
                                    Path = new PathFigure();
                                    m_subPaths.Clear();
                                    m_tempSubPaths.Clear();
                                    ClipRectangle = Rect.Empty;
                                    break;
                                }
                            case "n":
                                {
                                    ClipRectangle = Rect.Empty;
                                    m_subPaths.Clear();
                                    m_tempSubPaths.Clear();
                                    Path = new PathFigure();
                                    m_clipRectangleList.Clear();
                                    break;
                                }
                            case "W":
                                {
                                    Rect emptyRectangle = new Rect(0, 0, 0, 0);
                                    if (ClipRectangle != Rect.Empty && ClipRectangle != emptyRectangle)
                                    {
                                        PageGraphics.PushClip(ClipRectangle);
                                    }
                                    else
                                    {
                                        if (Path != null)
                                        {
                                            PageGraphics.PushClip(Path);
                                        }
                                    }
                                    break;
                                }
                            case "w":
                                {
                                    m_mitterLength = float.Parse(element[0]);
                                    break;
                                }
                            case "s":
                            case "S":
                                {
                                    DrawPath();
                                    break;
                                }
                            case "f*":
                                {
                                    FillPath("EvenOdd");
                                    CurrentLocation = new Point(0, 0);
                                    break;
                                }
                            case "f":
                                {
                                    FillPath("NonZero");
                                    CurrentLocation = new Point(0, 0);
                                    break;
                                }
                            case "v":
                                {
                                    AddBezierCurve2(element);
                                    break;
                                }
                            case "y":
                                {
                                    AddBezierCurve3(element);
                                    break;
                                }
                            case "h":
                                {
                                    if (Path != null)
                                    {
                                        if (!Path.IsClosed)
                                        {
                                            Point startPoint = Path.StartPoint;
                                            Path.Segments.Add(new LineSegment(startPoint, true));
                                            Path.IsClosed = true;
                                        }
                                        m_subPaths.Add(Path);
                                    }
                                    break;
                                }
                            case "m":
                                {
                                    BeginPath(element);
                                    break;
                                }
                            case "c":
                                {
                                    AddBezierCurve(element);
                                    break;
                                }
                            case "l":
                                {
                                    AddLine(element);
                                    break;
                                }
                            default:
                                {
                                    Debug.WriteLine(token + " not implemented");
                                    break;
                                }
                        }
                    }
                    // PageGraphics.PushScaleTransform(20, 20);
                }
            }
            catch (Exception exce)
            {
                exception.Exceptions.Append("\r\n\r\nAn exception occured while rendering the PDF document\r\n The complete stack trace is given below\r\n" + exce.StackTrace + "\r\n");
            }
        }

        Pen GetPen(Color color, double width)
        {
            return new Pen(
                new SolidColorBrush(color),
                width);
        }

        Brush GetBrush(Color color)
        {
            return new SolidColorBrush(color);
        }

        private void RenderFont(string[] fontElements)
        {
            int i;
            for (i = 0; i < fontElements.Length; i++)
            {
                if (fontElements[i].Contains("/"))
                {
                    CurrentFont = fontElements[i].Replace("/", "");
                    break;
                }
            }
            FontSize = float.Parse(fontElements[i + 1]);
        }

        private void RenderTextElement(string[] textElements, string tokenType)
        {
            if (FontSize < 0)
            {
                FontSize = -FontSize;
                IsNegativeFont = true;
            }

            string text = string.Join("", textElements);
            (m_resources[CurrentFont] as FontStructure).IsSameFont = m_resources.isSameFont();
            if ((m_resources[CurrentFont] as FontStructure).FontSize != FontSize)
                (m_resources[CurrentFont] as FontStructure).FontSize = FontSize;

            FontStructure structure = m_resources[CurrentFont] as FontStructure;
            text = structure.Decode(text, isSameFont);
            System.Drawing.Font embeddedFont = structure.CurrentFont;
            Dictionary<int, int> glyphWidths = structure.FontGlyphWidths;

            float adjustment = FontSize;


            PageGraphics.CharacterMapping = structure.CharacterMapTable;
            PageGraphics.IsNegativeFont = IsNegativeFont;
            GlyphTypeface glyphTypeface = null;
            byte[] fontData = structure.fontStream.GetBuffer();
            if (fontData.Length != 0 && embeddedFont != null)
            {
                Uri fontUri = CreateMemoryResourceUri(fontData, fontData.Length);
                glyphTypeface = new GlyphTypeface(fontUri);
            }

            Color brushColor = (StrokingColorSpace != m_emptyColor) ? StrokingColorSpace : Colors.Black;

            string fontName = ResolveFontName(structure.FontName);

            PIFont usedFont = new PIFont(fontName, FontSize, (PIFontStyle)structure.FontStyle);

            Typeface face = PageGraphics.GetTypeFace(usedFont);
            FormattedText fText = new FormattedText(text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, face, FontSize, new SolidColorBrush(brushColor));

            PageGraphics.DefaultGlyphWidth = structure.DefaultGlyphWidth;
            PageGraphics.IsType1Font = structure.IsType1Font;
            PageGraphics.type1Height = structure.Type1GlyphHeight;
            glyphWidths = structure.FontGlyphWidths;
            PageGraphics.IsType1Font = structure.IsType1Font;
            PageGraphics.Is1C = structure.Is1C;
            PageGraphics.CharacterMapTable = structure.CharacterMapTable;
            PageGraphics.ReverseMapTable = structure.ReverseMapTable;
            if (structure.IsType1Font)
            {
                PageGraphics.IsType1Font = true;
                PageGraphics.differenceTable = structure.differenceTable;
                PageGraphics.differenceMappedTable = structure.DifferencesDictionary;
                PageGraphics.m_cffGlyphs = structure.m_cffGlyphs;
                if (!m_glyphDataCollection.Contains(PageGraphics.m_cffGlyphs))
                {
                    m_glyphDataCollection.Add(PageGraphics.m_cffGlyphs);
                }
            }
            if (m_isCurrentPositionChanged)
            {
                m_isCurrentPositionChanged = false;
                textEndPosition = CurrentLocation.X;
                m_textElementWidth = PageGraphics.DrawString(text,
                        new PIFont(fontName, FontSize, (PIFontStyle)structure.FontStyle),
                        brushColor,
                        WordSpacing,
                        CharacterSpacing,
                        TextScaling,
                        textEndPosition, CurrentLocation.Y - adjustment, false, glyphTypeface, glyphWidths);
            }
            else
            {
                textEndPosition = textEndPosition - m_textElementWidth;
                m_textElementWidth = PageGraphics.DrawString(text,
                        new PIFont(fontName, FontSize, (PIFontStyle)structure.FontStyle),
                        brushColor,
                        WordSpacing,
                        CharacterSpacing,
                        TextScaling,
                        textEndPosition, CurrentLocation.Y - adjustment, false, glyphTypeface, glyphWidths);
            }
            m_previousTextWidth = m_textElementWidth;

            currentTransformLocation = new System.Drawing.PointF((float)(currentTransformationLoc.X + CurrentLocation.X * scalingFactor.X), (float)(currentTransformationLoc.Y + (CurrentLocation.Y - FontSize) * scalingFactor.Y));
            TextSearch txt = new TextSearch(text, currentTransformLocation, (float)-m_textElementWidth, FontSize, scalingFactor, new PIFont(fontName, FontSize, (PIFontStyle)structure.FontStyle), fText, face);
            txtDictonary.Add(txt);
            if (text.Contains("www") || text.Contains("http") || IsValidEmail(text))
            {
                string uri = text;

                currentTransformLocation = new System.Drawing.PointF((float)(currentTransformationLoc.X + CurrentLocation.X * scalingFactor.X), (float)(currentTransformationLoc.Y + (CurrentLocation.Y - FontSize) * scalingFactor.Y));

                Regex linkParser = new Regex(@"\b(?:http://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                string urlStr = string.Empty;
                foreach (Match m in linkParser.Matches(uri))
                {
                    urlStr = m.Value;
                }
                if (IsValidEmail(text))
                {
                    urlStr = "mailto:" + uri;
                }
                PageURL url = new PageURL(urlStr, currentTransformLocation, (float)-m_textElementWidth, FontSize, scalingFactor);
                URLDictonary.Add(url);
            }
        }

        private void RenderTextElementWithLeading(string[] textElements, string tokenType)
        {
            if (FontSize < 0)
            {
                FontSize = -FontSize;
                IsNegativeFont = true;
            }
            string text = string.Join("", textElements);
            (m_resources[CurrentFont] as FontStructure).IsSameFont = m_resources.isSameFont();
            if ((m_resources[CurrentFont] as FontStructure).FontSize != FontSize)
                (m_resources[CurrentFont] as FontStructure).FontSize = FontSize;

            FontStructure structure = m_resources[CurrentFont] as FontStructure;
            text = structure.Decode(text, isSameFont);
            Dictionary<int, int> glyphWidths = structure.FontGlyphWidths;
            System.Drawing.Font embeddedFont = structure.CurrentFont;

            GlyphTypeface glyphTypeface = null;
            byte[] fontData = structure.fontStream.GetBuffer();
            if (fontData.Length != 0 && embeddedFont != null)
            {
                Uri fontUri = CreateMemoryResourceUri(fontData, fontData.Length);
                glyphTypeface = new GlyphTypeface(fontUri);
            }
            PageGraphics.CharacterMapping = structure.CharacterMapTable;
            PageGraphics.IsNegativeFont = IsNegativeFont;
            Color brushColor = (StrokingColorSpace != m_emptyColor) ? StrokingColorSpace : Colors.Black;

            string fontName = ResolveFontName(structure.FontName);

            PIFont usedFont = new PIFont(fontName, FontSize, (PIFontStyle)structure.FontStyle);

            Typeface face = PageGraphics.GetTypeFace(usedFont);
            FormattedText fText = new FormattedText(text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, face, FontSize, new SolidColorBrush(brushColor));
            PageGraphics.DefaultGlyphWidth = structure.DefaultGlyphWidth;
            PageGraphics.IsType1Font = structure.IsType1Font;
            PageGraphics.type1Height = structure.Type1GlyphHeight;
            glyphWidths = structure.FontGlyphWidths;
            PageGraphics.IsType1Font = structure.IsType1Font;
            PageGraphics.Is1C = structure.Is1C;
            PageGraphics.CharacterMapTable = structure.CharacterMapTable;
            PageGraphics.ReverseMapTable = structure.ReverseMapTable;
            if (structure.IsType1Font)
            {
                PageGraphics.IsType1Font = true;
                PageGraphics.differenceTable = structure.differenceTable;
                PageGraphics.differenceMappedTable = structure.DifferencesDictionary;
                PageGraphics.m_cffGlyphs = structure.m_cffGlyphs;
                if (!m_glyphDataCollection.Contains(PageGraphics.m_cffGlyphs))
                {
                    m_glyphDataCollection.Add(PageGraphics.m_cffGlyphs);
                }
            }

            if (m_isCurrentPositionChanged)
            {
                m_isCurrentPositionChanged = false;
                textEndPosition = CurrentLocation.X;
                m_textElementWidth = PageGraphics.DrawString(text,
                        new PIFont(fontName, FontSize, (PIFontStyle)structure.FontStyle),
                        brushColor,
                        WordSpacing,
                        CharacterSpacing,
                        TextScaling,
                        textEndPosition, CurrentLocation.Y + (TextLeading / 4), false, glyphTypeface, glyphWidths);
            }
            else
            {
                textEndPosition = textEndPosition - m_textElementWidth;
                m_textElementWidth = PageGraphics.DrawString(text,
                        new PIFont(fontName, FontSize, (PIFontStyle)structure.FontStyle),
                        brushColor,
                        WordSpacing,
                        CharacterSpacing,
                        TextScaling,
                        textEndPosition, CurrentLocation.Y + (TextLeading / 4), false, glyphTypeface, glyphWidths);
            }

            m_previousTextWidth = m_textElementWidth;
            DrawNewLine();
            CharacterSpacing = 0;
            WordSpacing = 0;
            if (!structure.IsMappingDone)
            {
                text = structure.MapCharactersFromTable(text);
            }

            PdfUnitConvertor m_unitconvertor = new PdfUnitConvertor();
            float textLeadingPoint = m_unitconvertor.ConvertToPixels(TextLeading, PdfGraphicsUnit.Point);
            float fontSizePoint = m_unitconvertor.ConvertToPixels(FontSize, PdfGraphicsUnit.Point);
            int temp = (int)((textLeadingPoint - TextLeading) + (fontSizePoint - FontSize));
            if (IsRegionCliped)
            {
                if (textSearchClipRect.X == 0)
                {
                    currentTransformLocation = new System.Drawing.PointF((float)(CurrentLocation.X + currentTransformationLoc.X + textSearchClipRect.X + textSearchClipRect.Height) + TextLeading, (float)(((CurrentLocation.Y + textSearchClipRect.Height) + FontSize)));
                }
                else
                {
                    currentTransformLocation = new System.Drawing.PointF((float)(CurrentLocation.X + currentTransformationLoc.X + textSearchClipRect.X), (float)(((CurrentLocation.Y + currentTransformationLoc.Y + (TextLeading / 4)) - (FontSize)) + textSearchClipRect.Y));
                }
            }
            else
            {
                currentTransformLocation = new System.Drawing.PointF((float)CurrentLocation.X + currentTransformationLoc.X, (float)(CurrentLocation.Y + currentTransformationLoc.Y + (TextLeading / 4)) - (FontSize));
            }
            currentTransformLocation = new System.Drawing.PointF((float)CurrentLocation.X + currentTransformationLoc.X, (float)(CurrentLocation.Y + currentTransformationLoc.Y + (TextLeading - FontSize)) - (FontSize));
            TextSearch txt = new TextSearch(text, currentTransformLocation, (float)-m_textElementWidth, FontSize, scalingFactor, new PIFont(fontName, FontSize, (PIFontStyle)structure.FontStyle), fText, face);
            txtDictonary.Add(txt);
            if (text.Contains("www") || text.Contains("http") || IsValidEmail(text))
            {
                string uri = text;

                currentTransformLocation = new System.Drawing.PointF((float)CurrentLocation.X + currentTransformationLoc.X, (float)(CurrentLocation.Y + currentTransformationLoc.Y + (TextLeading / 4)) - (FontSize));

                Regex linkParser = new Regex(@"\b(?:http://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                string urlStr = string.Empty;
                foreach (Match m in linkParser.Matches(uri))
                {
                    urlStr = m.Value;
                }
                if (IsValidEmail(text))
                {
                    urlStr = "mailto:" + uri;
                }
                PageURL url = new PageURL(urlStr, currentTransformLocation, (float)-m_textElementWidth, FontSize, scalingFactor);
                URLDictonary.Add(url);
            }
        }

        private void RenderTextElementWithSpacing(string[] textElements, string tokenType)
        {
            List<string> decodedList = new List<string>();

            if (FontSize < 0)
            {
                FontSize = -FontSize;
                IsNegativeFont = true;
            }
            string text = string.Join("", textElements);
            (m_resources[CurrentFont] as FontStructure).IsSameFont = m_resources.isSameFont();
            if ((m_resources[CurrentFont] as FontStructure).FontSize != FontSize)
                (m_resources[CurrentFont] as FontStructure).FontSize = FontSize;

            FontStructure structure = m_resources[CurrentFont] as FontStructure;
            decodedList = structure.DecodeTextTJ(text, isSameFont);
            System.Drawing.Font embeddedFont = structure.CurrentFont;
            Dictionary<int, int> glyphWidths = structure.FontGlyphWidths;

            float adjustment = FontSize;

            GlyphTypeface glyphTypeface = null;
            byte[] fontData = structure.fontStream.GetBuffer();
            if (fontData.Length != 0 && embeddedFont != null)
            {
                Uri fontUri = CreateMemoryResourceUri(fontData, fontData.Length);
                glyphTypeface = new GlyphTypeface(fontUri);
            }
            PageGraphics.CharacterMapping = structure.CharacterMapTable;
            PageGraphics.IsNegativeFont = IsNegativeFont;
            Color brushColor = (StrokingColorSpace != m_emptyColor) ? StrokingColorSpace : Colors.Black;

            string fontName = ResolveFontName(structure.FontName);

            PIFont usedFont = new PIFont(fontName, FontSize, (PIFontStyle)structure.FontStyle);

            Typeface face = PageGraphics.GetTypeFace(usedFont);
            FormattedText fText = new FormattedText(text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, face, FontSize, new SolidColorBrush(brushColor));

            PageGraphics.DefaultGlyphWidth = structure.DefaultGlyphWidth;
            PageGraphics.IsType1Font = structure.IsType1Font;
            PageGraphics.type1Height = structure.Type1GlyphHeight;
            glyphWidths = structure.FontGlyphWidths;
            PageGraphics.IsType1Font = structure.IsType1Font;
            PageGraphics.Is1C = structure.Is1C;
            PageGraphics.CharacterMapTable = structure.CharacterMapTable;
            PageGraphics.ReverseMapTable = structure.ReverseMapTable;
            if (structure.IsType1Font)
            {
                PageGraphics.IsType1Font = true;
                PageGraphics.differenceTable = structure.differenceTable;
                PageGraphics.differenceMappedTable = structure.DifferencesDictionary;
                PageGraphics.m_cffGlyphs = structure.m_cffGlyphs;
                if (!m_glyphDataCollection.Contains(PageGraphics.m_cffGlyphs))
                {
                    m_glyphDataCollection.Add(PageGraphics.m_cffGlyphs);
                }
            }

            if (m_isCurrentPositionChanged)
            {
                m_isCurrentPositionChanged = false;
                textEndPosition = CurrentLocation.X;
                m_textElementWidth = PageGraphics.DrawStringWithSpacing(decodedList,
                        new PIFont(fontName, FontSize, (PIFontStyle)structure.FontStyle),
                        brushColor,
                        WordSpacing,
                        CharacterSpacing,
                        TextScaling,
                        textEndPosition, CurrentLocation.Y - adjustment, false, glyphTypeface, glyphWidths);
            }
            else
            {
                textEndPosition = textEndPosition - m_textElementWidth;
                m_textElementWidth = PageGraphics.DrawStringWithSpacing(decodedList,
                        new PIFont(fontName, FontSize, (PIFontStyle)structure.FontStyle),
                        brushColor,
                        WordSpacing,
                        CharacterSpacing,
                        TextScaling,
                        textEndPosition, CurrentLocation.Y - adjustment, false, glyphTypeface, glyphWidths);
            }
            m_previousTextWidth = m_textElementWidth;

            #region TextSearch
            float curLocX;
            if (scalingFactor.X > 0)
            {
                curLocX = (float)(currentTransformationLoc.X + (textEndPosition * scalingFactor.X));
            }
            else
            {
                curLocX = (float)(currentTransformationLoc.X + textEndPosition);
            }
            float curLocY;
            if (scalingFactor.Y > 0)
            {
                curLocY = (float)(currentTransformationLoc.Y + (CurrentLocation.Y - FontSize) * scalingFactor.Y);
            }
            else
            {
                curLocY = (float)(currentTransformationLoc.Y + (CurrentLocation.Y - FontSize));
            }

            currentTransformLocation = new System.Drawing.PointF((float)curLocX, (float)curLocY);

            TextSearch txt = new TextSearch(PageGraphics.renderedText, currentTransformLocation, (float)-m_textElementWidth, FontSize, scalingFactor, new PIFont(fontName, FontSize, (PIFontStyle)structure.FontStyle), fText, face);
            txtDictonary.Add(txt);
            #endregion
            if (PageGraphics.renderedText.Contains("www") || PageGraphics.renderedText.Contains("http") || IsValidEmail(PageGraphics.renderedText))
            {
                string uri = PageGraphics.renderedText;


                if (scalingFactor.X > 0)
                {
                    curLocX = (float)(currentTransformationLoc.X + (CurrentLocation.X) * scalingFactor.X);
                }
                else
                {
                    curLocX = (float)(currentTransformationLoc.X + (CurrentLocation.X));
                }

                if (scalingFactor.Y > 0)
                {
                    curLocY = (float)(currentTransformationLoc.Y + (CurrentLocation.Y - FontSize) * scalingFactor.Y);
                }
                else
                {
                    curLocY = (float)(currentTransformationLoc.Y + (CurrentLocation.Y - FontSize));
                }

                currentTransformLocation = new System.Drawing.PointF((float)curLocX, (float)curLocY);//+ (TextLeading / 4)
                Regex linkParser = new Regex(@"\b(?:http://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                string urlStr = string.Empty;
                foreach (Match m in linkParser.Matches(uri))
                {
                    urlStr = m.Value;
                }
                if (IsValidEmail(uri))
                {
                    urlStr = "mailto:" + uri;
                }
                PageURL url = new PageURL(urlStr, currentTransformLocation, (float)-m_textElementWidth, FontSize, scalingFactor);
                URLDictonary.Add(url);
            }

        }

        /// <summary>
        /// Method for determining is the user provided a valid email address        
        /// </summary>
        /// <param name="email">email address to validate</param>
        /// <returns>true is valid, false if not valid</returns>
        public bool IsValidEmail(string email)
        {
            //regular expression pattern for valid email

            string pattern = @"^[-a-zA-Z0-9][-.a-zA-Z0-9]*@[-.a-zA-Z0-9]+(\.[-.a-zA-Z0-9]+)*\.(com|edu|info|gov|int|mil|net|org|biz|name|museum|coop|aero|pro|tv|[a-zA-Z]{2})$";
            Regex check = new Regex(pattern, RegexOptions.IgnorePatternWhitespace);
            bool valid = false;
            if (string.IsNullOrEmpty(email))
            {
                valid = false;
            }
            else
            {
                valid = check.IsMatch(email);
            }
            return valid;
        }

        private void GetColorspace(string[] colorElement, string type, string colorSpace)
        {
            float r, g, b;
            if (!colorElement[0].Contains("/"))
            {
                if (colorSpace == "RGB" && colorElement.Length == 3)
                {
                    r = float.Parse(colorElement[0]);
                    g = float.Parse(colorElement[1]);
                    b = float.Parse(colorElement[2]);
                }
                else if (colorSpace == "Gray" && colorElement.Length == 1)
                {
                    r = g = b = float.Parse(colorElement[0]);
                }
                else if (colorSpace == "DeviceCMYK" && colorElement.Length == 4)
                {
                    float c, m, y, k;
                    float.TryParse(colorElement[0], out c);
                    float.TryParse(colorElement[1], out m);
                    float.TryParse(colorElement[2], out y);
                    float.TryParse(colorElement[3], out k);

                    r = 255 * (1 - c) * (1 - k);
                    g = 255 * (1 - m) * (1 - k);
                    b = 255 * (1 - y) * (1 - k);
                    if (type == "nonstroking")
                        Objects.NonStrokingColorspace = Color.FromArgb(255, (byte)r, (byte)g, (byte)b);
                    else
                        Objects.StrokingColorspace = Color.FromArgb(255, (byte)r, (byte)g, (byte)b);
                    return;
                }
                else
                {
                    r = 0;
                    b = 0;
                    g = 0;
                }
                if (type == "nonstroking")
                    Objects.NonStrokingColorspace = Color.FromRgb((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
                else
                    Objects.StrokingColorspace = Color.FromRgb((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
            }
        }
        private void GetExtendedGraphicsStateObject(string keyValue)
        {
            try
            {
                PdfPageResources res = this.m_resources;
                if (m_resources.ContainsKey(keyValue.Replace("/", "")))
                {
                    PdfName name = new PdfName(keyValue.Replace("/", ""));
                    PdfDictionary g = ((m_resources[keyValue.Replace("/", "")] as XObjectElement)).XObjectDictionary;
                    foreach (Syncfusion.Pdf.Primitives.PdfName k2 in g.Keys)
                    {
                        if (k2.Value == "ca" || k2.Value == "CA")
                        {
                            PdfNumber graphicsStateOpacity = g.Items[k2] as PdfNumber;
                            m_opacity = graphicsStateOpacity.FloatValue;
                            graphiStateOpacity = true;
                        }
                    }
                }
            }
            catch (Exception)
            {
                m_opacity = 1.0f;
            }
        }
        private void DrawNewLine()
        {
            m_isCurrentPositionChanged = true;
            FontStructure fontStructure = m_resources[CurrentFont] as FontStructure;
            if (TextLeading != 0)
                m_currentLocation.Y = (TextLeading < 0 ? m_currentLocation.Y - m_textLeading : m_currentLocation.Y + m_textLeading);
            else
                m_currentLocation.Y += FontSize;

        }

        private void GetClipRectangle(string[] rectangle)
        {
            double x = double.Parse(rectangle[0]);
            double y = -double.Parse(rectangle[1]);
            double width = double.Parse(rectangle[2]);
            double height = -double.Parse(rectangle[3]);

            if (height < 0)
            {
                height = -height;
                y = y - height;
            }

            if (width < 0)
            {
                width = -width;
                x = x - width;
            }
            //m_clipRectangle = new Rect(x, y, width, height);

            if ((ClipRectangle != new Rect(0, 0, 0, 0)) && !ClipRectangle.IsEmpty)
            {
                m_clipRectangleList.Add(m_clipRectangle);
            }

            if ((ClipRectangle != new Rect(0, 0, 0, 0)) && !ClipRectangle.IsEmpty)
            {
                m_clipRectangle = new Rect(x, y, width, height);
            }
            else
            {
                m_clipRectangle = new Rect(x, y, width, height);
            }
            textSearchClipRect = m_clipRectangle;
        }

        private void GetWordSpacing(string[] spacing)
        {
            m_wordSpacing = float.Parse(spacing[0]);
        }

        private void GetCharacterSpacing(string[] spacing)
        {
            m_characterSpacing = float.Parse(spacing[0]);
        }

        private void GetScalingFactor(string[] scaling)
        {
            m_textScaling = float.Parse(scaling[0]);
        }

        private void DrawPath()
        {
            Pen pathPen;

            if (MitterLength != 0)
                pathPen = GetPen(NonStrokingColorSpace == m_emptyColor ? Colors.Black : NonStrokingColorSpace, MitterLength);
            else
                pathPen = GetPen(NonStrokingColorSpace == m_emptyColor ? Colors.Black : NonStrokingColorSpace, 1.0f);

            if (m_dashedLine != null)
            {
                string dash = m_dashedLine[0];
                dash = dash.Substring(1, dash.Length - 2);
                dash = dash.Trim();
                List<string> dashlist = new List<string>();
                string[] dasharray = dash.Split(' ');
                foreach (string str in dasharray)
                {
                    if (str != "")
                        dashlist.Add(str);
                }
                DoubleCollection collection = new DoubleCollection(dashlist.Count);
                for (int x = 0; x < dashlist.Count; x++)
                {
                    if (dasharray[x] != "")
                        collection.Add(double.Parse(dasharray[x]));
                }
                if (collection.Count > 0 && m_mitterLength < collection[0])
                {
                    if (m_mitterLength != 0)
                    {
                        for (int i = 0; i < collection.Count; i++)
                        {
                            collection[i] = collection[i] / m_mitterLength;
                        }
                    }
                }

                DashStyle style = new DashStyle();
                style.Dashes = collection;
                pathPen.DashStyle = style;

                m_dashedLine = null;
            }

            if (ClipRectangle != Rect.Empty && ((ClipRectangle.X + ClipRectangle.Y + ClipRectangle.Width + ClipRectangle.Height) != 0))
            {
                if (graphiStateOpacity == true)
                {
                    SolidColorBrush brush = new SolidColorBrush(NonStrokingColorSpace == m_emptyColor ? Colors.Black : NonStrokingColorSpace);
                    brush.Opacity = m_opacity;
                    pathPen = new Pen(brush, (double)MitterLength);
                    PageGraphics.DrawRectangle(pathPen, new Rect(ClipRectangle.X, ClipRectangle.Y, ClipRectangle.Width, ClipRectangle.Height));
                }
                else
                {
                    PageGraphics.DrawRectangle(pathPen, new Rect(ClipRectangle.X, ClipRectangle.Y, ClipRectangle.Width, ClipRectangle.Height));
                }
                ClipRectangle = Rect.Empty;
            }
            foreach (Rect rect in m_clipRectangleList)
            {
                if (graphiStateOpacity)
                {
                    SolidColorBrush brush = new SolidColorBrush(NonStrokingColorSpace == m_emptyColor ? Colors.Black : NonStrokingColorSpace);
                    brush.Opacity = m_opacity;
                    pathPen = new Pen(brush, (double)MitterLength);
                }
                PageGraphics.DrawRectangle(pathPen, rect);
            }
            m_clipRectangleList.Clear();
            if (Path != null)
            {
                if (m_tempSubPaths.Count > 0)
                {
                    foreach (PathFigure path in m_tempSubPaths)
                    {
                        try
                        {
                            PageGraphics.DrawPath(pathPen, path);
                        }
                        catch (Exception)
                        {
                            exception.Exceptions.Append("\r\n\r\nExceptions in token S\r\n");
                        }
                    }
                    m_tempSubPaths.Clear();
                }
                else if (m_subPaths.Count > 0)
                {
                    foreach (PathFigure path in m_subPaths)
                    {
                        try
                        {
                            PageGraphics.DrawPath(pathPen, path);
                        }
                        catch (Exception)
                        {
                            exception.Exceptions.Append("\r\n\r\nExceptions in token S\r\n");
                        }
                    }
                }
                if (graphiStateOpacity == true)
                {
                    SolidColorBrush brush = new SolidColorBrush(NonStrokingColorSpace == m_emptyColor ? Colors.Black : NonStrokingColorSpace);
                    brush.Opacity = m_opacity;
                    pathPen = new Pen(brush, (double)MitterLength);
                    PageGraphics.DrawPath(pathPen, Path);
                }
                else
                {
                    PageGraphics.DrawPath(pathPen, Path);
                }

                m_subPaths.Clear();
                Path = new PathFigure();
            }
        }

        private void FillPath(string rule)
        {
            if (ClipRectangle != Rect.Empty)
            {
                if (StrokingColorSpace == m_emptyColor)
                {
                    PageGraphics.FillRectangle(
                    new SolidColorBrush(Colors.Black),
                    ClipRectangle);
                }
                else
                {
                    if (graphiStateOpacity == true)
                    {
                        SolidColorBrush brush = new SolidColorBrush(StrokingColorSpace);
                        brush.Opacity = m_opacity;
                        PageGraphics.FillRectangle(brush, ClipRectangle);
                    }
                    else
                    {
                        PageGraphics.FillRectangle(new SolidColorBrush(StrokingColorSpace), ClipRectangle);
                    }
                }
                ClipRectangle = Rect.Empty;
            }
            if (m_clipRectangleList.Count > 0)
            {
                foreach (Rect rect in m_clipRectangleList)
                {
                    if (graphiStateOpacity)
                    {
                        SolidColorBrush brush = new SolidColorBrush(StrokingColorSpace);
                        brush.Opacity = m_opacity;
                        PageGraphics.FillRectangle(brush, rect);
                    }
                    PageGraphics.FillRectangle(new SolidColorBrush(StrokingColorSpace), rect);
                }
                m_clipRectangleList.Clear();
            }
            else if (Path != null)
            {
                if (m_subPaths.Count > 0)
                {
                    foreach (PathFigure path in m_subPaths)
                    {
                        try
                        {
                            foreach (PathSegment segment in path.Segments)
                            {
                                if (!Path.Segments.Contains(segment))
                                    Path.Segments.Add(segment);
                            }
                        }
                        catch (Exception)
                        {
                            exception.Exceptions.Append("\r\n\r\nExceptions in token f\r\n");
                        }
                    }
                }

                try
                {
                    PathGeometry geom = new PathGeometry();
                    PathFigureCollection col = new PathFigureCollection();
                    col.Add(Path);
                    geom.Figures = col;
                    if (rule == "EvenOdd")
                        geom.FillRule = FillRule.EvenOdd;
                    else
                        geom.FillRule = FillRule.Nonzero;

                    PageGraphics.FillPath(new SolidColorBrush(StrokingColorSpace), geom);
                    m_subPaths.Clear();
                    Path = new PathFigure();
                }
                catch
                {
                    PageGraphics.FillRectangle(
                        new SolidColorBrush(StrokingColorSpace),
                        ClipRectangle);
                    ClipRectangle = Rect.Empty;
                    Path = new PathFigure();
                }
            }
        }

        private void AddLine(string[] line)
        {
            Path.Segments.Add(
                new LineSegment(
                    new Point(double.Parse(line[0]), -double.Parse(line[1]))
                    , true));

            CurrentLocation = new Point(float.Parse(line[0]), -float.Parse(line[1]));
        }

        private void AddBezierCurve(string[] curve)
        {
            Point point1 = new Point(CurrentLocation.X, CurrentLocation.Y);
            Point point2 = new Point(float.Parse(curve[0]), -float.Parse(curve[1]));
            Point point3 = new Point(float.Parse(curve[2]), -float.Parse(curve[3]));
            Point point4 = new Point(float.Parse(curve[4]), -float.Parse(curve[5]));


            Path.Segments.Add(
                new BezierSegment(point2, point3, point4, true));

            CurrentLocation = new Point(float.Parse(curve[4]), -float.Parse(curve[5]));
        }

        private void AddBezierCurve2(string[] curve)
        {
            Point point4 = new Point(float.Parse(curve[2]), -float.Parse(curve[3]));

            QuadraticBezierSegment quadraticbezier = new QuadraticBezierSegment();
            quadraticbezier.Point1 = new Point(float.Parse(curve[0], CultureInfo.InvariantCulture), -float.Parse(curve[1], CultureInfo.InvariantCulture));
            quadraticbezier.Point2 = new Point(float.Parse(curve[2], CultureInfo.InvariantCulture), -float.Parse(curve[3], CultureInfo.InvariantCulture));
            Path.Segments.Add(quadraticbezier);

            CurrentLocation = point4;//new PointF(float.Parse(curve[4]), -float.Parse(curve[5]));
        }

        private void AddBezierCurve3(string[] curve)
        {

            Point point1 = new Point(CurrentLocation.X, CurrentLocation.Y);
            Point point2 = new Point(float.Parse(curve[0]), -float.Parse(curve[1]));
            Point point3 = new Point(float.Parse(curve[2]), -float.Parse(curve[3]));
            Point point4 = new Point(float.Parse(curve[2]), -float.Parse(curve[3]));
            Path.StartPoint = point1;
            Path.Segments.Add(
               new BezierSegment(point2, point3, point4, true));

            CurrentLocation = point4;//new PointF(float.Parse(curve[4]), -float.Parse(curve[5]));
        }

        private void BeginPath(string[] point)
        {
            try
            {
                if (Path != null)
                {
                    m_tempSubPaths.Add(Path);
                    Path = new PathFigure();
                }
                else
                {
                    Path = new PathFigure();
                    m_subPaths.Clear();
                }
            }
            catch
            {
                Path = new PathFigure();
            }
            CurrentLocation = new Point(float.Parse(point[0]), -float.Parse(point[1]));
            Path.StartPoint = CurrentLocation;
        }

        private void GetXObject(string[] xobjectElement)
        {
            try
            {
                graphicDictonaykey = xobjectElement[0].Replace("/", "");
                if (m_resources.ContainsKey(xobjectElement[0].Replace("/", "")))
                {
                    if (m_resources[xobjectElement[0].Replace("/", "")] is XObjectElement)
                        m_graphicsState = (m_resources[xobjectElement[0].Replace("/", "")] as XObjectElement).Render(PageGraphics, m_resources, m_graphicsState);
                    else if (m_resources[xobjectElement[0].Replace("/", "")] is ImageStructure)
                    {
                        ImageStructure structure = m_resources[xobjectElement[0].Replace("/", "")] as ImageStructure;
                        System.Drawing.Image img = structure.EmbeddedImage;
                        Stream imgStream = new MemoryStream();
                        img.Save(imgStream, System.Drawing.Imaging.ImageFormat.Png);
                        imgStream.Position = 0;
                        if (img != null)
                            PageGraphics.DrawImage(imgStream, new Rect(0, 0, 1, 1));
                    }
                }
            }
            catch (Exception exce)
            {
                if (exce.Message.Contains(" does not supported") || exce.Message.Contains("Error in identifying the ImageFilter"))
                {
                    exception.Exceptions.Append("\r\n\r\n" + exce.Message + "\r\n");
                }
                else
                {
                    exception.Exceptions.Append("\r\n\r\n" + exce.Message + exce.StackTrace + "\r\n");
                }
            }
        }

        private string ResolveFontName(string fontName)
        {
            string sInput = fontName;
            if (sInput.Contains("#20"))
                sInput = sInput.Replace("#20", "");
            float sFontValue;
            string[] sReturn = new string[1];
            sReturn[0] = "";
            const string CUPPER = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            int iArrayCount = 0;
            for (int iIndex = 0; iIndex < sInput.Length; iIndex++)
            {
                string sChar = sInput.Substring(iIndex, 1); // get a char
                if ((CUPPER.Contains(sChar)) && (iIndex > 0))
                {
                    if (!CUPPER.Contains(sInput[iIndex - 1].ToString()) || (float.TryParse(sInput[iIndex - 1].ToString(), out sFontValue) && iIndex - 1 == 0))
                    {
                        iArrayCount++;
                        string[] sTemp = new string[iArrayCount + 1];
                        Array.Copy(sReturn, 0, sTemp, 0, iArrayCount);
                        sReturn = sTemp;
                    }
                }
                sReturn[iArrayCount] += sChar;
            }


            fontName = string.Empty;
            foreach (string word in sReturn)
            {
                fontName += word + " ";
            }

            if (fontName.Contains("Times"))
                fontName = "Times New Roman";
            if (fontName == "Bookshelf Symbol Seven")
                fontName = "Bookshelf Symbol 7";

            if (fontName.Contains("Regular"))
            {
                fontName = fontName.Replace("Regular", "");
            }
            else if (fontName.Contains("Bold"))
            {
                fontName = fontName.Replace("Bold", "");
            }
            else if (fontName.Contains("Italic"))
            {
                fontName = fontName.Replace("Italic", "");
            }


            fontName = fontName.Trim();
            return fontName;
        }

        /// <summary>
        /// Creates URI for the given byte array
        /// </summary>
        /// <param name="byteArray">Input byte array</param>
        /// <param name="Size">Size of the byte array</param>
        /// <returns>URI</returns>
        private Uri CreateMemoryResourceUri(byte[] byteArray, int Size)
        {
            MemoryStream packStream = new MemoryStream();
            Package pack = Package.Open(packStream, FileMode.Create, FileAccess.ReadWrite);
            string uri = System.Guid.NewGuid().ToString().Split('-')[0];
            uri = "a" + uri;
            Uri packUri = new Uri(uri + ":");
            PackageStore.AddPackage(packUri, pack);
            Uri packPartUri = new Uri("/AnyAfterSlash", UriKind.Relative);
            PackagePart packPart =
              pack.CreatePart(packPartUri, "AnyBeforeSlash/AnyAfterSlash");
            packPart.GetStream().Write(byteArray, 0, Size);
            Uri memoryResourceURI = PackUriHelper.Create(packUri, packPart.Uri);
            return memoryResourceURI;
        }

        #endregion
    }
}
