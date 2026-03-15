// <copyright file="Rotator.cs" company="Syncfusion">
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
    /// Represents the Rotator class which is used to rotate the <see cref="Node"/>.
    /// </summary>
#if !SyncfusionFramework3_5
    [DesignTimeVisible(false)]
#endif
    public class Rotator : Control
    {
        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="Rotator"/> class.
        /// </summary>
        static Rotator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Rotator), new FrameworkPropertyMetadata(typeof(Rotator)));
        }

        #endregion
    }
}
