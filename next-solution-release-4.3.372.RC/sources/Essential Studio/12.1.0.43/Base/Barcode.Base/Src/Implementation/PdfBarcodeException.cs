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
using System.Runtime.Serialization;
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
    /// Represents the general barcode exception class.
    /// </summary>
# if !XAML && !NETFX_CORE  && !GDI && !WP
    public class PdfBarcodeException : ApplicationException
#else
    public class BarcodeException : Exception
# endif
    {
        #region Constructors
# if !XAML && !NETFX_CORE  && !GDI && !WP
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfBarcodeException"/> class.
        /// </summary>
        public PdfBarcodeException()
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="BarcodeException"/> class.
        /// </summary>
        public BarcodeException()
#endif
            : base()
        {
        }

# if !XAML && !NETFX_CORE && !GDI && !WP
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfBarcodeException"/> class.
        /// </summary>
        /// <param name="message">User defined error message.</param>
        public PdfBarcodeException(string message)
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="BarcodeException"/> class.
        /// </summary>
        /// <param name="message">User defined error message.</param>
        public BarcodeException(string message)
#endif
            : base(message)
        {
        }

# if !XAML && !NETFX_CORE && !GDI && !WP
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfBarcodeException"/> class.
        /// </summary>
        /// <param name="message">User defined error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public PdfBarcodeException(string message, Exception innerException)
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="BarcodeException"/> class.
        /// </summary>
        /// <param name="message">User defined error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public BarcodeException(string message, Exception innerException)
#endif
            : base(message, innerException)
        {
        }

# if !XAML && !NETFX_CORE && !WP
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfBarcodeException"/> class.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
#if GDI
        protected BarcodeException(SerializationInfo info, StreamingContext context)
#else
        protected PdfBarcodeException(SerializationInfo info, StreamingContext context)
#endif
            : base(info, context)
        {
        }
#endif
        #endregion
    }
}
#endif