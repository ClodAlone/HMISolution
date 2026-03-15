// <copyright file="RibbonGalleryFilter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents RibbonGalleryFilter class.
    /// </summary>   
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class RibbonGalleryFilter : DependencyObject
    {
        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonGalleryFilter"/> class.
        /// </summary>
        public RibbonGalleryFilter()
        { 
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the label of the filter.
        /// </summary>
        /// Type: <see cref="string"/>
        /// Text that names the gallery filter.
        public string Label
        {
            get
            {
                return (string)GetValue(LabelProperty);
            }

            set
            {
                SetValue(LabelProperty, value);
            }
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// Defines label of the filter.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(RibbonGalleryFilter), new UIPropertyMetadata(string.Empty));
        #endregion
    }
}
