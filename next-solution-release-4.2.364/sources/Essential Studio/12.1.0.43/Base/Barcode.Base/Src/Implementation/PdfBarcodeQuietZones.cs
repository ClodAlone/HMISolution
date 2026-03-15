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
    /// Represents the Class for specifying Quiet zones around the barcode.
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
    /// //Set the barcode quiet zone.
    /// code93.QuietZone = quietZones;
    /// //Draw a barcode in the new Page.
    /// code93.Draw(page, new PointF(25, 500));
    /// //Save document to disk.
    /// document.Save("code93.pdf");
    /// </code>
    /// <code lang="VB">
    /// ' Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Creates a new PdfCode93Barcode.
    /// Dim code39 As PdfCode93Barcode = New PdfCode93Barcode("CODE93")
    /// 'Creates a new PdfBarcodeQuietZones.
    /// PdfBarcodeQuietZones quietZones = new PdfBarcodeQuietZones()
    /// quietZones.All = 0f;
    /// 'Set the barcode quiet zone.
    /// code93.QuietZone = quietZones
    /// 'Draw a barcode in the new Page.
    /// code93.Draw(page, new PointF(25, 500))
    /// 'Save the document.
    /// document.Save("code93.pdf")
    /// </code>
    /// </example> 
    public sealed class PdfBarcodeQuietZones
#else
    public sealed class BarcodeQuietZones
