// <copyright file="TabScrollButtonVisibility.cs" company="Syncfusion">
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
    /// Defines the scroll navigation button for <see
    /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabControl"/>.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public enum TabScrollButtonVisibility
    {
        /// <summary>
        /// Scroll Navigation button has been made visible for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabControl"/>.
        /// </summary>
        Visible,

        /// <summary>
        /// Scroll Navigation button has been collapsed for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabControl"/>.
        /// </summary>
        Collapsed,

        /// <summary>
        /// Scroll Navigation button will be made visible only when tab headers exceeds the visible viewport in <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfTabControl"/>.
        /// </summary>
        Auto,
    }

}
