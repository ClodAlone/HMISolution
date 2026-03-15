#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Interface implemented by tools that want to receive scroll events.
    /// </summary>
    /// <remarks>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Tool"/>
    /// </remarks>
    public interface IScrollEventReceiver
    {
        /// <summary>
        /// Called when a vertical scroll event occurs.
        /// </summary>
        /// <param name="e">Scroll event arguments.</param>
        void VerticalScroll(System.Windows.Forms.ScrollEventArgs e);

        /// <summary>
        /// Called when a horizontal scroll event occurs.
        /// </summary>
        /// <param name="e">Scroll event arguments.</param>
        void HorizontalScroll(System.Windows.Forms.ScrollEventArgs e);
    }
}
