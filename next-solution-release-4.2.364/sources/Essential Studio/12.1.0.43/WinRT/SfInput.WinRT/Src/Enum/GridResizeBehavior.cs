#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using Windows.UI.Xaml.Controls;

namespace Syncfusion.UI.Xaml.Controls.Input
{
    /// <summary>
    /// Represents the control that redistributes space between columns or rows
    /// of a Grid control.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    public partial class SfGridSplitter : Control
    {
        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <QualityBand>Mature</QualityBand>
        internal enum GridResizeBehavior
        {
            /// <summary>
            /// Inherited code: Requires comment.
            /// </summary>
            BasedOnAlignment,

            /// <summary>
            /// Inherited code: Requires comment.
            /// </summary>
            CurrentAndNext,

            /// <summary>
            /// Inherited code: Requires comment.
            /// </summary>
            PreviousAndCurrent,

            /// <summary>
            /// Inherited code: Requires comment.
            /// </summary>
            PreviousAndNext
        }
    }
}