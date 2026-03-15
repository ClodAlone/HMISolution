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
using System.Globalization;
using System.IO;
using System.Text;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Graphics.Fonts;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;
using System.Collections.Generic;

/// <summary>
/// The Syncfusion.Pdf.Parsing namespace contains classes, which are used to load or modify an existing PDF document.
/// </summary>
namespace Syncfusion.Pdf.Parsing
{
    /// <summary>
    /// Represents loaded styled field.
    /// </summary>
    /// <seealso cref="PdfLoadedField"/> Class
    public class PdfLoadedStyledField : PdfLoadedField
    {
        #region Constants
        /// <summary>
        /// Internal variable to store color shift value.
        /// </summary>
        private const byte ShadowShift = 64;
        #endregion

        #region Fields
        /// <summary>
        /// Internal variable to store Pdf Field Actions.
        /// </summary>
        private PdfFieldActions m_actions = null;
        /// <summary>
        /// Internal variable to store widget of the field.
        /// </summary>
        private WidgetAnnotation m_widget = new WidgetAnnotation();
        /// <summary>
        /// Internal variable to store enter action.
        /// </summary>
        private PdfAction m_mouseEnter = null;
        /// <summary>
        /// Internal variable to store leave action.
        /// </summary>
        private PdfAction m_mouseLeave = null;
        /// <summary>
        /// Internal variable to store mouse down action.
        /// </summary>
        private PdfAction m_mouseDown = null;
        /// <summary>
        /// Internal variable to store mouse up action.
        /// </summary>
        private PdfAction m_mouseUp = null;
        /// <summary>
        /// Internal variable to store get focus action.
        /// </summary>
        private PdfAction m_gotFocus = null;
        /// <summary>
        /// Internal variable to store lost focus action.
        /// </summary>
        private PdfAction m_lostFocus = null;
        /// <summary>
        /// Internal variable to store border pen.
        /// </summary>
        private PdfPen m_borderPen = null;
        /// <summary>
        /// Internal variable to store field's font.
        /// </summary>
        private PdfFont m_font = null;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the action to be performed when the cursor enters the annotation�s 
        /// active area.
        /// </summary>
        /// <value>The mouse enter action.</value>
        public PdfAction MouseEnter
        {
            get
            {
                return m_mouseEnter;
            }
            set
            {
                if (value != null)
                {
                    m_mouseEnter = value;
                    m_actions = new PdfFieldActions(Widget.Actions);
                    PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);
                    widget.SetProperty(DictionaryProperties.AA, m_actions);
                    PdfDictionary appearance = CrossTable.GetObject(widget[DictionaryProperties.AA]) as PdfDictionary;
                    appearance.SetProperty(DictionaryProperties.E, m_mouseEnter);
                    widget.SetProperty(DictionaryProperties.AA, appearance);
                    Changed = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the action to be performed when the mouse button is released 
        /// inside the annotation�s active area..
        /// </summary>
        /// <value>The mouse up action.</value>
        public PdfAction MouseUp
        {
            get
            {
                return m_mouseUp;
            }

            set
            {
                if (value != null)
                {
                    m_mouseUp = value;
                    m_actions = new PdfFieldActions(Widget.Actions);
                    PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);
                    widget.SetProperty(DictionaryProperties.AA, m_actions);
                    PdfDictionary appearance = CrossTable.GetObject(widget[DictionaryProperties.AA]) as PdfDictionary;
                    appearance.SetProperty(DictionaryProperties.U, m_mouseUp);
                    widget.SetProperty(DictionaryProperties.AA, appearance);
                    Changed = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the action to be performed when the mouse button is pressed inside the 
        /// annotation�s active area.
        /// </summary>
        /// <value>The mouse down action.</value>
        public PdfAction MouseDown
        {
            get
            {
                return m_mouseDown;
            }

            set
            {
                if (value != null)
                {
                    m_mouseDown = value;
                    m_actions = new PdfFieldActions(Widget.Actions);
                    PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);
                    widget.SetProperty(DictionaryProperties.AA, m_actions);
                    PdfDictionary appearance = CrossTable.GetObject(widget[DictionaryProperties.AA]) as PdfDictionary;
                    appearance.SetProperty(DictionaryProperties.D, m_mouseDown);
                    widget.SetProperty(DictionaryProperties.AA, appearance);
                    Changed = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the action to be performed when the cursor exits the annotation�s 
        /// active area.
        /// </summary>
        /// <value>The mouse leave action.</value>
        public PdfAction MouseLeave
        {
            get
            {
                return m_mouseLeave;
            }

            set
            {
                if (value != null)
                {
                    m_mouseLeave = value;
                    m_actions = new PdfFieldActions(Widget.Actions);
                    PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);
                    widget.SetProperty(DictionaryProperties.AA, m_actions);
                    PdfDictionary appearance = CrossTable.GetObject(widget[DictionaryProperties.AA]) as PdfDictionary;
                    appearance.SetProperty(DictionaryProperties.X, m_mouseLeave);
                    widget.SetProperty(DictionaryProperties.AA, appearance);
                    Changed = true;
                }
            }
        }


        /// <summary>
        /// Gets or sets the action to be performed when the annotation receives the 
        /// input focus.
        /// </summary>
        /// <value>The got focus action.</value>
        public PdfAction GotFocus
        {
            get
            {
                return m_gotFocus;
            }
            set
            {
                if (value != null)
                {
                    m_gotFocus = value;
                    m_actions = new PdfFieldActions(Widget.Actions);
                    PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);
                    widget.SetProperty(DictionaryProperties.AA, m_actions);
                    PdfDictionary appearance = CrossTable.GetObject(widget[DictionaryProperties.AA]) as PdfDictionary;
                    appearance.SetProperty(DictionaryProperties.Fo, m_gotFocus);
                    widget.SetProperty(DictionaryProperties.AA, appearance);
                    Changed = true;
                }
            }
        }

        /// <summary>
        /// Gets the fore color of the Field.
        /// </summary>
        /// <value>The color of the text.</value>
        private PdfColor ForeColor
        {
            get
            {
                PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);
                PdfColor color = new PdfColor(0, 0, 0);
                if ((widget != null) && (widget.ContainsKey(DictionaryProperties.DA)))
                {
                    PdfString defaultAppearance = CrossTable.GetObject(widget[DictionaryProperties.DA]) as PdfString;

                    color = GetForeColour(defaultAppearance.Value);
                }
                return color;
            }
        }

        /// <summary>
        /// Gets or sets the action to be performed when the annotation loses the 
        /// input focus.
        /// </summary>
        /// <value>The lost focus action.</value>
        public PdfAction LostFocus
        {
            get
            {
                return m_lostFocus;
            }
            set
            {
                if (value != null)
                {
                    m_lostFocus = value;
                    m_actions = new PdfFieldActions(Widget.Actions);
                    PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);
                    widget.SetProperty(DictionaryProperties.AA, m_actions);
                    PdfDictionary appearance = CrossTable.GetObject(widget[DictionaryProperties.AA]) as PdfDictionary;
                    appearance.SetProperty(DictionaryProperties.Bl, m_lostFocus);
                    widget.SetProperty(DictionaryProperties.AA, appearance);
                    Changed = true;
                }
            }
        }


        /// <summary>
        /// Gets the widget.
        /// </summary>
        /// <value>The widget.</value>
        internal WidgetAnnotation Widget
        {
            get
            {
                return m_widget;
            }
        }

        /// <summary>
        /// Gets or sets the bounds.
        /// </summary>
        public RectangleF Bounds
        {
            get
            {
                RectangleF rect = GetBounds(Dictionary, CrossTable);

                if (rect.Y > 0)
                {
                    if (Page != null)
                        rect.Y = Page.Size.Height - (rect.Y + rect.Height);
                    else
                        rect.Y = (rect.Y + rect.Height);
                }
                else
                    rect.Y = -(rect.Y+ rect.Height);

                return rect;
            }
            set
            {
                RectangleF rect = value;

                if (rect == RectangleF.Empty)
                    throw new ArgumentNullException("rectangle");

                float height = Page.Size.Height;

                PdfNumber[] values = new PdfNumber[]{
					new PdfNumber(rect.X),
					new PdfNumber(height-(rect.Y+rect.Height)),
					new PdfNumber(rect.X+rect.Width),
					new PdfNumber(height - rect.Y)};

                PdfDictionary dic = Dictionary;

                if (!dic.ContainsKey(DictionaryProperties.Rect))
                {
                    dic = GetWidgetAnnotation(Dictionary, CrossTable);
                }

                dic.SetArray(DictionaryProperties.Rect, values);

                Changed = true;
            }
        }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        public PointF Location
        {
            get
            {
                return Bounds.Location;
            }
            set
            {
                Bounds = new RectangleF(value, Bounds.Size);
            }
        }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        public SizeF Size
        {
            get
            {
                return Bounds.Size;
            }
            set
            {
                Bounds = new RectangleF(Bounds.Location, value);
            }
        }

        /// <summary>
        /// Gets the border pen.
        /// </summary>
        internal PdfPen BorderPen
        {
            get
            {
                return GetBorderPen();
            }
        }

        /// <summary>
        /// Gets or sets the color of the border.
        /// </summary>
        /// <value>The color of the border.</value>
        public PdfBorderStyle BorderStyle
        {
            get
            {
                return GetBorderStyle();
            }
            set
            {

                SetBorderStyle(value);
                CreateBorderPen();
            }
        }

        /// <summary>
        /// Gets or sets the color of the border.
        /// </summary>
        /// <value>The color of the border.</value>
        public PdfColor BorderColor
        {
            get
            {
                return m_widget.WidgetAppearance.BorderColor;
            }
            set
            {
                (this as PdfField).Form.SetAppearanceDictionary = true;
                m_widget.WidgetAppearance.BorderColor = value;
                SetBorderColor(value);
            }
        }

        /// <summary>
        /// Gets the DashPatern.
        /// </summary>
        internal float[] DashPatern
        {
            get
            {
                return GetDashPatern();
            }
        }

        /// <summary>
        /// Gets or Sets the width of the border.
        /// </summary>
        /// <value>The width of the border.</value>
        public int BorderWidth
        {
            get
            {
                return GetBorderWidth();
            }
            set
            {
                m_widget.WidgetBorder.Width = value;
                SetBorderWidth(value);
            }
        }

        /// <summary>
        /// Gets the string format.
        /// </summary>
        /// <value>The string format.</value>
        internal PdfStringFormat StringFormat
        {
            get
            {
                return GetStringFormat();
            }
        }

        /// <summary>
        /// Gets the back brush.
        /// </summary>
        /// <value>The back brush.</value>
        internal PdfBrush BackBrush
        {
            get
            {
                return GetBackBrush();
            }
            set
            {
                SetBackBrush(value);
            }
        }

        /// <summary>
        /// Gets the color of the fore.
        /// </summary>
        /// <value>The color of the fore.</value>
        internal PdfBrush ForeBrush
        {
            get
            {
                return GetForeBrush();
            }
        }

        /// <summary>
        /// Gets the shadow brush.
        /// </summary>
        /// <value>The shadow brush.</value>
        internal PdfBrush ShadowBrush
        {
            get
            {
                return GetShadowBrush();
            }
        }

        /// <summary>
        /// Gets the font.
        /// </summary>
        /// <value>The font.</value>
        public PdfFont Font
        {
            get
            {
                if (m_font != null)
                    return m_font;

                bool isCorrectFont;
                PdfFont font = PdfDocument.DefaultFont;

                PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);

                if ((widget != null) && (widget.ContainsKey(DictionaryProperties.DA)||(Dictionary.ContainsKey(DictionaryProperties.DA))))
                {
                    PdfString defaulAppearance = CrossTable.GetObject(widget[DictionaryProperties.DA]) as PdfString;
                    if (defaulAppearance == null)
                        defaulAppearance = CrossTable.GetObject(Dictionary[DictionaryProperties.DA]) as PdfString;
                    string[] fontParameters = defaulAppearance.Value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);                    
                    font = GetFont(defaulAppearance.Value, out isCorrectFont);

                    if (!isCorrectFont)
                    {
                        string correctFontName = "/Helv";
                        widget.SetProperty(DictionaryProperties.DA, new PdfString(defaulAppearance.Value.Replace(fontParameters[0], correctFontName)));
                    }
                }

                return font;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Font");
                }

                if (m_font != value)
                {
                    m_font = value;
                    (this as PdfField).Form.SetAppearanceDictionary = true;
                    //DefineDefaultAppearance();
                }
            }
        }

