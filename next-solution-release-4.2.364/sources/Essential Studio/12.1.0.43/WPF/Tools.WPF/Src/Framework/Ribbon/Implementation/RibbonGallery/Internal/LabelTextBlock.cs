// <copyright file="LabelTextBlock.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents LabelTextBlock control.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class LabelTextBlock : TextBlock
    {
        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="LabelTextBlock"/> class.
        /// </summary>
        static LabelTextBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LabelTextBlock), new FrameworkPropertyMetadata(typeof(LabelTextBlock)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LabelTextBlock"/> class.
        /// </summary>
        public LabelTextBlock()
        {
        }        
        #endregion
    }
}
