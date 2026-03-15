#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;

#if !WinRT
using System.Windows.Controls.Primitives;
using Syncfusion.Windows.GridCommon;
namespace Syncfusion.Windows.Controls.Scroll
#else
using Windows.UI.Xaml;

namespace Syncfusion.WinRT.Controls.Scroll
#endif
{
    /// <summary>
    /// An object that provides <see cref="HScrollBar"/>, <see cref="VScrollBar"/> and <see cref="Element"/> properties.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public interface IScrollBarProvider : IScrollableInfo
    {
        /// <summary>
        /// Gets the state describing for the horizontal scroll bar.
        /// </summary>
        /// <value>The horizontal scroll bar state.</value>
        ScrollInfo HScrollBar { get; }

        /// <summary>
        /// Gets the state describing for the vertical scroll bar.
        /// </summary>
        /// <value>The vertical scroll bar state.</value>
        ScrollInfo VScrollBar { get; }

        /// <summary>
        /// Gets the <see cref="FrameworkElement"/>.
        /// </summary>
        /// <value>The element.</value>
        FrameworkElement Element { get; }
    }

}