        /// <summary>
        /// Gets the default index.
        /// </summary>
        public new int DefaultIndex
        {
            get
            {
                return base.DefaultIndex;
            }
            set
            {
                if (value < 0)
                    throw new IndexOutOfRangeException("index");

                base.DefaultIndex = value;
            }
        }

        ///<summary>
        /// Gets the kids.
        /// </summary>
        internal PdfArray Kids
        {
            get
            {
                return GetKids();
            }
        }

        /// <summary>
        /// Gets a value indicating the visibility of the field.
        /// </summary>
        public bool Visible
        {
            get
            {
                return GetVisible();
            }
        }

        internal int RotationAngle
        {
            get
            {
                return GetRotationAngle();
            }
        }

       
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLoadedStyledField"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        internal PdfLoadedStyledField(PdfDictionary dictionary, PdfCrossTable crossTable)
            : base(dictionary, crossTable)
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the graphics properties.
        /// </summary>
        /// <param name="graphicsProperties">The graphics properties.</param>
        /// <param name="item">The item.</param>
        protected void GetGraphicsProperties(out GraphicsProperties graphicsProperties, PdfLoadedFieldItem item)
        {
            bool isItem = item != null;

            if (isItem)
            {
                graphicsProperties = new GraphicsProperties(item);
            }
            else
            {
                graphicsProperties = new GraphicsProperties(this);
            }
        }

