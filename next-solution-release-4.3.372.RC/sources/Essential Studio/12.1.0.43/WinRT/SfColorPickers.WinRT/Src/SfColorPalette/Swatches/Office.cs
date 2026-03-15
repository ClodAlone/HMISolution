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

#if WINDOWS_PHONE||WINDOWS_PHONE_7
namespace Syncfusion.WP.Controls.Media
#elif SILVERLIGHT
namespace Syncfusion.Tools.Controls.Media
#elif WPF
namespace Syncfusion.Windows.Controls.Media
#else
namespace Syncfusion.UI.Xaml.Controls.Media
#endif
{
    /// <summary>
    /// Represents a set of colors with Office theme
    /// </summary>
    public class Office : SwatchesBase
    {
#if!WINRT
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <seealso
        /// cref="N:Syncfusion.UI.Xaml.Controls.Media">Syncfusion.UI.Xaml.Controls.Media
        /// Namespace</seealso>
        [ClassReference(IsReviewed = false)]
#else
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="Syncfusion.UI.Xaml.Controls.Media.Office"/> class.
        /// </summary>
        /// <seealso
        /// cref="N:Syncfusion.UI.Xaml.Controls.Media">Syncfusion.UI.Xaml.Controls.Media
        /// Namespace</seealso>
        [ClassReference(IsReviewed = false)]
#endif
        public Office()
        {
            DefaultStyleKey = typeof(Office);
        }
    }
}
