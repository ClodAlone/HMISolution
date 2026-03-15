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
#if !WINDOWS_PHONE_7
using System.Threading.Tasks;
#endif
#if WINDOWS_PHONE||WINDOWS_PHONE_7
namespace Syncfusion.WP.Controls.Layout
#else
namespace Syncfusion.UI.Xaml.Controls.Layout
#endif
{
    /// <summary>
    /// Occurs when current <see
    /// cref="P:Syncfusion.UI.Xaml.Controls.Layout.TileviewItem.State"/> is changed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void StateChangedEventHandler(object sender, StateChangedEventArgs e);
}