        /// <summary>
        /// Creates the border style.
        /// </summary>
        /// <param name="bs">The bs.</param>
        /// <returns>The border style.</returns>
        private PdfBorderStyle CreateBorderStyle(PdfDictionary bs)
        {
            PdfBorderStyle style = PdfBorderStyle.Solid;

            if (bs.ContainsKey(DictionaryProperties.S))
            {
                PdfName name = CrossTable.GetObject(bs[DictionaryProperties.S]) as PdfName;

                if (name != null)
                {
                    switch (name.Value.ToLower())
                    {
                        case "d":
                            style = PdfBorderStyle.Dashed;
                            break;

                        case "b":
                            style = PdfBorderStyle.Beveled;
                            break;

                        case "i":
                            style = PdfBorderStyle.Inset;
                            break;

                        case "u":
                            style = PdfBorderStyle.Underline;
                            break;
                    }
                }
            }

            return style;
        }
        /// <summary>
        /// Sets the border style.
        /// </summary>
        /// <param name="bs">The bs.</param>
        /// <returns>The border style.</returns>
        private void SetBorderStyle(PdfBorderStyle borderStyle)
        {
            string style = "";

            PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);
            if (widget.ContainsKey(DictionaryProperties.BS))
            {
                PdfDictionary bs = CrossTable.GetObject(widget[DictionaryProperties.BS]) as PdfDictionary;

                switch (borderStyle)
                {
                    case PdfBorderStyle.Dashed:
                        style = "D";
                        break;
                    case PdfBorderStyle.Beveled:
                        style = "B";
                        break;
                    case PdfBorderStyle.Inset:
                        style = "I";
                        break;
                    case PdfBorderStyle.Underline:
                        style = "U";
                        break;
                    case PdfBorderStyle.Solid:
                        style = "S";
                        break;
                }

                (widget[DictionaryProperties.BS] as PdfDictionary)[DictionaryProperties.S] = new PdfName(style);
                Widget.WidgetBorder.Style = borderStyle;
            }
        }

        /// <summary>
        /// Gets border pen.
        /// </summary>
        /// <returns>The border style.</returns>
        internal PdfPen SetBorderColor(PdfColor borderColor)
        {
            PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);

            PdfPen pen = null;// new PdfPen( Color.Empty );

            if (widget != null)
            {
                if (widget.ContainsKey(DictionaryProperties.MK))
                {
                    PdfDictionary mk = CrossTable.GetObject(widget[DictionaryProperties.MK]) as PdfDictionary;
                    PdfArray array = borderColor.ToArray();
                    mk[DictionaryProperties.BC] = array;
                    PdfColor color = CreateColor(array);
                    pen = new PdfPen(color);
                }
                else
                {
                    PdfDictionary mk = new PdfDictionary();
                    PdfArray array = borderColor.ToArray();
                    mk[DictionaryProperties.BC] = array;
                    PdfColor color = CreateColor(array);
                    pen = new PdfPen(color);
                    widget[DictionaryProperties.MK] = mk;
                }
            }

            PdfBorderStyle style = BorderStyle;
            int borderWidth = BorderWidth;

            if (pen != null)
            {
                pen.Width = borderWidth;

                if (style == PdfBorderStyle.Dashed)
                {
                    float[] dashPatern = DashPatern;
                    pen.DashStyle = PdfDashStyle.Custom;

                    if (dashPatern != null)
                    {
                        pen.DashPattern = dashPatern;
                    }
                    else
                    {
                        pen.DashPattern = new float[] { 3 / borderWidth };
                    }
                }
            }

            return pen;
        }

        /// <summary>
        /// Gets the bounds.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <returns>The bounds.</returns>
        private RectangleF GetBounds(PdfDictionary dictionary, PdfCrossTable crossTable)
        {
            PdfArray array = null;

            if (dictionary.ContainsKey(DictionaryProperties.Kids))
            {
                PdfDictionary widget = GetWidgetAnnotation(dictionary, crossTable);

                if (widget.ContainsKey(DictionaryProperties.Rect))
                {
                    array = crossTable.GetObject(widget[DictionaryProperties.Rect]) as PdfArray;
                }
            }
            else
            {
                if (dictionary.ContainsKey(DictionaryProperties.Rect))
                {
                    array = crossTable.GetObject(dictionary[DictionaryProperties.Rect]) as PdfArray;
                }
            }

            RectangleF bounds;
            if (array != null)
            {
                bounds = array.ToRectangle();

                if ((array[1] as PdfNumber).FloatValue < 0)
                {
                    bounds.Y = (array[1] as PdfNumber).FloatValue;

                    if ((array[1] as PdfNumber).FloatValue > (array[3] as PdfNumber).FloatValue)
                        bounds.Y -= bounds.Height;
                }
            }
            else
            {
                bounds = new RectangleF();
            }

            return bounds;
        }

        /// <summary>
        /// Gets the high light string.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <returns>The highligt mode.</returns>
        private string GetHighLightString(PdfHighlightMode mode)
        {
            string str = null;

            switch (mode)
            {
                case PdfHighlightMode.NoHighlighting:
                    str = "N";
                    break;

                case PdfHighlightMode.Invert:
                    str = "I";
                    break;

                case PdfHighlightMode.Outline:
                    str = "O";
                    break;

                case PdfHighlightMode.Push:
                    str = "P";
                    break;
            }

            return str;
        }

        /// <summary>
        /// Creates the color.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns></returns>
        private PdfColor CreateColor(PdfArray array)
        {
            int dim = array.Count;

            PdfColor color = PdfColor.Empty;

            float[] colors = new float[array.Count];

            for (int i = 0, size = array.Count; i < size; ++i)
            {
                PdfNumber number = CrossTable.GetObject(array[i]) as PdfNumber;

                colors[i] = number.FloatValue;
            }

            switch (dim)
            {
                case 1:
                    color = new PdfColor(colors[0]);
                    break;

                case 3:
                    color = new PdfColor(colors[0], colors[1], colors[2]);
                    break;

                case 4:
                    color = new PdfColor(colors[0], colors[1], colors[2], colors[3]);
                    break;
            }

            return color;
        }

        /// <summary>
        /// Gets the font colour.
        /// </summary>
        /// <param name="defaultAppearance">The default appearance.</param>
        /// <returns>The colour of the text value.</returns>
        internal PdfColor GetForeColour(string defaultAppearance)
        {
            PdfColor colour = new PdfColor(0, 0, 0);

            if (defaultAppearance == null || defaultAppearance == string.Empty)
            {
                colour = new PdfColor(0, 0, 0);
            }
            else
            {
                byte[] buf = Encoding.UTF8.GetBytes(defaultAppearance);
                MemoryStream stream = new MemoryStream(buf);
                PdfReader reader = new PdfReader(stream);

                reader.Position = 0;
                bool symbol = false;
                Stack<string> operands = new Stack<string>();
                string token = reader.GetNextToken();
                if (token == "/")
                {
                    symbol = true;
                }

                while (token != null && token != string.Empty)
                {
                    if (symbol == true)
                    {
                        token = reader.GetNextToken();
                    }

                    symbol = true;

                    if (token == Operators.SetGrayColorForNonstroking)
                    {
                        string prevToken = operands.Pop();

                        float gray = ParseFloatColour(prevToken);
                        colour = new PdfColor(gray);
                    }
                    else if (token == Operators.SetRGBColorForNonStroking)
                    {
                        byte red, green, blue;
                        string operand;
                        operand = operands.Pop() as string;
                        blue = (byte)(ParseFloatColour(operand) * 255.0f);

                        operand = operands.Pop() as string;
                        green = (byte)(ParseFloatColour(operand) * 255.0f);

                        operand = operands.Pop() as string;
                        red = (byte)(ParseFloatColour(operand) * 255.0f);

                        colour = new PdfColor(red, green, blue);
                    }
                    else if (token == Operators.SetCMYKColorForNonstroking)
                    {
                        float cyan, magenta, yellow, black;
                        string operand;

                        operand = operands.Pop() as string;
                        black = ParseFloatColour(operand);

                        operand = operands.Pop() as string;
                        yellow = ParseFloatColour(operand);

                        operand = operands.Pop() as string;
                        magenta = ParseFloatColour(operand);

                        operand = operands.Pop() as string;
                        cyan = ParseFloatColour(operand);

                        colour = new PdfColor(cyan, magenta, yellow, black);
                    }
                    else
                    {
                        operands.Push(token);
                    }
                }
            }
            return colour;
        }

        /// <summary>
        /// Parses the float.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        private float ParseFloatColour(string text)
        {
            float num = (float)double.Parse(text, NumberStyles.Float,
                CultureInfo.InvariantCulture);

            return num;
        }

        /// <summary>
        /// Gets the font.
        /// </summary>
        /// <param name="fontString">The font string.</param>
        /// <returns></returns>
        private PdfFont GetFont(string fontString, out bool isCorrectFont)
        {
            float height = 0.0f;
            isCorrectFont = true;
            string name = FontName(fontString, out height);

            PdfFont font = new PdfStandardFont((PdfStandardFont)PdfDocument.DefaultFont, height);

#if !SILVERLIGHT && !NETFX_CORE && !WP
            PdfDictionary fontDictionary = CrossTable.GetObject(this.Form.Resources[DictionaryProperties.Font]) as PdfDictionary;
            
            if (fontDictionary != null && name != null && fontDictionary.ContainsKey(name))
            {
                fontDictionary = CrossTable.GetObject(fontDictionary[name]) as PdfDictionary;

                if (fontDictionary != null && fontDictionary.ContainsKey(DictionaryProperties.Subtype))
                {
                    PdfName fontSubtype = CrossTable.GetObject(fontDictionary[DictionaryProperties.Subtype]) as PdfName;

                    if (fontSubtype.Value == DictionaryProperties.Type1)
                    {
                        PdfName baseFont = CrossTable.GetObject(fontDictionary[DictionaryProperties.BaseFont]) as PdfName;
                        PdfFontStyle fontStyle = GetFontStyle(baseFont.Value);

                        string standardName;
                        PdfFontFamily fontFamily = GetFontFamily(baseFont.Value, out standardName);

                        if (standardName == null)
                            font = new PdfStandardFont(fontFamily, height, fontStyle);
                        else
                        {
                            if (fontStyle != PdfFontStyle.Regular)
                                font = new PdfStandardFont((PdfStandardFont)PdfDocument.DefaultFont, height, fontStyle);

                            if (baseFont.Value.Contains("MyriadPro"))
                            {
                                Font usedFont = new Font(baseFont.Value, height, (FontStyle)fontStyle);
                                font = new PdfTrueTypeFont(usedFont);
                            }
                            else
                                font.Metrics = ((CreateFont(fontDictionary, height, baseFont) != null) ? CreateFont(fontDictionary, height, baseFont) : font.Metrics);
                        }
                    }
                    else if (fontSubtype.Value == DictionaryProperties.TrueType)
                    {
                        PdfName baseFont = CrossTable.GetObject(fontDictionary[DictionaryProperties.BaseFont]) as PdfName;
                        string fontName = GetFontName(baseFont.Value);
                        PdfFontStyle fontStyle = GetFontStyle(baseFont.Value);

                        if (fontName != null || fontName != string.Empty)
                        {
                            bool isStandardFont = false;

                            foreach (string standardFontName in Enum.GetNames(typeof(PdfFontFamily)))
                            {
                                if (fontName.Contains(standardFontName))
                                {
                                    font = new PdfStandardFont((PdfFontFamily)Enum.Parse(typeof(PdfFontFamily), standardFontName, true), height, fontStyle);
                                    isStandardFont = true;
                                }
                            }
                            if (!isStandardFont)
                            {
                                try
                                {
                                    string sInput = fontName;
                                    string[] sReturn = new string[1];
                                    sReturn[0] = "";
                                    const string CUPPER = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
                                    int iArrayCount = 0;
                                    for (int iIndex = 0; iIndex < sInput.Length; iIndex++)
                                    {
                                        string sChar = sInput.Substring(iIndex, 1); // get a char
                                        if ((CUPPER.Contains(sChar)) && (iIndex > 0))
                                        {
                                            if (!CUPPER.Contains(sInput[iIndex - 1].ToString()))
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
                                    fontName = fontName.Trim();
                                    Font fieldFont = new Font(fontName, height, (FontStyle)fontStyle);
                                    if (Dictionary.ContainsKey(DictionaryProperties.V))
                                    {
                                        PdfString valueString = Dictionary.GetString(DictionaryProperties.V) as PdfString;

                                        PdfName type = Dictionary[DictionaryProperties.FT] as PdfName;
                                        if(type.Value.Equals(DictionaryProperties.Ch))
                                        {
                                           if(Dictionary.ContainsKey(DictionaryProperties.Opt))
                                           {
                                               PdfArray option = Dictionary[DictionaryProperties.Opt] as PdfArray;

                                               if (option[0] is PdfArray)
                                               {
                                                   foreach (PdfArray value in option)
                                                   {
                                                       if ((value[0] as PdfString).Value.Equals(valueString.Value))
                                                       {
                                                           valueString = value[1] as PdfString;
                                                           break;
                                                       }
                                                   }
                                               }
                                           }
                                        }
                                                     
                                        if (valueString != null)
                                        {
                                            string text = valueString.Value;
                                            if (PdfString.IsUnicode(text))
                                                font = new PdfTrueTypeFont(fieldFont, true);
                                            else
                                                font = new PdfTrueTypeFont(fieldFont);
                                        }
                                    }
                                    else
                                        font = new PdfTrueTypeFont(fieldFont);
                                }
                                catch (ArgumentException)
                                {
                                    font = new PdfStandardFont((PdfStandardFont)PdfDocument.DefaultFont, height, fontStyle);
                                }
                            }
                        }
                        else if (fontStyle != PdfFontStyle.Regular)
                        {
                            font = new PdfStandardFont((PdfStandardFont)PdfDocument.DefaultFont, height, fontStyle);
                        }
                        PdfName tempName = fontDictionary[DictionaryProperties.Name] as PdfName;
                        if(tempName!=null)
                            if (font.Name != tempName.Value)
                                font.Metrics = ((CreateFont(fontDictionary, height, baseFont) != null) ? CreateFont(fontDictionary, height, baseFont) : font.Metrics);                        

                    }
                    else if (fontSubtype.Value == DictionaryProperties.Type0)
                    {
                        if(fontDictionary.ContainsKey(DictionaryProperties.ToUnicode))
                        {

                            PdfArray descendantFontsArray = fontDictionary[DictionaryProperties.DescendantFonts] as PdfArray;
                            PdfDictionary descendantFontsDic = (descendantFontsArray[0] as PdfReferenceHolder).Object as PdfDictionary;
                            PdfReferenceHolder fontDescriptor = descendantFontsDic[DictionaryProperties.FontDescriptor] as PdfReferenceHolder;
                            PdfDictionary fontDescriptorDic = fontDescriptor.Object as PdfDictionary;

                            PdfName font_name = fontDescriptorDic[DictionaryProperties.FontName] as PdfName;
                        
                            PdfFontMetrics fontMetrics = CreateFont(descendantFontsDic, height, font_name);

                            string fontNameStr = font_name.Value.Substring(font_name.Value.IndexOf('+')+1);
                            
                            FontStyle style = FontStyle.Regular;
                            // Removing PSMT string if its present in the fontname
                            if (fontNameStr.Contains("PSMT"))                            
                                fontNameStr = fontNameStr.Remove(fontNameStr.IndexOf("PSMT"));                                                            
                            else if (fontNameStr.Contains("Bold"))
                                style = FontStyle.Bold;                                
                            else if (fontNameStr.Contains("Italic"))
                                style =  FontStyle.Italic;
                            if (fontNameStr.Contains("BoldItalic"))
                                style = FontStyle.Italic | FontStyle.Bold;
                            if (fontNameStr.Contains("PS"))
                                fontNameStr = fontNameStr.Remove(fontNameStr.IndexOf("PS"));   
                            if(fontNameStr.Contains("-"))
                                fontNameStr = fontNameStr.Remove(fontNameStr.IndexOf("-")); 

                            FontFamily[] families = FontFamily.Families;
                            foreach (FontFamily family in families)
                            {
                                string systemFontName = family.Name.Replace(" ", string.Empty);
                                if (fontNameStr.Contains(systemFontName))
                                {
                                    fontNameStr = family.Name;
                                    break;
                                }
                            }

                            font = new PdfTrueTypeFont(new Font(fontNameStr, height, style), true);
                            WidthTable widthTable = font.Metrics.WidthTable;
                            font.Metrics = fontMetrics;
                            font.Metrics.WidthTable = widthTable;
                        }

                    }

                }
            }
            else
            {
                /*if( name == "TiRo" )
                {
                    font = new PdfStandardFont( PdfFontFamily.TimesRoman, height );
                }*/
                PdfFont usedFont = GetFontByName(name, height);

                if (usedFont != null)
                    font = usedFont;
                else
                    isCorrectFont = false;
            }

            if (height == 0)
            {
                PdfStandardFont stdf = font as PdfStandardFont;
                height = GetFontHeight(stdf.FontFamily);
                font = new PdfStandardFont(stdf, height);
            }
#endif
            return font;
        }

        /// <summary>
        /// Reading Font Name from Dictionary.
        /// </summary>
        /// <param name="fontString"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        internal string FontName(string fontString, out float height)
        {
            if (fontString.Contains("#2C"))
            {
                StringBuilder fontName = new StringBuilder(fontString);
                fontName.Replace("#2C", ",");
                fontString = fontName.ToString();
            }
            byte[] buf = Encoding.UTF8.GetBytes(fontString);
            MemoryStream stream = new MemoryStream(buf);
            PdfReader reader = new PdfReader(stream);

            reader.Position = 0;


            string prevToken = reader.GetNextToken();
            string token = reader.GetNextToken();
            string name = null;
            height = 0.0f;
            while (token != null && token != string.Empty)
            {
                name = prevToken;
                prevToken = token;
                token = reader.GetNextToken();

                if (token == Operators.SetFont)
                {
                    height = (float)double.Parse(prevToken, NumberStyles.Float,
                        CultureInfo.InvariantCulture);

                    break;
                }
            }
            return name;
        }

        /// <summary>
        /// Create metrics for embed font
        /// </summary>
        /// <param name="fontDictionary"></param>
        /// <returns></returns>
        private PdfFontMetrics CreateFont(PdfDictionary fontDictionary, float height, PdfName baseFont)
        {
            PdfFontMetrics fontMetrics = new PdfFontMetrics();

            if (fontDictionary.ContainsKey(DictionaryProperties.FontDescriptor))
            {
                PdfDictionary dic = ((fontDictionary[DictionaryProperties.FontDescriptor] as PdfReferenceHolder).Object) as PdfDictionary;
                fontMetrics.Ascent = (dic[DictionaryProperties.Ascent] as PdfNumber).IntValue;
                fontMetrics.Descent = (dic[DictionaryProperties.Descent] as PdfNumber).IntValue;
                fontMetrics.Size = height;
                fontMetrics.Height = fontMetrics.Ascent - fontMetrics.Descent;
                fontMetrics.PostScriptName = baseFont.Value;
                PdfArray array = null;

                if (fontDictionary.ContainsKey(DictionaryProperties.Widths))
                {
                    if (fontDictionary[DictionaryProperties.Widths] is PdfReferenceHolder)
                    {

                        PdfReferenceHolder tableReference = new PdfReferenceHolder(fontDictionary[DictionaryProperties.Widths]);
                        PdfReferenceHolder tableArray = tableReference.Object as PdfReferenceHolder;
                        array = (tableArray.Object) as PdfArray;
                        int[] widthTable = new int[array.Count];


                        for (int i = 0; i < array.Count; i++)
                        {
                            widthTable[i] = (array[i] as PdfNumber).IntValue;
                        }

                        fontMetrics.WidthTable = new StandardWidthTable(widthTable);
                    }
                    else
                    {
                        array = (fontDictionary[DictionaryProperties.Widths] as PdfArray);
                        int[] widthTable = new int[array.Count];

                        for (int i = 0; i < array.Count; i++)
                        {
                            widthTable[i] = (array[i] as PdfNumber).IntValue;
                        }

                        fontMetrics.WidthTable = new StandardWidthTable(widthTable);
                    }
                }
             
                
                fontMetrics.Name = baseFont.Value;
            }
            return fontMetrics;
        }

        /// <summary>
        /// Gets the font by its name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="height">The height of the resulting font.</param>
        /// <returns>The proper font object.</returns>
        private PdfFont GetFontByName(string name, float height)
        {
            PdfFont font = null;

            switch (name)
            {
                case "CoBO": //"Courier-BoldOblique"
                    font = new PdfStandardFont(PdfFontFamily.Courier, height, PdfFontStyle.Bold | PdfFontStyle.Italic);
                    break;

                case "CoBo"://"Courier-Bold"
                    font = new PdfStandardFont(PdfFontFamily.Courier, height, PdfFontStyle.Bold);
                    break;

                case "CoOb"://"Courier-Oblique"
                    font = new PdfStandardFont(PdfFontFamily.Courier, height, PdfFontStyle.Italic);
                    break;

                case "Cour"://"Courier"
                    font = new PdfStandardFont(PdfFontFamily.Courier, height, PdfFontStyle.Regular);
                    break;

                case "HeBO"://"Helvetica-BoldOblique"
                    font = new PdfStandardFont(PdfFontFamily.Helvetica, height, PdfFontStyle.Bold | PdfFontStyle.Italic);
                    break;

                case "HeBo"://"Helvetica-Bold"
                    font = new PdfStandardFont(PdfFontFamily.Helvetica, height, PdfFontStyle.Bold);
                    break;

                case "HeOb"://"Helvetica-Oblique"
                    font = new PdfStandardFont(PdfFontFamily.Helvetica, height, PdfFontStyle.Italic);
                    break;

                case "Helv"://"Helvetica"
                    font = new PdfStandardFont(PdfFontFamily.Helvetica, height, PdfFontStyle.Regular);
                    break;

                case "Symb":// "Symbol"
                    font = new PdfStandardFont(PdfFontFamily.Symbol, height);
                    break;

                case "TiBI":// "Times-BoldItalic"
                    font = new PdfStandardFont(PdfFontFamily.TimesRoman, height, PdfFontStyle.Bold | PdfFontStyle.Italic);
                    break;

                case "TiBo":// "Times-Bold"
                    font = new PdfStandardFont(PdfFontFamily.TimesRoman, height, PdfFontStyle.Bold);
                    break;

                case "TiIt":// "Times-Italic"
                    font = new PdfStandardFont(PdfFontFamily.TimesRoman, height, PdfFontStyle.Italic);
                    break;

                case "TiRo":// "Times-Roman"
                    font = new PdfStandardFont(PdfFontFamily.TimesRoman, height, PdfFontStyle.Regular);
                    break;

                case "ZaDb":// "ZapfDingbats"
                    font = new PdfStandardFont(PdfFontFamily.ZapfDingbats, height);
                    break;

                //case "HySm":// "HYSMyeongJo-Medium", "UniKS-UCS2-H"
                //case "HyGo":// "HYGoThic-Medium", "UniKS-UCS2-H"
                //case "KaGo":// "HeiseiKakuGo-W5", "UniKS-UCS2-H"
                //case "KaMi":// "HeiseiMin-W3", "UniJIS-UCS2-H"
                //case "MHei":// "MHei-Medium", "UniCNS-UCS2-H"
                //case "MSun":// "MSung-Light", "UniCNS-UCS2-H"
                //case "STSo":// "STSong-Light", "UniGB-UCS2-H"
            }

            return font;
        }

        /// <summary>
        /// Gets the font style.
        /// </summary>
        /// <param name="fontFamilyString">The font family string.</param>
        /// <returns>The style of pdf font.</returns>
        private PdfFontStyle GetFontStyle(string fontFamilyString)
        {
            int position = fontFamilyString.IndexOf("-");
            if (position < 0)
                position = fontFamilyString.IndexOf(",");
            PdfFontStyle style = PdfFontStyle.Regular;

            if (position >= 0)
            {
                string standardName = fontFamilyString.Substring(position + 1, fontFamilyString.Length - position - 1);

                switch (standardName)
                {
                    case "Italic":
                    case "Oblique":
                    case "ItalicMT":
                    case "It":
                        style = PdfFontStyle.Italic;
                        break;

                    case "Bold":
                    case "BoldMT":
                        style = PdfFontStyle.Bold;
                        break;

                    case "BoldItalic":
                    case "BoldOblique":
                    case "BoldItalicMT":
                        style = PdfFontStyle.Italic | PdfFontStyle.Bold;
                        break;
                }
            }
            return style;
        }

        /// <summary>
        /// Gets the font name
        /// </summary>
        /// <param name="fontFamilyString"></param>
        /// <returns>font name</returns>
        internal string GetFontName(string fontFamilyString)
        {

#if !SILVERLIGHT && !NETFX_CORE && !WP
            if(fontFamilyString.Contains("-") || fontFamilyString.Contains("PSMT") || fontFamilyString.Contains("MT")||fontFamilyString.Contains(","))
            {
                string postScriptName = fontFamilyString;
                if (fontFamilyString.Contains("-"))
                    postScriptName = fontFamilyString.Replace("-", " ");
                if (fontFamilyString.Contains(","))
                    return (fontFamilyString.Split(','))[0];
                FontFamily[] families = FontFamily.Families;
                foreach (FontFamily family in families)
                {
                    string systemFontName = family.Name;
                    if (postScriptName == systemFontName)
                    {                       
                        return systemFontName;
                    }
                }
                if (fontFamilyString.Contains("PSMT"))
                    return (fontFamilyString.Replace("PSMT", ""));
                if (fontFamilyString.Contains("MT") && (!fontFamilyString.Contains("-")))
                    return (fontFamilyString.Replace("MT", ""));
            }
#endif
            return (fontFamilyString.Split('-'))[0];
        }
        /// <summary>
        /// Gets the font family.
        /// </summary>
        /// <param name="fontFamilyString">The font family string.</param>
        /// <returns>The font family.</returns>
        private PdfFontFamily GetFontFamily(string fontFamilyString, out string standardName)
        {
            int position = fontFamilyString.IndexOf("-");

            PdfFontFamily fontFamily = PdfFontFamily.Helvetica;

            standardName = fontFamilyString;

            if (position >= 0)
            {
                standardName = fontFamilyString.Substring(0, position);
            }

            if (standardName == "Times")
            {
                fontFamily = PdfFontFamily.TimesRoman;
                standardName = null;
            }
            else
            {
#if !SILVERLIGHT && !NETFX_CORE && !WP
                foreach (string fontName in Enum.GetNames(typeof(PdfFontFamily)))
                {
                    if (fontName.Contains(standardName))
                    {
                        fontFamily = (PdfFontFamily)Enum.Parse(typeof(PdfFontFamily), standardName, true);
                        standardName = null;
                        break;
                    }
                }
#else
                try
                {
                    fontFamily = (PdfFontFamily)Enum.Parse(typeof(PdfFontFamily), standardName, true);
                }
                catch (ArgumentException)
                {
                    standardName = "Helvetica";
                    return PdfFontFamily.Helvetica;
                }
#endif
            }
            return fontFamily;
        }


        /// <summary>
        /// Gets the border style.
        /// </summary>
        /// <returns>Border style of the field.</returns>
        internal PdfColor GetBackColor()
        {
            PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);

            PdfColor c = new PdfColor(0, 20, 200);
            if (widget.ContainsKey(DictionaryProperties.MK))
            {
                PdfDictionary bs = CrossTable.GetObject(widget[DictionaryProperties.MK]) as PdfDictionary;

                if (bs.ContainsKey(DictionaryProperties.BG))
                {
                    PdfArray array = bs[DictionaryProperties.BG] as PdfArray;

                    c = CreateColor(array);
                }
                // style = CreateBorderStyle(bs);
            }
            return c;
        }
        /// <summary>
        /// Gets the border style.
        /// </summary>
        /// <returns>Border style of the field.</returns>
        private PdfBorderStyle GetBorderStyle()
        {
            PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);

            PdfBorderStyle style = PdfBorderStyle.Solid;

            if (widget.ContainsKey(DictionaryProperties.BS))
            {
                PdfDictionary bs = CrossTable.GetObject(widget[DictionaryProperties.BS]) as PdfDictionary;

                style = CreateBorderStyle(bs);
            }

            return style;
        }

        /// <summary>
        /// Gets DashPatern.
        /// </summary>
        /// <returns>The DashPatern.</returns>
        private float[] GetDashPatern()
        {
            float[] array = null;

            if (BorderStyle == PdfBorderStyle.Dashed)
            {
                PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);

                if (widget.ContainsKey(DictionaryProperties.D))
                {
                    PdfArray dashes = CrossTable.GetObject(widget[DictionaryProperties.D]) as PdfArray;

                    if (array.Length == 2)
                    {
                        array = new float[2];

                        PdfNumber number = dashes[0] as PdfNumber;
                        array[0] = number.IntValue;
                        number = dashes[1] as PdfNumber;
                        array[1] = number.IntValue;
                    }
                    else
                    {
                        array = new float[1];

                        PdfNumber number = dashes[0] as PdfNumber;
                        array[0] = number.IntValue;
                    }
                }

            }

            return array;
        }

        /// <summary>
        /// Gets border width.
        /// </summary>
        /// <returns>The boder width.</returns>
        private int GetBorderWidth()
        {
            PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);

            int width = 0;

            if (widget.ContainsKey(DictionaryProperties.BS))
            {
                width = 1;
                PdfDictionary bs = CrossTable.GetObject(widget[DictionaryProperties.BS]) as PdfDictionary;

                PdfNumber number = CrossTable.GetObject(bs[DictionaryProperties.W]) as PdfNumber;

                if (number != null)
                {
                    width = number.IntValue;
                }
            }

            return width;
        }

        /// <summary>
        /// Sets border width.
        /// </summary>
        private void SetBorderWidth(int width)
        {
            PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);

            if (widget.ContainsKey(DictionaryProperties.BS))
            {
                (widget[DictionaryProperties.BS] as PdfDictionary)[DictionaryProperties.W]
                    = new PdfNumber(width);

                CreateBorderPen();
            }
        }


        /// <summary>
        /// Gets string format.
        /// </summary>
        /// <returns>The string format.</returns>
        private PdfStringFormat GetStringFormat()
        {
            PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);

            PdfStringFormat stringFormat = new PdfStringFormat();
            stringFormat.LineAlignment = PdfVerticalAlignment.Middle;

            stringFormat.LineAlignment
                                    = ((Flags & FieldFlags.Multiline) > 0) ? PdfVerticalAlignment.Top : PdfVerticalAlignment.Middle;

            PdfNumber number = null;
            if ((widget != null) && (widget.ContainsKey(DictionaryProperties.Q)))
                number = CrossTable.GetObject(widget[DictionaryProperties.Q]) as PdfNumber;
            else if(Dictionary.ContainsKey(DictionaryProperties.Q))
                number = CrossTable.GetObject(Dictionary[DictionaryProperties.Q]) as PdfNumber;

            if (number != null && number.IsInteger)
                stringFormat.Alignment = (PdfTextAlignment)number.IntValue;

            return stringFormat;
        }

        /// <summary>
        /// Gets back brush.
        /// </summary>
        /// <returns>The back brush.</returns>
        private PdfBrush GetBackBrush()
        {
            PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);

            PdfBrush brush = null; // new PdfSolidBrush( Color.Empty );

            if (widget != null)
            {
                if (widget.ContainsKey(DictionaryProperties.MK))
                {
                    PdfDictionary mk = CrossTable.GetObject(widget[DictionaryProperties.MK]) as PdfDictionary;

                    if (mk.ContainsKey(DictionaryProperties.BG))
                    {
                        PdfArray array = CrossTable.GetObject(mk[DictionaryProperties.BG]) as PdfArray;

                        PdfColor color = CreateColor(array);

                        brush = new PdfSolidBrush(color);
                    }
                }
            }

            return brush;
        }

        /// <summary>
        /// Sets the back color of the Field.
        /// </summary>
        /// <param name="brush">The brush.</param>
        private void SetBackBrush(PdfBrush brush)
        {
            PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);

            if (widget != null && brush is PdfSolidBrush)
            {
                PdfDictionary mk;

                if (widget.ContainsKey(DictionaryProperties.MK))
                {
                    mk = CrossTable.GetObject(widget[DictionaryProperties.MK]) as PdfDictionary;
                }
                else
                {
                    mk = new PdfDictionary();
                    widget[DictionaryProperties.MK] = mk;
                }

                PdfArray array = (brush as PdfSolidBrush).Color.ToArray();
                mk[DictionaryProperties.BG] = array;
            }
        }

        /// <summary>
        /// Gets fore brush.
        /// </summary>
        /// <returns>The fore brush.</returns>
        private PdfBrush GetForeBrush()
        {
            PdfBrush brush = PdfBrushes.Black;

            PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);

            if ((widget != null) && (widget.ContainsKey(DictionaryProperties.DA)))
            {
                PdfString defaultAppearance = CrossTable.GetObject(widget[DictionaryProperties.DA]) as PdfString;

                PdfColor foreColor = GetForeColour(defaultAppearance.Value);

                brush = new PdfSolidBrush(foreColor);
            }

            return brush;
        }

        /// <summary>
        /// Gets shadow brush.
        /// </summary>
        /// <returns>The shadow brush.</returns>
        private PdfBrush GetShadowBrush()
        {
            PdfBrush brush = PdfBrushes.White;

            PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);

            if ((widget != null) && (widget.ContainsKey(DictionaryProperties.DA)))
            {
                PdfString defaultAppearance = CrossTable.GetObject(widget[DictionaryProperties.DA]) as PdfString;

                PdfColor color = new PdfColor(255, 255, 255);
                PdfSolidBrush backBrush = BackBrush as PdfSolidBrush;

                if (backBrush != null)
                {
                    color = backBrush.Color;
                }

                color.R = (byte)(color.R - ShadowShift >= 0 ? color.R - ShadowShift : 0);
                color.G = (byte)(color.G - ShadowShift >= 0 ? color.G - ShadowShift : 0);
                color.B = (byte)(color.B - ShadowShift >= 0 ? color.B - ShadowShift : 0);

                brush = new PdfSolidBrush(color);
            }

            return brush;
        }

        /// <summary>
        /// Draws this instance if it is flatten.
        /// </summary>
        internal override void Draw()
        { }

        /// <summary>
        /// Creates a copy of loaded field item.
        /// </summary>
        internal override PdfLoadedFieldItem CreateLoadedItem(PdfDictionary dictionary)
        {
            return null;
        }

        /// <summary>
        /// Begins the save.
        /// </summary>
        internal override void BeginSave()
        {
            base.BeginSave();

            PdfSolidBrush sBrush = BackBrush as PdfSolidBrush;

            if ((sBrush != null) && (sBrush.Color.IsEmpty))
            {
                PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);
                PdfDictionary MK = new PdfDictionary();
                PdfArray arr = new PdfArray(new float[] { 1.0f, 1.0f, 1.0f });
                MK.SetProperty(DictionaryProperties.BG, arr);
                widget.SetProperty(DictionaryProperties.MK, MK);
            }
        }

        /// <summary>
        /// Gets the height of the font.
        /// </summary>
        /// <returns>The calculated size of font.</returns>
        internal virtual float GetFontHeight(PdfFontFamily family)
        {
            return 0;
        }

        /// <summary>
        /// Gets border pen.
        /// </summary>
        /// <returns>The border style.</returns>
        internal PdfPen GetBorderPen()
        {
            PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);

            PdfPen pen = null;// new PdfPen( Color.Empty );

            if (widget == null)
            {
                widget = Dictionary;
            }

            if (widget != null)
            {
                if (widget.ContainsKey(DictionaryProperties.MK))
                {
                    PdfDictionary mk = CrossTable.GetObject(widget[DictionaryProperties.MK]) as PdfDictionary;

                    if (mk.ContainsKey(DictionaryProperties.BC))
                    {
                        PdfArray array = CrossTable.GetObject(mk[DictionaryProperties.BC]) as PdfArray;

                        PdfColor color = CreateColor(array);

                        pen = new PdfPen(color);
                    }
                }
            }

            PdfBorderStyle style = BorderStyle;
            int borderWidth = BorderWidth;

            if (pen != null)
            {
                pen.Width = borderWidth;

                if (style == PdfBorderStyle.Dashed)
                {
                    float[] dashPatern = DashPatern;
                    pen.DashStyle = PdfDashStyle.Custom;

                    if (dashPatern != null)
                    {
                        pen.DashPattern = dashPatern;
                    }
                    else
                    {
                        pen.DashPattern = new float[] { 3 / borderWidth };
                    }
                }
            }

            return pen;
        }

        /// <summary>
        /// Gets the field's annotation.
        /// </summary>
        /// <returns>The array of fields annotations.</returns>
        private PdfArray GetKids()
        {
            PdfArray kids = null;

            if (Dictionary.ContainsKey(DictionaryProperties.Kids))
            {
                kids = CrossTable.GetObject(Dictionary[DictionaryProperties.Kids]) as PdfArray;
            }

            return kids;
        }

        /// <summary>
        /// Gets the visibility of the field.
        /// </summary>
        /// <returns></returns>
        private bool GetVisible()
        {
            PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);

            if (widget == null)
            {
                widget = Dictionary;
            }

            if (widget != null)
            {
                if (widget.ContainsKey(DictionaryProperties.F))
                {
                    PdfNumber num = CrossTable.GetObject(widget[DictionaryProperties.F]) as PdfNumber;
                    //Check if hidden.
                    if (num.IntValue == 2)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Creates the border pen.
        /// </summary>
        private void CreateBorderPen()
        {
            float width = m_widget.WidgetBorder.Width;

            m_borderPen = new PdfPen(m_widget.WidgetAppearance.BorderColor, width);

            if (Widget.WidgetBorder.Style == PdfBorderStyle.Dashed)
            {
                m_borderPen.DashStyle = PdfDashStyle.Custom;
                m_borderPen.DashPattern = new float[] { 3 / width };
            }
        }

        /// <summary>
        /// Defines the default appearance.
        /// </summary>
        protected override void DefineDefaultAppearance()
        {
            WidgetAnnotation annot = m_widget;
            if (Form != null && m_font != null)
            {
                PdfDictionary widget = GetWidgetAnnotation(Dictionary, CrossTable);
                PdfName name = Form.Resources.GetName(m_font);
                Form.Resources.Add(m_font, name);
                Form.NeedAppearances = true;
                PdfDefaultAppearance defaultAppearance = new PdfDefaultAppearance();
                defaultAppearance.FontName = name.Value;
                defaultAppearance.FontSize = m_font.Size;
                defaultAppearance.ForeColor = ForeColor;
                widget[DictionaryProperties.DA] = new PdfString(defaultAppearance.ToString());

            }
        }

        private int GetRotationAngle()
        {
            int rotationAngle = 0;

            if(base.Dictionary != null
                && base.Dictionary.ContainsKey(DictionaryProperties.MK))
            {
                PdfDictionary appearance = base.Dictionary[DictionaryProperties.MK] as PdfDictionary;
                if (appearance != null)
                rotationAngle = (appearance.ContainsKey(DictionaryProperties.R)) ?
                    (appearance[DictionaryProperties.R] as PdfNumber).IntValue : 0;
            }

            return rotationAngle;
        }

        internal PdfField Clone(PdfDictionary dictionary, PdfPage page)
        {
            PdfCrossTable newTable = page.Section.ParentDocument.CrossTable;
            PdfLoadedStyledField field = new PdfLoadedStyledField(dictionary, newTable);
            field.Page = page;
            field.Widget.Dictionary = Widget.Dictionary.Clone(newTable) as PdfDictionary;

            return field;
        }
        #endregion

        #region Internals
        /// <summary>
        /// Structure that holds graphics properties.
        /// </summary>
        protected struct GraphicsProperties
        {
            #region Fields
            public RectangleF Rect;
            public PdfPen Pen;
            public PdfBorderStyle Style;
            public int BorderWidth;
            public PdfBrush BackBrush;
            public PdfBrush ForeBrush;
            public PdfBrush ShadowBrush;
            public PdfFont Font;
            public PdfStringFormat StringFormat;
            public int RotationAngle;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="GraphicsProperties"/> struct.
            /// </summary>
            /// <param name="field">The field.</param>
            public GraphicsProperties(PdfLoadedStyledField field)
            {
                if (field == null)
                    throw new ArgumentNullException("field");

                Rect = field.Bounds;
                Pen = field.BorderPen;
                Style = field.BorderStyle;
                BorderWidth = field.BorderWidth;
                BackBrush = field.BackBrush;
                ForeBrush = field.ForeBrush;
                ShadowBrush = field.ShadowBrush;
                Font = field.Font;
                StringFormat = field.StringFormat;
                RotationAngle = field.RotationAngle;    
                
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="GraphicsProperties"/> struct.
            /// </summary>
            /// <param name="item">The item.</param>
            public GraphicsProperties(PdfLoadedFieldItem item)
            {
                if (item == null)
                    throw new ArgumentNullException("item");

                Rect = item.Bounds;
                Pen = item.BorderPen;
                Style = item.BorderStyle;
                BorderWidth = item.BorderWidth;
                BackBrush = item.BackBrush;
                ForeBrush = item.ForeBrush;
                ShadowBrush = item.ShadowBrush;
                Font = item.Font;
                StringFormat = item.StringFormat;
                RotationAngle = 0;
            }
            #endregion
        }
        #endregion
    }
}
