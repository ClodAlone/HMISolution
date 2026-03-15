#if !SILVERLIGHT
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
#if !XAML && !GDI
using System.Drawing;
using Syncfusion.Pdf.Graphics;
#elif BARCODE_WINRT
using Windows.Foundation;
using Windows.UI;
#elif GDI
using System.Drawing;
#endif

#if XAML && !BARCODE_WINRT
using System.Windows.Media;
using System.Windows;
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
    /// Represents a base class for all barcode types.
    /// </summary>
# if !XAML && !GDI
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// //Creates a new PdfCodabarBarcode.
    /// PdfCodabarBarcode codaBarcode = new PdfCodabarBarcode();
    /// //Sets the barcode text.
    /// codaBarcode.Text = "0123";
    /// //Draws a barcode on the new Page.
    /// codaBarcode.Draw(page, new PointF(25, 500));
    /// //Save document to disk.
    /// document.Save("PdfBarcode.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Creates a new PdfCodabarBarcode.
    /// Dim codaBarcode As PdfCodabarBarcode = New PdfCodabarBarcode()
    /// 'Sets the barcode text.
    /// codaBarcode.Text = "0123"
    /// 'Draws a barcode on the new Page.
    /// codaBarcode.Draw(page, new PointF(25, 500))
    /// 'Save document to disk.
    /// document.Save("PdfBarcode.pdf")
    /// </code>
    /// </example>
    public class PdfBarcode
#else
    public class BarcodeBase
