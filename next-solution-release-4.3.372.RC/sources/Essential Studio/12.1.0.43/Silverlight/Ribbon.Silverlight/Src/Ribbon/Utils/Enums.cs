#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Tools.Controls
{
    #region Enums

    /// <summary>
    /// Specifies state of the Ribbon.
    /// </summary>
    public enum RibbonState
    {
        /// <summary>
        /// Normal state.
        /// </summary>
        Normal,

        /// <summary>
        /// Hidden state.
        /// </summary>
        Hide,

        /// <summary>
        /// Adorned above the content.
        /// </summary>
        Adorner
    }

    /// <summary>
    /// Specifies available states of Ribbon toolbar
    /// </summary>
    public enum QATState
    {
        /// <summary>
        /// Toolbar is hidden
        /// </summary>
        Hidden,

        /// <summary>
        /// Toolbar is placed above the ribbon tabs
        /// </summary>
        AboveRibbon,

        /// <summary>
        /// Toolbar is placed below ribbon tabs
        /// </summary>
        BelowRibbon
    }

    #endregion
}
