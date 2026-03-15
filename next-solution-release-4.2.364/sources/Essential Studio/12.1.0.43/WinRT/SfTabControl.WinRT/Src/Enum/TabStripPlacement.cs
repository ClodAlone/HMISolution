// <copyright file="TabStripPlacement.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syncfusion.UI.Xaml.Controls.Navigation
{
    /// <summary>
    /// Defines the different placement options that a <see
    /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabControl"/>  can arrange the
    /// tab strip.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public enum TabStripPlacement
    {
        /// <summary>
        /// A child element that is positioned on the left side of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabControl"/>.
        /// </summary>
        Left = 0,

        /// <summary>
        /// A child element that is positioned on the top of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabControl"/>.
        /// </summary>
        Top = 1,

        /// <summary>
        /// A child element that is positioned on the right side of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabControl"/>.
        /// </summary>
        Right = 2,

        /// <summary>
        /// A child element that is positioned on the bottom of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabControl"/>.
        /// </summary>
        Bottom = 3,

    }

}