#endif
    {
#region Class constants
        /// <summary>
        /// Internal variable to store margin.
        /// </summary>
        private const float DEF_MARGIN = 0f;
        #endregion

#region Class members
        /// <summary>
        /// Internal variable to store right margin.
        /// </summary>
        private float m_right = DEF_MARGIN;

        /// <summary>
        /// Internal variable to store top margin.
        /// </summary>
        private float m_top = DEF_MARGIN;

        /// <summary>
        /// Internal variable to store left margin.
        /// </summary>
        private float m_left = DEF_MARGIN;

        /// <summary>
        /// Internal variable to store bottom margin.
        /// </summary>
        private float m_bottom = DEF_MARGIN;
        #endregion

#region Class properties
        /// <summary>
        /// Gets or sets the quiet zones at the right side of the barcode.
        /// </summary>
# if !XAML
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
        /// quietZones.Right = 0f;
        /// //Set the barcode quiet zone.
        /// code93.QuietZone = quietZones;
        /// //Draw a barcode in the new Page.
        /// code93.Draw(page, new PointF(25, 500));
        /// //Save document to disk.
        /// document.Save("code93.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Creates a new PdfCode93Barcode.
        /// Dim code39 As PdfCode93Barcode = New PdfCode93Barcode("CODE93")
        /// 'Creates a new PdfBarcodeQuietZones.
        /// PdfBarcodeQuietZones quietZones = new PdfBarcodeQuietZones()
        /// quietZones.Right = 0f;
        /// 'Set the barcode quiet zone.
        /// code93.QuietZone = quietZones
        /// 'Draw a barcode in the new Page.
        /// code93.Draw(page, new PointF(25, 500))
        /// 'Save the document.
        /// document.Save("code93.pdf")
        /// </code>
        /// </example> 
#endif
        public float Right
        {
            get
            {
                return m_right;
            }

            set
            {
                m_right = value;
            }
        }

        /// <summary>
        ///  Gets or sets the quiet zones at Top of the barcode.
        /// </summary>
# if !XAML
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
        /// quietZones.Top = 0f;
        /// //Set the barcode quiet zone.
        /// code93.QuietZone = quietZones;
        /// //Draw a barcode in the new Page.
        /// code93.Draw(page, new PointF(25, 500));
        /// //Save document to disk.
        /// document.Save("code93.pdf");
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
        /// quietZones.Top = 0f;
        /// 'Set the barcode quiet zone.
        /// code93.QuietZone = quietZones
        /// 'Draw a barcode in the new Page.
        /// code93.Draw(page, new PointF(25, 500))
        /// 'Save the document.
        /// document.Save("code93.pdf")
        /// </code>
        /// </example> 
#endif
        public float Top
        {
            get
            {
                return m_top;
            }

            set
            {
                m_top = value;
            }
        }

        /// <summary>
        ///  Gets or sets the quiet zones at the left side of the barcode.
        /// </summary>
# if !XAML
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
        /// quietZones.Left = 0f;
        /// //Set the barcode quiet zone.
        /// code93.QuietZone = quietZones;
        /// //Draw a barcode in the new Page.
        /// code93.Draw(page, new PointF(25, 500));
        /// //Save document to disk.
        /// document.Save("code93.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Creates a new PdfCode93Barcode.
        /// Dim code39 As PdfCode93Barcode = New PdfCode93Barcode("CODE93")
        /// 'Creates a new PdfBarcodeQuietZones.
        /// PdfBarcodeQuietZones quietZones = new PdfBarcodeQuietZones()
        /// quietZones.Left = 0f;
        /// 'Set the barcode quiet zone.
        /// code93.QuietZone = quietZones
        /// 'Draw a barcode in the new Page.
        /// code93.Draw(page, new PointF(25, 500))
        /// 'Save the document.
        /// document.Save("code93.pdf")
        /// </code>
        /// </example> 
#endif
        public float Left
        {
            get
            {
                return m_left;
            }

            set
            {
                m_left = value;
            }
        }

        /// <summary>
        ///  Gets or sets the quiet zones at bottom of the barcode.
        /// </summary>
# if !XAML
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
        /// quietZones.Bottom = 0f;
        /// //Set the barcode quiet zone.
        /// code93.QuietZone = quietZones;
        /// //Draw a barcode in the new Page.
        /// code93.Draw(page, new PointF(25, 500));
        /// //Save document to disk.
        /// document.Save("code93.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Creates a new PdfCode93Barcode.
        /// Dim code39 As PdfCode93Barcode = New PdfCode93Barcode("CODE93")
        /// 'Creates a new PdfBarcodeQuietZones.
        /// PdfBarcodeQuietZones quietZones = new PdfBarcodeQuietZones()
        /// quietZones.Bottom = 0f;
        /// 'Set the barcode quiet zone.
        /// code93.QuietZone = quietZones
        /// 'Draw a barcode in the new Page.
        /// code93.Draw(page, new PointF(25, 500))
        /// 'Save the document.
        /// document.Save("code93.pdf")
        /// </code>
        /// </example> 
#endif
        public float Bottom
        {
            get
            {
                return m_bottom;
            }

            set
            {
                m_bottom = value;
            }
        }

        /// <summary>
        ///  Gets or sets the quiet zones around the bar code.
        /// </summary>
# if !XAML
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
        /// //Set the barcode quiet zone.
        /// code93.QuietZone = quietZones;
        /// //Draw a barcode in the new Page.
        /// code93.Draw(page, new PointF(25, 500));
        /// //Save document to disk.
        /// document.Save("code93.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Creates a new PdfCode93Barcode.
        /// Dim code39 As PdfCode93Barcode = New PdfCode93Barcode("CODE93")
        /// 'Creates a new PdfBarcodeQuietZones.
        /// PdfBarcodeQuietZones quietZones = new PdfBarcodeQuietZones()
        /// quietZones.All = 0f;
        /// 'Set the barcode quiet zone.
        /// code93.QuietZone = quietZones
        /// 'Draw a barcode in the new Page.
        /// code93.Draw(page, new PointF(25, 500))
        /// 'Save the document.
        /// document.Save("code93.pdf")
        /// </code>
        /// </example> 
#endif
        public float All
        {
            get
            {
                return m_right;
            }

            set
            {
                m_right = m_top = m_left = m_bottom = value;
            }
        }

        /// <summary>
        /// Check whether all the margin values are equal.
        /// </summary>
# if !XAML
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
        /// //Set the barcode quiet zone.
        /// code93.QuietZone = quietZones;
        /// bool isAll=code93.QuietZone.IsAll;
        /// //Draw a barcode in the new Page.
        /// code93.Draw(page, new PointF(25, 500));
        /// //Save document to disk.
        /// document.Save("code93.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Creates a new PdfCode93Barcode.
        /// Dim code39 As PdfCode93Barcode = New PdfCode93Barcode("CODE93")
        /// 'Creates a new PdfBarcodeQuietZones.
        /// PdfBarcodeQuietZones quietZones = new PdfBarcodeQuietZones()
        /// quietZones.All = 0f
        /// code39.QuietZone=quietZones
        /// Dim isAll As bool = code39.QuietZone.IsAll
        /// 'Set the barcode quiet zone.
        /// code93.QuietZone = quietZones
        /// 'Draw a barcode in the new Page.
        /// code93.Draw(page, new PointF(25, 500))
        /// 'Save the document.
        /// document.Save("code93.pdf")
        /// </code>
        /// </example> 
#endif
        public bool IsAll
        {
            get
            {
                return (m_left == m_top && m_left == m_right && m_left == m_bottom);
            }
        }
        #endregion
    }
}
#endif