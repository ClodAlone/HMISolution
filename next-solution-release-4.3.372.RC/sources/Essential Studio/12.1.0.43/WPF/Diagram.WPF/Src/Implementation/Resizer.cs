// <copyright file="Resizer.cs" company="Syncfusion">
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
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Represents the Resizer class which enables resizing of the node.
    /// </summary>
#if !SyncfusionFramework3_5
    [DesignTimeVisible(false)]
#endif
    public class Resizer : Control
    {
        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="Resizer"/> class.
        /// </summary>
        static Resizer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Resizer), new FrameworkPropertyMetadata(typeof(Resizer)));
        }

        #endregion
    }
}
