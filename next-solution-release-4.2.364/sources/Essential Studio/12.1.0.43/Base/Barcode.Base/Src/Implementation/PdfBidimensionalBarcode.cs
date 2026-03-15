#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !SILVERLIGHT
using System;
using System.Collections.Generic;
using System.Text;

#if !XAML
using System.Drawing;
#elif BARCODE_WINRT
using Windows.UI.Xaml.Controls;
#else
using System.Windows.Controls;
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
    /// Base class for all two dimensional barcodes.
    /// </summary>
# if !XAML && !GDI
    public abstract class PdfBidimensionalBarcode
#else
    public abstract class BidimensionalBarcode
#endif
    {
        # region Private Members
        /// <summary>
        /// Holds data of barcode.
        /// </summary>
        private string m_text;

        /// <summary>
        /// Holds the quietzone.
        /// </summary>
# if !XAML && !GDI
        private PdfBarcodeQuietZones m_quietZone;
#else
        private BarcodeQuietZones m_quietZone;
#endif

        /// <summary>
        /// Holds the dimension of the barcode.
        /// </summary>
        private float m_xDimension;
        # endregion

        # region Constructor
        /// <summary>
        /// Initializes two dimensional barcode.
        /// </summary>
# if !XAML && !GDI
        public PdfBidimensionalBarcode()
#else
        public BidimensionalBarcode()
#endif
        {
# if !XAML && !GDI
            m_quietZone = new PdfBarcodeQuietZones();
#else
            m_quietZone = new BarcodeQuietZones();
#endif
        }
        # endregion

        # region Properties
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        public String Text
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

        /// <summary>
        /// Gets or sets Quietzone for the barcode.
        /// </summary>
# if !XAML && !GDI
        internal PdfBarcodeQuietZones QuietZone
#else
        internal BarcodeQuietZones QuietZone
#endif
        {
            get
            {
                return m_quietZone;
            }
            set
            {
                m_quietZone = value;
            }
        }

        /// <summary>
        /// Gets or sets the dimension for the barcode.
        /// </summary>
        public float XDimension
        {
            get
            {
                return m_xDimension;
            }
            set
            {
                m_xDimension = value;
            }
        }
        # endregion

        # region Methods
        /// <summary>
        /// Returns the data as byte array.
        /// </summary>
        /// <returns></returns>
        internal byte[] GetData()
        {
#if !XAML && !NETFX_CORE && !WP
            return Encoding.Default.GetBytes(m_text);
#else
            return Encoding.UTF8.GetBytes(m_text);
#endif

        }

# if!XAML && !GDI
        public abstract void Draw(PdfPageBase page, PointF location);
#if !NETFX_CORE && !WP
        public abstract Image ToImage();
#endif
#elif GDI
        public abstract Image Draw(int angle);
#else
        internal abstract void Draw(Canvas m_canvas);
#endif
        # endregion
    }
}
#endif