#endif
    {
        #region Fields

        /// <summary>
        /// Indicates the region of the barcode.
        /// </summary>
#if !XAML
        protected internal RectangleF m_bounds;
#else
        protected internal Rect m_bounds;
#endif

        /// <summary>
        /// Indicates the backColor of the barcode.
        /// </summary>
#if !XAML && !GDI
        private PdfColor m_backColor;
#elif XAML && !BARCODE_WINRT
        private System.Windows.Media.Color m_backColor;
#elif GDI
        private Color m_backColor;
#else
        private Windows.UI.Color m_backColor;
#endif

        /// <summary>
        /// Indicates the barcolor of the barcode.
        /// </summary>
#if !XAML && !GDI
        private PdfColor m_barColor;
#elif XAML && !BARCODE_WINRT
        private System.Windows.Media.Color m_barColor;
#elif GDI
        private Color m_barColor;
#else
        private Windows.UI.Color m_barColor;
#endif

        /// <summary>
        /// Indicates the textcolor of the barcode.
        /// </summary>
#if !XAML && !GDI
        private PdfColor m_textColor;
#elif XAML && !BARCODE_WINRT
        private System.Windows.Media.Color m_textColor;
#elif GDI
        private Color m_textColor;
#else
        private Windows.UI.Color m_textColor;
#endif

        /// <summary>
        /// Indicates the narrow bar width.
        /// </summary>
        private float m_narrowBarWidth;

        /// <summary>
        /// Indicates the wide bar width.
        /// </summary>
        private float m_wideBarWidth;

        /// <summary>
        /// Indicates the location on where to draw the barcode in the PDF Document.
        /// </summary>
#if !XAML
        private PointF m_location;
#else
        private Point m_location;
#endif

        /// <summary>
        /// Indicates the data string which is to be encoded.
        /// </summary>
        private string m_text = string.Empty;

        /// <summary>
        /// Indicates the free area around the barcode label.
        /// </summary>
#if !XAML && !GDI
        private PdfBarcodeQuietZones m_quietZones;
# else
        private BarcodeQuietZones m_quietZones;
#endif

        /// <summary>
        /// Indicates the actual width of the barcode.
        /// </summary>
        private float m_width;

        /// <summary>
        /// Indicates the actual height of the barcode.
        /// </summary>
        private float m_height;

        /// <summary>
        /// Indicates the bar height of the barcode.
        /// </summary>
        private float m_barHeight;

        /// <summary>
        /// Indicates the extended text;
        /// </summary>
        private string m_extendedText = string.Empty;
        #endregion

        #region Constructor
# if !XAML && !GDI
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfBarcode"/> class.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Creates a new PdfCodabarBarcode.
        /// PdfCodabarBarcode codaBarcode = new PdfCodabarBarcode();
        /// //Sets the barcode text.
        /// codaBarcode.Text = "0123";
        /// //Draws a barcode on the new Page.
        /// codaBarcode.Draw(page, new PointF(25, 500));
        /// //Save document to disk.
        /// document.Save("PdfBarcode.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Creates a new PdfCodabarBarcode.
        /// Dim codaBarcode As PdfCodabarBarcode = New PdfCodabarBarcode()
        /// 'Sets the barcode text.
        /// codaBarcode.Text = "0123"
        /// 'Draws a barcode on the new Page.
        /// codaBarcode.Draw(page, new PointF(25, 500))
        /// 'Save document to disk.
        /// document.Save("PdfBarcode.pdf")
        /// </code>
        /// </example>
        public PdfBarcode()
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="Barcode"/> class.
        /// </summary>
        public BarcodeBase()
#endif
        {
            Initialize();
        }

#if !XAML && !GDI
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfBarcode"/> class.
        /// </summary>
        /// <param name="text">Set the barcode text.</param>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Creates a new PdfCodabarBarcode.
        /// PdfCodabarBarcode codaBarcode = new PdfCodabarBarcode("0123");
        /// //Draws a barcode on the new Page.
        /// codaBarcode.Draw(page, new PointF(25, 500));
        /// //Save document to disk.
        /// document.Save("PdfBarcode.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Creates a new PdfCodabarBarcode.
        /// Dim codaBarcode As PdfCodabarBarcode = New PdfCodabarBarcode("0123")
        /// 'Draws a barcode on the new Page.
        /// codaBarcode.Draw(page, new PointF(25, 500))
        /// 'Save document to disk.
        /// document.Save("PdfBarcode.pdf")
        /// </code>
        /// </example>
        public PdfBarcode(string text)
            : this()
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="Barcode"/> class.
        /// </summary>
        /// <param name="text">Set the barcode text.</param>
        public BarcodeBase(string text)
            : this()
# endif
        {
            this.Text = text;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the back color of the barcode.
        /// </summary>
#if !XAML && !GDI
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Creates a new PdfCode93Barcode.
        /// PdfCode93Barcode code93 = new PdfCode93Barcode("CODE93");
        /// //Sets the barcode back color.
        /// code93.BackColor = new PdfColor(Color.Green);
        /// //Draws a barcode on the new Page.
        /// code93.Draw(page, new PointF(25, 500));
        /// //Save document to disk.
        /// document.Save("PdfBarcode.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Creates a new PdfCode93Barcode.
        /// Dim code39 As PdfCode93Barcode = New PdfCode93Barcode("CODE93")
        /// 'Sets the barcode back color.
        /// code93.BackColor = new PdfColor(Color.Green)
        /// 'Draws a barcode on the new Page.
        /// code93.Draw(page, new PointF(25, 500))
        /// 'Save document to disk.
        /// document.Save("PdfBarcode.pdf")
        /// </code>
        /// </example> 
        public PdfColor BackColor
#elif XAML && !BARCODE_WINRT
        public System.Windows.Media.Color BackColor
#elif GDI
        public Color BackColor
#else
        public Windows.UI.Color BackColor
#endif
        {
            get
            {
                return m_backColor;
            }

            set
            {
                m_backColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the bar color of the barcode.
        /// </summary>
#if !XAML && !GDI
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Creates a new PdfCode93Barcode.
        /// PdfCode93Barcode code93 = new PdfCode93Barcode("CODE93");
        /// //Sets the barcode color.
        /// code93.BarColor = new PdfColor(Color.Green);
        /// //Draws a barcode on the new Page.
        /// code93.Draw(page, new PointF(25, 500));
        /// //Save document to disk.
        /// document.Save("PdfBarcode.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Creates a new PdfCode93Barcode.
        /// Dim code39 As PdfCode93Barcode = New PdfCode93Barcode("CODE93")
        /// 'Sets the barcode color.
        /// code93.BarColor = new PdfColor(Color.Green)
        /// 'Draws a barcode on the new Page.
        /// code93.Draw(page, new PointF(25, 500))
        /// 'Save document to disk.
        /// document.Save("PdfBarcode.pdf")
        /// </code>
        /// </example> 
        public PdfColor BarColor
#elif XAML && !BARCODE_WINRT
        public System.Windows.Media.Color BarColor
#elif GDI
        public Color BarColor
#else
        public Windows.UI.Color BarColor
#endif
        {
            get
            {
                return m_barColor;
            }

            set
            {
                m_barColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the text color of the barcode text.
        /// </summary>
#if !XAML && !GDI
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Creates a new PdfCode93Barcode.
        /// PdfCode93Barcode code93 = new PdfCode93Barcode("CODE93");
        /// //Sets the barcode text color.
        /// code93.TextColor = new PdfColor(Color.Green);
        /// //Draws a barcode on the new Page.
        /// code93.Draw(page, new PointF(25, 500));
        /// //Save document to disk.
        /// document.Save("PdfBarcode.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Creates a new PdfCode93Barcode.
        /// Dim code39 As PdfCode93Barcode = New PdfCode93Barcode("CODE93")
        /// 'Sets the barcode text color.
        /// code93.TextColor = new PdfColor(Color.Green)
        /// 'Draws a barcode on the new Page.
        /// code93.Draw(page, new PointF(25, 500))
        /// 'Save the document to disk.
        /// document.Save("PdfBarcode.pdf")
        /// </code>
        /// </example> 
        public PdfColor TextColor
#elif XAML && !BARCODE_WINRT
        private System.Windows.Media.Color TextColor
#elif GDI
        public Color TextColor
#else
        public Windows.UI.Color TextColor
#endif
        {
            get
            {
                return m_textColor;
            }

            set
            {
                m_textColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the narrow bar width.
        /// </summary>
# if !XAML && !GDI
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Creates a new PdfCode93Barcode.
        /// PdfCode93Barcode code93 = new PdfCode93Barcode("CODE93");
        /// //Sets the barcode narrow width.
        /// code93.NarrowBarWidth =1f;
        /// //Draws a barcode on the new Page.
        /// code93.Draw(page, new PointF(25, 500));
        /// //Save document to disk.
        /// document.Save("PdfBarcode.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Creates a new PdfCode93Barcode.
        /// Dim code39 As PdfCode93Barcode = New PdfCode93Barcode("CODE93")
        /// 'Sets the barcode narrow width.
        /// code93.NarrowBarWidth = 1f
        /// 'Draws a barcode on the new Page.
        /// code93.Draw(page, new PointF(25, 500))
        /// 'Save document to disk.
        /// document.Save("PdfBarcode.pdf")
        /// </code>
        /// </example> 
#endif
        public float NarrowBarWidth
        {
            get
            {
                return m_narrowBarWidth;
            }

            set
            {
                m_narrowBarWidth = value;
            }
        }

        /// <summary>
        /// Gets or Sets the barcode text.
        /// </summary>
# if !XAML && !GDI
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Creates a new PdfCode93Barcode.
        /// PdfCode93Barcode code93 = new PdfCode93Barcode();
        /// //Sets the barcode text.
        /// code93.Text ="CODE93";
        /// //Draws a barcode on the new Page.
        /// code93.Draw(page, new PointF(25, 500));
        /// //Save document to disk.
        /// document.Save("PdfBarcode.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Creates a new PdfCode93Barcode.
        /// Dim code39 As PdfCode93Barcode = New PdfCode93Barcode()
        /// 'Sets the barcode back color.
        /// code93.Text ="CODE93"
        /// 'Draws a barcode on the new Page.
        /// code93.Draw(page, new PointF(25, 500))
        /// 'Save document to disk.
        /// document.Save("PdfBarcode.pdf")
        /// </code>
        /// </example> 
#endif
        public string Text
        {
            get
            {
                return m_text;
            }

            set
            {
                m_text = value;
            }
        }

#if !XAML
        /// <summary>
        /// Gets or sets the location to render barcode in the PDF Document.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfCode93Barcode code93 = new PdfCode93Barcode("CODE93");
        /// //Sets the barcode location.
        /// code93.Location = new PointF(50, 50);
        /// //Draws a barcode on the new Page.
        /// code93.Draw(page, new PointF(25, 500));
        /// //Save document to disk.
        /// document.Save("PdfBarcode.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Creates a new PdfCode93Barcode.
        /// Dim code93 As PdfCode93Barcode = New PdfCode93Barcode("CODE93")
        /// 'Sets the barcode location.
        /// code93.Location = new PointF(50, 50)
        /// 'Draws a barcode on the new Page.
        /// code93.Draw(page, new PointF(25, 500))
        /// 'Save document to disk.
        /// document.Save("PdfBarcode.pdf")
        /// </code>
        /// </example> 
        public PointF Location
#else
        public Point Location
#endif
        {
            get
            {
                return m_location;
            }

            set
            {
                m_location = value;
            }
        }

        /// <summary>
        /// Gets or sets the empty area which is to be allocated around the barcode.
        /// </summary>
# if !XAML && !GDI
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Creates a new PdfCode93Barcode.
        /// PdfCode93Barcode code93 = new PdfCode93Barcode("CODE93");
        /// //Creates a new PdfBarcodeQuietZones.
        /// PdfBarcodeQuietZones quietZones = new PdfBarcodeQuietZones();
        /// quietZones.All = 0f;
        /// //Sets the barcode quiet zone.
        /// code93QuietZone = quietZones;
        /// //Draws a barcode on the new Page.
        /// code93.Draw(page, new PointF(25, 500));
        /// //Save document to disk.
        /// document.Save("PdfBarcode.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Creates a new PdfCode93Barcode.
        /// Dim code39 As PdfCode93Barcode = New PdfCode93Barcode("CODE93")
        /// 'Creates a new PdfBarcodeQuietZones.
        /// PdfBarcodeQuietZones quietZones = new PdfBarcodeQuietZones()
        /// quietZones.All = 0f
        /// 'Sets the barcode quiet zone.
        /// code93QuietZone = quietZones
        /// 'Draws a barcode on the new Page.
        /// code93.Draw(page, new PointF(25, 500))
        /// 'Save document to disk.
        /// document.Save("PdfBarcode.pdf")
        /// </code>
        /// </example> 
        public PdfBarcodeQuietZones QuietZone
#else
        public BarcodeQuietZones QuietZone
#endif
        {
            get
            {
                return m_quietZones;
            }

            set
            {
                m_quietZones = value;
            }
        }

        /// <summary>
        /// Gets or sets the bar height.
        /// </summary>
# if !XAML && !GDI
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Creates a new PdfCode93Barcode.
        /// PdfCode93Barcode code93 = new PdfCode93Barcode("CODE93");
        /// //Sets the barcode height.
        /// code93.BarHeight = 50f;
        /// //Draws a barcode on the new Page.
        /// code93.Draw(page, new PointF(25, 500));
        /// //Save document to disk.
        /// document.Save("PdfBarcode.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Creates a new PdfCode93Barcode.
        /// Dim code39 As PdfCode93Barcode = New PdfCode93Barcode("CODE93")
        /// 'Sets the barcode height.
        /// code93.BarHeight = 50f
        /// 'Draws a barcode on the new Page.
        /// code93.Draw(page, new PointF(25, 500))
        /// 'Save document to disk.
        /// document.Save("PdfBarcode.pdf")
        /// </code>
        /// </example> 
#endif
        public float BarHeight
        {
            get
            {
                return m_barHeight;
            }

            set
            {
                m_barHeight = value;
            }
        }

        /// <summary>
        /// Gets the size of the barcode.
        /// </summary>
#if !XAML
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Creates a new PdfCode93Barcode.
        /// PdfCode93Barcode code93 = new PdfCode93Barcode("CODE93");
        /// //Sets the barcode size
        /// SizeF size=code93.Size;
        /// //Draws a barcode on the new Page.
        /// code93.Draw(page, new PointF(25, 500));
        /// //Save document to disk.
        /// document.Save("PdfBarcode.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Creates a new PdfCode93Barcode.
        /// Dim code39 As PdfCode93Barcode = New PdfCode93Barcode("CODE93")
        /// 'Get the barcode size.
        /// SizeF size=code93.Size
        /// 'Draws a barcode on the new Page.
        /// code93.Draw(page, new PointF(25, 500))
        /// 'Save document to disk.
        /// document.Save("PdfBarcode.pdf")
        /// </code>
        /// </example> 
        public SizeF Size
#else
        public Size Size
#endif
        {
            get
            {
                return GetSize();
            }
        }

        /// <summary>
        /// Gets or sets the rectangular area occupied by the barcode.
        /// </summary>
#if !XAML
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Creates a new PdfCode93Barcode.
        /// PdfCode93Barcode code93 = new PdfCode93Barcode("CODE93");
        /// //Sets the barcode bounds.
        /// RectangleF bounds=code39.Bounds;
        /// //Draws a barcode on the new Page.
        /// code93.Draw(page, new PointF(25, 500));
        /// //Save document to disk.
        /// document.Save("PdfBarcode.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Creates a new PdfCode93Barcode.
        /// Dim code39 As PdfCode93Barcode = New PdfCode93Barcode("CODE93")
        /// 'Get the barcode bounds.
        /// RectangleF bounds=code39.Bounds
        /// 'Draws a barcode on the new Page.
        /// code93.Draw(page, new PointF(25, 500))
        /// 'Save document to disk.
        /// document.Save("PdfBarcode.pdf")
        /// </code>
        /// </example> 
        public RectangleF Bounds
#else
        public Rect Bounds
#endif
        {
            get
            {
                return m_bounds;
            }

            set
            {
                m_bounds = value;
            }
        }

        /// <summary>
        /// Gets or sets the barcode text.
        /// </summary>
        internal string ExtendedText
        {
            get
            {
                return m_extendedText;
            }

            set
            {
                m_extendedText = value;
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Internal method which validates whether the given text is acceptable by the current barcode
        /// specification or not.
        /// </summary>
        /// <param name="data">The Text.</param>
        /// <returns>True if Valid, Otherwise False</returns>
        protected internal virtual bool Validate(string data)
        {
            return true;
        }

        /// <summary>
        /// Internal method which calculates the size of the barcode which is going to rendered.
        /// </summary>
        /// <returns></returns>
# if !XAML
        protected internal virtual SizeF GetSize()
        {
            return new SizeF(0, 0);
        }
#else
        protected internal virtual Size GetSize()
        {
            return new Size(0, 0);
        }
#endif

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
#if !XAML && !GDI
#if !NETFX_CORE && !GDI && !WP
            m_barColor = new PdfColor(Color.Black);
            m_backColor = new PdfColor(Color.White);
            m_textColor = new PdfColor(Color.Black);
#else
            m_barColor = new PdfColor(Color.FromArgb(255,0,0,0));
            m_backColor = new PdfColor(Color.FromArgb(255, 255, 255, 255));
            m_textColor = new PdfColor(Color.FromArgb(255, 0, 0, 0));
#endif
            m_narrowBarWidth = 0.864f;
            m_wideBarWidth = 2.592f;
            PdfBarcodeQuietZones quietZones = new PdfBarcodeQuietZones();
#else
#if GDI
            m_barColor = Color.Black;
            m_backColor = Color.White;
            m_textColor = Color.Black;
#else
            m_barColor = Colors.Black;
            m_backColor = Colors.White;
            m_textColor = Colors.Black;
#endif
            m_narrowBarWidth = 1f;
            m_wideBarWidth = 3f;
            BarcodeQuietZones quietZones = new BarcodeQuietZones();
#endif
            quietZones.All = 0f;
            m_quietZones = quietZones;
            m_barHeight = 80f;
        }

        #endregion
    }
}
#endif