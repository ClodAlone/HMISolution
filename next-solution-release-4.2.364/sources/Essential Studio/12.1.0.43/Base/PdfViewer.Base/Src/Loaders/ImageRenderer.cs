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
using Syncfusion.Pdf.Graphics;
using System.Drawing;
using Syncfusion.Pdf;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using Syncfusion.Pdf.Primitives;
#if WINDOWS
using Syncfusion.Windows.Forms.PdfViewer;
#endif
using System.Text.RegularExpressions;

namespace Syncfusion.PdfViewer.Base
{
    internal class ImageRenderer
    {
#if WINDOWS
        internal static List<PageURL> URLDictonary = new List<PageURL>();
        internal static List<PageText> textDictonary = new List<PageText>();
#endif
        public bool IsExtendedGraphicsState = false;
        public bool IsGraphicsState = false;
        public GraphicsPath m_graphicspathtoclip = null;
        internal Matrix transformMatrix = new Matrix();
        internal PointF currentTransformLocation = new PointF();
        char[] m_symbolChars = new char[] { '(', ')', '[', ']', '<', '>' };
        char[] m_startText = new char[] { '(', '[', '<', };
        char[] m_endText = new char[] { ')', ']', '>' };
        PdfPageResources m_resources;
        PdfRecordCollection m_contentElements;
        private PointF m_currentLocation = PointF.Empty;
        private string m_currentFont;
        private bool m_beginText;
        private float m_fontSize;
        private float m_wordSpacing;
        private Graphics m_graphics;
        private RectangleF m_clipRectangle;
        private GraphicsPath m_path;
        private float m_mitterLength;
        private Stack<GraphicsState> m_graphicsState = new Stack<GraphicsState>();
        private Stack<GraphicObjectData> m_objects = new Stack<GraphicObjectData>();
        private float m_textScaling = 100;
        bool textMatrix = false;
        private float m_textAngle = 0;
        private List<GraphicsPath> m_subPaths = new List<GraphicsPath>();
        private List<GraphicsPath> m_tempSubPaths = new List<GraphicsPath>();
        private float m_textElementWidth;
        private PointF m_endTextPosition;
        private bool m_isCurrentPositionChanged;
        private float m_characterSpacing;
        private Color transperentStrokingColor;
        private Color transperentNonStrokingColor;
        private PdfViewerExceptions exception = new PdfViewerExceptions();
        DeviceCMYK decodecmykColor;
        private string[] m_dashedLine;
        private bool isNegativeFont = false;
        private string m_clippingPath;
        private Region clippingregion = null;
        private int RenderingMode = 0;
        private float m_opacity = 0;
        private List<RectangleF> m_clipRectangleList = new List<RectangleF>();
        private bool IsTransparentText = false;
        private List<CffGlyphs> m_glyphDataCollection = new List<CffGlyphs>();

        public ImageRenderer(PdfRecordCollection contentElements, PdfPageResources resources, Graphics g, bool newPage, DeviceCMYK cmyk)
        {
            GraphicObjectData newObject = new GraphicObjectData();
            m_objects.Push(newObject);
            this.m_contentElements = contentElements;
            this.m_resources = resources;
            this.m_graphics = g;
            this.m_graphics.SmoothingMode = SmoothingMode.AntiAlias;
            g.PageUnit = GraphicsUnit.Point;
            if (newPage)
                g.TranslateTransform(0, g.VisibleClipBounds.Bottom);
            decodecmykColor = cmyk;
        }

        public ImageRenderer(PdfRecordCollection contentElements, PdfPageResources resources, Graphics g, bool newPage, float pageBottom, DeviceCMYK cmyk)
        {
            GraphicObjectData newObject = new GraphicObjectData();
            m_objects.Push(newObject);
            this.m_contentElements = contentElements;
            this.m_resources = resources;
            this.m_graphics = g;
            this.m_graphics.SmoothingMode = SmoothingMode.AntiAlias;
            g.PageUnit = GraphicsUnit.Point;
            if (newPage)
                g.TranslateTransform(0, g.VisibleClipBounds.Top + pageBottom);
            decodecmykColor = cmyk;
        }

        public ImageRenderer(PdfRecordCollection contentElements, PdfPageResources resources, Graphics g, bool newPage, float pageBottom, float left, DeviceCMYK cmyk)
        {
            GraphicObjectData newObject = new GraphicObjectData();
            m_objects.Push(newObject);
            this.m_contentElements = contentElements;
            this.m_resources = resources;
            this.m_graphics = g;
            this.m_graphics.SmoothingMode = SmoothingMode.AntiAlias;
            g.PageUnit = GraphicsUnit.Point;
            if (newPage)
                g.TranslateTransform(left, pageBottom);
            decodecmykColor = cmyk;
        }

        public ImageRenderer(PdfRecordCollection contentElements, PdfPageResources resources, Graphics g, bool newPage, int height, DeviceCMYK cmyk)
        {
            GraphicObjectData newObject = new GraphicObjectData();
            m_objects.Push(newObject);
            this.m_contentElements = contentElements;
            this.m_resources = resources;
            this.m_graphics = g;
            this.m_graphics.SmoothingMode = SmoothingMode.AntiAlias;
            g.PageUnit = GraphicsUnit.Point;
            if (newPage)
                g.TranslateTransform(0, height);
            decodecmykColor = cmyk;
        }

        public Graphics PageGraphics
        {
            get
            {
                return m_graphics;
            }
        }

        private PointF CurrentLocation
        {
            get
            {
                return m_currentLocation;
            }
            set
            {
                m_currentLocation = value;
                m_isCurrentPositionChanged = true;
            }
        }

        private Color NonStrokingColorSpace
        {
            get
            {
                if (Objects.NonStrokingColorspace != Color.Empty)
                {
                    return Objects.NonStrokingColorspace;
                }
                else
                {
                    foreach (GraphicObjectData objectData in m_objects)
                    {
                        if (objectData.NonStrokingColorspace != Color.Empty)
                        {
                            return objectData.NonStrokingColorspace;
                        }
                    }
                    return Color.Empty;
                }
            }
        }

        private Color StrokingColorSpace
        {
            get
            {
                if (Objects.StrokingColorspace != Color.Empty)
                {
                    return Objects.StrokingColorspace;
                }
                else
                {
                    foreach (GraphicObjectData objectData in m_objects)
                    {
                        if (objectData.StrokingColorspace != Color.Empty)
                        {
                            return objectData.StrokingColorspace;
                        }
                    }
                    return Color.Empty;
                }
            }
        }

        private RectangleF ClipRectangle
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

        private GraphicsPath Path
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

