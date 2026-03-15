// <copyright file="SelectionStyle.cs" company="Syncfusion">
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
    /// Represents a mode for selecting items
    /// </summary>
    public enum SelectionStyle
    {
        /// <summary>
        /// In this mode, the foreground of the item's header is highlighted
        /// </summary>
        HeaderText,

        /// <summary>
        /// In this mode, the background of the item's header is highlighted
        /// </summary>
        CompleteHeader 
    }

}
