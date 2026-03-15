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
#if WPF
namespace Syncfusion.Windows.Controls.Input
#elif WINDOWS_PHONE||WINDOWS_PHONE_7
namespace Syncfusion.WP.Controls.Input
#elif SILVERLIGHT
namespace Syncfusion.Tools.Controls.Input
#else
namespace Syncfusion.UI.Xaml.Controls.Input
#endif
{
    /// <summary>
    /// Occurs when an event is invoked due to change in the range.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void RangeChangedEventHandler(object sender, RangeChangedEventArgs e);
}
