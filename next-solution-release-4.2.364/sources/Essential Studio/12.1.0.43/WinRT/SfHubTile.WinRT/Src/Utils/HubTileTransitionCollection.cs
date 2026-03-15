// <copyright file="HubTileTransitionCollection.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE||WINDOWS_PHONE_7
namespace Syncfusion.WP.Controls.Notification
#else
#if SILVERLIGHT
namespace Syncfusion.Tools.Controls.Notification
#else
#if WPF
namespace Syncfusion.Windows.Controls.Notification
#else
namespace Syncfusion.UI.Xaml.Controls.Notification
#endif
#endif
#endif
{
    /// <summary>
    /// HubTileTransitionCollection is a collection of <see
    /// cref="N:Windows.UI.Xaml.Controls.ContentTransition"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1405:ComVisibleTypeBaseTypesShouldBeComVisible")]
    [ClassReference(IsReviewed = false)]
#if WPF
    [CLSCompliant(false)]
#endif
    public class HubTileTransitionCollection : Collection<ContentTransition>
    {
        
    }
}