        private GraphicObjectData Objects
        {
            get
            {
                return m_objects.Peek();
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

        private float TextLeading
        {
            get
            {
                if (Objects.CurrentFont != null)
                {
                    return Objects.TextLeading;
                }
                else
                {
                    float tempTextLeading = 0;
                    foreach (GraphicObjectData objectData in m_objects)
                    {
                        if (objectData.CurrentFont != null)
                        {
                            tempTextLeading = objectData.TextLeading;
                        }
                    }
                    return tempTextLeading;
                }
            }
            set
            {
                Objects.TextLeading = value;
            }
        }

        public void RenderAsImage()
        {
            try
            {
                if (m_contentElements != null)
                {
                    foreach (PdfRecord record in m_contentElements)
                    {
                        string token = record.OperatorName;
                        string[] element = record.Operands;

                        foreach (char ch in m_symbolChars)
                        {
                            if (token.Contains(ch.ToString()))
                                token = token.Replace(ch.ToString(), "");
                        }
                        switch (token.Trim())
                        {
                            case "q":
                                {
                                    GraphicObjectData data = new GraphicObjectData();
                                    m_objects.Push(data);
                                    GraphicsState state = PageGraphics.Save();
                                    m_graphicsState.Push(state);
                                    break;
                                }
                            case "Q":
                                {
                                    m_objects.Pop();
                                    PageGraphics.Restore(m_graphicsState.Pop());
                                    m_graphicspathtoclip = null;
                                    IsGraphicsState = false;
                                    m_characterSpacing = 0;
                                    IsTransparentText = false;
                                    break;
                                }
                            case "Tr":
                                {
                                    RenderingMode = int.Parse(element[0]);
                                    if (float.Parse(element[0]) == 3)
                                    {
                                        transperentNonStrokingColor = Objects.NonStrokingColorspace;
                                        transperentStrokingColor = Objects.StrokingColorspace;
                                        Objects.NonStrokingColorspace = Color.Transparent;
                                        Objects.StrokingColorspace = Color.Transparent;
                                    }
                                    else
                                    {
                                        if (Objects.NonStrokingColorspace == Color.Transparent || Objects.StrokingColorspace == Color.Transparent)
                                        {
                                            Objects.NonStrokingColorspace = transperentNonStrokingColor != Color.Empty ? transperentNonStrokingColor : Color.Black;
                                            Objects.StrokingColorspace = transperentStrokingColor != Color.Empty ? transperentStrokingColor : Color.Black;
                                        }
                                    }
                                    break;
                                }
                            case "Tm":
                                {
                                    //[a b c d e f]
                                    float a = float.Parse(element[0]);
                                    float b = float.Parse(element[1]);
                                    float c = float.Parse(element[2]);
                                    float d = float.Parse(element[3]);
                                    float e = float.Parse(element[4]);
                                    float f = float.Parse(element[5]);

                                    if (textMatrix)
                                    {
                                        PageGraphics.Restore(m_graphicsState.Pop());
                                    }
                                    GraphicsState state = PageGraphics.Save();
                                    m_graphicsState.Push(state);
                                    
                                    PageGraphics.MultiplyTransform(new Matrix(a, -b, -c, d, e, -f));

                                    CurrentLocation = new Point(0, 0);
                                    textMatrix = true;
                                    break;
                                }
                            case "cm":
                                {

                                    //[a b c d e f]
                                    float a = float.Parse(element[0]);
                                    float b = float.Parse(element[1]);
                                    float c = float.Parse(element[2]);
                                    float d = float.Parse(element[3]);
                                    float e = float.Parse(element[4]);
                                    float f = float.Parse(element[5]);

                                    //TranslateTransform [ 1 0 0 1 tx ty ]
                                    //if(e != 0 || f != 0)
                                    {
                                        if ((a != 1 || d != 1) /*&& a != 0 && d != 0*/)
                                        {
                                            if (a == 0 && d == 0)
                                            {
                                                PageGraphics.TranslateTransform(c + e, -(f + d));
                                                PageGraphics.ScaleTransform(-c, b);
                                            }
                                            else
                                            {
                                                if (e == 0 && f == 0)
                                                {
                                                    if (b == 0 && c == 0)
                                                    {
                                                        if (a > 0 && d == 1 && b == 0 && c == 0 && e == 0 & f == 0)
                                                        {
                                                            PageGraphics.ScaleTransform(a, d);
                                                        }
                                                        else
                                                        {
                                                            PageGraphics.TranslateTransform(e, -(f + d));
                                                            PageGraphics.ScaleTransform(a, d);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        PageGraphics.TranslateTransform(e, -(f + d));
                                                    }
                                                }
                                                else
                                                {
                                                    PageGraphics.TranslateTransform(e, -(f + d));
                                                    if ((b <= 0) || (a > 1 && b != -c))
                                                        PageGraphics.ScaleTransform(a, d);
                                                }
                                            }
                                            CurrentLocation = PointF.Empty;
                                        }
                                        else
                                        {
                                            PageGraphics.TranslateTransform(e, -f);
                                            CurrentLocation = PointF.Empty;
                                        }
                                    }

                                    //ScaleTransform [ sx 0 0 sy 0 0 ]
                                    if ((b == c) && (e == f) && (c == e) && (c == 0))
                                    {
                                        //PageGraphics.ScaleTransform(a, d);
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

                                        if (degree == checkDegree)
                                        {
                                            PageGraphics.RotateTransform(-(float)degree);
                                        }
                                        else
                                        {
                                            if( (a == 0 && d == 0)||(a>1&&b!=-c))
                                            {
                                                if (!double.IsNaN(degree))
                                                    PageGraphics.RotateTransform(-(float)degree);
                                            }
                                            else
                                            {
                                                if (!double.IsNaN(checkDegree))
                                                    PageGraphics.RotateTransform(-(float)checkDegree);
                                                else
                                                {
                                                    if (!double.IsNaN(degree))
                                                        PageGraphics.RotateTransform(-(float)degree);
                                                }
                                            }
                                        }

                                    }

                                    break;
                                }
                            case "BT":
                                {
                                    m_beginText = true;
                                    CurrentLocation = PointF.Empty;
                                    break;
                                }
                            case "ET":
                                {
                                    CurrentLocation = PointF.Empty;
                                    if (textMatrix)
                                    {
                                        PageGraphics.Restore(m_graphicsState.Pop());
                                        textMatrix = false;
                                    }
                                    RenderingMode = 0;
                                    break;
                                }
                            case "T*":
                                {
                                    DrawNewLine();
                                    break;
                                }
                            case "TJ":
                                {
                                    RenderTextElementWithSpacing(element, token);
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
                            case "Tf":
                                {
                                    RenderFont(element);
                                    break;
                                }
                            case "TD":
                                {
                                    TextLeading = float.Parse(element[1]);
                                    CurrentLocation = new PointF(CurrentLocation.X + float.Parse(element[0]), CurrentLocation.Y - (float.Parse(element[1])));
                                    break;
                                }
                            case "Td":
                                {
                                    CurrentLocation = new PointF(CurrentLocation.X + float.Parse(element[0]), CurrentLocation.Y - (float.Parse(element[1])));
                                    break;
                                }
                            case "TL":
                                {
                                    TextLeading = float.Parse(element[0]);
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
                            case "RG":
                            case "SC":
                            case "cs":
                            case "SCN":
                                {
                                    GetColorspace(element, "nonstroking", "RGB");
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
                            case "rg":
                            case "sc":
                            case "scn":
                            case "CS":
                                {
                                    if (token == "scn" && element.Length == 4)
                                        GetColorspace(element, "stroking", "DeviceCMYK");
                                    else if (token == "sc" && element.Length == 1)
                                        GetColorspace(element, "stroking", "Gray");
                                    else
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
                            case "Do":
                                {
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
                            case "gs":
                                {
                                    if (this.m_resources.ContainsKey(element[0].Substring(1)))
                                    {
                                        int opm = 0;
                                        string ht = null;
                                        XObjectElement resource = m_resources[element[0].Substring(1)] as XObjectElement;
                                        PdfDictionary resourceDictionary = resource.XObjectDictionary;
                                        if (resourceDictionary.ContainsKey(Syncfusion.Pdf.IO.DictionaryProperties.OPM))
                                            opm = (resourceDictionary[Syncfusion.Pdf.IO.DictionaryProperties.OPM] as PdfNumber).IntValue;
                                        if (resourceDictionary.ContainsKey(Syncfusion.Pdf.IO.DictionaryProperties.HT))
                                        {
                                            PdfName name = resourceDictionary[Syncfusion.Pdf.IO.DictionaryProperties.HT] as PdfName;
                                            if (name != null)
                                            {
                                                ht = name.Value;
                                            }
                                            else
                                            {
                                                PdfReferenceHolder refname = resourceDictionary[Syncfusion.Pdf.IO.DictionaryProperties.HT] as PdfReferenceHolder;
                                                name = refname.Object as PdfName;
                                                if (name != null)
                                                    ht = name.Value;
                                            }
                                        }
                                        else if (resourceDictionary.ContainsKey(Syncfusion.Pdf.IO.DictionaryProperties.CA))
                                        {
                                            m_opacity = (resourceDictionary[Syncfusion.Pdf.IO.DictionaryProperties.CA] as PdfNumber).FloatValue;
                                            IsGraphicsState = true;
                                        }
                                        if (opm == 1 && ht == "Default")
                                            IsExtendedGraphicsState = true;
                                        if (m_opacity == 0 && IsGraphicsState)
                                            IsTransparentText = true;
                                    }
                                    break;
                                }
                            case "b":
                            case "b*":
                                {
                                    if (Path != null)
                                    {
                                        Pen borderPen = new Pen(NonStrokingColorSpace);
                                        borderPen.Width = MitterLength;
                                        Path.CloseFigure();
                                        PageGraphics.DrawPath(borderPen, Path);
                                        if (token.Trim() == "b")
                                            Path.FillMode = FillMode.Winding;
                                        else
                                            Path.FillMode = FillMode.Alternate;

                                        PageGraphics.FillPath(new Pen(StrokingColorSpace).Brush, Path);
                                        if (Path.FillMode == FillMode.Alternate)
                                            FillPath("Alternate");
                                        else
                                            FillPath("Winding");
                                        //DrawPath();
                                    }
                                    ClipRectangle = Rectangle.Empty;
                                    break;
                                }
                            case "B":
                            case "B*":
                                {
                                    if (ClipRectangle == RectangleF.Empty)
                                    {
                                        Pen borderPen = new Pen(NonStrokingColorSpace);
                                        borderPen.Width = MitterLength;
                                        Path.CloseFigure();
                                        if (m_subPaths != null)
                                            if (m_subPaths.Count > 0)
                                            {
                                                foreach (GraphicsPath path in m_subPaths)
                                                {
                                                    if (path.PointCount > 0)
                                                    {
                                                        if (path != Path)
                                                            Path.AddPath(path, true);
                                                    }

                                                }
                                            }
                                        PageGraphics.DrawPath(borderPen, Path);
                                        if (token.Trim() == "B")
                                            Path.FillMode = FillMode.Winding;
                                        else
                                            Path.FillMode = FillMode.Alternate;
                                        PageGraphics.FillPath(new Pen(StrokingColorSpace).Brush, Path);
                                    }
                                    else
                                    {
                                        Pen borderPen = new Pen(NonStrokingColorSpace);
                                        borderPen.Width = MitterLength;
                                        PageGraphics.DrawRectangle(borderPen, ClipRectangle.X, ClipRectangle.Y, ClipRectangle.Width, ClipRectangle.Height);
                                        PageGraphics.FillRectangle(new Pen(StrokingColorSpace).Brush, ClipRectangle.X, ClipRectangle.Y, ClipRectangle.Width, ClipRectangle.Height);
                                    }
                                    Path = new GraphicsPath();
                                    m_subPaths.Clear();
                                    m_tempSubPaths.Clear();
                                    ClipRectangle = Rectangle.Empty;
                                    break;
                                }
                            case "n":
                                {
                                    m_clippingPath = null;
                                    ClipRectangle = RectangleF.Empty;
                                    m_subPaths.Clear();
                                    m_tempSubPaths.Clear();
                                    Path = new GraphicsPath();
                                    m_clipRectangleList.Clear();
                                    break;
                                }
                            case "w":
                                {
                                    m_mitterLength = float.Parse(element[0]);
                                    break;
                                }
                            case "W":
                                {
                                    m_clippingPath = token;
                                    RectangleF clip = PageGraphics.ClipBounds;
                                    if (ClipRectangle != RectangleF.Empty)
                                    {
                                        clip.Intersect(ClipRectangle);
                                        //if (clip != Rectangle.Empty)
                                        ClipRectangle = clip;
                                    }
                                    if (ClipRectangle != RectangleF.Empty)
                                    {
                                        if (m_clippingPath != null || m_clippingPath == "W" || (m_clippingPath == "W*" && m_mitterLength != 0))
                                            PageGraphics.SetClip(ClipRectangle);
                                    }
                                    else if (Path != null)
                                    {
                                        if (Path.PointCount > 0)
                                            m_graphicspathtoclip = Path;
                                        if (Path.PointCount > 0 && m_tempSubPaths.Count == 0)
                                        {
                                            Path.FillMode = FillMode.Winding;
                                            PageGraphics.SetClip(Path);
                                        }
                                        else if (m_subPaths.Count > 0 && m_tempSubPaths.Count == 0)
                                        {
                                            GraphicsPath subpath = new GraphicsPath();
                                            foreach (GraphicsPath path in m_subPaths)
                                            {
                                                if (path.PointCount > 0)
                                                    subpath.AddPath(path, true);
                                            }
                                            subpath.FillMode = FillMode.Winding;
                                            PageGraphics.SetClip(subpath);
                                        }
                                    }
                                    break;
                                }
                            case "W*":
                                {
                                    m_clippingPath = token;
                                    if (ClipRectangle != RectangleF.Empty)
                                    {
                                        if ((m_clippingPath != null || m_clippingPath == "W" || (m_clippingPath == "W*" && m_mitterLength != 0)) && m_clipRectangleList.Count == 0)
                                            PageGraphics.SetClip(ClipRectangle);
                                    }
                                    else if (Path != null)
                                    {
                                        if (Path.PointCount > 0)
                                            m_graphicspathtoclip = Path;
                                        if (Path.PointCount > 0 && m_tempSubPaths.Count == 0)
                                        {
                                            Path.FillMode = FillMode.Alternate;
                                            PageGraphics.SetClip(Path);
                                        }
                                        else if (m_subPaths.Count > 0 && m_tempSubPaths.Count == 0)
                                        {
                                            GraphicsPath subpath = new GraphicsPath();
                                            foreach (GraphicsPath path in m_subPaths)
                                            {
                                                if (path.PointCount > 0)
                                                    subpath.AddPath(path, true);
                                            }
                                            subpath.FillMode = FillMode.Alternate;
                                            PageGraphics.SetClip(subpath);
                                        }
                                    }
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
                                    FillPath("Alternate");
                                    CurrentLocation = PointF.Empty;
                                    break;
                                }
                            case "f":
                                {
                                    FillPath("Winding");
                                    CurrentLocation = PointF.Empty;
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
                                        Path.CloseAllFigures();
                                        m_subPaths.Add(Path);
                                        foreach (GraphicsPath path in m_tempSubPaths)
                                        {
                                            path.CloseAllFigures();
                                            m_subPaths.Add(path);
                                        }
                                        m_tempSubPaths.Clear();
                                        Path = new GraphicsPath();
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
            string text = string.Join("", textElements);

            if (FontSize < 0)
            {
                FontSize = -FontSize;
                isNegativeFont = true;
            }
            if (m_resources.ContainsKey(CurrentFont))
            {
                (m_resources[CurrentFont] as FontStructure).IsSameFont = m_resources.isSameFont();
                if ((m_resources[CurrentFont] as FontStructure).FontSize != FontSize)
                    (m_resources[CurrentFont] as FontStructure).FontSize = FontSize;
                FontStructure structure = m_resources[CurrentFont] as FontStructure;
                text = structure.Decode(text, m_resources.isSameFont());

                TextElement element = new TextElement(text);
                element.FontName = structure.FontName;
                element.FontStyle = structure.FontStyle;
                element.FontSize = FontSize;
                element.TextScaling = m_textScaling;
                element.Font = structure.CurrentFont;
                element.FontEncoding = structure.FontEncoding;
                element.FontGlyphWidths = structure.FontGlyphWidths;
                element.DefaultGlyphWidth = structure.DefaultGlyphWidth;
                element.isNegativeFont = isNegativeFont;
                element.UnicodeCharMapTable = structure.UnicodeCharMapTable;
                Dictionary<int, int> glyphWidths = structure.FontGlyphWidths;
                element.IsType1Font = structure.IsType1Font;
                element.Is1C = structure.Is1C;
                element.IsCID = structure.IsCID;
                element.CharacterMapTable = structure.CharacterMapTable;
                element.ReverseMapTable = structure.ReverseMapTable;
                if (structure.IsType1Font)
                {
                    element.IsType1Font = true;
                    element.differenceTable = structure.differenceTable;
                    element.differenceMappedTable = structure.DifferencesDictionary;
                    element.m_cffGlyphs = structure.m_cffGlyphs;
                    element.OctDecMapTable = structure.OctDecMapTable;
                    if (!m_glyphDataCollection.Contains(element.m_cffGlyphs))
                    {
                        m_glyphDataCollection.Add(element.m_cffGlyphs);
                    }
                }
                if (StrokingColorSpace != Color.Empty)
                {
                    element.BrushColor = StrokingColorSpace;
                }
                else
                {
                    element.BrushColor = Color.Black;
                }
                element.WordSpacing = WordSpacing;
                element.CharacterSpacing = CharacterSpacing;
                if (m_beginText)
                {
                    m_beginText = false;
                }
                if (m_isCurrentPositionChanged)
                {
                    m_isCurrentPositionChanged = false;
                    m_endTextPosition = CurrentLocation;
                    m_textElementWidth = element.Render(PageGraphics, new PointF(m_endTextPosition.X, m_endTextPosition.Y - FontSize), m_textScaling, glyphWidths, structure.Type1GlyphHeight, structure.differenceTable, structure.DifferencesDictionary, structure.differenceEncoding);
                }
                else
                {
                    m_endTextPosition = new PointF(m_endTextPosition.X + m_textElementWidth, m_endTextPosition.Y);
                    m_textElementWidth = element.Render(PageGraphics, new PointF(m_endTextPosition.X, m_endTextPosition.Y - FontSize), m_textScaling, glyphWidths, structure.Type1GlyphHeight, structure.differenceTable, structure.DifferencesDictionary, structure.differenceEncoding);
                }
#if WINDOWS
                string checkURL = element.renderedText;
                if (!structure.IsMappingDone)
                {
                    if (structure.CharacterMapTable != null && structure.CharacterMapTable.Count > 0)
                        checkURL = structure.MapCharactersFromTable(checkURL);
                    else if (structure.DifferencesDictionary != null && structure.DifferencesDictionary.Count > 0)
                        checkURL = structure.MapDifferences(checkURL);
                }
                transformMatrix = PageGraphics.Transform;
                currentTransformLocation = new PointF(CurrentLocation.X, m_endTextPosition.Y);
                float textHeight = CalculateTextHeight();
                PageText txt = new PageText(transformMatrix, checkURL, new PointF(m_endTextPosition.X, (m_endTextPosition.Y - FontSize)), m_textElementWidth, FontSize, element.textFont);
                if (textDictonary.Count > 0)
                {
                    PageText prevtxt = textDictonary[textDictonary.Count - 1];
                    if (prevtxt.CurrentLocation.Y == txt.CurrentLocation.Y && prevtxt.TextFont == txt.TextFont && prevtxt.TransformPoints == prevtxt.TransformPoints && prevtxt.FontSize == txt.FontSize)
                    {
                        string combinestring = prevtxt.Text + txt.Text;
                        PointF currentlocation = prevtxt.CurrentLocation;
                        float width = prevtxt.TextElementWidth + txt.TextElementWidth;
                        PageText newtxt = new PageText(prevtxt.TransformPoints, combinestring, currentlocation, width, prevtxt.FontSize, prevtxt.TextFont);
                        textDictonary.RemoveAt(textDictonary.Count - 1);
                        textDictonary.Add(newtxt);
                    }
                    else
                        textDictonary.Add(txt);
                }
                else
                    textDictonary.Add(txt);
                if (checkURL.Contains("www") || checkURL.Contains("http") || IsValidEmail(checkURL))
                {
                    transformMatrix = PageGraphics.Transform;
                    currentTransformLocation = new PointF(CurrentLocation.X, m_endTextPosition.Y);
                    textHeight = CalculateTextHeight();
                    Regex linkParser = new Regex(@"\b(?:http://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    string urlStr = string.Empty;
                    foreach (Match m in linkParser.Matches(checkURL))
                    {
                        urlStr = m.Value;
                    }
                    if (IsValidEmail(element.renderedText))
                    {
                        urlStr = "mailto:" + checkURL;
                    }
                    PageURL url = new PageURL(transformMatrix, urlStr, new PointF(m_endTextPosition.X, (m_endTextPosition.Y - FontSize)), m_textElementWidth, FontSize);
                    URLDictonary.Add(url);
                }
#endif
            }
        }

        private void RenderTextElementWithLeading(string[] textElements, string tokenType)
        {
            string text = string.Join("", textElements);

            if (m_resources.ContainsKey(CurrentFont))
            {
                (m_resources[CurrentFont] as FontStructure).IsSameFont = m_resources.isSameFont();
                if ((m_resources[CurrentFont] as FontStructure).FontSize != FontSize)
                    (m_resources[CurrentFont] as FontStructure).FontSize = FontSize;
                FontStructure structure = m_resources[CurrentFont] as FontStructure;
                text = structure.Decode(text, m_resources.isSameFont());

                TextElement element = new TextElement(text);
                element.FontName = structure.FontName;
                element.FontStyle = structure.FontStyle;
                element.FontSize = FontSize;
                element.TextScaling = m_textScaling;
                element.Font = structure.CurrentFont;
                element.FontEncoding = structure.FontEncoding;
                element.FontGlyphWidths = structure.FontGlyphWidths;
                element.DefaultGlyphWidth = structure.DefaultGlyphWidth;
                element.Text = text;
                element.IsTransparentText = IsTransparentText;
                element.UnicodeCharMapTable = structure.UnicodeCharMapTable;
                Dictionary<int, int> glyphWidths = structure.FontGlyphWidths;
                System.Drawing.Font embeddedFont = structure.CurrentFont;
                element.IsType1Font = structure.IsType1Font;
                element.Is1C = structure.Is1C;
                element.IsCID = structure.IsCID;
                element.CharacterMapTable = structure.CharacterMapTable;
                element.ReverseMapTable = structure.ReverseMapTable;
                if (structure.IsType1Font)
                {
                    element.IsType1Font = true;
                    element.differenceTable = structure.differenceTable;
                    element.differenceMappedTable = structure.DifferencesDictionary;
                    element.m_cffGlyphs = structure.m_cffGlyphs;
                    if (!m_glyphDataCollection.Contains(element.m_cffGlyphs))
                    {
                        m_glyphDataCollection.Add(element.m_cffGlyphs);
                    }
                }
                if (StrokingColorSpace != Color.Empty)
                {
                    element.BrushColor = StrokingColorSpace;
                }
                else
                {
                    element.BrushColor = Color.Black;
                }
                element.WordSpacing = WordSpacing;
                element.CharacterSpacing = CharacterSpacing;
                if (m_beginText)
                {
                    m_beginText = false;
                }
                if (m_isCurrentPositionChanged)
                {
                    m_isCurrentPositionChanged = false;
                    m_endTextPosition = CurrentLocation;
                    m_textElementWidth = element.Render(PageGraphics, new PointF(m_endTextPosition.X, m_endTextPosition.Y + (TextLeading / 4)), m_textScaling, glyphWidths, structure.Type1GlyphHeight, structure.differenceTable, structure.DifferencesDictionary, structure.differenceEncoding);
                }
                else
                {
                    m_endTextPosition = new PointF(m_endTextPosition.X + m_textElementWidth, m_endTextPosition.Y);
                    m_textElementWidth = element.Render(PageGraphics, new PointF(m_endTextPosition.X, m_endTextPosition.Y + (TextLeading / 4)), m_textScaling, glyphWidths, structure.Type1GlyphHeight, structure.differenceTable, structure.DifferencesDictionary, structure.differenceEncoding);
                }
#if WINDOWS
                string checkURL = element.renderedText;
                if (!structure.IsMappingDone)
                {
                    if (structure.CharacterMapTable != null && structure.CharacterMapTable.Count > 0)
                        checkURL = structure.MapCharactersFromTable(checkURL);
                    else if (structure.DifferencesDictionary != null && structure.DifferencesDictionary.Count > 0)
                        checkURL = structure.MapDifferences(checkURL);
                }
                transformMatrix = PageGraphics.Transform;
                currentTransformLocation = new PointF(CurrentLocation.X, m_endTextPosition.Y);
                float textHeight = CalculateTextHeight();


                PageText txt = new PageText(transformMatrix, checkURL, new PointF(m_endTextPosition.X, m_endTextPosition.Y + (TextLeading / 4)), m_textElementWidth, FontSize, element.textFont);
                textDictonary.Add(txt);

                if (checkURL.Contains("www") || element.renderedText.Contains("http") || IsValidEmail(checkURL))
                {
                    transformMatrix = PageGraphics.Transform;
                    currentTransformLocation = new PointF(CurrentLocation.X, m_endTextPosition.Y);
                    textHeight = CalculateTextHeight();
                    Regex linkParser = new Regex(@"\b(?:http://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    string urlStr = string.Empty;
                    foreach (Match m in linkParser.Matches(checkURL))
                    {
                        urlStr = m.Value;
                    }
                    if (IsValidEmail(element.renderedText))
                    {
                        urlStr = "mailto:" + checkURL;
                    }
                    PageURL url = new PageURL(transformMatrix, urlStr, new PointF(m_endTextPosition.X, m_endTextPosition.Y + (TextLeading / 4)), m_textElementWidth, FontSize);
                    URLDictonary.Add(url);
                }
#endif
                DrawNewLine();
            }
        }

        private void RenderTextElementWithSpacing(string[] textElements, string tokenType)
        {
            List<string> decodedList = new List<string>();
            string text = string.Join("", textElements);
            string tempText = text;
            if (m_resources.ContainsKey(CurrentFont))
            {
                (m_resources[CurrentFont] as FontStructure).IsSameFont = m_resources.isSameFont();
                if ((m_resources[CurrentFont] as FontStructure).FontSize != FontSize)
                    (m_resources[CurrentFont] as FontStructure).FontSize = FontSize;
                FontStructure structure = m_resources[CurrentFont] as FontStructure;

                List<float> characterSpacings = null;//GetCharacterSpacings(tempText);
                decodedList = structure.DecodeTextTJ(text, m_resources.isSameFont());

                TextElement element = new TextElement(text);
                element.FontName = structure.FontName;
                element.FontStyle = structure.FontStyle;
                element.FontSize = FontSize;
                element.TextScaling = m_textScaling;
                element.Font = structure.CurrentFont;
                element.FontEncoding = structure.FontEncoding;
                element.FontGlyphWidths = structure.FontGlyphWidths;
                element.DefaultGlyphWidth = structure.DefaultGlyphWidth;
                element.RenderingMode = RenderingMode;
                element.UnicodeCharMapTable = structure.UnicodeCharMapTable;
                Dictionary<int, int> glyphWidths = structure.FontGlyphWidths;
                element.CidToGidReverseMapTable = structure.CidToGidReverseMapTable;
                element.IsType1Font = structure.IsType1Font;
                element.Is1C = structure.Is1C;
                element.IsCID = structure.IsCID;
                element.CharacterMapTable = structure.CharacterMapTable;
                element.ReverseMapTable = structure.ReverseMapTable;
                if (structure.IsType1Font)
                {
                    element.IsType1Font = true;
                    element.differenceTable = structure.differenceTable;
                    element.differenceMappedTable = structure.DifferencesDictionary;
                    element.m_cffGlyphs = structure.m_cffGlyphs;
                    if (!m_glyphDataCollection.Contains(element.m_cffGlyphs))
                    {
                        m_glyphDataCollection.Add(element.m_cffGlyphs);
                    }
                }
                if (StrokingColorSpace != Color.Empty)
                {
                    element.BrushColor = StrokingColorSpace;
                }
                else
                {
                    element.BrushColor = Color.Black;
                }
                element.WordSpacing = WordSpacing;
                element.CharacterSpacing = CharacterSpacing;
                if (m_beginText)
                {
                    m_beginText = false;
                }
                if (m_isCurrentPositionChanged)
                {
                    m_isCurrentPositionChanged = false;
                    m_endTextPosition = CurrentLocation;

                    m_textElementWidth = element.RenderWithSpace(PageGraphics, new PointF(m_endTextPosition.X, m_endTextPosition.Y - FontSize), decodedList, characterSpacings, m_textScaling, glyphWidths, structure.Type1GlyphHeight, structure.differenceTable, structure.DifferencesDictionary, structure.differenceEncoding);
                }
                else
                {
                    m_endTextPosition = new PointF(m_endTextPosition.X + m_textElementWidth, m_endTextPosition.Y);
                    m_textElementWidth = element.RenderWithSpace(PageGraphics, new PointF(m_endTextPosition.X, m_endTextPosition.Y - FontSize), decodedList, characterSpacings, m_textScaling, glyphWidths, structure.Type1GlyphHeight, structure.differenceTable, structure.DifferencesDictionary, structure.differenceEncoding);
                }
#if WINDOWS
                string checkURL = element.renderedText;
                if (!structure.IsMappingDone)
                {
                    if (structure.CharacterMapTable != null && structure.CharacterMapTable.Count > 0)
                        checkURL = structure.MapCharactersFromTable(checkURL);
                    else if (structure.DifferencesDictionary != null && structure.DifferencesDictionary.Count > 0)
                        checkURL = structure.MapDifferences(checkURL);
                }

                transformMatrix = PageGraphics.Transform;
                currentTransformLocation = new PointF((CurrentLocation.X), (m_endTextPosition.Y));
                float textHeight = CalculateTextHeight();

                if (element.textFont == null)
                {
                    element.CheckFontStyle(element.FontName);
                    string FontName = FontStructure.CheckFontName(element.FontName);
                    Font txtFont = new Font(FontName, element.FontSize, element.FontStyle);
                }


                PageText txt;
                if ((m_endTextPosition.Y + (TextLeading - FontSize)) != 0)
                {

                    if (element.textFont == null)
                    {
                        element.CheckFontStyle(element.FontName);
                        string FontName = FontStructure.CheckFontName(element.FontName);
                        Font txtFont = new Font(FontName, element.FontSize, element.FontStyle);
                        txt = new PageText(transformMatrix, checkURL, new PointF(m_endTextPosition.X, m_endTextPosition.Y - FontSize), m_textElementWidth, FontSize, txtFont);
                    }
                    else
                    {
                        txt = new PageText(transformMatrix, checkURL, new PointF(m_endTextPosition.X, m_endTextPosition.Y - FontSize), m_textElementWidth, FontSize, element.textFont);
                    }
                }
                else
                {

                    txt = new PageText(transformMatrix, checkURL, new PointF(m_endTextPosition.X, (m_endTextPosition.Y)), m_textElementWidth, FontSize, element.textFont);
                }
                textDictonary.Add(txt);


                if (checkURL.Contains("www") || checkURL.Contains("http") || IsValidEmail(checkURL))
                {
                    float textLeading = TextLeading;

                    transformMatrix = PageGraphics.Transform;
                    currentTransformLocation = new PointF((CurrentLocation.X), (m_endTextPosition.Y));
                    textHeight = CalculateTextHeight();

                    Regex linkParser = new Regex(@"\b(?:http://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    string urlStr = string.Empty;
                    foreach (Match m in linkParser.Matches(checkURL))
                    {
                        urlStr = m.Value;
                    }

                    if (IsValidEmail(element.renderedText))
                    {
                        urlStr = "mailto:" + checkURL;
                    }

                    PageURL url;
                    if ((m_endTextPosition.Y + (TextLeading - FontSize)) != 0)
                    {
                        url = new PageURL(transformMatrix, urlStr, new PointF(m_endTextPosition.X, m_endTextPosition.Y - FontSize), m_textElementWidth, FontSize);
                    }
                    else
                    {
                        url = new PageURL(transformMatrix, urlStr, new PointF(m_endTextPosition.X, (m_endTextPosition.Y)), m_textElementWidth, FontSize);
                    }
                    URLDictonary.Add(url);
                }
#endif
            }

        }

        /// <summary>
        /// Determining valid email address
        /// </summary>
        /// <param name="email">email address to validate</param>
        /// <returns>true is valid, false if not valid</returns>
        public bool IsValidEmail(string email)
        {
            string pattern = @"^[-a-zA-Z0-9][-.a-zA-Z0-9]*@[-.a-zA-Z0-9]+(\.[-.a-zA-Z0-9]+)*\.(com|edu|info|gov|int|mil|net|org|biz|name|museum|coop|aero|pro|tv|[a-zA-Z]{2})$";
            //Regular expression object
            Regex check = new Regex(pattern, RegexOptions.IgnorePatternWhitespace);
            //boolean variable to return to calling method
            bool valid = false;

            //make sure an email address was provided
            if (string.IsNullOrEmpty(email))
            {
                valid = false;
            }
            else
            {
                //use IsMatch to validate the address
                valid = check.IsMatch(email);
            }
            //return the value to the calling method
            return valid;
        }

        private void GetColorspace(string[] colorElement, string type, string colorSpace)
        {
            float r, g, b;
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
                    Objects.NonStrokingColorspace = Color.FromArgb(255, (int)r, (int)g, (int)b);
                else
                    Objects.StrokingColorspace = Color.FromArgb(255, (int)r, (int)g, (int)b);
                return;
            }
            else
            {
                r = g = b = 0;
            }

            if (type == "nonstroking")
                Objects.NonStrokingColorspace = Color.FromArgb((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
            else
                Objects.StrokingColorspace = Color.FromArgb((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
        }

        private void DrawNewLine()
        {
            m_isCurrentPositionChanged = true;
            FontStructure fontStructure = m_resources[CurrentFont] as FontStructure;
            Font font = new Font(TextElement.CheckFontName(fontStructure.FontName), FontSize, fontStructure.FontStyle);
            if (TextLeading != 0)
                m_currentLocation.Y = (TextLeading < 0 ? m_currentLocation.Y - TextLeading : m_currentLocation.Y + TextLeading);
            else
                m_currentLocation.Y += font.Size;
            font.Dispose();
        }

        private void GetClipRectangle(string[] rectangle)
        {
            float x = float.Parse(rectangle[0]);
            float y = -float.Parse(rectangle[1]);
            float width = float.Parse(rectangle[2]);
            float height = -float.Parse(rectangle[3]);
            RectangleF currentRect = new RectangleF(x, y, width, height);

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

            if ((ClipRectangle != new RectangleF(0, 0, 0, 0)) && !ClipRectangle.IsEmpty)
            {
                m_clipRectangleList.Add(m_clipRectangle);
            }

            if ((ClipRectangle != new RectangleF(0, 0, 0, 0)) && !ClipRectangle.IsEmpty)
            {
                m_clipRectangle = new RectangleF(x, y, width, height);
            }
            else
            {
                m_clipRectangle = new RectangleF(x, y, width, height);
            }
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
            if (m_textScaling < 0)
                m_textScaling = -m_textScaling;
        }

        private void DrawPath()
        {
            Pen pathPen = new Pen(NonStrokingColorSpace == Color.Empty ? Color.Black : NonStrokingColorSpace);
            if (MitterLength != 0)
                pathPen.Width = MitterLength;
            if (m_dashedLine != null)
            {
                string dash = m_dashedLine[0];
                dash = dash.Substring(1, dash.Length - 2);
                dash = dash.Trim();
                List<string> dashlist = new List<string>();
                string[] dasharray = dash.Split(' ');
                foreach (string str in dasharray)
                {
                    if (str != "" && str != "0")
                        dashlist.Add(str);
                }
                float[] dashpattern = new float[dashlist.Count];
                for (int x = 0; x < dashlist.Count; x++)
                {
                    if (dashlist[x] != "")
                        dashpattern[x] = float.Parse(dashlist[x]);
                }
                if (dashpattern.Length > 0 && m_mitterLength < dashpattern[0])
                {
                    if (m_mitterLength != 0)
                    {
                        for (int i = 0; i < dashpattern.Length; i++)
                        {
                            dashpattern[i] = dashpattern[i] / m_mitterLength;
                        }
                    }
                }
                if (dashpattern.Length > 0)
                    pathPen.DashPattern = dashpattern;
                m_dashedLine = null;
            }
            if (ClipRectangle != RectangleF.Empty)
            {
                if (IsExtendedGraphicsState)
                    PageGraphics.DrawRectangle(pathPen, ClipRectangle.X, ClipRectangle.Y, ClipRectangle.Width, ClipRectangle.Height);
                else if (IsGraphicsState)
                {
                    if (m_opacity != 0)
                        PageGraphics.DrawRectangle(pathPen, ClipRectangle.X, ClipRectangle.Y, ClipRectangle.Width, ClipRectangle.Height);
                }
                else
                    PageGraphics.DrawRectangle(pathPen, ClipRectangle.X, ClipRectangle.Y, ClipRectangle.Width, ClipRectangle.Height);
                ClipRectangle = RectangleF.Empty;
            }
            foreach (RectangleF rect in m_clipRectangleList)
            {
                if (IsExtendedGraphicsState)
                    PageGraphics.DrawRectangle(pathPen, rect.X, rect.Y, rect.Width, rect.Height);
                else if (IsGraphicsState)
                {
                    if (m_opacity != 0)
                        PageGraphics.DrawRectangle(pathPen, rect.X, rect.Y, rect.Width, rect.Height);
                }
                else
                    PageGraphics.DrawRectangle(pathPen, rect.X, rect.Y, rect.Width, rect.Height);
            }
            m_clipRectangleList.Clear();
            if (Path != null)
            {
                if (m_subPaths.Count > 0)
                {
                    foreach (GraphicsPath path in m_subPaths)
                    {
                        try
                        {
                            if (path.PointCount > 0)
                            {
                                Path.AddPath(path, true);
                            }
                        }
                        catch
                        {
                            if (path != null)
                                Path.AddPath(path, false);
                        }
                    }
                }
            }
            if (Path != null && m_tempSubPaths.Count > 0)
            {
                if (m_tempSubPaths.Count > 0)
                {
                    foreach (GraphicsPath path in m_tempSubPaths)
                    {
                        try
                        {
                            if (path != null && path.PointCount > 0)
                            {
                                //Path.AddPath(path, true);
                                if (IsExtendedGraphicsState)
                                    PageGraphics.DrawPath(pathPen, path);
                                else if (IsGraphicsState)
                                {
                                    if (m_opacity != 0)
                                        PageGraphics.DrawPath(pathPen, path);
                                }
                                else
                                    PageGraphics.DrawPath(pathPen, path);
                            }
                        }
                        catch
                        {
                            if (path != null && path.PointCount > 0)
                                Path.AddPath(path, false);
                        }
                    }
                    m_tempSubPaths.Clear();
                }

            }
            if (Path != null && Path.PointCount > 0)
            {
                if (IsExtendedGraphicsState)
                    PageGraphics.DrawPath(pathPen, Path);
                else if (IsGraphicsState)
                {
                    if (m_opacity != 0)
                        PageGraphics.DrawPath(pathPen, Path);
                }
                else
                    PageGraphics.DrawPath(pathPen, Path);
                m_subPaths.Clear();
                Path.Reset();
            }

        }

        private void FillPath(string mode)
        {
            Color ColorBrush = StrokingColorSpace;
            bool IsContainPathPoint = false;
            if (StrokingColorSpace == Color.Empty)
                ColorBrush = Color.Black;

            Rectangle emptyRectangle = new Rectangle(0, 0, 0, 0);
            if (ClipRectangle != RectangleF.Empty && ClipRectangle != emptyRectangle)
            {
                if (m_graphicspathtoclip != null && IsExtendedGraphicsState)
                {
                    clippingregion = new Region(m_graphicspathtoclip);
                    clippingregion.Intersect(ClipRectangle);
                    PageGraphics.FillRegion(new Pen(ColorBrush).Brush, clippingregion);
                }
                else
                {
                    if (IsGraphicsState)
                    {
                        Color BrushColor = new Pen(ColorBrush).Color;
                        float alpha = Math.Max(0.0f, Math.Min(1.0f, m_opacity));
                        int a = (int)Math.Floor(alpha == 1.0f ? 255f : alpha * 255.0f);
                        BrushColor = Color.FromArgb(a, BrushColor.R, BrushColor.G, BrushColor.B);
                        PageGraphics.FillRectangle(new Pen(BrushColor).Brush, ClipRectangle);
                    }
                    else
                    {
                        if (m_subPaths.Count != 0)
                        {
                            foreach (GraphicsPath path in m_subPaths)
                            {
                                if (path.PointCount > 0)
                                {
                                    IsContainPathPoint = true;
                                    try
                                    {
                                        if (path != Path)
                                            Path.AddPath(path, true);
                                    }
                                    catch
                                    {
                                        Path.AddPath(path, false);
                                    }
                                }
                            }
                            if (IsContainPathPoint)
                            {
                                Region tempRecRegion = new Region(ClipRectangle);
                                Region tempPathRegion = new Region(Path);
                                tempPathRegion.Intersect(tempRecRegion);
                                PageGraphics.SetClip(tempPathRegion, CombineMode.Exclude);
                                PageGraphics.FillRegion(new Pen(ColorBrush).Brush, tempRecRegion);
                                Path.Reset();
                            }
                            else
                                PageGraphics.FillRectangle(new Pen(ColorBrush).Brush, ClipRectangle);
                        }
                        else
                            PageGraphics.FillRectangle(new Pen(ColorBrush).Brush, ClipRectangle);
                    }
                }
                ClipRectangle = RectangleF.Empty;
            }
            foreach (RectangleF rect in m_clipRectangleList)
            {
                if (m_graphicspathtoclip != null && IsExtendedGraphicsState)
                {
                    clippingregion = new Region(m_graphicspathtoclip);
                    clippingregion.Intersect(rect);
                    PageGraphics.FillRegion(new Pen(ColorBrush).Brush, clippingregion);
                }
                else
                {
                    PageGraphics.FillRectangle(new Pen(ColorBrush).Brush, rect);
                }
            }
            m_clipRectangleList.Clear();
            if (m_subPaths.Count > 0)
            {
                foreach (GraphicsPath temp in m_tempSubPaths)
                {
                    temp.CloseAllFigures();
                    m_subPaths.Add(temp);
                }
                m_tempSubPaths.Clear();
            }
            if (Path != null)
            {
                if (m_subPaths.Count > 0)
                {
                    foreach (GraphicsPath path in m_subPaths)
                    {
                        if (path.PointCount > 0)
                        {
                            try
                            {
                                if (path != Path)
                                    Path.AddPath(path, true);
                            }
                            catch
                            {
                                Path.AddPath(path, false);
                            }
                        }

                    }
                }
                else if (m_tempSubPaths.Count > 0)
                {
                    if (m_tempSubPaths.Count > 0)
                    {
                        foreach (GraphicsPath path in m_tempSubPaths)
                        {
                            try
                            {
                                if (Path == null)
                                {
                                    Path = new GraphicsPath();
                                }
                                if (path != null && path.PointCount > 0)
                                {
                                    path.CloseAllFigures();
                                    Path.AddPath(path, true);
                                }
                            }
                            catch
                            {
                                if (path != null && path.PointCount > 0)
                                {
                                    Path = new GraphicsPath();
                                    Path.AddPath(path, true);
                                }
                            }
                        }
                        m_tempSubPaths.Clear();
                    }
                }
                try
                {
                    if (Path.PointCount > 0)
                    {
                        if (mode == "Alternate")
                            Path.FillMode = FillMode.Alternate;
                        else
                            Path.FillMode = FillMode.Winding;
                        if (IsExtendedGraphicsState)
                        {
                            if (clippingregion == null)
                                PageGraphics.FillPath(new Pen(Color.Transparent).Brush, Path);
                            else
                            {
                                Region region = new Region(m_graphicspathtoclip);
                                region.Intersect(Path);
                                PageGraphics.FillRegion(new Pen(ColorBrush).Brush, region);
                            }
                        }
                        else
                        {
                            if (IsGraphicsState)
                            {
                                Color BrushColor = new Pen(ColorBrush).Color;
                                float alpha = Math.Max(0.0f, Math.Min(1.0f, m_opacity));
                                int a = (int)Math.Floor(alpha == 1.0f ? 255f : alpha * 255.0f);
                                BrushColor = Color.FromArgb(a, BrushColor.R, BrushColor.G, BrushColor.B);
                                PageGraphics.FillPath(new Pen(BrushColor).Brush, Path);
                            }
                            else
                                PageGraphics.FillPath(new Pen(ColorBrush).Brush, Path);
                        }
                        Path.Reset();
                    }
                }
                catch
                {
                    PageGraphics.FillRectangle(new Pen(ColorBrush).Brush, ClipRectangle);
                    ClipRectangle = RectangleF.Empty;
                }
                m_subPaths.Clear();
            }

        }

        private void AddLine(string[] line)
        {
            Path.AddLine(CurrentLocation.X, CurrentLocation.Y, float.Parse(line[0]), -float.Parse(line[1]));
            CurrentLocation = new PointF(float.Parse(line[0]), -float.Parse(line[1]));
        }

        private void AddBezierCurve(string[] curve)
        {
            PointF point1 = new PointF(CurrentLocation.X, CurrentLocation.Y);
            PointF point2 = new PointF(float.Parse(curve[0]), -float.Parse(curve[1]));
            PointF point3 = new PointF(float.Parse(curve[2]), -float.Parse(curve[3]));
            PointF point4 = new PointF(float.Parse(curve[4]), -float.Parse(curve[5]));
            Path.AddBezier(point1, point2, point3, point4);

            CurrentLocation = new PointF(float.Parse(curve[4]), -float.Parse(curve[5]));
        }

        private void AddBezierCurve2(string[] curve)
        {
            PointF point1 = new PointF(CurrentLocation.X, CurrentLocation.Y);
            PointF point2 = new PointF(CurrentLocation.X, CurrentLocation.Y);
            PointF point3 = new PointF(float.Parse(curve[0]), -float.Parse(curve[1]));
            PointF point4 = new PointF(float.Parse(curve[2]), -float.Parse(curve[3]));
            Path.AddBezier(point1, point2, point3, point4);

            CurrentLocation = point4;//new PointF(float.Parse(curve[4]), -float.Parse(curve[5]));
        }

        private void AddBezierCurve3(string[] curve)
        {
            PointF point1 = new PointF(CurrentLocation.X, CurrentLocation.Y);
            PointF point2 = new PointF(float.Parse(curve[0]), -float.Parse(curve[1]));
            PointF point3 = new PointF(float.Parse(curve[2]), -float.Parse(curve[3]));
            PointF point4 = new PointF(float.Parse(curve[2]), -float.Parse(curve[3]));
            Path.AddBezier(point1, point2, point3, point4);

            CurrentLocation = point4;//new PointF(float.Parse(curve[4]), -float.Parse(curve[5]));
        }

        private void BeginPath(string[] point)
        {
            try
            {
                if (Path != null && Path.PointCount > 0)
                {
                    m_tempSubPaths.Add(Path);

                    Path = new GraphicsPath();
                }
                else
                {
                    Path = new GraphicsPath();
                }
            }
            catch
            {
                Path = new GraphicsPath();
            }

            CurrentLocation = new PointF(float.Parse(point[0]), -float.Parse(point[1]));
        }

        private void GetXObject(string[] xobjectElement)
        {
            if (m_resources.ContainsKey(xobjectElement[0].Replace("/", "")))
            {
                if (m_resources[xobjectElement[0].Replace("/", "")] is XObjectElement)
                    m_graphicsState = (m_resources[xobjectElement[0].Replace("/", "")] as XObjectElement).Render(PageGraphics, m_resources, m_graphicsState);
                else if (m_resources[xobjectElement[0].Replace("/", "")] is ImageStructure)
                {
                    ImageStructure structure = m_resources[xobjectElement[0].Replace("/", "")] as ImageStructure;
                    Image img = structure.EmbeddedImage;
                    if (img != null)
                        PageGraphics.DrawImage(img, new RectangleF(0, 0, 1, 1));
                }
            }
        }
        /// <summary>
        /// Calculates the height of text.
        /// </summary>
        private float CalculateTextHeight()
        {
            float height = 0;
            FontStructure fontStructure = m_resources[CurrentFont] as FontStructure;
            Font font = new Font(TextElement.CheckFontName(fontStructure.FontName), FontSize, fontStructure.FontStyle);
            height = font.Size;
            font.Dispose();
            return height;
        }

    }
}
