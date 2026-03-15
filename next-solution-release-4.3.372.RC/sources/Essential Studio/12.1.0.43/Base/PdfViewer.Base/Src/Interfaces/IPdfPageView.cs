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
using Syncfusion.Pdf;

#if WINDOWS
using Syncfusion.Windows;
using System.Drawing;
#elif WPF
using Syncfusion.Windows;
using System.Windows.Media;
#endif

namespace Syncfusion.PdfViewer.Base
{
    interface IPdfPageView
    {
        void Initialize(PdfPageBase page);
        void ZoomTo(float factor);
    }
}

#if WINDOWS
namespace Syncfusion.Windows.Forms.PdfViewer
#elif WPF
namespace Syncfusion.Windows.PdfViewer
#elif MVC
namespace Syncfusion.PdfViewer.Mvc

#endif
{
    /// <summary>
    /// Specifies ZoomMode.
    /// </summary>
    public enum ZoomMode
    {
        /// <summary>
        /// Displays with detault.
        /// </summary>
        Default,
        /// <summary>
        /// Fits the entire page in the viewer.
        /// </summary>
        FitPage,
        /// <summary>
        /// Fits the width of the page in the viewer.
        /// </summary>
        FitWidth
    }
}
