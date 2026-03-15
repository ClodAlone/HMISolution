#if !SILVERLIGHT
#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

#if !XAML && !GDI
using System.Drawing;
#if!NETFX_CORE  && !WP
using System.Drawing.Imaging;
#endif
using Syncfusion.Pdf.Graphics;
#elif XAML && !BARCODE_WINRT
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
#elif GDI
using System.Drawing;
using System.Drawing.Imaging;
#if WINFORMS
using System.Windows.Forms;
#endif
#else
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI;
#endif

# if WPF || BARCODE_SILVERLIGHT || BARCODE_WINRT
namespace Syncfusion.UI.Xaml.Controls.Barcode
#elif WINDOWS_PHONE
namespace Syncfusion.WP.Controls.Barcode
#elif ASPNET
namespace Syncfusion.Web.UI.WebControls.Barcode
#elif WINFORMS
namespace Syncfusion.Windows.Forms.Barcode
#elif MVC
namespace Syncfusion.Mvc.Barcode
#else
/// <summary>
/// The Syncfusion.Pdf.Barcode namespace contains classes for creating barcodes.
/// </summary>
namespace Syncfusion.Pdf.Barcode
#endif
{
    /// <summary>
    /// Represents the Base class for all the Single dimensional barcodes
    /// </summary>
# if !XAML && !GDI
    public abstract class PdfUnidimensionalBarcode : PdfBarcode
#else
    public abstract class UnidimensionalBarcode : BarcodeBase
#endif
    {
        #region Fields

        /// <summary>
        /// Indicates whether the Checkdigit is already added to the barcode text or not.
        /// Used with the barcodes which involves multiple checksum calculations.
        /// </summary>
        protected bool isCheckDigitAdded;

        /// <summary>
        /// To check whether Barcode is Continuous  Barcode or discrete 
        /// </summary>
        protected bool continuous = false;

        /// <summary>
        /// Automatically adds the check digit to the barcode when true. 
        /// </summary>
        protected bool check = false;

        /// <summary>
        /// Indicates barcode pattern.  <see cref="PdfBarcodePattern"/>
        /// </summary>
        private Dictionary<char, BarcodeSymbolTable> m_barcodeSymbols = new Dictionary<char, BarcodeSymbolTable>();

#if !XAML && !GDI
        /// <summary>
        /// Indicates the barcode text display location. <seealso cref="TextDisplayLocation"/>
        /// </summary>
        private TextLocation m_textDisplayLocation;

        /// <summary>
        /// Indicates the font used to draw the text.
        /// </summary>
        private PdfFont m_font;
#elif GDI
        /// <summary>
        /// Indicates the font used to draw the text.
        /// </summary>
        private Font m_font;
# endif

#if GDI || BARCODE_WINRT || XAML
        /// <summary>
        /// Indicates the barcode text display location. <seealso cref="BarcodeTextDisplayLocation"/>
        /// </summary>
        private BarcodeTextLocation m_textDisplayLocation;
#endif
        /// <summary>
        /// Indicates the start symbol.
        /// </summary>
        private char m_startSymbol;

        /// <summary>
        /// Indicates the stop symbol.
        /// </summary>
        private char m_stopSymbol;

        /// <summary>
        /// Indicates the validation expression which is used to validate the input text.
        /// </summary>
        private string m_validatorExpression = string.Empty;

        /// <summary>
        /// Indicates the validation expression.
        /// </summary>
        private Regex m_codeValidator;

        /// <summary>
        /// Indicates whether to show check digit or not.
        /// </summary>
        private bool m_showCheckDigit;

        /// <summary>
        /// Indicates whether to enable / disable the check digits.
        /// </summary>
        private bool m_enableCheckDigit;

        /// <summary>
        /// Indicates the intercharcter gap between bars.
        /// </summary>
        private float m_intercharacterGap;

        /// <summary>
        /// Indicates the gap between barcode and the text.
        /// </summary>
        private float m_barcodeToTextGapHeight;

        /// <summary>
        /// Indicates the text alignment.
        /// </summary>
# if !XAML && !GDI
        private PdfBarcodeTextAlignment m_textAlignment;
#else
        private BarcodeTextAlignment m_textAlignment;
#endif

        /// <summary>
        /// Indicates whether to encode start and stop symbols or not.
        /// </summary>
        private bool m_encodeStartStopSymbols;

        #endregion

        #region Constructors
# if !XAML && !GDI
        /// <summary>
        /// Initializes the new instance of <see cref="PdfUnidimensionalBarcode"/>
        /// </summary>
        public PdfUnidimensionalBarcode()
#else
        /// <summary>
        /// Initializes the new instance of <see cref="UnidimensionalBarcode"/>
        /// </summary>
        public UnidimensionalBarcode()
#endif
            : base()
        {
            m_startSymbol = '\0';
            m_stopSymbol = '\0';
            m_intercharacterGap = 1f;
            m_barcodeToTextGapHeight = 5f;
#if !XAML && !GDI
            m_font = new PdfStandardFont(PdfFontFamily.Helvetica, 8f);
            m_textAlignment = PdfBarcodeTextAlignment.Center;
             m_textDisplayLocation = TextLocation.Bottom;
#else
            m_textAlignment = BarcodeTextAlignment.Center;
            m_textDisplayLocation = BarcodeTextLocation.Bottom;
# endif
#if GDI
            m_font = new Font("Arial", 8f);
#endif
            m_encodeStartStopSymbols = true;
            m_enableCheckDigit = false;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the Text font.
        /// </summary>
# if !XAML && !GDI
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Creates a new PdfCode93Barcode
        /// PdfCode93Barcode code93 = new PdfCode93Barcode("CODE93");
        /// //Create font and font style.
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold) ;
        /// //Set the barcode font.
        /// code93.Font = font;
        /// //Draw a barcode in the new Page.
        /// code93.Draw(page, new PointF(25, 500));
        /// //Save the  document to disk.
        /// document.Save("Code93.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Creates a new PdfCode93Barcode.
        /// Dim code93 As PdfCode93Barcode = New PdfCode93Barcode("CODE93")
        /// //Create font and font style.
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
        /// 'Set the barcode font.
        /// code93.Font = font
        /// 'Draw a barcode in the new Page.
        /// code93.Draw(page, new PointF(25, 500))
        /// 'Save the  document to disk.
        /// document.Save("code93.pdf")
        /// </code>
        /// </example> 
        public PdfFont Font
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
#elif GDI
        public Font Font
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
#endif

        /// <summary>
        /// Gets or sets the text display location. <see cref="TextDisplayLocation"/>
        /// </summary>
# if !XAML && !GDI
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfCode93Barcode code93 = new PdfCode93Barcode("CODE93");
        /// //Set the barcode text display location.
        /// code93.TextDisplayLocation = TextLocation.Bottom;
        /// //Draw a barcode in the new Page.
        /// code93.Draw(page, new PointF(25, 500));
        /// //Save the  document to disk.
        /// document.Save("Code93.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Creates a new PdfCode93Barcode.
        /// Dim code93 As PdfCode93Barcode = New PdfCode93Barcode("CODE93")
        /// 'Set the barcode text display location.
        /// code93.TextDisplayLocation =  TextLocation.Bottom
        /// 'Draw a barcode in the new Page.
        /// code93.Draw(page, new PointF(25, 500))
        /// 'Save the  document to disk.
        /// document.Save("code93.pdf")
        /// </code>
        /// </example> 
        public TextLocation TextDisplayLocation
#else
        public BarcodeTextLocation TextDisplayLocation
#endif
        {
            get
            {
                return m_textDisplayLocation;
            }

            set
            {
                m_textDisplayLocation = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to Show Check digit in the generated barcode or not.<seealso cref="EnableCheckDigit"/>
        /// </summary>
# if !XAML
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Creates a new PdfCode93Barcode
        /// PdfCode93Barcode code93 = new PdfCode93Barcode("CODE93");
        /// //Set the barcode show check digit.
        /// code93.ShowCheckDigit = true;
        /// //Draw a barcode in the new Page.
        /// code93.Draw(page, new PointF(25, 500));
        /// //Save the  document to disk.
        /// document.Save("Code93.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Creates a new PdfCode93Barcode.
        /// Dim code93 As PdfCode93Barcode = New PdfCode93Barcode("CODE93")
        /// 'Set the barcode show check digit.
        /// code93.ShowCheckDigit = true
        /// 'Draw a barcode in the new Page.
        /// code93.Draw(page, new PointF(25, 500))
        /// 'Save the  document to disk.
        /// document.Save("code93.pdf")
        /// </code>
        /// </example> 
#endif
        /// <remarks>The Default value is false.</remarks>
        public bool ShowCheckDigit
        {
            get
            {
                return m_showCheckDigit;
            }

            set
            {
                m_showCheckDigit = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable to check digit calculation in the generated barcode or not.
        /// </summary>
# if !XAML
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Creates a new PdfCode93Barcode
        /// PdfCode93Barcode code93 = new PdfCode93Barcode("CODE93");
        /// //Set the barcode enable check digit.
        /// code93.EnableCheckDigit = true;
        /// //Draw a barcode in the new Page.
        /// code93.Draw(page, new PointF(25, 500));
        /// //Save the  document to disk.
        /// document.Save("Code93.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Creates a new PdfCode93Barcode.
        /// Dim code93 As PdfCode93Barcode = New PdfCode93Barcode("CODE93")
        /// 'Set the barcode enable check digit.
        /// code93.EnableCheckDigit =true
        /// 'Draw a barcode in the new Page.
        /// code93.Draw(page, new PointF(25, 500))
        /// 'Save the  document to disk.
        /// document.Save("code93.pdf")
        /// </code>
        /// </example> 
#endif
        /// <remarks>The Default value is True.</remarks>
        public bool EnableCheckDigit
        {
            get
            {
                return m_enableCheckDigit;
            }

            set
            {
                m_enableCheckDigit = value;
            }
        }

        /// <summary>
        /// Gets or sets the gap between the barcode and the displayed text.
        /// </summary>
# if !XAML
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Creates a new PdfCode93Barcode
        /// PdfCode93Barcode code93 = new PdfCode93Barcode("CODE93");
        /// //Set the barcode text gap height.
        /// code93.ToTextGapHeight = 20f;
        /// //Draw a barcode in the new Page.
        /// code93.Draw(page, new PointF(25, 500));
        /// //Save the  document to disk.
        /// document.Save("Code93.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Creates a new PdfCode93Barcode.
        /// Dim code93 As PdfCode93Barcode = New PdfCode93Barcode("CODE93")
        /// 'Set the barcode text gap height.
        /// code93.ToTextGapHeight = 20f
        /// 'Draw a barcode in the new Page.
        /// code93.Draw(page, new PointF(25, 500))
        /// 'Save the  document to disk.
        /// document.Save("code93.pdf")
        /// </code>
        /// </example> 
#endif
        /// <seealso cref="TextLocation"/>
        public float BarcodeToTextGapHeight
        {
            get
            {
                return m_barcodeToTextGapHeight;
            }

            set
            {
                if (value < 0)
                {
#if!XAML && !NETFX_CORE && !GDI && !WP
                    throw new PdfBarcodeException("Text to barcode gap cannot be negative.");
#else
                    throw new BarcodeException("Text to barcode gap cannot be negative.");
#endif
                }

                m_barcodeToTextGapHeight = value;
            }
        }

        /// <summary>
        /// Gets or sets the alignment of the text displayed on the barcode.
        /// </summary>
# if !XAML && !GDI
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Creates a new PdfCode93Barcode
        /// PdfCode93Barcode code93 = new PdfCode93Barcode("CODE93");
        /// //Set the barcode text alignment.
        /// code93.TextAlignment = PdfBarcodeTextAlignment.Center;
        /// //Draw a barcode in the new Page.
        /// code93.Draw(page, new PointF(25, 500));
        /// //Save the  document to disk.
        /// document.Save("Code93.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Creates a new PdfCode93Barcode.
        /// Dim code93 As PdfCode93Barcode = New PdfCode93Barcode("CODE93")
        /// 'Set the barcode text alignment.
        /// code93.TextAlignment = PdfBarcodeTextAlignment.Center;
        /// 'Draw a barcode in the new Page.
        /// code93.Draw(page, new PointF(25, 500))
        /// 'Save the  document to disk.
        /// document.Save("code93.pdf")
        /// </code>
        /// </example> 
#endif
        /// <remarks>Default value is Center.</remarks>
        /// <seealso cref="TextLocation"/>
# if !XAML && !GDI
        public PdfBarcodeTextAlignment TextAlignment
#else
        public BarcodeTextAlignment TextAlignment
#endif
        {
            get
            {
                return m_textAlignment;
            }

            set
            {
                m_textAlignment = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [encode start stop symbols].
        /// </summary>
# if !XAML && !GDI
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Creates a new PdfCode93Barcode
        /// PdfCode93Barcode code93 = new PdfCode93Barcode("CODE93");
        /// //Set the barcode encode start stop symbols.
        /// code93.EncodeStartStopSymbols = true;
        /// //Draw a barcode in the new Page.
        /// code93.Draw(page, new PointF(25, 500));
        /// //Save the  document to disk.
        /// document.Save("Code93.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Creates a new PdfCode93Barcode.
        /// Dim code93 As PdfCode93Barcode = New PdfCode93Barcode("CODE93")
        /// 'Set the barcode encode start stop symbols.
        /// code93.EncodeStartStopSymbols = true
        /// 'Draw a barcode in the new Page.
        /// code93.Draw(page, new PointF(25, 500))
        /// 'Save the  document to disk.
        /// document.Save("code93.pdf")
        /// </code>
        /// </example> 
#endif
        /// <value>
        /// <c>true</c> if [encode start stop symbols]; otherwise, <c>false</c>.
        /// </value>
        public bool EncodeStartStopSymbols
        {
            get
            {
                return m_encodeStartStopSymbols;
            }

            set
            {
                m_encodeStartStopSymbols = value;
            }
        }

        /// <summary>
        /// Gets or sets the barcode symbols.
        /// </summary>
        internal Dictionary<char, BarcodeSymbolTable> BarcodeSymbols
        {
            get
            {
                return m_barcodeSymbols;
            }

            set
            {
                m_barcodeSymbols = value;
            }
        }

        /// <summary>
        /// Gets or sets the start symbol for the current barcode specification.
        /// </summary>
        internal char StartSymbol
        {
            get
            {
                return m_startSymbol;
            }

            set
            {
                m_startSymbol = value;
            }
        }

        /// <summary>
        /// Gets or sets the stop symbol for the current barcode specification.
        /// </summary>
        internal char StopSymbol
        {
            get
            {
                return m_stopSymbol;
            }

            set
            {
                m_stopSymbol = value;
            }
        }

        /// <summary>
        /// Gets or sets the validation expression to validate the given text.
        /// </summary>
        internal string ValidatorExpression
        {
            get
            {
                return m_validatorExpression;
            }

            set
            {
                m_validatorExpression = value;
            }
        }

        /// <summary>
        /// Gets or sets the IntercharacterGap. 
        /// </summary>
        internal float IntercharacterGap
        {
            get
            {
                return m_intercharacterGap;
            }

            set
            {
                m_intercharacterGap = value;
            }
        }

        #endregion

        #region Methods
#if!XAML && !GDI
        /// <summary>
        /// Draws the barcode on the <see cref="PdfPage"/> at the specified location.
        /// </summary>
        /// <param name="page">The pdf page.</param>
        /// <param name="location">The barcode location.</param>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create font and font style.
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold) ;
        /// //Creates a new PdfCode11Barcode.
        /// PdfCode11Barcode code11 = new PdfCode11Barcode();
        /// //Set the font to code11.
        /// code11.Font = font;
        /// //Set the barcode text.
        /// code11.Text = "012345678";
        /// //Draw a barcode in the new Page.
        /// code11.Draw(page, new PointF(25, 500));
        /// //Save the  document to disk.
        /// document.Save("Code11.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create font and font style.
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
        /// 'Creates a new PdfCode11Barcode.
        /// Dim code11 As PdfCode11Barcode = New PdfCode11Barcode()
        /// 'Set the font to code11.
        /// code11.Font = font
        /// 'Set the barcode text.
        /// code11.Text = "012345678"
        /// 'Draw a barcode in the new Page.
        /// code11.Draw(page, new PointF(25, 500))
        /// 'Save the  document to disk.
        /// document.Save("Code11.pdf")
        /// </code>
        /// </example>
        public virtual void Draw(PdfPageBase page, PointF location)
        {
            string actualText = Text;

            if (!Validate(Text))
            {
#if !NETFX_CORE && !WP
                throw new PdfBarcodeException("Barcode Text contains characters that are not accepted by this barcode specification.");
#else
                throw new BarcodeException("Barcode Text contains characters that are not accepted by this barcode specification.");
#endif
            }

            string code = GetTextToEncode();
            float left = 0;
            float right = 0;
            float top = 0;

            if (code == null || code.Length == 0)
            {
#if !NETFX_CORE && !WP
                throw new PdfBarcodeException("Barcode Text cannot be null or empty.");
#else
                throw new BarcodeException("Barcode Text cannot be null or empty.");
#endif

            }

            if (m_encodeStartStopSymbols)
            {
                if (m_startSymbol != '\0' && m_stopSymbol != '\0')
                {
                    code = m_startSymbol.ToString() + code + m_stopSymbol.ToString();
                }
            }

            SizeF textSize = Font.MeasureString(base.Text);
            float w = base.QuietZone.Left + base.QuietZone.Right;
            foreach (char c in code)
            {
                w += GetCharWidth(c) + m_intercharacterGap;
            }

            if (continuous == false)
            {
                w -= m_intercharacterGap * code.Length;
            }
            else
            {
                if (base.ExtendedText.Length > 0)
                {
                    w -= m_intercharacterGap * (base.ExtendedText.Length - base.Text.Length);
                }
                else
                {
                    w -= m_intercharacterGap;
                }
            }

            float h = base.QuietZone.Top + base.QuietZone.Bottom + base.BarHeight;

            Color backcolor = Color.FromArgb(base.BackColor.A, base.BackColor.R, base.BackColor.G, base.BackColor.B);
            PdfBrush brush = new PdfSolidBrush(backcolor);
            RectangleF rect = new RectangleF();

            if (m_textDisplayLocation == TextLocation.Top)
            {
                PointF loc = location;
                location.Y += m_barcodeToTextGapHeight + 10f;
                h += m_barcodeToTextGapHeight + 10f;
                rect = new RectangleF(loc, new SizeF(w, h));
            }

            if (m_textDisplayLocation == TextLocation.Bottom)
            {
                h += m_barcodeToTextGapHeight + 10f;
                rect = new RectangleF(location, new SizeF(w, h));
            }

            if (m_textDisplayLocation == TextLocation.None)
            {
                rect = new RectangleF(location, new SizeF(w, h));
            }

            page.Graphics.DrawRectangle(brush, rect);

            Bounds = rect;

            left = base.QuietZone.Left + rect.Left;
            top = 0f + rect.Top;

            if (m_textDisplayLocation == TextLocation.Top)
            {
                top = location.Y;
            }
            else
            {
                top = location.Y + base.QuietZone.Top;
            }

            foreach (char c in code)
            {
                foreach (KeyValuePair<char, BarcodeSymbolTable> entry in BarcodeSymbols)
                {
                    BarcodeSymbolTable pattern = (BarcodeSymbolTable)entry.Value;
                    if (pattern.Symbol == c)
                    {
                        byte[] bars = pattern.Bars;
                        for (int i = 0; i < bars.Length; i++)
                        {
                            float barWidth = bars[i] * base.NarrowBarWidth;
                            RectangleF barRect = new RectangleF(left, top, barWidth, base.BarHeight);
                            if (i % 2 == 0)
                            {
                                left += PaintRectangle(page, barRect);
                            }
                            else
                            {
                                left += barWidth;
                            }
                        }

                        if (bars.Length % 2 != 0)
                        {
                            left += IntercharacterGap;
                        }
                    }
                }
            }

            if (m_textDisplayLocation != TextLocation.None)
            {
                base.Text = base.Text.Trim(m_startSymbol);
                base.Text = base.Text.Trim(m_stopSymbol);

                PdfStringFormat format = new PdfStringFormat((PdfTextAlignment)m_textAlignment);
                Color textColor = Color.FromArgb(base.TextColor.A, base.TextColor.R, base.TextColor.G, base.TextColor.B);
                PdfBrush textBrush = new PdfSolidBrush(textColor);

                if (m_textAlignment == PdfBarcodeTextAlignment.Left)
                {
                    left = rect.Left + base.QuietZone.Left;
                    right = rect.Width;
                }
                else if (m_textAlignment == PdfBarcodeTextAlignment.Right)
                {
                    left = rect.Left;
                    right = rect.Width - base.QuietZone.Right;
                }
                else
                {
                    left = rect.Left + base.QuietZone.Left;
                    right = rect.Width - (base.QuietZone.Right + base.QuietZone.Left);
                }

                if (m_textDisplayLocation == TextLocation.Top)
                {
                    RectangleF layoutRect = new RectangleF(new PointF(left, location.Y + base.QuietZone.Top - m_barcodeToTextGapHeight - textSize.Height), new SizeF(right, textSize.Height));
                    page.Graphics.DrawString(base.Text, Font, textBrush, layoutRect, format);
                }
                else
                {
                    RectangleF layoutRect = new RectangleF();
                    layoutRect = new RectangleF(new PointF(left, location.Y + base.QuietZone.Top + m_barcodeToTextGapHeight + base.BarHeight), new SizeF(right, textSize.Height));
                    page.Graphics.DrawString(base.Text, Font, textBrush, layoutRect, format);
                }
            }

            Text = actualText;
        }

        /// <summary>
        /// Exports the barcode as image.
        /// <returns>The barcode image.</returns>
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Creates a new PdfCode11Barcode.
        /// PdfCode11Barcode code11 = new PdfCode11Barcode();
        /// //Set the barcode text.
        /// code11.Text = "012345678";
        /// //Get the image for Code32 Barcode.
        /// Image image= code32.ToImage();
        /// //Save the image into Disk
        /// image.Save("Code32.png", ImageFormat.Png);
        /// </code>
        /// <code lang="VB">
        /// 'Creates a new PdfCode11Barcode.
        /// Dim code11 As PdfCode11Barcode = New PdfCode11Barcode()
        /// 'Set the barcode text.
        /// code11.Text = "012345678"
        /// 'Get the image for Code32 Barcode.
        /// Image image= code32.ToImage()
        /// 'Save the image into Disk
        /// image.Save("Code32.png", ImageFormat.Png)
        /// </code>
        /// </example>
#if !NETFX_CORE && !WP
        public Image ToImage()
        {
            PointF location = PointF.Empty;
            string actualText = Text;
            isCheckDigitAdded = false;
            PdfBarcodeQuietZones actualQuiteZones = QuietZone;

            if (!Validate(Text))
            {
                throw new PdfBarcodeException("Barcode Text contains characters that are not accepted by this barcode specification.");
            }

            if (NarrowBarWidth < 1)
            {
                NarrowBarWidth = 1;
            }

            string code = GetTextToEncode();

            float left = 0;
            float right = 0;
            float top = 0;

            if (code == null || code.Length == 0)
            {
                throw new PdfBarcodeException("Barcode Text cannot be null or empty.");
            }

            if (EncodeStartStopSymbols)
            {
                if (StartSymbol != '\0' && StopSymbol != '\0')
                {
                    code = StartSymbol.ToString() + code + StopSymbol.ToString();
                }
            }

            System.Drawing.Font gFont = new Font(Font.Name.ToString(), Font.Size);

            SizeF textSize = Font.MeasureString(Text);
            textSize.Height = gFont.Height;

            float width = QuietZone.Left + QuietZone.Right;

            foreach (char c in code)
            {
                width += GetCharWidth(c) + m_intercharacterGap;
            }

            if (continuous == false)
            {
                width -= m_intercharacterGap * code.Length;
            }
            else
            {
                width -= (ExtendedText.Length > 0) ?
                    m_intercharacterGap * (base.ExtendedText.Length - base.Text.Length) :
                    m_intercharacterGap;
            }

            float height = QuietZone.Top + QuietZone.Bottom + BarHeight + textSize.Height + BarcodeToTextGapHeight;

            if (TextDisplayLocation == TextLocation.None)
            {
                height -= (textSize.Height + BarcodeToTextGapHeight);
            }

            Color backcolor = Color.FromArgb(BackColor.ToArgb());
            PdfBrush brush = new PdfSolidBrush(backcolor);

            Bitmap bmp = new Bitmap((int)width, (int)height, PixelFormat.Format32bppRgb);

            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);
            g.FillRectangle(Brushes.White, new Rectangle(0, 0, bmp.Width, bmp.Height));

            RectangleF rect = new RectangleF();

            if (TextDisplayLocation == TextLocation.Top)
            {
                PointF loc = location;
                location.Y += BarcodeToTextGapHeight + 10f;
                height += BarcodeToTextGapHeight + 10f;
                rect = new RectangleF(loc, new SizeF(width, height));
            }
            else if (TextDisplayLocation == TextLocation.Bottom)
            {
                height += BarcodeToTextGapHeight + 10f;
                rect = new RectangleF(location, new SizeF(width, height));
            }
            else
            {
                rect = new RectangleF(location, new SizeF(width, height));
            }

            Color backColor = Color.FromArgb(BackColor.ToArgb());
            Brush backgroundBrush = new SolidBrush(backcolor);

            g.FillRectangle(backgroundBrush, rect);

            Bounds = rect;

            left = base.QuietZone.Left + rect.Left;
            top = 0f + rect.Top;

            if (TextDisplayLocation == TextLocation.Top)
            {
                top = location.Y + base.QuietZone.Top;
            }
            else
            {
                top = location.Y + base.QuietZone.Top;
            }

            foreach (char c in code)
            {
                foreach (KeyValuePair<char, BarcodeSymbolTable> entry in BarcodeSymbols)
                {
                    BarcodeSymbolTable pattern = (BarcodeSymbolTable)entry.Value;
                    if (pattern.Symbol == c)
                    {
                        byte[] bars = pattern.Bars;
                        for (int i = 0; i < bars.Length; i++)
                        {
                            float barWidth = bars[i] * base.NarrowBarWidth;
                            RectangleF barRect = new RectangleF(left, top, barWidth, base.BarHeight);
                            if (i % 2 == 0)
                            {
                                left += PaintToImage(ref g, barRect);
                            }
                            else
                            {
                                left += barWidth;
                            }
                        }

                        if (bars.Length % 2 != 0)
                        {
                            left += IntercharacterGap;
                        }
                    }
                }
            }

            if (TextDisplayLocation != TextLocation.None)
            {
                base.Text = base.Text.Trim(m_startSymbol);
                base.Text = base.Text.Trim(m_stopSymbol);

                StringFormat gStringFormat = new StringFormat();
                gStringFormat.LineAlignment = StringAlignment.Center;

                if (TextAlignment == PdfBarcodeTextAlignment.Left)
                {
                    gStringFormat.Alignment = StringAlignment.Near;
                }
                else if (TextAlignment == PdfBarcodeTextAlignment.Right)
                {
                    gStringFormat.Alignment = StringAlignment.Far;
                }
                else
                {
                    gStringFormat.Alignment = StringAlignment.Center;
                }

                Color textColor = Color.FromArgb(TextColor.ToArgb());
                SolidBrush textBrush = new SolidBrush(textColor);

                if (TextAlignment == PdfBarcodeTextAlignment.Left)
                {
                    left = rect.Left + QuietZone.Left;
                    right = rect.Width;
                }
                else if (TextAlignment == PdfBarcodeTextAlignment.Right)
                {
                    left = rect.Left;
                    right = rect.Width - QuietZone.Right;
                }
                else
                {
                    left = rect.Left + QuietZone.Left;
                    right = rect.Width - (QuietZone.Right + QuietZone.Left);
                }

                if (m_textDisplayLocation == TextLocation.Top)
                {
                    RectangleF layoutRect = new RectangleF(new PointF(left, location.Y + QuietZone.Top - m_barcodeToTextGapHeight - textSize.Height), new SizeF(right, textSize.Height));
                    layoutRect.Y = (layoutRect.Y < 0) ? 0 : layoutRect.Y;
                    g.DrawString(Text, gFont, textBrush, layoutRect, gStringFormat);
                }
                else
                {
                    RectangleF layoutRect = new RectangleF(new PointF(left, location.Y + QuietZone.Bottom + BarcodeToTextGapHeight + BarHeight - 2f), new SizeF(right, textSize.Height));
                    g.DrawString(Text, gFont, textBrush, layoutRect, gStringFormat);
                }
            }

            QuietZone = actualQuiteZones;
            Text = actualText;
            return bmp;
        }
#endif
#elif GDI
        public virtual Image Draw(int angle)
        {
            PointF location = PointF.Empty;
            string actualText = Text;
            isCheckDigitAdded = false;
            BarcodeQuietZones actualQuiteZones = QuietZone;

            if (!Validate(Text))
            {
                throw new BarcodeException("Barcode Text contains characters that are not accepted by this barcode specification.");
            }

            if (NarrowBarWidth < 1)
            {
                NarrowBarWidth = 1;
            }

            string code = GetTextToEncode();

            float left = 0;
            float right = 0;
            float top = 0;

            if (code == null || code.Length == 0)
            {
                throw new BarcodeException("Barcode Text cannot be null or empty.");
            }

            if (EncodeStartStopSymbols)
            {
                if (StartSymbol != '\0' && StopSymbol != '\0')
                {
                    code = StartSymbol.ToString() + code + StopSymbol.ToString();
                }
            }

            System.Drawing.Font gFont = new Font(Font.Name.ToString(), Font.Size);

            SizeF textSize;

            using (Bitmap img = new Bitmap(1, 1))
            {
                using (Graphics grap = Graphics.FromImage(img))
                    textSize = grap.MeasureString(Text, gFont);
            }
            textSize.Height = gFont.Height;

            float width = QuietZone.Left + QuietZone.Right;

            foreach (char c in code)
            {
                width += GetCharWidth(c) + m_intercharacterGap;
            }

            if (continuous == false)
            {
                width -= m_intercharacterGap * code.Length;
            }
            else
            {
                width -= (ExtendedText.Length > 0) ?
                    m_intercharacterGap * (base.ExtendedText.Length - base.Text.Length) :
                    m_intercharacterGap;
            }

            float height = QuietZone.Top + QuietZone.Bottom + BarHeight + textSize.Height + BarcodeToTextGapHeight;

            if (TextDisplayLocation == BarcodeTextLocation.None)
            {
                height -= (textSize.Height + BarcodeToTextGapHeight);
            }

            Bitmap bmp = null;
            if (angle == 90 || angle == 270)
                bmp = new Bitmap((int)height, (int)width, PixelFormat.Format32bppRgb);
            else
                bmp = new Bitmap((int)width, (int)height, PixelFormat.Format32bppRgb);

            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);
            System.Drawing.Drawing2D.GraphicsState state = g.Save();
            if (angle == 90 || angle == 270)
            {
                g.TranslateTransform(bmp.Width / 2, bmp.Height / 2);
                g.RotateTransform(angle);
                g.TranslateTransform(-(float)bmp.Height / 2, -(float)bmp.Width / 2);
            }
            
            g.FillRectangle(Brushes.White, new Rectangle(0, 0, bmp.Width, bmp.Height));

            RectangleF rect = new RectangleF();

            if (TextDisplayLocation == BarcodeTextLocation.Top)
            {
                PointF loc = location;
                location.Y += BarcodeToTextGapHeight + 10f;
                height += BarcodeToTextGapHeight + 10f;
                rect = new RectangleF(loc, new SizeF(width, height));
            }
            else if (TextDisplayLocation == BarcodeTextLocation.Bottom)
            {
                height += BarcodeToTextGapHeight + 10f;
                rect = new RectangleF(location, new SizeF(width, height));
            }
            else
            {
                rect = new RectangleF(location, new SizeF(width, height));
            }

            Color backcolor = Color.FromArgb(BackColor.ToArgb());
            Brush backgroundBrush = new SolidBrush(backcolor);

            g.FillRectangle(backgroundBrush, rect);

            Bounds = rect;

            left = base.QuietZone.Left + rect.Left;
            top = 0f + rect.Top;

            if (TextDisplayLocation == BarcodeTextLocation.Top)
            {
                top = location.Y + base.QuietZone.Top;
            }
            else
            {
                top = location.Y + base.QuietZone.Top;
            }

            foreach (char c in code)
            {
                foreach (KeyValuePair<char, BarcodeSymbolTable> entry in BarcodeSymbols)
                {
                    BarcodeSymbolTable pattern = (BarcodeSymbolTable)entry.Value;
                    if (pattern.Symbol == c)
                    {
                        byte[] bars = pattern.Bars;
                        for (int i = 0; i < bars.Length; i++)
                        {
                            float barWidth = bars[i] * base.NarrowBarWidth;
                            RectangleF barRect = new RectangleF(left, top, barWidth, base.BarHeight);
                            if (i % 2 == 0)
                            {
                                left += PaintToImage(ref g, barRect);
                            }
                            else
                            {
                                left += barWidth;
                            }
                        }

                        if (bars.Length % 2 != 0)
                        {
                            left += IntercharacterGap;
                        }
                    }
                }
            }

            if (TextDisplayLocation != BarcodeTextLocation.None)
            {
                base.Text = base.Text.Trim(m_startSymbol);
                base.Text = base.Text.Trim(m_stopSymbol);

                StringFormat gStringFormat = new StringFormat();
                gStringFormat.LineAlignment = StringAlignment.Center;

                if (TextAlignment == BarcodeTextAlignment.Left)
                {
                    gStringFormat.Alignment = StringAlignment.Near;
                }
                else if (TextAlignment == BarcodeTextAlignment.Right)
                {
                    gStringFormat.Alignment = StringAlignment.Far;
                }
                else
                {
                    gStringFormat.Alignment = StringAlignment.Center;
                }

                Color textColor = Color.FromArgb(TextColor.ToArgb());
                SolidBrush textBrush = new SolidBrush(textColor);

                if (TextAlignment == BarcodeTextAlignment.Left)
                {
                    left = rect.Left + QuietZone.Left;
                    right = rect.Width;
                }
                else if (TextAlignment == BarcodeTextAlignment.Right)
                {
                    left = rect.Left;
                    right = rect.Width - QuietZone.Right;
                }
                else
                {
                    left = rect.Left + QuietZone.Left;
                    right = rect.Width - (QuietZone.Right + QuietZone.Left);
                }

                if (m_textDisplayLocation == BarcodeTextLocation.Top)
                {
                    RectangleF layoutRect = new RectangleF(new PointF(left, location.Y + QuietZone.Top - m_barcodeToTextGapHeight - textSize.Height), new SizeF(right, textSize.Height));
                    layoutRect.Y = (layoutRect.Y < 0) ? 0 : layoutRect.Y;
                    g.DrawString(Text, gFont, textBrush, layoutRect, gStringFormat);
                }
                else
                {
                    RectangleF layoutRect = new RectangleF(new PointF(left, location.Y + QuietZone.Bottom + BarcodeToTextGapHeight + BarHeight - 2f), new SizeF(right, textSize.Height));
                    g.DrawString(Text, gFont, textBrush, layoutRect, gStringFormat);
                }
            }

            QuietZone = actualQuiteZones;
            Text = actualText;

            if (state != null && (angle == 90 || angle == 270))
                g.Restore(state);

            return bmp;
        }
#if WINFORMS
        public SizeF Draw(Panel panel)
        {
            PointF location = PointF.Empty;
            string actualText = Text;
            isCheckDigitAdded = false;
            BarcodeQuietZones actualQuiteZones = QuietZone;

            if (!Validate(Text))
            {
                throw new BarcodeException("Barcode Text contains characters that are not accepted by this barcode specification.");
            }

            if (NarrowBarWidth < 1)
            {
                NarrowBarWidth = 1;
            }

            string code = GetTextToEncode();

            float left = 0;
            float right = 0;
            float top = 0;

            if (code == null || code.Length == 0)
            {
                throw new BarcodeException("Barcode Text cannot be null or empty.");
            }

            if (EncodeStartStopSymbols)
            {
                if (StartSymbol != '\0' && StopSymbol != '\0')
                {
                    code = StartSymbol.ToString() + code + StopSymbol.ToString();
                }
            }

            System.Drawing.Font gFont = new Font(Font.Name.ToString(), Font.Size);

            SizeF textSize;

            using (Bitmap img = new Bitmap(1, 1))
            {
                using (Graphics grap = Graphics.FromImage(img))
                    textSize = grap.MeasureString(Text, gFont);
            }
            textSize.Height = gFont.Height;

            float width = QuietZone.Left + QuietZone.Right;

            foreach (char c in code)
            {
                width += GetCharWidth(c) + m_intercharacterGap;
            }

            if (continuous == false)
            {
                width -= m_intercharacterGap * code.Length;
            }
            else
            {
                width -= (ExtendedText.Length > 0) ?
                    m_intercharacterGap * (base.ExtendedText.Length - base.Text.Length) :
                    m_intercharacterGap;
            }

            float height = QuietZone.Top + QuietZone.Bottom + BarHeight + textSize.Height + BarcodeToTextGapHeight;

            if (TextDisplayLocation == BarcodeTextLocation.None)
            {
                height -= (textSize.Height + BarcodeToTextGapHeight);
            }

            System.Drawing.Graphics g = panel.CreateGraphics();
            g.Clear(Color.White);

            RectangleF rect = new RectangleF();

            if (TextDisplayLocation == BarcodeTextLocation.Top)
            {
                PointF loc = location;
                location.Y += BarcodeToTextGapHeight + 10f;
                height += BarcodeToTextGapHeight + 10f;
                rect = new RectangleF(loc, new SizeF(width, height));
            }
            else if (TextDisplayLocation == BarcodeTextLocation.Bottom)
            {
                height += BarcodeToTextGapHeight + 10f;
                rect = new RectangleF(location, new SizeF(width, height));
            }
            else
            {
                rect = new RectangleF(location, new SizeF(width, height));
            }

            Color backcolor = Color.FromArgb(BackColor.ToArgb());
            Brush backgroundBrush = new SolidBrush(backcolor);

            g.FillRectangle(backgroundBrush, rect);

            Bounds = rect;

            left = base.QuietZone.Left + rect.Left;
            top = 0f + rect.Top;

            if (TextDisplayLocation == BarcodeTextLocation.Top)
            {
                top = location.Y + base.QuietZone.Top;
            }
            else
            {
                top = location.Y + base.QuietZone.Top;
            }

            foreach (char c in code)
            {
                foreach (KeyValuePair<char, BarcodeSymbolTable> entry in BarcodeSymbols)
                {
                    BarcodeSymbolTable pattern = (BarcodeSymbolTable)entry.Value;
                    if (pattern.Symbol == c)
                    {
                        byte[] bars = pattern.Bars;
                        for (int i = 0; i < bars.Length; i++)
                        {
                            float barWidth = bars[i] * base.NarrowBarWidth;
                            RectangleF barRect = new RectangleF(left, top, barWidth, base.BarHeight);
                            if (i % 2 == 0)
                            {
                                left += PaintToImage(ref g, barRect);
                            }
                            else
                            {
                                left += barWidth;
                            }
                        }

                        if (bars.Length % 2 != 0)
                        {
                            left += IntercharacterGap;
                        }
                    }
                }
            }

            if (TextDisplayLocation != BarcodeTextLocation.None)
            {
                base.Text = base.Text.Trim(m_startSymbol);
                base.Text = base.Text.Trim(m_stopSymbol);

                StringFormat gStringFormat = new StringFormat();
                gStringFormat.LineAlignment = StringAlignment.Center;

                if (TextAlignment == BarcodeTextAlignment.Left)
                {
                    gStringFormat.Alignment = StringAlignment.Near;
                }
                else if (TextAlignment == BarcodeTextAlignment.Right)
                {
                    gStringFormat.Alignment = StringAlignment.Far;
                }
                else
                {
                    gStringFormat.Alignment = StringAlignment.Center;
                }

                Color textColor = Color.FromArgb(TextColor.ToArgb());
                SolidBrush textBrush = new SolidBrush(textColor);

                if (TextAlignment == BarcodeTextAlignment.Left)
                {
                    left = rect.Left + QuietZone.Left;
                    right = rect.Width;
                }
                else if (TextAlignment == BarcodeTextAlignment.Right)
                {
                    left = rect.Left;
                    right = rect.Width - QuietZone.Right;
                }
                else
                {
                    left = rect.Left + QuietZone.Left;
                    right = rect.Width - (QuietZone.Right + QuietZone.Left);
                }

                if (m_textDisplayLocation == BarcodeTextLocation.Top)
                {
                    RectangleF layoutRect = new RectangleF(new PointF(left, location.Y + QuietZone.Top - m_barcodeToTextGapHeight - textSize.Height), new SizeF(right, textSize.Height));
                    layoutRect.Y = (layoutRect.Y < 0) ? 0 : layoutRect.Y;
                    g.DrawString(Text, gFont, textBrush, layoutRect, gStringFormat);
                }
                else
                {
                    RectangleF layoutRect = new RectangleF(new PointF(left, location.Y + QuietZone.Bottom + BarcodeToTextGapHeight + BarHeight - 2f), new SizeF(right, textSize.Height));
                    g.DrawString(Text, gFont, textBrush, layoutRect, gStringFormat);
                }
            }
            else
                right = rect.Width;

            QuietZone = actualQuiteZones;
            Text = actualText;
            return new SizeF(right + base.QuietZone.Right + base.QuietZone.Left, location.Y + base.QuietZone.Top + m_barcodeToTextGapHeight + base.BarHeight + textSize.Height);
        }
#endif
#else
        internal virtual void Draw(Canvas m_canvas)
        {
            // Draw barcode in Canvas
            m_textDisplayLocation = BarcodeTextLocation.Bottom;

            string actualText = Text;
            Point location = new Point(0, 0);

            string code = GetTextToEncode();
            double left = 0;
            double right = 0;
            double top = 0;
            double bottom = 0;

            if (code == null || code.Length == 0)
                throw new Exception("Barcode Text cannot be null or empty.");

            if (m_encodeStartStopSymbols)
            {
                if (m_startSymbol != '\0' && m_stopSymbol != '\0')
                    code = m_startSymbol.ToString() + code + m_stopSymbol.ToString();
            }

            Size textSize = new Size(0, 0);
            float w = base.QuietZone.Left + base.QuietZone.Right;
            float h = QuietZone.Top + QuietZone.Bottom + BarHeight;

            Rect rect = new Rect();
            rect = new Rect(location, new Size(w, h));

            Bounds = rect;

            left = base.QuietZone.Left + rect.Left;
            top = 0f + rect.Top;

            if (m_textDisplayLocation == BarcodeTextLocation.Top)
                top = location.Y;
            else
                top = location.Y + base.QuietZone.Top;
            
            foreach (char c in code)
            {
                foreach (KeyValuePair<char, BarcodeSymbolTable> entry in BarcodeSymbols)
                {
                    BarcodeSymbolTable pattern = (BarcodeSymbolTable)entry.Value;
                    if (pattern.Symbol == c)
                    {
                        byte[] bars = pattern.Bars;
                        for (int i = 0; i < bars.Length; i++)
                        {
                            float barWidth = bars[i] * base.NarrowBarWidth;
                            Rect barRect = new Rect(left, top, barWidth, base.BarHeight);
                            if (i % 2 == 0)
                            {
                                RectangleGeometry rectangleGeometry = new RectangleGeometry();

                                Path path = new Path();
                                path.Fill = new SolidColorBrush(BarColor);
#if XAML && !BARCODE_WINRT
                                rectangleGeometry.Rect = new System.Windows.Rect(barRect.X, barRect.Y, barRect.Width, barRect.Height);
#else
                                rectangleGeometry.Rect = new Rect(barRect.X, barRect.Y, barRect.Width, barRect.Height);
#endif
                                path.Data = rectangleGeometry;
                                m_canvas.Children.Add(path);

                                left += barRect.Width;
                            }
                            else
                            {
                                left += barWidth;
                            }
                            bottom = barRect.Height;
                        }

                        if (bars.Length % 2 != 0)
                            left += IntercharacterGap;
                    }
                }
            }

            m_canvas.Width = left;
            m_canvas.Height = h;
            m_canvas.Background = new SolidColorBrush(BackColor);
        }
#endif
        /// <summary>
        /// Internal method used to validate the given barcode text.
        /// </summary>
        /// <param name="data">The Text.</param>
        /// <returns>True if valid, Otherwise False.</returns>
        protected internal override bool Validate(string data)
        {
            m_codeValidator = new Regex(m_validatorExpression
#if !BARCODE_WINRT && !NETFX_CORE && !BARCODE_SILVERLIGHT && !WP
, RegexOptions.Compiled);
#else
);
#endif
            return m_codeValidator.Match(data).Success;
        }

# if !XAML
        /// <summary>
        /// Returns the size of the barcode.
        /// </summary>
        /// <returns>The Size.</returns>
        protected internal override SizeF GetSize()
        {
            float height = GetHeight();
            float width = BarcodeWidth();
            return new SizeF(width, height);

        }

        /// <summary>
        /// Returns the Width of the barcode.
        /// </summary>
        /// <returns></returns>
        private float BarcodeWidth()
        {
            string text = base.Text;

            if (EnableCheckDigit == true)
            {
                if (isCheckDigitAdded == true)
                {
                    isCheckDigitAdded = false;
                }
            }

            string code = GetTextToEncode();

            isCheckDigitAdded = false;
            base.ExtendedText = "";
            base.Text = text;

            if (code == null || code.Length == 0)
            {
#if !NETFX_CORE && !GDI && !WP
                throw new PdfBarcodeException("Barcode Text cannot be null or empty.");
#else
                throw new BarcodeException("Barcode Text cannot be null or empty.");
#endif
            }

            if (m_encodeStartStopSymbols)
            {
                if (m_startSymbol != '\0' && m_stopSymbol != '\0')
                    code = m_startSymbol.ToString() + code + m_stopSymbol.ToString();
            }
            
            //SizeF textSize = Font.MeasureString(base.Text);
            float w = base.QuietZone.Left + base.QuietZone.Right;
            foreach (char c in code)
                w += GetCharWidth(c) + m_intercharacterGap;

            float left = 0;

            foreach (char c in code)
            {
                foreach (KeyValuePair<char, BarcodeSymbolTable> entry in BarcodeSymbols)
                {
                    BarcodeSymbolTable pattern = (BarcodeSymbolTable)entry.Value;
                    if (pattern.Symbol == c)
                    {
                        byte[] bars = pattern.Bars;
                        for (int i = 0; i < bars.Length; i++)
                        {
                            float barWidth = bars[i] * base.NarrowBarWidth;
                            left += barWidth;
                        }

                        if (bars.Length % 2 != 0)
                        {
                            left += IntercharacterGap;
                        }
                    }
                }
            }

            return left;

        }
#endif
        /// <summary>
        /// Calculates the check digit based on the barcode specification.
        /// </summary>
        /// <returns>Char array containing Check digits</returns>
        protected internal virtual char[] CalculateCheckDigit()
        {
            return null;
        }

        /// <summary>
        /// Returns the Character width.
        /// </summary>
        /// <param name="c"></param>
        /// <returns>The width.</returns>
        protected float GetCharWidth(char c)
        {
            float w = 0;
            foreach (KeyValuePair<char, BarcodeSymbolTable> entry in BarcodeSymbols)
            {
                BarcodeSymbolTable pattern = (BarcodeSymbolTable)entry.Value;
                if (pattern.Symbol == c)
                {
                    byte[] bars = pattern.Bars;
                    for (int i = 0; i < bars.Length; i++)
                    {
                        if (check == false)
                        {
                            if (bars.Length % 2 != 0)
                            {
                                continuous = true;
                            }
                        }

                        w += bars[i] * base.NarrowBarWidth;
                        check = true;
                    }

                    return w;
                }
            }

            return 0;
        }

        /// <summary>
        /// Returns the Actual text to encode.
        /// </summary>
        /// <returns>The Actual Text.</returns>
        protected virtual string GetTextToEncode()
        {
            if (!Validate(base.Text))
            {
#if!XAML && !NETFX_CORE && !GDI && !WP
                throw new PdfBarcodeException("Barcode text contains characters that are not accepted by this barcode specification.");
#else
                throw new BarcodeException("Barcode text contains characters that are not accepted by this barcode specification.");
#endif
            }

            string code = (base.ExtendedText.Equals(string.Empty)) ? base.Text.Trim('*') : base.ExtendedText.Trim('*');

            if (isCheckDigitAdded || !EnableCheckDigit)
            {
                return code;
            }

            char[] checkDigit = CalculateCheckDigit();
            //Note : Some barcode symbologies does not have check digits.
            if (checkDigit == null || checkDigit.Length == 0)
            {
                return code;
            }

            if (EnableCheckDigit && checkDigit[checkDigit.Length - 1] != '\0' && !isCheckDigitAdded)
            {
                foreach (char c in checkDigit)
                {
                    code += c.ToString();
                }
            }

            if (ShowCheckDigit && checkDigit[checkDigit.Length - 1] != '\0' && !isCheckDigitAdded)
            {
                if (code[code.Length - 1] != checkDigit[checkDigit.Length - 1])
                {
                    foreach (char c in checkDigit)
                    {
                        code += c.ToString();
                    }
                }

                isCheckDigitAdded = true;

                if (base.ExtendedText.Equals(string.Empty))
                {
                    code = base.Text;
                    foreach (char c in checkDigit)
                    {
                        code += c.ToString();
                    }
                }
                else
                {
                    code = base.ExtendedText;
                    foreach (char c in checkDigit)
                    {
                        code += c.ToString();
                    }
                }
            }

            if (ShowCheckDigit)
            {
                foreach (char c in checkDigit)
                {
                    base.Text += c.ToString();
                }
            }

            isCheckDigitAdded = true;
            return code;
        }
#if !XAML
#if !GDI
        /// <summary>
        /// Internal method used to paint bars on the page.
        /// </summary>
        /// <param name="page">The Page.</param>
        /// <param name="barRect">The Rectangle.</param>
        /// <returns>Returns the right margin.</returns>
        protected virtual float PaintRectangle(PdfPageBase page, RectangleF barRect)
        {
            Color barColor = Color.FromArgb(base.BarColor.A, base.BarColor.R, base.BarColor.G, base.BarColor.B);
            PdfBrush brush = new PdfSolidBrush(barColor);
            page.Graphics.DrawRectangle(brush, barRect);
            return barRect.Width;
        }
#endif
        /// <summary>
        /// Returns the width of the barcode.
        /// </summary>
        /// <returns>The Width.</returns>
        private float GetWidth()
        {
            float w = base.QuietZone.Left + base.QuietZone.Right;
            foreach (char c in base.Text)
            {
                w += GetCharWidth(c) + m_intercharacterGap;
            }

            if (continuous == true)
            {
                w -= m_intercharacterGap * base.Text.Length;
            }
            else
            {
                w -= m_intercharacterGap;
            }

            return w;
        }

#if GDI
        private float GetHeight()
        {
            using (Bitmap bmp = new Bitmap(1, 1))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    float h = base.QuietZone.Top + base.QuietZone.Bottom + base.BarHeight;
                    SizeF textSize = g.MeasureString(base.Text, Font);
                    if (m_textDisplayLocation == BarcodeTextLocation.Bottom || m_textDisplayLocation == BarcodeTextLocation.Top)
                    {
                        h += textSize.Height + 10f;
                    }

                    return h;
                }
            }
        }
#else
        /// <summary>
        /// Returns the height of the barcode.
        /// </summary>
        /// <returns>The Height.</returns>
        private float GetHeight()
        {
            float h = base.QuietZone.Top + base.QuietZone.Bottom + base.BarHeight;
            SizeF textSize = Font.MeasureString(base.Text);
            if (m_textDisplayLocation == TextLocation.Bottom || m_textDisplayLocation == TextLocation.Top)
            {
                h += textSize.Height + 10f;
            }

            return h;
        }
#endif
        /// <summary>
        /// Internal method used to paint bars on the image.
        /// </summary>
        /// <param name="g">The graphics to draw.</param>
        /// <param name="barRect">The Rectangle.</param>
        /// <returns>Returns the right margin.</returns>
#if !NETFX_CORE && !WP
        private float PaintToImage(ref System.Drawing.Graphics g, RectangleF barRect)
        {
            Color barColor = Color.FromArgb(BarColor.ToArgb());
            SolidBrush brush = new SolidBrush(barColor);
            g.FillRectangle(brush, barRect);
            return barRect.Width;
        }
#endif
#endif
        #endregion
    }
}
#endif